using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace ZigGis.Tester
{
    class DoTest
    {
        public static void ApplySimpleRenderer(IGeoFeatureLayer fl)
        {
            ISimpleRenderer sr = new SimpleRendererClass();
            sr.Symbol = (ISymbol)CreateSimpleFillSymbol(255,0,0);
            fl.Renderer = (IFeatureRenderer)sr;
        }

        public static void ApplyUniqueValueRenderer(IGeoFeatureLayer fl)
        {
            //COD (101,103...)
            IUniqueValueRenderer uvr = new UniqueValueRendererClass();
            uvr.FieldCount = 1;
            uvr.set_Field(0, "COD");
            uvr.DefaultSymbol = (ISymbol)CreateSimpleFillSymbol(0,0,255);
            uvr.UseDefaultSymbol = true;
            //add values
            uvr.AddValue("101", "101", (ISymbol)CreateSimpleFillSymbol(255,0,0));
            uvr.AddValue("103", "103", (ISymbol)CreateSimpleFillSymbol(0, 255, 0));
            //render
            fl.Renderer = (IFeatureRenderer)uvr;
        }

        private static ISimpleFillSymbol CreateSimpleFillSymbol(int red, int green, int blue)
        {
            ISimpleFillSymbol sfs = new SimpleFillSymbolClass();
            IRgbColor color = new RgbColorClass();
            color.Red = red;
            color.Green = green;
            color.Blue = blue;
            sfs.Color = color;
            return sfs;
        }
    }

}
