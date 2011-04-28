namespace GNet
{
    partial class ScriptEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.stopButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.editor = new ICSharpCode.TextEditor.TextEditorControl();
            this.lblEventQueue = new System.Windows.Forms.Label();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.lblEventQueue);
            this.buttonPanel.Controls.Add(this.stopButton);
            this.buttonPanel.Controls.Add(this.runButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(563, 25);
            this.buttonPanel.TabIndex = 0;
            // 
            // stopButton
            // 
            this.stopButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(75, 0);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 25);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // runButton
            // 
            this.runButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.runButton.Location = new System.Drawing.Point(0, 0);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 25);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.IsReadOnly = false;
            this.editor.Location = new System.Drawing.Point(0, 25);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(563, 429);
            this.editor.TabIndex = 1;
            // 
            // lblEventQueue
            // 
            this.lblEventQueue.AutoSize = true;
            this.lblEventQueue.Location = new System.Drawing.Point(156, 6);
            this.lblEventQueue.Name = "lblEventQueue";
            this.lblEventQueue.Size = new System.Drawing.Size(0, 13);
            this.lblEventQueue.TabIndex = 2;
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editor);
            this.Controls.Add(this.buttonPanel);
            this.Name = "ScriptEditor";
            this.Size = new System.Drawing.Size(563, 454);
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel buttonPanel;
        private ICSharpCode.TextEditor.TextEditorControl editor;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label lblEventQueue;
    }
}
