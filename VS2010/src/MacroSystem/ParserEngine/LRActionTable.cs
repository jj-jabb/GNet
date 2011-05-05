using System;
using System.Collections.Generic;

namespace GOLDSimpleParserEngine
{
    internal class LRActionTable
    {
        public LRAction[] actions;
        public Dictionary<short, LRAction> actionLookup;

        public LRActionTable(GrammarReader reader, int entryCount, TableCounts tableCounts)
        {
            // reserved
            reader.ReadByte();

            actions = new LRAction[(entryCount - 3) / 4];
            actionLookup = new Dictionary<short, LRAction>();

            LRAction action;

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = action = new LRAction(reader, tableCounts);
                actionLookup[action.symbol.index] = action;
            }
        }

        public LRAction this[short index]
        {
            get
            {
                if (actionLookup.ContainsKey(index))
                    return actionLookup[index];

                return null;
            }
        }
    }
}
