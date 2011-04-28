using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GNet
{
    /// <summary>
    /// Interaction logic for ScriptEditor.xaml
    /// </summary>
    public partial class ScriptEditor : UserControl
    {
        IScriptRunner scriptRunner;

        public ScriptEditor()
        {
            InitializeComponent();

            // Boo
//            editor.Text =
//@"# Name:
//# Description:
//# Executables:
//
//import GNet.Lib
//
//static def Run(d as G13Device):
//	d.KeyPressed += def(key):
//		print key as uint
//";
            // Lua

            if (File.Exists(@".\Profiles\Lua\_default.lua"))
            {
                using (var fs = File.OpenText(@".\Profiles\Lua\_default.lua"))
                {
                    editor.Text = fs.ReadToEnd();
                }
            }
            else
            {
                editor.Text =
@"-- Name:
-- Description:
-- Executables:

function OnEvent(event, arg, family)
    OutputLogMessage(event.."" : ""..arg.."" : ""..family)
end
";
            }
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            runButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            try
            {
                switch (editor.SyntaxHighlighting.Name)
                {
                    case "Boo":
                        scriptRunner = new BooRunner("Test", editor.Text).Run();
                        break;

                    case "Lua":
                        scriptRunner = new LuaRunner(editor.Text).Run();
                        break;
                }
            }
            catch (Exception ex)
            {
                StopScript();
                Console.WriteLine(ex.ToString());
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
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

            runButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }
    }
}
