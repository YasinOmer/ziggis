using Nini.Config;
using Nini.Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ZigGis.ArcGIS.ArcMapUI
{
    public partial class PostGisConnectionForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetWindowLong(int hWnd, int index, int NewInt);

        public PostGisConnectionForm()
        {
            InitializeComponent();
        }

        public PostGisConnectionForm(int ownerHwnd)
        {
            InitializeComponent();
            SetWindowLong(this.Handle.ToInt32(), -8, ownerHwnd);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool success = saveZigFile();
            this.Hide();
            this.Dispose();
        }

        private bool saveZigFile()
        {
            bool retVal = false;
            try
            {
                IniConfigSource source = new IniConfigSource();
                string appDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZigGIS";
                if (!System.IO.Directory.Exists(appDataPath))
                {
                    System.IO.Directory.CreateDirectory(appDataPath);
                }
                IConfig config = source.AddConfig("connection");
                config.Set("server", this.txtServer.Text);
                config.Set("port", this.txtSchema.Text);
                config.Set("database", this.txtDatabase.Text);
                config.Set("user", this.txtUserName.Text);
                config.Set("password", this.txtPassword.Text);

                config = source.AddConfig("logging");
                config.Set("configfile", this.txtLogFile.Text);

                string zigFileName = this.txtServer.Text + "." + this.txtDatabase.Text + "." + this.txtUserName.Text + "." + System.Guid.NewGuid().ToString() + ".zig";
                source.Save(appDataPath + "\\" + zigFileName);
                return retVal;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Dispose();
        }

        private void tmrValidate_Tick(object sender, EventArgs e)
        {
            this.btnOK.Enabled = ((this.txtDatabase.Text.Trim() != string.Empty) & (this.txtServer.Text.Trim() != string.Empty) & (this.txtSchema.Text.Trim() != string.Empty) & (this.txtUserName.Text.Trim() != string.Empty) & (this.txtPassword.Text.Trim() != string.Empty) & (this.txtLogFile.Text.Trim() != string.Empty));
        }
    }
}