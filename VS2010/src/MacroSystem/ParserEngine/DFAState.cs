using System;
using System.Collections.Generic;

namespace GOLDSimpleParserEngine
{
    internal class DFAState
    {
        public short acceptSymbolIndex;
        public DFAEdge[] edges;
        public Dictionary<char, DFAEdge> edgeLookup;

        public DFAState(GrammarReader reader, int entryCount, TableCounts tableCounts)
        {
            DFAEdge edge;

            edgeLookup = new Dictionary<char, DFAEdge>();

            // read accept state 
            reader.ReadBooleanEntry();
            acceptSymbolIndex = (short)reader.ReadIntegerEntry(); // already returns -1 if not accept state

            // reserved
            reader.ReadByte();

            edges = new DFAEdge[(entryCount - 5)/3];

            for (int i = 0; i < edges.Length; i++)
            {
                edge = new DFAEdge(reader, tableCounts);
                edges[i] = edge;
                foreach (char c in edge.characters)
                    edgeLookup[c] = edge;
            }
        }

        public DFAEdge this[char ch]
        {
            get
            {
                if (edgeLookup.ContainsKey(ch))
                    return edgeLookup[ch];

                return null;
            }
        }
    }
}
