using System;
using System.Text;

namespace HidLibrary
{
    public class EventArgs<T> : EventArgs
    {
        T _data;

        public EventArgs(T data)
        {
            _data = data;
        }

        public T Data { get { return _data; } }

        public override string ToString()
        {
            if (_data == null)
                return "<NO DATA>";

            return _data.ToString();
        }
    }
}
