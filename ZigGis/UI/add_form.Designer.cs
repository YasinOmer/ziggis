namespace ZigGis.ArcGIS.ArcMapUI //Pulp.ArcGis.ZigGis
{
    partial class AddForm
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
			this.openFile = new System.Windows.Forms.Button();
			this.zigFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.clbLayers = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// openFile
			// 
			this.openFile.Location = new System.Drawing.Point(246, 10);
			this.openFile.Name = "openFile";
			this.openFile.Size = new System.Drawing.Size(28, 23);
			this.openFile.TabIndex = 0;
			this.openFile.Text = "...";
			this.openFile.Click += new System.EventHandler(this.openFile_Click);
			// 
			// zigFile
			// 
			this.zigFile.Location = new System.Drawing.Point(91, 12);
			this.zigFile.Name = "zigFile";
			this.zigFile.Size = new System.Drawing.Size(149, 20);
			this.zigFile.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "zig File";
			// 
			// openFileDlg
			// 
			this.openFileDlg.DefaultExt = "zig";
			this.openFileDlg.Filter = "zigGIS files|*.zig";
			this.openFileDlg.Title = "zigGIS Configuration File";
			// 
			// ok
			// 
			this.ok.Location = new System.Drawing.Point(176, 269);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(37, 23);
			this.ok.TabIndex = 5;
			this.ok.Text = "ok";
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.Location = new System.Drawing.Point(219, 269);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(55, 23);
			this.cancel.TabIndex = 6;
			this.cancel.Text = "cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// clbLayers
			// 
			this.clbLayers.CheckOnClick = true;
			this.clbLayers.FormattingEnabled = true;
			this.clbLayers.Location = new System.Drawing.Point(15, 51);
			this.clbLayers.Name = "clbLayers";
			this.clbLayers.Size = new System.Drawing.Size(259, 199);
			this.clbLayers.TabIndex = 7;
			// 
			// AddForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(291, 304);
			this.Controls.Add(this.clbLayers);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.zigFile);
			this.Controls.Add(this.openFile);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add PostGIS data";
			this.Load += new System.EventHandler(this.AddForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openFile;
        private System.Windows.Forms.TextBox zigFile;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.CheckedListBox clbLayers;
    }
}
