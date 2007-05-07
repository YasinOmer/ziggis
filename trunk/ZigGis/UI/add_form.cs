using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ZigGis.ArcGIS.Geodatabase;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Display;
#endif

namespace ZigGis.ArcGIS.ArcMapUI
{
    public partial class AddForm : Form
    {
		private IWorkspaceFactory wksf;
		private IFeatureWorkspace fwks;

        public AddForm()
        {
            InitializeComponent();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDlg.ShowDialog(this);
			if (res == DialogResult.OK)
			{
				try
				{
					zigFile.Text = openFileDlg.FileName;
					// Open workspace
					wksf = new PostGisWorkspaceFactory();
					fwks = (IFeatureWorkspace)wksf.OpenFromFile(zigFile.Text, 0);
					// Open workspace
					IWorkspace ws = fwks as IWorkspace;
					// Show workspace PostGIS layer
					IEnumDatasetName edsn = ws.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
					IDatasetName dsn;
					// Load PostGIS layer names
					clbLayers.Items.Clear();
					while ((dsn = edsn.Next()) != null)
					{
						clbLayers.Items.Add(dsn.Name);
					}
				}
				catch (COMException COMex)
				{
					MessageBox.Show("Error " + COMex.ErrorCode.ToString() + ": " + COMex.Message);
				}
				catch (System.Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
			}
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

		/// <summary>
		/// Add PostGIS Layer to ArcMap
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void ok_Click(object sender, EventArgs e)
        {
            Hide();
            owner.application.RefreshWindow();
            try
            {
                // Create a new layer for each checked PostGIS layers in the list
				if (clbLayers.CheckedItems.Count > 0)
				{
					foreach (string layerToAdd in clbLayers.CheckedItems)
					{
						IFeatureClass fc = fwks.OpenFeatureClass(layerToAdd);
						IFeatureLayer layer = new PostGisFeatureLayer();
						//IFeatureLayer layer = new FeatureLayerClass();

						IGeoFeatureLayer gfl = layer as IGeoFeatureLayer;
						/* 
						// Paolo - remove this
						IFeatureRenderer fr = new VerySimpleCustomRenderer();
						gfl.Renderer = fr;
						//end of sample....
						*/

						/*
						//COD (101,103...)
						IUniqueValueRenderer uvr = new CustomUniqueValueRenderer();
						uvr.FieldCount = 1;
						uvr.set_Field(0, "code");
						uvr.DefaultSymbol = (ISymbol)CreateSimpleFillSymbol(0, 0, 255);
						uvr.UseDefaultSymbol = true;
						//add values
						uvr.AddValue("CA06", "CA06", (ISymbol)CreateSimpleFillSymbol(255, 0, 0));
						uvr.AddValue("CA05", "CA05", (ISymbol)CreateSimpleFillSymbol(0, 255, 0));
						//render
						gfl.Renderer = (IFeatureRenderer)uvr;
						*/

						layer.FeatureClass = fc;
						layer.Name = fc.AliasName;
						// Add the new layer.
						IMxDocument doc = (IMxDocument)owner.application.Document;
						doc.AddLayer(layer);
					}
				}
				else
				{
					MessageBox.Show("No PostGIS layers added.");
				}
            }
            catch (COMException COMex)
            {
                MessageBox.Show("Error " + COMex.ErrorCode.ToString() + ": " + COMex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private Button m_btn = null;
        private Button owner { get { return m_btn; } }

        public void show(Button ownerButton)
        {
            m_btn = ownerButton;
            Show(new ArcMapWindowWrapper(ownerButton.application));
			//remove all checked items
			for (int i = 0; i < clbLayers.Items.Count; i++)
			{
				if (clbLayers.GetItemChecked(i) == true)
				{
					clbLayers.SetItemChecked(i, false);
				}
			}
            Focus();
        }

        private void AddForm_Load(object sender, EventArgs e)
        {
            
        }

		/// <summary>
		/// Load PostGIS layers name
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void butGetLayers_Click(object sender, EventArgs e)
		{
			try
			{
				
			}
			catch (COMException COMex)
			{
				MessageBox.Show("Error " + COMex.ErrorCode.ToString() + ": " + COMex.Message);
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}
    }

    // This is a helper class so we can get the
    // ArcMap Win32 window owner as a SWF object.
    public class ArcMapWindowWrapper : IWin32Window
    {
        public ArcMapWindowWrapper(IApplication application)
        {
            m_hwnd = new IntPtr(application.hWnd);
        }

        private IntPtr m_hwnd;
        public IntPtr Handle { get { return m_hwnd; } }
    }
}
