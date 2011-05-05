using System;

namespace GOLDSimpleParserEngine
{
    internal enum RecordType
    {
        Parameters = 80,
        TableCounts = 84,
        Initial = 73,
        Symbols = 83,
        CharSets = 67,
        Rules = 82,
        DFAStates = 68,
        LRTables = 76,
        Comment = 33
    }
}
