using System;

namespace GOLDSimpleParserEngine
{
    public enum ActionType
    {
        Shift = 1,
        Reduce = 2,
        Goto = 3,
        Accept = 4,
        Error = 5
    }
}
