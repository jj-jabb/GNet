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
        public ScriptEditor()
        {
            InitializeComponent();
            editor.Text =
@"# Name:
# Description:
# Executables:
";
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            new BooRunner().Run("Test", editor.Text);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
