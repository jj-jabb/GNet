using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;

namespace GNet
{
    public class TextBoxStreamWriter : TextWriter
    {
        delegate void InvokeAction(char value);
        public static TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            if (_output.Dispatcher.Thread != Thread.CurrentThread)
                _output.Dispatcher.BeginInvoke(new InvokeAction(InvokeWrite), value);
            else
                InvokeWrite(value);
        }

        void InvokeWrite(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
