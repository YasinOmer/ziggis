using System;
using System.Data;
using Npgsql;
using System.Text;
using ZigGis.Utilities;
using ESRI.ArcGIS.Catalog;

namespace ZigGis.PostGis
{
    public class GxPostGisDatabaseFolder :
        IGxCachedObjects,
        IGxDataElement,
        IGxDataElementHelper,
        IGxObject,
        IGxObjectContainer,
        IGxObjectEdit,
        IGxObjectProperties,
        IGxObjectUI,
        IGxPasteTarget,
        IGxRemoteContainer
    {
        #region IGxCachedObjects Members

        public void LoadCachedObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReleaseCachedObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxDataElement Members

        public ESRI.ArcGIS.Geodatabase.IDataElement GetDataElement(ESRI.ArcGIS.Geodatabase.IDEBrowseOptions pBrowseOptions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxDataElementHelper Members

        public void RetrieveDEBaseProperties(ESRI.ArcGIS.Geodatabase.IDataElement ppDataElement)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RetrieveDEFullProperties(ESRI.ArcGIS.Geodatabase.IDataElement ppDataElement)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObject Members

        public void Attach(IGxObject Parent, IGxCatalog pCatalog)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string BaseName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Category
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ESRI.ArcGIS.esriSystem.UID ClassID
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Detach()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string FullName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ESRI.ArcGIS.esriSystem.IName InternalObjectName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsValid
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IGxObject Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Refresh()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectContainer Members

        public IGxObject AddChild(IGxObject child)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool AreChildrenViewable
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IEnumGxObject Children
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void DeleteChild(IGxObject child)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasChildren
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IGxObjectEdit Members

        public bool CanCopy()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanDelete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CanRename()
        {
            throw new Exception("The method or operation is not implemented.");
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

        #region IGxObjectProperties Members

        public void GetPropByIndex(int index, ref string pName, ref object pValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetProperty(string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int PropertyCount
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void SetProperty(string Name, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGxObjectUI Members

        public ESRI.ArcGIS.esriSystem.UID ContextMenu
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

        public ESRI.ArcGIS.esriSystem.UID NewMenu
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

        #region IGxPasteTarget Members

        public bool CanPaste(ESRI.ArcGIS.esriSystem.IEnumName names, ref bool moveOperation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Paste(ESRI.ArcGIS.esriSystem.IEnumName names, ref bool moveOperation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}