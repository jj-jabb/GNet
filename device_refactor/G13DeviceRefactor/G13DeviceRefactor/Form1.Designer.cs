namespace G13DeviceRefactor
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartDevice = new System.Windows.Forms.Button();
            this.btnStopDevice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartDevice
            // 
            this.btnStartDevice.Location = new System.Drawing.Point(12, 12);
            this.btnStartDevice.Name = "btnStartDevice";
            this.btnStartDevice.Size = new System.Drawing.Size(75, 23);
            this.btnStartDevice.TabIndex = 0;
            this.btnStartDevice.Text = "Start Device";
            this.btnStartDevice.UseVisualStyleBackColor = true;
            this.btnStartDevice.Click += new System.EventHandler(this.btnStartDevice_Click);
            // 
            // btnStopDevice
            // 
            this.btnStopDevice.Location = new System.Drawing.Point(12, 41);
            this.btnStopDevice.Name = "btnStopDevice";
            this.btnStopDevice.Size = new System.Drawing.Size(75, 23);
            this.btnStopDevice.TabIndex = 1;
            this.btnStopDevice.Text = "Stop Device";
            this.btnStopDevice.UseVisualStyleBackColor = true;
            this.btnStopDevice.Click += new System.EventHandler(this.btnStopDevice_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnStopDevice);
            this.Controls.Add(this.btnStartDevice);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartDevice;
        private System.Windows.Forms.Button btnStopDevice;
    }
}

