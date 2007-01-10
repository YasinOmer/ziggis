// Copyright (C) 2005 Essejnet
// http://www.essejnet.com
// Abe Gillespie, abe@essejnet.com
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
using NUnit.Framework;
using ZigGis.ArcGIS.Geodatabase;
using zigGISTester.Utilities;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace zigGISTester
{
	[TestFixture]
	public class PostGisFeatureClassTester
	{
		IFeatureLayer layer;

		[SetUp]
		public void Init()
		{
			// Open workspace and feature class.
			IWorkspaceFactory wksf = new PostGisWksFactory();
			IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.OpenFromFile(@"C:\ziggis\ZigGis\example.zig", 0);
			IFeatureClass fc = fwks.OpenFeatureClass("zone");
			// Create the new layer (default renderer is ISimpleRenderer)
			layer = new PostGisFeatureLayer();
			layer.FeatureClass = fc;
			layer.Name = fc.AliasName;
		}

		[Test]
		public void RendererIsISimpleRenderer()
		{
			IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
			IFeatureRenderer fr = gfl.Renderer;
			Type expected = typeof(IFeatureRenderer);
			System.Console.WriteLine(expected.ToString() + ", " + fr.GetType().ToString());
			Assert.IsInstanceOfType(expected, fr, "test ok");
		}

		[Test]
		public void RendererIsIUniqueValueRenderer()
		{
			//from simple (default) to unique value
			TesterUtilities.ApplyUniqueValueRenderer(layer as IGeoFeatureLayer);
			IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
			IFeatureRenderer fr = gfl.Renderer;
			Type expected = typeof(IUniqueValueRenderer);
			System.Console.WriteLine(expected.ToString() + ", " + fr.GetType().ToString());
			Assert.IsInstanceOfType(expected, fr, "test ok");
		}
	}
}