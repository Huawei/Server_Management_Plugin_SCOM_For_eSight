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
// Created          : 12-11-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="KunLunConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Huawei.SCOM.ESightPlugin.Core
{
    using System;
    using System.Linq;

    using Huawei.SCOM.ESightPlugin.Core.Const;
    using Huawei.SCOM.ESightPlugin.Core.Models;
    using Huawei.SCOM.ESightPlugin.Models.Devices;
    using Huawei.SCOM.ESightPlugin.Models.Server;

    using LogUtil;

    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Configuration;
    using Microsoft.EnterpriseManagement.ConnectorFramework;
    using Microsoft.EnterpriseManagement.Monitoring;
    using MPObject = Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject;
    using System.Collections.Generic;

    /// <summary>
    /// The kun lun connector.
    /// </summary>
    public class KunLunConnector : BaseConnector
    {
        #region Fields

        /// <summary>
        /// The connector guid.
        /// </summary>
        private static Guid connectorGuid = new Guid("{528C8486-2E62-42FB-9AFB-96CB8C089864}");

        /// <summary>
        /// The instance.
        /// </summary>
        private static KunLunConnector instance;

        /// <summary>
        /// The disk class.
        /// </summary>
        private ManagementPackClass diskClass;

        /// <summary>
        /// The disk group class.
        /// </summary>
        private ManagementPackClass diskGroupClass;

        /// <summary>
        /// The fan class.
        /// </summary>
        private ManagementPackClass fanClass;

        /// <summary>
        /// The fan group class.
        /// </summary>
        private ManagementPackClass fanGroupClass;

        /// <summary>
        /// The kun lun class.
        /// </summary>
        private ManagementPackClass kunLunClass;

        /// <summary>
        /// The power supply class.
        /// </summary>
        private ManagementPackClass powerSupplyClass;

        /// <summary>
        /// The power supply group class.
        /// </summary>
        private ManagementPackClass powerSupplyGroupClass;

        /// <summary>
        /// The raid class.
        /// </summary>
        private ManagementPackClass raidClass;

        /// <summary>
        /// The raid group class.
        /// </summary>
        private ManagementPackClass raidGroupClass;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static KunLunConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KunLunConnector
                    {
                        ConnectorName = "KunLunServer.Connector",
                        ConnectorGuid = connectorGuid,
                        ConnectorInfo = new ConnectorInfo
                        {
                            Description = "KunLunServer Connector Description",
                            DisplayName = "KunLunServer Connector",
                            Name = "KunLunServer.Connector",
                            DiscoveryDataIsManaged = true
                        }
                    };
                    instance.Install();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the fan class.
        /// </summary>
        public ManagementPackClass FanClass
        {
            get
            {
                return this.fanClass ?? (this.fanClass =
                                             MGroup.Instance.GetManagementPackClass(EntityTypeConst.KunLunServer.Fan));
            }
        }

        /// <summary>
        /// Gets the fan group class.
        /// </summary>
        public ManagementPackClass FanGroupClass
        {
            get
            {
                return this.fanGroupClass ?? (this.fanGroupClass =
                                                  MGroup.Instance.GetManagementPackClass(
                                                      EntityTypeConst.KunLunServer.FanGroup));
            }
        }

        /// <summary>
        /// Gets the kun lun class.
        /// </summary>
        public ManagementPackClass KunLunClass => this.kunLunClass ?? (this.kunLunClass =
                                                                           MGroup.Instance.GetManagementPackClass(
                                                                               EntityTypeConst.KunLunServer.MainName));

        /// <summary>
        /// Gets the physical disk class.
        /// </summary>
        public ManagementPackClass PhysicalDiskClass => this.diskClass ?? (this.diskClass =
                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                   EntityTypeConst.KunLunServer.PhysicalDisk));

        /// <summary>
        /// Gets the physical disk group class.
        /// </summary>
        public ManagementPackClass PhysicalDiskGroupClass => this.diskGroupClass ?? (this.diskGroupClass =
                                                                                         MGroup.Instance.GetManagementPackClass(
                                                                                             EntityTypeConst.KunLunServer.PhysicalDiskGroup));

        /// <summary>
        /// Gets the power supply class.
        /// </summary>
        public ManagementPackClass PowerSupplyClass => this.powerSupplyClass ?? (this.powerSupplyClass =
                                                                                     MGroup.Instance.GetManagementPackClass(
                                                                                         EntityTypeConst.KunLunServer.PowerSupply));

        /// <summary>
        /// Gets the power supply group class.
        /// </summary>
        public ManagementPackClass PowerSupplyGroupClass => this.powerSupplyGroupClass ?? (this.powerSupplyGroupClass =
                                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                                   EntityTypeConst.KunLunServer.PowerSupplyGroup));

        /// <summary>
        /// Gets the raid class.
        /// </summary>
        public ManagementPackClass RaidClass => this.raidClass ?? (this.raidClass =
                                                                       MGroup.Instance.GetManagementPackClass(
                                                                           EntityTypeConst.KunLunServer.RaidController));

        /// <summary>
        /// Gets the raid group class.
        /// </summary>
        public ManagementPackClass RaidGroupClass => this.raidGroupClass ?? (this.raidGroupClass =
                                                                                 MGroup.Instance.GetManagementPackClass(
                                                                                     EntityTypeConst.KunLunServer.RaidControllerGroup));
        #endregion

        #region Public Methods

        /// <summary>
        /// Synchronizes the server.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SyncServer(KunLunServer model)
        {
            // 存在则更新
            if (ExsitsKunLunServer(model.DeviceId))
            {
                this.UpdateKunLun(model, true);
            }
            else
            {
                this.InsertDetials(model);
            }
        }

        /// <summary>
        /// The insert detials.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        private void InsertDetials(KunLunServer model)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Insert KunLun:{model.DN}");
                var discoveryData = new IncrementalDiscoveryData();

                #region KunLunServer

                var kunLunServer = this.CreateKunLunServer(model);
                discoveryData.Add(kunLunServer);

                #endregion

                #region Fan

                var fanGroup = this.CreateLogicalGroup(this.FanGroupClass, model.DeviceId);
                discoveryData.Add(fanGroup);
                model.FanList.ForEach(
                    x =>
                    {
                        var fan = this.CreateFan(x);
                        fan[this.PartGroupKey].Value = fanGroup[this.PartGroupKey].Value;

                        fan[this.HuaweiServerKey].Value = model.DeviceId;

                        discoveryData.Add(fan);
                    });

                #endregion

                #region PSU

                var powerSupplyGroup = this.CreateLogicalGroup(this.PowerSupplyGroupClass, model.DeviceId);
                discoveryData.Add(powerSupplyGroup);
                model.PowerSupplyList.ForEach(
                    x =>
                    {
                        var powerSupply = this.CreatePowerSupply(x);
                        powerSupply[this.PartGroupKey].Value = powerSupplyGroup[this.PartGroupKey].Value;
                        powerSupply[this.HuaweiServerKey].Value = model.DeviceId;

                        discoveryData.Add(powerSupply);
                    });

                #endregion

                #region Raid

                var raidGroup = this.CreateLogicalGroup(this.RaidGroupClass, model.DeviceId);
                discoveryData.Add(raidGroup);
                model.RaidList.ForEach(
                    y =>
                    {
                        var raid = this.CreateRaidControl(y);
                        raid[this.PartGroupKey].Value = raidGroup[this.PartGroupKey].Value;
                        raid[this.HuaweiServerKey].Value = model.DeviceId;

                        discoveryData.Add(raid);
                    });

                #endregion

                #region Disk

                var diskGroup = this.CreateLogicalGroup(this.PhysicalDiskGroupClass, model.DeviceId);
                discoveryData.Add(diskGroup);
                model.DiskList.ForEach(
                    x =>
                    {
                        var disk = this.CreateDisk(x);
                        disk[this.PartGroupKey].Value = diskGroup[this.PartGroupKey].Value;
                        disk[this.HuaweiServerKey].Value = model.DeviceId;

                        discoveryData.Add(disk);
                    });

                #endregion

                discoveryData.Commit(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Insert KunLun Error:{model.DN}", e);
            }
        }

        /// <summary>
        /// The update kun lun.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">是否是轮询</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="Exception">Can not find the server</exception>
        public void UpdateKunLun(KunLunServer model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start UpdateKunLun.[{model.DN}] [isPolling:{isPolling}]");
                var oldServer = this.GetKunLunServer(model.DeviceId);
                if (oldServer == null)
                {
                    throw new Exception($"Can not find the server:{model.DN}");
                }
                var propertys = this.KunLunClass.PropertyCollection; // 获取到class的属性
                var discoveryData = new IncrementalDiscoveryData();
                oldServer[this.DisplayNameField].Value = model.Name;
                if (model.Status != "-3")
                {
                    oldServer[propertys["Status"]].Value = model.StatusTxt;
                }
                oldServer[propertys["Vendor"]].Value = "HUAWEI";
                oldServer[propertys["UUID"]].Value = model.UUID;
                oldServer[propertys["IPAddress"]].Value = model.IpAddress;
                if (isPolling)
                {
                    oldServer[propertys["Manufacturer"]].Value = model.Manufacturer;
                }
                // oldBlade[propertys["iBMCVersion"]].Value = model.Version;
                oldServer[propertys["CPLDVersion"]].Value = string.Empty;
                oldServer[propertys["UbootVersion"]].Value = string.Empty;
                oldServer[propertys["ProductSn"]].Value = model.ProductSN;
                oldServer[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
                oldServer[propertys["CPUNums"]].Value = model.CPUNums;
                oldServer[propertys["CPUCores"]].Value = model.CPUCores;

                // oldBlade[propertys["ServerName"]].Value = model.ServerName;
                oldServer[propertys["BMCMacAddr"]].Value = model.BmcMacAddr;
                oldServer[propertys["eSight"]].Value = model.ESight;
                discoveryData.Add(oldServer);

                var fanGroup = oldServer.GetRelatedMonitoringObjects(this.FanGroupClass).First();
                discoveryData.Add(fanGroup);

                var relatedFanObjects = fanGroup.GetRelatedMonitoringObjects(this.FanClass);
                var deleteFan = relatedFanObjects.Where(
                        x => model.FanList.All(y => y.UUID != x[this.FanClass.PropertyCollection["UUID"]].Value.ToString()))
                    .ToList();
                deleteFan.ForEach(x => { discoveryData.Remove(x); });

                model.FanList.ForEach(
                    x =>
                    {
                        var oldFan = relatedFanObjects.FirstOrDefault(y => y[this.FanClass.PropertyCollection["UUID"]].Value.ToString() == x.UUID);
                        if (oldFan == null)
                        {
                            var newFan = this.CreateFan(x);
                            newFan[this.PartGroupKey].Value = fanGroup[this.PartGroupKey].Value;
                            newFan[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newFan);
                        }
                        else
                        {
                            this.UpdateFan(x, oldFan);
                            discoveryData.Add(oldFan);
                        }
                    });

                #region PSU

                var psuGroup = oldServer.GetRelatedMonitoringObjects(this.PowerSupplyGroupClass).First();
                discoveryData.Add(psuGroup);

                var relatedPsuObjects = psuGroup.GetRelatedMonitoringObjects(this.PowerSupplyClass);
                var deletePsu = relatedPsuObjects.Where(
                    x => model.PowerSupplyList.All(
                        y => y.UUID != x[this.PowerSupplyClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deletePsu.ForEach(x => { discoveryData.Remove(x); });
                model.PowerSupplyList.ForEach(
                    x =>
                    {
                        var oldPsu = relatedPsuObjects.FirstOrDefault(y => y[this.PowerSupplyClass.PropertyCollection["UUID"]].Value.ToString() == x.UUID);
                        if (oldPsu == null)
                        {
                            var newpsu = this.CreatePowerSupply(x);
                            newpsu[this.PartGroupKey].Value = psuGroup[this.PartGroupKey].Value;
                            newpsu[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newpsu);
                        }
                        else
                        {
                            this.UpdatePowerSupply(x, oldPsu);
                            discoveryData.Add(oldPsu);
                        }
                    });

                #endregion

                #region Disk

                var diskGroup = oldServer.GetRelatedMonitoringObjects(this.PhysicalDiskGroupClass).First();
                discoveryData.Add(diskGroup);

                var relatedDiskObjects = diskGroup.GetRelatedMonitoringObjects(this.PhysicalDiskClass);
                var deleteDisk = relatedDiskObjects.Where(
                    x => model.DiskList.All(
                        y => y.UUID != x[this.PhysicalDiskClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deleteDisk.ForEach(x => { discoveryData.Remove(x); });
                model.DiskList.ForEach(
                    y =>
                    {
                        var oldDisk = relatedDiskObjects.FirstOrDefault(z => z[this.PhysicalDiskClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldDisk == null)
                        {
                            var newDisk = this.CreateDisk(y);
                            newDisk[this.HuaweiServerKey].Value = model.DeviceId;
                            newDisk[this.PartGroupKey].Value = diskGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newDisk);
                        }
                        else
                        {
                            this.UpdateDisk(y, oldDisk);
                            discoveryData.Add(oldDisk);
                        }
                    });

                #endregion

                #region Raid

                var raidGroup = oldServer.GetRelatedMonitoringObjects(this.RaidGroupClass).First();
                discoveryData.Add(raidGroup);

                var relatedRaidObjects = raidGroup.GetRelatedMonitoringObjects(this.RaidClass);
                var deleteRaid = relatedRaidObjects.Where(
                        x => model.RaidList.All(
                            y => y.UUID != x[this.RaidClass.PropertyCollection["UUID"]].Value.ToString()))
                    .ToList();
                deleteRaid.ForEach(x => { discoveryData.Remove(x); });
                model.RaidList.ForEach(
                    y =>
                    {
                        var oldRaid = relatedRaidObjects.FirstOrDefault(z => z[this.RaidClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldRaid == null)
                        {
                            var newRaid = this.CreateRaidControl(y);
                            newRaid[this.HuaweiServerKey].Value = model.DeviceId;
                            newRaid[this.PartGroupKey].Value = raidGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newRaid);
                        }
                        else
                        {
                            this.UpdateRaidControl(y, oldRaid);
                            discoveryData.Add(oldRaid);
                        }
                    });

                #endregion

                discoveryData.Overwrite(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update KunLun Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// The exsits kun lun server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool ExsitsKunLunServer(string deviceId)
        {
            return this.ExsitsDeviceId(deviceId, this.KunLunClass);
        }

        /// <summary>
        /// The get kun lun server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetKunLunServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.KunLunClass);
        }
        #endregion

        #region Event

        /// <summary>
        /// The insert event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertEvent(EventData eventData, string eSightIp)
        {
            this.InsertEvent(this.KunLunClass, eventData, eSightIp);
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertDeviceChangeEvent(DeviceChangeEventData deviceChangeEventData, string eSightIp)
        {
            this.InsertDeviceChangeEvent(this.KunLunClass, deviceChangeEventData, eSightIp);
        }
        #endregion

        #region Remove

        /// <summary>
        /// Removes the e sight kunLun.
        /// </summary>
        /// <param name="eSightIp">
        /// The e sight ip.
        /// </param>
        public void RemoveServerFromMGroup(string eSightIp)
        {
            this.RemoverServersByESight(this.KunLunClass, eSightIp);
        }

        /// <summary>
        /// The remover all kun lun.
        /// </summary>
        public void RemoverAllKunLun()
        {
            this.RemoverServers(this.KunLunClass);
        }

        /// <summary>
        /// Deletes the kun lun on synchronize.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="newDeviceIds">The new device ids.</param>
        public void RemoveKunLunOnSync(string eSightIp, List<string> newDeviceIds)
        {
            this.RemoveServersOnSync(eSightIp, newDeviceIds, this.KunLunClass);
        }

        #endregion

        #region  Create Methods

        /// <summary>
        /// The create kun lun server.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateKunLunServer(KunLunServer model)
        {
            var propertys = this.KunLunClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.KunLunClass); // 实例化一个class
            obj[this.HuaweiServerKey].Value = model.DeviceId;
            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["Vendor"]].Value = "HUAWEI";
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["IPAddress"]].Value = model.IpAddress;
            obj[propertys["iBMCVersion"]].Value = model.Version;
            obj[propertys["CPLDVersion"]].Value = string.Empty;
            obj[propertys["UbootVersion"]].Value = string.Empty;
            obj[propertys["ProductSn"]].Value = model.ProductSN;
            obj[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
            obj[propertys["CPUNums"]].Value = model.CPUNums;
            obj[propertys["CPUCores"]].Value = model.CPUCores;
            obj[propertys["ServerName"]].Value = model.ServerName;
            obj[propertys["BMCMacAddr"]].Value = model.BmcMacAddr;

            obj[propertys["eSight"]].Value = model.ESight;
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }


        /// <summary>
        /// Creates the disk.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateDisk(HWDisk model)
        {
            var propertys = this.PhysicalDiskClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.PhysicalDiskClass); // 实例化一个class
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["Locator"]].Value = model.Location;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Diskcapacity"]].Value = string.Empty;
            obj[propertys["IndterfaceType"]].Value = string.Empty;
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// Creates the raid control.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateFan(HWFAN model)
        {
            var propertys = this.FanClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.FanClass); // 实例化一个class
            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Speed"]].Value = model.Rotate;
            obj[propertys["RotatePercent"]].Value = model.RotatePercent;
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// Creates the power supply.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// MPObject.
        /// </returns>
        private MPObject CreatePowerSupply(HWPSU model)
        {
            var propertys = this.PowerSupplyClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.PowerSupplyClass); // 实例化一个class
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["InputMode"]].Value = model.InputMode;
            obj[propertys["Model"]].Value = model.Model;
            obj[propertys["PowerRating"]].Value = model.RatePower;
            obj[propertys["InputPower"]].Value = model.InputPower;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// Creates the raid control.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateRaidControl(HWRAID model)
        {
            var propertys = this.RaidClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.RaidClass); // 实例化一个class
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["Type"]].Value = model.RaidType;
            obj[propertys["DeviceInterface"]].Value = model.InterfaceType;
            obj[propertys["BBUType"]].Value = model.BbuType;
            obj[propertys["FirmwareVersion"]].Value = string.Empty;
            obj[propertys["DirverVersion"]].Value = string.Empty;
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Updates the disk.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateDisk(HWDisk model, MonitoringObject oldObject)
        {
            var propertys = this.PhysicalDiskClass.PropertyCollection; // 获取到class的属性
            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["Locator"]].Value = model.Location;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Diskcapacity"]].Value = string.Empty;
            oldObject[propertys["IndterfaceType"]].Value = string.Empty;
            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the fan.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>MPObject.</returns>
        private void UpdateFan(HWFAN model, MonitoringObject oldObject)
        {
            var propertys = this.FanClass.PropertyCollection; // 获取到class的属性
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Speed"]].Value = model.Rotate;
            oldObject[propertys["RotatePercent"]].Value = model.RotatePercent;
            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the power supply.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>MPObject.</returns>
        private void UpdatePowerSupply(HWPSU model, MonitoringObject oldObject)
        {
            var propertys = this.PowerSupplyClass.PropertyCollection; // 获取到class的属性
            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["InputMode"]].Value = model.InputMode;
            oldObject[propertys["Model"]].Value = model.Model;
            oldObject[propertys["PowerRating"]].Value = model.RatePower;
            oldObject[propertys["InputPower"]].Value = model.InputPower;

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the raid control.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateRaidControl(HWRAID model, MonitoringObject oldObject)
        {
            var propertys = this.RaidClass.PropertyCollection; // 获取到class的属性
            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["Type"]].Value = model.RaidType;
            oldObject[propertys["DeviceInterface"]].Value = model.InterfaceType;
            oldObject[propertys["BBUType"]].Value = model.BbuType;
            oldObject[propertys["FirmwareVersion"]].Value = string.Empty;
            oldObject[propertys["DirverVersion"]].Value = string.Empty;
            oldObject[this.DisplayNameField].Value = model.Name;
        }
        #endregion
    }
}