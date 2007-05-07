using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ZigGis.ArcGIS.Geodatabase;
using zigGISTester.Utilities;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ArcMapUI;

namespace ZigGisSimpleWinApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadPostGisLayer();
			//LoadShapefile();
		}

		/// <summary>
		/// Load a PostGis layer in the test MapControl
		/// </summary>
		private void LoadPostGisLayer()
		{
			// Open workspace and feature class.
			IWorkspaceFactory wksf = new PostGisWorkspaceFactory();

			//Open from zig file
			IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.OpenFromFile(@"C:\ziggis\ZigGis\example2.zig", 0);

			/*
			//Open from PropertySet
			IPropertySet ps = new PropertySetClass();
			ps.SetProperty("server", "localhost");
			ps.SetProperty("database", "TUTORIAL");
			ps.SetProperty("user", "psqluser");
			ps.SetProperty("password", "psqluser");
			ps.SetProperty("port", "5432");
			ps.SetProperty("configfile", @"C:\ziggis\ZigGis\logging.config");
			IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.Open(ps, 0);
			*/

			IFeatureClass fc = fwks.OpenFeatureClass("paolo.canada");
			// Create the new layer (default renderer is ISimpleRenderer)
			IFeatureLayer layer = new PostGisFeatureLayer();
			layer.FeatureClass = fc;
			layer.Name = fc.AliasName;
			ILayer ly = layer as ILayer;
			IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
			//doSimpleRenderer(gfl);
			doUniqueValueRenderer(gfl);
			//IFeatureRenderer fr = new VerySimpleCustomRenderer();
			//gfl.Renderer = fr;
			axMapControl1.AddLayer(gfl as ILayer, 0);
			//SelectFeaturesFromFeatureClass(fc, fwks as IWorkspace);
			//SelectFeaturesFromFeatureLayer(layer);
		}

		/// <summary>
		/// Load a Shapefile layer in the test MapControl
		/// </summary>
		private void LoadShapefile()
		{
			IWorkspaceFactory wf = new ShapefileWorkspaceFactoryClass();
			IFeatureWorkspace fw = (IFeatureWorkspace)wf.OpenFromFile(@"C:\data", 0);
			IFeatureClass fc = fw.OpenFeatureClass("canada");
			IFeatureLayer layer = new FeatureLayerClass();
			layer.FeatureClass = fc;
			layer.Name = fc.AliasName;
			//SelectFeaturesFromFeatureLayer(layer);
			//SelectFeaturesFromFeatureClass(fc, fw as IWorkspace);
			axMapControl1.AddLayer(layer as ILayer, 0);
			IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
			doUniqueValueRenderer(gfl);
		}

		private void SelectFeaturesFromFeatureLayer(IFeatureLayer fl)
		{
			IFeatureSelection fs = fl as IFeatureSelection;
			IQueryFilter qf = new QueryFilterClass();
			qf.WhereClause = "";
			axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
			fs.SelectFeatures(qf, esriSelectionResultEnum.esriSelectionResultNew, false);
			PrintSelectionSet(fs.SelectionSet);
			axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
		}

		private void SelectFeaturesFromFeatureClass(IFeatureClass fc, IWorkspace ws)
		{
			IQueryFilter qf = new QueryFilterClass();
			qf.WhereClause = "";
			axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
			ISelectionSet ss = fc.Select(qf, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, ws);
			PrintSelectionSet(ss);
			axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
		}

		private void PrintSelectionSet(ISelectionSet ss)
		{
			IEnumIDs enumIDs = ss.IDs as IEnumIDs;
			try
			{
				int oid = enumIDs.Next();
				while (oid != -1)
				{
					System.Diagnostics.Debug.WriteLine("OID:" + oid);
					oid = enumIDs.Next();
				}
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("End of enum!");
			}
		}

		private void doUniqueValueRenderer(IGeoFeatureLayer gfl)
		{
			TesterUtilities.ApplyUniqueValueRenderer(gfl);
		}

		private void doSimpleRenderer(IGeoFeatureLayer gfl)
		{
			TesterUtilities.ApplySimpleRenderer(gfl);
		}

		private void ShowTableWindow(ILayer layer)
		{
			ITableWindow2 tw = new TableWindowClass();
			tw.FindViaLayer(layer);
			tw.Layer = layer;
		}

	}
}