using System;
using System.Collections.Generic;
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
using System.IO;

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
            editor.Text =
@"-- Name:
-- Description:
-- Executables:

function OnEvent(event, arg, family)
    OutputLogMessage(event.."" : ""..arg.."" : ""..family)
end
";
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            runButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            switch(editor.SyntaxHighlighting.Name)
            {
                case "Boo":
                    scriptRunner = new BooRunner("Test", editor.Text).Run();
                    break;

                case "Lua":
                    scriptRunner = new LuaRunner(editor.Text).Run();
                    break;
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
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
