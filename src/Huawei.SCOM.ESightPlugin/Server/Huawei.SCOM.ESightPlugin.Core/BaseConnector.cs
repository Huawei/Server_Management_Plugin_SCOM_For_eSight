// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 11-19-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="BaseConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The base connector.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core.Const;
    using Huawei.SCOM.ESightPlugin.Core.Models;

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
        public Guid ConnectorGuid { get; set; }

        /// <summary>
        /// Gets or sets the connector info.
        /// </summary>
        public ConnectorInfo ConnectorInfo { get; set; }

        /// <summary>
        /// Gets or sets the connector name.
        /// </summary>
        public string ConnectorName { get; set; }

        /// <summary>
        /// The display name field.
        /// </summary>
        public ManagementPackProperty DisplayNameField => this.BaseEntityClass.PropertyCollection["DisplayName"];

        /// <summary>
        /// Gets the huawei server class.
        /// </summary>
        public ManagementPackClass HuaweiServerClass => this.huaweiServerClass ?? (this.huaweiServerClass = MGroup.Instance.GetManagementPackClass(EntityTypeConst.ESight.HuaweiServer));

        /// <summary>
        /// The huawei server key.
        /// </summary>
        public ManagementPackProperty HuaweiServerKey => this.HuaweiServerClass.PropertyCollection["DN"];

        /// <summary>
        /// Gets or sets the montioring connector.
        /// </summary>
        /// <value>The montioring connector.</value>
        public MonitoringConnector MontioringConnector { get; set; }

        /// <summary>
        /// Gets the part child group class.
        /// </summary>
        public ManagementPackClass PartChildGroupClass => this.partChildGroupClass ?? (this.partChildGroupClass = MGroup.Instance.GetManagementPackClass("ESight.PartChildGroup"));

        /// <summary>
        /// The part child group key.
        /// </summary>
        public ManagementPackProperty PartChildGroupKey => this.PartChildGroupClass.PropertyCollection["ID"];

        /// <summary>
        /// Gets the part group class.
        /// </summary>
        public ManagementPackClass PartGroupClass => this.partGroupClass ?? (this.partGroupClass = MGroup.Instance.GetManagementPackClass("ESight.PartGroup"));

        /// <summary>
        /// The part group key.
        /// </summary>
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
        /// 插入历史告警-插入前请先排重
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eventDatas">The event datas.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertHistoryEvent(ManagementPackClass mpClass, List<EventData> eventDatas, string eSightIp)
        {
            HWLogger.GetESightSdkLogger(eSightIp).Info($"Start InsertHistoryEvent.[{JsonUtil.SerializeObject(eventDatas)}]");
            // 获取到历史的事件记录
            // var eventHistory = obj.GetMonitoringEvents();
            // 过滤掉已经存在的事件-如果已存在，则不再重复添加
            // var filterEventList = eventDatas.Where(y => eventHistory.All(x => x.Parameters[5] != y.AlarmSn.ToString())).ToList();
            var deviceId = eventDatas[0].DeviceId;
            var obj = this.GetReadyObject(mpClass, deviceId, eSightIp);
            var alertHistory = obj.GetMonitoringAlerts();
            // 过滤掉已经存在的告警 
            var filterAlertList = eventDatas.Where(y => alertHistory.All(x => x.CustomField6 != y.AlarmSn.ToString())).ToList();

            HWLogger.GetESightSdkLogger(eSightIp).Info($"Filter HistoryEvent.[deviceId:{deviceId}] [Event Count:{eventDatas.Count}] [New Alert Count:{filterAlertList.Count}]");
            if (!filterAlertList.Any())
            {
                return;
            }
            foreach (var eventData in filterAlertList)
            {
                try
                {
                    obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                }
                catch (Exception ex)
                {
                    HWLogger.GetESightSdkLogger(eSightIp).Error($"InsertHistoryEvent Error.AlarmSn: {eventData.AlarmSn}", ex);
                }
            }
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertDeviceChangeEvent(ManagementPackClass mpClass, DeviceChangeEventData deviceChangeEventData, string eSightIp)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Start Insert DeviceChangeEvent.[{JsonUtil.SerializeObject(deviceChangeEventData)}]");
                MGroup.Instance.CheckConnection();
                PartialMonitoringObject obj = this.GetNewReadyObject(mpClass, deviceChangeEventData.DeviceId, eSightIp);
                obj?.InsertCustomMonitoringEvent(deviceChangeEventData.ToCustomMonitoringEvent());
            }
            catch (Exception ex)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error("Insert DeviceChangeEvent Error", ex);
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
            try
            {
                MGroup.Instance.CheckConnection();
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Start Insert Event.[{JsonUtil.SerializeObject(eventData)}]");
                PartialMonitoringObject obj = GetNewReadyObject(mpClass, eventData.DeviceId, eSightIp);
                if (obj == null)
                {
                    return;
                }
                var alertHistory = obj.GetMonitoringAlerts();
                if (eventData.OptType == 1 || eventData.OptType == 6)
                {
                    // 如果已存在，则不再重复添加
                    if (alertHistory.All(x => x.CustomField6 != eventData.AlarmSn.ToString()))
                    {
                        obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                    }
                }
                else if (eventData.OptType == 2)
                {
                    var alerts = obj.GetMonitoringAlerts().Where(x => x.CustomField6 == eventData.AlarmSn.ToString()).ToList();
                    if (!alerts.Any())
                    {
                        // throw new Exception($"cannot find alert '{eventData.AlarmSn}'");
                    }
                    else
                    {
                        alerts.ForEach(alert =>
                            {
                                alert.ResolutionState = this.CloseState.ResolutionState;
                                alert.Update(eventData.AlarmData.Comments);
                            });
                    }
                }
                else if (eventData.OptType == 5)
                {
                    var alerts = obj.GetMonitoringAlerts().Where(x => x.CustomField6 == eventData.AlarmSn.ToString()).ToList();
                    if (!alerts.Any())
                    {
                        // 未找到，则当做是新增告警
                        obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                    }
                    else
                    {
                        alerts.ForEach(alert =>
                            {
                                alert.CustomField2 = eventData.AlarmData.AdditionalInformation;
                                alert.CustomField3 = eventData.AlarmData.AdditionalText;
                                alert.CustomField4 = eventData.AlarmData.AlarmId.ToString();
                                alert.CustomField5 = eventData.AlarmData.AlarmName;
                                alert.CustomField6 = eventData.AlarmData.AlarmSN.ToString();
                                alert.CustomField7 = TimeHelper.StampToDateTime(eventData.AlarmData.ArrivedTime.ToString()).ToString();
                                alert.CustomField8 = eventData.AlarmData.DevCsn.ToString();
                                alert.CustomField9 = eventData.AlarmData.EventType.ToString();
                                alert.CustomField10 = eventData.AlarmData.MoName;

                                alert.Update(eventData.AlarmData.Comments);
                            });
                    }
                }
                else
                {
                    HWLogger.GetESightSdkLogger(eSightIp).Error($"Unknown optType {eventData.OptType}");
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
                HWLogger.Service.Error("Install connector error:", ex);
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
        /// 插入告警时，等待对象的healthState不再是Not Monitor
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <returns>PartialMonitoringObject.</returns>
        private PartialMonitoringObject GetReadyObject(ManagementPackClass mpClass, string deviceId, string eSightIp)
        {
            MGroup.Instance.CheckConnection();
            var obj = this.GetObjectByDeviceId(mpClass, deviceId);
            if (obj.StateLastModified == null)
            {
                #region 新增对象
                HWLogger.GetESightSdkLogger(eSightIp).Debug($"New Object:{deviceId}");
                while (true)
                {
                    // 重新查询obj状态
                    obj = this.GetObjectByDeviceId(mpClass, deviceId);
                    if (obj.HealthState != HealthState.Uninitialized)
                    {
                        HWLogger.GetESightSdkLogger(eSightIp).Debug($"{deviceId} first healthState is {obj.HealthState}.");
                        break;
                    }
                    HWLogger.GetESightSdkLogger(eSightIp).Debug($"wait {deviceId} first Initialized...");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                #endregion
            }
            else
            {
                #region 对象之前缓存过

                HWLogger.GetESightSdkLogger(eSightIp).Debug($"Existed Object before:{deviceId}. StateLastModified(Utc):{obj.StateLastModified}.");
                //var stateLastModified = obj.StateLastModified;
                //var startTime = DateTime.UtcNow;
                //// 距离上次状态变更大于5分钟 说明需要等待刷新健康状态
                //if ((startTime - stateLastModified.Value).TotalMinutes > 5)
                //{
                //    // 最多尝试5分钟
                //    while ((DateTime.UtcNow - startTime).TotalMinutes < 5)
                //    {
                //        // 重新查询obj状态
                //        obj = this.GetObjectByDeviceId(mpClass, deviceId);
                //        if (obj.StateLastModified > stateLastModified.Value)
                //        {
                //            HwLogger.Sdk.Debug($"{deviceId} Reset HealthState :{obj.HealthState} StateLastModified(Utc):{obj.StateLastModified} UtcNow:{DateTime.UtcNow}.");
                //            break;
                //        }
                //        HwLogger.Sdk.Debug($"wait {deviceId}  Reinitialize HealthState...");
                //        Thread.Sleep(TimeSpan.FromSeconds(5));
                //    }
                //}
                //else
                //{
                //    // 距离上次状态变更小于5分钟
                //    HwLogger.Sdk.Debug($"{deviceId} healthState :{obj.HealthState} StateLastModified:(Utc){obj.StateLastModified}  UtcNow:{startTime}.");
                //}
                #endregion
            }
            return obj;
        }

        /// <summary>
        /// 插入告警时，等待新增的对象的healthState不再是Not Monitor
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <returns>PartialMonitoringObject.</returns>
        private PartialMonitoringObject GetNewReadyObject(ManagementPackClass mpClass, string deviceId, string eSightIp)
        {
            MGroup.Instance.CheckConnection();
            var obj = this.GetObjectByDeviceId(mpClass, deviceId);

            if (obj != null && obj.StateLastModified == null)
            {
                #region 新增对象
                HWLogger.GetESightSdkLogger(eSightIp).Debug($"New Object:{deviceId}");
                while (true)
                {
                    // 重新查询obj状态
                    obj = this.GetObjectByDeviceId(mpClass, deviceId);
                    if (obj.HealthState != HealthState.Uninitialized)
                    {
                        HWLogger.GetESightSdkLogger(eSightIp).Debug($"{deviceId} first healthState is {obj.HealthState}.");
                        break;
                    }
                    HWLogger.GetESightSdkLogger(eSightIp).Debug($"wait {deviceId} first Initialized...");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                #endregion
            }
            return obj;
        }

        /// <summary>
        /// 判断首次插入事件是否成功
        /// 首次安装后 第一次插入事件会失败
        /// 此处进行多次查找，以确定事件插入成功
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="firstEvent">The first event.</param>
        private void FindFirstEvent(PartialMonitoringObject obj, EventData firstEvent)
        {
            var ev = firstEvent.ToCustomMonitoringInitEvent();
            obj.InsertCustomMonitoringEvent(ev);
            int i = 0;
            while (i < 100)
            {
                var eventHistory = obj.GetMonitoringEvents();
                HWLogger.Service.Debug($"try Find FirstEvent:{i}.AlarmSn: {firstEvent.AlarmSn}. eventHistory Count:{eventHistory.Count}");
                if (eventHistory.Any(x => x.Parameters[5] == firstEvent.AlarmSn.ToString()))
                {
                    HWLogger.Service.Debug($"Find FirstEvent Finish:{i}.AlarmSn: {firstEvent.AlarmSn}");
                    break;
                }
                Thread.Sleep(2000);
                i++;
            }
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
    }
}