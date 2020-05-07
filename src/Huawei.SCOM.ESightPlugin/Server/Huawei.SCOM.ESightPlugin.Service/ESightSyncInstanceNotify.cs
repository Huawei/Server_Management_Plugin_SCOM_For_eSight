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
// Assembly         : Huawei.SCOM.ESightPlugin.Service
// Author           : yayun
// Created          : 07-03-2018
//
// Last Modified By : yayun
// Last Modified On : 07-03-2018
// ***********************************************************************
// <copyright file="ESightSyncInstanceNotify.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Threading.Tasks;
using Huawei.SCOM.ESightPlugin.Core;
using Huawei.SCOM.ESightPlugin.Core.Models;
using Huawei.SCOM.ESightPlugin.Models;
using Huawei.SCOM.ESightPlugin.Models.Devices;
using Huawei.SCOM.ESightPlugin.Models.Server;
using Microsoft.EnterpriseManagement.Monitoring;

namespace Huawei.SCOM.ESightPlugin.Service
{
    public partial class ESightSyncInstance
    {

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="serverType">Type of the server.</param>
        public void InsertDeviceChangeEvent(NedeviceData data, ServerTypeEnum serverType)
        {
            Task.Run(() =>
            {
                try
                {
                    //logger.NotifyProcess.Info($"Start Insert deviceChangeEvent.[Dn:{data.DeviceId}] [serverType:{serverType}]");
                    var deviceChangeEventData = new DeviceChangeEventData(data, this.ESightIp, serverType);
                    switch (serverType)
                    {
                        case ServerTypeEnum.Blade:
                        case ServerTypeEnum.ChildBlade:
                        case ServerTypeEnum.Switch:
                            BladeConnector.Instance.InsertDeviceChangeEvent(deviceChangeEventData, serverType, this.ESightIp);
                            break;
                        case ServerTypeEnum.Highdensity:
                        case ServerTypeEnum.ChildHighdensity:
                            HighdensityConnector.Instance.InsertDeviceChangeEvent(deviceChangeEventData, serverType, this.ESightIp);
                            break;
                        case ServerTypeEnum.Rack:
                            RackConnector.Instance.InsertDeviceChangeEvent(deviceChangeEventData, this.ESightIp);
                            break;
                        case ServerTypeEnum.KunLun:
                            KunLunConnector.Instance.InsertDeviceChangeEvent(deviceChangeEventData, this.ESightIp);
                            break;
                    }
                    //logger.NotifyProcess.Info($"End deviceChangeEvent: {data.DeviceId}");
                }
                catch (Exception ex)
                {
                    logger.NotifyProcess.Error(ex, "InsertEvent Error: ");
                }
            });
        }

        /// <summary>
        /// The insert event.
        /// </summary>
        /// <param name="alarmData">The alarm data.</param>
        /// <param name="serverType">Type of the server.</param>
        public void InsertEvent(AlarmData alarmData, ServerTypeEnum serverType)
        {
            Task.Run(() =>
            {
                try
                {
                    //logger.NotifyProcess.Info($"Start InsertEvent. [Dn:{alarmData.NeDN}] [serverType:{serverType}]");

                    var alertModel = new EventData(alarmData, this.ESightIp, serverType);
                    switch (serverType)
                    {
                        case ServerTypeEnum.Blade:
                        case ServerTypeEnum.ChildBlade:
                        case ServerTypeEnum.Switch:
                            BladeConnector.Instance.InsertEvent(alertModel, serverType, this.ESightIp);
                            break;
                        case ServerTypeEnum.Highdensity:
                        case ServerTypeEnum.ChildHighdensity:
                            HighdensityConnector.Instance.InsertEvent(alertModel, serverType, this.ESightIp);
                            break;
                        case ServerTypeEnum.Rack:
                            RackConnector.Instance.InsertEvent(alertModel, this.ESightIp);
                            break;
                        case ServerTypeEnum.KunLun:
                            KunLunConnector.Instance.InsertEvent(alertModel, this.ESightIp);
                            break;
                    }
                    //logger.NotifyProcess.Info($"End InsertEvent {alarmData.NeDN}");
                }
                catch (Exception ex)
                {
                    logger.NotifyProcess.Error(ex, "InsertEvent Error: ");
                }
            });
        }

