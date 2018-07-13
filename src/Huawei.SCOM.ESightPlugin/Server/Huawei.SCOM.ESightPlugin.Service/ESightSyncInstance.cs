// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.RESTeSightLib
// Author           : yayun
// Created          : 12-25-2017
//
// Last Modified By : yayun
// Last Modified On : 12-25-2017
// ***********************************************************************
// <copyright file="ESightSyncInstance.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Threading;
    using System.Threading.Tasks;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core;
    using Huawei.SCOM.ESightPlugin.Core.Models;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Devices;
    using Huawei.SCOM.ESightPlugin.Models.Server;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;

    using Timer = System.Timers.Timer;
    using LogUtil;

    /// <summary>
    ///     Class ESightSyncInstance.
    /// </summary>
    public partial class ESightSyncInstance
    {
        #region field
        /// <summary>
        ///     The task list.
        /// </summary>
        private readonly List<Task> taskList = new List<Task>();

        private ESightLogger logger;

        #endregion

        #region Constuctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ESightSyncInstance"/> class.
        /// </summary>
        /// <param name="eSight">
        /// The e sight.
        /// </param>
        public ESightSyncInstance(HWESightHost eSight)
        {
            logger = new ESightLogger(eSight.HostIP);
            this.IsRunning = true;
            this.Session = new ESightSession(eSight);
            this.UpdateDnTasks = new List<UpdateDnTask>();
            this.LastAliveTime = DateTime.Now;
            this.StartKeepAliveJob();
        }
        #endregion

        #region Property


        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public ESightSession Session { get; set; }

        /// <summary>
        ///     The is complete.
        /// </summary>
        /// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete
        {
            get
            {
                foreach (var task in this.taskList)
                {
                    if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
                    {
                        continue;
                    }
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the e sight ip.
        /// </summary>
        /// <value>The e sight ip.</value>
        public string ESightIp => this.Session.ESight.HostIP;

        /// <summary>
        /// Gets the need subscribe alarm.
        /// </summary>
        /// <value>The need subscribe alarm.</value>
        public bool NeedSubscribeAlarm => this.Session.ESight.SubscriptionAlarmStatus != 1;

        /// <summary>
        /// Gets the need subscribe device change.
        /// </summary>
        /// <value>The need subscribe device change.</value>
        public bool NeedSubscribeDeviceChange => this.Session.ESight.SubscriptionNeDeviceStatus != 1;

        /// <summary>
        /// 是否需要订阅保活
        /// </summary>
        /// <value><c>true</c> if [need subscribe keep alive]; otherwise, <c>false</c>.</value>
        public bool NeedSubscribeKeepAlive => this.Session.ESight.SubKeepAliveStatus != 1;

        public List<UpdateDnTask> UpdateDnTasks { get; set; }

        /// <summary>
        /// 上次保活时间
        /// </summary>
        /// <value>The last alive time.</value>
        public DateTime LastAliveTime { get; set; }

        /// <summary>
        /// 是否需要保活（订阅保活成功后改为true,保活超时后改为false）
        /// </summary>
        /// <value><c>true</c> if this instance is need keep alive; otherwise, <c>false</c>.</value>
        public bool IsNeedKeepAlive { get; set; }

        /// <summary>
        /// 是否允许
        /// </summary>
        public bool IsRunning { get; set; }

        #endregion

        /// <summary>
        /// Updates the e sight.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        public void UpdateESight(HWESightHost eSight)
        {
            this.Session.ESight = eSight;
        }

        /// <summary>
        /// Synchronizes this instance.
        /// </summary>
        public void Sync()
        {
            logger.Polling.Info("Start Polling Task.");
            logger.Sdk.Info("Start Polling Task.");
            if (!this.IsComplete)
            {
                logger.Polling.Warn("The last task was not completed, and the task was given up.");
                return;
            }
            if (!this.Session.IsCanConnect())
            {
                OnPollingError($"Can not connect the esight {this.ESightIp}, and the task was given up.");
                return;
            }
            // 清除完成的任务
            this.taskList.Clear();
            this.SyncBladeList();
            this.SyncHighdensityList();
            this.SyncRackList();
            this.SyncKunLunList();
            if (this.NeedSubscribeAlarm || this.NeedSubscribeDeviceChange || this.NeedSubscribeKeepAlive)
            {
                OnPollingInfo("Need Subscribe");
                // 开启一个timer来监听本次轮询任务是否执行完
                Timer timer = new Timer(1000);
                timer.Elapsed += (sender, e) =>
                {
                    if (this.IsComplete)
                    {
                        this.SyncHistoryAlarm();
                        timer.Stop();
                    }
                };
                timer.Start();
            }
        }

        /// <summary>
        /// Synchronizes the history alarm.
        /// </summary>
        public void SyncHistoryAlarm()
        {
            Task.Run(async () =>
            {
                logger.Polling.Debug($"Start Sync History Alarm");
                await Task.Delay(3 * 1000);

                int totalPage = 1;
                int startPage = 0;
                var historyAlarms = new List<AlarmData>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.GetAlarmHistory(startPage);
                        totalPage = result.TotalPage;
                        if (result.Code != 0)
                        {
                            OnPollingError($"SyncHistoryAlarm faild .pageNo:{startPage}.result:[{JsonUtil.SerializeObject(result)}");
                        }
                        else
                        {
                            var deviceEvents = result.Data.Where(x => x.EventType == 2).ToList();
                            OnPollingInfo($"SyncHistoryAlarm Success:[TotalCount:{result.Data.Count} EventType2Count:{deviceEvents.Count}]");
                            deviceEvents.ForEach(x =>
                            {
                                historyAlarms.Add(new AlarmData(x));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncHistoryAlarm Error.eSight:{this.ESightIp} pageNo:{startPage}.", ex);
                    }
                }

                OnPollingInfo($"Get History Alarm:[Count:{historyAlarms.Count}]");
                // 插入历史告警完成后调用订阅接口
                this.InsertHistoryEvent(historyAlarms, this.Subscribe);
            });
        }

        /// <summary>
        /// Subscribes the eSight.
        /// </summary>
        public void Subscribe()
        {
            // 如果没有订阅，则订阅
            Task.Run(async () =>
            {
                await Task.Delay(3 * 1000);
                if (this.NeedSubscribeKeepAlive)
                {
                    SubscribeKeepAlive();
                }
                if (this.NeedSubscribeAlarm)
                {
                    SubscribeAlarm();
                }
                if (this.NeedSubscribeDeviceChange)
                {
                    SubscribeDeviceChange();
                }
            });
        }

        /// <summary>
        /// 订阅设备变更
        /// </summary>
        private void SubscribeDeviceChange()
        {
            try
            {
                logger.Subscribe.Debug($"Start Subscribe DeviceChange On Polling");
                var result = this.Session.SubscribeNeDevice();
                var status = 0;
                if (result.Code != 0)
                {
                    status = -1;
                    logger.Subscribe.Error($"Subscription DeviceChange faild.[result:{JsonUtil.SerializeObject(result)}]");
                }
                else
                {
                    status = 1;
                    logger.Subscribe.Debug($"Subscription DeviceChange Success");
                }
                this.Session.ESight.SubscriptionNeDeviceStatus = status;
                this.Session.ESight.SubscripeNeDeviceError = result.Description;
                // 订阅后更新实体
                ESightDal.Instance.UpdateESightNeDevice(this.ESightIp, status, result.Description);
            }
            catch (Exception e)
            {
                logger.Subscribe.Error($"Subscription DeviceChange error: {this.ESightIp}", e);
            }
        }

        /// <summary>
        /// 订阅告警
        /// </summary>
        private void SubscribeAlarm()
        {
            try
            {
                logger.Subscribe.Debug($"Start Subscribe Alarm On Polling");
                var result = this.Session.SubscribeAlarm();

                var status = 0;
                if (result.Code != 0)
                {
                    status = -1;
                    logger.Subscribe.Error($"Subscription Alarm faild.[result:{JsonUtil.SerializeObject(result)}]");
                }
                else
                {
                    status = 1;
                    logger.Subscribe.Debug($"Subscription Alarm Success");
                }
                this.Session.ESight.SubscriptionAlarmStatus = status;
                this.Session.ESight.SubscripeAlarmError = result.Description;
                // 订阅后更新实体
                ESightDal.Instance.UpdateESightAlarm(this.ESightIp, status, result.Description);
            }
            catch (Exception e)
            {
                logger.Subscribe.Error($"Subscription Alarm error: {this.ESightIp}", e);
            }
        }

        /// <summary>
        /// 订阅保活
        /// </summary>
        private void SubscribeKeepAlive()
        {
            #region SubscribeKeepAlive
            try
            {
                logger.Subscribe.Debug($"Start Subscribe KeepAlive On Polling");
                var result = this.Session.SubscribeKeepAlive();

                var status = 0;
                if (result.Code != 0)
                {
                    status = -1;
                    logger.Subscribe.Error($"SubscribeKeepAlive Alarm faild.[result:{JsonUtil.SerializeObject(result)}]");
                }
                else
                {
                    //如果一直收不到保活消息，则再订阅成功后更新最后保活时间
                    this.LastAliveTime = DateTime.Now;
                    this.IsNeedKeepAlive = true;
                    status = 1;
                    logger.Subscribe.Info($"SubscribeKeepAlive Success.Start KeepAlive Taks.Update LastAliveTime [{ DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                }
                this.Session.ESight.SubKeepAliveStatus = status;
                this.Session.ESight.SubKeepAliveError = result.Description;
                // 订阅后更新实体
                ESightDal.Instance.UpdateESightKeepAlive(this.ESightIp, status, result.Description);
            }
            catch (Exception e)
            {
                logger.Subscribe.Error($"SubscribeKeepAlive error: {this.ESightIp}", e);
            }

            #endregion
        }

        /// <summary>
        /// The insert event.
        /// </summary>
        /// <param name="alarmDatas">The alarm datas.</param>
        /// <param name="callback">The callback.</param>
        public void InsertHistoryEvent(List<AlarmData> alarmDatas, Action callback)
        {
            Task.Run(() =>
            {
                try
                {
                    var r = alarmDatas.GroupBy(x => x.NeDN).ToList();
                    r.ForEach(datas =>
                    {
                        var dn = datas.Key;
                        try
                        {
                            logger.Polling.Debug($"Start InsertHistoryEvent:{dn}");
                            var serverType = this.GetServerTypeByDn(dn);
                            var eventDatas = datas.Select(x => new EventData(x, this.ESightIp, serverType)).ToList();
                            switch (serverType)
                            {
                                case ServerTypeEnum.Blade:
                                case ServerTypeEnum.ChildBlade:
                                case ServerTypeEnum.Switch:
                                    BladeConnector.Instance.InsertHistoryEvent(eventDatas, serverType, this.ESightIp);
                                    break;
                                case ServerTypeEnum.Highdensity:
                                case ServerTypeEnum.ChildHighdensity:
                                    HighdensityConnector.Instance.InsertHistoryEvent(eventDatas, serverType, this.ESightIp);
                                    break;
                                case ServerTypeEnum.Rack:
                                    RackConnector.Instance.InsertHistoryEvent(eventDatas, this.ESightIp);
                                    break;
                                case ServerTypeEnum.KunLun:
                                    KunLunConnector.Instance.InsertHistoryEvent(eventDatas, this.ESightIp);
                                    break;
                            }

                            logger.Polling.Debug($"End InsertHistoryEvent :{dn} [Count:{eventDatas.Count}]");
                        }
                        catch (Exception ex)
                        {
                            OnPollingError($"End InsertHistoryEvent :{dn}", ex);
                        }
                    });
                    callback();
                }
                catch (Exception ex)
                {
                    OnPollingError("InsertHistoryEvent Error: ", ex);
                }
            });
        }

        #region 刀片

        /// <summary>
        /// 同步刀片服务器
        /// </summary>
        private void SyncBladeList()
        {
            logger.Sdk.Info("Start Polling Blade Task.");
            var task = Task.Run(() =>
            {
                int totalPage = 1;
                int startPage = 0;
                var newDeviceIds = new List<string>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.QueryBladeServer(startPage);
                        totalPage = result.TotalPage;
                        foreach (var x in result.Data)
                        {
                            newDeviceIds.Add($"{this.ESightIp}-{x.DN}");
                            this.QueryBladeDetial(x);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncBladeList Error.eSight:{this.ESightIp} ", ex);
                    }
                }
                logger.Polling.Debug($"SyncBladeList Finish.[{string.Join(",", newDeviceIds).Replace($"{this.ESightIp}-", "")}]");
                BladeConnector.Instance.RemoveBladeOnSync(this.ESightIp, newDeviceIds);
            });
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the blade detial.
        /// </summary>
        /// <param name="x">The x.</param>
        private void QueryBladeDetial(BladeServer x)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var device = this.Session.GetServerDetails(x.DN);
                    x.MakeDetail(device, this.ESightIp);
                    x.ChildBlades.ForEach(m =>
                    {
                        var deviceDatail = this.Session.GetServerDetails(m.DN);
                        m.MakeChildBladeDetail(deviceDatail);
                    });

                    // 插入數據
                    BladeConnector.Instance.SyncServer(x);
                }
                catch (Exception ex)
                {
                    OnPollingError($"QueryBladeDetial Error:DN:{x.DN}", ex);
                }
            });
            this.taskList.Add(task);
        }
        #endregion

        #region 高密

        /// <summary>
        /// 同步高密服务器的详情
        /// </summary>
        public void SyncHighdensityList()
        {
            logger.Sdk.Info("Start Polling Highdensity Task.");
            var task = Task.Run(() =>
            {
                int totalPage = 1;
                int startPage = 0;
                var newDeviceIds = new List<string>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.QueryHighDesentyServer(startPage);
                        totalPage = result.TotalPage;
                        foreach (var x in result.Data)
                        {
                            newDeviceIds.Add($"{this.ESightIp}-{x.DN}");
                            this.QueryHighdensityDetial(x);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncHighdensityList Error.eSight:{this.ESightIp} ", ex);
                    }
                }
                logger.Polling.Debug($"SyncHighdensityList Finish.[{string.Join(",", newDeviceIds).Replace($"{this.ESightIp}-", "")}]");
                HighdensityConnector.Instance.RemoveHighOnSync(this.ESightIp, newDeviceIds);
            });
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the highdensity detial.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="isPolling">是否轮询</param>
        private void QueryHighdensityDetial(HighdensityServer x)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var device = this.Session.GetServerDetails(x.DN);
                    x.MakeDetail(device, this.ESightIp);
                    x.ChildHighdensitys.ForEach(
                        m =>
                            {
                                var deviceDatail = this.Session.GetServerDetails(m.DN);
                                m.MakeChildBladeDetail(deviceDatail);
                            });

                    // 插入數據
                    HighdensityConnector.Instance.SyncServer(x);
                }
                catch (Exception ex)
                {
                    OnPollingError($"QueryHighdensityDetial Error:DN:{x.DN}", ex);
                }
            });
            this.taskList.Add(task);
        }
        #endregion

        #region 机架

        /// <summary>
        /// 同步机架服务器的详情
        /// </summary>
        private void SyncRackList()
        {
            logger.Sdk.Info("Start Polling Rack Task.");
            var task = Task.Run(() =>
            {
                int totalPage = 1;
                int startPage = 0;
                var newDeviceIds = new List<string>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.QueryRackServer(startPage);
                        totalPage = result.TotalPage;
                        foreach (var x in result.Data)
                        {
                            newDeviceIds.Add($"{this.ESightIp}-{ x.DN}");
                            this.QueryRackDetial(x);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncRackList Error.eSight:{this.ESightIp} ", ex);
                    }
                }
                logger.Polling.Debug($"SyncRackList Finish.[{string.Join(",", newDeviceIds).Replace($"{this.ESightIp}-", "")}]");
                RackConnector.Instance.DeleteRackOnSync(this.ESightIp, newDeviceIds);
            });
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the rack detial.
        /// </summary>
        /// <param name="rack">The rack.</param>
        private void QueryRackDetial(RackServer rack)
        {
            var task = Task.Run(() =>
                {
                    try
                    {
                        var device = this.Session.GetServerDetails(rack.DN);
                        rack.MakeDetail(device, this.ESightIp);
                        RackConnector.Instance.SyncServer(rack);
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"QueryRackDetial Error: {rack.DN}", ex);
                    }
                });
            this.taskList.Add(task);
        }
        #endregion

        #region 昆仑

        /// <summary>
        /// 同步昆仑服务器的详情
        /// </summary>
        private void SyncKunLunList()
        {
            logger.Sdk.Info("Start Polling KunLun Task.");
            var task = Task.Run(() =>
            {
                int totalPage = 1;
                int startPage = 0;
                var newDeviceIds = new List<string>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.QueryKunLunServer(startPage);
                        totalPage = result.TotalPage;
                        foreach (var x in result.Data)
                        {
                            newDeviceIds.Add($"{this.ESightIp}-{x.DN}");
                            this.QueryKunLunDetial(x);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncKunLunList Error.eSight:{this.ESightIp} ", ex);
                    }
                }
                logger.Polling.Debug($"SyncKunLunList Finish.[{string.Join(",", newDeviceIds).Replace($"{this.ESightIp}-", "")}]");
                KunLunConnector.Instance.RemoveKunLunOnSync(this.ESightIp, newDeviceIds);
            });
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the kunLun detial.
        /// </summary>
        /// <param name="kunLun">The kun lun.</param>
        private void QueryKunLunDetial(KunLunServer kunLun)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var device = this.Session.GetServerDetails(kunLun.DN);
                    kunLun.MakeDetail(device, this.ESightIp);
                    KunLunConnector.Instance.SyncServer(kunLun);
                }
                catch (Exception ex)
                {
                    OnPollingError($"QueryKunLunDetial Error:{kunLun.DN}", ex);
                }
            });
            this.taskList.Add(task);
        }
        #endregion

        #region KeepAlive

        /// <summary>
        /// 检查保活任务
        /// 系统在运行过程中10分钟内收不到esight 的系统保活信息，需要重新进行系统保活订阅和告警以及设备变更订阅
        /// </summary>
        private void StartKeepAliveJob()
        {
            Task.Run(() =>
            {
                logger.Subscribe.Info("Run KeepAlive Job");
                while (this.IsRunning)
                {
                    if (IsNeedKeepAlive)
                    {
                        #region Check
                        try
                        {
                            if ((DateTime.Now - this.LastAliveTime).TotalMinutes > 10)
                            {
                                logger.Subscribe.Error($"[{this.ESightIp}]-keep Alive TimeOut.Will subcribe again.LastAliveTime:{this.LastAliveTime:yyyy-MM-dd HH:mm:ss}");
                                this.IsNeedKeepAlive = false;
                                this.SubscribeKeepAlive();
                                this.SubscribeAlarm();
                                this.SubscribeDeviceChange();
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Subscribe.Error(e);
                            throw;
                        }
                        #endregion
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(2));
                }
            });
        }

        /// <summary>
        /// 更新最后保活时间
        /// </summary>
        /// <param name="dt">The dt.</param>
        public void UpdateAliveTime(DateTime dt)
        {
            this.LastAliveTime = DateTime.Now;
            logger.Subscribe.Debug($"Update LastAliveTime:[{this.LastAliveTime:yyyy-MM-dd HH:mm:ss}]");
        }

        /// <summary>
        /// 关闭实例
        /// </summary>
        public void Close()
        {
            logger.Subscribe.Info("Delete ESight SyncInstance");
            this.IsRunning = false;
            this.IsNeedKeepAlive = false;
        }
        #endregion

        private void OnPollingInfo(string msg)
        {
            logger.Polling.Info(msg);
        }

        private void OnPollingError(string msg, Exception ex = null)
        {
            logger.Polling.Error(msg, ex);
        }
    }
}