using System;
using System.IO;
using System.Text;

namespace GOLDSimpleParserEngine
{
    internal class GrammarReader : BinaryReader
    {
        const String fileType = "GOLD Parser Tables/v1.0";

        public GrammarReader(Stream stream)
            : base(stream)
        {
            if (ReadString() != fileType)
                throw new ArgumentException("File header is invalid - expecting " + fileType);
        }

        public override string ReadString()
        {
            StringBuilder builder = new StringBuilder();

            short c;

            while((c = ReadInt16()) != 0)
                builder.Append((char)c);

            return builder.ToString();
        }

        public Grammar ReadGrammar()
        {
            Grammar grammar = new Grammar();
            EntryType entryType;
            RecordType recordType;

            short index;

            short entryCount;

            while (BaseStream.Position < BaseStream.Length && (entryType = (EntryType)ReadByte()) == EntryType.Multi)
            {
                entryCount = ReadInt16();
                recordType = (RecordType)ReadByteEntry();

                switch (recordType)
                {
                    case RecordType.Parameters:
                        grammar.parameters = new Parameters(this);
                        break;

                    case RecordType.TableCounts:
                        grammar.tableCounts = new TableCounts(this);
                        break;

                    case RecordType.Initial:
                        grammar.initialState = new InitialState(this);
                        break;

                    case RecordType.CharSets:
                        index = ReadIntegerEntry();
                        grammar.tableCounts.charsets[index] = ReadStringEntry();
                        break;

                    case RecordType.Symbols:
                        index = ReadIntegerEntry();
                        grammar.tableCounts.symbols[index] = new Symbol(index, this);
                        break;

                    case RecordType.Rules:
                        index = ReadIntegerEntry();
                        grammar.tableCounts.rules[index] = new Rule(index, this, entryCount, grammar.tableCounts);
                        break;

                    case RecordType.DFAStates:
                        index = ReadIntegerEntry();
                        grammar.tableCounts.states[index] = new DFAState(this, entryCount, grammar.tableCounts);
                        break;

                    case RecordType.LRTables:
                        index = ReadIntegerEntry();
                        grammar.tableCounts.actions[index] = new LRActionTable(this, entryCount, grammar.tableCounts);
                        break;

                    case RecordType.Comment:
                        break;

                    default:
                        throw new IOException("Unknown record type: " + (int)recordType);
                }
            }

            return grammar;
        }

        public bool ReadBooleanEntry()
        {
            ReadByte();
            return ReadByte() == 1;
        }

        public byte ReadByteEntry()
        {
            ReadByte();
            return ReadByte();
        }

        public short ReadIntegerEntry()
        {
            ReadByte();
            return ReadInt16();
        }

        public string ReadStringEntry()
        {
            ReadByte();
            return ReadString();
        }
    }
}
