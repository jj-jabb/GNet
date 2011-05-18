using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;

using GNet.Profiler;
using GNet.Profiler.MacroSystem;

namespace GNet.UI
{
    public partial class MainForm : Form
    {
        ProfileListForm profileListForm = new ProfileListForm();

        public MainForm()
        {
            string path = "/Test/SubTest/Sub/";
            var list = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            InitializeComponent();

            //Macro macro = new Macro(
            //    new Delay(2345)//,
            //    //new KeyDown(PInvoke.ScanCode.x)
            //    );

            Profile profile = new Profile
            {
                Name = "Test",
                Script = @"
function blah<T>(T t) {
    var a = t[[0]] >10;
    return ""a"" > ""b && ""c"" < ""d"";
    
}
",
                Macros = new List<Macro>
                {
                    new Macro
                    {
                        Name = "TestMacro",
                        Steps = new List<Step>
                        {
                            new Delay(12345),
                            new KeyCharDown('a'),
                            new KeyScanCodeTap(PInvoke.ScanCode.semicolon)
                        }
                    }
                },

                InputAssignments = new List<InputAssignment>
                {
                    new InputAssignment
                    {
                        MinJoystickAngle = 0,
                        MaxJoystickAngle = 90
                    }
                }
            };

            var ser = new XmlSerializer(typeof(Profile));
            var sb = new StringBuilder();
            using (var sr = new StringWriter(sb))
                ser.Serialize(sr, profile);
            System.Diagnostics.Debug.WriteLine(sb.ToString());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            profileListForm.Show(mainDockPanel, DockState.DockLeft);

            runner = new G13ProfileRunner(testProfile);
        }


        Profile testProfile = new Profile
        {
            Macros = new List<Macro>
            {
                new Macro
                {
                    Name = "TestMacro",
                    Steps = new List<Step>
                    {
                        new KeyScanCodeDown(PInvoke.ScanCode.lshift),
                        new KeyCharTap('a'),
                        new Delay(400),
                        new KeyCharTap('b'),
                        new Delay(400),
                        new KeyCharTap('c'),
                        new Delay(400),
                        new KeyCharTap('d'),
                        new Delay(400),
                        new KeyCharTap('e')
                    }
                },
                new Macro
                {
                    Name = "TestMacro2",
                    IsCanceling = CancelingType.Forced,
                    Steps = new List<Step>
                    {
                        new KeyCharTap('1'),
                        new Delay(400),
                        new KeyCharTap('2'),
                        new Delay(400),
                        new KeyCharTap('3'),
                        new Delay(400),
                        new KeyCharTap('4'),
                        new Delay(400),
                        new KeyCharTap('5')
                    }
                }
            },
            InputAssignments = new List<InputAssignment>
            {
                new InputAssignment
                {
                    MacroName = "TestMacro",
                    Key = G13Keys.G1
                },
                new InputAssignment
                {
                    MacroName = "TestMacro2",
                    Key = G13Keys.G2
                }
            }
        };

        G13ProfileRunner runner;
        private void btnStartTest_Click(object sender, EventArgs e)
        {
            runner.Start();
        }

        private void btnStopTest_Click(object sender, EventArgs e)
        {
            runner.Stop();
        }
    }
}
