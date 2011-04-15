using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace GNet
{
    public partial class ScriptEditor : UserControl
    {
        IScriptRunner scriptRunner;

        public ScriptEditor()
        {
            InitializeComponent();

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
            var h = HighlightingManager.Manager.FindHighlighter("Lua");
            if (h != null)
            {
                editor.SetHighlighting("Lua");
                HighlightingManager.Manager.AddHighlightingStrategy(h);
            }

            editor.Text =
@"-- Name:
-- Description:
-- Executables:

function OnEvent(event, arg, family)
    OutputLogMessage(event.."" : ""..arg.."" : ""..family)
end
";
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            runButton.Enabled = false;
            stopButton.Enabled = true;

            try
            {
                scriptRunner = new LuaRunner(editor.Text).Run();
            }
            catch (Exception ex)
            {
                StopScript();
                Console.WriteLine(ex.ToString());
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopScript();
        }

        void StopScript()
        {
            if (scriptRunner != null)
            {
                scriptRunner.Stop();
                scriptRunner = null;
            }

            runButton.Enabled = true;
            stopButton.Enabled = false;
        }
    }
}
