// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.PackageHelper
// Author           : yayun
// Created          : 11-23-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="Program.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.PackageHelper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ServiceProcess;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;

    using LogUtil;

    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Configuration;
    using Microsoft.EnterpriseManagement.Configuration.IO;
    using Microsoft.EnterpriseManagement.Packaging;
    using Microsoft.Win32;

    using Models;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
#if DEBUG

        /// <summary>
        /// The run path.
        /// </summary>
        private static readonly string RunPath = @"E:\Projects\scom-plugin\SCOM\release\Configuration";
#else
        private static readonly string RunPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
        /// <summary>
        /// The port
        /// </summary>
        private static int port;

        /// <summary>
        /// The port
        /// </summary>
        private static string ipAddress;

        /// <summary>
        /// Gets or sets a value indicating whether is have exception.
        /// </summary>
        private static bool IsHaveException { get; set; }

        /// <summary>
        /// The service name.
        /// </summary>
        private static string ServiceName => "Huawei SCOM Plugin For eSight.Service";

        /// <summary>
        /// Gets the name of the e sight configuration library.
        /// </summary>
        /// <value>The name of the e sight configuration library.</value>
        private static string ESightConfigLibraryName => "eSight.Config.Library";

        /// <summary>
        /// Gets the framework install dir.
        /// </summary>
        /// <value>The framework install dir.</value>
        private static string FrameworkInstallDir => System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

        private static string ScomInstallPath { get; set; }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            try
            {
                ScomInstallPath = ReadScomInstallPath();
                //CreateESightConfigLibraryMp();
#if DEBUG
                //ReadScomInstallPath();
                //ResetESightSubscribeStatus();
                // CreateESightConfigLibraryMp();
                // ModifyWebServerConfig();
                // Install();
                // SealMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\sealTemp"));
                // CreateMP(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\Temp"), 44301);
                // UnInstall(); 
                Console.ReadLine();

#else
                if (args.Length == 0)
                {
                    return;
                }
                OnLog($"RunPath:{RunPath}");
                OnLog($"FrameworkInstallDir:{FrameworkInstallDir}");
                OnLog($"ScomInstallPath:{ScomInstallPath}");
                if (args[0] == "/u")
                {
                    if (!GetAttrValue(args, "keepIISExpress"))
                    {
                        UnInstallIISExpress();
                    }

                    UnInstall(GetAttrValue(args, "keepESight"));
                    OnLog("PackageHelper work done.");
                }
                else if (args[0] == "/i")
                {
                    if (!int.TryParse(GetParamValue(args, "port"), out port))
                    {
                        throw new Exception("port is error");
                    }
                    if (port < 0 || port > 65536)
                    {
                        throw new Exception("port is error");
                    }
                    ipAddress = GetParamValue(args, "ip");
                    OnLog($"ip:{ipAddress} port:{port}");
                    Install();
                    OnLog("PackageHelper work done.");
                }
                else if (args[0] == "/h")
                {
                    OnLog("/i install");
                    OnLog("/u uninstall");
                    Console.ReadLine();
                }
#endif
            }
            catch (Exception ex)
            {
                OnLog("Main Error", ex);
            }
        }

        /// <summary>
        /// Uns the subscribe all es session.
        /// </summary>
        private static void UnSubscribeAllESight()
        {
            try
            {
                OnLog("Start cancel eSight subcribe.");

                var eSights = ESightDal.Instance.GetList();
                foreach (var hweSightHost in eSights)
                {
                    try
                    {
                        var session = new ESightSession(hweSightHost);
                        var resut = session.UnSubscribeAlarm(hweSightHost.SystemID);
                        OnLog($"UnSubscribeAlarm.eSight:{hweSightHost.HostIP} result:{JsonUtil.SerializeObject(resut)}");
                        resut = session.UnSubscribeNeDevice(hweSightHost.SystemID);
                        OnLog($"UnSubscribeNeDevice.eSight:{hweSightHost.HostIP} result:{JsonUtil.SerializeObject(resut)}");
                    }
                    catch (Exception ex)
                    {
                        OnLog($"UnSubscribe eSight:{hweSightHost.HostIP}", ex);
                    }
                }
                OnLog("eSight cancel subcribe finish.");
            }
            catch (Exception ex)
            {
                OnLog("eSight cancel subcribe:", ex);
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>System.Boolean.</returns>
        private static string GetParamValue(string[] args, string attrName)
        {
            foreach (var s in args)
            {
                if (s.Contains(attrName))
                {
                    return s.Split('=')[1].Trim();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>System.Boolean.</returns>
        private static bool GetAttrValue(string[] args, string attrName)
        {
            foreach (var s in args)
            {
                if (s.Contains(attrName))
                {
                    return Convert.ToBoolean(s.Split('=')[1].Trim());
                }
            }
            return false;
        }

        /// <summary>
        /// The install.
        /// </summary>
        private static void Install()
        {
            SaveConfig();
            CopySdkFiles();
            ModifyWebServerConfig();
            ResetESightSubscribeStatus();
            CreateESightConfigLibraryMp();
            // 服务已安装 则跳过安装
            if (ServiceController.GetServices().All(s => s.ServiceName == ServiceName))
            {
                StopService();
                UnInstallService();
            }

            InstallService();
            if (!IsIISExpressInstalled())
            {
                InstallIISExpress();
            }
            InstallMpPlugin();
            InstallConnector();
            // StartService();
        }

        /// <summary>
        /// The un install.
        /// </summary>
        /// <param name="keepESight">The keep e sight.</param>
        private static void UnInstall(bool keepESight)
        {
            // 删除服务
            if (ServiceController.GetServices().Any(s => s.ServiceName == ServiceName))
            {
                StopService();
                UnInstallService();
            }
            else
            {
                OnLog("Huawei SCOM Plugin For eSight.Service is not installed");
            }

            // 卸载前先取消订阅
            UnSubscribeAllESight();

            if (!keepESight)
            {
                DeleteESight();
            }
            // RemoveSCOMServers();
            UnInstallMpPlugin();
            UnInstallConnector();
        }

        #region Install

        /// <summary>
        /// Copies the SDK files.
        /// </summary>
        private static void CopySdkFiles()
        {
            OnLog("CopySdkFiles Start.");
            var packagingFile = "Microsoft.EnterpriseManagement.Packaging.dll";
            var packagingFilePath = Path.Combine(ScomInstallPath, "Setup", packagingFile);

            var operationsManagerFile = "Microsoft.EnterpriseManagement.OperationsManager.dll";
            var operationsManagerFilePath = Path.Combine(ScomInstallPath, "Server\\SDK Binaries", operationsManagerFile);
            var coreFile = "Microsoft.EnterpriseManagement.Core.dll";
            var coreFilePath = Path.Combine(ScomInstallPath, "Server\\SDK Binaries", coreFile);
            var runtimeFile = "Microsoft.EnterpriseManagement.Runtime.dll";
            var runtimeFilePath = Path.Combine(ScomInstallPath, "Server\\SDK Binaries", runtimeFile);
            if (!File.Exists(packagingFilePath))
            {
                throw new Exception("can not find sdk dll " + packagingFile);
            }
            if (!File.Exists(operationsManagerFilePath))
            {
                throw new Exception("can not find sdk dll " + operationsManagerFile);
            }
            if (!File.Exists(coreFilePath))
            {
                throw new Exception("can not find sdk dll " + coreFile);
            }
            if (!File.Exists(runtimeFilePath))
            {
                throw new Exception("can not find sdk dll " + runtimeFile);
            }

            File.Copy(packagingFilePath, Path.Combine(RunPath, packagingFile), true);
            File.Copy(operationsManagerFilePath, Path.Combine(RunPath, operationsManagerFile), true);
            File.Copy(coreFilePath, Path.Combine(RunPath, coreFile), true);
            File.Copy(runtimeFilePath, Path.Combine(RunPath, runtimeFile), true);
            OnLog("CopySdkFiles Finish.");
        }

        /// <summary>
        /// Resets the e sight subscribe status.
        /// </summary>
        private static void ResetESightSubscribeStatus()
        {
            try
            {
                var filePath = RunPath + "\\KN\\eSight.bin";
                if (File.Exists(filePath))
                {
                    List<HWESightHost> list;
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost> ?? new List<HWESightHost>();
                        list.ForEach(x =>
                            {
                                x.SubscriptionAlarmStatus = 0;
                                x.SubscriptionNeDeviceStatus = 0;
                                x.SubscripeAlarmError = string.Empty;
                                x.SubscripeNeDeviceError = string.Empty;
                                x.LatestConnectInfo = string.Empty;
                                x.LatestStatus = "Ready";
                            });
                        fs.Dispose();
                    }
                    using (var fsSave = new FileStream(filePath, FileMode.Create))
                    {
                        new BinaryFormatter().Serialize(fsSave, list);
                        fsSave.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog("ResetESightSubscribeStatus Error:" + ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// Installs the connector.
        /// </summary>
        private static void InstallConnector()
        {
            try
            {
                OnLog("Install Connectors");
                BladeConnector.Instance.Init();
                RackConnector.Instance.Init();
                HighdensityConnector.Instance.Init();
                KunLunConnector.Instance.Init();
                OnLog("Install Connectors success");
            }
            catch (Exception ex)
            {
                OnLog("Install Connector faild", ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// The install iis express.
        /// </summary>
        private static void InstallIISExpress()
        {
            try
            {
                OnLog("Install IISExpress");
                var process = new Process
                {
                    StartInfo =
                     {
                         FileName = "msiexec",
                         Arguments =
                             $" /i \"{RunPath}\\iisexpress_amd64_en-US.msi\" /qn /l* iisexpress_amd64_en-US.install.log",
                         Verb = "runas",
                         WindowStyle = ProcessWindowStyle.Hidden,
                         UseShellExecute = false,
                         RedirectStandardInput = true,
                         RedirectStandardOutput = true,
                         RedirectStandardError = true
                     }
                };
                process.OutputDataReceived += (s, e) => { OnLog(e.Data); };
                process.ErrorDataReceived += (s, e) => { OnLog(e.Data); };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit(60000);
            }
            catch (Exception ex)
            {
                IsHaveException = true;
                OnLog("Install IISExpress faild", ex);
            }
        }

        /// <summary>
        /// The install mp plugin.
        /// </summary>
        private static void InstallMpPlugin()
        {
            try
            {
                OnLog("Start Install eSight ManagementPacks");
#if DEBUG
                MGroup.Instance.InstallMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\eSight.View.Library.mpb"));
                MGroup.Instance.InstallMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\eSight.BladeServer.Library.mpb"));
                MGroup.Instance.InstallMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\eSight.HighDensityServer.Library.mpb"));
                MGroup.Instance.InstallMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\eSight.RackServer.Library.mpb"));
                MGroup.Instance.InstallMpb(Path.GetFullPath("..\\..\\..\\..\\..\\..\\..\\release\\MPFiles\\eSight.KunLunServer.Library.mpb"));
#else
                OnLog("Start Install eSight.View.Library");
                MGroup.Instance.InstallMpb(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\eSight.View.Library.mpb"));
                OnLog($"Start Install {ESightConfigLibraryName}");
                MGroup.Instance.InstallMp(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\{ESightConfigLibraryName}.mp"));
                OnLog("Start Install eSight.BladeServer.Library");
                MGroup.Instance.InstallMpb(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\eSight.BladeServer.Library.mpb"));
                OnLog("Start Install eSight.HighDensityServer.Library");
                MGroup.Instance.InstallMpb(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\eSight.HighDensityServer.Library.mpb"));
                OnLog("Start Install eSight.RackServer.Library");
                MGroup.Instance.InstallMpb(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\eSight.RackServer.Library.mpb"));
                OnLog("Start Install eSight.KunLunServer.Library");
                MGroup.Instance.InstallMpb(Path.GetFullPath($"{RunPath}\\..\\MPFiles\\eSight.KunLunServer.Library.mpb"));
#endif
                OnLog("Install eSight ManagementPacks Finish.");
            }
            catch (Exception ex)
            {
                IsHaveException = true;
                OnLog("Install ManagementPack faild.", ex);
            }
        }

        /// <summary>
        /// The install service.
        /// </summary>
        private static void InstallService()
        {
            try
            {
                OnLog($"Install {ServiceName} as Windows Service");
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = $"{FrameworkInstallDir}\\InstallUtil.exe",
                        Arguments =
                            $" \"{RunPath}\\Huawei.SCOM.ESightPlugin.Service.exe\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                process.OutputDataReceived += (s, e) => { OnLog(e.Data); };
                process.ErrorDataReceived += (s, e) => { OnLog(e.Data); };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit(60000);
            }
            catch (Exception ex)
            {
                IsHaveException = true;
                OnLog("UnInstall Connector faild", ex);
            }
        }

        /// <summary>
        /// The is iis express installed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        private static bool IsIISExpressInstalled()
        {
            var path = Path.GetFullPath(
                $"{Environment.GetEnvironmentVariable("ProgramFiles(x86)")}\\IIS Express\\iisexpress.exe");
            return File.Exists(path);
        }

        #endregion

        #region UnInstall

        /// <summary>
        /// The kill.
        /// </summary>
        private static void Kill()
        {
            OnLog("Kill Service Process Start");
            try
            {
                var p = Process.GetProcessesByName("Huawei.SCOM.ESightPlugin.Service");
                if (p.Length > 0)
                {
                    OnLog("Kill Process : Huawei.SCOM.ESightPlugin.Service");
                    p.First().Kill();
                }

                p = Process.GetProcessesByName("iisexpress");
                if (p.Length > 0)
                {
                    OnLog("Kill Process : iisexpress");
                    p.First().Kill();
                }
            }
            catch (Exception ex)
            {
                IsHaveException = true;
                OnLog("Kill Process faild", ex);
            }
        }

        /// <summary>
        /// delete eSight data.
        /// </summary>
        private static void DeleteESight()
        {
            try
            {
                OnLog("Start delete eSight .");
                ESightDal.Instance.DeleteDbFile();
                OnLog("eSight delete finish.");
            }
            catch (Exception ex)
            {
                OnLog("eSight delete :", ex);
            }
        }

        /// <summary>
        /// 删除所有管理的HuaweiServer对象
        /// </summary>
        private static void RemoveSCOMServers()
        {
            try
            {
                OnLog($"Remove eSight Servers From SCOM Start.");
                BladeConnector.Instance.RemoverAllBlade();
                RackConnector.Instance.RemoverAllRack();
                HighdensityConnector.Instance.RemoverAllHighdensity();
                KunLunConnector.Instance.RemoverAllKunLun();
                OnLog($"Remove eSight Servers From SCOM Finish.");
            }
            catch (Exception ex)
            {
                OnLog("Remove eSight Servers From SCOM error", ex);
            }
        }

        /// <summary>
        /// The start service.
        /// </summary>
        private static void StartService()
        {
            try
            {
                OnLog("Start Huawei SCOM Plugin For eSight.Service");
                var sc = new ServiceController { ServiceName = "Huawei SCOM Plugin For eSight.Service" };
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                }
                OnLog("Start Service success");
            }
            catch (Exception ex)
            {
                OnLog("Start Service faild", ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        private static void StopService()
        {
            try
            {
                OnLog($"Stop {ServiceName}");
                var service = new ServiceController { ServiceName = ServiceName };
                if (service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.StartPending || service.Status == ServiceControllerStatus.ContinuePending)
                {
                    service.Stop();
                    for (int i = 0; i < 60; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            break;
                        }
                        if (i == 59)
                        {
                            throw new Exception("Stop Service Error：" + ServiceName);
                        }
                    }
                }
                OnLog("Stop Service success");
            }
            catch (Exception ex)
            {
                OnLog("Stop Service faild", ex);
                Kill(); // 服务停止失败 杀死进程。
                IsHaveException = true;
            }
        }

        /// <summary>
        /// UnInstalls the connector.
        /// </summary>
        private static void UnInstallConnector()
        {
            try
            {
                OnLog("UnInstall Connector");

                OnLog("Start UnInstall BladeConnector");
                MGroup.Instance.UnInstallConnector(BladeConnector.Instance.ConnectorGuid);
                OnLog("Start UnInstall RackConnector");
                MGroup.Instance.UnInstallConnector(RackConnector.Instance.ConnectorGuid);
                OnLog("Start UnInstall HighdensityConnector");
                MGroup.Instance.UnInstallConnector(HighdensityConnector.Instance.ConnectorGuid);
                OnLog("Start UnInstall KunLunConnector");
                MGroup.Instance.UnInstallConnector(KunLunConnector.Instance.ConnectorGuid);
                OnLog("UnInstall Connector success");
            }
            catch (Exception ex)
            {
                OnLog("UnInstall Connector faild", ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// UnInstalls the IIS express.
        /// </summary>
        private static void UnInstallIISExpress()
        {
            try
            {
                OnLog("Start UnInstall IISExpress");

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec",
                        Arguments =
                            $" /x \"{RunPath}\\iisexpress_amd64_en-US.msi\" /qn /l* iisexpress_amd64_en-US.uninstall.log",
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                process.OutputDataReceived += (s, e) => { OnLog(e.Data); };
                process.ErrorDataReceived += (s, e) => { OnLog(e.Data); };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit(60000);

                OnLog("UnInstall IISExpress success");
            }
            catch (Exception ex)
            {
                OnLog("Stop Service faild", ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// UnInstalls the mp.
        /// </summary>
        private static void UnInstallMpPlugin()
        {
            try
            {
                OnLog("Start UnInstall eSight ManagementPacks");
                OnLog("Start UnInstall eSight.BladeServer.Library");
                MGroup.Instance.UnInstallMp("eSight.BladeServer.Library");
                OnLog("Start UnInstall eSight.HighDensityServer.Library");
                MGroup.Instance.UnInstallMp("eSight.HighDensityServer.Library");
                OnLog("Start UnInstall eSight.RackServer.Library");
                MGroup.Instance.UnInstallMp("eSight.RackServer.Library");
                OnLog("Start UnInstall eSight.KunLunServer.Library");
                MGroup.Instance.UnInstallMp("eSight.KunLunServer.Library");
                OnLog($"Start UnInstall {ESightConfigLibraryName}");
                MGroup.Instance.UnInstallMp(ESightConfigLibraryName);
                OnLog("Start UnInstall eSight.View.Library");
                MGroup.Instance.UnInstallMp("eSight.View.Library");
                OnLog("UnInstall eSight ManagementPacks Finish.");
            }
            catch (Exception ex)
            {
                OnLog("UnInstall eSight ManagementPacks faild", ex);
                IsHaveException = true;
            }
        }

        /// <summary>
        /// Uns the install service.
        /// </summary>
        private static void UnInstallService()
        {
            try
            {
                OnLog($"Start UnInstall {ServiceName}");
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = $"{FrameworkInstallDir}\\InstallUtil.exe",
                        Arguments =
                            $"/u \"{RunPath}Huawei.SCOM.ESightPlugin.Service.exe\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                process.OutputDataReceived += (s, e) => { OnLog(e.Data); };
                process.ErrorDataReceived += (s, e) => { OnLog(e.Data); };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit(60000);
            }
            catch (Exception ex)
            {
                IsHaveException = true;
                OnLog("UnInstall IISExpress faild", ex);
            }
        }
        #endregion

        #region Utils

        /// <summary>
        /// The on log.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        private static void OnLog(string data)
        {
            HWLogger.UPDATER.Info(data);
            Console.WriteLine(data);
        }

        /// <summary>
        /// The on log.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private static void OnLog(string data, Exception ex)
        {
            HWLogger.UPDATER.Error(data, ex);
            Console.WriteLine(data);
            Console.WriteLine(ex);
        }

        /// <summary>
        /// Reads the scom install path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string ReadScomInstallPath()
        {
            string softPath = @"SOFTWARE\Microsoft\System Center Operations Manager\12\Setup";
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            RegistryKey installKey = localKey.OpenSubKey(softPath, false);
            if (installKey == null)
            {
                throw new Exception($"can not find the installKey: {softPath}");
            }
            var installPath = installKey.GetValue("InstallDirectory").ToString();
            return installPath;
        }
        #endregion

        #region Modify applicationhost.config

        /// <summary>
        /// Modifies the web server configuration.
        /// </summary>
        private static void ModifyWebServerConfig()
        {
            try
            {
                var filePath = Path.Combine(RunPath, "applicationhost.config");
                // filePath= Path.Combine("..\\..\\..\\..\\..\\..\\..\\release\\Configuration\\", "applicationhost.config");
                // 读取文本　  
                StreamReader sr = new StreamReader(filePath);
                string str = sr.ReadToEnd();
                sr.Close();
                // 替换文本  
                str = str.Replace("44301", port.ToString());
                // 更改保存文本  
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.WriteLine(str);
                sw.Close();
            }
            catch (Exception ex)
            {
                OnLog("Modify applicationhost.config faild", ex);

                throw;
            }

        }
        #endregion

        /// <summary>
        /// Creates the mp.
        /// </summary>
        private static void CreateESightConfigLibraryMp()
        {
            try
            {
                // var apmMpPath = @"E:\Projects\scom-plugin\SCOM\release\MPFiles\Temp";
                OnLog("CreateESightConfigLibraryMp-ScomInstallPath:" + ScomInstallPath);
                var apmMpPath = $"{ScomInstallPath}\\Server\\ApmConnector";
                OnLog(apmMpPath);
                var keyPath = $"{RunPath}\\..\\MPFiles\\Temp";
                string outPath = $"{RunPath}\\..\\MPFiles";
                string companyName = "广州摩赛网络技术有限公司";
                string copyRight = "Copyright (c) 广州摩赛网络技术有限公司. All rights reserved.";
                string keyName = "esight.snk";

                #region read eSight.View.Library
                var mpStore = new ManagementPackFileStore();
                mpStore.AddDirectory(outPath);

                var reader = ManagementPackBundleFactory.CreateBundleReader();
                var bundle = reader.Read($"{outPath}\\eSight.View.Library.mpb", mpStore);
                var eSightViewMp = bundle.ManagementPacks.FirstOrDefault();
                if (eSightViewMp == null)
                {
                    throw new Exception($"can not find mp : eSight.View.Library.mpb");
                }

                #endregion

                var mMpStore = new ManagementPackFileStore();
                mMpStore.AddDirectory(apmMpPath);
                mMpStore.AddDirectory(outPath);
                var mMp = new ManagementPack(ESightConfigLibraryName, ESightConfigLibraryName, eSightViewMp.Version, mMpStore);

                #region AddReferences
                var mLibraryMp = new ManagementPack($"{apmMpPath}\\System.Library.mp", mMpStore);
                var mWindowsLibraryMp = new ManagementPack($"{apmMpPath}\\Microsoft.SystemCenter.Library.mp", mMpStore);
                mMp.References.Add("System", new ManagementPackReference(mLibraryMp));
                mMp.References.Add("SC", new ManagementPackReference(mWindowsLibraryMp));
                mMp.References.Add("EVL", new ManagementPackReference(eSightViewMp));
                #endregion

                #region AddView

                var view = new ManagementPackView(mMp, "ESight.Config.ESightConfigView", ManagementPackAccessibility.Public)
                {
                    Target = mLibraryMp.GetClass("System.WebSite"),
                    TypeID = mWindowsLibraryMp.GetViewType("Microsoft.SystemCenter.UrlViewType"),
                    Description = "eSight Config View",
                    DisplayName = "eSight Config View",
                    Category = "Operations",
                    Configuration = $"<Criteria><Url>https://localhost:{port}/StaticWeb/eSight.html</Url></Criteria><Presentation></Presentation><Target></Target>"
                };

                var folderItem = new ManagementPackFolderItem("ESight.Config.ESightConfigView.FolderItem", view, eSightViewMp.GetFolder("ESight.Folder"));
                folderItem.Status = ManagementPackElementStatus.PendingAdd;
                #endregion

                mMp.AcceptChanges();

                #region seal

                var mpWriterSettings = new ManagementPackAssemblyWriterSettings(companyName, Path.Combine(keyPath, keyName), false)
                {
                    OutputDirectory = outPath,
                    Copyright = copyRight
                };

                ManagementPackAssemblyWriter mpWriter = new ManagementPackAssemblyWriter(mpWriterSettings);
                mpWriter.WriteManagementPack(mMp);
                #endregion
                // Remove Temp files
                if (Directory.Exists(keyPath))
                {
                    Directory.Delete(keyPath, true);
                }
                var tempFile = Path.Combine(RunPath, "..\\", "MPResources.resources");
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                OnLog($"create {ESightConfigLibraryName} faild", ex);
                throw;
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        private static void SaveConfig()
        {
            var config = new PluginConfig
            {
                InternetIp = ipAddress,
                InternetPort = port,
                PollingInterval = 14400000,
                TempTcpPort = 40001
            };
            var path = $"{RunPath}\\PluginConfig.xml";
            ConfigHelper.SavePluginConfig(config, path);
        }
    }
}