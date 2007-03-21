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
using System.Data;
using System.Collections;
using ZigGis.Utilities;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Geodatabase;
#endif

namespace ZigGis.ArcGIS.Geodatabase
{
    [Guid("97B53923-9AC9-47c2-9D8C-8BB3216FEC93"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisFeatureCursor :
        IFeatureCursor,
        ICursor
    {
        public PostGisFeatureCursor(PostGisFeatureClass postGisFeatureClass, IDataReader dataReader)
        {
            m_featClass = postGisFeatureClass;
            m_dr = dataReader;
        }

        private PostGisFeatureClass m_featClass;
        private PostGisFeatureClass postGisFeatureClass { get { return m_featClass; } }

        private IDataReader m_dr;
        private IDataReader dataReader { get { return m_dr; } }

        #region IFeatureCursor
        public void DeleteFeature()
        {
        }

        public IFields Fields { get { return postGisFeatureClass.Fields; } }

        public int FindField(string Name)
        {
            return Fields.FindField(Name);
        }

        public void Flush()
        {
        }

        public object InsertFeature(IFeatureBuffer Buffer)
        {
            return null;
        }

        public IFeature NextFeature()
        {
            IFeature retVal = null;
			if (dataReader.Read())
				retVal = new PostGisFeature(postGisFeatureClass, dataReader);
			else
			{
				Helper.signalEndOfEnum();
			}

            return retVal;
        }

        public void UpdateFeature(IFeature Object)
        {
        }
        #endregion

        #region ICursor
        public void DeleteRow()
        {
        }

        public object InsertRow(IRowBuffer Buffer)
        {
            return null;
        }

        public IRow NextRow()
        {
            return (IRow)NextFeature();
        }

        public void UpdateRow(IRow Row)
        {
        }
        #endregion
    }

    [Guid("B8BFBB71-6544-4e2f-B52D-3451334494BB"), ClassInterface(ClassInterfaceType.None)]
    public class PostGisEnumIds : IEnumIDs
    {
        static private readonly CLogger log = new CLogger(typeof(PostGisEnumIds));

        public PostGisEnumIds(PostGisSelectionSet postGisSelectionSet)
        {
            m_ss = postGisSelectionSet;
            Reset();
        }

        private PostGisSelectionSet m_ss;
        private PostGisSelectionSet postGisSelectionSet { get { return m_ss; } }

        private IEnumerator m_enum;
        private IEnumerator enumerator { get { return m_enum; } }

        #region IEnumIDs
        public int Next()
        {
            //log.enterFunc("Next");

            int retVal = -1;
            try
            {
                if (enumerator.MoveNext())
                    retVal = (int)enumerator.Current;
                else
                {
                    log.Debug("End of enum.");
                    Helper.signalEndOfEnum();
                }
            }
            finally
            {
                //if (log.IsDebugEnabled) log.Debug(retVal);
                //log.leaveFunc();
            }
            return retVal;
        }

        public void Reset()
        {
            m_enum = postGisSelectionSet.oids.GetEnumerator();
        }
        #endregion
    }
}
