using System;

namespace GOLDSimpleParserEngine
{
    public class Parameters
    {
        internal string name;
        internal string version;
        internal string author;
        internal string about;

        internal bool caseSensitive;

        internal short startSymbol;

        internal Parameters(GrammarReader reader)
        {
            name = reader.ReadStringEntry();
            version = reader.ReadStringEntry();
            author = reader.ReadStringEntry();
            about = reader.ReadStringEntry();

            caseSensitive = reader.ReadBooleanEntry();

            startSymbol = reader.ReadIntegerEntry();
        }

        public string Name { get { return name; } }
        public string Version { get { return version; } }
        public string Author { get { return author; } }
        public string About { get { return about; } }
        public bool CaseSensitive { get { return caseSensitive; } }
    }
}
