using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += ProjectInstaller_AfterInstall;
        }

        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
           
        }
    }
}
