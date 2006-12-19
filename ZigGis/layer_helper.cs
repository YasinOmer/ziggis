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

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
#endif

namespace ZigGis.Utilities
{
    // The classes help to deal with a few AO items
    // with polymorphism as opposed to using if / switch
    // statements everywhere.

    abstract public class CLayerHelperBase
    {
        abstract public void draw(IDisplay display, IGeometry geometry);
        
        abstract public esriGeometryType shapeType { get;}
        
        // Construct a C<foo>LayerHelper object according to its PostGIS type.
        static public CLayerHelperBase createLayerHelper(string postGisGeometryType)
        {
            CLayerHelperBase retVal = null;
            switch (postGisGeometryType.ToLower())
            {
                case "point":
                    retVal = new CPointLayerHelper();
                    break;
                
                case "linestring":
                case "multilinestring":
                    retVal = new CPolylineLayerHelper();
                    break;
                
                case "polygon":
                case "multipolygon":
                    retVal = new CPolygonLayerHelper();
                    break;
            }

            return retVal;
        }
    }

    public class CPolygonLayerHelper : CLayerHelperBase
    {
        public override void draw(IDisplay display, IGeometry geometry)
        {
            display.DrawPolygon(geometry);
        }

        public override esriGeometryType shapeType {get { return esriGeometryType.esriGeometryPolygon; }}
    }

    public class CPolylineLayerHelper : CLayerHelperBase
    {
        public override void draw(IDisplay display, IGeometry geometry)
        {
            display.DrawPolyline(geometry);
        }

        public override esriGeometryType shapeType { get { return esriGeometryType.esriGeometryPolyline; } }
    }

    public class CPointLayerHelper : CLayerHelperBase
    {
        public override void draw(IDisplay display, IGeometry geometry)
        {
            display.DrawPoint(geometry);
        }

        public override esriGeometryType shapeType { get { return esriGeometryType.esriGeometryPoint; } }
    }
}
