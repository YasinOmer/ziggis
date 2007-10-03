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
                    IEnumDataset edsn = _wkspc.get_Datasets(esriDatasetType.esriDTFeatureClass);
                    IDataset dsn;
                    // Load PostGIS layer names
                    while ((dsn = edsn.Next()) != null)
                    {
                        
                        //IFeatureWorkspace fwks = (IFeatureWorkspace)_wkspc;
                        ////System.Windows.Forms.MessageBox.Show(dsn.Name);
                        //IFeatureClass fc = fwks.OpenFeatureClass(dsn.Name);
                        ////bool tst = (fc == null);
                        ////System.Windows.Forms.MessageBox.Show(tst.ToString());
                        //IFeatureLayer layer = new PostGisFeatureLayer();
                        ////IFeatureLayer layer = new FeatureLayerClass();

                        //layer.FeatureClass = fc;
                        //layer.Name = fc.AliasName;
                        //GxPostGisLayer gxl = new GxPostGisLayer((ILayer)layer);
                        GxPostGisDataset gxd = new GxPostGisDataset(dsn);
                        m_children.Insert(-1, (IGxObject)gxd);

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
}