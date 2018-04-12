// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Service
// Author           : suxiaobo
// Created          : 12-12-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="ESightPluginService.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The e sight plugin service.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Server;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;

    using LogUtil;

    using Microsoft.EnterpriseManagement;

    using Newtonsoft.Json;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The e sight plugin service.
    /// </summary>
    public partial class ESightPluginService : ServiceBase
    {
        /// <summary>
        /// The polling timer
        /// </summary>
        private Timer pollingTimer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ESightPluginService"/> class.
        /// </summary>
        public ESightPluginService()
        {
            this.InitializeComponent();
            this.SyncInstances = new List<ESightSyncInstance>();
            this.CanHandlePowerEvent = true;
        }
        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the synchronize instances.
        /// </summary>
        /// <value>The synchronize instances.</value>
        public List<ESightSyncInstance> SyncInstances { get; set; }

        /// <summary>
        /// The env variable.
        /// </summary>
        public string EnvVariable => "ESIGHTSCOMPLUGIN";

        /// <summary>
        /// Gets or sets the iis process.
        /// </summary>
        public Process IISProcess { get; set; }

        /// <summary>
        /// The install path.
        /// </summary>
        public string InstallPath => Environment.GetEnvironmentVariable(this.EnvVariable);

        /// <summary>
        /// 接收web转发的订阅消息
        /// </summary>
        /// <value>The TCP server taxk.</value>
        public Task TcpServerTask { get; private set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// The debug.
        /// </summary>
        public void Debug()
        {
            this.OnStart(new[] { string.Empty });
            //this.CreateTcpServer();
        }

        #endregion

        #region Task
        /// <summary>
        /// The on start.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <exception cref="Exception"> ex
        /// </exception>
        protected override void OnStart(string[] args)
        {
            try
            {
                var maxTryTimes = 0;
                while (true)
                {
                    maxTryTimes++;
                    if (this.CheckScomConnection())
                    {
                        this.Start();
                        break;
                    }
                    else
                    {
                        if (maxTryTimes >= 200)
                        {
                            this.OnError($"Stop retry: maxTryTimes:{maxTryTimes}");
                            break;
                        }
                        this.OnError("The Data Access service is either not running or not yet initialized,try again 3 seconds later.");
                        Thread.Sleep(3000);
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnLog("Start Service Error：" + ex);
                this.Stop();
            }
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        /// <exception cref="Exception">ex
        /// </exception>
        protected override void OnStop()
        {
            try
            {
                if (this.IISProcess != null)
                {
                    if (!this.IISProcess.HasExited)
                    {
                        this.OnLog("Kill IISProcess");
                        this.IISProcess.Kill();
                    }
                    else
                    {
                        this.OnLog("IISProcess Has Exited");
                    }
                }
                this.OnLog("Stop Service Success");
            }
            catch (Exception ex)
            {
                this.OnLog("Stop Service Error：" + ex);
            }
        }

        /// <summary>
        /// Called when [power event].
        /// </summary>
        /// <param name="powerStatus">The power status.</param>
        /// <returns>System.Boolean.</returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            string message = $" powerStatus:{powerStatus} tcpServerTaskStatus:{this.TcpServerTask.Status} this.pollingTimer{this.pollingTimer.Enabled} ";
            this.OnLog(message);
            return true;
        }

        /// <summary>
        /// Called when [continue].
        /// </summary>
        protected override void OnContinue()
        {
            string message = $"OnContinue: tcpServerTaskStatus:{this.TcpServerTask.Status} this.pollingTimer{this.pollingTimer.Enabled} ";
            this.OnLog(message);
        }

        /// <summary>
        /// Called when [pause].
        /// </summary>
        protected override void OnPause()
        {
            string message = $"OnPause: tcpServerTaskStatus:{this.TcpServerTask.Status} this.pollingTimer{this.pollingTimer.Enabled} ";
            this.OnLog(message);
        }

        /// <summary>
        /// Checks the connection.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        private bool CheckScomConnection()
        {
            try
            {
                MGroup.Instance.Init();
            }
            catch (Exception ex)
            {
                if (ex.GetType().ToString() != "Microsoft.EnterpriseManagement.Common.ServiceNotRunningException")
                {
                    this.OnError("CheckScomConnection", ex);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <exception cref="Exception"> ex
        /// </exception>
        private void Start()
        {
            this.OnLog("env:ESIGHTSCOMPLUGIN：" + this.InstallPath);
            if (string.IsNullOrEmpty(this.InstallPath))
            {
                throw new Exception("Can not find the environment variable \"ESIGHTSCOMPLUGIN\"");
            }

            this.CheckAndInstallMp();

            this.TcpServerTask = Task.Run(() => this.CreateTcpServer());
#if !DEBUG
            this.RunPolling();
#endif
            this.RunSync(); // 先执行一次 

            this.RunWebServer();

            this.ImportToRoot();

            this.OnLog("Service Start Success");
        }

        /// <summary>
        ///     Checks the and install mp.
        /// </summary>
        private void CheckAndInstallMp()
        {
#if DEBUG
            if (!MGroup.Instance.CheckIsInstallMp("eSight.View.Library"))
            {
                var path = $"{this.InstallPath}\\MPFiles\\eSight.View.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }

            // if (!MGroup.Instance.CheckIsInstallMp("eSight.BladeServer.Library"))
            // {
            // string path = $"{InstallPath}\\MPFiles\\eSight.BladeServer.Library.mpb";
            // MGroup.Instance.InstallMp(path);
            // }
            // if (!MGroup.Instance.CheckIsInstallMp("eSight.HighDensityServer.Library"))
            // {
            // string path = $"{InstallPath}\\MPFiles\\eSight.HighDensityServer.Library.mpb";
            // MGroup.Instance.InstallMp(path);
            // }
            // if (!MGroup.Instance.CheckIsInstallMp("eSight.RackServer.Library"))
            // {
            // string path = $"{InstallPath}\\MPFiles\\eSight.RackServer.Library.mpb";
            // MGroup.Instance.InstallMp(path);
            // }
            // if (!MGroup.Instance.CheckIsInstallMp("eSight.KunLunServer.Library"))
            // {
            // string path = $"{InstallPath}\\MPFiles\\eSight.KunLunServer.Library.mpb";
            // MGroup.Instance.InstallMp(path);
            // }
#else
            if (!MGroup.Instance.CheckIsInstallMp("eSight.View.Library"))
            {
                string path = $"{this.InstallPath}\\MPFiles\\eSight.View.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }
            if (!MGroup.Instance.CheckIsInstallMp("eSight.BladeServer.Library"))
            {
                string path = $"{this.InstallPath}\\MPFiles\\eSight.BladeServer.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }
            if (!MGroup.Instance.CheckIsInstallMp("eSight.HighDensityServer.Library"))
            {
                string path = $"{this.InstallPath}\\MPFiles\\eSight.HighDensityServer.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }
            if (!MGroup.Instance.CheckIsInstallMp("eSight.RackServer.Library"))
            {
                string path = $"{this.InstallPath}\\MPFiles\\eSight.RackServer.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }
            if (!MGroup.Instance.CheckIsInstallMp("eSight.KunLunServer.Library"))
            {
                string path = $"{this.InstallPath}\\MPFiles\\eSight.KunLunServer.Library.mpb";
                MGroup.Instance.InstallMpb(path);
            }
#endif
        }

        /// <summary>
        ///     开始轮询
        /// </summary>
        private void RunPolling()
        {
#if DEBUG
            this.pollingTimer = new Timer(5 * 60 * 1000) { Enabled = true, AutoReset = true };
            this.pollingTimer.Elapsed += (s, e) => { this.RunSync(); };
            this.pollingTimer.Start();
#else
            var config = ConfigHelper.GetPluginConfig();
            this.pollingTimer = new Timer(config.PollingInterval)
            {
                Enabled = true,
                AutoReset = true,
            };
            this.pollingTimer.Elapsed += (s, e) =>
                {
                    this.RunSync();
                };
            this.pollingTimer.Start();
#endif
        }

        /// <summary>
        ///     执行轮询任务
        /// </summary>
        private void RunSync()
        {
            try
            {
                this.OnLog("Start Sync Task");

                var hwESightHostList = ESightDal.Instance.GetList();
                if (hwESightHostList != null)
                {
                    this.OnLog($"hwESightHostList Count :{hwESightHostList.Count}");
                    foreach (var x in hwESightHostList)
                    {
                        var instance = this.FindInstance(x);
                        instance.Sync();
                    }
                }
                else
                {
                    this.OnLog("hwESightHostList is null");
                }
            }
            catch (Exception ex)
            {
                this.OnError("GetServerList Error: ", ex);
            }
        }

        /// <summary>
        /// 启动IIS Express
        /// </summary>
        private void RunWebServer()
        {
            // var cmd = $"cd %{EnvVariable}%\\IISExpress&&iisexpress /config:\"%{EnvVariable}%\\Configuration\\applicationhost.config\" /site:SCOMPluginWebServer /systray:true ";
            this.OnLog("Start IISExpress");
            var iisPath = Path.GetFullPath(
                $"{Environment.GetEnvironmentVariable("ProgramFiles(x86)")}\\IIS Express\\iisexpress.exe");
            var configFilePath = Path.GetFullPath($"{this.InstallPath}\\Configuration\\applicationhost.config");
            this.IISProcess = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = iisPath;
            startInfo.Arguments = $" /config:\"{configFilePath}\" /systray:false ";
            startInfo.UseShellExecute = false; // 是否使用操作系统shell启动
            startInfo.RedirectStandardInput = true; // 接受来自调用程序的输入信息
            startInfo.RedirectStandardOutput = true; // 由调用程序获取输出信息
            startInfo.RedirectStandardError = true; // 重定向标准错误输出

            this.IISProcess.StartInfo = startInfo;
            this.IISProcess.OutputDataReceived += (s, e) => { this.OnIISLog(e.Data); };
            this.IISProcess.ErrorDataReceived += (s, e) => { this.OnIISLog(e.Data); };
            this.IISProcess.Start();
            this.IISProcess.BeginErrorReadLine();
            this.IISProcess.BeginOutputReadLine();
        }

        /// <summary>
        /// Finds the instance.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.Service.ESightSyncInstance.</returns>
        private ESightSyncInstance FindInstance(HWESightHost eSight)
        {
            var syncInstance = this.SyncInstances.FirstOrDefault(y => y.ESightIp == eSight.HostIP);
            if (syncInstance == null)
            {
                syncInstance = new ESightSyncInstance(eSight);
                this.SyncInstances.Add(syncInstance);
            }
            else
            {
                syncInstance.UpdateESight(eSight);
            }
            return syncInstance;
        }

        #endregion

        #region Notify

        /// <summary>
        /// Creates the TCP server.
        /// </summary>
        private void CreateTcpServer()
        {
            try
            {
                int localTcpPort = this.GetPort();
                IPAddress localAddr = IPAddress.Parse("127.0" + ".0.1");
                TcpListener tcpListener = new TcpListener(localAddr, localTcpPort);
                tcpListener.Start();

                var pluginConfig = ConfigHelper.GetPluginConfig();
                pluginConfig.TempTcpPort = localTcpPort;
                ConfigHelper.SavePluginConfig(pluginConfig);

                var bytes = new byte[256 * 256 * 16];
                while (true)
                {
                    try
                    {
                        TcpClient tcp = tcpListener.AcceptTcpClient();
                        NetworkStream stream = tcp.GetStream();
                        int i;
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var json = Encoding.UTF8.GetString(bytes, 0, i);
                            HWLogger.NOTIFICATION_PROCESS.Info($"RecieveMsg data:{json}");
                            Task.Run(() =>
                            {
                                var tcpMessage = JsonUtil.DeserializeObject<TcpMessage<object>>(json);
                                switch (tcpMessage.MsgType)
                                {
                                    case TcpMessageType.SyncESight:
                                        this.RunSyncESight(json);
                                        break;
                                    case TcpMessageType.DeleteESight:
                                        this.RunDeleteESight(json);
                                        break;
                                    case TcpMessageType.Alarm:
                                        this.AnalysisAlarm(json);
                                        break;
                                    case TcpMessageType.NeDevice:
                                        this.AnalysisNeDevice(json);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            });

                            // Send back a response.
                            byte[] responseMsg = Encoding.UTF8.GetBytes("Received");
                            stream.Write(responseMsg, 0, responseMsg.Length);
                        }
                        stream.Close();
                        tcp.Close();
                    }
                    catch (Exception ex)
                    {
                        this.OnError(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex.ToString());
            }
        }


        /// <summary>
        /// Runs the synchronize e sight.
        /// </summary>
        /// <param name="json">The json.</param>
        private void RunDeleteESight(string json)
        {
            try
            {
                var messge = JsonUtil.DeserializeObject<TcpMessage<string>>(json);
                HWLogger.NOTIFICATION_PROCESS.Info($"RunDeleteESight:{messge.ESightIp} ");
                var eSightIp = messge.ESightIp;
                Task.Run(() =>
               {
                   BladeConnector.Instance.RemoveServerFromMGroup(eSightIp);
                   HighdensityConnector.Instance.RemoveServerFromMGroup(eSightIp);
                   RackConnector.Instance.RemoveServerFromMGroup(eSightIp);
                   KunLunConnector.Instance.RemoveServerFromMGroup(eSightIp);
               });
                var syncInstance = this.SyncInstances.FirstOrDefault(y => y.ESightIp == eSightIp);
                if (syncInstance != null)
                {
                    this.SyncInstances.Remove(syncInstance);
                }
            }
            catch (Exception ex)
            {
                HWLogger.NOTIFICATION_PROCESS.Error($"RunDeleteESight error. data:{json}", ex);
            }
        }

        /// <summary>
        /// Runs the synchronize e sight.
        /// </summary>
        /// <param name="json">The json.</param>
        private void RunSyncESight(string json)
        {
            try
            {
                var messge = JsonUtil.DeserializeObject<TcpMessage<string>>(json);
                HWLogger.NOTIFICATION_PROCESS.Info($"RunSyncESight:{messge.ESightIp} ");
                var eSight = ESightDal.Instance.GetEntityByHostIp(messge.ESightIp);
                if (eSight == null)
                {
                    throw new Exception("can not find eSight:" + messge.ESightIp);
                }
                var instance = this.FindInstance(eSight);
                instance.Sync();
            }
            catch (Exception ex)
            {
                HWLogger.NOTIFICATION_PROCESS.Error($"RunSyncESight error. data:{json}", ex);
            }
        }

        /// <summary>
        /// 解析告警变更消息
        /// </summary>
        /// <param name="json">The json.</param>
        private void AnalysisAlarm(string json)
        {
            try
            {
                var messge = JsonUtil.DeserializeObject<TcpMessage<NotifyModel<AlarmData>>>(json);
                var alarmData = messge.Data;
                if (alarmData == null)
                {
                    throw new Exception($"AnalysisAlarm error. MsgId:{messge.Id} ");
                }
                var eSight = ESightDal.Instance.GetEntityBySubscribeId(alarmData.SubscribeId);
                if (eSight == null)
                {
                    throw new Exception($"can not find the eSight,SystemID:{alarmData.Data.SystemID},subscribeID:{alarmData.SubscribeId}");
                }

                var instance = this.FindInstance(eSight);
                instance.Enqueue(alarmData.Data.NeDN);
                instance.InsertEvent(alarmData.Data); // 插入事件
            }
            catch (Exception ex)
            {
                HWLogger.NOTIFICATION_PROCESS.Error($"Analysis Alarm error. data:{json}", ex);
            }
        }

        /// <summary>
        /// 解析设备变更消息
        /// </summary>
        /// <param name="json">The json.</param>
        private void AnalysisNeDevice(string json)
        {
            try
            {
                var messge = JsonUtil.DeserializeObject<TcpMessage<NotifyModel<NedeviceData>>>(json);
                var nedeviceData = messge.Data;
                if (nedeviceData == null)
                {
                    throw new Exception($"Analysis NeDevice error. MsgId:{messge.Id} ");
                }
                var dn = nedeviceData.Data.DeviceId;
                var eSight = ESightDal.Instance.GetList().FirstOrDefault(x => x.SubscribeID == nedeviceData.SubscribeId);
                if (eSight == null)
                {
                    throw new Exception($"can not find the eSight,subscribeID:{nedeviceData.SubscribeId}");
                }
                this.OnLog($"msgType is {nedeviceData.MsgType}  Start sync server {eSight.HostIP}");
                var instance = this.FindInstance(eSight);
                var serverType = instance.GetServerType(dn);
                // 高密的设备变更消息需要单独处理
                if (serverType == ServerTypeEnum.ChildHighdensity || serverType == ServerTypeEnum.Highdensity)
                {
                    this.HandlerHighdensityServerDeviceChange(instance, nedeviceData);
                    return;
                }
                switch (nedeviceData.MsgType)
                {
                    case 1: // 新增
                        instance.Sync();
                        break;
                    case 2: // 删除
                        this.OnLog($"Start removing the server.Dn:{dn}");
                        instance.DeleteServer(dn);
                        break;
                    case 3: // 修改
                        instance.Enqueue(dn);
                        instance.InsertDeviceChangeEvent(nedeviceData.Data);
                        break;
                    default:
                        HWLogger.NOTIFICATION_PROCESS.Error($"UnKnown MsgType :{dn}");
                        break;
                }
            }
            catch (Exception ex)
            {
                HWLogger.NOTIFICATION_PROCESS.Error($"Analysis NeDevice  error. data:{json}", ex);
            }
        }

        /// <summary>
        /// 处理高密服务器的设备变更事件
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="nedeviceData">The nedevice data.</param>
        private void HandlerHighdensityServerDeviceChange(ESightSyncInstance instance, NotifyModel<NedeviceData> nedeviceData)
        {
            var dn = nedeviceData.Data.DeviceId;
            HWLogger.NOTIFICATION_PROCESS.Error($"Handler HighdensityServer DeviceChange :{dn}");
            switch (nedeviceData.MsgType)
            {
                case 1: // 新增
                    instance.Sync();
                    break;
                case 2: // 删除时，更新该eSight下所有的高密服务器
                    this.OnLog($"Start removing the server.Dn:{dn}");
                    instance.SyncHighdensityList();
                    break;
                case 3: // 修改时，更新整个高密服务器，并插入事件。
                    instance.Enqueue(dn);
                    instance.InsertDeviceChangeEvent(nedeviceData.Data);
                    break;
                default:
                    HWLogger.NOTIFICATION_PROCESS.Error($"UnKnown MsgType :{dn}");
                    break;
            }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetPort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var udpPorts = properties.GetActiveUdpListeners().Select(x => x.Port).ToList();
            var tcpPorts = properties.GetActiveTcpListeners().Select(x => x.Port).ToList();
            for (int i = 40001; i < 40500; i++)
            {
                if (!udpPorts.Contains(i) && !tcpPorts.Contains(i))
                {
                    return i;
                }
            }
            return 0;
        }

        #endregion

        #region Import Self Cert
        /// <summary>
        /// Gets the self cert.
        /// </summary>
        /// <returns>System.Security.Cryptography.X509Certificates.X509Certificate2.</returns>
        private X509Certificate2 GetSelfCert()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            foreach (var cert in store.Certificates)
            {
                if (cert.Subject == "CN=localhost")
                {
                    return cert;
                }
            }
            store.Close();
            return null;
        }

        /// <summary>
        /// Determines whether [is exsit cert].
        /// </summary>
        /// <returns>System.Boolean.</returns>
        private bool IsNeedImport()
        {
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            foreach (var cert in store.Certificates)
            {
                if (cert.Subject == "CN=localhost")
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Imports to root.
        /// </summary>
        private void ImportToRoot()
        {
            if (this.IsNeedImport())
            {
                this.OnLog("Start import the self signed certificate");
                var self = this.GetSelfCert();
                if (self == null)
                {
                    throw new Exception("can not fin Self Signed Certificate");
                }
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(self);
                store.Close();
            }
            else
            {
                this.OnLog("Do not need import the self signed certificate");
            }
        }
        #endregion

        #region Utils

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void OnError(string msg, Exception ex = null)
        {
            HWLogger.SERVICE.Error(msg, ex);
        }

        /// <summary>
        /// The on log.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        private void OnLog(string msg)
        {
            HWLogger.SERVICE.Info(msg);
        }

        /// <summary>
        /// Called when [IIS log].
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private void OnIISLog(string msg)
        {
            // IIS请求的日志
            if (msg != null)
            {
                if (!msg.StartsWith("Request"))
                {
                    HWLogger.SERVICE.Info("IIS-" + msg);
                }
            }
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void OnNotifyError(string msg, Exception ex = null)
        {
            HWLogger.NOTIFICATION_PROCESS.Error(msg, ex);
        }

        #endregion


    }
}