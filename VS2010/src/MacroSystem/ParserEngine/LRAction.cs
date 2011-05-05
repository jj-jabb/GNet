using System;

namespace GOLDSimpleParserEngine
{
    internal class LRAction
    {
        public Symbol symbol;
        public ActionType action;
        public short value;

        internal LRAction(GrammarReader reader, TableCounts tableCounts)
        {
            symbol = tableCounts.symbols[reader.ReadIntegerEntry()];
            action = (ActionType)reader.ReadIntegerEntry();
            value = reader.ReadIntegerEntry();

            // reserved
            reader.ReadByte();
        }
    }
}
