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
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Threading;
using ZigGis.ArcGIS.Geodatabase;
using ZigGis.PostGis;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
#endif

namespace ZigGis.Utilities
{
    public class Helper
    {
        static public UID getTypeUid(Type type)
        {
            UID uid = new UIDClass();
            uid.Value = "{" + type.GUID.ToString() + "}";
            return uid;
        }

        static public void checkWithinBounds(int index, int length)
        {
            // This is how AO does it.
            if (index > length || index < 0)
                throw new ArgumentException("Value does not fall within the expected range.");
        }

        static public void signalEndOfEnum()
        {
            throw new COMException("", 1);  // Hack to return the S_FALSE success code.
        }

        static public string objectToString(object o)
        {
            if (o == null)
                return "null";
            else
                return o.ToString();
        }
    }

    // This class helps with various conversion tasks.
    // ArcObjects -> WKT
    // WKT -> ArcObjects
    // etc ...
    public class GeomHelper
    {
        static private readonly CLogger log = new CLogger(typeof(GeomHelper));

        static public string aoGeomToWkt(IGeometry geometry, bool includeFromTextMarkup, int srid)
        {
            if (geometry is IPoint)
                return aoPointToWkt((IPoint)geometry, includeFromTextMarkup, srid);
            else if (geometry is IPolyline)
                return aoPolylineToWkt((IPolyline)geometry, includeFromTextMarkup, srid);
            else if (geometry is IPolygon)
                return aoPolygonToWkt((IPolygon)geometry, includeFromTextMarkup, srid);
            else
                return "";
        }

        static private string xyString(double x, double y)
        {
            //local settings issue, OGC is always en-US culture
            CultureInfo OGCCultureInfo = new CultureInfo("en-US");
            StringBuilder sb = new StringBuilder(x.ToString(OGCCultureInfo));
            sb.Append(" ");
            sb.Append(y.ToString(OGCCultureInfo));
            return sb.ToString();
        }

