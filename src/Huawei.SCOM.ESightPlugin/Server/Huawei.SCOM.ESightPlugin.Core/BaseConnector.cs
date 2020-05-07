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
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 11-19-2017
//
// Last Modified By : yayun
// Last Modified On : 05-22-2019
// ***********************************************************************
// <copyright file="BaseConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The base connector.</summary>
// ************************************************************************

using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core.Const;
    using Huawei.SCOM.ESightPlugin.Core.Models;
    using Huawei.SCOM.ESightPlugin.Models;
    using LogUtil;

    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Configuration;
    using Microsoft.EnterpriseManagement.ConnectorFramework;
    using Microsoft.EnterpriseManagement.Monitoring;
    using MPObject = Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject;

    /// <summary>
    /// The base connector.
    /// </summary>
    public class BaseConnector
    {
        #region Fields

        /// <summary>
        /// The base entity class.
        /// </summary>
        private ManagementPackClass baseEntityClass;

        /// <summary>
        /// The huawei server class.
        /// </summary>
        private ManagementPackClass huaweiServerClass;

        /// <summary>
        /// The part child group class.
        /// </summary>
        private ManagementPackClass partChildGroupClass;

        /// <summary>
        /// The part group class.
        /// </summary>
        private ManagementPackClass partGroupClass;

        /// <summary>
        /// The _close state
        /// </summary>
        private MonitoringAlertResolutionState closeState;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the base entity class.
        /// </summary>
        /// <value>The base entity class.</value>
        public ManagementPackClass BaseEntityClass => this.baseEntityClass ?? (this.baseEntityClass = MGroup.Instance.GetManagementPackClass("System.Entity"));

        /// <summary>
        /// Gets or sets the connector guid.
        /// </summary>
        /// <value>The connector unique identifier.</value>
        public Guid ConnectorGuid { get; set; }

        /// <summary>
        /// Gets or sets the connector info.
        /// </summary>
        /// <value>The connector information.</value>
        public ConnectorInfo ConnectorInfo { get; set; }

        /// <summary>
        /// Gets or sets the connector name.
        /// </summary>
        /// <value>The name of the connector.</value>
        public string ConnectorName { get; set; }

        /// <summary>
        /// The display name field.
        /// </summary>
        /// <value>The display name field.</value>
        public ManagementPackProperty DisplayNameField => this.BaseEntityClass.PropertyCollection["DisplayName"];

        /// <summary>
        /// Gets the huawei server class.
        /// </summary>
        /// <value>The huawei server class.</value>
        public ManagementPackClass HuaweiServerClass => this.huaweiServerClass ?? (this.huaweiServerClass = MGroup.Instance.GetManagementPackClass(EntityTypeConst.ESight.HuaweiServer));

        /// <summary>
        /// The huawei server key.
        /// </summary>
        /// <value>The huawei server key.</value>
        public ManagementPackProperty HuaweiServerKey => this.HuaweiServerClass.PropertyCollection["DN"];

        /// <summary>
        /// Gets or sets the montioring connector.
        /// </summary>
        /// <value>The montioring connector.</value>
        public MonitoringConnector MontioringConnector { get; set; }

        /// <summary>
        /// Gets the part child group class.
        /// </summary>
        /// <value>The part child group class.</value>
        public ManagementPackClass PartChildGroupClass => this.partChildGroupClass ?? (this.partChildGroupClass = MGroup.Instance.GetManagementPackClass("ESight.PartChildGroup"));

        /// <summary>
        /// The part child group key.
        /// </summary>
        /// <value>The part child group key.</value>
        public ManagementPackProperty PartChildGroupKey => this.PartChildGroupClass.PropertyCollection["ID"];

        /// <summary>
        /// Gets the part group class.
        /// </summary>
        /// <value>The part group class.</value>
        public ManagementPackClass PartGroupClass => this.partGroupClass ?? (this.partGroupClass = MGroup.Instance.GetManagementPackClass("ESight.PartGroup"));

        /// <summary>
        /// The part group key.
        /// </summary>
        /// <value>The part group key.</value>
        public ManagementPackProperty PartGroupKey => this.PartGroupClass.PropertyCollection["ID"];

        /// <summary>
        /// Gets the state of the close.
        /// </summary>
        /// <value>The state of the close.</value>
        public MonitoringAlertResolutionState CloseState
        {
            get
            {
                return this.closeState ?? (this.closeState = MGroup.Instance.OperationalData.GetMonitoringAlertResolutionStates().ToList().FirstOrDefault(x => x.Name == "Closed"));
            }
        }

        /// <summary>
        /// The new state
        /// </summary>
        private MonitoringAlertResolutionState newState;

        /// <summary>
        /// Gets the state of the New.
        /// </summary>
        /// <value>The state of the close.</value>
        public MonitoringAlertResolutionState NewState
        {
            get
            {
                return this.newState ?? (this.newState = MGroup.Instance.OperationalData.GetMonitoringAlertResolutionStates().ToList().FirstOrDefault(x => x.Name == "New"));
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// 创建逻辑Group
        /// </summary>
        /// <param name="mpClass">The class.</param>
        /// <param name="parentDn">The parent dn.</param>
        /// <param name="parentKey">The parent key.</param>
        /// <returns>Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject.</returns>
        public MPObject CreateLogicalChildGroup(ManagementPackClass mpClass, string parentDn, string parentKey)
        {
            var obj = new MPObject(MGroup.Instance, mpClass); // 实例化一个class

            obj[this.HuaweiServerKey].Value = parentDn;
            obj[this.PartChildGroupKey].Value = parentKey + "-" + mpClass.DisplayName;
            obj[this.DisplayNameField].Value = mpClass.DisplayName;
            return obj;
        }

        /// <summary>
        /// 创建逻辑Group
        /// </summary>
        /// <param name="mpClass">The class.</param>
        /// <param name="parentKey">The parent key.</param>
        /// <returns>Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject.</returns>
        public MPObject CreateLogicalGroup(ManagementPackClass mpClass, string parentKey)
        {
            var obj = new MPObject(MGroup.Instance, mpClass); // 实例化一个class

            obj[this.HuaweiServerKey].Value = parentKey;
            obj[this.PartGroupKey].Value = parentKey + "-" + mpClass.DisplayName;
            obj[this.DisplayNameField].Value = mpClass.DisplayName;
            return obj;
        }

        /// <summary>
        /// DN是否存在
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="mpClass">The mp class.</param>
        /// <returns>PartialMonitoringObject.</returns>
        public bool ExsitsDeviceId(string deviceId, ManagementPackClass mpClass)
        {
            MGroup.Instance.CheckConnection();
            var criteria = new MonitoringObjectCriteria($"DN = '{deviceId}'", mpClass);
            var reader =
                MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(
                    criteria,
                    ObjectQueryOptions.Default);
            return reader.Any();
        }

        /// <summary>
        /// The get object.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="mpClass">The class.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetObject(string expression, ManagementPackClass mpClass)
        {
            MGroup.Instance.CheckConnection();
            var criteria = new MonitoringObjectCriteria(expression, mpClass);
            var reader =
                MGroup.Instance.EntityObjects.GetObjectReader<MonitoringObject>(criteria, ObjectQueryOptions.Default);
            return reader.FirstOrDefault();
        }

        /// <summary>
        /// Deletes the servers on synchronize.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="newDeviceIds">The new device ids.</param>
        /// <param name="mpClass">The mp class.</param>
        public void RemoveServersOnSync(string eSightIp, List<string> newDeviceIds, ManagementPackClass mpClass)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Check Removed Servers On Polling.[mpClass:{mpClass}][curQueryResult:{string.Join(",", newDeviceIds)}]");
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"eSight = '{eSightIp}'", mpClass);
                var exsitDevices = MGroup.Instance.EntityObjects.GetObjectReader<MonitoringObject>(criteria, ObjectQueryOptions.Default).ToList();

                var deleteDevices = exsitDevices.Where(x =>
                        newDeviceIds.All(newDeviceId => newDeviceId != x[this.HuaweiServerKey].Value.ToString()))
                    .ToList();

                var discovery = new IncrementalDiscoveryData();
                deleteDevices.ForEach(deleteDevice =>
                {
                    discovery.Remove(deleteDevice);
                });
                discovery.Commit(this.MontioringConnector);

                if (deleteDevices.Any())
                {
                    HWLogger.GetESightSdkLogger(eSightIp).Info($"RemoveServers OnSync:delete servers[{mpClass.Name}]:{string.Join(",", deleteDevices.Select(x => x[this.HuaweiServerKey].Value.ToString()).Distinct())} ");
                }
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error($"RemoveServers OnSync.", e);
            }
        }

        /// <summary>
        /// Removes the server by device identifier.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="deviceId">The device identifier.</param>
        public void RemoveServerByDeviceId(string eSightIp, string deviceId)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"RemoveServerByDeviceId.[{deviceId}]");
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"DN = '{deviceId}'", HuaweiServerClass);
                var reader = MGroup.Instance.EntityObjects.GetObjectReader<MonitoringObject>(criteria, ObjectQueryOptions.Default);
                if (reader.Any())
                {
                    var existingObject = reader.First();
                    var discovery = new IncrementalDiscoveryData();
                    discovery.Remove(existingObject);
                    discovery.Commit(this.MontioringConnector);
                }
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error($"RemoveServerByDeviceId.", e);
            }
        }

        /// <summary>
        /// 删除Server
        /// </summary>
        /// <param name="mpClass">The class.</param>
        public void RemoverServers(ManagementPackClass mpClass)
        {
            try
            {
                HWLogger.Service.Info($"RemoverServers by MpClass.[{mpClass}]");
                MGroup.Instance.CheckConnection();
                var devices = MGroup.Instance.EntityObjects.GetObjectReader<MonitoringObject>(mpClass, ObjectQueryOptions.Default).ToList();
                if (devices.Any())
                {
                    var discovery = new IncrementalDiscoveryData();
                    devices.ForEach(device => discovery.Remove(device));
                    discovery.Commit(this.MontioringConnector);
                }
            }
            catch (Exception ex)
            {
                HWLogger.Service.Error("RemoverServers", ex);
                throw;
            }
        }

        /// <summary>
        /// 根据eSight删除Server以及父级的Computer
        /// </summary>
        /// <param name="mpClass">The class.</param>
        /// <param name="eSightIp">The eSightIp.</param>
        public void RemoverServersByESight(ManagementPackClass mpClass, string eSightIp)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"RemoverServersByESight.[{eSightIp}]");
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"eSight = '{eSightIp}'", mpClass);
                var devices = MGroup.Instance.EntityObjects.GetObjectReader<MonitoringObject>(criteria, ObjectQueryOptions.Default).ToList();
                if (devices.Any())
                {
                    var discovery = new IncrementalDiscoveryData();
                    devices.ForEach(device => discovery.Remove(device));
                    discovery.Commit(this.MontioringConnector);
                }
            }
            catch (Exception ex)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error("RemoverServersByESight", ex);
            }
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertDeviceChangeEvent(ManagementPackClass mpClass, DeviceChangeEventData eventData, string eSightIp)
        {
            try
            {
                var logger = HWLogger.GetESightSdkLogger(eventData.ESightIp);
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Start Insert DeviceChangeEvent.[{JsonUtil.SerializeObject(eventData)}]");
                MGroup.Instance.CheckConnection();
                var obj = GetObjectByDeviceId(mpClass, eventData.DeviceId);

                if (obj == null)
                {
                    logger.Warn($"InsertDeviceChangeEvent:Can not find the MonitoringObject:{eventData.DeviceId}");
                    return;
                }
                var isReady = CheckAndWaitHealthStateReady(mpClass, obj, eventData.DeviceId, eSightIp);
                if (!isReady)
                {
                    logger.Warn($"InsertDeviceChangeEvent:The MonitoringObject state is uninitialized.Drop the event.");
                    return;
                }
                obj?.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
            }
            catch (Exception ex)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error(ex, "Insert DeviceChangeEvent Error");
            }
        }

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertEvent(ManagementPackClass mpClass, EventData eventData, string eSightIp)
        {
            var logger = HWLogger.GetESightSdkLogger(eventData.ESightIp);
            try
            {
                var sn = eventData.AlarmSn.ToString();
                MGroup.Instance.CheckConnection();
                var logPre = $"[Sn={sn}] [OptType={eventData.OptType}] [LevelId={eventData.LevelId.ToString()}] ";
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Start Insert Event.[{JsonUtil.SerializeObject(eventData)}]");
                var obj = GetObjectByDeviceId(mpClass, eventData.DeviceId);
                if (obj == null)
                {
                    logger.Warn($"{logPre} Can not find the MonitoringObject:{eventData.DeviceId}");
                    return;
                }
                var isReady = CheckAndWaitHealthStateReady(mpClass, obj, eventData.DeviceId, eSightIp);
                if (!isReady)
                {
                    logger.Warn($"{logPre} The MonitoringObject state is uninitialized.Drop the event.");
                    return;
                }
                var eventHistory = obj.GetMonitoringEvents().Where(x => x.EventData.Contains("<AlarmData>")).ToList();
                switch (eventData.OptType)
                {
                    case 1:
                    case 5:
                        #region 新增/修改 告警-告警会重复上报
                        //如果不存在，则插入
                        //如果上次安装时的事件未清除，本次同步后，一个sn会存在两条数据，需要取最新添加的一条
                        var existEvent = eventHistory.OrderByDescending(x => x.TimeAdded).FirstOrDefault(x => x.GetAlarmData().AlarmSN.ToString() == sn);
                        if (existEvent == null || existEvent.TimeAdded < MGroup.Instance.MpInstallTime)
                        {
                            obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                            logger.Info($"{logPre}Insert new Event.");
                            if (eventData.LevelId == EventLogEntryType.Error || eventData.LevelId == EventLogEntryType.Warning)
                            {
                                if (eventData.AlarmData.Cleared)//如果告警是清除状态
                                {
                                    logger.Info($"{logPre}Need to close Event when insert.");
                                    Task.Run(() =>
                                    {
                                        int i = 0;
                                        while (i < 10)
                                        {
                                            i++;
                                            Thread.Sleep(TimeSpan.FromMinutes(1));
                                            var alertToClose = obj.GetMonitoringAlerts().FirstOrDefault(x => x.CustomField6 == sn);
                                            if (alertToClose != null)
                                            {
                                                alertToClose.ResolutionState = this.CloseState.ResolutionState;
                                                var comment = eventData.AlarmData.ClearedType.ToString();
                                                alertToClose.Update(comment);
                                                logger.Info($"{logPre}Close Event success.");
                                                break;
                                            }
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            #region 存在则更新
                            var alertHistory = obj.GetMonitoringAlerts();
                            var alertToUpdate = alertHistory.FirstOrDefault(x => x.CustomField6 == sn);
                            if (alertToUpdate != null)
                            {
                                alertToUpdate.CustomField2 = eventData.AlarmData.AdditionalInformation;
                                alertToUpdate.CustomField3 = eventData.AlarmData.AdditionalText;
                                alertToUpdate.CustomField4 = eventData.AlarmData.AlarmId.ToString();
                                alertToUpdate.CustomField5 = eventData.AlarmData.AlarmName;
                                alertToUpdate.CustomField6 = eventData.AlarmData.AlarmSN.ToString();
                                alertToUpdate.CustomField7 = TimeHelper.StampToDateTime(eventData.AlarmData.ArrivedTime.ToString()).ToString();
                                alertToUpdate.CustomField8 = eventData.AlarmData.DevCsn.ToString();
                                alertToUpdate.CustomField9 = eventData.AlarmData.EventType.ToString();
                                alertToUpdate.CustomField10 = eventData.AlarmData.MoName;

                                alertToUpdate.Update(eventData.AlarmData.AdditionalInformation);
                                logger.Debug($"{logPre}Update Event.");
                                if (eventData.AlarmData.Cleared)//如果告警是清除状态
                                {
                                    alertToUpdate.ResolutionState = this.CloseState.ResolutionState;
                                    alertToUpdate.Update(eventData.AlarmData.AdditionalInformation);
                                    logger.Info($"{logPre}Close Alert On Update Event.");
                                }
                                else
                                {
                                    //如果原来的告警是关闭状态，本次是Open,则重新打开告警
                                    if (alertToUpdate.ResolutionState == this.CloseState.ResolutionState)
                                    {
                                        alertToUpdate.ResolutionState = this.NewState.ResolutionState;
                                        alertToUpdate.Update(eventData.AlarmData.AdditionalInformation);
                                        logger.Info($"{logPre}Reopen Alert On Update Event.");
                                    }
                                }
                            }
                            else
                            {
                                logger.Warn($"{logPre}Ingore Event.Can not find the alert.");
                            }
                            #endregion
                        }
                        #endregion
                        break;
                    case 2:
                        #region 清除告警
                        var _alertHistory = obj.GetMonitoringAlerts();
                        var _alertToClose = _alertHistory.FirstOrDefault(x => x.CustomField6 == sn);
                        if (_alertToClose != null)
                        {
                            _alertToClose.ResolutionState = this.CloseState.ResolutionState;
                            var comment = eventData.AlarmData.ClearedType.ToString();
                            _alertToClose.Update(comment);
                            logger.Info($"{logPre}Close Event.");
                        }
                        else
                        {
                            logger.Warn($"{logPre}Ingore Event.Can not find the alert.");
                        }
                        #endregion
                        break;
                    case 6:
                        #region 插入事件
                        if (eventData.LevelId == EventLogEntryType.Information)
                        {
                            var existAlarmDatas = obj.GetMonitoringEvents().Where(x => x.EventData.StartsWith("<AlarmData")).Select(x => x.GetAlarmData()).ToList();
                            //插入事件
                            if (existAlarmDatas.All(x => x.AlarmSN.ToString() != sn))
                            {
                                logger.Info($"{logPre}Insert new Event.");
                                obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                            }
                            else
                            {
                                logger.Warn($"{logPre}Ignore Event.The event is exist."); //忽略已存在
                            }
                        }
                        else
                        {
                            logger.Warn($"{logPre}Ignore Event."); //忽略非事件
                        }
                        break;
                    #endregion
                    default:
                        HWLogger.GetESightSdkLogger(eSightIp).Error($"Unknown optType {eventData.OptType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error(ex);
            }
        }

        /// <summary>
        /// The install.
        /// </summary>
        /// <exception cref="Exception">ex</exception>
        public void Install()
        {
            try
            {
                var connector = this.GetConnector();
                if (connector == null)
                {
                    HWLogger.Service.Info($"Start install {this.ConnectorName}");
                    var cfMgmt = MGroup.Instance.GetConnectorFramework();
                    this.MontioringConnector = cfMgmt.Setup(this.ConnectorInfo, this.ConnectorGuid);
                    HWLogger.Service.Info($"{this.ConnectorName} install finish.");
                }
                else
                {
                    this.MontioringConnector = connector;
                    //HwLogger.Sdk.Info($"Skip install {this.ConnectorName}");
                }
                if (!this.MontioringConnector.Initialized)
                {
                    this.MontioringConnector.Initialize();
                }
            }
            catch (Exception ex)
            {
                HWLogger.Service.Error(ex, "Install connector error:");
                throw;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the parent server.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>PartialMonitoringObject.</returns>
        protected PartialMonitoringObject GetParentServer(MonitoringObject obj)
        {
            var group = obj.GetParentPartialMonitoringObjects();
            if (group.Any())
            {
                var parent = group.First();
                var t = parent.GetParentPartialMonitoringObjects();
                if (t.Any())
                {
                    return t.First();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the full parent server.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>MonitoringObject.</returns>
        protected MonitoringObject GetFullParentServer(MonitoringObject obj)
        {
            var group = obj.GetParentPartialMonitoringObjects();
            if (group.Any())
            {
                var parent = group.First();
                var t = parent.GetParentMonitoringObjects();
                if (t.Any())
                {
                    return t.First();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the object by device identifier.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>PartialMonitoringObject.</returns>
        private PartialMonitoringObject GetObjectByDeviceId(ManagementPackClass mpClass, string deviceId)
        {
            var criteria = new MonitoringObjectCriteria($"DN = '{deviceId}'", mpClass);
            var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
            if (!reader.Any())
            {
                return null;
                //throw new Exception($"cannot find deviceId :'{deviceId}'");
            }
            return reader.First();
        }

        /// <summary>
        /// 插入告警时，等待新增的对象的healthState不再是Not Monitor
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="obj">The object.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <returns>PartialMonitoringObject.</returns>
        public bool CheckAndWaitHealthStateReady(ManagementPackClass mpClass, PartialMonitoringObject obj, string deviceId, string eSightIp)
        {
            var logger = HWLogger.GetESightSdkLogger(eSightIp);
            if (obj.StateLastModified == null)
            {
                //如果对象添加超过5分钟，仍然没有健康状态，防止阻塞只查询一次
                if ((DateTime.Now - obj.TimeAdded).TotalMinutes > 5)
                {
                    obj = GetObjectByDeviceId(mpClass, deviceId);
                    if (obj.HealthState != HealthState.Uninitialized)
                    {
                        logger.Info($"{deviceId} first healthState is {obj.HealthState}.");
                        return true;
                    }
                    return false;
                }
                #region 新增对象
                logger.Info($"New Object:{deviceId}");
                int i = 0;
                while (i < 48)
                {
                    i++;
                    // 重新查询obj状态
                    obj = GetObjectByDeviceId(mpClass, deviceId);
                    if (obj.HealthState != HealthState.Uninitialized)
                    {
                        logger.Info($"{deviceId} first healthState is {obj.HealthState}.");
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        return true;
                    }
                    logger.Info($"wait {deviceId} first Initialized...");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                return false;
                #endregion
            }
            return true;
        }

        /// <summary>
        /// Determines whether this instance is install.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        private MonitoringConnector GetConnector()
        {
            try
            {
                MGroup.Instance.CheckConnection();
                var cfMgmt = MGroup.Instance.GetConnectorFramework();
                return cfMgmt.GetConnector(this.ConnectorGuid);
            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 检查未关闭的告警，在本次历史告警查询中是否存在，不存在则关闭
        /// 防止遗漏
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="sns">The SNS.</param>
        public void CheckUnclosedAlert(ManagementPackClass mpClass, string eSightIp, List<string> sns)
        {
            //读取所有的未关闭的告警
            var unCloseAlarm = MGroup.Instance.OperationalData.GetMonitoringAlerts(
                new MonitoringAlertCriteria($"ResolutionState = '0' And CustomField1 like '%{eSightIp}%'"),
                mpClass, TraversalDepth.OneLevel, null).ToList();
            //判断这些未关闭的告警是否在本次的查询结果中
            var needManualCloseAlert = unCloseAlarm.Where(x => sns.All(y => y != x.CustomField6));
            foreach (var monitoringAlert in needManualCloseAlert)
            {
                monitoringAlert.ResolutionState = this.CloseState.ResolutionState;
                monitoringAlert.Update("Manaul Close By SDK");
                HWLogger.Service.Info("Manaul Close By SDK,SNS:" + monitoringAlert.CustomField6);
            }
        }

        /// <summary>
        /// Gets the exist events.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <returns>List&lt;MonitoringEvent&gt;.</returns>
        public List<MonitoringEvent> GetExistEvents(ManagementPackClass mpClass, string eSightIp)
        {
            //https://docs.microsoft.com/en-us/previous-versions/system-center/developer/bb423658(v=msdn.10)
            var list = MGroup.Instance.OperationalData.GetMonitoringEvents(
                new MonitoringEventCriteria($"LoggingComputer = '{eSightIp}'"),
                mpClass, TraversalDepth.OneLevel).ToList();
            return list;
        }

        /// <summary>
        /// Gets the history alarm datas.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eSightIp">The eSight Ip.</param>
        /// <returns>List&lt;AlarmData&gt;.</returns>
        public List<AlarmData> GetExistAlarmDatas(ManagementPackClass mpClass, string eSightIp)
        {
            var existEvents = GetExistEvents(mpClass, eSightIp);
           // HWLogger.Service.Info($"ExistEvents :[{ JsonUtil.SerializeObject(existEvents)}]");
            return existEvents.Where(x => x.EventData.Contains("<AlarmData>")).Select(x => x.GetAlarmData()).ToList();
        }
    }
}