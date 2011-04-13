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

namespace GNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupCommandBindings();

        }

        private void SetupCommandBindings()
        {
            var binding = new CommandBinding(ApplicationCommands.New);
            binding.Executed += (s, e) =>
            {

            };
            binding.CanExecute += (s, e) =>
            {
                e.CanExecute = true;
            };
            CommandBindings.Add(binding);
        }
    }
}
