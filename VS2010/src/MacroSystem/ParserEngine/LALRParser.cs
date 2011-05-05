using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GOLDSimpleParserEngine
{
    public class LALRParser
    {
        Grammar grammer;

        LookAheadReader source;

        Stack<Token> inputs;
        Stack<Token> expectedTokens;
        Stack<Token> results;

        int line;
        int column;
        int position;

        int commentLevel;

        int currentLalrState;

        public LALRParser(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new ArgumentException("Resource '" + resourceName + "' was not found - make sure the CGT file has a Build Action of Embedded Resource.");

            GrammarReader reader = new GrammarReader(stream);
            grammer = reader.ReadGrammar();
            reader.Close();
        }

        public IEnumerable<Token> Inputs
        {
            get { return inputs; }
        }

        public IEnumerable<Token> ExpectedTokens
        {
            get { return expectedTokens; }
        }

        public IEnumerable<Token> Results
        {
            get { return results; }
        }

        public Token Parse(string source)
        {
            return Parse(new StringReader(source));
        }

        public Token Parse(TextReader source)
        {
            this.source = new LookAheadReader(source);

            inputs = new Stack<Token>();
            expectedTokens = new Stack<Token>();
            results = new Stack<Token>();

            line = 1;
            column = 1;
            position = 1;

            currentLalrState = grammer.InitialLalrStateIndex;

            Token token = new Token(currentLalrState, grammer.StartSymbol);
            results.Push(token);

            ParseLoop();

            return results.Peek();
        }

        protected virtual void ParseLoop()
        {
            bool continueParsing = true;

            while (continueParsing)
            {
                switch (Parse())
                {
                    case ParseMessage.InternalError:
                    case ParseMessage.SyntaxError:
                    case ParseMessage.CommentError:
                    case ParseMessage.Accept:
                        continueParsing = false;
                        break;
                }
            }
        }

        // public bool TrimReductions { get { return trimReductions; } }

        public event EventHandler<ParseEventArgs<Token>> TokenRead;
        public event EventHandler CommentEndError;
        public event EventHandler<ParseEventArgs<Token>> Accept;
        public event EventHandler<ParseEventArgs<Token>> Shift;
        public event EventHandler<ParseEventArgs<Reduction>> Reduced;
        public event EventHandler InternalError;
        public event EventHandler SyntaxError;

        ParseMessage Parse()
        {
            Token token;

            while (true)
            {
                if (inputs.Count == 0)
                {
                    token = NextToken();

                    if (token.Type != SymbolType.Whitespace)
                    {
                        inputs.Push(token);
                        if (commentLevel == 0 && !token.IsCommentToken)
                        {
                            if (TokenRead != null) TokenRead(this, new ParseEventArgs<Token>(token));

                            return ParseMessage.TokenRead;
                        }
                    }
                }
                else if (commentLevel > 0)
                {
                    token = inputs.Pop();

                    switch (token.Type)
                    {
                        case SymbolType.CommentStart:
                            ++commentLevel;
                            break;
                        case SymbolType.CommentEnd:
                            --commentLevel;
                            break;
                        case SymbolType.End:
                            if (CommentEndError != null) CommentEndError(this, EventArgs.Empty);
                            return ParseMessage.CommentError;
                    }
                }
                else
                {
                    token = inputs.Peek();
                    switch (token.Type)
                    {
                        case SymbolType.CommentStart:
                            inputs.Pop();
                            ++commentLevel;
                            break;

                        case SymbolType.CommentLine:
                            inputs.Pop();
                            source.ReadLine();
                            ++line;
                            break;

                        default:
                            switch (ParseToken(token))
                            {
                                case ParseResult.Accept:
                                    return ParseMessage.Accept;

                                case ParseResult.InternalError:
                                    if (InternalError != null) InternalError(this, EventArgs.Empty);
                                    return ParseMessage.InternalError;

                                case ParseResult.ReduceNormal:
                                    return ParseMessage.Reduction;

                                case ParseResult.Shift:
                                    inputs.Pop();
                                    break;

                                case ParseResult.SyntaxError:
                                    if (SyntaxError != null) SyntaxError(this, EventArgs.Empty);
                                    return ParseMessage.SyntaxError;
                            }
                            break;
                    }
                }
            }
        }

        Token NextToken()
        {
            Token result = null;
            int currentPos = 0;
            int lastAcceptState = -1;
            int lastAcceptPos = -1;
            int lastPosition = 0;
            int lastLine = 0;
            int lastColumn = 0;
            DFAState currentState = grammer.InitialDfaState;
            int ich;
            char ch = '\0';
            DFAEdge edge;

            while (true)
            {
                edge = null;

                if ((ich = source.Peek(currentPos)) > -1)
                {
                    ch = grammer.CaseSensitive ? (char)ich : Char.ToLower((char)ich);
                    edge = currentState[ch];
                }

                if (edge != null)
                {
                    if (grammer.DFAState(edge.targetIndex).acceptSymbolIndex > -1)
                    {
                        lastAcceptState = edge.targetIndex;
                        lastAcceptPos = currentPos;
                        lastPosition = position;
                        lastLine = line;
                        lastColumn = column;
                    }

                    currentState = grammer.DFAState(edge.targetIndex);
                    currentPos++;

                    ++position;
                    if (ch == '\n')
                    {
                        ++line;
                        column = 1;
                    }
                    else
                        ++column;
                }
                else
                {
                    if (lastAcceptState == -1)
                    {
                        if (source.Eof)
                            result = new Token(Symbol.End);
                        else
                            result = new Token(Symbol.Error, source, 1);
                    }
                    else
                    {
                        Symbol symbol = grammer.Symbol(grammer.DFAState(lastAcceptState).acceptSymbolIndex);
                        result = new Token(symbol, source, lastAcceptPos + 1);
                    }

                    break;
                }
            }

            return result;
        }

        ParseResult ParseToken(Token token)
        {   
            ParseResult result = ParseResult.InternalError;

            LRActionTable table = grammer.LRActionTable(currentLalrState);
            LRAction action = table[token.TableIndex];

            if (action != null)
            {
                expectedTokens.Clear();

                switch (action.action)
                {
                    case ActionType.Accept:
                        if (Accept != null) Accept(this, new ParseEventArgs<Token>(token));
                        result = ParseResult.Accept;
                        break;
                    case ActionType.Shift:
                        token.state = currentLalrState = action.value;
                        if (Shift != null) Shift(this, new ParseEventArgs<Token>(token));
                        results.Push(token);
                        result = ParseResult.Shift;
                        break;
                    case ActionType.Reduce:
                        result = Reduce(grammer.Rule(action.value));
                        break;
                }
            }
            else
            {
                // syntax error - fill expected tokens.				
                expectedTokens.Clear();
                foreach (LRAction a in table.actions)
                {
                    SymbolType kind = a.symbol.type;

                    if (kind == SymbolType.Terminal || kind == SymbolType.End)
                    {
                        if(inputs.Count > 0)
                            expectedTokens.Push(new Token(a.symbol, inputs.Peek().location));
                        else
                            expectedTokens.Push(new Token(a.symbol, source.Location));
                    }
                }

                result = ParseResult.SyntaxError;
            }

            return result;
        }

        ParseResult Reduce(Rule rule)
        {
            ParseResult result;
            Token head;

            Reduction reduction = new Reduction(rule);
            Stack<Token> poppedTokens = new Stack<Token>(rule.symbolList.Length);

            for (int i = 0; i < rule.symbolList.Length; i++)
                poppedTokens.Push(results.Pop());

            while (poppedTokens.Count > 0)
                reduction.tokens.Add(poppedTokens.Pop());

            head = new Token(rule.head);
            head.reduction = reduction;

            result = ParseResult.ReduceNormal;

            LRAction action = grammer.LRActionTable(results.Peek().state)[rule.head.index];

            if (action != null)
            {
                head.state = currentLalrState = action.value;
                results.Push(head);
            }
            else
                throw new ArgumentException("Action for LALR state is null");

            if (Reduced != null) Reduced(this, new ParseEventArgs<Reduction>(reduction));

            return result;
        }
    }
}