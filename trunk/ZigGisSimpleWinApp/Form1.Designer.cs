namespace ZigGisSimpleWinApp
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.axMapControl1 = new ESRI.ArcGIS.MapControl.AxMapControl();
			((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// axMapControl1
			// 
			this.axMapControl1.Location = new System.Drawing.Point(12, 12);
			this.axMapControl1.Name = "axMapControl1";
			this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
			this.axMapControl1.Size = new System.Drawing.Size(242, 231);
			this.axMapControl1.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(289, 272);
			this.Controls.Add(this.axMapControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ESRI.ArcGIS.MapControl.AxMapControl axMapControl1;
	}
}

