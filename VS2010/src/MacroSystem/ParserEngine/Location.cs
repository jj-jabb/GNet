using System;

namespace GOLDSimpleParserEngine
{
    public struct Location
    {
        internal int position;
        internal int line;
        internal int column;

        internal Location(int position, int line, int column)
        {
            this.position = position;
            this.line = line;
            this.column = column;
        }

        public int Position { get { return position; } }
        public int Line { get { return line; } }
        public int Column { get { return column; } }
    }
}
