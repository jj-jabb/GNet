using System;
using System.Collections.Generic;

namespace GOLDSimpleParserEngine
{
    public class Reduction
    {
        internal List<Token> tokens;
        internal Rule parentRule;

        internal Reduction(Rule parentRule)
        {
            this.parentRule = parentRule;

            tokens = new List<Token>();
        }

        public Rule Rule { get { return parentRule; } }
        public int TokenCount { get { return tokens.Count; } }
        public Token this[int index] { get { return tokens[index]; } }
    }
}
