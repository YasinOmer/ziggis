using System;
using System.Drawing;
using System.Windows.Forms;
using ZigGis.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;

#if ARCGIS_8X
using ESRI.ArcObjects.Core;
#else
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
#endif

namespace ZigGis.ArcGIS.ArcMapUI
{
    public partial class AddForm : Form
    {
        public AddForm()
        {
            InitializeComponent();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDlg.ShowDialog(this);
            if (res == DialogResult.OK)
                zigFile.Text = openFileDlg.FileName;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            Hide();
            owner.application.RefreshWindow();
            try
            {
                // Open workspace and feature class.
                IWorkspaceFactory wksf = new PostGisWksFactory();
                IFeatureWorkspace fwks = (IFeatureWorkspace)wksf.OpenFromFile(zigFile.Text, 0);
                IFeatureClass fc = fwks.OpenFeatureClass(postgisView.Text);
                // Create the new layer.
                IFeatureLayer layer = new PostGisFeatureLayer();
                layer.FeatureClass = fc;
                layer.Name = fc.AliasName;
                // Add the new layer.
                IMxDocument doc = (IMxDocument)owner.application.Document;
                doc.AddLayer(layer);
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
            Focus();
        }

        private void AddForm_Load(object sender, EventArgs e)
        {
            
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