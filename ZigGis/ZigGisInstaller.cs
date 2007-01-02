using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;

namespace ZigGis.Installation
{
    [RunInstaller(true)]
    public partial class ZigGisInstaller : Installer
    {
        public ZigGisInstaller()
        {
            InitializeComponent();
        }
        
        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                RegistrationServices regsrv = new RegistrationServices();
                if (!regsrv.RegisterAssembly(base.GetType().Assembly, AssemblyRegistrationFlags.SetCodeBase))
                {
                    throw (new InstallException("Failed To Register for COM"));

                }
            }
            catch (InstallException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error during installation");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                RegistrationServices regsrv = new RegistrationServices();

                if (!regsrv.UnregisterAssembly(base.GetType().Assembly))
                {
                    throw (new InstallException("Failed To Unregister for COM"));

                }
            }
            catch (InstallException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error during unistallation");
            }
        } 
    }
}