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
        /// The base computer class.
        /// </summary>
        private ManagementPackClass baseComputerClass;

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
        /// Gets the base computer class.
        /// </summary>
        /// <value>The base computer class.</value>
        public ManagementPackClass BaseComputerClass => this.baseComputerClass ?? (this.baseComputerClass = MGroup.Instance.GetManagementPackClass("Microsoft.Windows.Computer"));

        /// <summary>
        /// Gets the base entity class.
        /// </summary>
        /// <value>The base entity class.</value>
        public ManagementPackClass BaseEntityClass => this.baseEntityClass ?? (this.baseEntityClass = MGroup.Instance.GetManagementPackClass("System.Entity"));

        /// <summary>
        /// The computer key.
        /// </summary>
        public ManagementPackProperty ComputerKey => this.BaseComputerClass.PropertyCollection["PrincipalName"];

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
        /// The create computer.
        /// </summary>
        /// <param name="principalName">The principal name.</param>
        /// <returns>The <see cref="MPObject" />.</returns>
        public MPObject CreateComputer(string principalName)
        {
            var obj = new MPObject(MGroup.Instance, this.BaseComputerClass); // 实例化一个class
            obj[this.BaseComputerClass.PropertyCollection["PrincipalName"]].Value = principalName;
            return obj;
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
            obj[this.ComputerKey].Value = parentDn;

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
            obj[this.ComputerKey].Value = parentKey;

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
        /// Gets the computer by dn.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>Microsoft.EnterpriseManagement.Monitoring.MonitoringObject.</returns>
        public MonitoringObject GetComputerByDeviceId(string deviceId)
        {
            MGroup.Instance.CheckConnection();
            return this.GetObject($"PrincipalName = '{deviceId}'", this.BaseComputerClass);
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
        /// 根据dn移除Computer
        /// </summary>
        /// <param name="deviceId">
        /// The dn.
        /// </param>
        public void RemoveComputerByDeviceId(string deviceId)
        {
            MGroup.Instance.CheckConnection();
            var existingObject = this.GetComputerByDeviceId(deviceId);
            if (existingObject != null)
            {
                var discovery = new IncrementalDiscoveryData();
                discovery.Remove(existingObject);
                discovery.Commit(this.MontioringConnector);
            }
        }

        /// <summary>
        /// 删除Server以及父级的Computer
        /// </summary>
        /// <param name="mpClass">The class.</param>
        public void RemoverServers(ManagementPackClass mpClass)
        {
            try
            {
                MGroup.Instance.CheckConnection();
                var blades =
                    MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(
                        mpClass,
                        ObjectQueryOptions.Default);
                var deviceIds = blades.ToList().Select(x => x.Path).ToList();
                var discovery = new IncrementalDiscoveryData();
                deviceIds.ForEach(
                    deviceId =>
                        {
                            var existingObject = this.GetComputerByDeviceId(deviceId);
                            if (existingObject != null)
                            {
                                discovery.Remove(existingObject);
                            }
                        });
                discovery.Commit(this.MontioringConnector);
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error("RemoverServers", ex);
                throw;
            }
        }

        /// <summary>
        /// 根据eSight删除Server以及父级的Computer
        /// </summary>
        /// <param name="mpClass">
        /// The class.
        /// </param>
        /// <param name="eSightIp">
        /// The eSightIp.
        /// </param>
        public void RemoverServers(ManagementPackClass mpClass, string eSightIp)
        {
            try
            {
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"eSight = '{eSightIp}'", mpClass);
                var reader =
                    MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(
                        criteria,
                        ObjectQueryOptions.Default);
                var deviceIds = reader.ToList().Select(x => x.Path).ToList();
                var discovery = new IncrementalDiscoveryData();
                deviceIds.ForEach(
                    deviceId =>
                        {
                            var existingObject = this.GetComputerByDeviceId(deviceId);
                            if (existingObject != null)
                            {
                                discovery.Remove(existingObject);
                            }
                        });
                discovery.Commit(this.MontioringConnector);
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error("RemoverServers", ex);
            }
        }

        /// <summary>
        /// Inserts the child event.
        /// </summary>
        /// <param name="childClass">The child class.</param>
        /// <param name="eventData">The event data.</param>
        public void InsertChildEvent(ManagementPackClass childClass, EventData eventData)
        {
            MGroup.Instance.CheckConnection();
            var criteria = new MonitoringObjectCriteria($"DN = '{eventData.DeviceId}'  and eSight='{eventData.ESightIp}'", childClass);
            var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
            if (!reader.Any())
            {
                throw new Exception($"cannot find DN '{eventData.DeviceId}'");
            }
            var obj = reader.First();
            this.InsertEvent(obj, eventData);
        }

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eventData">The event data.</param>
        public void InsertEvent(ManagementPackClass mpClass, EventData eventData)
        {
            MGroup.Instance.CheckConnection();
            var criteria = new MonitoringObjectCriteria($"DN = '{eventData.DeviceId}'", mpClass);
            var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
            if (!reader.Any())
            {
                throw new Exception($"cannot find DN '{eventData.DeviceId}'");
            }
            var obj = reader.First();
            this.InsertEvent(obj, eventData);
        }

        /// <summary>
        /// Inserts the child event.
        /// </summary>
        /// <param name="childClass">The child class.</param>
        /// <param name="eventDatas">The event datas.</param>
        public void InsertChildHistoryEvent(ManagementPackClass childClass, List<EventData> eventDatas)
        {
            MGroup.Instance.CheckConnection();
            var deviceId = eventDatas[0].DeviceId;
            var eSightIp = eventDatas[0].ESightIp;

            var criteria = new MonitoringObjectCriteria($"DN = '{deviceId}' and eSight='{eSightIp}'", childClass);
            var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
            if (!reader.Any())
            {
                throw new Exception($"cannot find DN '{deviceId}'");
            }
            var obj = reader.First();
            this.InsertHistoryEvent(obj, eventDatas);
        }

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="eventDatas">The event datas.</param>
        public void InsertHistoryEvent(ManagementPackClass mpClass, List<EventData> eventDatas)
        {
            MGroup.Instance.CheckConnection();
            var deviceId = eventDatas[0].DeviceId;

            var criteria = new MonitoringObjectCriteria($"DN = '{deviceId}'", mpClass);
            var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
            if (!reader.Any())
            {
                throw new Exception($"cannot find DN '{deviceId}'");
            }
            var obj = reader.First();
            this.InsertHistoryEvent(obj, eventDatas);
        }

        /// <summary>
        /// 插入历史告警-插入前请先排重
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="eventDatas">The event datas.</param>
        private void InsertHistoryEvent(PartialMonitoringObject obj, List<EventData> eventDatas)
        {
            // 获取到历史的事件记录
            // var eventHistory = obj.GetMonitoringEvents();
            // 过滤掉已经存在的事件-如果已存在，则不再重复添加
            // var filterEventList = eventDatas.Where(y => eventHistory.All(x => x.Parameters[5] != y.AlarmSn.ToString())).ToList();

            var alertHistory = obj.GetMonitoringAlerts();
            // 过滤掉已经存在的告警 
            var filterAlertList = eventDatas.Where(y => alertHistory.All(x => x.CustomField6 != y.AlarmSn.ToString())).ToList();

            HWLogger.SERVICE.Info($"InsertHistoryEvent : [Event Count:{eventDatas.Count}] [New Alert Count:{filterAlertList.Count}]");
            if (!filterAlertList.Any())
            {
                return;
            }
            var firstData = filterAlertList.FirstOrDefault();
            if (firstData == null)
            {
                return;
            }
            this.FindFirstEvent(obj, firstData);
            Thread.Sleep(2 * 60 * 1000); // 睡眠120秒-alert首次生成慢。
            foreach (var eventData in filterAlertList)
            {
                try
                {
                    obj.InsertCustomMonitoringEvent(eventData.ToCustomMonitoringEvent());
                }
                catch (Exception ex)
                {
                    HWLogger.SERVICE.Error($"InsertHistoryEvent Error.AlarmSn: {eventData.AlarmSn}", ex);
                }
            }
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
            var ev = firstEvent.ToCustomMonitoringEvent();
            ev.LevelId = 2;
            ev.Channel = "scom plugin for eSight waring initialization";
            ev.Parameters[4] = "scom plugin for eSight waring initialization";
            obj.InsertCustomMonitoringEvent(ev);
            int i = 0;
            while (i < 100)
            {
                var eventHistory = obj.GetMonitoringEvents();
                HWLogger.SERVICE.Debug($"try Find FirstEvent:{i}.AlarmSn: {firstEvent.AlarmSn}. eventHistory Count:{eventHistory.Count}");
                if (eventHistory.Any(x => x.Parameters[5] == firstEvent.AlarmSn.ToString()))
                {
                    HWLogger.SERVICE.Debug($"Find FirstEvent Finish:{i}.AlarmSn: {firstEvent.AlarmSn}");
                    break;
                }
                Thread.Sleep(2000);
                i++;
            }
        }

        /// <summary>
        /// Inserts the child event.
        /// </summary>
        /// <param name="childClass">The child class.</param>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        public void InsertChildDeviceChangeEvent(ManagementPackClass childClass, DeviceChangeEventData deviceChangeEventData)
        {
            try
            {
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"DN = '{deviceChangeEventData.DeviceId}'  and eSight='{deviceChangeEventData.ESightIp}'", childClass);
                var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
                if (reader.Any())
                {
                    var obj = reader.First();
                    {
                        obj.InsertCustomMonitoringEvent(deviceChangeEventData.ToCustomMonitoringEvent());
                    }
                }
                else
                {
                    throw new Exception($"cannot find DN '{deviceChangeEventData.DeviceId}'");
                }
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="mpClass">The mp class.</param>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        public void InsertDeviceChangeEvent(ManagementPackClass mpClass, DeviceChangeEventData deviceChangeEventData)
        {
            try
            {
                MGroup.Instance.CheckConnection();
                var criteria = new MonitoringObjectCriteria($"DN = '{deviceChangeEventData.DeviceId}'", mpClass);
                var reader = MGroup.Instance.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);
                if (reader.Any())
                {
                    var obj = reader.First();
                    {
                        obj.InsertCustomMonitoringEvent(deviceChangeEventData.ToCustomMonitoringEvent());
                    }
                }
                else
                {
                    throw new Exception($"cannot find DN '{deviceChangeEventData.DeviceId}'");
                }
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="eventData">The event data.</param>
        private void InsertEvent(PartialMonitoringObject obj, EventData eventData)
        {
            try
            {

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
                    HWLogger.SERVICE.Error($"Unknown optType {eventData.OptType}");
                }
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error(ex);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether this instance is install.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public MonitoringConnector GetConnector()
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
                    HWLogger.SERVICE.Info($"Start install {this.ConnectorName}");
                    var cfMgmt = MGroup.Instance.GetConnectorFramework();
                    this.MontioringConnector = cfMgmt.Setup(this.ConnectorInfo, this.ConnectorGuid);
                    HWLogger.SERVICE.Info($"{this.ConnectorName} install finish.");
                }
                else
                {
                    this.MontioringConnector = connector;
                    //HWLogger.SERVICE.Info($"Skip install {this.ConnectorName}");
                }
                if (!this.MontioringConnector.Initialized)
                {
                    this.MontioringConnector.Initialize();
                }
            }
            catch (Exception ex)
            {
                HWLogger.SERVICE.Error("Install connector error:", ex);
                throw;
            }
        }
        #endregion
    }
}