        #region Notify

        /// <summary>
        /// 开启一个更新dn的任务
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <param name="serverType">Type of the server.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        public void StartUpdateTask(string dn, ServerTypeEnum serverType, int alarmSn)
        {
            var deviceId = $"{this.ESightIp}-{dn}";
            try
            {
                logger.NotifyProcess.Info($"[alarmSn:{alarmSn}] New UpdateDnTask.[Dn:{dn}] [serverType:{serverType}]");
                var exsit = this.UpdateDnTasks.FirstOrDefault(x => x.Dn == dn);
                //Dn已存在
                if (exsit != null)
                {
                    #region Dn已存在
                    //如果首次刷新已经执行过，此处再执行一次
                    if (exsit.FirstRefreshTime < DateTime.Now)
                    {
                        exsit.FirstRefreshTime = DateTime.Now.AddSeconds(60);
                        exsit.StartFirstUpdateTask();
                        logger.NotifyProcess.Debug($"[alarmSn:{alarmSn}]-[{dn}] ReStart FirstRefreshTask.[{exsit.FirstRefreshTime:yyyy-MM-dd HH:mm:ss}]");
                    }

                    var oldAlarmSn = exsit.AlarmSn;
                    exsit.AlarmSn = alarmSn;
                    //延长最后一次刷新时间
                    exsit.LastRefreshTime = DateTime.Now.AddSeconds(5 * 60);
                    logger.NotifyProcess.Debug($"[alarmSn:{alarmSn}] [preAlarmSn:{oldAlarmSn}] [{dn}] Delay LastRefreshTime:{exsit.LastRefreshTime:yyyy-MM-dd HH:mm:ss}.");
                    return;
                    #endregion
                }
                if (serverType == ServerTypeEnum.Switch)
                {
                    return;
                }
                // 暂不可以通过交换板的dn获取交换板的详情
                // 交换板DN 上报的告警信息刷新管理板所有部件（包括交换板）健康状态
                var task = new UpdateDnTask(this, dn, alarmSn);

                if (serverType == ServerTypeEnum.Blade)
                {
                    task.UpdateAction = UpdateBladeServer;
                }
                if (serverType == ServerTypeEnum.ChildBlade)
                {
                    task.UpdateAction = UpdateChildBladeServer;
                }
                if (serverType == ServerTypeEnum.ChildHighdensity)
                {
                    task.UpdateAction = UpdateChildHighdensityServer;
                }
                // 高密管理板不会有告警上来
                if (serverType == ServerTypeEnum.Highdensity)
                {
                    task.UpdateAction = UpdateHighdensityServer;
                }
                if (serverType == ServerTypeEnum.Rack)
                {
                    task.UpdateAction = UpdateRackServer;
                }
                if (serverType == ServerTypeEnum.KunLun)
                {
                    task.UpdateAction = UpdateKunLunServer;
                }
                task.FirstRefreshTime = DateTime.Now.AddSeconds(60);
                task.LastRefreshTime = DateTime.Now.AddSeconds(5 * 60);

                logger.NotifyProcess.Debug($"[alarmSn:{alarmSn}]-[{dn}] Will StartFirstUpdateTask [{task.FirstRefreshTime:yyyy-MM-dd HH:mm:ss}].");
                logger.NotifyProcess.Debug($"[alarmSn:{alarmSn}]-[{dn}] Will StartLastUpdateTask near [{task.LastRefreshTime:yyyy-MM-dd HH:mm:ss}].");

                task.StartFirstUpdateTask();
                task.StartLastUpdateTask((isSuccess, isChange) =>
                 {
                     if (isSuccess)
                     {
                         logger.NotifyProcess.Debug($"[alarmSn:{task.AlarmSn}]-[{dn}] End LastRefreshTask.[isChange:{isChange}] .");
                     }
                     else
                     {
                         logger.NotifyProcess.Error($"[alarmSn:{task.AlarmSn}]-[{dn}] LastRefreshTask Faild.");
                     }
                     this.UpdateDnTasks.Remove(task);
                 });
                this.UpdateDnTasks.Add(task);
            }
            catch (Exception e)
            {
                logger.NotifyProcess.Debug(e, $"[alarmSn:{alarmSn}]-[Dn:{dn}] [serverType:{serverType}] StartUpdateTask Error.");
            }
        }

