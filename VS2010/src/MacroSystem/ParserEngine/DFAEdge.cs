using System;

namespace GOLDSimpleParserEngine
{
    internal class DFAEdge
    {
        public string characters;
        public short targetIndex;

        public DFAEdge(GrammarReader reader, TableCounts tableCounts)
        {
            characters = tableCounts.charsets[reader.ReadIntegerEntry()];
            targetIndex = reader.ReadIntegerEntry();

            // reserved
            reader.ReadByte();
        }
    }
}
