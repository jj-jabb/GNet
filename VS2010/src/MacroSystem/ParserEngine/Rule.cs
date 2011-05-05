using System;

namespace GOLDSimpleParserEngine
{
    public class Rule
    {
        internal int index;
        internal Symbol head;
        internal Symbol[] symbolList;

        internal Rule(int index, GrammarReader reader, int entryCount, TableCounts tableCounts)
        {
            this.index = index;
            head = tableCounts.symbols[reader.ReadIntegerEntry()];

            // reserved
            reader.ReadByte();

            symbolList = new Symbol[entryCount - 4];

            for (int i = 0; i < symbolList.Length; i++)
                symbolList[i] = tableCounts.symbols[reader.ReadIntegerEntry()];
        }

        public int Id { get { return index; } }
        public Symbol Head { get { return head; } }
        public int SymbolCount { get { return symbolList.Length; } }
        public Symbol this[int index] { get { return symbolList[index]; } }
    }
}
