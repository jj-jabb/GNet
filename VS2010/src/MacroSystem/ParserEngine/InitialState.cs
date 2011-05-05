using System;

namespace GOLDSimpleParserEngine
{
    internal class InitialState
    {
        public short dfaState;
        public short lalrState;

        public InitialState(GrammarReader reader)
        {
            dfaState = reader.ReadIntegerEntry();
            lalrState = reader.ReadIntegerEntry();
        }
    }
}
