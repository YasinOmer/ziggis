using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.SystemUI;
#endif

namespace ZigGis.ArcGIS.ArcMapUI //Pulp.ArcGis.ZigGis
{
	[ClassInterface(ClassInterfaceType.AutoDual)] 
    [Guid("2FDB84D1-08D3-400A-9728-84A1141E1238")]
    public class Button : ICommand
    {
        private IntPtr m_hIcon;

        private void createIcon()
        {
            //Bitmap icon = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.zig.bmp"));
			Bitmap icon = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.pgsql_sm.bmp"));
            if (icon != null)
            {
                icon.MakeTransparent(icon.GetPixel(1, 1));
                m_hIcon = icon.GetHbitmap();
            }
        }

        private void destroyIcon()
        {
            if (m_hIcon.ToInt32() != 0)
            {
                DeleteObject(m_hIcon);
            }
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public Button()
        {
            createIcon();
        }

        ~Button()
        {
            destroyIcon();
        }

        static private void doError(Exception e)
        {
        }

        private IApplication m_app = null;
        internal IApplication application { get { return m_app; } }

        private AddForm m_dlg = null;
        private AddForm insertDialog
        {
            get
            {
                if (m_dlg == null)
                {
                    m_dlg = new AddForm();
                    m_dlg.Disposed += new EventHandler(insertDialog_Disposed);
                }
                return m_dlg;
            }
        }

        private void insertDialog_Disposed(object sender, EventArgs e)
        {
            m_dlg = null;
        } 

        #region ICommand
        public void OnClick()
        {
            insertDialog.show(this);
        }

        public void OnCreate(object hook)
        {
            try
            {
                m_app = (IApplication)hook;
            }
            catch (Exception e)
            {
                doError(e);
            }
        }

        public int Bitmap
        {
            get
            {
                return this.m_hIcon.ToInt32();
            }
        }

        public string Caption
        {
            get
            {
                return "Add PostGIS Layer";
            }
        }

        public string Category
        {
            get
            {
                return "ZigGIS";
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public int HelpContextID
        {
            get
            {
                return 0;
            }
        }

        public string HelpFile
        {
            get
            {
                return null;
            }
        }

        public string Message
        {
            get
            {
                return "Add PostGIS Layer";
            }
        }

        public string Name
        {
            get
            {
                return "Name";
            }
        }

        public string Tooltip
        {
            get
            {
                return "zigGIS";
            }
        }
        #endregion

        #region COM Registration
        [ComRegisterFunction()]
        static void RegistrationFunction(Type t)
        {
            string key = @"\CLSID\{" + t.GUID.ToString() + @"}\Implemented Categories";
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(key, true);
            if (regKey != null)
                regKey.CreateSubKey("{B56A7C42-83D4-11D2-A2E9-080009B6F22B}");
        }

        [ComUnregisterFunction()]
        static void UnregistrationFunction(Type t)
        {
            string key = @"\CLSID\{" + t.GUID.ToString() + @"}\Implemented Categories";
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(key, true);
            if (regKey != null)
                regKey.DeleteSubKey("{B56A7C42-83D4-11D2-A2E9-080009B6F22B}");
        }
        #endregion
    }
}
