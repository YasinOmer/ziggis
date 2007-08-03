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