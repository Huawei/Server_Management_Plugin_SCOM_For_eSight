// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 11-22-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="MGroup.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The m group.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Timers;

    using LogUtil;

    using Microsoft.EnterpriseManagement;
    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Configuration;
    using Microsoft.EnterpriseManagement.Configuration.IO;
    using Microsoft.EnterpriseManagement.ConnectorFramework;
    using Microsoft.EnterpriseManagement.Packaging;

    /// <summary>
    /// The m group.
    /// </summary>
    public class MGroup : ManagementGroup
    {
        /// <summary>
        /// The this.
        /// </summary>
        private static MGroup instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MGroup"/> class.
        /// </summary>
        /// <param name="serverName">
        /// The server name.
        /// </param>
        public MGroup(string serverName)
            : base(serverName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MGroup"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public MGroup(ManagementGroupConnectionSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static MGroup Instance
        {
            get
            {
                if (instance == null)
                {
#if DEBUG
                    var settings = new ManagementGroupConnectionSettings("192.168" + ".10.83")
                    {
                        UserName = "Administrator",
                        Domain = "ADTEST",//"MOSAI",
                        Password = ConvertToSecureString("Simple.0"),//Mosai@520
                    };
                    instance = new MGroup(settings);
#else

                    instance = new MGroup("localhost");
#endif
                }

                return instance;
            }
        }

        /// <summary>
        /// 检查MP文件是否已安装
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// <c>true</c> if XXXX, <c>false</c> otherwise.
        /// </returns>
        public bool CheckIsInstallMp(string name)
        {
            var criteria = new ManagementPackCriteria($"Name = '{name}'");
            return this.ManagementPacks.GetManagementPacks(criteria).Count > 0;
        }

        /// <summary>
        /// The get connector.
        /// </summary>
        /// <param name="connectorGuid">
        /// The connector guid.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringConnector"/>.
        /// </returns>
        public MonitoringConnector GetConnector(Guid connectorGuid)
        {
            var cfMgmt = this.GetConnectorFramework();
            try
            {
                var montioringConnector = cfMgmt.GetConnector(connectorGuid);
                return montioringConnector;
            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            Timer timer = new Timer(5 * 60 * 1000)
            {
                Enabled = true,
                AutoReset = true,
            };
            timer.Elapsed += (s, e) =>
                {
                    try
                    {
                        // 保持对scom 的连接
                        this.GetManagementPackClass("Microsoft.Windows.Computer");
                    }
                    catch (Exception ex)
                    {
                        HWLogger.SERVICE.Error("keep Management Group Connection error", ex);
                        this.Reconnect();
                    }
                };
            timer.Start();
        }

        /// <summary>
        /// The get connector framework.
        /// </summary>
        /// <returns>
        /// The <see cref="IConnectorFrameworkManagement"/>.
        /// </returns>
        public IConnectorFrameworkManagement GetConnectorFramework()
        {
            var icfm = this.ConnectorFramework;
            return icfm;
        }

        /// <summary>
        /// The get management pack.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ManagementPack"/>.
        /// </returns>
        public ManagementPack GetManagementPack(string name)
        {
            var criteria = new ManagementPackCriteria($"Name = '{name}'");
            return this.ManagementPacks.GetManagementPacks(criteria).FirstOrDefault();
        }

        /// <summary>
        /// The get management pack class.
        /// </summary>
        /// <param name="className">
        /// The class name.
        /// </param>
        /// <returns>
        /// The <see cref="ManagementPackClass"/>.
        /// </returns>
        /// <exception cref="ApplicationException">Failed to find management pack class </exception>
        public ManagementPackClass GetManagementPackClass(string className)
        {
            IList<ManagementPackClass> mpClasses;

            mpClasses = this.EntityTypes.GetClasses(new ManagementPackClassCriteria("Name='" + className + "'"));

            if (mpClasses.Count == 0)
            {
                throw new ApplicationException("Failed to find management pack class " + className);
            }
            return mpClasses[0];
        }

        /// <summary>
        /// The get management pack relationship.
        /// </summary>
        /// <param name="relationshipName">
        /// The relationship name.
        /// </param>
        /// <returns>
        /// The <see cref="ManagementPackRelationship"/>.
        /// </returns>
        /// <exception cref="ApplicationException">Failed to find monitoring relationship  </exception>
        public ManagementPackRelationship GetManagementPackRelationship(string relationshipName)
        {
            IList<ManagementPackRelationship> relationshipClasses;

            relationshipClasses =
                this.EntityTypes.GetRelationshipClasses(
                    new ManagementPackRelationshipCriteria("Name='" + relationshipName + "'"));

            if (relationshipClasses.Count == 0)
            {
                throw new ApplicationException("Failed to find monitoring relationship " + relationshipName);
            }
            return relationshipClasses[0];
        }

        /// <summary>
        /// 安装MP
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public void InstallMp(string path)
        {
            var newMp = new ManagementPack(path);
            var criteria = new ManagementPackCriteria($"Name = '{newMp.Name}'");
            var oldMp = this.ManagementPacks.GetManagementPacks(criteria).FirstOrDefault();
            if (oldMp != null)
            {
                // 已安装则跳过
                HWLogger.UPDATER.Warn($"Skip install：{oldMp.Name}-{oldMp.Version} has Installed.");
                //// 已安装
                // if (oldMp.Version != newMp.Version)
                // {
                // // 版本不一致则先进行卸载
                // this.UnInstallMp(oldMp.Name);
                // this.ManagementPacks.ImportManagementPack(newMp);
                // Console.WriteLine($"Install {newMp.Name} Finish.");
                // }
                // else
                // {
                // HWLogger.UPDATER.Warn($"Skip install：{newMp.Name}-{newMp.Version} has Installed.");
                // }
            }
            else
            {
                this.ManagementPacks.ImportManagementPack(newMp);
                HWLogger.UPDATER.Warn($"Install {newMp.Name} Finish.");
            }
        }

        /// <summary>
        /// Installs the MPB.
        /// </summary>
        /// <param name="path">The path.</param>
        public void InstallMpb(string path)
        {
            var mpStore = new ManagementPackFileStore();
            mpStore.AddDirectory(Path.GetDirectoryName(path));

            var reader = ManagementPackBundleFactory.CreateBundleReader();
            var bundle = reader.Read(path, mpStore);

            var newMp = bundle.ManagementPacks.FirstOrDefault();
            if (newMp == null)
            {
                HWLogger.UPDATER.Warn($"Install faild. can not find ManagementPack in the path :{path}");
                return;
            }

            var criteria = new ManagementPackCriteria($"Name = '{newMp.Name}'");
            var oldMp = this.ManagementPacks.GetManagementPacks(criteria).FirstOrDefault();
            if (oldMp != null)
            {
                HWLogger.UPDATER.Warn($"Skip install：{oldMp.Name}-{oldMp.Version} has Installed.");
            }
            else
            {
                this.ManagementPacks.ImportBundle(bundle);
                HWLogger.UPDATER.Warn($"Install {newMp.Name} Finish.");
            }
        }

        /// <summary>
        /// The un install connector.
        /// </summary>
        /// <param name="connectorGuid">The connector guid.</param>
        /// <exception cref="Exception">Error</exception>
        public void UnInstallConnector(Guid connectorGuid)
        {
            var montioringConnector = this.GetConnector(connectorGuid);
            Console.WriteLine($"Start Uninstall connector: {connectorGuid}");
            HWLogger.SERVICE.Info($"Start Uninstall connector {connectorGuid}");
            try
            {
                if (montioringConnector != null)
                {
                    var connectorName = montioringConnector.Name;
                    var icfm = this.GetConnectorFramework();
                    IList<MonitoringConnectorSubscription> subscriptions =
                        icfm.GetConnectorSubscriptions().Where(c => c.MonitoringConnectorId.Equals(connectorGuid))
                            .ToList();
                    foreach (var subscription in subscriptions)
                    {
                        icfm.DeleteConnectorSubscription(subscription);
                    }
                    try
                    {
                        montioringConnector.Uninitialize();
                    }
                    catch (Exception ex)
                    {
                        HWLogger.SERVICE.Error($"Error on {connectorName} Uninitialize.", ex);
                        Console.WriteLine($" Error on {connectorName} Uninitialize", ex);
                    }

                    icfm.Cleanup(montioringConnector);
                }
                else
                {
                    HWLogger.SERVICE.Info($"Error uninstalling : Can not find connector: {connectorGuid}");
                    Console.WriteLine($"Error uninstalling :Can not find connector: {connectorGuid} ");
                }
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error("Error uninstalling connector...", ex);
                Console.WriteLine($" Error uninstalling {connectorGuid} ", ex);
                throw;
            }

            Console.WriteLine($"{connectorGuid} UnInstall finish");
        }

        /// <summary>
        /// Uninstalls the mp.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void UnInstallMp(string name)
        {
            var criteria = new ManagementPackCriteria($"Name = '{name}'");
            var mp = this.ManagementPacks.GetManagementPacks(criteria).FirstOrDefault();
            if (mp == null)
            {
                Console.WriteLine($"ManagementPack {name} is not installed.");
            }
            else
            {
                this.ManagementPacks.UninstallManagementPack(mp);
                Console.WriteLine($"Uninstall {name} Finish.");
            }
        }

        /// <summary>
        /// Checks the connection.
        /// </summary>
        public void CheckConnection()
        {
            if (!this.IsConnected)
            {
                HWLogger.SERVICE.Info("Reconnect");
                this.Reconnect();
            }
        }
#if DEBUG
        /// <summary>
        /// ConvertToSecureString
        /// </summary>
        /// <param name="pd">pd</param>
        /// <returns>SecureString</returns>
        private static SecureString ConvertToSecureString(string pd)
        {
            if (pd == null)
            {
                throw new ArgumentNullException("pd");
            }

            var securePd = new SecureString();
            foreach (char c in pd)
            {
                securePd.AppendChar(c);
            }
            securePd.MakeReadOnly();
            return securePd;
        }
#endif
    }
}