using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GOLDSimpleParserEngine;

namespace GOLDSimpleParserEngine
{
    public partial class MacroParser
    {
        private LALRParser parser;
        private List<string> errors;
        
        public MacroParser()
        {
            errors = new List<string>();
            parser = new LALRParser(CGTResourceName);

            parser.CommentEndError += new EventHandler(OnCommentEndError);
            parser.InternalError += new EventHandler(OnInternalError);
            parser.SyntaxError += new EventHandler(OnSyntaxError);
        }
        
        public List<string> Errors
        {
            get
            {
                return errors;
            }
        }

        private void OnCommentEndError(object sender, EventArgs e)
        {
            errors.Add("End-of-file reached due to unterminated comment.");
        }

        private void OnInternalError(object sender, EventArgs e)
        {
            errors.Add("An internal error occured.");
        }

        private void OnSyntaxError(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            
            int tokenCount = 0;
            Location location = new Location();
            
            foreach(Token token in parser.ExpectedTokens)
            {
                if(tokenCount > 0)
                    sb.Append(", ");
                else
                    location = token.Location;
                
                ++tokenCount;
                
                sb.Append(token.Name);
            }
            
            sb2.Append("Syntax error at line ").Append(location.Line).Append(", column ").Append(location.Column);
            
            if(tokenCount == 1)
                sb2.Append("; expecting ").Append(sb);
            else
                sb2.Append("; expecting one of the following: ").Append(sb);
        }

        public object Parse(string source)
        {
            return CreateObject(parser.Parse(source));
        }

        public object Parse(TextReader source)
        {
            return CreateObject(parser.Parse(source));
        }

        private object CreateObject(Token token)
        {
            switch(token.Type)
            {
                case SymbolType.Terminal:
                    return CreateObjectFromTerminal(token);

                case SymbolType.NonTerminal:
                    return CreateObjectFromNonterminal(token);
                
                default:
                    errors.Add("Unexpected token type: " + token.Type.ToString());
                    return null;
            }
        }

        public class SymbolException : System.Exception
        {
            public SymbolException(string message)
                : base(message)
            {
            }
        }

        public class RuleException : System.Exception
        {

            public RuleException(string message)
                : base(message)
            {
            }
        }
    }
}
