using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GOLDSimpleParserEngine
{
    internal class LookAheadReader
    {
        TextReader source;
        List<char> buffer;
        int position;
        bool eof;

        Location location;

        public LookAheadReader(TextReader source)
        {
            this.source = source;

            location = new Location(1, 1, 1);
            buffer = new List<char>(256);
        }

        public bool Eof
        {
            get { return eof; }
        }

        public Location Location
        {
            get { return location; }
        }

        public int Peek(int pos)
        {
            int value;
            pos += position;

            while (pos >= buffer.Count)
            {
                value = source.Read();
                if (value == -1)
                {
                    eof = true;
                    return value;
                }

                buffer.Add((char)value);
            }

            return buffer[pos];
        }

        public string Read(int length)
        {
            StringBuilder value = new StringBuilder();
            char ch;

            for (; position < buffer.Count && length > 0; position++, length--)
            {
                IncLocation(buffer[position]);
                value.Append(buffer[position]);
            }

            if (length > 0)
            {
                position = 0;
                buffer.Clear();
                int c;

                while (length > 0)
                {
                    c = source.Read();

                    if (c == -1)
                    {
                        eof = true;
                        return value.ToString();
                    }

                    ch = (char)c;

                    IncLocation(ch);
                    value.Append(ch);
                    --length;
                }
            }

            return value.ToString();
        }

        void IncLocation(char c)
        {
            ++location.position;
            if (c == '\n')
            {
                location.column = 1;
                ++location.line;
            }
            else
                ++location.column;
        }

        public void ReadLine()
        {
            int c;

            // ensure there's something in the buffer
            Peek(1);

            ++location.line;

            while (position < buffer.Count)
            {
                ++location.position;

                if (buffer[position] == '\n')
                {
                    ++position;
                    return;
                }

                ++position;
            }

            while ((c = source.Read()) > -1)
            {
                ++location.position;
                if ((char)c == '\n')
                    return;
            }
        }
    }
}
