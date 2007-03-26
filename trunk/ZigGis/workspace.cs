// Copyright (C) 2005 Pulp
// http://www.digital-pulp.com
// Abe Gillespie, abe@digital-pulp.com
//
// This program is free software; you can redistribute it
// and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation;
// either version 2 of the License, or (at your option) any
// later version.
//
// This program is distributed in the hope that it will be
// useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330,
// Boston, MA 02111-1307 USA

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Nini.Config;
using Microsoft.Win32;
using ZigGis.Utilities;
using ZigGis.PostGis;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
#endif

namespace ZigGis.ArcGIS.Geodatabase
{
    internal class ComReg
    {
        static public void addComponentCategory(Type t, string guid)
        {
            RegistryKey key = getComponentCategory(t);
            if (key != null)
                key.CreateSubKey(addBrackets(guid));
        }

        static public void remComponentCategory(Type t, string guid)
        {
            RegistryKey key = getComponentCategory(t);
            if (key != null)
                key.DeleteSubKey(addBrackets(guid));
        }
    
        static public RegistryKey getComponentCategory(Type t)
        {
            string key = @"\CLSID\{" + t.GUID.ToString() + @"}\Implemented Categories";
            return Registry.ClassesRoot.OpenSubKey(key, true);
        }

        static public string addBrackets(string key)
        {
            return "{" + key + "}";
        }
    }
     [Guid("D06CF752-8595-4fa3-83FD-D14B71ADF93F"), ClassInterface(ClassInterfaceType.None), ProgId("ComInterOpClass")]
   public class PostGisWorkspaceFactory :
       IWorkspaceFactory
    {
        [ComRegisterFunction()]
        static void RegistrationFunction(Type t)
        {
            // ESRI Workspace Factories
            ComReg.addComponentCategory(t, "07BC056E-DB8A-11D1-AA81-00C04FA33A15");
        }

        [ComUnregisterFunction()]
        static void UnregistrationFunction(Type t)
        {
            // ESRI Workspace Factories
            ComReg.remComponentCategory(t, "07BC056E-DB8A-11D1-AA81-00C04FA33A15");
        }

        #region Logging setup
        static private readonly CLogger log = new CLogger(typeof(PostGisWorkspaceFactory));

        static PostGisWorkspaceFactory()
        {
        }
        #endregion

        // Let this be creatable.
        public PostGisWorkspaceFactory() {}

        #region IWorkspaceFactory
        public bool ContainsWorkspace(string parentDirectory, IFileNames FileNames)
        {
            return false;
        }

        public bool Copy(IWorkspaceName WorkspaceName, string destinationFolder, out IWorkspaceName workspaceNameCopy)
        {
            workspaceNameCopy = null;
            return false;
        }

        public IWorkspaceName Create(string parentDirectory, string Name, IPropertySet ConnectionProperties, int hWnd)
        {
            return new PostGisWorkspaceName(this, m_config);
        }

        public UID GetClassID()
        {
            return Helper.getTypeUid(typeof(PostGisWorkspaceFactory));
        }

        public IWorkspaceName GetWorkspaceName(string parentDirectory, IFileNames FileNames)
        {
            return null;
        }

        public bool IsWorkspace(string FileName)
        {
            return false;
        }

        public bool Move(IWorkspaceName WorkspaceName, string destinationFolder)
        {
            return false;
        }

		/// <summary>
		/// Open a PostGIS workspace from ConnectionProperties
		/// (Paolo Corti, january 2007)
		/// </summary>
		/// <param name="ConnectionProperties"></param>
		/// <param name="hWnd"></param>
		/// <returns></returns>
        public IWorkspace Open(IPropertySet ConnectionProperties, int hWnd)
        {
			IWorkspace retVal = null;
			try
			{
				m_config = new Config(ConnectionProperties);
				// immediately setup logging.
				if (config.loggingConfigInfo != null)
					log4net.Config.XmlConfigurator.Configure(config.loggingConfigInfo);

				// Log stuff.
				log.enterFunc("OpenFromConnectionProperties");
				if (log.IsDebugEnabled)
				{
					log.Debug("Logging now setup.");
					log.Debug(hWnd.ToString());
				}

				PostGisWorkspaceName wksName = new PostGisWorkspaceName(this, config);
				retVal = new PostGisFeatureWorkspace(wksName);
			
			}
			finally
			{
				log.leaveFunc();
			}
			return retVal;
        }

		/// <summary>
		/// Open a PostGIS workspace from a zig file
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="hWnd"></param>
		/// <returns></returns>
        public IWorkspace OpenFromFile(string FileName, int hWnd)
        {            
            IWorkspace retVal = null;
            try
            {
                // Grab the configuration and immediately setup logging.
                FileInfo fi = new FileInfo(FileName);
                //Abe
				//m_config = new Config(fi);
				m_config = new Config(fi);
                if (config.loggingConfigInfo != null)
                    log4net.Config.XmlConfigurator.Configure(config.loggingConfigInfo);

                // Log stuff.
                log.enterFunc("OpenFromFile");
                if (log.IsDebugEnabled)
                {
                    log.Debug("Logging now setup.");
                    log.Debug(FileName + "," + hWnd.ToString());
                }
                
                PostGisWorkspaceName wksName = new PostGisWorkspaceName(this, config);
                retVal = new PostGisFeatureWorkspace(wksName);
            }
            finally
            {
                log.leaveFunc();
            }
            return retVal;
        }

		/*
        private Config m_config = null;
        internal Config config { get { return m_config; } }
		*/

		private Config m_config = null;
		internal Config config { get { return m_config; } }

        public IPropertySet ReadConnectionPropertiesFromFile(string FileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public esriWorkspaceType WorkspaceType { get { return esriWorkspaceType.esriRemoteDatabaseWorkspace; } }

        public string get_WorkspaceDescription(bool plural)
        {
            return "zigGIS - PostGIS Connection" + (plural ? "s" : "");
        }
        #endregion
    }

    [Guid("C7BE986C-24A5-4d32-A146-3A4D48D389E3"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisWorkspaceName :
        IWorkspaceName,
        IName,
        IPersistStream
    {
        // We want a way to create an empty PostGisWorkspaceName
        // since we need a way to do its Load() function.
        internal PostGisWorkspaceName()
        {
        }

		/// <summary>
		/// PostGisWorkspaceName constructor from Config (both for zig file and PropertySet)
		/// (Paolo Corti, january 2007)
		/// </summary>
		/// <param name="PostGisWorkspaceFactory"></param>
		/// <param name="connectionProperties"></param>
		public PostGisWorkspaceName(PostGisWorkspaceFactory PostGisWorkspaceFactory, Config conf)
		{
			m_factory = PostGisWorkspaceFactory;
			//loadConfig(zigFileName);
			m_cfg = conf;
		}

		/*
		 * old constructor from Abe
		/// <summary>
		/// PostGisWorkspaceName constructor from zigFile
		/// </summary>
		/// <param name="PostGisWorkspaceFactory"></param>
		/// <param name="zigFileName"></param>
        public PostGisWorkspaceName(PostGisWorkspaceFactory PostGisWorkspaceFactory, string zigFileName)
        {
            m_factory = PostGisWorkspaceFactory;
            loadConfig(zigFileName);
        }
		*/

        private void loadConfig(string filePath)
        {
            // Todo - error on file DNE.
            FileInfo fileInfo = new FileInfo(filePath);
            m_cfg = new Config(fileInfo);
        }

		private Config m_cfg;
		internal Config config 
		{ 
			get { return m_cfg; }
		}

        #region IWorkspaceName
        private string m_browseName = "PostGIS Data";
        public string BrowseName
        {
            get { return m_browseName; }
            // Not sure why you can set this.  But hey, whatever.
            set { m_browseName = value; }
        }

        public string Category {get { return "zigGIS.PostGIS"; }}

        public IPropertySet ConnectionProperties
        {
            get { return config.connectionPropertySet; }
            // Todo - implement this.
			set { }
        }

        public string PathName
        {
            //get { return config2.fileInfo.FullName; }
			get { return null; }
            set { throw new NotImplementedException(); }
        }

        public esriWorkspaceType Type { get { return esriWorkspaceType.esriRemoteDatabaseWorkspace; } }

        private PostGisWorkspaceFactory m_factory = null;
        public IWorkspaceFactory WorkspaceFactory
        {
            get
            {
                if (m_factory == null)
                    m_factory = new PostGisWorkspaceFactory();
                return m_factory;
            }
        }

        // Todo - only PostGisFeatureWorkspace is implemented right now.
        // Perhaps in the future I'll implement PostGisTableWks.
        // The ProgId must be limited to this one or these two.
        private string m_wksfGuid;
        public string WorkspaceFactoryProgID
        {
            get { return m_wksfGuid; }
            // Todo - Constrain ProgId.
            set { m_wksfGuid = value; }
        }
        #endregion

        // Todo - implement this.  I think the IName::NameString is the
        // same as IWorkspaceName.BrowseName ... but the IName allows
        // for persistence functionality.
        #region IName
        public string NameString
        {
            get { return ""; }
            set { }
        }

        public object Open()
        {
            return new PostGisFeatureWorkspace(this);
        }
        #endregion

        #region IPersistStream
        public void GetClassID(out Guid pClassID)
        {
            pClassID = this.GetType().GUID;
        }

        public void GetSizeMax(out _ULARGE_INTEGER pcbSize)
        {
            throw new NotImplementedException();
        }

        public void IsDirty()
        {
        }

        public void Load(IStream pstm)
        {
            StreamHelper helper = new StreamHelper(pstm);

			/* Paolo : we don't use zig file anymore
            // Load the zig file.
            string path = helper.readString();
            loadConfig(path);
			 */

			// Paolo - Restore Connection properties
			IPropertySet ps = new PropertySetClass();
			ps.SetProperty("server", helper.readString());
			ps.SetProperty("database", helper.readString());
			ps.SetProperty("user", helper.readString());
			ps.SetProperty("password", helper.readString());
			ps.SetProperty("port", helper.readString());

			m_cfg = new Config(ps);
        }

        public void Save(IStream pstm, int fClearDirty)
        {
            StreamHelper helper = new StreamHelper(pstm);

            // Save the path to the zig file.
            //helper.writeString(config2.fileInfo.FullName);
        }
        #endregion
    }

    [Guid("E9804134-A3F6-44f9-84AC-3BBB8B2EA955"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFeatureWorkspace :
        IWorkspace,
        IFeatureWorkspace,
        IWorkspaceProperties,
        IWorkspaceEdit,
        IDataset,
        IDatasetContainer,
        IFeatureWorkspaceManage,
        IFeatureWorkspaceSchemaEdit,
        ISpatialCacheManager,
        ISQLSyntax,
        IWorkspaceSpatialReferenceInfo,
        IDatabaseConnectionInfo
    {
        static private readonly CLogger log = new CLogger(typeof(PostGisFeatureWorkspace));
        
        public PostGisFeatureWorkspace(PostGisWorkspaceName PostGisWorkspaceName)
        {
            m_wksName = PostGisWorkspaceName;
        }

        private PostGisWorkspaceName m_wksName;
        private PostGisWorkspaceName PostGisWorkspaceName { get { return m_wksName; } }

        private Connection m_con;
        internal Connection connection
        {
            get
            {
                log.enterFunc("connection");

                if (m_con == null)
                {
                    if (log.IsDebugEnabled) log.Debug(PostGisWorkspaceName.config.connectionString);
                    m_con = new Connection(PostGisWorkspaceName.config.connectionString, true);
                }

                log.leaveFunc();
                
                return m_con;
            }
        }

        private string getConnectionProperty(string property)
        {
            return (string)ConnectionProperties.GetProperty(property) + ";";
        }

        #region IWorkspace
        public IPropertySet ConnectionProperties { get { return PostGisWorkspaceName.config.connectionPropertySet; } }

        public void ExecuteSQL(string sqlStmt)
        {
            // This shouldn't be implemented since we don't
            // want to run random, arbitrary sql against the Db.
            // Any admin stuff should be taken care of directly
            // by a Db admin.  For this release anyways.
            throw new NotImplementedException();
        }

        // Todo - this probably isn't a correct implementation.
        // I think we should be trying to connect to the DB here.
        // Connect = yes, Exists = true.
        // Connect = no, Exists = false.
        public bool Exists()
        {
            return File.Exists(PostGisWorkspaceName.PathName);
        }

        public bool IsDirectory()
        {
            // dir \
            //      - file1.zig \
            //                  - connection info
            //      - file1.zig \
            //                  - connection info
            return false;
        }

        public string PathName { get { return PostGisWorkspaceName.PathName; } }

        public esriWorkspaceType Type { get { return esriWorkspaceType.esriRemoteDatabaseWorkspace; } }

        public IWorkspaceFactory WorkspaceFactory { get { return PostGisWorkspaceName.WorkspaceFactory; } }

        public IEnumDatasetName get_DatasetNames(esriDatasetType DatasetType)
        {
            if (DatasetType == esriDatasetType.esriDTFeatureClass)
            {
                // Todo - this should return any table registered with the geometry columns table.
            }

            return new PostGisEnumDatasetName(connection);

            // Todo - perhaps in the future support of tables might be nice.
        }

        public IEnumDataset get_Datasets(esriDatasetType DatasetType)
        {
            return new PostGisEnumDataset();
        }
        #endregion

        #region IFeatureWorkspace
        public IFeatureClass CreateFeatureClass(string Name, IFields Fields, UID CLSID, UID EXTCLSID, esriFeatureType FeatureType, string ShapeFieldName, string ConfigKeyword)
        {
            // See ExecuteSQL() for argument.
            throw new NotImplementedException();
        }

        public IFeatureDataset CreateFeatureDataset(string Name, ISpatialReference SpatialReference)
        {
            // See ExecuteSQL() for argument.
            throw new NotImplementedException();
        }

        public IQueryDef CreateQueryDef()
        {
            // This may be useful in the future.  But will skip implementation for now.
            // This allows a custom query to return an ICursor on the dataset.
            throw new NotImplementedException();
        }

        public IRelationshipClass CreateRelationshipClass(string relClassName, IObjectClass OriginClass, IObjectClass DestinationClass, string forwardLabel, string backwardLabel, esriRelCardinality Cardinality, esriRelNotification Notification, bool IsComposite, bool IsAttributed, IFields relAttrFields, string OriginPrimaryKey, string destPrimaryKey, string OriginForeignKey, string destForeignKey)
        {
            // I don't think this needs to be implemented for now.
            throw new NotImplementedException();
        }

        public ITable CreateTable(string Name, IFields Fields, UID CLSID, UID EXTCLSID, string ConfigKeyword)
        {
            // See ExecuteSQL() for argument.
            throw new NotImplementedException();
        }

        public IFeatureClass OpenFeatureClass(string Name)
        {
            try
            {
                log.enterFunc("OpenFeatureClass");

                // Name should look like "view" or "schema.view".
                // Default the schema to "public".
                string[] bits = Name.Split('.');
                string schema = "public";
                string view = bits[0];
                if (bits.Length > 1)
                {
                    schema = bits[0];
                    view = bits[1];
                }
                PostGisDatasetName dsName = new PostGisDatasetName();
                dsName.WorkspaceName = PostGisWorkspaceName;
                dsName.Name = schema;

                // Todo - ensure the schema exists.  Is it possible?

                PostGisFeatureDataset featureDs = new PostGisFeatureDataset(dsName, this);

                IFeatureClass retVal = new PostGisFeatureClass(featureDs, view);

                log.leaveFunc();

                return retVal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("OpenFeatureClass", ex.ToString() + "///" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Information);
                return null;
            }
        }

        // Todo - to be implemented.
        public IFeatureDataset OpenFeatureDataset(string Name)
        {
            // Name = schema.
            return null;
        }

        public IFeatureDataset OpenFeatureQuery(string QueryName, IQueryDef pQueryDef)
        {
            // See CreateQueryDef() for argument.
            throw new NotImplementedException();
        }

        public IRelationshipClass OpenRelationshipClass(string Name)
        {
            // I don't think this needs to be implemented for now.
            throw new NotImplementedException();
        }

        public ITable OpenRelationshipQuery(IRelationshipClass pRelClass, bool joinForward, IQueryFilter pSrcQueryFilter, ISelectionSet pSrcSelectionSet, string TargetColumns, bool DoNotPushJoinToDB)
        {
            // I don't think this needs to be implemented for now.
            throw new NotImplementedException();
        }

        public ITable OpenTable(string Name)
        {
            // Not supporting tables just yet.
            throw new NotImplementedException();
        }
        #endregion

        #region IWorkspaceProperties
        public IWorkspaceProperty get_Property(esriWorkspacePropertyGroupType propertyGroup, int PropertyType)
        {
            IWorkspaceProperty retVal = null;
            switch (propertyGroup)
            {
                case esriWorkspacePropertyGroupType.esriWorkspacePropertyGroup:
                    retVal = getWorkspaceProperty(PropertyType);
                    break;

                case esriWorkspacePropertyGroupType.esriWorkspaceTablePropertyGroup:
                    retVal = getTableProperty(PropertyType);
                    break;
            }
            return retVal;
        }

        // Todo - what to do here?
        public void set_Property(esriWorkspacePropertyGroupType propertyGroup, int PropertyType, IWorkspaceProperty WorkspaceProperty)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        private static IWorkspaceProperty getTableProperty(int type)
        {
            IWorkspaceProperty retVal = null;

            // Todo - try to catch a cast exception.
            // If this fails then it's unsupported.
            esriWorkspaceTablePropertyType propType = ((esriWorkspaceTablePropertyType)type);

            switch (propType)
            {
                case esriWorkspaceTablePropertyType.esriTablePropCanAddField:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspaceTablePropertyType.esriTablePropCanAddIndex:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspaceTablePropertyType.esriTablePropCanDeleteField:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspaceTablePropertyType.esriTablePropCanDeleteIndex:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspaceTablePropertyType.esriTablePropOIDIsRecordNumber:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspaceTablePropertyType.esriTablePropRowCountIsCalculated:
                    retVal = newWksProperty(true, true, true);
                    break;
            
                default:
                    retVal = newWksProperty(true, false, null);
                    break;
            }
            return retVal;
        }

        private static IWorkspaceProperty getWorkspaceProperty(int type)
        {
            IWorkspaceProperty retVal = null;

            // Todo - try to catch a cast exception.
            // If this fails then it's unsupported.
            esriWorkspacePropertyType propType = ((esriWorkspacePropertyType)type);

            switch (propType)
            {
                case esriWorkspacePropertyType.esriWorkspacePropCanAnalyze:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropCanEdit:
// Todo - implement IWorkspaceEdit for this to be faithful.
                    retVal = newWksProperty(true, true, true);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropCanExecuteSQL:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropCanGetConfigurationKeywords:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropIsGeoDatabase:
                    retVal = newWksProperty(true, true, true);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropIsReadonly:
// Todo - not sure if this is correct.
                    retVal = newWksProperty(true, true, true);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropMaxWhereClauseLength:
// Todo - get this actual value.
                    retVal = newWksProperty(true, false, null);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropSupportsMetadata:
                    retVal = newWksProperty(true, true, false);
                    break;

                case esriWorkspacePropertyType.esriWorkspacePropSupportsQualifiedNames:
                    retVal = newWksProperty(true, true, false);
                    break;
                
                default:
                    retVal = newWksProperty(true, false, null);
                    break;
            }

            return retVal;
        }

        private static IWorkspaceProperty newWksProperty(bool readOnly, bool supported, object value)
        {
            IWorkspaceProperty prop = new WorkspacePropertyClass();
            prop.IsReadOnly = readOnly;
            prop.IsSupported = supported;
            prop.PropertyValue = value;
            return prop;
        }

        #region IWorkspaceEdit
        public void AbortEditOperation()
        {
        }

        public void DisableUndoRedo()
        {
        }

        public void EnableUndoRedo()
        {
        }

        public void HasEdits(ref bool pHasEdits)
        {
        }

        public void HasRedos(ref bool pHasRedos)
        {
        }

        public void HasUndos(ref bool pHasUndos)
        {
        }

        private bool m_beingEdited = false;
        public bool IsBeingEdited()
        {
            return m_beingEdited;
        }

        public void RedoEditOperation()
        {
        }

        public void StartEditOperation()
        {
            //System.Windows.Forms.MessageBox.Show("Edit operation started");
        }

        public void StartEditing(bool withUndoRedo)
        {
            m_beingEdited = true;
            //System.Windows.Forms.MessageBox.Show("StartEditing");
        }

        public void StopEditOperation()
        {
            //System.Windows.Forms.MessageBox.Show("Edit operation stopped");
        }

        public void StopEditing(bool saveEdits)
        {
            m_beingEdited = false;
            //System.Windows.Forms.MessageBox.Show("StopEditing");
        }

        public void UndoEditOperation()
        {
        }
        #endregion

        #region IDataset
        public string BrowseName
        {
            get { return PostGisWorkspaceName.BrowseName; }
            set { PostGisWorkspaceName.BrowseName = value; }
        }

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

        public string Category { get { return PostGisWorkspaceName.Category;}}

        public IDataset Copy(string copyName, IWorkspace copyWorkspace)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IName FullName { get { return PostGisWorkspaceName;}}

        public string Name { get { return "PostGis Workspace (Read Only)";}}

        public IPropertySet PropertySet { get { return PostGisWorkspaceName.ConnectionProperties; } }

        public void Rename(string Name)
        {
            throw new NotImplementedException();
        }

        public IEnumDataset Subsets
        {
            get { return null; }
        }

        esriDatasetType IDataset.Type { get { return esriDatasetType.esriDTFeatureDataset; } }

        public IWorkspace Workspace { get { return this; } }
        #endregion

        #region IDatasetContainer
        public void AddDataset(IDataset pDatasetToAdd)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region IFeatureWorkspaceManage
        public void AnalyzeIndex(string TableName, string Index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AnalyzeTable(string TableName, int tableComponents)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanDelete(IName aName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanRename(IName aName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DeleteByName(IDatasetName aName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRegisteredAsObjectClass(string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRegisteredAsVersioned(IName aName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IField ValidateField(IField pInField)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region IFeatureWorkspaceSchemaEdit
        public void AlterClassExtensionCLSID(string Name, UID ClassExtensionCLSID, IPropertySet classExtensionProperties)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AlterInstanceCLSID(string Name, UID InstanceCLSID)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ISpatialCacheManager
        public IEnvelope CacheExtent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool CacheIsFull
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void EmptyCache()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FillCache(IEnvelope pExtent)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ISQLSyntax
        public bool GetDelimitedIdentifierCase()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFunctionName(esriSQLFunctionName sqlFunc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool GetIdentifierCase()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetInvalidCharacters()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetInvalidStartingCharacters()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumBSTR GetKeywords()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetSpecialCharacter(esriSQLSpecialCharacters sqlSC)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool GetStringComparisonCase()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetSupportedClauses()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetSupportedPredicates()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ParseColumnName(string FullName, out string dbName, out string ownerName, out string TableName, out string ColumnName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ParseTableName(string FullName, out string dbName, out string ownerName, out string TableName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string QualifyColumnName(string TableName, string ColumnName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string QualifyTableName(string dbName, string ownerName, string TableName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region IWorkspaceSpatialReferenceInfo
        public IEnumSpatialReferenceInfo SpatialReferenceInfo
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
        #endregion

        #region IDatabaseConnectionInfo Members
        public string ConnectedDatabase
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ConnectedUser
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
        #endregion
	}
	#region IEnumDatasetName
	
	/// <summary>
	/// PostGisEnumDatasetName class
	/// Bill Dollins, Paolo Corti (january 2007)
	/// </summary>
    [Guid("6CAE99D6-72D5-4c2c-B7ED-9A84281E9DD2"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisEnumDatasetName :
        IEnumDatasetName
    {
        private PostGisDatasetName [] pgdsn; //array to store dataset names
        private Connection m_conn; //connection to postgis
        private int layerIndex = -1; //counter for enumeration
		private int layerCount; //layer counter

		/// <summary>
		/// parameterless constructor required for COM
		/// </summary>
        public PostGisEnumDatasetName()
        {
        }
        /// <summary>
        /// This constructor accepts a connection and loads the array that will drive the enumerator
        /// </summary>
        /// <param name="conn">connection</param>
        public PostGisEnumDatasetName(Connection conn) //constructor with connection
        {
            try
            {
                m_conn = conn; //save the connection. we save not need to do list.
                string sql = "select count(*) from public.geometry_columns;";
                AutoDataReader dr = conn.doQuery(sql); //get the number of layers in the database
                dr.Read();
               layerCount = Convert.ToInt32(dr["count"]); //capture the layer count
                dr.Close();
                sql = "select * from public.geometry_columns order by f_table_schema, f_table_name;";
                dr = conn.doQuery(sql); //get the records for the layers
                if (layerCount > 0) //if there's data
                {
                    pgdsn = new PostGisDatasetName[layerCount]; //init the array
                    int i = 0;
                    while (dr.Read()) //loop the data reader
                    {
                        pgdsn[i] = new PostGisDatasetName(); //instantiate a new dataset name
                        pgdsn[i].Name = dr["f_table_schema"] + "." + dr["f_table_name"]; //assign the name using the schema.view format
                        i += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                //what the heck went wrong here?
                System.Diagnostics.EventLog.WriteEntry("PostGisEnumDatasetName", ex.ToString() + "///" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
        }		

		/// <summary>
		/// Go to next IDatasetName
		/// </summary>
		/// <returns></returns>
        public IDatasetName Next()
        {
			layerIndex = layerIndex + 1;
			if (layerIndex >= layerCount)
			{
				return null;
			}
			else
			{
				return pgdsn[layerIndex] as IDatasetName;
			}
        }

		/// <summary>
		/// Reset EnumDatasetName
		/// </summary>
        public void Reset()
        {
			layerIndex = -1;
        }
        #endregion
    }

    [Guid("DA3CD42B-8239-4637-BB7E-AF50DD37BA9E"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisDatasetName :
        IDatasetName,
        IName,
        IPersistStream
    {
        #region IDatasetName
        // Todo - implement this.  Not sure what to do yet.
        public string Category
        {
            get { return "Category"; }
            set { }
        }
        
        // I guess this should just be the schema?
        private string m_name = "";
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public IEnumDatasetName SubsetNames
        {
            get { return null; }
        }

        public esriDatasetType Type
        {
            get { return esriDatasetType.esriDTFeatureDataset; }
        }

        private IWorkspaceName m_wksName = null;
        public IWorkspaceName WorkspaceName
        {
            get
            {
                //if (m_wksName == null)
                //    m_wksName = new PostGisWorkspaceName(
                return m_wksName;
            }
            set {m_wksName = value;}
        }
        #endregion

        // Todo - implement this.  I think the IName::NameString is the
        // same as IDatasetName::Name ... but the IName allows
        // for persistence functionality.
        #region IName
        public string NameString
        {
            get { return ""; }
            set { }
        }

        public object Open()
        {
            // Open the Workspace first.
            IWorkspaceName wksn = ((IDatasetName)this).WorkspaceName;
            PostGisFeatureWorkspace wks = (PostGisFeatureWorkspace)((IName)wksn).Open();
            return new PostGisFeatureDataset(this, wks);
        }
        #endregion

        #region IPersistStream
        public void GetClassID(out Guid pClassID)
        {
            pClassID = this.GetType().GUID;
        }

        public void GetSizeMax(out _ULARGE_INTEGER pcbSize)
        {
            throw new NotImplementedException();
        }

        public void IsDirty()
        {
        }

        public void Load(IStream pstm)
        {
            StreamHelper helper = new StreamHelper(pstm);

            // Restore the Postgres schema name.
            m_name = helper.readString();

            // Restore the WorkspaceName.
            m_wksName = new PostGisWorkspaceName();
            ((IPersistStream)m_wksName).Load(pstm);
        }

        public void Save(IStream pstm, int fClearDirty)
        {
            StreamHelper helper = new StreamHelper(pstm);

            // Save the Postgres schema name.
            helper.writeString(Name);

			// Paolo - Save connection properties (server, database, user, password, port)
			helper.writeString(m_wksName.ConnectionProperties.GetProperty("server").ToString());
			helper.writeString(m_wksName.ConnectionProperties.GetProperty("database").ToString());
			helper.writeString(m_wksName.ConnectionProperties.GetProperty("user").ToString());
			helper.writeString(m_wksName.ConnectionProperties.GetProperty("password").ToString());
			helper.writeString(m_wksName.ConnectionProperties.GetProperty("port").ToString());

            // Save the WorkspaceName.
            IWorkspaceName wksName = ((IDatasetName)this).WorkspaceName;
            ((IPersistStream)wksName).Save(pstm, fClearDirty);
        }
        #endregion
    }

    [Guid("FABC20AD-8324-4b17-99F6-5626067CA4BE"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisEnumDataset :
        IEnumDataset
    {
        #region IEnumDataset

		public PostGisEnumDataset()
		{
			//add 
		}
        public IDataset Next()
        {
            return null;
        }

        public void Reset()
        {
        }
        #endregion
    }

    // The PostGisFeatureDataset is the concept of a schema in Postgres.
    // There can be a number of spatial tables in a schema.
    // Each table can be thought of as a PostGisFeatureClass.
    [Guid("3423E4D4-65A7-4db9-B2FB-415FF5403472"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFeatureDataset :
        IFeatureDataset,
        IDatasetEditInfo,
        IDatasetEdit,
        IDataset
    {
        public PostGisFeatureDataset(PostGisDatasetName postGisDatasetName, PostGisFeatureWorkspace PostGisFeatureWorkspace)
        {
            m_dsName = postGisDatasetName;
            m_fwks = PostGisFeatureWorkspace;
        }

        public PostGisFeatureDataset(PostGisDatasetName postGisDatasetName)
        {
            
        }

        private PostGisDatasetName m_dsName;
        private PostGisDatasetName postGisDatasetName { get { return m_dsName; } }

        private PostGisFeatureWorkspace m_fwks;
        private PostGisFeatureWorkspace PostGisFeatureWorkspace { get { return m_fwks; } }

        #region IFeatureDataset
        public string BrowseName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PostGisFeatureWorkspace.ConnectionProperties.GetProperty("db"));
                sb.Append(".");
                sb.Append(postGisDatasetName.Name);
                return sb.ToString();
            }
            // Todo - should this be implemented?
            set {}
        }

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

        // Todo - implement this.  Probably "PostGIS" ... but is there a global way to do this?
        public string Category
        {
            get { return postGisDatasetName.Category; }
        }

        // Todo - I think this is ok since we flagged it as uncopiable.
        // Maybe throw an exception instead.
        public IDataset Copy(string copyName, IWorkspace copyWorkspace)
        {
            return null;
        }

        public IFeatureClass CreateFeatureClass(string Name, IFields Fields, UID CLSID, UID EXTCLSID, esriFeatureType FeatureType, string ShapeFieldName, string ConfigKeyword)
        {
            throw new NotImplementedException();
        }

        // Todo - throw an exception?
        public void Delete()
        {
        }

        public IName FullName
        {
            get { return postGisDatasetName; }
        }

        public string Name
        {
            get { return postGisDatasetName.Name; }
        }

        public IPropertySet PropertySet
        {
            get { return PostGisFeatureWorkspace.ConnectionProperties; }
        }

        // Todo - throw an exception?
        public void Rename(string Name)
        {
        }

        public IEnumDataset Subsets
        {
            get { return null; }
        }

        public esriDatasetType Type
        {
            get { return esriDatasetType.esriDTFeatureDataset; }
        }

        public IWorkspace Workspace
        {
            get { return PostGisFeatureWorkspace; }
        }
        #endregion

        #region IDatasetEditInfo
        public bool CanEdit { get { return true; } }

        public bool CanRedo { get { return false; } }

        public bool CanUndo { get { return false; } }
        #endregion

        #region IDatasetEdit Members
        public bool IsBeingEdited()
        {
            return true; //Bill Dollins
            //throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }

	/// <summary>
	/// Config
	/// Paolo Corti, February 2007
	/// </summary>
	public class Config
	{
		/// <summary>
		/// Constructor from PropertySet (in the SDEWorkspace fashion)
		/// </summary>
		/// <param name="propertySet"></param>
		public Config(IPropertySet propertySet)
		{
			m_ps = propertySet;
			//generate connection string
		    addConnectionItem("server", m_ps.GetProperty("server").ToString());
			addConnectionItem("database", m_ps.GetProperty("database").ToString());
			addConnectionItem("user", m_ps.GetProperty("user").ToString());
			addConnectionItem("password", m_ps.GetProperty("password").ToString());
			addConnectionItem("port", m_ps.GetProperty("port").ToString());
			//if configfile is passed set the log setting
			try //manage COM except if property is missing
			{
				object configFileProp = m_ps.GetProperty("configfile");
				if (configFileProp != null)
				{
					string path = configFileProp.ToString();
					setLoggingSettings(path);
				}
			}
			catch(SystemException e)
			{
				System.Diagnostics.Debug.WriteLine("configfile property not set");
			}
		}

		/// <summary>
		/// Constructor from zig file
		/// </summary>
		/// <param name="filePath">zigGIS file</param>
		public Config(FileInfo filePath)
        {
            // Todo - throw exception on file DNE.
            IConfigSource m_iniCfgSrc = new IniConfigSource(filePath.FullName);
			// Load the connection settings.
			IConfig configConn = m_iniCfgSrc.Configs["connection"];
            //loadPropertySet;
			m_ps = new PropertySetClass();
			m_conStr = "";
			string[] keys = configConn.GetKeys();
			foreach (string key in keys)
			{
				string value = configConn.GetString(key);
				//load PropertySet
				m_ps.SetProperty(key, value);
				//generate connection string
				addConnectionItem(key, value);
			}
			// Load the logging settings.
			IConfig configLog = m_iniCfgSrc.Configs["logging"];
			if (configLog != null)
			{
				string path = configLog.GetString("configfile");
				setLoggingSettings(path);
			}
        }

		/// <summary>
		/// Build a connection string from zig file parameters or PropertySet
		/// </summary>
		/// <param name="item"></param>
		/// <param name="value"></param>
		private void addConnectionItem(string item, string value)
		{
			connectionPropertySet.SetProperty(item, value);
			m_conStr += item + "=" + value + ";";
		}

		/// <summary>
		/// Set a log file for logging
		/// </summary>
		/// <param name="path">path to logfile (from zig file or PropertySet)</param>
		private void setLoggingSettings(string path)
		{
			if (File.Exists(path))
			{
				m_logConfigFi = new FileInfo(path);
			}
		}

		/// <summary>
		/// connectionPropertySet
		/// PropertySet for accessing/storing connection parameters of OGC Workspace
		/// </summary>
		private IPropertySet m_ps;
		// Todo - add a setter.
		public IPropertySet connectionPropertySet 
		{ 
			get { return m_ps; }
			// Paolo (added set)
			set { m_ps = value; }
		}

		/// <summary>
		/// connectionString
		/// Connection string for accessing OGC RDBMS (PostGIS)
		/// </summary>
		private string m_conStr;
		public string connectionString { get { return m_conStr; } }

		/// <summary>
		/// loggingConfigInfo
		/// FileInfo for logging, if not configured may be null and logging is not performed
		/// </summary>
		private FileInfo m_logConfigFi;

		public FileInfo loggingConfigInfo { get { return m_logConfigFi; } }
	}

	/*
	 * 
	/// <summary>
	/// Old Config class from Abe
	/// </summary>
    public class Config :
        IConfigSource
    {
        public Config(FileInfo filePath)
        {
            // Todo - throw exception on file DNE.
            m_path = filePath;
            m_iniCfgSrc = new IniConfigSource(filePath.FullName);
            loadPropertySet();
        }

        private FileInfo m_path = null;
        public FileInfo fileInfo { get { return m_path; } }

        private IniConfigSource m_iniCfgSrc;
        private IniConfigSource iniConfigSource { get { return m_iniCfgSrc; } }

        // Build the PropertySet.
        // As a side affect, create a new connection string.
        private void loadPropertySet()
        {
            // Load the connection settings.
            IConfig config = Configs["connection"];

            m_ps = new PropertySetClass();
            m_conStr = "";
            string [] keys = config.GetKeys();

            foreach (string key in keys)
                addConnectionItem(key, config.GetString(key));

            // Load the logging settings.
            config = Configs["logging"];
            if (config != null)
            {
                string path = config.GetString("configfile");
                if (File.Exists(path))
                    m_logConfigFi = new FileInfo(path);
            }

            // Todo - get the optional items into the PropertySet.
            // Todo - get the optional items into the connection string.
        }

        private IPropertySet m_ps;
        // Todo - add a setter.
        public IPropertySet connectionPropertySet { get { return m_ps; } }

        private string m_conStr;
        public string connectionString { get { return m_conStr; } }

        private void addConnectionItem(string item, string value)
        {
            connectionPropertySet.SetProperty(item, value);
            m_conStr += item + "=" + value + ";";
        }

        private FileInfo m_logConfigFi;
        public FileInfo loggingConfigInfo { get { return m_logConfigFi; } }

        #region IConfigSource
        public IConfig AddConfig(string name)
        {
            return iniConfigSource.AddConfig(name);
        }

        public AliasText Alias { get { return iniConfigSource.Alias; } }

        public bool AutoSave
        {
            get { return iniConfigSource.AutoSave; }
            set { iniConfigSource.AutoSave = value; }
        }

        public ConfigCollection Configs { get { return iniConfigSource.Configs; } }

        public void Merge(IConfigSource source)
        {
            iniConfigSource.Merge(source);
        }

        public void Reload()
        {
            iniConfigSource.Reload();
            loadPropertySet();
        }

        public void ReplaceKeyValues()
        {
            iniConfigSource.ReplaceKeyValues();
        }

        public void ExpandKeyValues()
        {
            iniConfigSource.ExpandKeyValues(); 
        }

        public string GetExpanded(IConfig conf, string s)
        {
            return null;
        }

        public void Save()
        {
            iniConfigSource.Save();
        }
        #endregion

        #region IConfigSource Members
        public event EventHandler Reloaded;
        public event EventHandler Saved;
        #endregion
    }
	 * */
}
