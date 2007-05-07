using System;
using System.IO;
using System.Runtime.InteropServices;
using ZigGis.Utilities;


#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
#endif

namespace ZigGis.ArcGIS.Geodatabase
{
    // This class is mostly a wrapper class for the FeatureLayer class.
    // This just allows us to control the behavior of IPersistStream
    [Guid("2593D36C-BE12-4217-A346-C9446AB13ABC")]
    public class PostGisFeatureLayer :
        IAttributeTable,
        IClass,
        IConnectionPointContainer,
        IDataLayer,
        IDataLayer2,
        IDataset,
        IDisplayAdmin,
        IDisplayFilterManager,
        IDisplayRelationshipClass,
        IDisplayTable,
        IFeatureLayer,
        IFeatureLayer2,
        IFeatureLayerDefinition,
		//IFeatureLayerDefinition2, //Paolo
        IFeatureSelection,
		IFeatureLayerSelectionEvents, //Paolo
        IFind,
        IGeoDataset,
        IGeoFeatureLayer,
        IHotlinkContainer,
        IHotlinkMacro,
        IHyperlinkContainer,
        IIdentify,
        IIdentify2,
        ILayer,
        ILayer2,
        ILayerDrawingProperties,
        ILayerEffects,
        ILayerExtensions,
		ILayerEvents, //Paolo
        ILayerFields,
		//ILayerGeneralProperties, //Paolo
        ILayerInfo,
        ILayerPosition,
		//ILayerSymbologyExtents, //Paolo
        ILegendInfo,
        IMapLevel,
        IObjectClassSchemaEvents,
        IPersistStream,
        IPropertySupport,
        IPublishLayer,
        IRelationshipClassCollection,
        IRelationshipClassCollectionEdit,
		//ISymbolLevels, //Paolo
        ITable,
        ITableDefinition,
        ITableFields,
        ITableSelection
		//ITemporaryLayer //Paolo

