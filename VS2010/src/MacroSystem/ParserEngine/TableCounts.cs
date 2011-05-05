using System;

namespace GOLDSimpleParserEngine
{
    internal class TableCounts
    {
        public Symbol[] symbols;
        public string[] charsets;
        public Rule[] rules;
        public DFAState[] states;
        public LRActionTable[] actions;

        public TableCounts(GrammarReader reader)
        {
            symbols = new Symbol[reader.ReadIntegerEntry()];
            charsets = new string[reader.ReadIntegerEntry()];
            rules = new Rule[reader.ReadIntegerEntry()];
            states = new DFAState[reader.ReadIntegerEntry()];
            actions = new LRActionTable[reader.ReadIntegerEntry()];
        }
    }
}