        static public string aoPointToWkt(IPoint point, bool includeFromTextMarkup, int srid)
        {
            StringBuilder sb;
            if (includeFromTextMarkup)
                sb = new StringBuilder("PointFromText('POINT(");
            else
                sb = new StringBuilder("POINT(");
            sb.Append(xyString(point.X, point.Y));
            if (includeFromTextMarkup)
            {
                sb.Append(")',");
                sb.Append(srid.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }

        static public string aoPolylineToWkt(IPolyline polyline, bool includeFromTextMarkup, int srid)
        {
            StringBuilder sb = aoPolyToWtkHelper((IPointCollection)polyline, false);

            /*
            if (multi)
                return (includeFromTextMarkup ? "MLineFromText('" : "") +
                    "MULTILINESTRING(" + sb.ToString() +
                    (includeFromTextMarkup ? "'," + srid.ToString() : "") +
                    ")";
            else
            */
            return (includeFromTextMarkup ? "LineFromText('" : "") +
                "LINESTRING" + sb.ToString() +
                (includeFromTextMarkup ? "'," + srid.ToString() : "") +
                ")";
        }

        static public string aoPolygonToWkt(IPolygon polygon, bool includeFromTextMarkup, int srid)
        {
            StringBuilder sb = aoPolyToWtkHelper((IPointCollection)polygon, true);

            /*
            if (multi)
                return (includeFromTextMarkup ? "MPolyFromText('" : "") +
                    "MULTIPOLYGON(" + sb.ToString() +
                    (includeFromTextMarkup ? "'," + srid.ToString() : "") +
                    ")";
            else
            */
            return (includeFromTextMarkup ? "PolyFromText('" : "") +
                "POLYGON" + sb.ToString() +
                (includeFromTextMarkup ? "'," + srid.ToString() : "") +
                ")";
        }

        static private StringBuilder aoPolyToWtkHelper(IPointCollection points, bool isPolygon)
        {
            IEnumVertex vertices = points.EnumVertices;
            StringBuilder sbMain = new StringBuilder("(");

            IPoint p;
            IPoint firstInPart = new PointClass();
            int pIdx;
            int vIdx;
            int pIdxCurr = 0;  // The current part index.
            StringBuilder sbPart = new StringBuilder();
            while ((p = nextPoint(vertices, out pIdx, out vIdx)) != null)
            {
                // Have we gotten a new part?
                // If so, close it out.
                if (pIdx != pIdxCurr)
                {
                    // We need to close this ring if it's a polygon.
                    if (isPolygon)
                    {
                        aoPolyToWktAddPoint(sbPart, firstInPart);
                        replaceLastChar(sbPart, ')');
                    }

                    aoPolyToWktAddPart(sbMain, sbPart);
                    sbMain.Append(",");
                    sbPart = new StringBuilder();
                    pIdxCurr = pIdx;
                }

                aoPolyToWktAddPoint(sbPart, p);

                // Save the first point in each part to later close polygons.
                if (vIdx == 0)
                    firstInPart.PutCoords(p.X, p.Y);
            }

            // Close the last part of a polygon.
            if (isPolygon)
                aoPolyToWktAddPoint(sbPart, firstInPart);
            aoPolyToWktAddPart(sbMain, sbPart);

            sbMain.Append(")");

            return sbMain;
        }

        static private void aoPolyToWktAddPoint(StringBuilder part, IPoint pointToAdd)
        {
            part.Append(xyString(pointToAdd.X, pointToAdd.Y));
            part.Append(",");
        }

        static private void aoPolyToWktAddPart(StringBuilder multipart, StringBuilder partToAdd)
        {
            replaceLastChar(partToAdd, ')');
            multipart.Append("(");
            multipart.Append(partToAdd.ToString());
        }

        static private void replaceLastChar(StringBuilder stringBuilder, char c)
        {
            stringBuilder[stringBuilder.Length - 1] = c;
        }

        static private IPoint nextPoint(IEnumVertex vertices, out int partIndex, out int vertexIndex)
        {
            IPoint p;
            vertices.Next(out p, out partIndex, out vertexIndex);
            return p;
        }

        /// <summary>
        /// Generate a query for PostGIS from an IQueryFilter for a Layer
        /// </summary>
        /// <param name="query"></param>
        /// <param name="postGisLayer"></param>
        /// <param name="fields">output fields for the query</param>
        /// <param name="where">output where clause for the query</param>
        static public void aoQryToPostGisQry(IQueryFilter query, Layer postGisLayer, out string fields, out string where)
        {
            log.enterFunc("aoQryToPostGisQry");
            if (log.IsDebugEnabled) log.Debug(Helper.objectToString(query) + "," + Helper.objectToString(postGisLayer));

            // Todo - must use the IQueryFilter::OutputSpatialReference
            // to project the resulting geometries.
            fields = "";
            where = "";

            try
            {
                // Get the SQL stuff.
                fields = query.SubFields;

                // Add the spatial stuff.
                ISpatialFilter sQuery = query as ISpatialFilter;


                //Paolo: FactoryCode=0 for srid=-1
                int outSrid = postGisLayer.SpatialReference.FactoryCode;
                if (outSrid == 0)
                {
                    outSrid = -1;
                }
                //if outSrid=-1, could be that the Sr is set to Unknown, but srid is != -1 (for PostGIS layers with a srid not supported from esri, ex: 27563)
                if (outSrid == -1 && postGisLayer.srid > 0)
                {
                    outSrid = postGisLayer.srid;
                }

                aoFieldsToPostGisFields(ref fields, postGisLayer, outSrid);
                StringBuilder sb = new StringBuilder(query.WhereClause);
                //if there is a spatial filter do the following...
                if (sQuery != null)
                {
                    /*
                    ISpatialReference sRef = sQuery.Geometry.SpatialReference;
                    if (sRef != null && sRef.FactoryCode != 0)
                        srid = sRef.FactoryCode;

                    aoFieldsToPostGisFields(ref fields, postGisLayer, srid);
                    */

                    // Debug - just create a polygon from the envelope for now.
                    object o = System.Type.Missing;
                    IEnvelope env = sQuery.Geometry.Envelope;
                    IPointCollection pg = (IPointCollection)(new PolygonClass());
                    pg.AddPoint(env.LowerLeft, ref o, ref o);
                    pg.AddPoint(env.LowerRight, ref o, ref o);
                    pg.AddPoint(env.UpperRight, ref o, ref o);
                    pg.AddPoint(env.UpperLeft, ref o, ref o);

                    if (sb.Length > 0)
                        sb.Append(" and ");

                    //Paolo: for srid=-1 not doing transform
                    sb.Append("intersects(");
                    sb.Append("transform(");
                    sb.Append(aoPolygonToWkt((IPolygon)pg, true, outSrid));
                    sb.Append("," + outSrid.ToString() + "),");
                    sb.Append("transform(" + postGisLayer.geometryField + "," + outSrid.ToString() + ")");
                    sb.Append(")=true");
                }
                where = sb.ToString();
                //System.Diagnostics.EventLog.WriteEntry("GeomHelper.aoQryToPostGisQry", where, System.Diagnostics.EventLogEntryType.Information);

                if (log.IsDebugEnabled)
                {
                    log.Debug(fields);
                    log.Debug(where);
                }
            }
            catch (Exception ex)
            {
                log.Debug("GeomHelper.aoQryToPostGisQry", ex);
            }
            finally
            {
                log.leaveFunc();
            }
        }

        static public void aoFieldsToPostGisFields(ref string fields, Layer postGisLayer, int outSrid)
        {
            // We want to load every field so we can easily
            // interoperate with the rest of the framework.
            // However, load nulls for the fields that aren't specified.

            log.enterFunc("aoFieldsToPostGisFields");
            bool loadAll = (fields == "*");
            log.Debug("outSrid = " + outSrid.ToString());

            string[] fieldArray = fields.Split(',');
            Hashtable fieldMap = new Hashtable(fieldArray.Length);
            foreach (string f in fieldArray)
                fieldMap.Add(f.ToLower().Trim(), true);  // Use a dummy value. (Paolo: I added a Trim)
            string name;
            bool load;
            StringBuilder sb = new StringBuilder();
            DataTable dataFields = postGisLayer.getDataFields(false);
            foreach (DataRow r in dataFields.Rows)
            {
                name = ((string)r["ColumnName"]).ToLower();

                // Are we loading the data?
                // (if the field is found in the Hashtable
                // then the field was supplied to this
                // function, therefore we load its data)
                // We also load if we are loading all.
                load = (fieldMap[name] != null || loadAll);

                // Geometry fields are special - add the asbinary() func.
                // Plus, we *always* load the geometry data.
                // DONE: apply correct coordinate transformation here
                if (name == postGisLayer.geometryField)
                    name = postGisLayer.transformGeometryFieldAsBinary(outSrid) + " as " + name;
                else if (!load)
                {
                    sb.Append("null as ");
                }

                sb.Append(name);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            fields = sb.ToString();
            log.leaveFunc();
        }

        static public ISpatialReference setEsriSpatiaReferenceFromSrText(int srid, Connection conn)
        {
            ISpatialReference sr = new UnknownCoordinateSystemClass();
            string srText = "";
            int i = 0;
            try
            {
                //Bill: query srtext associated with srid
                AutoDataReader dr = conn.doQuery("select * from spatial_ref_sys where srid = " + srid.ToString());
                if (dr.Read())
                {
                    srText = dr["srtext"] + "";
                    ISpatialReferenceFactory2 srf = new SpatialReferenceEnvironmentClass();
                    if (srText == "")
                    {
                        sr = new UnknownCoordinateSystemClass();
                    }
                    else
                    {
                        //use srText to construct SR.
                        srf.CreateESRISpatialReference(srText, out sr, out i);
                    }
                }
                return sr;
            }
            catch
            {
                //PostGis srid is not implemented as an Esri Factory Code
                sr = new UnknownCoordinateSystemClass();
                return sr;
            }
        }
        static public ISpatialReference setEsriSpatiaReferenceFromSrid(int srid)
        {
            //there is not always a perfect corrispondence between PostGIS srid and Esri Factory code: so we have to catch the possible error in CreateSpatialReference, and in that case set it to Unknow (it won't be possible to project layers, but at least it will be displayed)
            ISpatialReference sr;
            try
            {
                //Paolo : set Spatial Reference
                ISpatialReferenceFactory2 srf = new SpatialReferenceEnvironmentClass();
                if (srid == -1)
                {
                    sr = new UnknownCoordinateSystemClass();
                }
                else
                {
                    sr = srf.CreateSpatialReference(srid);
                }
                return sr;
            }
            catch
            {
                //PostGis srid is not implemented as an Esri Factory Code
                sr = new UnknownCoordinateSystemClass();
                return sr;
            }
        }

    }

    enum BitConversion { toLittle, toBig, none };

    public class WkbParser
    {
        private GeometryEnvironmentClass m_geoEnv = new GeometryEnvironmentClass();
        private GeometryEnvironmentClass geometryEnvironment { get { return m_geoEnv; } }

        public IGeometry parseWkb(byte[] wkb)
        {
            // Get the bit order conversion.
            BitConversion convertType = getConversionType(wkb);

            // Get the geometry type.
            UInt32 tempInt;
            int i = 1;
            getWkbUInt32(wkb, ref i, convertType, out tempInt);
            wkbGeometryType geomType = (wkbGeometryType)tempInt;

            IGeometry geom;
            switch (geomType)
            {
                // AO has trouble parsing MultiLinestrings from PostGis.
                // So we need to do it by hand.
                case wkbGeometryType.wkbMultiLinestring:
                    geom = parseAoUnsafeWkb(wkb, convertType, geomType);
                    break;

                // All other geometries AO can handle.
                default:
                    geom = parseAoSafeWkb(wkb);
                    break;
            }

            return geom;
        }

        private IGeometry parseAoSafeWkb(byte[] wkb)
        {
            IGeometry geom;
            int bytesRead;
            geometryEnvironment.CreateGeometryFromWkbVariant(wkb, out geom, out bytesRead);
            return geom;
        }

        static private IGeometry parseAoUnsafeWkb(byte[] wkb, BitConversion bitConversion, wkbGeometryType geometryType)
        {
            IGeometry geom = null;

            // Keeps track of what byte we're at.
            // (assume the byte order and geometry
            // type data has already been read)
            int i = 5;

            switch (geometryType)
            {
                case wkbGeometryType.wkbMultiLinestring:
                    createWkbMultiLineString(wkb, ref i, bitConversion, out geom);
                    break;
            }

            return geom;
        }

        static void createWkbMultiLineString(byte[] wkb, ref int startIndex, BitConversion bitConversion, out IGeometry geometry)
        {
            geometry = new PolylineClass();
            IPointCollection mp = (IPointCollection)geometry;

            // Get the number of line strings.
            UInt32 lineStringCnt;
            getWkbUInt32(wkb, ref startIndex, bitConversion, out lineStringCnt);

            UInt32 pointCnt;
            double x, y;
            object missing = Type.Missing;
            // Loop through each LineString.
            for (int i = 0; i < lineStringCnt; i++)
            {
                startIndex += 5;  // Jump past useless header stuff.
                getWkbUInt32(wkb, ref startIndex, bitConversion, out pointCnt);

                IPointCollection tempPc = new MultipointClass();
                // Loop through each point.
                for (int j = 0; j < pointCnt; j++)
                {
                    getWkbDouble(wkb, ref startIndex, bitConversion, out x);
                    getWkbDouble(wkb, ref startIndex, bitConversion, out y);
                    tempPc.AddPoint(createAoPoint(x, y), ref missing, ref missing);
                }
                mp.AddPointCollection(tempPc);
            }
        }

        static IPoint createAoPoint(double x, double y)
        {
            IPoint retVal = new PointClass();
            retVal.PutCoords(x, y);
            return retVal;
        }

        static BitConversion getConversionType(byte[] wkb)
        {
            BitConversion retVal = BitConversion.none;

            // Big to Little.
            if (BitConverter.IsLittleEndian && wkb[0] == 0)
                retVal = BitConversion.toLittle;
            // Little to Big.
            else if (!BitConverter.IsLittleEndian && wkb[0] == 1)
                retVal = BitConversion.toBig;

            return retVal;
        }

        static void getWkbUInt32(byte[] wkb, ref int startIndex, BitConversion conversionType, out UInt32 theUInt32)
        {
            switch (conversionType)
            {
                case BitConversion.toLittle:
                    theUInt32 = 0;
                    break;

                case BitConversion.toBig:
                    theUInt32 = 0;
                    break;

                case BitConversion.none:
                default:
                    theUInt32 = BitConverter.ToUInt32(wkb, startIndex);
                    break;
            }
            startIndex += 4;
        }

        static void getWkbDouble(byte[] wkb, ref int startIndex, BitConversion conversionType, out double theDouble)
        {
            switch (conversionType)
            {
                case BitConversion.toLittle:
                    theDouble = 0;
                    break;

                case BitConversion.toBig:
                    theDouble = 0;
                    break;

                case BitConversion.none:
                default:
                    theDouble = BitConverter.ToDouble(wkb, startIndex);
                    break;
            }
            startIndex += 8;
        }
    }
}
