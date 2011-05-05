using System;

namespace GOLDSimpleParserEngine
{
    public enum ParseResult
    {
        Accept = 301,
        Shift = 302,
        ReduceNormal = 303,
        ReduceEliminated = 304,
        SyntaxError = 305,
        InternalError = 406
    }
}
