using System;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Nini.Config;
using Nini.Ini;
using Npgsql;
using System.Drawing;
using System.Text;
using ZigGis.ArcGIS.Geodatabase;
using ZigGis.PostGis;
using ZigGis.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ZigGis.PostGis.Catalog
{
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ComInteropClass")]
    [Guid("56BF7C24-FCD3-4f9b-BE3D-CA5C2794C498")]
    public class GxAddPostGisConnection :
            IGxBasicObject,
            IGxObject,
            IGxObjectUI,
            IGxObjectWizard
    {
        private IGxObject m_parentObject = null;
        private IGxCatalog m_parentCatalog = null;
        private IGxObjectArray m_children = null;
        private bool m_childrenLoaded = false;
        private Bitmap m_bitmapLrg;
        private IntPtr m_hBitmapLrg;
        private Bitmap m_bitmapSm;
        private IntPtr m_hBitmapSm;

        #region Windows API
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        #endregion

        #region Constructors/Destructors
        public GxAddPostGisConnection()
        {
            m_bitmapLrg = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.pgsql_lrg.bmp"));
            m_bitmapLrg.MakeTransparent(m_bitmapLrg.GetPixel(1, 1));
            m_hBitmapLrg = m_bitmapLrg.GetHbitmap();
            m_bitmapSm = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.pgsql_sm.bmp"));
            m_bitmapSm.MakeTransparent(m_bitmapSm.GetPixel(1, 1));
            m_hBitmapSm = m_bitmapSm.GetHbitmap();

        }

        ~GxAddPostGisConnection()
        {
            DeleteObject(m_hBitmapLrg);
            DeleteObject(m_hBitmapSm);
        }
        #endregion

        #region IGxObject Members

        public void Attach(IGxObject Parent, IGxCatalog pCatalog)
        {
            m_parentCatalog = pCatalog;
            m_parentObject = Parent;
            //throw new Exception("The method or operation is not implemented.");
        }

        public string BaseName
        {
            get { return "Add New PostGIS Connection"; }
        }

        public string Category
        {
            get { return "Add New PostGIS Connection"; }
        }

        public UID ClassID
        {
            get
            {
                IUID uid = new UIDClass();
                uid.Value = "{56BF7C24-FCD3-4f9b-BE3D-CA5C2794C498}";
                return uid as UID;
            }
        }

        public void Detach()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public string FullName
        {
            get { return "Add New PostGIS Connection"; }
        }

        public IName InternalObjectName
        {
            get { return null; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public string Name
        {
            get { return "Add New PostGIS Connection"; }
        }

        public IGxObject Parent
        {
            get { return m_parentObject; }
        }

        public void Refresh()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectUI Members

        public UID ContextMenu
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int LargeImage
        {
            get { return m_hBitmapLrg.ToInt32(); }
        }

        public int LargeSelectedImage
        {
            get { return m_hBitmapLrg.ToInt32(); }
        }

        public UID NewMenu
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int SmallImage
        {
            get { return m_hBitmapSm.ToInt32(); }
        }

        public int SmallSelectedImage
        {
            get { return m_hBitmapSm.ToInt32(); }
        }

        #endregion

        #region IGxObjectWizard Members

        public void Invoke(int hParentWnd)
        {
            ZigGis.ArcGIS.ArcMapUI.PostGisConnectionForm frmPgConnect = new ZigGis.ArcGIS.ArcMapUI.PostGisConnectionForm(hParentWnd);
            frmPgConnect.Show();
        }
        #endregion
    }
}