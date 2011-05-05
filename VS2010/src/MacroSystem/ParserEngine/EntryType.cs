using System;

namespace GOLDSimpleParserEngine
{
    internal enum EntryType : byte
    {
        Empty = 69,
        Integer = 73,
        String = 83,
        Boolean = 66,
        Byte = 98,
        Multi = 77
    };	
}
