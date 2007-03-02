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
    }
}