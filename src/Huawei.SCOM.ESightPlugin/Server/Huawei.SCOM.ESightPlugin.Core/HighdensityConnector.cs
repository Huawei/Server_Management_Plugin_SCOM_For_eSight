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
// Created          : 11-20-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="HighdensityConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The highdensity connector.</summary>
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
    /// The highdensity connector.
    /// </summary>
    public class HighdensityConnector : BaseConnector
    {
        #region Fields

        /// <summary>
        /// The connector guid.
        /// </summary>
        private static Guid connectorGuid = new Guid("{528C8486-2E62-42FB-9AFB-96CB8C089863}");

        /// <summary>
        /// The instance.
        /// </summary>
        private static HighdensityConnector instance;

        /// <summary>
        /// The child highdensity class.
        /// </summary>
        private ManagementPackClass childHighdensityClass;

        /// <summary>
        /// The child highdensity group class.
        /// </summary>
        private ManagementPackClass childHighdensityGroupClass;

        /// <summary>
        /// The cpu class.
        /// </summary>
        private ManagementPackClass cpuClass;

        /// <summary>
        /// The cpu group class.
        /// </summary>
        private ManagementPackClass cpuGroupClass;

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
        /// The highdensity class.
        /// </summary>
        private ManagementPackClass highdensityClass;

        /// <summary>
        /// The memory class.
        /// </summary>
        private ManagementPackClass memoryClass;

        /// <summary>
        /// The memory group class.
        /// </summary>
        private ManagementPackClass memoryGroupClass;

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
        public static HighdensityConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HighdensityConnector
                    {
                        ConnectorName = "HighdensityServer.Connector",
                        ConnectorGuid = connectorGuid,
                        ConnectorInfo = new ConnectorInfo
                        {
                            Description = "HighdensityServer Connector Description",
                            DisplayName = "HighdensityServer Connector",
                            Name = "HighdensityServer.Connector",
                            DiscoveryDataIsManaged = true
                        }
                    };
                    instance.Install();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the child highdensity class.
        /// </summary>
        public ManagementPackClass ChildHighdensityClass => this.childHighdensityClass ?? (this.childHighdensityClass =
                                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                                   EntityTypeConst.HighdensityServer.Highdensity
                                                                                                       .HighdensityName));

        /// <summary>
        /// Gets the child highdensity group class.
        /// </summary>
        public ManagementPackClass ChildHighdensityGroupClass => this.childHighdensityGroupClass ?? (this.childHighdensityGroupClass =
                                                                                                         MGroup.Instance.GetManagementPackClass(
                                                                                                             EntityTypeConst.HighdensityServer.Highdensity
                                                                                                                 .MainGroup));

        /// <summary>
        /// Gets the cpu class.
        /// </summary>
        public ManagementPackClass CpuClass => this.cpuClass ?? (this.cpuClass =
                                                                     MGroup.Instance.GetManagementPackClass(
                                                                         EntityTypeConst.HighdensityServer.Highdensity.Cpu));

        /// <summary>
        /// Gets the cpu group class.
        /// </summary>
        public ManagementPackClass CpuGroupClass => this.cpuGroupClass ?? (this.cpuGroupClass =
                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                   EntityTypeConst.HighdensityServer.Highdensity.CpuGroup));

        /// <summary>
        /// Gets the disk class.
        /// </summary>
        public ManagementPackClass DiskClass => this.diskClass ?? (this.diskClass =
                                                                       MGroup.Instance.GetManagementPackClass(
                                                                           EntityTypeConst.HighdensityServer.Highdensity.Disk));

        /// <summary>
        /// Gets the disk group class.
        /// </summary>
        public ManagementPackClass DiskGroupClass => this.diskGroupClass ?? (this.diskGroupClass =
                                                                                 MGroup.Instance.GetManagementPackClass(
                                                                                     EntityTypeConst.HighdensityServer.Highdensity.DiskGroup));

        /// <summary>
        /// Gets the fan class.
        /// </summary>
        public ManagementPackClass FanClass => this.fanClass ?? (this.fanClass =
                                                                     MGroup.Instance.GetManagementPackClass(
                                                                         EntityTypeConst.HighdensityServer.Fan));

        /// <summary>
        /// Gets the fan group class.
        /// </summary>
        public ManagementPackClass FanGroupClass => this.fanGroupClass ?? (this.fanGroupClass =
                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                   EntityTypeConst.HighdensityServer.FanGroup));

        /// <summary>
        /// Gets the highdensity class.
        /// </summary>
        public ManagementPackClass HighdensityClass => this.highdensityClass ?? (this.highdensityClass =
                                                                                     MGroup.Instance.GetManagementPackClass(
                                                                                         EntityTypeConst.HighdensityServer.MainName));

        /// <summary>
        /// Gets the memory class.
        /// </summary>
        public ManagementPackClass MemoryClass => this.memoryClass ?? (this.memoryClass =
                                                                           MGroup.Instance.GetManagementPackClass(
                                                                               EntityTypeConst.HighdensityServer.Highdensity.Memory));

        /// <summary>
        /// Gets the memory group class.
        /// </summary>
        public ManagementPackClass MemoryGroupClass => this.memoryGroupClass ?? (this.memoryGroupClass =
                                                                                     MGroup.Instance.GetManagementPackClass(
                                                                                         EntityTypeConst.HighdensityServer.Highdensity.MemoryGroup));

        /// <summary>
        /// Gets the power supply class.
        /// </summary>
        public ManagementPackClass PowerSupplyClass => this.powerSupplyClass ?? (this.powerSupplyClass =
                                                                                     MGroup.Instance.GetManagementPackClass(
                                                                                         EntityTypeConst.HighdensityServer.PowerSupply));

        /// <summary>
        /// Gets the power supply group class.
        /// </summary>
        public ManagementPackClass PowerSupplyGroupClass => this.powerSupplyGroupClass ?? (this.powerSupplyGroupClass =
                                                                                               MGroup.Instance.GetManagementPackClass(
                                                                                                   EntityTypeConst.HighdensityServer.PowerSupplyGroup));

        /// <summary>
        /// Gets the raid class.
        /// </summary>
        public ManagementPackClass RaidClass => this.raidClass ?? (this.raidClass =
                                                                       MGroup.Instance.GetManagementPackClass(
                                                                           EntityTypeConst.HighdensityServer.Highdensity.RaidController));

        /// <summary>
        /// Gets the raid group class.
        /// </summary>
        public ManagementPackClass RaidGroupClass => this.raidGroupClass ?? (this.raidGroupClass =
                                                                                 MGroup.Instance.GetManagementPackClass(
                                                                                     EntityTypeConst.HighdensityServer.Highdensity
                                                                                         .RaidControllerGroup));
        #endregion

        #region Public Methods

        /// <summary>
        /// Synchronizes the server.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SyncServer(HighdensityServer model)
        {
            // 存在则更新
            if (ExsitsHighdensityServer(model.DeviceId))
            {
                this.UpdateMain(model, true);
            }
            else
            {
                this.InsertDetials(model);
            }
        }

        /// <summary>
        /// The insert detials.
        /// </summary>
        /// <param name="model">The model.</param>
        private void InsertDetials(HighdensityServer model)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Insert Highdensity:{model.DN}");
                var discoveryData = new IncrementalDiscoveryData();

                #region HighdensityServer

                var highdensityServer = this.CreateHighdensityServer(model);
                highdensityServer[this.HuaweiServerKey].Value = model.DeviceId;

                discoveryData.Add(highdensityServer);

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

                var psuGroup = this.CreateLogicalGroup(this.PowerSupplyGroupClass, model.DeviceId);
                discoveryData.Add(psuGroup);
                model.PowerSupplyList.ForEach(
                    x =>
                    {
                        var powerSupply = this.CreatePowerSupply(x);
                        powerSupply[this.PartGroupKey].Value = psuGroup[this.PartGroupKey].Value;

                        powerSupply[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(powerSupply);
                    });

                #endregion

                #region Child Highdensity

                var childHighdensityGroup = this.CreateLogicalGroup(this.ChildHighdensityGroupClass, model.DeviceId);
                var childHighdensityGroupKey = childHighdensityGroup[this.PartGroupKey].Value.ToString();
                discoveryData.Add(childHighdensityGroup);
                model.ChildHighdensitys.ForEach(
                    x =>
                    {
                        var childHighdensity = this.CreateChildHighdensity(x, model.ServerName);
                        childHighdensity[this.PartGroupKey].Value = childHighdensityGroupKey;

                        childHighdensity[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(childHighdensity);
                        var childHighdensityKey = this.ChildHighdensityClass.PropertyCollection["DN"];

                        #region CPU

                        var cpuGroup = this.CreateLogicalChildGroup(this.CpuGroupClass, model.DeviceId, x.DeviceId);

                        cpuGroup[childHighdensityKey].Value = x.DeviceId;
                        cpuGroup[this.PartGroupKey].Value = childHighdensityGroupKey;
                        discoveryData.Add(cpuGroup);
                        x.CPUList.ForEach(
                        y =>
                                {
                                    var cpu = this.CreateCpu(y);
                                    cpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                                    cpu[childHighdensityKey].Value = x.DeviceId;
                                    cpu[this.PartGroupKey].Value = childHighdensityGroupKey;

                                    cpu[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(cpu);
                                });

                        #endregion

                        #region Memory

                        var memoryGroup = this.CreateLogicalChildGroup(this.MemoryGroupClass, model.DeviceId, x.DeviceId);
                        memoryGroup[childHighdensityKey].Value = x.DeviceId;
                        memoryGroup[this.PartGroupKey].Value = childHighdensityGroupKey;
                        discoveryData.Add(memoryGroup);
                        x.MemoryList.ForEach(
                        y =>
                                {
                                    var memory = this.CreateMemory(y);
                                    memory[this.PartChildGroupKey].Value =
                                    memoryGroup[this.PartChildGroupKey].Value;
                                    memory[childHighdensityKey].Value = x.DeviceId;
                                    memory[this.PartGroupKey].Value = childHighdensityGroupKey;

                                    memory[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(memory);
                                });

                        #endregion

                        #region Disk

                        var diskGroup = this.CreateLogicalChildGroup(this.DiskGroupClass, model.DeviceId, x.DeviceId);
                        diskGroup[this.PartGroupKey].Value = childHighdensityGroupKey;
                        diskGroup[childHighdensityKey].Value = x.DeviceId;
                        discoveryData.Add(diskGroup);
                        x.DiskList.ForEach(
                        y =>
                                {
                                    var disk = this.CreateDisk(y);
                                    disk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                                    disk[childHighdensityKey].Value = x.DeviceId;
                                    disk[this.PartGroupKey].Value = childHighdensityGroupKey;

                                    disk[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(disk);
                                });

                        #endregion

                        #region Raid

                        var raidGroup = this.CreateLogicalChildGroup(this.RaidGroupClass, model.DeviceId, x.DeviceId);
                        raidGroup[this.PartGroupKey].Value = childHighdensityGroupKey;
                        raidGroup[childHighdensityKey].Value = x.DeviceId;
                        discoveryData.Add(raidGroup);
                        x.RaidList.ForEach(
                        y =>
                                {
                                    var raid = this.CreateRaidControl(y);
                                    raid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                                    raid[childHighdensityKey].Value = x.DeviceId;
                                    raid[this.PartGroupKey].Value = childHighdensityGroupKey;
                                    raid[this.HuaweiServerKey].Value = model.DeviceId;

                                    discoveryData.Add(raid);
                                });

                        #endregion
                    });

                #endregion

                discoveryData.Commit(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Insert Highdensity Error:{model.DN}", e);
            }

        }

        /// <summary>
        /// 更新高密管理板，及子刀片
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">是否是轮询</param>
        /// <exception cref="System.Exception"></exception>
        public void UpdateMain(HighdensityServer model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start UpdateHighdensity.[{model.DN}] [isPolling:{isPolling}]");
                var oldBlade = this.GetHighdensityServer(model.DeviceId);
                if (oldBlade == null)
                {
                    throw new Exception($"Can not find the server:{model.DN}");
                }
                var propertys = this.HighdensityClass.PropertyCollection; // 获取到class的属性
                var discoveryData = new IncrementalDiscoveryData();

                oldBlade[propertys["eSight"]].Value = model.ESight;
                if (model.Status != "-3")
                {
                    oldBlade[propertys["Status"]].Value = model.StatusTxt;
                }
                oldBlade[propertys["Vendor"]].Value = "HUAWEI";
                oldBlade[propertys["Manufacturer"]].Value = model.Manufacturer;
                oldBlade[propertys["IPAddress"]].Value = model.IpAddress;
                if (isPolling)
                {
                    oldBlade[propertys["UUID"]].Value = model.UUID;
                    oldBlade[propertys["ProductSn"]].Value = model.ProductSN;
                    oldBlade[propertys["iBMCVersion"]].Value = model.Version;
                }

                // oldBlade[propertys["ServerName"]].Value = model.ServerName;
                oldBlade[this.DisplayNameField].Value = model.ServerName;
                discoveryData.Add(oldBlade);

                var fanGroup = oldBlade.GetRelatedMonitoringObjects(this.FanGroupClass).First();
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

                var psuGroup = oldBlade.GetRelatedMonitoringObjects(this.PowerSupplyGroupClass).First();
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

                if (isPolling)
                {
                    #region ChildBlade

                    var childBladeGroup = oldBlade.GetRelatedMonitoringObjects(this.ChildHighdensityGroupClass).First();
                    var childBladeGroupKey = childBladeGroup[this.PartGroupKey].Value.ToString();

                    discoveryData.Add(childBladeGroup);

                    var relatedChildBladeObjects = childBladeGroup.GetRelatedMonitoringObjects(this.ChildHighdensityClass);
                    var deleteChildBlade = relatedChildBladeObjects.Where(
                            x => model.ChildHighdensitys.All(y =>
                                y.DeviceId != x[this.ChildHighdensityClass.PropertyCollection["DN"]].Value.ToString()))
                        .ToList();
                    deleteChildBlade.ForEach(x => { discoveryData.Remove(x); });
                    if (deleteChildBlade.Count > 0)
                    {
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"new child boards:{string.Join(",", model.ChildHighdensitys.Select(x => x.DeviceId))}");
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"old child boards:{string.Join(",", relatedChildBladeObjects.Select(x => x[this.ChildHighdensityClass.PropertyCollection["DN"]].Value.ToString()))}");
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"remove child board:{deleteChildBlade.Count}");
                    }

                    model.ChildHighdensitys.ForEach(
                        x =>
                        {
                            var oldChildServer = relatedChildBladeObjects.FirstOrDefault(y => y[this.ChildHighdensityClass.PropertyCollection["DN"]].Value.ToString() == x.DeviceId);
                            if (oldChildServer == null)
                            {
                                var newChildBlade = this.CreateChildHighdensity(x, model.ServerName);
                                newChildBlade[this.PartGroupKey].Value = childBladeGroup[this.PartGroupKey].Value;

                                newChildBlade[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(newChildBlade);
                                var childHighdensityKey = this.ChildHighdensityClass.PropertyCollection["DN"];

                                #region CPU

                                var cpuGroup = this.CreateLogicalChildGroup(this.CpuGroupClass, model.DeviceId, x.DeviceId);

                                cpuGroup[childHighdensityKey].Value = x.DeviceId;
                                cpuGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                discoveryData.Add(cpuGroup);
                                x.CPUList.ForEach(
                                    y =>
                                    {
                                        var cpu = this.CreateCpu(y);
                                        cpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                                        cpu[childHighdensityKey].Value = x.DeviceId;
                                        cpu[this.PartGroupKey].Value = childBladeGroupKey;

                                        cpu[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(cpu);
                                    });

                                #endregion

                                #region Memory

                                var memoryGroup =
                                    this.CreateLogicalChildGroup(this.MemoryGroupClass, model.DeviceId, x.DeviceId);
                                memoryGroup[childHighdensityKey].Value = x.DeviceId;
                                memoryGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                discoveryData.Add(memoryGroup);
                                x.MemoryList.ForEach(
                                    y =>
                                    {
                                        var memory = this.CreateMemory(y);
                                        memory[this.PartChildGroupKey].Value = memoryGroup[this.PartChildGroupKey].Value;
                                        memory[childHighdensityKey].Value = x.DeviceId;
                                        memory[this.PartGroupKey].Value = childBladeGroupKey;

                                        memory[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(memory);
                                    });

                                #endregion

                                #region Disk

                                var diskGroup =
                                    this.CreateLogicalChildGroup(this.DiskGroupClass, model.DeviceId, x.DeviceId);
                                diskGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                diskGroup[childHighdensityKey].Value = x.DeviceId;
                                discoveryData.Add(diskGroup);
                                x.DiskList.ForEach(
                                    y =>
                                    {
                                        var disk = this.CreateDisk(y);
                                        disk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                                        disk[childHighdensityKey].Value = x.DeviceId;
                                        disk[this.PartGroupKey].Value = childBladeGroupKey;

                                        disk[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(disk);
                                    });

                                #endregion

                                #region Raid

                                var raidGroup =
                                    this.CreateLogicalChildGroup(this.RaidGroupClass, model.DeviceId, x.DeviceId);
                                raidGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                raidGroup[childHighdensityKey].Value = x.DeviceId;
                                discoveryData.Add(raidGroup);
                                x.RaidList.ForEach(
                                    y =>
                                    {
                                        var raid = this.CreateRaidControl(y);
                                        raid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                                        raid[childHighdensityKey].Value = x.DeviceId;
                                        raid[this.PartGroupKey].Value = childBladeGroupKey;
                                        raid[this.HuaweiServerKey].Value = model.DeviceId;

                                        discoveryData.Add(raid);
                                    });

                                #endregion
                            }
                            else
                            {
                                this.UpdateChildBoard(x, true);
                            }
                        });

                    #endregion
                }

                // var relatedObjects = oldBlade.GetRelatedMonitoringObjects(ChildHighdensityClass);
                // relatedObjects.ToList().ForEach(x => discoveryData.Add(x));
                discoveryData.Overwrite(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update Highdensity Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// The update child blade.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">if set to <c>true</c> [is polling].</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="Exception">Can not find the child blade server</exception>
        public void UpdateChildBoard(ChildHighdensity model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start UpdateChildBoard. [{model.DN}] [isPolling:{isPolling}]");
                var oldObject = this.GetObject($"DN = '{model.DeviceId}'", this.ChildHighdensityClass);
                if (oldObject == null)
                {
                    throw new Exception($"Can not find the child blade server:{model.DN}");
                }
                var propertys = this.ChildHighdensityClass.PropertyCollection; // 获取到class的属性

                var discoveryData = new IncrementalDiscoveryData();

                var childServerKey = this.ChildHighdensityClass.PropertyCollection["DN"];

                oldObject[propertys["eSight"]].Value = model.ESight;

                if (model.Status != "-3")
                {
                    oldObject[propertys["Status"]].Value = model.StatusTxt;
                }
                oldObject[propertys["IPAddress"]].Value = model.IpAddress;
                oldObject[propertys["UUID"]].Value = model.UUID;
                oldObject[propertys["ProductSn"]].Value = model.ProductSn;
                oldObject[propertys["Type"]].Value = model.Type;
                var parent = this.GetParentServer(oldObject);
                if (parent != null)
                {
                    oldObject[this.DisplayNameField].Value = $"{parent.DisplayName}-{model.Name}";
                }
                discoveryData.Add(oldObject);

                #region CPU
                var cpuGroup = oldObject.GetRelatedMonitoringObjects(this.CpuGroupClass).First();
                discoveryData.Add(cpuGroup);

                var relatedCpuObjects = cpuGroup.GetRelatedMonitoringObjects(this.CpuClass);
                var deleteCpu = relatedCpuObjects.Where(
                        x => model.CPUList.All(y => y.UUID != x[this.CpuClass.PropertyCollection["UUID"]].Value.ToString()))
                    .ToList();
                deleteCpu.ForEach(x => { discoveryData.Remove(x); });
                model.CPUList.ForEach(
                    y =>
                    {
                        var oldCpu = relatedCpuObjects.FirstOrDefault(z => z[this.CpuClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldCpu == null)
                        {
                            var newCpu = this.CreateCpu(y);
                            newCpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                            newCpu[childServerKey].Value = model.DeviceId;
                            newCpu[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newCpu[this.HuaweiServerKey].Value = oldObject[this.HuaweiServerKey].Value;
                            discoveryData.Add(newCpu);
                        }
                        else
                        {
                            this.UpdateCpu(y, oldCpu);
                            discoveryData.Add(oldCpu);
                        }
                    });

                #endregion

                #region Memory

                var memoryGroup = oldObject.GetRelatedMonitoringObjects(this.MemoryGroupClass).First();
                discoveryData.Add(memoryGroup);

                var relatedMemoryObjects = memoryGroup.GetRelatedMonitoringObjects(this.MemoryClass);
                var deleteMemory = relatedMemoryObjects.Where(
                    x => model.MemoryList.All(
                        y => y.UUID != x[this.MemoryClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deleteMemory.ForEach(x => { discoveryData.Remove(x); });
                model.MemoryList.ForEach(
                    y =>
                    {
                        var oldMemory = relatedMemoryObjects.FirstOrDefault(z => z[this.MemoryClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldMemory == null)
                        {
                            var newMemory = this.CreateMemory(y);
                            newMemory[this.PartChildGroupKey].Value = memoryGroup[this.PartChildGroupKey].Value;
                            newMemory[childServerKey].Value = model.DeviceId;
                            newMemory[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newMemory[this.HuaweiServerKey].Value = oldObject[this.HuaweiServerKey].Value;
                            discoveryData.Add(newMemory);
                        }
                        else
                        {
                            this.UpdateMemory(y, oldMemory);
                            discoveryData.Add(oldMemory);
                        }
                    });

                #endregion

                #region Disk

                var diskGroup = oldObject.GetRelatedMonitoringObjects(this.DiskGroupClass).First();
                discoveryData.Add(diskGroup);

                var relatedDiskObjects = diskGroup.GetRelatedMonitoringObjects(this.DiskClass);
                var deleteDisk = relatedDiskObjects.Where(
                        x => model.DiskList.All(
                            y => y.UUID != x[this.DiskClass.PropertyCollection["UUID"]].Value.ToString()))
                    .ToList();
                deleteDisk.ForEach(x => { discoveryData.Remove(x); });
                model.DiskList.ForEach(
                    y =>
                    {
                        var oldDisk = relatedDiskObjects.FirstOrDefault(z => z[this.DiskClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldDisk == null)
                        {
                            var newDisk = this.CreateDisk(y);
                            newDisk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                            newDisk[childServerKey].Value = model.DeviceId;
                            newDisk[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newDisk[this.HuaweiServerKey].Value = oldObject[this.HuaweiServerKey].Value;
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

                var raidGroup = oldObject.GetRelatedMonitoringObjects(this.RaidGroupClass).First();
                discoveryData.Add(raidGroup);

                var relatedRaidObjects = raidGroup.GetRelatedMonitoringObjects(this.RaidClass);
                var deleteRaid = relatedRaidObjects.Where(x => model.RaidList.All(y => y.UUID != x[this.RaidClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deleteRaid.ForEach(x => { discoveryData.Remove(x); });
                model.RaidList.ForEach(
                    y =>
                    {
                        var oldRaid = relatedRaidObjects.FirstOrDefault(z => z[this.RaidClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldRaid == null)
                        {
                            var newRaid = this.CreateRaidControl(y);
                            newRaid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                            newRaid[childServerKey].Value = model.DeviceId;
                            newRaid[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newRaid[this.HuaweiServerKey].Value = oldObject[this.HuaweiServerKey].Value;
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
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update ChildBoard Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// Gets the parent dn.
        /// </summary>
        /// <param name="childDeviceId">The child device identifier.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">e
        /// </exception>
        public string GetParentDn(string childDeviceId)
        {
            var oldObject = this.GetObject($"DN = '{childDeviceId}'", this.ChildHighdensityClass);
            if (oldObject == null)
            {
                throw new Exception($"Can not find the child blade server:{childDeviceId}");
            }
            var propertys = this.HighdensityClass.PropertyCollection; // 获取到class的属性

            var parent = this.GetFullParentServer(oldObject);
            if (parent == null)
            {
                throw new Exception($"Can not find the parent.the childServerDeviceId:{childDeviceId}");
            }
            var deviceId = parent[this.HuaweiServerKey].Value.ToString();
            var esight = parent[propertys["eSight"]].Value.ToString();
            return deviceId.Replace(esight + "-", string.Empty);
        }

        /// <summary>
        /// The exsits get highdensity server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool ExsitsHighdensityServer(string deviceId)
        {
            return this.ExsitsDeviceId(deviceId, this.HighdensityClass);
        }

        /// <summary>
        /// The get highdensity server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetHighdensityServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.HighdensityClass);
        }

        /// <summary>
        /// The get child blade server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetChildHighdensityServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.ChildHighdensityClass);
        }
        #endregion

        #region Remove
        /// <summary>
        /// The remove child high density server.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="deviceId">The device identifier.</param>
        public void RemoveChildHighDensityServer(string eSightIp, string deviceId)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Remove ChildBlade.[deviceId:{deviceId}]");
                var existingObject = this.GetObject($"DN = '{deviceId}'", this.ChildHighdensityClass);
                if (existingObject != null)
                {
                    var discovery = new IncrementalDiscoveryData();
                    discovery.Remove(existingObject);
                    discovery.Commit(this.MontioringConnector);
                }
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error($"Remove ChildBlade.[deviceId:{deviceId}]", e);
            }
        }

        /// <summary>
        /// Removes the e sight highdensity.
        /// </summary>
        /// <param name="eSightIp">
        /// The e sight ip.
        /// </param>
        public void RemoveServerFromMGroup(string eSightIp)
        {
            this.RemoverServersByESight(this.HighdensityClass, eSightIp);
        }

        /// <summary>
        /// The remover all highdensity.
        /// </summary>
        public void RemoverAllHighdensity()
        {
            this.RemoverServers(this.HighdensityClass);
        }

        /// <summary>
        /// Removes the high on synchronize.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="newDeviceIds">The new device ids.</param>
        public void RemoveHighOnSync(string eSightIp, List<string> newDeviceIds)
        {
            this.RemoveServersOnSync(eSightIp, newDeviceIds, this.HighdensityClass);
        }

        #endregion

        #region Event
        /// <summary>
        /// The insert event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="serverType">Type of the server.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertEvent(EventData eventData, ServerTypeEnum serverType, string eSightIp)
        {
            switch (serverType)
            {
                case ServerTypeEnum.Highdensity:
                    this.InsertEvent(this.HighdensityClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.ChildHighdensity:
                    this.InsertEvent(this.ChildHighdensityClass, eventData, eSightIp);
                    break;
            }
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="serverType">Type of the server.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertDeviceChangeEvent(DeviceChangeEventData eventData, ServerTypeEnum serverType, string eSightIp)
        {
            switch (serverType)
            {
                case ServerTypeEnum.Highdensity:
                    this.InsertDeviceChangeEvent(this.HighdensityClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.ChildHighdensity:
                    this.InsertDeviceChangeEvent(this.ChildHighdensityClass, eventData, eSightIp);
                    break;
            }
        }

        #endregion

        #region Create Methods

        /// <summary>
        /// The create highdensity server.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateHighdensityServer(HighdensityServer model)
        {
            var propertys = this.HighdensityClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.HighdensityClass); // 实例化一个class

            obj[propertys["eSight"]].Value = model.ESight;
            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["Vendor"]].Value = "HUAWEI";
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["IPAddress"]].Value = model.IpAddress;
            obj[propertys["iBMCVersion"]].Value = model.Version;
            obj[propertys["ProductSn"]].Value = model.ProductSN;
            obj[propertys["ServerName"]].Value = model.ServerName;

            var entityClass = MGroup.Instance.GetManagementPackClass("System.Entity");
            obj[entityClass.PropertyCollection["DisplayName"]].Value = model.ServerName;
            return obj;
        }

        /// <summary>
        /// Creates the child Highdensity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <returns>MPObject.</returns>
        private MPObject CreateChildHighdensity(ChildHighdensity model, string parentName)
        {
            var propertys = this.ChildHighdensityClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.ChildHighdensityClass); // 实例化一个class

            obj[propertys["DN"]].Value = model.DeviceId;
            obj[propertys["eSight"]].Value = model.ESight;
            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["IPAddress"]].Value = model.IpAddress;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["ProductSn"]].Value = model.ProductSn;
            obj[propertys["Type"]].Value = model.Type;

            obj[this.DisplayNameField].Value = $"{parentName}-{model.Name}";
            return obj;
        }

        /// <summary>
        /// Creates the cpu.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateCpu(HWCPU model)
        {
            var propertys = this.CpuClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.CpuClass); // 实例化一个class

            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Type"]].Value = model.Model;
            obj[propertys["Frequency"]].Value = model.Frequency;
            obj[propertys["CoreCount"]].Value = string.Empty;

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
            var propertys = this.DiskClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.DiskClass); // 实例化一个class

            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Locator"]].Value = model.Location;
            obj[propertys["Status"]].Value = model.HealthStateTxt;
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

            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["Status"]].Value = model.HealthStateTxt;

            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["RotatePercent"]].Value = model.GetRotatePercent("highdensity");
            obj[propertys["Speed"]].Value = model.Rotate;

            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// Creates the memory.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateMemory(HWMemory model)
        {
            var propertys = this.MemoryClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.MemoryClass); // 实例化一个class

            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["Size"]].Value = model.Capacity;
            obj[propertys["Frequency"]].Value = model.Frequency;

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

            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["InputMode"]].Value = model.InputMode;
            obj[propertys["Model"]].Value = model.Model;
            obj[propertys["PowerRating"]].Value = model.RatePower;
            obj[propertys["InputPower"]].Value = model.InputPower;

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
            obj[propertys["FirmwareVersion"]].Value = string.Empty;
            obj[propertys["DirverVersion"]].Value = string.Empty;
            obj[propertys["BBUType"]].Value = model.BbuType;

            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the cpu.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateCpu(HWCPU model, MonitoringObject oldObject)
        {
            var propertys = this.CpuClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Type"]].Value = model.Model;
            oldObject[propertys["Frequency"]].Value = model.Frequency;
            oldObject[propertys["CoreCount"]].Value = string.Empty;

            oldObject[this.DisplayNameField].Value = model.Name;

        }

        /// <summary>
        /// Updates the disk.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateDisk(HWDisk model, MonitoringObject oldObject)
        {
            var propertys = this.DiskClass.PropertyCollection; // 获取到class的属性

            // obj[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Locator"]].Value = model.Location;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
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

            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["RotatePercent"]].Value = model.GetRotatePercent("highdensity");
            oldObject[propertys["Speed"]].Value = model.Rotate;
            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the memory.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateMemory(HWMemory model, MonitoringObject oldObject)
        {
            var propertys = this.MemoryClass.PropertyCollection; // 获取到class的属性
            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["Size"]].Value = model.Capacity;
            oldObject[propertys["Frequency"]].Value = model.Frequency;
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

            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["InputMode"]].Value = model.InputMode;
            oldObject[propertys["Model"]].Value = model.Model;
            oldObject[propertys["PowerRating"]].Value = model.RatePower;
            oldObject[propertys["InputPower"]].Value = model.InputPower;

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

            // obj[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["Type"]].Value = model.RaidType;
            oldObject[propertys["DeviceInterface"]].Value = model.InterfaceType;
            oldObject[propertys["FirmwareVersion"]].Value = string.Empty;
            oldObject[propertys["DirverVersion"]].Value = string.Empty;
            oldObject[propertys["BBUType"]].Value = model.BbuType;
            oldObject[this.DisplayNameField].Value = model.Name;
        }
        #endregion
    }
}