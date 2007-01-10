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
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

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
			// Open workspace and feature class.
			IWorkspaceFactory wksf = new PostGisWksFactory();
			IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.OpenFromFile(@"C:\ziggis\ZigGis\example.zig", 0);
			IFeatureClass fc = fwks.OpenFeatureClass("zone");
			// Create the new layer (default renderer is ISimpleRenderer)
			IFeatureLayer layer = new PostGisFeatureLayer();
			layer.FeatureClass = fc;
			layer.Name = fc.AliasName;
			ILayer ly = layer as ILayer;
			IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
			doSimpleRenderer(gfl);
			//doUniqueValueRenderer(gfl);
			axMapControl1.AddLayer(gfl as ILayer, 0);
		}

		private void doUniqueValueRenderer(IGeoFeatureLayer gfl)
		{
			TesterUtilities.ApplyUniqueValueRenderer(gfl);
		}

		private void doSimpleRenderer(IGeoFeatureLayer gfl)
		{
			TesterUtilities.ApplySimpleRenderer(gfl);
		}

	}
}