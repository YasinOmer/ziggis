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
using System.Data;
using System.Text;
using ZigGis.Utilities;
using ESRI.ArcGIS.Geometry;

namespace ZigGis.PostGis
{
    public class Layer
    {
        static private readonly CLogger log = new CLogger(typeof(Layer));
        
        private Layer(Connection connection)
        {
            m_con = connection;
        }
        
        internal Layer(Connection connection, string schema, string view) : this(connection)
        {
            m_schema = schema;
            m_view = view;

            loadLayer(false);
        }

        internal Layer(Connection connection, int oid) : this(connection)
        {
            m_oid = oid;

            loadLayer(true);
        }

        private string m_schema = "";
        public string schema { get { return m_schema; } }

        private string m_view = "";
        public string view { get { return m_view; } }

        private Connection m_con;
        public Connection connection { get { return m_con; } }

        private int m_oid = -1;
        public int oid { get { return m_oid; } }

        // Gets the different layer attributes stored by PostGis.
        private void loadLayer(bool loadFromOid)
        {
            log.enterFunc("loadLayer");
            
            // Create a query that selects layer attributes from the geometry columns
            // table and selects the spatial reference from the spatial reference table.
            // The join must be constructed such that a record is gotten whether this
            // layer has a spatial reference or not (hence the left join).
            string where;
            if (loadFromOid)
                where = "g.oid=" + oid.ToString();
            else
                where = PostGisConstants.schemaField + "=" + DbHelper.quote(schema) +
                            " and " + PostGisConstants.tableField + "=" + DbHelper.quote(view);
            
            string sql = DbHelper.createSelectSql(PostGisConstants.geometryColumnsTable + " as g",
                            "g.*,g.oid,s." + PostGisConstants.spatialReferenceSrField,
                            "left join " + PostGisConstants.spatialReferenceTable + " as s on g." +
                            PostGisConstants.spatialReferenceIdField + "=s." +
                            PostGisConstants.spatialReferenceIdField, where, null);
            
            AutoDataReader dr = connection.doQuery(sql);
            if (dr.Read())
            {
                m_geomFld = DbHelper.getValueAsString(dr[PostGisConstants.geometryColumnLookupField]);
                m_geomType = DbHelper.getValueAsString(dr[PostGisConstants.geometryTypeField]);
                m_spatialRefText = DbHelper.getValueAsString(dr[PostGisConstants.spatialReferenceSrField]);
				m_srid = (int)dr[PostGisConstants.spatialReferenceIdField];

				m_spatialReference = GeomHelper.setEsriSpatiaReferenceFromSrid(m_srid);
				if (m_spatialReference.FactoryCode == 0 && m_srid != -1)
				{
					//PostGis srid is not implemented as an Esri Factory Code
					System.Windows.Forms.MessageBox.Show("PostGis srid is not implemented as an Esri Factory Code: this PostGis table can not be reprojected in ArcMap.");
				}

                if (loadFromOid)
                {
                    m_schema = DbHelper.getValueAsString(dr[PostGisConstants.schemaField]);
                    m_view = DbHelper.getValueAsString(dr[PostGisConstants.tableField]);
                }
                else
                    m_oid = (int)((Int64)dr["oid"]);
            }
            else
            {
                // Todo - throw exception.
            }
			//Initialize spatial reference
            log.leaveFunc();
        }

        private string m_geomFld;
        public string geometryField { get { return m_geomFld; } }

        private string m_geomType;
        public string geometryType { get { return m_geomType; } }

        private string m_spatialRefText;
        public string spatialReferenceText { get { return m_spatialRefText; } }

		//Paolo - we need to syncronize srid and spatial reference with the featurelayer!
		private ISpatialReference m_spatialReference;
		public ISpatialReference SpatialReference { get { return m_spatialReference; } }

		private int m_srid = -1;
		public int srid { get { return m_srid; } }

        public string schemaAndView
        {
            get
            {
                StringBuilder sb = new StringBuilder(schema);
                sb.Append(".");
                sb.Append(view);
                return sb.ToString();
            }
        }

        public AutoDataReader doQuery(string fields, string where)
        {
            string sql = DbHelper.createSelectSql(schemaAndView, fields, where);
            return connection.doQuery(sql);
        }

        // Returns the amount of records 
        public int getRecordCount(string where)
        {
            string sql = DbHelper.createSelectSql(schemaAndView, "count(" + geometryField + ")", where);
            return (int)((Int64)connection.doScalarQuery(sql));
        }
    
        // Returns the record for the given Id.
        public IDataRecord getRecord(int id)
        {
            string sql = DbHelper.createSelectSql(schemaAndView, "*", "gid=" + id.ToString());
            AutoDataReader dr = connection.doQuery(sql);
            if (!dr.Read())
                dr = null;
            return dr;
        }

		// Paolo (Returns the record for the given Id with corrected geometry)
		public IDataRecord getRecord(string fields, string where)
		{
			string sql = DbHelper.createSelectSql(schemaAndView, fields, where);
			AutoDataReader dr = connection.doQuery(sql);
			if (!dr.Read())
				dr = null;
			return dr;
		}

        // Returns the fields for the table of this layer.
        private DataTable m_fields = null;
        public DataTable getDataFields(bool reloadFromDatabase)
        {
            log.enterFunc("getDataFields");

            // reloadFromDatabase - true to refresh the cache from the database.
            if (m_fields == null || reloadFromDatabase)
            {
                if (log.IsDebugEnabled) log.Debug("1");
                // Query the view for its fields and store them in the array.
				// Paolo: gid is not always in the PostGIS layer: let's use limit 1 instead than gid=-1
				string sql = DbHelper.createSelectSql(schemaAndView, "*", null, null, "limit 1");
                if (log.IsDebugEnabled) log.Debug(sql);
                using (AutoDataReader dr = connection.doQuery(sql))
                {
                    log.Debug(dr == null ? "null" : "not null");
                    log.Debug("2");
                    m_fields = dr.GetSchemaTable();
                }
            }

			log.leaveFunc();

            return m_fields;
        }

        public byte [] wkbExtent
        {
            get
            {
                byte [] retVal = null;
				//TODO we should transform the extent here, according to srid (Paolo)
                string sql = DbHelper.createSelectSql(schemaAndView, "asbinary(extent(" + geometryField + "))");
                using (AutoDataReader dr = connection.doQuery(sql))
                {
                    if (dr.Read())
                        retVal = (byte[])dr[0];
                }
                return retVal;
            }
        }

        public string geometryFieldAsBinary { get { return "asbinary(" + geometryField + ")"; } }

		//Bill Dollins: Added to support transform
		public string transformGeometryFieldAsBinary(int toSrid)
		{
			return "asbinary(transform(" + geometryField + "," + toSrid.ToString() + "))";
		}

        public string geometryBinaryName { get { return geometryField + "_asbinary"; } }
    }
}
