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
        delegate void OnScriptStoppedHandler();
        //IScriptRunner scriptRunner;
        //IDeviceScript script;
        Profile profile;

        public ScriptEditor(Profile profile)
        {
            InitializeComponent();

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
            IHighlightingStrategy h;
            switch(profile.Language)
            {
                case ScriptLanguage.Lua:
                    h = HighlightingManager.Manager.FindHighlighter("Lua");
                    if (h != null)
                    {
                        editor.SetHighlighting("Lua");
                        HighlightingManager.Manager.AddHighlightingStrategy(h);
                    }
                    break;

                case ScriptLanguage.Boo:
                    h = HighlightingManager.Manager.FindHighlighter("Boo");
                    if (h != null)
                    {
                        editor.SetHighlighting("Boo");
                        HighlightingManager.Manager.AddHighlightingStrategy(h);
                    }
                    break;
            }

            this.profile = profile;
            editor.Text = profile.Contents;

            ProfileManager.Current.ScriptStopped += new EventHandler(Current_ScriptStopped);
        }

        void Current_ScriptStopped(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new OnScriptStoppedHandler(OnScriptStopped));
            else
                OnScriptStopped();
        }

        void OnScriptStopped()
        {
            runButton.Enabled = true;
            stopButton.Enabled = false;
            editor.IsReadOnly = false;
        }

        public Profile Profile { get { return profile; } }
        public TextEditorControl Editor { get { return editor; } }

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
                ProfileManager.Current.SetProfile(profile).Start();
            }
            catch (Exception ex)
            {
                ProfileManager.Current.SetProfile(profile).Stop();
                Console.WriteLine(ex.ToString());
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopScript();
        }

        public void StopScript()
        {
            ProfileManager.Current.Stop();

            runButton.Enabled = true;
            stopButton.Enabled = false;
            editor.IsReadOnly = false;
        }
    }
}
