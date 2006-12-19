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

namespace ZigGis.PostGis
{
    public class PostGisConstants
    {
        public static string geometryColumnsTable { get { return "geometry_columns"; } }
        public static string spatialReferenceTable { get { return "spatial_ref_sys"; } }
        public static string geometryColumnLookupField { get { return "f_geometry_column"; } }
        public static string geometryTypeField { get { return "type"; } }
        public static string schemaField { get { return "f_table_schema"; } }
        public static string tableField { get { return "f_table_name"; } }
        public static string spatialReferenceIdField { get { return "srid"; } }
        public static string spatialReferenceSrField { get { return "srtext"; } }
        public static string idField { get { return "gid"; } }
    }    
}
