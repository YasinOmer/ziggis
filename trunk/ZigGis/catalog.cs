using System;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Npgsql;
using System.Drawing;
using System.Text;
using ZigGis.PostGis;
using ZigGis.Utilities;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;

namespace ZigGis.PostGis.Catalog
{
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ComInteropClass")]
    [Guid("AFFC20DB-F7C7-4cc5-95A2-E6914EBD94A1")]
    public class GxOgcDatabaseFolder :
        IGxCachedObjects,
        IGxDataElement,
        IGxDataElementHelper,
        IGxObject,
        IGxObjectContainer,
        IGxObjectEdit,
        IGxObjectProperties,
        IGxObjectUI,
        IGxPasteTarget,
        IGxRemoteContainer
    {
        private IGxObject m_parentObject = null;
        private IGxCatalog m_parentCatalog = null;
        private IGxObjectArray m_children = null;
        private bool m_childrenLoaded = false;
        private Bitmap m_bitmapLrg;
        private IntPtr m_hBitmapLrg;
        private Bitmap m_bitmapSm;
        private IntPtr m_hBitmapSm;

        public GxOgcDatabaseFolder()
        {
            m_children = new GxObjectArrayClass();
            m_bitmapLrg = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ogc_lrg.bmp"));
            m_bitmapLrg.MakeTransparent(m_bitmapLrg.GetPixel(1, 1));
            m_hBitmapLrg = m_bitmapLrg.GetHbitmap();
            m_bitmapSm = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ogc_sm.bmp"));
            m_bitmapSm.MakeTransparent(m_bitmapSm.GetPixel(1, 1));
            m_hBitmapSm = m_bitmapSm.GetHbitmap();
        }

        ~GxOgcDatabaseFolder()
        {
            DeleteObject(m_hBitmapLrg);
            DeleteObject(m_hBitmapSm);
        }

        //HKEY_CLASSES_ROOT\Component Categories\{6E208C9A-DBD3-11D2-9F2F-00C04F6BC69E}
        #region Windows API
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        #endregion

        #region COM Registration
        [ComRegisterFunction()]
        static void RegistrationFunction(Type t)
        {
            string key = @"\CLSID\{" + t.GUID.ToString() + @"}\Implemented Categories";
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(key, true);
            if (regKey != null)
                regKey.CreateSubKey("{6E208C9A-DBD3-11D2-9F2F-00C04F6BC69E}");
        }

        [ComUnregisterFunction()]
        static void UnregistrationFunction(Type t)
        {
            string key = @"\CLSID\{" + t.GUID.ToString() + @"}\Implemented Categories";
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(key, true);
            if (regKey != null)
                regKey.DeleteSubKey("{6E208C9A-DBD3-11D2-9F2F-00C04F6BC69E}");
        }
        #endregion

        #region IGxCachedObjects Members

        public void LoadCachedObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReleaseCachedObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxDataElement Members

        public ESRI.ArcGIS.Geodatabase.IDataElement GetDataElement(ESRI.ArcGIS.Geodatabase.IDEBrowseOptions pBrowseOptions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxDataElementHelper Members

        public void RetrieveDEBaseProperties(ESRI.ArcGIS.Geodatabase.IDataElement ppDataElement)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RetrieveDEFullProperties(ESRI.ArcGIS.Geodatabase.IDataElement ppDataElement)
        {
            throw new Exception("The method or operation is not implemented.");
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
            get { return "OGC Database Connections"; }
        }

        public string Category
        {
            get { return "OGC Database Connections"; }
        }

        public ESRI.ArcGIS.esriSystem.UID ClassID
        {
            get 
            {
                IUID uid = new UIDClass();
                uid.Value = "{AFFC20DB-F7C7-4cc5-95A2-E6914EBD94A1}";
                return uid as UID;
            }
        }

        public void Detach()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string FullName
        {
            get { return "OGC Database Connections"; }
        }

        public ESRI.ArcGIS.esriSystem.IName InternalObjectName
        {
            get { return null; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public string Name
        {
            get { return "OGC Database Connections"; }
        }

        public IGxObject Parent
        {
            get { return m_parentObject; }
        }

        public void Refresh()
        {
            m_children.Empty();
            m_childrenLoaded = false;
            addWizards();
            //loadChildren;
        }

        #endregion

        #region IGxObjectContainer Members

        public IGxObject AddChild(IGxObject child)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool AreChildrenViewable
        {
            get { return true; }
        }

        public IEnumGxObject Children
        {
            get 
            {
                addWizards();
                return m_children as IEnumGxObject;
            }
        }

        public void DeleteChild(IGxObject child)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasChildren
        {
            get { return true; }
        }

        #endregion

        #region IGxObjectEdit Members

        public bool CanCopy()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanDelete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanRename()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Delete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void EditProperties(int hParent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Rename(string newShortName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectProperties Members

        public void GetPropByIndex(int index, ref string pName, ref object pValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetProperty(string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int PropertyCount
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void SetProperty(string Name, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectUI Members

        public ESRI.ArcGIS.esriSystem.UID ContextMenu
        {
            get { return null; }
        }

        public int LargeImage
        {
            get { return m_hBitmapLrg.ToInt32(); }
        }

        public int LargeSelectedImage
        {
            get { return m_hBitmapLrg.ToInt32(); }
        }

        public ESRI.ArcGIS.esriSystem.UID NewMenu
        {
            get { return null; }
        }

        public int SmallImage
        {
            get { return m_hBitmapSm.ToInt32(); ; }
        }

        public int SmallSelectedImage
        {
            get { return m_hBitmapSm.ToInt32(); }
        }

        #endregion

        #region IGxPasteTarget Members

        public bool CanPaste(ESRI.ArcGIS.esriSystem.IEnumName names, ref bool moveOperation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Paste(ESRI.ArcGIS.esriSystem.IEnumName names, ref bool moveOperation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region private members

        private void addWizards()
        {
            if (!m_childrenLoaded)
            {
                GxAddPostGisConnection pgWizard = new GxAddPostGisConnection();
                m_children.Insert(0, (IGxObject)pgWizard);
                m_childrenLoaded = true;
            }
        }
        #endregion
    }

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
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Refresh()
        {
            throw new Exception("The method or operation is not implemented.");
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