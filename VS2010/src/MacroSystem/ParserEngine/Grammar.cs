using System;

namespace GOLDSimpleParserEngine
{
    internal class Grammar
    {
        public Parameters parameters;
        public TableCounts tableCounts;
        public InitialState initialState;

        public short InitialDfaStateIndex
        {
            get { return initialState.dfaState; }
        }

        public DFAState InitialDfaState
        {
            get { return tableCounts.states[initialState.dfaState]; }
        }

        public short InitialLalrStateIndex
        {
            get { return initialState.lalrState; }
        }

        public Symbol StartSymbol
        {
            get { return tableCounts.symbols[parameters.startSymbol]; }
        }

        public bool CaseSensitive
        {
            get { return parameters.caseSensitive; }
        }

        public DFAState DFAState(int index)
        {
            return tableCounts.states[index];
        }

        public Symbol Symbol(int index)
        {
            return tableCounts.symbols[index];
        }

        public LRActionTable LRActionTable(int index)
        {
            return tableCounts.actions[index];
        }

        public Rule Rule(int index)
        {
            return tableCounts.rules[index];
        }
    }
}
