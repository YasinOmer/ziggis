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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

using ZigGis.Tester;

namespace zigGISTester.Utilities
{
	public class TesterUtilities
	{
		public static void ApplySimpleRenderer(IGeoFeatureLayer fl)
		{
			ISimpleRenderer sr = new SimpleRendererClass();
			sr.Symbol = (ISymbol)CreateSimpleFillSymbol(255, 0, 0);
			fl.Renderer = (IFeatureRenderer)sr;
		}

		public static void ApplyUniqueValueRenderer(IGeoFeatureLayer fl)
		{
			//COD (101,103...)
			IUniqueValueRenderer uvr = new CustomUniqueValueRenderer(); // UniqueValueRendererClass();
			uvr.FieldCount = 1;
			uvr.set_Field(0, "code");
			uvr.DefaultSymbol = (ISymbol)CreateSimpleFillSymbol(0, 0, 255);
			uvr.UseDefaultSymbol = true;
			//add values
			uvr.AddValue("CA01", "CA01", (ISymbol)CreateSimpleFillSymbol(255, 0, 0));
			uvr.AddValue("CA02", "CA02", (ISymbol)CreateSimpleFillSymbol(0, 255, 0));
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