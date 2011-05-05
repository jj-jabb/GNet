using System;

namespace GOLDSimpleParserEngine
{
    public class Symbol
    {
        static public Symbol Error;
        static public Symbol End;

        //static Symbol()
        //{
        //    Error = new Symbol(SymbolType.Error);
        //    End = new Symbol(SymbolType.End);
        //}

        internal short index;
        internal string name;
        internal SymbolType type;

        internal Symbol(SymbolType kind)
        {
            this.type = kind;

            if (kind == SymbolType.Error)
                Error = this;
            else if (kind == SymbolType.End)
                End = this;
        }

        internal Symbol(short index, GrammarReader reader)
        {
            this.index = index;

            name = reader.ReadStringEntry();
            type = (SymbolType)reader.ReadIntegerEntry();

            if (type == SymbolType.Error)
                Error = this;
            else if (type == SymbolType.End)
                End = this;
        }

        public int Id { get { return index; } }
        public string Name { get { return name; } }
        public SymbolType Type { get { return type; } }
    }
}
