//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿// ***********************************************************************
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

        private ESightLogger logger { get; set; }

        private readonly TaskFactory taskFactory;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private PluginConfig pluginConfig;

        private Timer waitSyncTaskFinishedTimer;



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
            this.alarmQueue = new Queue<AlarmData>();
            this.StartAlarmEventProcessor();
            this.StartNeDeviceEventProcessor();
            this.StartKeepAliveJob();
            this.pluginConfig = ConfigHelper.GetPluginConfig();
            var scheduler = new LimitedConcurrencyLevelTaskScheduler(pluginConfig.ThreadCount);
            taskFactory = new TaskFactory(cts.Token, TaskCreationOptions.HideScheduler, TaskContinuationOptions.HideScheduler, scheduler);
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

            //Timer timer = new Timer(1000);
            //timer.Elapsed += (sender, e) =>
            //{
            //    if (this.IsComplete)
            //    {
            //        this.SyncESightOpenAlarms();
            //        timer.Stop();
            //    }
            //};
            //timer.Start();

            if (waitSyncTaskFinishedTimer == null)
            {
                // 开启一个timer来监听本次轮询任务是否执行完
                waitSyncTaskFinishedTimer = new Timer(3000)
                {
                    AutoReset = false,
                    Enabled = true,
                };

                waitSyncTaskFinishedTimer.Elapsed += (sender, e) =>
                {
                    try
                    {
                        if (IsComplete)
                        {
                            logger.Polling.Info("All sync task has finished, sync open alarms now.");
                            SyncESightOpenAlarms();
                        }
                    }
                    finally
                    {
                        if (!IsComplete)
                        {
                            waitSyncTaskFinishedTimer.Start(); // Restart timer for next tick's checking
                        }
                    }
                };
            }

            waitSyncTaskFinishedTimer.Start();
        }



        /// <summary>
        /// Subscribes messages from eSight.
        /// </summary>
        public void Subscribe()
        {
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
        }


        /// <summary>
        /// UNsubscribes messages from eSight.
        /// </summary>
        public void Unsubscribe()
        {
            logger.Subscribe.Info($"Unsubscribe ESight `{this.ESightIp}` now.");
            UnSubscribeKeepAlive();
            UnSubscribeAlarm();
            UnSubscribeNeDevice();
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
                logger.Subscribe.Error(e, $"Subscription DeviceChange error: {this.ESightIp}");
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
                logger.Subscribe.Error(e, $"Subscription Alarm error: {this.ESightIp}");
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
                logger.Subscribe.Error(e, $"SubscribeKeepAlive error: {this.ESightIp}");
            }

            #endregion
        }

        /// <summary>
        /// 取消订阅保活
        /// </summary>
        public void UnSubscribeKeepAlive()
        {
            try
            {
                var resut = this.Session.UnSubscribeKeepAlive(this.Session.ESight.SystemID);
                ESightDal.Instance.UpdateESightKeepAlive(this.ESightIp, 0, string.Empty);
                logger.Subscribe.Info($"Unsubscribe keepalive messages from eSight `{this.ESightIp}`.");
            }
            catch (Exception e)
            {
                logger.Subscribe.Error(e, $"Unsubscribe keepalive messages from eSight `{this.ESightIp}` failed.");
            }

        }

        /// <summary>
        /// 取消订阅告警
        /// </summary>
        public void UnSubscribeAlarm()
        {
            try
            {
                var resut = this.Session.UnSubscribeAlarm(this.Session.ESight.SystemID);
                // 订阅后更新实体
                ESightDal.Instance.UpdateESightAlarm(this.ESightIp, 0, string.Empty);
                logger.Subscribe.Info($"Unsubscribe alarm messages from eSight `{this.ESightIp}`.");
            }
            catch (Exception e)
            {
                logger.Subscribe.Error(e, $"Unsubscribe alarm messages from eSight `{this.ESightIp}` failed.");
            }

        }

        /// <summary>
        /// 取消订阅设备变更
        /// </summary>
        public void UnSubscribeNeDevice()
        {
            try
            {
                var resut = this.Session.UnSubscribeNeDevice(this.Session.ESight.SystemID);
                ESightDal.Instance.UpdateESightNeDevice(this.ESightIp, 0, string.Empty);
                logger.Subscribe.Info($"Unsubscribe NE Device messages from eSight `{this.ESightIp}`.");
            }
            catch (Exception e)
            {
                logger.Subscribe.Error(e, $"Unsubscribe NE Device messages from eSight `{this.ESightIp}` failed.");
            }

        }

        #region 刀片

        /// <summary>
        /// 同步刀片服务器
        /// </summary>
        private void SyncBladeList()
        {
            logger.Sdk.Info("Start Polling Blade Task.");
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the blade detial.
        /// </summary>
        /// <param name="x">The x.</param>
        private void QueryBladeDetial(BladeServer x)
        {
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
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
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the highdensity detial.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="isPolling">是否轮询</param>
        private void QueryHighdensityDetial(HighdensityServer x)
        {
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
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
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the rack detial.
        /// </summary>
        /// <param name="rack">The rack.</param>
        private void QueryRackDetial(RackServer rack)
        {
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
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
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
            this.taskList.Add(task);
        }

        /// <summary>
        /// Queries the kunLun detial.
        /// </summary>
        /// <param name="kunLun">The kun lun.</param>
        private void QueryKunLunDetial(KunLunServer kunLun)
        {
            var task = taskFactory.StartNew(() =>
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
            }, cts.Token);
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
                        this.IsNeedKeepAlive = false;
                        #region Check
                        try
                        {
                            if ((DateTime.Now - this.LastAliveTime).TotalMinutes > 10)
                            {
                                logger.Subscribe.Error($"[{this.ESightIp}]-keep Alive TimeOut.Will subcribe again.LastAliveTime:{this.LastAliveTime:yyyy-MM-dd HH:mm:ss}");
                                
                                // TODO(turnbig) what if failed here ...
                                this.SubscribeAlarm();
                                this.SubscribeDeviceChange();
                                this.SubscribeKeepAlive();
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Subscribe.Error(e);
                            throw;
                        } 
                        finally
                        {
                            this.IsNeedKeepAlive = true;
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
            this.alarmQueue.Clear();
            this.cts.Cancel(false);
        }
        #endregion

        /// <summary>
        /// log [polling information].
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private void OnPollingInfo(string msg)
        {
            logger.Polling.Info(msg);
        }

        /// <summary>
        ///  log [polling error].
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="ex">The ex.</param>
        private void OnPollingError(string msg, Exception ex = null)
        {
            logger.Polling.Error(ex, msg);
        }
    }
}