    {
        private IFeatureLayer m_layer = new FeatureLayer();
        protected IFeatureLayer featureLayer { get { return m_layer; } }

        #region IAttributeTable
        ITable IAttributeTable.AttributeTable
        {
            get { return ((IAttributeTable)featureLayer).AttributeTable; }
        }
        #endregion

        #region IClass
        void IClass.AddField(IField Field)
        {
            ((IClass)featureLayer).AddField(Field);
        }

        void IClass.AddIndex(IIndex Index)
        {
            ((IClass)featureLayer).AddIndex(Index);
        }

        UID IClass.CLSID
        {
            get { return ((IClass)featureLayer).CLSID; }
        }

        void IClass.DeleteField(IField Field)
        {
            ((IClass)featureLayer).DeleteField(Field);
        }

        void IClass.DeleteIndex(IIndex Index)
        {
            ((IClass)featureLayer).DeleteIndex(Index);
        }

        UID IClass.EXTCLSID
        {
            get { return ((IClass)featureLayer).EXTCLSID; }
        }

        object IClass.Extension
        {
            get { return ((IClass)featureLayer).Extension; }
        }

        IPropertySet IClass.ExtensionProperties
        {
            get { return ((IClass)featureLayer).ExtensionProperties; }
        }

        IFields IClass.Fields
        {
            get { return ((IClass)featureLayer).Fields; }
        }

        int IClass.FindField(string Name)
        {
            return ((IClass)featureLayer).FindField(Name);
        }

        bool IClass.HasOID
        {
            get { return ((IClass)featureLayer).HasOID; }
        }

        IIndexes IClass.Indexes
        {
            get { return ((IClass)featureLayer).Indexes; }
        }

        string IClass.OIDFieldName
        {
            get { return ((IClass)featureLayer).OIDFieldName; }
        }
        #endregion

        #region IConnectionPointContainer
        void IConnectionPointContainer.EnumConnectionPoints(out IEnumConnectionPoints ppEnum)
        {
            ((IConnectionPointContainer)featureLayer).EnumConnectionPoints(out ppEnum);
        }

        void IConnectionPointContainer.FindConnectionPoint(ref Guid riid, out IConnectionPoint ppCP)
        {
            ((IConnectionPointContainer)featureLayer).FindConnectionPoint(ref riid, out ppCP);
        }
        #endregion

        #region IDataLayer
        bool IDataLayer.Connect(IName pOptRepairName)
        {
            return ((IDataLayer)featureLayer).Connect(pOptRepairName);
        }

        IName IDataLayer.DataSourceName
        {
            get
            {
                return ((IDataLayer)featureLayer).DataSourceName;
            }
            set
            {
                ((IDataLayer)featureLayer).DataSourceName = value;
            }
        }

        string IDataLayer.RelativeBase
        {
            get
            {
                return ((IDataLayer)featureLayer).RelativeBase;
            }
            set
            {
                ((IDataLayer)featureLayer).RelativeBase = value;
            }
        }

        bool IDataLayer.get_DataSourceSupported(IName Name)
        {
            return ((IDataLayer)featureLayer).get_DataSourceSupported(Name);
        }
        #endregion

        #region IDataLayer2
        bool IDataLayer2.Connect(IName pOptRepairName)
        {
            return ((IDataLayer2)featureLayer).Connect(pOptRepairName);
        }

        IName IDataLayer2.DataSourceName
        {
            get
            {
                return ((IDataLayer2)featureLayer).DataSourceName;
            }
            set
            {
                ((IDataLayer2)featureLayer).DataSourceName = value;
            }
        }

        void IDataLayer2.Disconnect()
        {
            ((IDataLayer2)featureLayer).Disconnect();
        }

        bool IDataLayer2.InWorkspace(IWorkspace Workspace)
        {
            return ((IDataLayer2)featureLayer).InWorkspace(Workspace);
        }

        string IDataLayer2.RelativeBase
        {
            get
            {
                return ((IDataLayer2)featureLayer).RelativeBase;
            }
            set
            {
                ((IDataLayer2)featureLayer).RelativeBase = value;
            }
        }

        bool IDataLayer2.get_DataSourceSupported(IName Name)
        {
            return ((IDataLayer2)featureLayer).get_DataSourceSupported(Name);
        }
        #endregion

        #region IDataset
        string IDataset.BrowseName
        {
            get
            {
                return ((IDataset)featureLayer).BrowseName;
            }
            set
            {
                ((IDataset)featureLayer).BrowseName = value;
            }
        }

        bool IDataset.CanCopy()
        {
            return ((IDataset)featureLayer).CanCopy();
        }

        bool IDataset.CanDelete()
        {
            return ((IDataset)featureLayer).CanDelete();
        }

        bool IDataset.CanRename()
        {
            return ((IDataset)featureLayer).CanRename();
        }

        string IDataset.Category
        {
            get { return ((IDataset)featureLayer).Category; }
        }

        IDataset IDataset.Copy(string copyName, IWorkspace copyWorkspace)
        {
            return ((IDataset)featureLayer).Copy(copyName, copyWorkspace);
        }

        void IDataset.Delete()
        {
            ((IDataset)featureLayer).Delete();
        }

        IName IDataset.FullName
        {
            get { return ((IDataset)featureLayer).FullName; }
        }

        string IDataset.Name
        {
            get { return ((IDataset)featureLayer).Name; }
        }

        IPropertySet IDataset.PropertySet
        {
            get { return ((IDataset)featureLayer).PropertySet; }
        }

        void IDataset.Rename(string Name)
        {
            ((IDataset)featureLayer).Rename(Name);
        }

        IEnumDataset IDataset.Subsets
        {
            get { return ((IDataset)featureLayer).Subsets; }
        }

        esriDatasetType IDataset.Type
        {
            get { return ((IDataset)featureLayer).Type; }
        }

        IWorkspace IDataset.Workspace
        {
            get { return ((IDataset)featureLayer).Workspace; }
        }
        #endregion

        #region IDisplayAdmin
        bool IDisplayAdmin.UsesFilter
        {
            get { return ((IDisplayAdmin)featureLayer).UsesFilter; }
        }
        #endregion

        #region IDisplayFilterManager
        IDisplayFilter IDisplayFilterManager.DisplayFilter
        {
            get
            {
                return ((IDisplayFilterManager)featureLayer).DisplayFilter;
            }
            set
            {
                ((IDisplayFilterManager)featureLayer).DisplayFilter = value;
            }
        }

        bool IDisplayFilterManager.UsesFilter
        {
            get { return ((IDisplayFilterManager)featureLayer).UsesFilter; }
        }
        #endregion

        #region IDisplayRelationshipClass
        void IDisplayRelationshipClass.DisplayRelationshipClass(IRelationshipClass relClass, esriJoinType JoinType)
        {
            ((IDisplayRelationshipClass)featureLayer).DisplayRelationshipClass(relClass, JoinType);
        }

        esriJoinType IDisplayRelationshipClass.JoinType
        {
            get { return ((IDisplayRelationshipClass)featureLayer).JoinType; }
        }

        IRelationshipClass IDisplayRelationshipClass.RelationshipClass
        {
            get { return ((IDisplayRelationshipClass)featureLayer).RelationshipClass; }
        }
        #endregion

        #region IDisplayTable
        ISelectionSet IDisplayTable.DisplaySelectionSet
        {
            get { return ((IDisplayTable)featureLayer).DisplaySelectionSet; }
        }

        ITable IDisplayTable.DisplayTable
        {
            get { return ((IDisplayTable)featureLayer).DisplayTable; }
        }

        ICursor IDisplayTable.SearchDisplayTable(IQueryFilter pQueryFilter, bool Recycling)
        {
            return ((IDisplayTable)featureLayer).SearchDisplayTable(pQueryFilter, Recycling);
        }

        ISelectionSet IDisplayTable.SelectDisplayTable(IQueryFilter pQueryFilter, esriSelectionType selType, esriSelectionOption selOption, IWorkspace pSelWorkspace)
        {
            return ((IDisplayTable)featureLayer).SelectDisplayTable(pQueryFilter, selType, selOption, pSelWorkspace);
        }
        #endregion

        #region IFeatureLayer
        IEnvelope IFeatureLayer.AreaOfInterest
        {
            get { return ((IFeatureLayer)featureLayer).AreaOfInterest; }
        }

        bool IFeatureLayer.Cached
        {
            get
            {
                return ((IFeatureLayer)featureLayer).Cached;
            }
            set
            {
                ((IFeatureLayer)featureLayer).Cached = value;
            }
        }

        string IFeatureLayer.DataSourceType
        {
            get
            {
                return ((IFeatureLayer)featureLayer).DataSourceType;
            }
            set
            {
                ((IFeatureLayer)featureLayer).DataSourceType = value;
            }
        }

        string IFeatureLayer.DisplayField
        {
            get
            {
                return ((IFeatureLayer)featureLayer).DisplayField;
            }
            set
            {
                ((IFeatureLayer)featureLayer).DisplayField = value;
            }
        }

        void IFeatureLayer.Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            ((IFeatureLayer)featureLayer).Draw(drawPhase, Display, trackCancel);
        }

        IFeatureClass IFeatureLayer.FeatureClass
        {
            get
            {
                return ((IFeatureLayer)featureLayer).FeatureClass;
            }
            set
            {
                ((IFeatureLayer)featureLayer).FeatureClass = value;
            }
        }

        double IFeatureLayer.MaximumScale
        {
            get
            {
                return ((IFeatureLayer)featureLayer).MaximumScale;
            }
            set
            {
                ((IFeatureLayer)featureLayer).MaximumScale = value;
            }
        }

        double IFeatureLayer.MinimumScale
        {
            get
            {
                return ((IFeatureLayer)featureLayer).MinimumScale;
            }
            set
            {
                ((IFeatureLayer)featureLayer).MinimumScale = value;
            }
        }

        string IFeatureLayer.Name
        {
            get
            {
                return ((IFeatureLayer)featureLayer).Name;
            }
            set
            {
                ((IFeatureLayer)featureLayer).Name = value;
            }
        }

        bool IFeatureLayer.ScaleSymbols
        {
            get
            {
                return ((IFeatureLayer)featureLayer).ScaleSymbols;
            }
            set
            {
                ((IFeatureLayer)featureLayer).ScaleSymbols = value;
            }
        }

