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

        }

        private bool saveZigFile()
        {
            bool retVal = false;
            try
            {
                IniConfigSource source = new IniConfigSource();
                //System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                IConfig config = source.AddConfig("connection");
                config.Set("Debug", "false");
                config.Set("Logging", "On");

                config = source.AddConfig("logging");
                config.Set("FilePath", "C:\\temp\\MyApp.log");

                source.Save("MyApp.ini"); 
                return retVal;
            }
            catch
            {
                return false;
            }
        }
    }
}