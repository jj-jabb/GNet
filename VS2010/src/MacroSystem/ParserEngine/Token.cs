using System;

namespace GOLDSimpleParserEngine
{
    public partial class Token
    {
        internal int state;
        internal string text;
        internal Symbol parent;
        internal Reduction reduction;
        internal Location location;
        object data;

        Token[] tokens;

        internal Token()
        {
            state = -1;
        }

        internal Token(int state, Symbol parent, string data)
        {
            this.state = state;
            this.parent = parent;
            this.text = data;
        }

        internal Token(int state, Symbol parent)
            : this(state, parent, null)
        { }

        internal Token(Symbol parent, string data)
            : this(-1, parent, data)
        { }

        internal Token(Symbol parent, char data)
            : this(parent, new string(data, 1))
        { }

        internal Token(Symbol parent, int data)
            : this(parent, (char)data)
        { }

        internal Token(Symbol parent, char[] data)
            : this(parent, new string(data))
        { }

        internal Token(Symbol parent)
            : this(parent, (string)null)
        { }

        internal Token(Symbol parent, Location location)
            : this(parent, (string)null)
        { this.location = location; }

        internal Token(Symbol parent, LookAheadReader reader, int lenth)
        {
            state = -1;
            this.parent = parent;
            location = reader.Location;
            text = reader.Read(lenth);
        }

        internal bool IsCommentToken
        {
            get
            {
                SymbolType kind = Type;

                return
                    kind == SymbolType.CommentLine ||
                    kind == SymbolType.CommentStart
                    ;
            }
        }

        public SymbolType Type
        {
            get { return parent == null ? SymbolType.Undefined : parent.type; }
        }

        public short TableIndex
        {
            get { return parent == null ? (short)-1 : parent.index; }
        }

        public string Name
        {
            get { return parent == null ? null : parent.name; }
        }

        public Token[] Tokens
        {
            get { return tokens ?? (tokens = reduction == null ? null : reduction.tokens.ToArray()); }
        }

        public Rule Rule
        {
            get { return reduction == null ? null : reduction.Rule; }
        }

        public Symbol Symbol
        {
            get { return parent; }
        }

        public string Text
        {
            get { return text; }
        }

        public Location Location
        {
            get { return location; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
