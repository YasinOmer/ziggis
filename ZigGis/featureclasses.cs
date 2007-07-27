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

// Uncomment to build a brand new clean build.
// Then comment this out and rebuild to get a release build.
//#define STUB_BUILD

using System;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using ZigGis.Utilities;
using ZigGis.PostGis;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

#endif

namespace ZigGis.ArcGIS.Geodatabase
{
    // A PostGisFeatureClass is any spatial data in a Postgres schema.
    // Perhaps we need to implement IFeatureEdit to have this editable?
    [Guid("82CF1A49-7D1B-453f-8DFF-33808C737E73"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFeatureClass :
		//IClass, //Paolo
		IDataset,
		IDatasetEdit,
		IDatasetEditInfo,
        IFeatureClass,
		//IFeatureDraw, //Paolo
		IGeoDataset,
		//IModelInfo, //Paolo
		//IObjectClass, //Paolo
        ITable,
        ITableCapabilities
        
    {
        static private readonly CLogger log = new CLogger(typeof(PostGisFeatureClass));
        
        private PostGisFeatureClass(PostGisFeatureDataset postGisFeatureDataset)
        {
            m_featureDs = postGisFeatureDataset;
        }

        internal PostGisFeatureClass(PostGisFeatureDataset postGisFeatureDataset, int featureClassId) : this(postGisFeatureDataset)
        {
            m_layer = ((PostGisFeatureWorkspace)postGisFeatureDataset.Workspace).connection.getLayer(featureClassId);
            init();
        }

        internal PostGisFeatureClass(PostGisFeatureDataset postGisFeatureDataset, string view) : this(postGisFeatureDataset)
        {
            m_layer = ((PostGisFeatureWorkspace)postGisFeatureDataset.Workspace).connection.getLayer(postGisFeatureDataset.Name, view);
            init();
        }

        private Layer m_layer;
        internal Layer postGisLayer { get { return m_layer; } }

        private void init()
        {
					m_layHelper = CLayerHelperBase.createLayerHelper(postGisLayer.geometryType);
					if (m_layer.srid != -1)
					{
						ISpatialReferenceFactory2 srf = new SpatialReferenceEnvironmentClass();
						//m_spaRef = srf.CreateSpatialReference(m_layer.srid);
                        m_spaRef = GeomHelper.setEsriSpatiaReferenceFromSrText(m_layer.srid, m_layer.connection);
						//m_spaRef = GeomHelper.setEsriSpatiaReferenceFromSrid(m_layer.srid);
					}
        }

        private CLayerHelperBase m_layHelper;
        internal CLayerHelperBase layerHelper { get { return m_layHelper; } }

        #region IFeatureClass
        public void AddField(IField Field)
        {
            log.enterFunc("AddField");
        }

        public void AddIndex(IIndex Index)
        {
            log.enterFunc("AddIndex");
        }

        public string AliasName { get { return Name; } }

        public IField AreaField { get { return null; } }

        public UID CLSID { get { return Helper.getTypeUid(typeof(PostGisFeatureClass)); } }

        public IFeature CreateFeature()
        {
            log.enterFunc("CreateFeature");
            return null;
        }

        public IFeatureBuffer CreateFeatureBuffer()
        {
            log.enterFunc("CreateFeatureBuffer");
            return null;
        }

        public void DeleteField(IField Field)
        {
            log.enterFunc("DeleteField");
        }

        public void DeleteIndex(IIndex Index)
        {
            log.enterFunc("DeleteIndex");
        }

        public UID EXTCLSID { get { return null; } }

        public object Extension { get { return null; } }

        public IPropertySet ExtensionProperties { get { return null; } }

        public int FeatureClassID { get { return postGisLayer.oid; } }

        public int FeatureCount(IQueryFilter QueryFilter)
        {
            log.enterFunc("FeatureCount");
            
            string fields;
            string where;
            GeomHelper.aoQryToPostGisQry(QueryFilter, postGisLayer, out fields, out where);
            return postGisLayer.getRecordCount(where);
        }

        private PostGisFeatureDataset m_featureDs;
        public IFeatureDataset FeatureDataset { get { return m_featureDs; } }

        public esriFeatureType FeatureType { get { return esriFeatureType.esriFTSimple; } }

        // Todo - to speed things up, we might want to cache this in the future.
        // This depends, of course, how often this is queried for by ArcGIS.
        private PostGisFields m_flds = null;
        public IFields Fields
        {
            get
            {
                log.enterFunc("Fields");
                
                if (m_flds == null)
                    m_flds = new PostGisFields(postGisLayer);

                log.leaveFunc();
                
                return m_flds;
            }
        }

        public int FindField(string Name)
        {
            log.enterFunc("FindField");

            if (log.IsDebugEnabled) log.Debug(Name);

            int i = Fields.FindField(Name);

            log.leaveFunc();

            return i;
        }

		//Paolo (march 2007): we need to get the geometry field in the usual manner
        public IFeature GetFeature(int ID)
        {
            log.enterFunc("GetFeature");
            
			/* old: wrong
            IDataRecord rec = postGisLayer.getRecord(ID);
            return new PostGisFeature(this, rec);
			*/
			
			//SELECT * where gid = 12
			string fields;
			string where;
			IQueryFilter qf = new QueryFilterClass();
			qf.WhereClause = PostGisConstants.idField.ToString() + "=" + ID.ToString();
			GeomHelper.aoQryToPostGisQry(qf, postGisLayer, out fields, out where);
			IDataRecord rec = postGisLayer.getRecord(fields, where);
			return new PostGisFeature(this, rec);
        }

        public IFeatureCursor GetFeatures(object fids, bool Recycling)
        {
            log.enterFunc("GetFeatures");
            
            return null;
        }

        public bool HasOID { get { return true; } }

        public IIndexes Indexes
        {
            get { return null; }
        }

        public IFeatureCursor Insert(bool useBuffering)
        {
            log.enterFunc("Insert");
            
            return null;
        }

        public IField LengthField { get { return null; } }

        public string OIDFieldName { get { return PostGisConstants.idField; } }

        public int ObjectClassID { get { return FeatureClassID; } }

        public IFeatureCursor Search(IQueryFilter Filter, bool Recycling)
        {
            log.enterFunc("Search");

            if (log.IsDebugEnabled) log.Debug(Helper.objectToString(Filter) + "," + Recycling.ToString());

            IFeatureCursor retVal = null;
            try
            {
                string where;
                string fields;
                GeomHelper.aoQryToPostGisQry(Filter, postGisLayer, out fields, out where);
                AutoDataReader dr = postGisLayer.doQuery(fields, where);
#if !STUB_BUILD
                retVal = new PostGisFeatureCursor(this, dr);
#endif
            }
            finally
            {
                log.leaveFunc();
            }
            return retVal;
        }

		/// <summary>
		/// Get a ISelectionSet from the feature class (Paolo, march 2007)
		/// </summary>
		/// <param name="QueryFilter"></param>
		/// <param name="selType"></param>
		/// <param name="selOption"></param>
		/// <param name="selectionContainer"></param>
		/// <returns></returns>
        public ISelectionSet Select(IQueryFilter QueryFilter, esriSelectionType selType, esriSelectionOption selOption, IWorkspace selectionContainer)
        {
            log.enterFunc("Select");
            
            if (log.IsDebugEnabled) log.Debug(Helper.objectToString(QueryFilter) + "," + selType.ToString() + "," + selOption.ToString() + "," + Helper.objectToString(selectionContainer));

            ISelectionSet retVal = null;

            try
            {
				//retVal = new PostGisSelectionSet(this);

				//2 cases, with null QueryFilter and without
				if (QueryFilter == null)
				{
					retVal = new PostGisSelectionSet(this);
				}
				else
				{
					string sql = "";
					string fields = "";
					string where = "";
					GeomHelper.aoQryToPostGisQry(QueryFilter, postGisLayer, out fields, out where);
					AutoDataReader dr = postGisLayer.doQuery(fields, where);
					retVal = new PostGisSelectionSet(this, dr);
				}
				
            }
            finally
            {
                log.leaveFunc();
            }

			return retVal as ISelectionSet;

        }

        public string ShapeFieldName { get { return postGisLayer.geometryField; } }

        public esriGeometryType ShapeType { get { return layerHelper.shapeType; } }

        public IFeatureCursor Update(IQueryFilter QueryFilter, bool Recycling)
        {
            log.enterFunc("Update");
            
            return null;
        }

        public IEnumRelationshipClass get_RelationshipClasses(esriRelRole role)
        {
            log.enterFunc("get_RelationshipClasses");
            
            return null;
        }
        #endregion

        #region ITable
        public IRow CreateRow()
        {
            log.enterFunc("CreateRow");
            
            return (IRow)CreateFeature();
        }

        // Todo - implement for editing.
        public IRowBuffer CreateRowBuffer()
        {
            log.enterFunc("CreateRowBuffer");
            
            return (IRowBuffer)CreateFeatureBuffer();
        }

        // Todo - implement for editing.
        public void DeleteSearchedRows(IQueryFilter QueryFilter)
        {
            log.enterFunc("DeleteSearchedRows");
        }

        public IRow GetRow(int OID)
        {
            log.enterFunc("GetRow");
            
            return (IRow)GetFeature(OID);
        }

        public ICursor GetRows(object oids, bool Recycling)
        {
            log.enterFunc("GetRows");

            return (ICursor)GetFeatures(oids, Recycling);
        }

        ICursor ITable.Insert(bool useBuffering)
        {
            log.enterFunc("Insert");
            
            return null;
        }

        // Todo - implement this puppy.
        public int RowCount(IQueryFilter QueryFilter)
        {
            log.enterFunc("RowCount");
            
            return 0;
        }

        ICursor ITable.Search(IQueryFilter QueryFilter, bool Recycling)
        {
            log.enterFunc("Search");
            
            return (ICursor)((IFeatureClass)this).Search(QueryFilter, Recycling);
        }

        ICursor ITable.Update(IQueryFilter QueryFilter, bool Recycling)
        {
            log.enterFunc("Update");
            
            return null;
        }

        public void UpdateSearchedRows(IQueryFilter QueryFilter, IRowBuffer Buffer)
        {
            log.enterFunc("UpdateSearchedRows");
        }
        #endregion

        #region IDataset
        public string BrowseName
        {
            get {return FeatureDataset.BrowseName;}
            set {FeatureDataset.BrowseName = value;}
        }

        public bool CanCopy()
        {
            log.enterFunc("CanCopy");
            
            return FeatureDataset.CanCopy();
        }

        public bool CanDelete()
        {
            log.enterFunc("CanDelete");
            
            return FeatureDataset.CanDelete();
        }

        public bool CanRename()
        {
            log.enterFunc("CanRename");
            
            return FeatureDataset.CanRename();
        }

        public string Category {get { return FeatureDataset.Category; }}

        public IDataset Copy(string copyName, IWorkspace copyWorkspace)
        {
            log.enterFunc("Copy");
            
            return FeatureDataset.Copy(copyName, copyWorkspace);
        }

        public void Delete()
        {
            log.enterFunc("Delete");
            
            FeatureDataset.Delete();
        }

        public IName FullName { get { return FeatureDataset.FullName; } }

        public string Name {get { return postGisLayer.view; }}

        public IPropertySet PropertySet {get { return FeatureDataset.PropertySet; }}

        public void Rename(string Name)
        {
            log.enterFunc("Rename");
            
            FeatureDataset.Rename(Name);
        }

        public IEnumDataset Subsets { get { return FeatureDataset.Subsets; } }

        public esriDatasetType Type {get { return FeatureDataset.Type; }}

        public IWorkspace Workspace 
		{
			get 
			{ 
				return FeatureDataset.Workspace; 
			}
		}
        #endregion

        #region IDatasetEditInfo
        public bool CanEdit { get { return true; } }

        public bool CanRedo { get { return true; } }

        public bool CanUndo { get { return true; } }
        #endregion

        #region IGeoDataset
        public IEnvelope Extent
        {
            get
            {
                log.enterFunc("Extent");
                
                byte [] wkb = postGisLayer.wkbExtent;
                WkbParser parser = new WkbParser();
                IGeometry geom = parser.parseWkb(wkb);
                return geom.Envelope;
            }
        }

        // Todo - implement this correctly.
        private ISpatialReference m_spaRef = new UnknownCoordinateSystemClass();
        public ISpatialReference SpatialReference { get { return m_spaRef; } }
        #endregion

        #region ITableCapabilities
        public bool CanSelect { get { return true; } }
        #endregion

        #region IDatasetEdit
        private bool m_beingEdited = false;
        public bool IsBeingEdited()
        {
            log.enterFunc("IsBeingEdited");
            
            return m_beingEdited;
        }
        #endregion

		/*
		#region IFeatureDraw

		void IFeatureDraw.Draw(esriDrawPhase drawPhase, IDisplay Display, ISymbol symbol, bool symbolInstalled, IGeometry Geometry, esriDrawStyle drawStyle)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		IInvalidArea IFeatureDraw.InvalidArea
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
		*/

		/*

		#region IClass

		void IClass.AddField(IField Field)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void IClass.AddIndex(IIndex Index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		UID IClass.CLSID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		void IClass.DeleteField(IField Field)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void IClass.DeleteIndex(IIndex Index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		UID IClass.EXTCLSID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		object IClass.Extension
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IPropertySet IClass.ExtensionProperties
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IFields IClass.Fields
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		int IClass.FindField(string Name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		bool IClass.HasOID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IIndexes IClass.Indexes
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		string IClass.OIDFieldName
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	
		#region IObjectClass

		void  IObjectClass.AddField(IField Field)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		void  IObjectClass.AddIndex(IIndex Index)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		string  IObjectClass.AliasName
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		UID  IObjectClass.CLSID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		void  IObjectClass.DeleteField(IField Field)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		void  IObjectClass.DeleteIndex(IIndex Index)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		UID  IObjectClass.EXTCLSID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		object  IObjectClass.Extension
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IPropertySet  IObjectClass.ExtensionProperties
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IFields  IObjectClass.Fields
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		int  IObjectClass.FindField(string Name)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		bool  IObjectClass.HasOID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IIndexes  IObjectClass.Indexes
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		string  IObjectClass.OIDFieldName
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		int  IObjectClass.ObjectClassID
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IEnumRelationshipClass  IObjectClass.get_RelationshipClasses(esriRelRole role)
		{
 			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
		

		#region IModelInfo Members

		public string ModelName
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
		*/
	}

    [Guid("40D05FB9-6063-41d7-A2F5-68197D9BB545"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFeature :
        IFeature,
        IFeatureDraw,
        IRowEdit,
        IFeatureEdit,
        IRowBuffer,
        IFeatureBuffer,
        IRowChanges,
        IFeatureChanges,
        IRowCompare,
        IRowEvents,
        IFeatureEvents
    {
        static private readonly CLogger log = new CLogger(typeof(PostGisFeature));
        
        public PostGisFeature(PostGisFeatureClass postGisFeatureClass, IDataRecord dataRecord)
        {
            //log.enterFunc("ctor");
            //if (log.IsDebugEnabled) log.Debug(CHelper.objectToString(postGisFeatureClass) + "," + CHelper.objectToString(dataRecord));

            try
            {
                m_featClass = postGisFeatureClass;
                m_values = new object[Fields.FieldCount];

                // Load the record.
                object o;
                string name;
                string idFld = PostGisConstants.idField.ToLower();
                string geomFld = postGisFeatureClass.postGisLayer.geometryField.ToLower();
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    // Do some book keeping.
                    o = dataRecord[i];
                    name = dataRecord.GetName(i).ToLower();

                    if (o == DBNull.Value) continue;

                    // *--- Handle special fields ---*

                    // Load the Id.
					if (name == idFld)
						m_oid = dataRecord.GetInt32(i);
					//m_oid = (int)o;

					// Load the geometry.
					else if (name == geomFld)
					{
						WkbParser parser = new WkbParser();
						m_geom = parser.parseWkb((byte[])o);
						o = Shape;
					}

                    // *-----------------------------*

                    m_values[i] = o;
                }
            }
            finally
            {
                log.leaveFunc();
            }
        }

        private object[] m_values;
        private object[] values { get { return m_values; } }

        private PostGisFeatureClass m_featClass;
        private PostGisFeatureClass postGisFeatureClass { get { return m_featClass; } }

        #region IFeature
        public IObjectClass Class { get { return postGisFeatureClass; } }

        public void Delete()
        {
        }

		/// <summary>
		/// Get IEnvelope from feature (Paolo, march 2007)
		/// </summary>
        public IEnvelope Extent
        {
            get 
			{
				return m_geom.Envelope;
				//remove this exception, thrown from selections: feature envelope is needed
				//throw new Exception("The method or operation is not implemented."); 
			}
        }

        public esriFeatureType FeatureType { get { return postGisFeatureClass.FeatureType; } }

        public IFields Fields { get { return postGisFeatureClass.Fields; } }

        public bool HasOID { get { return true; } }

        private int m_oid = -1;
        public int OID { get { return m_oid; } }

        // Todo - implement the setter.
        private IGeometry m_geom = null;
        public IGeometry Shape
        {
            get { return m_geom; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IGeometry ShapeCopy
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Store()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ITable Table { get { return postGisFeatureClass; } }

        public object get_Value(int Index)
        {
            //log.enterFunc("get_Value");
            //if (log.IsDebugEnabled) log.Debug(Index);
            
            object retVal = null;
            try
            {
                Helper.checkWithinBounds(Index, postGisFeatureClass.Fields.FieldCount);
                retVal = values[Index];
                //explicit cast for esriFieldTypeDouble and esriFieldTypeInteger (if not doing so ArcMap won't show or will crash)
                if (postGisFeatureClass.Fields.get_Field(Index).Type == esriFieldType.esriFieldTypeDouble)
                {
                    retVal = (object)double.Parse(retVal.ToString());
                }
                if (postGisFeatureClass.Fields.get_Field(Index).Type == esriFieldType.esriFieldTypeInteger)
                {
                    retVal = (object)int.Parse(retVal.ToString());
                }
            }
            finally
            {
                //log.leaveFunc();
            }
            return retVal;
        }

        public void set_Value(int Index, object Value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region IFeatureDraw

		/// <summary>
		/// Draw feture (Paolo, march 2007)
		/// </summary>
		/// <param name="drawPhase"></param>
		/// <param name="Display"></param>
		/// <param name="Symbol"></param>
		/// <param name="symbolInstalled"></param>
		/// <param name="Geometry"></param>
		/// <param name="DrawStyle"></param>
        public void Draw(esriDrawPhase drawPhase, IDisplay Display, ISymbol Symbol, bool symbolInstalled, IGeometry Geometry, esriDrawStyle DrawStyle)
        {
            // This if-statement might not be necessary.  The proper
            // filtering may have already occurred before this.

			//Paolo : remove this filter, without it selection should work
			/*
            if (!(drawPhase == esriDrawPhase.esriDPGeography && DrawStyle == esriDrawStyle.esriDSNormal)) return;
			*/

            if (Shape == null) return;

            Display.SetSymbol(Symbol);
            postGisFeatureClass.layerHelper.draw(Display, Shape);
        }

        public IInvalidArea InvalidArea
        {
            get
            {
				//not sure about this (Paolo)
				/*
				IInvalidArea ia = new InvalidAreaClass();
				ia.Add(m_geom);
				*/
				return null;
            }
            set
            {
                InvalidArea = value;
            }
        }
        #endregion

        #region IFeatureEdit Members
        public IDisplayFeedback BeginMoveSet(ISet features, IPoint Start)
        {
            return null;
        }

        public void DeleteSet(ISet Rows)
        {
        }

        public void MoveSet(ISet features, ILine MoveVector)
        {
        }

        public void RotateSet(ISet features, IPoint Origin, double Angle)
        {
        }

        public ISet Split(IGeometry Point)
        {
            return null;
        }

        public void SplitAttributes(IFeature baseFeature)
        {
        }
        #endregion

        #region IFeatureChanges
        public IGeometry OriginalShape
        {
            get { return null; }
        }

        public bool ShapeChanged
        {
            get { return false; }
        }
        #endregion

        #region IRowChanges
        public object get_OriginalValue(int Index)
        {
            return null;
        }

        public bool get_ValueChanged(int Index)
        {
            return false;
        }
        #endregion

        #region IRowCompare Members

        public bool get_IsEqual(IRow pOtherRow)
        {
            return false;
        }
        #endregion

        #region IFeatureEvents
        public void InitShape()
        {
        }

        public void OnMerge()
        {
        }

        public void OnSplit()
        {
        }
        #endregion

        #region IRowEvents
        public void OnChanged()
        {
        }

        public void OnDelete()
        {
        }

        public void OnInitialize()
        {
        }

        public void OnNew()
        {
        }

        public void OnValidate()
        {
        }
        #endregion
    }

    [Guid("CC5C6470-97C2-4249-8438-FF5BA9131D02"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFields : 
		IFields,
		IFields2 //Paolo (without this rendering do not work)
		//IFieldsEdit //Paolo
    {
        static private readonly CLogger log = new CLogger(typeof(PostGisFields));
        
        internal PostGisFields(Layer postGisLayer)
        {
            log.enterFunc("ctor");

            try
            {
				bool hasGIDField = false;
                if (log.IsDebugEnabled) log.Debug("1");
                DataTable dataFields = postGisLayer.getDataFields(false);
                if (log.IsDebugEnabled) log.Debug("2");
                m_flds = new PostGisField[dataFields.Rows.Count];
                m_ids = new Hashtable(fields.Length);
                PostGisField pgisFld;
                int i = 0;
                foreach (DataRow r in dataFields.Rows)
                {
                    pgisFld = new PostGisField(postGisLayer, r, i);
                    m_flds[i] = pgisFld;
                    m_ids.Add(pgisFld.Name, i);
					if(pgisFld.Name.ToLower()=="gid")
					{
						hasGIDField=true;
					}
                    ++i;
                }
				//gid (int4 or int8) is mandatory as GDB unique OID
				if (hasGIDField==false)
				{
					System.Windows.Forms.MessageBox.Show("This PostGis layer will be added in ArcMap but will not correctly work for some funcitonality (selections, rendering, ...): it misses a mandatory gid (int) field as unique OID for each feature. This gid field is necessary for the Esri Geodatabase model.");
				}
            }
            catch (Exception e)
            {
                log.Error("", e);
                throw;
            }
            finally
            {
                log.leaveFunc();
            }
        }

        private PostGisField [] m_flds;
        private PostGisField[] fields { get { return m_flds; } }

        private Hashtable m_ids;
        private Hashtable idMap { get { return m_ids; } }


        #region IFields
        public int FieldCount { get { return fields.Length; } }

        public int FindField(string Name)
        {
            log.enterFunc("FindField");
            
            string name = Name.ToLower();
            object o = idMap[name];
            int i = (o == null ? -1 : (int)o);

            log.leaveFunc();

            return i;
        }

        public int FindFieldByAliasName(string Name)
        {
            throw new NotImplementedException();
        }

        public IField get_Field(int Index)
        {
            //log.enterFunc("get_Field");
            //if (log.IsDebugEnabled) log.Debug(Index);

            IField retVal = null;
            try
            {
                Helper.checkWithinBounds(Index, fields.Length);
                retVal = fields[Index];
            }
            finally
            {
                //log.leaveFunc();
            }

            return retVal;
        }
        #endregion

		

		#region IFields2 Members


		public void FindFieldIgnoreQualification(ISQLSyntax sqlSyntax, string Name, out int Index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		/*
		
		#region IFieldsEdit Members

		public void AddField(IField Field)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DeleteAllFields()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DeleteField(IField Field)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int FieldCount_2
		{
			set { throw new Exception("The method or operation is not implemented."); }
		}

		public void set_Field(int Index, IField __p2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
		*/
	}

    [Guid("65C0A853-C2CF-4182-B70F-9AF435486716"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisField : 
		IField
		//IField2, //Paolo
		//IModelInfo //Paolo
    {
        public PostGisField(Layer postGisLayer, DataRow row, int id)
        {
            m_layer = postGisLayer;
            m_id = id;
            //m_default = column.DefaultValue;
            m_name = (string)row["ColumnName"];
            setType((Type)row["DataType"]);
            m_precision = (int)row["NumericPrecision"];
            m_scale = (int)row["NumericScale"];
            m_nullable = (bool)row["AllowDBNull"];
            m_length = (Type == esriFieldType.esriFieldTypeGeometry ? 0 : (int)row["ColumnSize"]);
            //esriFieldTypeGeometry: length=0
            if (Type == esriFieldType.esriFieldTypeGeometry)
            {
                m_length = 0;
            }
            //esriFieldTypeDouble: SchemaTable not contains precision,scale,length
            if (Type == esriFieldType.esriFieldTypeDouble)
            {
                m_length = 8;
                m_precision = 0;
                m_scale = 0;
            }
        }

        private Layer m_layer;
        private Layer postGisLayer { get { return m_layer; } }

        private int m_id;
        internal int id { get { return m_id; } }

        // Set what type of AO field this is.
        // This depends on the name already being set.
        private void setType(Type type)
        {
            if (Name == postGisLayer.geometryField)
                m_type = esriFieldType.esriFieldTypeGeometry;
            else if (Name == PostGisConstants.idField)
                m_type = esriFieldType.esriFieldTypeOID;
            else
            {
                //PG Int8 (.NET Int64) makes ArcMap crash & PG Numeric (.NET Decimal) takes all null values 
                if (type == typeof(double) || type == typeof(Decimal))
                    m_type = esriFieldType.esriFieldTypeDouble;
                else if (type == typeof(Int32) || type == typeof(Int64))
                    m_type = esriFieldType.esriFieldTypeInteger;
                else if (type == typeof(Int16) || type == typeof(Byte))
                    m_type = esriFieldType.esriFieldTypeSmallInteger;
                else if (type == typeof(string))
                    m_type = esriFieldType.esriFieldTypeString;
            }
        
            // Todo - implement the other fields.
        }

        #region IField
        // Todo - is there a better way to do this?
        public string AliasName
        {
            get { return Name; }
        }

        public bool CheckValue(object Value)
        {
            throw new NotImplementedException();
        }

        private object m_default;
        public object DefaultValue { get { return m_default; } }

        // I don't know what I'm doing here ... but this is what shapefiles do.
        public IDomain Domain
        {
            get { return null; }
        }

        // I don't know what I'm doing here ... but this is what shapefiles do.
        public bool DomainFixed
        {
            get { return false; }
        }

        // The database policies will take care of access rights.
        public bool Editable
        {
            get { return true; }
        }

        // This needs to be the geom type of a shape field, otherwise it's null.
        public IGeometryDef GeometryDef
        {
            get { return null; }
        }

        private bool m_nullable;
        public bool IsNullable
        {
            get { return m_nullable; }
        }

        private int m_length;
        public int Length
        {
            get { return m_length; }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
        }

        private int m_precision;
        public int Precision
        {
            get { return m_precision; }
        }

        // Todo - is the following correct?
        public bool Required
        {
            get { return !IsNullable; }
        }

        private int m_scale;
        public int Scale
        {
            get { return m_scale; }
        }

        private esriFieldType m_type;
        public esriFieldType Type { get { return m_type; } }

        // Todo - figure this out.
        public int VarType
        {
            get
            {
                int retVal = 1; // Null
                switch (Type)
                {
                    case esriFieldType.esriFieldTypeString:
                        retVal = 8;  // vbString
                        break;

                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSmallInteger:
                    case esriFieldType.esriFieldTypeOID:
                        retVal = 3;  // vbLong
                        break;

                    case esriFieldType.esriFieldTypeGeometry:
                        retVal = 13;  // vbDataObject
                        break;
                }
                return retVal;
            }
        }
        #endregion

		/*
		#region IField2

		public IRasterDef RasterDef
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion

		#region IModelInfo

		public string ModelName
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
		*/
	}

    [Guid("C9AA8D6E-9706-42a9-A400-D74B1C664A07"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisSelectionSet :
        ISelectionSet,
        ISelectionSet2
    {

		/// <summary>
		/// Create an empty SelectionSet
		/// </summary>
		/// <param name="postGisFeatureClass"></param>
        public PostGisSelectionSet(PostGisFeatureClass postGisFeatureClass) : this(postGisFeatureClass, null)
        {
			System.Diagnostics.Debug.WriteLine("Empty PostGisSelectionSet...");
			m_featClass = postGisFeatureClass;
        }
        
		/// <summary>
		/// Create a SelectionSet from a dataReader
		/// </summary>
		/// <param name="postGisFeatureClass"></param>
		/// <param name="dataReader"></param>
        public PostGisSelectionSet(PostGisFeatureClass postGisFeatureClass, AutoDataReader dataReader)
        {
			System.Diagnostics.Debug.WriteLine("NOT-Empty PostGisSelectionSet...");
			m_featClass = postGisFeatureClass;
			//oids.Add(1);
            m_dr = dataReader;
            if (dataReader != null)
            {
				while (dataReader.Read())
				{
					oids.Add((object)dataReader[PostGisConstants.idField]);
				}
				dataReader.Close();
            }
        }

        private PostGisFeatureClass m_featClass;
        private PostGisFeatureClass postGisFeatureClass { get { return m_featClass; } }

        private AutoDataReader m_dr;
        private AutoDataReader dataReader { get { return m_dr; } }

        private ArrayList m_oids = new ArrayList();
        public ArrayList oids { get { return m_oids; } }

        #region ISelectionSet

		/// <summary>
		/// Add an OID to the OIDS ISelectionSet
		/// </summary>
		/// <param name="OID"></param>
        public void Add(int OID)
        {
            oids.Add(OID);
        }

        /// <summary>
		/// Add an OID list from the OIDS ISelectionSet (Paolo, march 2007)
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="OIDList"></param>
        public void AddList(int Count, ref int OIDList)
        {
			//first let's clear the OIDS
			oids.Clear();
			int[] ids = ConvertUnmanagedArray(ref OIDList, Count);
			for (int i = 0; i < Count; i++)
			{
				oids.Add((object)ids[i]);
			}
        }

		/// <summary>
		/// Combine selections (Paolo, march 2007)
		/// </summary>
		/// <param name="otherSet">current selectionset</param>
		/// <param name="setOp">selection operation</param>
		/// <param name="resultSet">output selectionset</param>
		public void Combine(ISelectionSet otherSet, esriSetOperation setOp, out ISelectionSet resultSet)
        {
			resultSet = this as ISelectionSet;
			//esriSetUnion
			IEnumIDs eids = otherSet.IDs;
			for (int i = 0; i < otherSet.Count; i++)
			{
				int oid = eids.Next();
				if (oids.Contains(oid)) //oid already in selectionset
				{
					if (setOp == esriSetOperation.esriSetDifference)
					{
						oids.Remove(oid);
					}
					if (setOp == esriSetOperation.esriSetSymDifference)
					{
						oids.Remove(oid);
					}
				}
				else //oid not in selectionset
				{
					if (setOp == esriSetOperation.esriSetUnion)
					{
						oids.Add(oid);
					}
					if (setOp == esriSetOperation.esriSetIntersection)
					{
						oids.Remove(oid);
					}
					if (setOp == esriSetOperation.esriSetSymDifference)
					{
						oids.Add(oid);
					}
				}
			}
        }

		/// <summary>
		/// Count selected OIDS
		/// </summary>
        public int Count {get { return oids.Count; }}

        // I think this stays this way.  There's "not implemented" in the docs.
        public IName FullName { get { throw new NotImplementedException(); } }

#if STUB_BUILD
        public IEnumIDs IDs { get { return null; } }
#else
        public IEnumIDs IDs { get { return new PostGisEnumIds(this); } }
#endif

        // Todo - implement this.
        public void MakePermanent()
        {
        }

        // Todo - implement this.
        public void Refresh()
        {	
        }

        /// <summary>
        /// Remove an OID list from the OIDS ISelectionSet (Paolo, march 2007)
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="OIDList"></param>
        public void RemoveList(int Count, ref int OIDList)
        {
			int[] ids = ConvertUnmanagedArray(ref OIDList, Count);
			for (int i = 0; i < Count; i++)
			{
				oids.Remove((object)ids[i]);
			}
        }

		/// <summary>
		/// Get an int[] array from an array pointer in unsafe code (Paolo, march 2007, thanks Abe!)
		/// </summary>
		/// <param name="arrayPointer"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		unsafe protected static int[] ConvertUnmanagedArray(ref int
arrayPointer, int size)
		{
			int[] managedArray = new int[size];
			fixed (int* pArray = &arrayPointer)
			{
				for (int i = 0; i < size; ++i)
					managedArray[i] = *(pArray + i);
			}
			return managedArray;
		}

		/// <summary>
		/// Generate a cursor from the selection set (Paolo, march 2007)
		/// </summary>
		/// <param name="pQueryFilter"></param>
		/// <param name="Recycling"></param>
		/// <param name="ppCursor"></param>
		public void Search(IQueryFilter pQueryFilter, bool Recycling, out ICursor ppCursor)
		{
			//we need to include in the search only the oids from the selection
			if (oids.Count > 0)
			{
				string where = PostGisConstants.idField + " in (";
				for (int i = 0; i < oids.Count; i++)
				{
					where += oids[i].ToString();
					if (i < oids.Count - 1)
					{
						where += ",";
					}
				}
				where += ")";
				pQueryFilter.WhereClause = where;
			}
			ITable pTable = m_featClass as ITable;
			ppCursor =  pTable.Search(pQueryFilter, Recycling);
		}

		// Todo - implement this.
		public ISelectionSet Select(IQueryFilter QueryFilter, esriSelectionType selType, esriSelectionOption selOption, IWorkspace selectionContainer)
		{
			return null;
			//throw new NotImplementedException();
		}

		public ITable Target { get { return postGisFeatureClass; } }
		#endregion

		#region ISelectionSet2
		public void Update(IQueryFilter pQueryFilter, bool Recycling, out ICursor ppCursor)
		{
			ppCursor = null;
		}
		#endregion
	}
}
