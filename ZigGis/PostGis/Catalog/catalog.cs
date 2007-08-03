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
    //[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    //Guid("2E9B4CA5-4F53-4068-AEB0-72D04EE8F742"), ComVisible(true)]
    [Guid("2E9B4CA5-4F53-4068-AEB0-72D04EE8F742")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IGxPostGisDatabase
    {
        string Host { get;set;}
        int Port { get;set;}
        string Database { get;set;}
        string UserName { get;set;}
        string Password { get;set;}
        void setZigFile(string zigFilePath);
    }
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
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("zigCatalog.GxPostGisDatabase")]
    [Guid("4625623D-0498-482a-8992-B71DBC9DB304")]
    public class GxPostGisDatabase : IGxPostGisDatabase, IGxDatabase, IGxObject, IGxObjectContainer, IGxObjectEdit, IGxObjectUI, IGxRemoteContainer
    {
        private IGxObject m_parentObject = null;
        private IGxCatalog m_parentCatalog = null;
        private IGxObjectArray m_children = null;
        private bool m_childrenLoaded = false;
        private Bitmap m_bitmapLrg;
        private IntPtr m_hBitmapLrg;
        private Bitmap m_bitmapSm;
        private IntPtr m_hBitmapSm;
        private string _host = "";
        private int _port = 0;
        private string _database = "";
        private string _username = "";
        private string _password = "";
        private string _zigFilePath = "";
        private string _zigFileBase = "";
        private IWorkspace _wkspc = null;

        #region Windows API
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        #endregion

        #region Constructors/Destructors
        public GxPostGisDatabase()
        {
            m_bitmapLrg = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ziggis_big.bmp"));
            m_bitmapLrg.MakeTransparent(m_bitmapLrg.GetPixel(1, 1));
            m_hBitmapLrg = m_bitmapLrg.GetHbitmap();
            m_bitmapSm = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ziggis_small.bmp"));
            m_bitmapSm.MakeTransparent(m_bitmapSm.GetPixel(1, 1));
            m_hBitmapSm = m_bitmapSm.GetHbitmap();
            m_children = new GxObjectArrayClass();
        }

        public GxPostGisDatabase(string zigFilePath)
        {
            m_bitmapLrg = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ziggis_big.bmp"));
            m_bitmapLrg.MakeTransparent(m_bitmapLrg.GetPixel(1, 1));
            m_hBitmapLrg = m_bitmapLrg.GetHbitmap();
            m_bitmapSm = new Bitmap(GetType().Assembly.GetManifestResourceStream("Pulp.ArcGis.ZigGis.images.ziggis_small.bmp"));
            m_bitmapSm.MakeTransparent(m_bitmapSm.GetPixel(1, 1));
            m_hBitmapSm = m_bitmapSm.GetHbitmap();
            m_children = new GxObjectArrayClass();
            this.setZigFile(zigFilePath);
            this.Refresh();
        }

        ~GxPostGisDatabase()
        {
            DeleteObject(m_hBitmapLrg);
            DeleteObject(m_hBitmapSm);
        }
        #endregion

        #region IGxPostGisDatabase Members

        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public void setZigFile(string zigFilePath)
        {
            this._zigFilePath = zigFilePath;
            System.IO.FileInfo fi = new System.IO.FileInfo(zigFilePath);
            _zigFileBase = fi.Name;
            parseZigFile();
        }

        #endregion

        #region IGxDatabase Members

        public void Disconnect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsConnected
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsRemoteDatabase
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ESRI.ArcGIS.Geodatabase.IWorkspace Workspace
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ESRI.ArcGIS.Geodatabase.IWorkspaceName WorkspaceName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IGxObject Members

        public void Attach(IGxObject Parent, IGxCatalog pCatalog)
        {
            m_parentCatalog = pCatalog;
            m_parentObject = Parent;
            //this.Refresh();
            //throw new Exception("The method or operation is not implemented.");
        }

        public string BaseName
        {
            get { return _zigFileBase; }
        }

        public string Category
        {
            get { return "ZigGIS"; }
        }

        public UID ClassID
        {
            get
            {
                IUID uid = new UIDClass();
                uid.Value = "{4625623D-0498-482a-8992-B71DBC9DB304}";
                return uid as UID;
            }

        }

        public void Detach()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public string FullName
        {
            get 
            {
                return _zigFileBase;
            }
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
            get { return _zigFileBase; }
        }

        public IGxObject Parent
        {
            get { return m_parentObject; }
        }

        public void Refresh()
        {
            try
            {
                m_childrenLoaded = false;
                m_children.Empty();
                addLayers();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectContainer Members

        public IGxObject AddChild(IGxObject child)
        {
            m_children.Insert(-1, child);
            return child;
        }

        public bool AreChildrenViewable
        {
            get { return true; }
        }

        public IEnumGxObject Children
        {
            get { return (IEnumGxObject)m_children; }
        }

        public void DeleteChild(IGxObject child)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasChildren
        {
            get { return m_children.Count > 0; }
        }

        #endregion

        #region IGxObjectEdit Members

        public bool CanCopy()
        {
            return false;
        }

        public bool CanDelete()
        {
            return false;
        }

        public bool CanRename()
        {
            return false;
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

        #region IGxObjectUI Members

        public UID ContextMenu
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int LargeImage
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int LargeSelectedImage
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public UID NewMenu
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int SmallImage
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int SmallSelectedImage
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region Private Members
        private void addLayers()
        {
            //System.Windows.Forms.MessageBox.Show("adding layers " + m_childrenLoaded.ToString());
            try
            {
                if (!m_childrenLoaded)
                {
                    //System.Windows.Forms.MessageBox.Show("adding layers");
                    IEnumDatasetName edsn = _wkspc.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                    IDatasetName dsn;
                    // Load PostGIS layer names
                    while ((dsn = edsn.Next()) != null)
                    {
                     
                        IFeatureWorkspace fwks = (IFeatureWorkspace)_wkspc;
                        //System.Windows.Forms.MessageBox.Show(dsn.Name);
                        IFeatureClass fc = fwks.OpenFeatureClass(dsn.Name);
                        //bool tst = (fc == null);
                        //System.Windows.Forms.MessageBox.Show(tst.ToString());
                        IFeatureLayer layer = new PostGisFeatureLayer();
                        //IFeatureLayer layer = new FeatureLayerClass();

                        layer.FeatureClass = fc;
                        layer.Name = fc.AliasName;
                        GxPostGisLayer gxl = new GxPostGisLayer((ILayer)layer);
                        m_children.Insert(-1, (IGxObject)gxl);

                    }
                }
            }
            catch (Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.ToString());
            }

        }

        private void parseZigFile()
        {
            try
            {
                string pth = _zigFilePath;
                // Open workspace
                IWorkspaceFactory wksf = new PostGisWorkspaceFactory();
                IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.OpenFromFile(pth, 0);
                // Open workspace
                IWorkspace ws = fwks as IWorkspace;
                _wkspc = ws;
                _host = ws.ConnectionProperties.GetProperty("server").ToString();
                _port = Convert.ToInt32(ws.ConnectionProperties.GetProperty("port"));
                _database = ws.ConnectionProperties.GetProperty("database").ToString();
                _username = ws.ConnectionProperties.GetProperty("user").ToString();
                _password = ws.ConnectionProperties.GetProperty("password").ToString();
                //IEnumDatasetName edsn = ws.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                //IDatasetName dsn;
                //// Load PostGIS layer names
                //clbLayers.Items.Clear();
                //while ((dsn = edsn.Next()) != null)
                //{
                //    clbLayers.Items.Add(dsn.Name);
                //}
            }
            catch (COMException COMex)
            {
                //MessageBox.Show("Error " + COMex.ErrorCode.ToString() + ": " + COMex.Message);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
            }
        }
        #endregion
    }
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("zigCatalog.GxPostGisLayer")]
    [Guid("D418F337-EEC0-4204-92A2-03219088CABB")]
    public class GxPostGisLayer : ESRI.ArcGIS.Catalog.GxLayerClass
    {
        private ILayer _lyr;
        public GxPostGisLayer(ILayer fl)
        {
            _lyr = fl;
        }

        public override void Attach(IGxObject Parent, IGxCatalog pCatalog)
        {
            base.Attach(Parent, pCatalog);
        }

        public override string BaseName
        {
            get
            {
                return base.BaseName;
            }
        }

        public override string Category
        {
            get
            {
                return base.Category;
            }
        }

        public override UID ClassID
        {
            get
            {
                IUID uid = new UIDClass();
                uid.Value = "{D418F337-EEC0-4204-92A2-03219088CABB}";
                return uid as UID;
            }
        }

        public override void Detach()
        {
            base.Detach();
        }

        public override IName InternalObjectName
        {
            get
            {
                return base.InternalObjectName;
            }
        }

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public override IGxObject Parent
        {
            get
            {
                return base.Parent;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        public override ILayer Layer
        {
            get
            {
                return _lyr;
            }
            set
            {
               _lyr = value;
            }
        }

        public override string Name
        {
            get
            {
                return _lyr.Name;
            }
        }

        public override string FullName
        {
            get
            {
                return _lyr.Name;
            }
        }

        //public override 
    }

}