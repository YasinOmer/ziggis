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
        private bool m_wizardsLoaded = false;
        private bool m_databasesLoaded = false;
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
            this.Refresh();
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
            m_wizardsLoaded = false;
            m_databasesLoaded = false;
            m_children.Empty();
            addWizards();
            addDatabases();
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
                addDatabases();
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
            if (!m_wizardsLoaded)
            {
                GxAddPostGisConnection pgWizard = new GxAddPostGisConnection();
                m_children.Insert(0, (IGxObject)pgWizard);
                m_wizardsLoaded = true;
            }
        }

        private void addDatabases()
        {
            if (!m_databasesLoaded)
            {
                string appDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZigGIS";
                if (System.IO.Directory.Exists(appDataPath))
                {
                    string[] f = System.IO.Directory.GetFiles(appDataPath, "*.zig", System.IO.SearchOption.TopDirectoryOnly);
                    for (int ii = 0; ii < f.Length; ii++)
                    {
                        GxPostGisDatabase pgdb = new GxPostGisDatabase(f[ii]);
                        m_children.Insert(-1, (IGxObject)pgdb);
                    }
                    m_databasesLoaded = true;
                }
            }
        }
        #endregion
    }
}