        IFeatureCursor IFeatureLayer.Search(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((IFeatureLayer)featureLayer).Search(QueryFilter, Recycling);
        }

        bool IFeatureLayer.Selectable
        {
            get
            {
                return ((IFeatureLayer)featureLayer).Selectable;
            }
            set
            {
                ((IFeatureLayer)featureLayer).Selectable = value;
            }
        }

        bool IFeatureLayer.ShowTips
        {
            get
            {
                return ((IFeatureLayer)featureLayer).ShowTips;
            }
            set
            {
                ((IFeatureLayer)featureLayer).ShowTips = value;
            }
        }

        ISpatialReference IFeatureLayer.SpatialReference
        {
            set { ((IFeatureLayer)featureLayer).SpatialReference = value; }
        }

        int IFeatureLayer.SupportedDrawPhases
        {
            get { return ((IFeatureLayer)featureLayer).SupportedDrawPhases; }
        }

        bool IFeatureLayer.Valid
        {
            get { return ((IFeatureLayer)featureLayer).Valid; }
        }

        bool IFeatureLayer.Visible
        {
            get
            {
                return ((IFeatureLayer)featureLayer).Visible;
            }
            set
            {
                ((IFeatureLayer)featureLayer).Visible = value;
            }
        }

        string IFeatureLayer.get_TipText(double X, double Y, double Tolerance)
        {
            return ((IFeatureLayer)featureLayer).get_TipText(X, Y, Tolerance);
        }
        #endregion

		#region ILayer
		IEnvelope ILayer.AreaOfInterest
        {
            get { return ((ILayer)featureLayer).AreaOfInterest; }
        }

        bool ILayer.Cached
        {
            get
            {
                return ((ILayer)featureLayer).Cached;
            }
            set
            {
                ((ILayer)featureLayer).Cached = value;
            }
        }

