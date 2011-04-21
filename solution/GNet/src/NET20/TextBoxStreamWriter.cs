using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
            if (_output != null)
            {
                if (_output.InvokeRequired)
                    _output.Invoke(new InvokeAction(InvokeWrite), value);
                else
                    InvokeWrite(value);
            }
        }

        void InvokeWrite(char value)
        {
            System.Diagnostics.Debug.Write(value);
            base.Write(value);
            if (_output != null && !_output.IsDisposed)
                _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
