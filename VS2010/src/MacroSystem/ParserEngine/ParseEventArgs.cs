using System;
namespace GOLDSimpleParserEngine
{
    public class ParseEventArgs<T> : EventArgs
    {
        T data;

        public ParseEventArgs(T data)
        {
            this.data = data;
        }

        public T Data { get { return data; } }
    }
}
