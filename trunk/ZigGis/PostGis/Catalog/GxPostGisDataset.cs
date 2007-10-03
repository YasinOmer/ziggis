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
    [Guid("381f654a-fb00-4f98-a96b-eee9e0e33ac7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ZigGis.GxPostGisDataset")]
    public class GxPostGisDataset : GxDatasetClass
    {
        private IDataset _ds;
        public GxPostGisDataset(IDataset ds)
        {
            _ds = ds;
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
                uid.Value = "{381f654a-fb00-4f98-a96b-eee9e0e33ac7}";
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

        public override IDataset Dataset
        {
            get
            {
                return _ds;
            }
        }

        public override IDatasetName DatasetName
        {
            get
            {
                return base.DatasetName;
            }
            set
            {
                base.DatasetName = value;
            }
        }

        public override esriDatasetType Type
        {
            get
            {
                return esriDatasetType.esriDTFeatureClass;
            }
        }

        public override string Name
        {
            get
            {
                return _ds.Name;
            }
        }

        public override string FullName
        {
            get
            {
                return _ds.Name;
            }
        }
   }
}
