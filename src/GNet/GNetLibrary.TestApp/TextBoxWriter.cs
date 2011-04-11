using System;
using System.IO;
using System.Text;
using System.Windows.Forms; 

namespace GNetLibrary.TestApp
{
    // http://remark.wordpress.com/articles/embedding-ironpython-as-a-scripting-language/
    public class TextBoxWriter : TextWriter
    {
        private readonly Encoding encoding;
        private readonly RichTextBox textBox;

        public TextBoxWriter(RichTextBox textBox)
            : this(textBox, Encoding.UTF8)
        {
        }

        public TextBoxWriter(RichTextBox textBox, Encoding encoding)
        {
            this.textBox = textBox;
            this.encoding = encoding;
        }

        public override void WriteLine(string value)
        {
            this.textBox.AppendText(value);
        }

        public override void Write(char value)
        {
            this.textBox.AppendText(value.ToString());
        }

        public override void Write(string value)
        {
            this.textBox.AppendText(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.textBox.AppendText(new string(buffer));
        }

        public override Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }
    }
}