        /// <summary>
        /// The delete server.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <param name="serverType">Type of the server.</param>
        public void DeleteServer(string dn, ServerTypeEnum serverType)
        {
            Task.Run(() =>
            {
                try
                {
                    logger.NotifyProcess.Debug($"Delete the server on device change.[Dn:{dn}] [serverType:{serverType}]");
                    var deviceId = $"{this.ESightIp}-{dn}";
                    switch (serverType)
                    {
                        case ServerTypeEnum.Blade:
                            BladeConnector.Instance.RemoveServerByDeviceId(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.ChildBlade:
                            BladeConnector.Instance.RemoveChildBlade(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.Highdensity:
                            HighdensityConnector.Instance.RemoveServerByDeviceId(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.ChildHighdensity:
                            HighdensityConnector.Instance.RemoveChildHighDensityServer(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.Switch:
                            BladeConnector.Instance.RemoveChildSwitch(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.Rack:
                            RackConnector.Instance.RemoveServerByDeviceId(this.ESightIp, deviceId);
                            break;
                        case ServerTypeEnum.KunLun:
                            KunLunConnector.Instance.RemoveServerByDeviceId(this.ESightIp, deviceId);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.NotifyProcess.Error(ex, $"DeleteServer {dn} Error: ");
                }
            });
        }

        /// <summary>
        /// Gets the type of the server.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns>ServerTypeEnum.</returns>
        public ServerTypeEnum GetServerTypeByDn(string dn)
        {
            var deviceId = $"{this.ESightIp}-{dn}";
            var serverType = this.GetServerTypeByDeviceId(deviceId);
            return serverType;
        }

        /// <summary>
        /// The get server type.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="ServerTypeEnum" />.</returns>
        /// <exception cref="System.Exception">e</exception>
        private ServerTypeEnum GetServerTypeByDeviceId(string deviceId)
        {
            var server = BladeConnector.Instance.GetBladeServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.Blade;
            }
            server = BladeConnector.Instance.GetChildBladeServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.ChildBlade;
            }
            server = HighdensityConnector.Instance.GetHighdensityServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.Highdensity;
            }
            server = HighdensityConnector.Instance.GetChildHighdensityServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.ChildHighdensity;
            }
            server = RackConnector.Instance.GetRackServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.Rack;
            }
            server = KunLunConnector.Instance.GetKunLunServer(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.KunLun;
            }
            server = BladeConnector.Instance.GetSwitchBoard(deviceId);
            if (server != null)
            {
                return ServerTypeEnum.Switch;
            }
            throw new Exception($"GetServerType Faild: {deviceId}");
        }

        /// <summary>
        /// Get device by "DN"
        /// </summary>
        /// <param name="dn">DN</param>
        /// <returns></returns>
        private MonitoringDeviceObject GetDeviceByDN(string dn)
        {
            var deviceId = $"{this.ESightIp}-{dn}";
            return this.GetDeviceByDeveceId(deviceId);
        }

        /// <summary>
        /// Get device by "SCOM deviceId"
        /// </summary>
        /// <param name="deviceId">SCOM DN</param>
        /// <returns></returns>
        private MonitoringDeviceObject GetDeviceByDeveceId(string deviceId)
        {
            var server = BladeConnector.Instance.GetBladeServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.Blade, BladeConnector.Instance.GetBladeServer);
            }

