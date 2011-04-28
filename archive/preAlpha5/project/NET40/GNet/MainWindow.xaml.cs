using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using ICSharpCode.AvalonEdit;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Security.Policy;
using System.Security;
using System.Security.Permissions;
using System.IO;

namespace GNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBoxStreamWriter tbsw;

        public MainWindow()
        {
            InitializeComponent();
            //SetupCommandBindings();

            //documents = new ObservableCollection<TabItem>();
            //documentTabs.ItemsSource = documents;
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var doctab = new DocumentTab("Test");
            documentTabs.Items.Add(doctab);
            documentTabs.SelectedItem = doctab;
            doctab.Focus();
        }

        private void CommandBinding_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbsw = new TextBoxStreamWriter(output);
            Console.SetOut(tbsw);
        }
    }

    public class DocumentTab : TabItem
    {
        public string FileName { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public ScriptEditor Editor { get; set; }

        public DocumentTab(string name)
        {
            FileName = name + ".boo";
            DocumentName = name;
            Header = name ;
            Editor = new ScriptEditor();
            Content = Editor;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ScriptRunnerSAMPLE : MarshalByRefObject
    // make class remotable
    {
        ICodeCompiler compiler;
        CompilerParameters param;
        
        public static ScriptRunnerSAMPLE CreateInSeparateDomain()
        {
            AppDomain dom = AppDomain.CreateDomain("sandbox");
            return dom.CreateInstanceAndUnwrap(typeof(ScriptRunnerSAMPLE).Assembly.GetName().Name, typeof(ScriptRunnerSAMPLE).FullName) as ScriptRunnerSAMPLE;
        }

        public ScriptRunnerSAMPLE()
        {
            string parameters = "";
            // C# compiler
            compiler = new CSharpCodeProvider().CreateCompiler();
            // parameters = "/debug+"; // uncomment for debugging	
            // uncomment for JScript.NET	
            //compiler = new JScriptCodeProvider().CreateCompiler();	
            //parameters ="/debug+ /versionsafe+ ";	
            param = new CompilerParameters();
            param.CompilerOptions = parameters;
            param.GenerateExecutable = false; 
            param.GenerateInMemory = true;
            param.IncludeDebugInformation = true;
            param.ReferencedAssemblies.Add("Scripting.dll");
            // whatever you need here	
            // set policy		
            PolicyLevel level = PolicyLevel.CreateAppDomainLevel(); 
            PermissionSet permissions = new PermissionSet(PermissionState.None);
            // uncomment all permissions you need	
            // (never allow "Assertion"...)	
            // which flags are required minimally also depends	
            // on .NET runtime Version	
            SecurityPermissionFlag permissionFlags =
                //                SecurityPermissionFlag.Assertion |	
                //                SecurityPermissionFlag.BindingRedirects |
                //                SecurityPermissionFlag.ControlAppDomain |	
                //                SecurityPermissionFlag.ControlDomainPolicy |	
                //                SecurityPermissionFlag.ControlEvidence |	
                //                SecurityPermissionFlag.ControlPolicy |	
                //                SecurityPermissionFlag.ControlThread |
                //                SecurityPermissionFlag.ControlPrincipal |	
                //                SecurityPermissionFlag.Infrastructure |	
                //                SecurityPermissionFlag.RemotingConfiguration |
                //                SecurityPermissionFlag.SerializationFormatter |
                //                SecurityPermissionFlag.Infrastructure |		
                SecurityPermissionFlag.SkipVerification |
                SecurityPermissionFlag.UnmanagedCode |
                SecurityPermissionFlag.Execution;

            permissions.AddPermission(new SecurityPermission(permissionFlags));
            // allow reflection		
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.AllFlags));
            PolicyStatement policy = new PolicyStatement(permissions, PolicyStatementAttribute.Exclusive);
            CodeGroup group = new UnionCodeGroup(new AllMembershipCondition(), policy);
            level.RootCodeGroup = group;
            AppDomain.CurrentDomain.SetAppDomainPolicy(level);
        }
    }
    // add code for compiling and running scripts}
}

