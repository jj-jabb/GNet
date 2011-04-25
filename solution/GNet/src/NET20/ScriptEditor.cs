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

using GNet.Scripting;

namespace GNet
{
    public partial class ScriptEditor : UserControl
    {
        //IScriptRunner scriptRunner;
        IDeviceScript script;
        Profile profile;

        public ScriptEditor(Profile profile)
        {
            InitializeComponent();

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
            var h = HighlightingManager.Manager.FindHighlighter("Lua");
            if (h != null)
            {
                editor.SetHighlighting("Lua");
                HighlightingManager.Manager.AddHighlightingStrategy(h);
            }

            this.profile = profile;
            editor.Text = profile.Contents;

            script = new LuaScript();
            script.Profile = profile;

        }

        string eventQueue;
        void ScriptEditor_EventQueueUpdated(object sender, Hid.EventArgs<string> e)
        {
            if (eventQueue == e.Data)
                return;

            eventQueue = e.Data;

            if (InvokeRequired)
                Invoke(new Action<string>(UpdateEventQueueLabel), e.Data);
            else
                UpdateEventQueueLabel(e.Data);
        }

        void UpdateEventQueueLabel(string s)
        {
            lblEventQueue.Text = s;
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            DisposeScript();
            base.OnControlRemoved(e);
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            runButton.Enabled = false;
            stopButton.Enabled = true;

            try
            {
                editor.IsReadOnly = true;
                profile.Contents = editor.Text;
                script.Start();
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

        public void StopScript()
        {
            script.Stop();

            runButton.Enabled = true;
            stopButton.Enabled = false;
            editor.IsReadOnly = false;
        }

        public void DisposeScript()
        {
            script.Stop();
        }
    }
}