        void ILayer.Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
			((ILayer)featureLayer).Draw(drawPhase, Display, trackCancel);
        }

        double ILayer.MaximumScale
        {
            get
            {
                return ((ILayer)featureLayer).MaximumScale;
            }
            set
            {
                ((ILayer)featureLayer).MaximumScale = value;
            }
        }

        double ILayer.MinimumScale
        {
            get
            {
                return ((ILayer)featureLayer).MinimumScale;
            }
            set
            {
                ((ILayer)featureLayer).MinimumScale = value;
            }
        }

        string ILayer.Name
        {
            get
            {
                return ((ILayer)featureLayer).Name;
            }
            set
            {
                ((ILayer)featureLayer).Name = value;
            }
        }

        bool ILayer.ShowTips
        {
            get
            {
                return ((ILayer)featureLayer).ShowTips;
            }
            set
            {
                ((ILayer)featureLayer).ShowTips = value;
            }
        }

        ISpatialReference ILayer.SpatialReference
        {
            set { ((ILayer)featureLayer).SpatialReference = value; }
        }

        int ILayer.SupportedDrawPhases
        {
            get { return ((ILayer)featureLayer).SupportedDrawPhases; }
        }

        bool ILayer.Valid
        {
            get { return ((ILayer)featureLayer).Valid; }
        }

        bool ILayer.Visible
        {
            get
            {
                return ((ILayer)featureLayer).Visible;
            }
            set
            {
                ((ILayer)featureLayer).Visible = value;
            }
        }

        string ILayer.get_TipText(double X, double Y, double Tolerance)
        {
            return ((ILayer)featureLayer).get_TipText(X, Y, Tolerance);
        }
        #endregion

        #region IFeatureLayer2
        string IFeatureLayer2.DataSourceType
        {
            get
            {
                return ((IFeatureLayer2)featureLayer).DataSourceType;
            }
            set
            {
                ((IFeatureLayer2)featureLayer).DataSourceType = value;
            }
        }

        string IFeatureLayer2.DisplayField
        {
            get
            {
                return ((IFeatureLayer2)featureLayer).DisplayField;
            }
            set
            {
                ((IFeatureLayer2)featureLayer).DisplayField = value;
            }
        }

        void IFeatureLayer2.ExpandRegionForSymbols(IDisplay Display, IGeometry region)
        {
            ((IFeatureLayer2)featureLayer).ExpandRegionForSymbols(Display, region);
        }

        IFeatureClass IFeatureLayer2.FeatureClass
        {
            get
            {
                return ((IFeatureLayer2)featureLayer).FeatureClass;
            }
            set
            {
                ((IFeatureLayer2)featureLayer).FeatureClass = value;
            }
        }

        bool IFeatureLayer2.ScaleSymbols
        {
            get
            {
                return ((IFeatureLayer2)featureLayer).ScaleSymbols;
            }
            set
            {
                ((IFeatureLayer2)featureLayer).ScaleSymbols = value;
            }
        }

        IFeatureCursor IFeatureLayer2.Search(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((IFeatureLayer2)featureLayer).Search(QueryFilter, Recycling);
        }

        bool IFeatureLayer2.Selectable
        {
            get
            {
                return ((IFeatureLayer2)featureLayer).Selectable;
            }
            set
            {
                ((IFeatureLayer2)featureLayer).Selectable = value;
            }
        }

        esriGeometryType IFeatureLayer2.ShapeType
        {
            get { return ((IFeatureLayer2)featureLayer).ShapeType; }
        }
        #endregion

        #region IFeatureLayerDefinition
        IFeatureLayer IFeatureLayerDefinition.CreateSelectionLayer(string LayerName, bool useCurrentSelection, string joinTableNames, string Expression)
        {
            return ((IFeatureLayerDefinition)featureLayer).CreateSelectionLayer(LayerName, useCurrentSelection, joinTableNames, Expression);
        }

        string IFeatureLayerDefinition.DefinitionExpression
        {
            get
            {
                return ((IFeatureLayerDefinition)featureLayer).DefinitionExpression;
            }
            set
            {
                ((IFeatureLayerDefinition)featureLayer).DefinitionExpression = value;
            }
        }

        ISelectionSet IFeatureLayerDefinition.DefinitionSelectionSet
        {
            get { return ((IFeatureLayerDefinition)featureLayer).DefinitionSelectionSet; }
        }

        IRelationshipClass IFeatureLayerDefinition.RelationshipClass
        {
            get
            {
                return ((IFeatureLayerDefinition)featureLayer).RelationshipClass;
            }
            set
            {
                ((IFeatureLayerDefinition)featureLayer).RelationshipClass = value;
            }
        }
        #endregion

		#region IFeatureLayerSelectionEvents

		void IFeatureLayerSelectionEvents.FeatureLayerSelectionChanged()
		{
			((IFeatureLayerSelectionEvents)featureLayer).FeatureLayerSelectionChanged();
		}

		#endregion

		#region IFeatureSelection
		void IFeatureSelection.Add(IFeature Feature)
        {
            ((IFeatureSelection)featureLayer).Add(Feature);
        }

        double IFeatureSelection.BufferDistance
        {
            get
            {
                return ((IFeatureSelection)featureLayer).BufferDistance;
            }
            set
            {
                ((IFeatureSelection)featureLayer).BufferDistance = value;
            }
        }

        void IFeatureSelection.Clear()
        {
            ((IFeatureSelection)featureLayer).Clear();
        }

        esriSelectionResultEnum IFeatureSelection.CombinationMethod
        {
            get
            {
                return ((IFeatureSelection)featureLayer).CombinationMethod;
            }
            set
            {
                ((IFeatureSelection)featureLayer).CombinationMethod = value;
            }
        }

        void IFeatureSelection.SelectFeatures(IQueryFilter Filter, esriSelectionResultEnum Method, bool justOne)
        {
            ((IFeatureSelection)featureLayer).SelectFeatures(Filter, Method, justOne);
        }

        void IFeatureSelection.SelectionChanged()
        {
            ((IFeatureSelection)featureLayer).SelectionChanged();
        }

        IColor IFeatureSelection.SelectionColor
        {
            get
            {
                return ((IFeatureSelection)featureLayer).SelectionColor;
            }
            set
            {
                ((IFeatureSelection)featureLayer).SelectionColor = value;
            }
        }

        ISelectionSet IFeatureSelection.SelectionSet
        {
            get
            {
                return ((IFeatureSelection)featureLayer).SelectionSet;
            }
            set
            {
                ((IFeatureSelection)featureLayer).SelectionSet = value;
            }
        }

        ISymbol IFeatureSelection.SelectionSymbol
        {
            get
            {
                return ((IFeatureSelection)featureLayer).SelectionSymbol;
            }
            set
            {
                ((IFeatureSelection)featureLayer).SelectionSymbol = value;
            }
        }

        bool IFeatureSelection.SetSelectionSymbol
        {
            get
            {
                return ((IFeatureSelection)featureLayer).SetSelectionSymbol;
            }
            set
            {
                ((IFeatureSelection)featureLayer).SetSelectionSymbol = value;
            }
        }
        #endregion

        #region IFind
        IArray IFind.Find(string Search, bool Contains, object Fields, ITrackCancel trackCancel)
        {
            return ((IFind)featureLayer).Find(Search, Contains, Fields, trackCancel);
        }

        string IFind.FindDisplayField
        {
            get { return ((IFind)featureLayer).FindDisplayField; }
        }

        object IFind.FindFields
        {
            get { return ((IFind)featureLayer).FindFields; }
        }
        #endregion

        #region IGeoDataset
        IEnvelope IGeoDataset.Extent
        {
            get { return ((IGeoDataset)featureLayer).Extent; }
        }

        ISpatialReference IGeoDataset.SpatialReference
        {
            get { return ((IGeoDataset)featureLayer).SpatialReference; }
        }
        #endregion

        #region IGeoFeatureLayer
        IAnnotateLayerPropertiesCollection IGeoFeatureLayer.AnnotationProperties
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).AnnotationProperties;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).AnnotationProperties = value;
            }
        }

        UID IGeoFeatureLayer.AnnotationPropertiesID
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).AnnotationPropertiesID;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).AnnotationPropertiesID = value;
            }
        }

        IEnvelope IGeoFeatureLayer.AreaOfInterest
        {
            get { return ((IGeoFeatureLayer)featureLayer).AreaOfInterest; }
        }

        bool IGeoFeatureLayer.Cached
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).Cached;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).Cached = value;
            }
        }

        int IGeoFeatureLayer.CurrentMapLevel
        {
            set { ((IGeoFeatureLayer)featureLayer).CurrentMapLevel = value; }
        }

        string IGeoFeatureLayer.DataSourceType
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).DataSourceType;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).DataSourceType = value;
            }
        }

        bool IGeoFeatureLayer.DisplayAnnotation
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).DisplayAnnotation;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).DisplayAnnotation = value;
            }
        }

        IFeatureClass IGeoFeatureLayer.DisplayFeatureClass
        {
            get { return ((IGeoFeatureLayer)featureLayer).DisplayFeatureClass; }
        }

        string IGeoFeatureLayer.DisplayField
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).DisplayField;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).DisplayField = value;
            }
        }

        void IGeoFeatureLayer.Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            ((IGeoFeatureLayer)featureLayer).Draw(drawPhase, Display, trackCancel);
        }

        IFeatureIDSet IGeoFeatureLayer.ExclusionSet
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).ExclusionSet;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).ExclusionSet = value;
            }
        }

        IFeatureClass IGeoFeatureLayer.FeatureClass
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).FeatureClass;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).FeatureClass = value;
            }
        }

        double IGeoFeatureLayer.MaximumScale
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).MaximumScale;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).MaximumScale = value;
            }
        }

        double IGeoFeatureLayer.MinimumScale
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).MinimumScale;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).MinimumScale = value;
            }
        }

        string IGeoFeatureLayer.Name
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).Name;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).Name = value;
            }
        }

        IFeatureRenderer IGeoFeatureLayer.Renderer
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).Renderer;
            }
            set
            {
				((IGeoFeatureLayer)featureLayer).Renderer = value;
            }
        }

        UID IGeoFeatureLayer.RendererPropertyPageClassID
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).RendererPropertyPageClassID;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).RendererPropertyPageClassID = value;
            }
        }

        bool IGeoFeatureLayer.ScaleSymbols
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).ScaleSymbols;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).ScaleSymbols = value;
            }
        }

        IFeatureCursor IGeoFeatureLayer.Search(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((IGeoFeatureLayer)featureLayer).Search(QueryFilter, Recycling);
        }

        IFeatureCursor IGeoFeatureLayer.SearchDisplayFeatures(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((IGeoFeatureLayer)featureLayer).SearchDisplayFeatures(QueryFilter, Recycling);
        }

        bool IGeoFeatureLayer.Selectable
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).Selectable;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).Selectable = value;
            }
        }

        bool IGeoFeatureLayer.ShowTips
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).ShowTips;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).ShowTips = value;
            }
        }

        ISpatialReference IGeoFeatureLayer.SpatialReference
        {
            set { ((IGeoFeatureLayer)featureLayer).SpatialReference = value; }
        }

        int IGeoFeatureLayer.SupportedDrawPhases
        {
            get { return ((IGeoFeatureLayer)featureLayer).SupportedDrawPhases; }
        }

        bool IGeoFeatureLayer.Valid
        {
            get { return ((IGeoFeatureLayer)featureLayer).Valid; }
        }

        bool IGeoFeatureLayer.Visible
        {
            get
            {
                return ((IGeoFeatureLayer)featureLayer).Visible;
            }
            set
            {
                ((IGeoFeatureLayer)featureLayer).Visible = value;
            }
        }

        string IGeoFeatureLayer.get_TipText(double X, double Y, double Tolerance)
        {
            return ((IGeoFeatureLayer)featureLayer).get_TipText(X, Y, Tolerance);
        }
        #endregion

        #region IHotlinkContainer
        string IHotlinkContainer.HotlinkField
        {
            get
            {
                return ((IHotlinkContainer)featureLayer).HotlinkField;
            }
            set
            {
                ((IHotlinkContainer)featureLayer).HotlinkField = value;
            }
        }

        esriHyperlinkType IHotlinkContainer.HotlinkType
        {
            get
            {
                return ((IHotlinkContainer)featureLayer).HotlinkType;
            }
            set
            {
                ((IHotlinkContainer)featureLayer).HotlinkType = value;
            }
        }
        #endregion

        #region IHotlinkMacro
        string IHotlinkMacro.MacroName
        {
            get
            {
                return ((IHotlinkMacro)featureLayer).MacroName;
            }
            set
            {
                ((IHotlinkMacro)featureLayer).MacroName = value;
            }
        }
        #endregion

        #region IHyperlinkContainer
        void IHyperlinkContainer.AddHyperlink(IHyperlink Link)
        {
            ((IHyperlinkContainer)featureLayer).AddHyperlink(Link);
        }

        int IHyperlinkContainer.HyperlinkCount
        {
            get { return ((IHyperlinkContainer)featureLayer).HyperlinkCount; }
        }

        void IHyperlinkContainer.RemoveHyperlink(int Index)
        {
            ((IHyperlinkContainer)featureLayer).RemoveHyperlink(Index);
        }

        IHyperlink IHyperlinkContainer.get_Hyperlink(int Index)
        {
            return ((IHyperlinkContainer)featureLayer).get_Hyperlink(Index);
        }

        void IHyperlinkContainer.set_Hyperlink(int Index, IHyperlink Link)
        {
            ((IHyperlinkContainer)featureLayer).set_Hyperlink(Index, Link);
        }
        #endregion

        #region IIdentify
        IArray IIdentify.Identify(IGeometry pGeom)
        {
            return ((IIdentify)featureLayer).Identify(pGeom);
        }
        #endregion

        #region IIdentify2
        IArray IIdentify2.Identify(IGeometry pGeom, ITrackCancel pTrackCancel)
        {
            
            IArray arr = ((IIdentify2)featureLayer).Identify(pGeom, pTrackCancel);
            return arr;
        }

        double IIdentify2.Scale
        {
            set { ((IIdentify2)featureLayer).Scale = value; }
        }
        #endregion

        #region ILayer2
        IEnvelope ILayer2.AreaOfInterest
        {
            get
            {
                return ((ILayer2)featureLayer).AreaOfInterest;
            }
            set
            {
                ((ILayer2)featureLayer).AreaOfInterest = value;
            }
        }

        bool ILayer2.Cached
        {
            get
            {
                return ((ILayer2)featureLayer).Cached;
            }
            set
            {
                ((ILayer2)featureLayer).Cached = value;
            }
        }

        void ILayer2.Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            ((ILayer2)featureLayer).Draw(drawPhase, Display, trackCancel);
        }

        double ILayer2.MaximumScale
        {
            get
            {
                return ((ILayer2)featureLayer).MaximumScale;
            }
            set
            {
                ((ILayer2)featureLayer).MaximumScale = value;
            }
        }

        double ILayer2.MinimumScale
        {
            get
            {
                return ((ILayer2)featureLayer).MinimumScale;
            }
            set
            {
                ((ILayer2)featureLayer).MinimumScale = value;
            }
        }

        string ILayer2.Name
        {
            get
            {
                return ((ILayer2)featureLayer).Name;
            }
            set
            {
                ((ILayer2)featureLayer).Name = value;
            }
        }

        bool ILayer2.ScaleRangeReadOnly
        {
            get { return ((ILayer2)featureLayer).ScaleRangeReadOnly; }
        }

        bool ILayer2.ShowTips
        {
            get
            {
                return ((ILayer2)featureLayer).ShowTips;
            }
            set
            {
                ((ILayer2)featureLayer).ShowTips = value;
            }
        }

        ISpatialReference ILayer2.SpatialReference
        {
            set { ((ILayer2)featureLayer).SpatialReference = value; }
        }

        int ILayer2.SupportedDrawPhases
        {
            get { return ((ILayer2)featureLayer).SupportedDrawPhases; }
        }

        bool ILayer2.Valid
        {
            get { return ((ILayer2)featureLayer).Valid; }
        }

        bool ILayer2.Visible
        {
            get
            {
                return ((ILayer2)featureLayer).Visible;
            }
            set
            {
                ((ILayer2)featureLayer).Visible = value;
            }
        }

        string ILayer2.get_TipText(double X, double Y, double Tolerance)
        {
            return ((ILayer2)featureLayer).get_TipText(X, Y, Tolerance);
        }
        #endregion

        #region ILayerDrawingProperties
        bool ILayerDrawingProperties.DrawingPropsDirty
        {
            get
            {
                return ((ILayerDrawingProperties)featureLayer).DrawingPropsDirty;
            }
            set
            {
                ((ILayerDrawingProperties)featureLayer).DrawingPropsDirty = value;
            }
        }
        #endregion

        #region ILayerEffects
        short ILayerEffects.Brightness
        {
            get
            {
                return ((ILayerEffects)featureLayer).Brightness;
            }
            set
            {
                ((ILayerEffects)featureLayer).Brightness = value;
            }
        }

        short ILayerEffects.Contrast
        {
            get
            {
                return ((ILayerEffects)featureLayer).Contrast;
            }
            set
            {
                ((ILayerEffects)featureLayer).Contrast = value;
            }
        }

        bool ILayerEffects.SupportsBrightnessChange
        {
            get { return ((ILayerEffects)featureLayer).SupportsBrightnessChange; }
        }

        bool ILayerEffects.SupportsContrastChange
        {
            get { return ((ILayerEffects)featureLayer).SupportsContrastChange; }
        }

        bool ILayerEffects.SupportsInteractive
        {
            get
            {
                return ((ILayerEffects)featureLayer).SupportsInteractive;
            }
            set
            {
                ((ILayerEffects)featureLayer).SupportsInteractive = value;
            }
        }

        bool ILayerEffects.SupportsTransparency
        {
            get { return ((ILayerEffects)featureLayer).SupportsTransparency; }
        }

        short ILayerEffects.Transparency
        {
            get
            {
                return ((ILayerEffects)featureLayer).Transparency;
            }
            set
            {
                ((ILayerEffects)featureLayer).Transparency = value;
            }
        }
        #endregion

        #region ILayerExtensions
        void ILayerExtensions.AddExtension(object ext)
        {
            ((ILayerExtensions)featureLayer).AddExtension(ext);
        }

        int ILayerExtensions.ExtensionCount
        {
            get { return ((ILayerExtensions)featureLayer).ExtensionCount; }
        }

        void ILayerExtensions.RemoveExtension(int Index)
        {
            ((ILayerExtensions)featureLayer).RemoveExtension(Index);
        }

        object ILayerExtensions.get_Extension(int Index)
        {
            return ((ILayerExtensions)featureLayer).get_Extension(Index);
        }
        #endregion

        #region ILayerFields Members
        int ILayerFields.FieldCount
        {
            get { return ((ILayerFields)featureLayer).FieldCount; }
        }

        int ILayerFields.FindField(string FieldName)
        {
            return ((ILayerFields)featureLayer).FindField(FieldName);
        }

        IField ILayerFields.get_Field(int Index)
        {
            return ((ILayerFields)featureLayer).get_Field(Index);
        }

        IFieldInfo ILayerFields.get_FieldInfo(int Index)
        {
            return ((ILayerFields)featureLayer).get_FieldInfo(Index);
        }
        #endregion

		#region ILayerEvents

		void ILayerEvents.VisibilityChanged(bool currentState)
		{
			((ILayerEvents)featureLayer).VisibilityChanged(currentState);
		}

		#endregion

		#region ILayerInfo
		int ILayerInfo.LargeImage
        {
            get { return ((ILayerInfo)featureLayer).LargeImage; }
        }

        int ILayerInfo.LargeSelectedImage
        {
            get { return ((ILayerInfo)featureLayer).LargeSelectedImage; }
        }

        int ILayerInfo.SmallImage
        {
            get { return ((ILayerInfo)featureLayer).SmallImage; }
        }

        int ILayerInfo.SmallSelectedImage
        {
            get { return ((ILayerInfo)featureLayer).SmallSelectedImage; }
        }
        #endregion

        #region ILayerPosition
        double ILayerPosition.LayerWeight
        {
            get
            {
                return ((ILayerPosition)featureLayer).LayerWeight;
            }
            set
            {
                ((ILayerPosition)featureLayer).LayerWeight = value;
            }
        }
        #endregion

        #region ILegendInfo
        int ILegendInfo.LegendGroupCount
        {
            get { return ((ILegendInfo)featureLayer).LegendGroupCount; }
        }

        ILegendItem ILegendInfo.LegendItem
        {
            get { return ((ILegendInfo)featureLayer).LegendItem; }
        }

        bool ILegendInfo.SymbolsAreGraduated
        {
            get
            {
                return ((ILegendInfo)featureLayer).SymbolsAreGraduated;
            }
            set
            {
                ((ILegendInfo)featureLayer).SymbolsAreGraduated = value;
            }
        }

        ILegendGroup ILegendInfo.get_LegendGroup(int Index)
        {
            return ((ILegendInfo)featureLayer).get_LegendGroup(Index);
        }
        #endregion

        #region IMapLevel
        int IMapLevel.MapLevel
        {
            get
            {
                return ((IMapLevel)featureLayer).MapLevel;
            }
            set
            {
                ((IMapLevel)featureLayer).MapLevel = value;
            }
        }
        #endregion

        #region IObjectClassSchemaEvents
        void IObjectClassSchemaEvents.OnAddField(string FieldName)
        {
            ((IObjectClassSchemaEvents)featureLayer).OnAddField(FieldName);
        }

        void IObjectClassSchemaEvents.OnBehaviorChanged()
        {
            ((IObjectClassSchemaEvents)featureLayer).OnBehaviorChanged();
        }

        void IObjectClassSchemaEvents.OnDeleteField(string FieldName)
        {
            ((IObjectClassSchemaEvents)featureLayer).OnDeleteField(FieldName);
        }
        #endregion

        #region IPersistStream
        void IPersistStream.GetClassID(out Guid pClassID)
        {
            pClassID = this.GetType().GUID;
        }

        void IPersistStream.GetSizeMax(out _ULARGE_INTEGER pcbSize)
        {
            ((IPersistStream)featureLayer).GetSizeMax(out pcbSize);
        }

        void IPersistStream.IsDirty()
        {
            ((IPersistStream)featureLayer).IsDirty();
        }

        void IPersistStream.Load(IStream pstm)
        {
            // First let ESRI do their thing.
            ((IPersistStream)featureLayer).Load(pstm);

            // Load the FeatureClassId.
            StreamHelper helper = new StreamHelper(pstm);
            int id = helper.readInt();

            // Manually load the data.

            // I shouldn't have to do this because the FeatureLayerClass has
            // already called the Open() method on the DatasetName ... but
            // I can't figure out how to get at the opened PostGisFeatureDataset.
            PostGisDatasetName dsName = (PostGisDatasetName)((IDataset)this).FullName;
            PostGisFeatureDataset ds = (PostGisFeatureDataset)dsName.Open();
            ((IFeatureLayer)this).FeatureClass = new PostGisFeatureClass(ds, id);
        }

        void IPersistStream.Save(IStream pstm, int fClearDirty)
        {
            // First let ESRI do their thing.
            ((IPersistStream)featureLayer).Save(pstm, fClearDirty);

            // Write the FeatureClassId.
            StreamHelper helper = new StreamHelper(pstm);
            int id = ((IFeatureLayer)this).FeatureClass.FeatureClassID;
            helper.writeInt(id);
        }

        void IPersist.GetClassID(out Guid pClassID)
        {
            ((IPersist)featureLayer).GetClassID(out pClassID);
        }
        #endregion

        #region IPropertySupport
        bool IPropertySupport.Applies(object pUnk)
        {
            return ((IPropertySupport)featureLayer).Applies(pUnk);
        }

        object IPropertySupport.Apply(object NewObject)
        {
            return ((IPropertySupport)featureLayer).Apply(NewObject);
        }

        bool IPropertySupport.CanApply(object pUnk)
        {
            return ((IPropertySupport)featureLayer).CanApply(pUnk);
        }

        object IPropertySupport.get_Current(object pUnk)
        {
            return ((IPropertySupport)featureLayer).get_Current(pUnk);
        }
        #endregion

        #region IPublishLayer
        bool IPublishLayer.DataValid
        {
            get { return ((IPublishLayer)featureLayer).DataValid; }
        }

        void IPublishLayer.PrepareForPublishing()
        {
            ((IPublishLayer)featureLayer).PrepareForPublishing();
        }

        string IPublishLayer.PublishingDescription
        {
            get { return ((IPublishLayer)featureLayer).PublishingDescription; }
        }

        bool IPublishLayer.SupportsPublishing
        {
            get { return ((IPublishLayer)featureLayer).SupportsPublishing; }
        }

        string IPublishLayer.get_DataDetails(string bsPadding)
        {
            return ((IPublishLayer)featureLayer).get_DataDetails(bsPadding);
        }
        #endregion

        #region IRelationshipClassCollection
        IEnumRelationshipClass IRelationshipClassCollection.FindRelationshipClasses(IObjectClass ObjectClass, esriRelRole role)
        {
            return ((IRelationshipClassCollection)featureLayer).FindRelationshipClasses(ObjectClass, role);
        }

        IEnumRelationshipClass IRelationshipClassCollection.RelationshipClasses
        {
            get { return ((IRelationshipClassCollection)featureLayer).RelationshipClasses; }
        }
        #endregion

        #region IRelationshipClassCollectionEdit
        void IRelationshipClassCollectionEdit.AddRelationshipClass(IRelationshipClass RelationshipClass)
        {
            ((IRelationshipClassCollectionEdit)featureLayer).AddRelationshipClass(RelationshipClass);
        }

        void IRelationshipClassCollectionEdit.RemoveAllRelationshipClasses()
        {
            ((IRelationshipClassCollectionEdit)featureLayer).RemoveAllRelationshipClasses();
        }

        void IRelationshipClassCollectionEdit.RemoveRelationshipClass(IRelationshipClass RelationshipClass)
        {
            ((IRelationshipClassCollectionEdit)featureLayer).RemoveRelationshipClass(RelationshipClass);
        }
        #endregion


		#region ITable
		void ITable.AddField(IField Field)
        {
            ((ITable)featureLayer).AddField(Field);
        }

        void ITable.AddIndex(IIndex Index)
        {
            ((ITable)featureLayer).AddIndex(Index);
        }

        UID ITable.CLSID
        {
            get { return ((ITable)featureLayer).CLSID; }
        }

        IRow ITable.CreateRow()
        {
            return ((ITable)featureLayer).CreateRow();
        }

        IRowBuffer ITable.CreateRowBuffer()
        {
            return ((ITable)featureLayer).CreateRowBuffer();
        }

        void ITable.DeleteField(IField Field)
        {
            ((ITable)featureLayer).DeleteField(Field);
        }

        void ITable.DeleteIndex(IIndex Index)
        {
            ((ITable)featureLayer).DeleteIndex(Index);
        }

        void ITable.DeleteSearchedRows(IQueryFilter QueryFilter)
        {
            ((ITable)featureLayer).DeleteSearchedRows(QueryFilter);
        }

        UID ITable.EXTCLSID
        {
            get { return ((ITable)featureLayer).EXTCLSID; }
        }

        object ITable.Extension
        {
            get { return ((ITable)featureLayer).Extension; }
        }

        IPropertySet ITable.ExtensionProperties
        {
            get { return ((ITable)featureLayer).ExtensionProperties; }
        }

        IFields ITable.Fields
        {
            get { return ((ITable)featureLayer).Fields; }
        }

        int ITable.FindField(string Name)
        {
            return ((ITable)featureLayer).FindField(Name);
        }

        IRow ITable.GetRow(int OID)
        {
            return ((ITable)featureLayer).GetRow(OID);
        }

        ICursor ITable.GetRows(object oids, bool Recycling)
        {
            return ((ITable)featureLayer).GetRows(oids, Recycling);
        }

        bool ITable.HasOID
        {
            get { return ((ITable)featureLayer).HasOID; }
        }

        IIndexes ITable.Indexes
        {
            get { return ((ITable)featureLayer).Indexes; }
        }

        ICursor ITable.Insert(bool useBuffering)
        {
            return ((ITable)featureLayer).Insert(useBuffering);
        }

        string ITable.OIDFieldName
        {
            get { return ((ITable)featureLayer).OIDFieldName; }
        }

        int ITable.RowCount(IQueryFilter QueryFilter)
        {
            return ((ITable)featureLayer).RowCount(QueryFilter);
        }

        ICursor ITable.Search(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((ITable)featureLayer).Search(QueryFilter, Recycling);
        }

        ISelectionSet ITable.Select(IQueryFilter QueryFilter, esriSelectionType selType, esriSelectionOption selOption, IWorkspace selectionContainer)
        {
            return ((ITable)featureLayer).Select(QueryFilter, selType, selOption, selectionContainer);
        }

        ICursor ITable.Update(IQueryFilter QueryFilter, bool Recycling)
        {
            return ((ITable)featureLayer).Update(QueryFilter, Recycling);
        }

        void ITable.UpdateSearchedRows(IQueryFilter QueryFilter, IRowBuffer Buffer)
        {
            ((ITable)featureLayer).UpdateSearchedRows(QueryFilter, Buffer);
        }
        #endregion

        #region ITableDefinition
        string ITableDefinition.DefinitionExpression
        {
            get
            {
                return ((ITableDefinition)featureLayer).DefinitionExpression;
            }
            set
            {
                ((ITableDefinition)featureLayer).DefinitionExpression = value;
            }
        }

        ISelectionSet ITableDefinition.DefinitionSelectionSet
        {
            get { return ((ITableDefinition)featureLayer).DefinitionSelectionSet; }
        }
        #endregion

        #region ITableFields
        int ITableFields.FieldCount
        {
            get { return ((ITableFields)featureLayer).FieldCount; }
        }

        int ITableFields.FindField(string FieldName)
        {
            return ((ITableFields)featureLayer).FindField(FieldName);
        }

        IField ITableFields.get_Field(int Index)
        {
            return ((ITableFields)featureLayer).get_Field(Index);
        }

        IFieldInfo ITableFields.get_FieldInfo(int Index)
        {
            return ((ITableFields)featureLayer).get_FieldInfo(Index);
        }
        #endregion

        #region ITableSelection
        void ITableSelection.AddRow(IRow Row)
        {
            ((ITableSelection)featureLayer).AddRow(Row);
        }

        void ITableSelection.Clear()
        {
            ((ITableSelection)featureLayer).Clear();
        }

        void ITableSelection.SelectRows(IQueryFilter Filter, esriSelectionResultEnum Method, bool justOne)
        {
            ((ITableSelection)featureLayer).SelectRows(Filter, Method, justOne);
        }

        void ITableSelection.SelectionChanged()
        {
            ((ITableSelection)featureLayer).SelectionChanged();
        }

        ISelectionSet ITableSelection.SelectionSet
        {
            get
            {
                return ((ITableSelection)featureLayer).SelectionSet;
            }
            set
            {
                ((ITableSelection)featureLayer).SelectionSet = value;
            }
        }
        #endregion

		/*
		#region ISymbolLevels

		bool ISymbolLevels.UseSymbolLevels
		{
			get
			{
				return ((ISymbolLevels)featureLayer).UseSymbolLevels;
			}
			set
			{
				((ISymbolLevels)featureLayer).UseSymbolLevels = value;
			}
		}

		#endregion

		#region ITemporaryLayer

		bool ITemporaryLayer.Temporary
		{
			get
			{
				return ((ITemporaryLayer)featureLayer).Temporary;
			}
			set
			{
				((ITemporaryLayer)featureLayer).Temporary = value;
			}
		}

		#endregion

		#region ILayerSymbologyExtents

		void ILayerSymbologyExtents.ExpandRegionForSymbols(IDisplay pDisplay, IGeometry pRegion)
		{
			((ILayerSymbologyExtents)featureLayer).ExpandRegionForSymbols(pDisplay, pRegion);
		}

		#endregion

		#region IFeatureLayerDefinition2

		IFeatureLayer IFeatureLayerDefinition2.CreateSelectionLayer(string LayerName, bool useCurrentSelection, string joinTableNames, string Expression)
		{
			return ((IFeatureLayerDefinition2)featureLayer).CreateSelectionLayer(LayerName, useCurrentSelection, joinTableNames, Expression);
		}

		string IFeatureLayerDefinition2.DefinitionExpression
		{
			get
			{
				return ((IFeatureLayerDefinition2)featureLayer).DefinitionExpression;
			}
			set
			{
				((IFeatureLayerDefinition2)featureLayer).DefinitionExpression = value;
			}
		}

		ISelectionSet IFeatureLayerDefinition2.DefinitionSelectionSet
		{
			get { return ((IFeatureLayerDefinition2)featureLayer).DefinitionSelectionSet; }
		}

		IRelationshipClass IFeatureLayerDefinition2.RelationshipClass
		{
			get
			{
				return ((IFeatureLayerDefinition2)featureLayer).RelationshipClass;
			}
			set
			{
				((IFeatureLayerDefinition2)featureLayer).RelationshipClass = value;
			}
		}

		esriSearchOrder IFeatureLayerDefinition2.SearchOrder
		{
			get
			{
				return ((IFeatureLayerDefinition2)featureLayer).SearchOrder;
			}
			set
			{
				((IFeatureLayerDefinition2)featureLayer).SearchOrder = value;
			}
		}

		#endregion

		#region ILayerGeneralProperties

		double ILayerGeneralProperties.LastMaximumScale
		{
			get { return ((ILayerGeneralProperties)featureLayer).LastMaximumScale; }
		}

		double ILayerGeneralProperties.LastMinimumScale
		{
			get { return ((ILayerGeneralProperties)featureLayer).LastMinimumScale; }
		}

		string ILayerGeneralProperties.LayerDescription
		{
			get
			{
				return ((ILayerGeneralProperties)featureLayer).LayerDescription;
			}
			set
			{
				((ILayerGeneralProperties)featureLayer).LayerDescription = value;
			}
		}

		#endregion
		*/
	}
}