            server = BladeConnector.Instance.GetChildBladeServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.ChildBlade, BladeConnector.Instance.GetChildBladeServer);
            }

            server = HighdensityConnector.Instance.GetHighdensityServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.Highdensity, HighdensityConnector.Instance.GetHighdensityServer);
            }

            server = HighdensityConnector.Instance.GetChildHighdensityServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.ChildHighdensity, HighdensityConnector.Instance.GetChildHighdensityServer);
            }

            server = RackConnector.Instance.GetRackServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.Rack, RackConnector.Instance.GetRackServer);
            }

            server = KunLunConnector.Instance.GetKunLunServer(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.KunLun, KunLunConnector.Instance.GetKunLunServer);
            }

            server = BladeConnector.Instance.GetSwitchBoard(deviceId);
            if (server != null)
            {
                return new MonitoringDeviceObject(deviceId, server, ServerTypeEnum.Switch, BladeConnector.Instance.GetSwitchBoard);
            }

            return null;
        }

        /// <summary>
        /// 更新刀片机架
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateBladeServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update BladeServer:{model.DN}.[alarmSn:{alarmSn}]");
                var server = new BladeServer
                {
                    DN = model.DN,
                    ServerName = model.Name,
                    Manufacturer = string.Empty,
                    ServerModel = model.Mode,
                    IpAddress = model.IpAddress,
                    Location = string.Empty,
                    Status = model.Status
                };
                server.MakeDetail(model, this.ESightIp);
                BladeConnector.Instance.UpdateBlade(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateBladeServer Error. Dn:{model.DN}.[alarmSn:{alarmSn}]");
            }
        }

        /// <summary>
        /// 更新子刀片
        /// 子刀片DN 上报的告警 刷新该子刀片及管理板（包括交换板）
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateChildBladeServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update ChildBladeServer:{model.DN}.[alarmSn:{alarmSn}]");
                //更新子刀片
                var server = new ChildBlade(this.ESightIp);
                server.MakeChildBladeDetail(model);
                BladeConnector.Instance.UpdateChildBlade(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateChildBladeServer Error. Dn:{model.DN} .[alarmSn:{alarmSn}]. ");
            }
        }

        /// <summary>
        /// 当Dn为高密子服务器的DN时,需要更新整个管理板
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateChildHighdensityServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update Child HighdensityServer:{model.DN} .[alarmSn:{alarmSn}]");
                //更新子刀片
                var server = new ChildHighdensity(this.ESightIp);
                server.MakeChildBladeDetail(model);
                HighdensityConnector.Instance.UpdateChildBoard(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateChildHighdensityServer Error.Dn:{model.DN} .[alarmSn:{alarmSn}] ");
            }
        }

        /// <summary>
        /// 更新高密管理板
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateHighdensityServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update HighdensityServer:{model.DN} .[alarmSn:{alarmSn}]");
                var server = new HighdensityServer
                {
                    DN = model.DN,
                    ServerName = model.Name,
                    Manufacturer = string.Empty,
                    ServerModel = model.Mode,
                    IpAddress = model.IpAddress,
                    Location = string.Empty,
                    Status = model.Status
                };
                server.MakeDetail(model, this.ESightIp);
                HighdensityConnector.Instance.UpdateMain(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateHighdensityServer Error.Dn:{model.DN} .[alarmSn:{alarmSn}] ");
            }
        }

        /// <summary>
        /// 更新昆仑
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateKunLunServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update KunLunServer:{model.DN}.[alarmSn:{alarmSn}]");
                var server = new KunLunServer();
                server.MakeDetail(model, this.ESightIp);
                KunLunConnector.Instance.UpdateKunLun(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateKunLunServer Error. Dn:{model.DN}.[alarmSn:{alarmSn}] ");
            }
        }

        /// <summary>
        /// 更新机架
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        private void UpdateRackServer(HWDeviceDetail model, int alarmSn)
        {
            try
            {
                logger.NotifyProcess.Info($"Start Update RackServer:{model.DN}");
                var server = new RackServer();
                server.MakeDetail(model, this.ESightIp);
                RackConnector.Instance.UpdateRack(server, false);
            }
            catch (Exception ex)
            {
                logger.NotifyProcess.Error(ex, $"UpdateRackServer Error.Dn:{model.DN}.[alarmSn:{alarmSn}]");
            }
        }

        #endregion
    }
}
