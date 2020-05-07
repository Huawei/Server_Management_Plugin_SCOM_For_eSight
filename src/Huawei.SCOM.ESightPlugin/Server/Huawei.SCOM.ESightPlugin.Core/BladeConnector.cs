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
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="BladeConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Huawei.SCOM.ESightPlugin.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Huawei.SCOM.ESightPlugin.Core.Const;
    using Huawei.SCOM.ESightPlugin.Core.Models;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Devices;
    using Huawei.SCOM.ESightPlugin.Models.Server;

    using LogUtil;

    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Configuration;
    using Microsoft.EnterpriseManagement.ConnectorFramework;
    using Microsoft.EnterpriseManagement.Monitoring;
    using MPObject = Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject;

    /// <summary>
    /// Class BladeConnector.
    /// </summary>
    public class BladeConnector : BaseConnector
    {
        #region Fields

        /// <summary>
        /// The connector unique identifier
        /// </summary>
        private static Guid connectorGuid = new Guid("{528C8486-2E62-42FB-9AFB-96CB8C089861}");

        /// <summary>
        /// The instance.
        /// </summary>
        private static BladeConnector instance;

        /// <summary>
        ///     The blade class
        /// </summary>
        private ManagementPackClass bladeClass;

        /// <summary>
        ///     The child blade class
        /// </summary>
        private ManagementPackClass childBladeClass;

        /// <summary>
        /// The child blade group class.
        /// </summary>
        private ManagementPackClass childBladeGroupClass;

        /// <summary>
        ///     The cpu class
        /// </summary>
        private ManagementPackClass cpuClass;

        /// <summary>
        /// The cpu group class.
        /// </summary>
        private ManagementPackClass cpuGroupClass;

        /// <summary>
        ///     The disk class
        /// </summary>
        private ManagementPackClass diskClass;

        /// <summary>
        /// The disk group class.
        /// </summary>
        private ManagementPackClass diskGroupClass;

        /// <summary>
        ///     The fan class
        /// </summary>
        private ManagementPackClass fanClass;

        /// <summary>
        /// The fan group class.
        /// </summary>
        private ManagementPackClass fanGroupClass;

        /// <summary>
        ///     The HMM class
        /// </summary>
        private ManagementPackClass hmmClass;

        /// <summary>
        /// The hmm group class.
        /// </summary>
        private ManagementPackClass hmmGroupClass;

        /// <summary>
        ///     The memory class
        /// </summary>
        private ManagementPackClass memoryClass;

        /// <summary>
        /// The memory group class.
        /// </summary>
        private ManagementPackClass memoryGroupClass;

        /// <summary>
        ///     The mezz class
        /// </summary>
        private ManagementPackClass mezzClass;

        /// <summary>
        /// The mezz group class.
        /// </summary>
        private ManagementPackClass mezzGroupClass;

        /// <summary>
        ///     The power supply class
        /// </summary>
        private ManagementPackClass powerSupplyClass;

        /// <summary>
        /// The power supply group class.
        /// </summary>
        private ManagementPackClass powerSupplyGroupClass;

        /// <summary>
        ///     The raid class
        /// </summary>
        private ManagementPackClass raidClass;

        /// <summary>
        /// The raid group class.
        /// </summary>
        private ManagementPackClass raidGroupClass;

        /// <summary>
        ///     The switch class
        /// </summary>
        private ManagementPackClass switchClass;

        /// <summary>
        /// The switch group class.
        /// </summary>
        private ManagementPackClass switchGroupClass;
        #endregion

        #region Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static BladeConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BladeConnector
                    {
                        ConnectorName = "BladeServer.Connector",
                        ConnectorGuid = connectorGuid,
                        ConnectorInfo = new ConnectorInfo
                        {
                            Description = "BladeServer Connector Description",
                            DisplayName = "BladeServer Connector",
                            Name = "BladeServer.Connector",
                            DiscoveryDataIsManaged = true
                        }
                    };
                    instance.Install();
                }
                return instance;
            }
        }

        /// <summary>
        ///     The blade class
        /// </summary>
        public ManagementPackClass BladeClass => this.bladeClass
                                                 ?? (this.bladeClass =
                                                         MGroup.Instance.GetManagementPackClass(
                                                             EntityTypeConst.BladeServer.MainName));

        /// <summary>
        /// The child blade class.
        /// </summary>
        public ManagementPackClass ChildBladeClass => this.childBladeClass
                                                      ?? (this.childBladeClass =
                                                              MGroup.Instance.GetManagementPackClass(
                                                                  EntityTypeConst.BladeServer.Blade.BladeName));

        /// <summary>
        /// The child blade group class.
        /// </summary>
        public ManagementPackClass ChildBladeGroupClass => this.childBladeGroupClass
                                                           ?? (this.childBladeGroupClass =
                                                                   MGroup.Instance.GetManagementPackClass(
                                                                       EntityTypeConst.BladeServer.Blade.MainGroup));

        /// <summary>
        /// The cpu class.
        /// </summary>
        public ManagementPackClass CpuClass => this.cpuClass
                                               ?? (this.cpuClass =
                                                       MGroup.Instance.GetManagementPackClass(
                                                           EntityTypeConst.BladeServer.Blade.Cpu));

        /// <summary>
        /// The cpu group class.
        /// </summary>
        public ManagementPackClass CpuGroupClass => this.cpuGroupClass
                                                    ?? (this.cpuGroupClass =
                                                            MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.BladeServer.Blade.CpuGroup));

        /// <summary>
        /// The disk class.
        /// </summary>
        public ManagementPackClass DiskClass => this.diskClass
                                                ?? (this.diskClass =
                                                        MGroup.Instance.GetManagementPackClass(
                                                            EntityTypeConst.BladeServer.Blade.Disk));

        /// <summary>
        /// The disk group class.
        /// </summary>
        public ManagementPackClass DiskGroupClass => this.diskGroupClass
                                                     ?? (this.diskGroupClass =
                                                             MGroup.Instance.GetManagementPackClass(
                                                                 EntityTypeConst.BladeServer.Blade.DiskGroup));

        /// <summary>
        ///     Gets the fan class.
        /// </summary>
        /// <value>The fan class.</value>
        public ManagementPackClass FanClass => this.fanClass
                                               ?? (this.fanClass =
                                                       MGroup.Instance.GetManagementPackClass(
                                                           EntityTypeConst.BladeServer.Fan));

        /// <summary>
        /// The fan group class.
        /// </summary>
        public ManagementPackClass FanGroupClass => this.fanGroupClass
                                                    ?? (this.fanGroupClass =
                                                            MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.BladeServer.FanGroup));

        /// <summary>
        /// The hmm class.
        /// </summary>
        public ManagementPackClass HmmClass => this.hmmClass
                                               ?? (this.hmmClass =
                                                       MGroup.Instance.GetManagementPackClass(
                                                           EntityTypeConst.BladeServer.Hmm));

        /// <summary>
        /// The hmm group class.
        /// </summary>
        public ManagementPackClass HmmGroupClass => this.hmmGroupClass
                                                    ?? (this.hmmGroupClass =
                                                            MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.BladeServer.HmmGroup));

        /// <summary>
        /// The memory class.
        /// </summary>
        public ManagementPackClass MemoryClass => this.memoryClass
                                                  ?? (this.memoryClass =
                                                          MGroup.Instance.GetManagementPackClass(
                                                              EntityTypeConst.BladeServer.Blade.Memory));

        /// <summary>
        ///     Gets the memory group class.
        /// </summary>
        /// <value>The memory group class.</value>
        public ManagementPackClass MemoryGroupClass => this.memoryGroupClass
                                                       ?? (this.memoryGroupClass =
                                                               MGroup.Instance.GetManagementPackClass(
                                                                   EntityTypeConst.BladeServer.Blade.MemoryGroup));

        /// <summary>
        /// The mezz class.
        /// </summary>
        public ManagementPackClass MezzClass => this.mezzClass
                                                ?? (this.mezzClass =
                                                        MGroup.Instance.GetManagementPackClass(
                                                            EntityTypeConst.BladeServer.Blade.Mezz));

        /// <summary>
        /// The mezz group class.
        /// </summary>
        public ManagementPackClass MezzGroupClass => this.mezzGroupClass
                                                     ?? (this.mezzGroupClass =
                                                             MGroup.Instance.GetManagementPackClass(
                                                                 EntityTypeConst.BladeServer.Blade.MezzGroup));

        /// <summary>
        /// The power supply class.
        /// </summary>
        public ManagementPackClass PowerSupplyClass => this.powerSupplyClass
                                                       ?? (this.powerSupplyClass =
                                                               MGroup.Instance.GetManagementPackClass(
                                                                   EntityTypeConst.BladeServer.PowerSupply));

        /// <summary>
        /// The power supply group class.
        /// </summary>
        public ManagementPackClass PowerSupplyGroupClass => this.powerSupplyGroupClass
                                                            ?? (this.powerSupplyGroupClass =
                                                                    MGroup.Instance.GetManagementPackClass(
                                                                        EntityTypeConst.BladeServer.PowerSupplyGroup));

        /// <summary>
        /// The raid class.
        /// </summary>
        public ManagementPackClass RaidClass => this.raidClass
                                                ?? (this.raidClass =
                                                        MGroup.Instance.GetManagementPackClass(
                                                            EntityTypeConst.BladeServer.Blade.RaidController));

        /// <summary>
        /// The raid group class.
        /// </summary>
        public ManagementPackClass RaidGroupClass => this.raidGroupClass
                                                     ?? (this.raidGroupClass =
                                                             MGroup.Instance.GetManagementPackClass(
                                                                 EntityTypeConst.BladeServer.Blade
                                                                     .RaidControllerGroup));

        /// <summary>
        ///     Gets the switch class.
        /// </summary>
        /// <value>The switch class.</value>
        public ManagementPackClass SwitchClass => this.switchClass
                                                  ?? (this.switchClass =
                                                          MGroup.Instance.GetManagementPackClass(
                                                              EntityTypeConst.BladeServer.Switch));

        /// <summary>
        /// The switch group class.
        /// </summary>
        public ManagementPackClass SwitchGroupClass => this.switchGroupClass
                                                       ?? (this.switchGroupClass =
                                                               MGroup.Instance.GetManagementPackClass(
                                                                   EntityTypeConst.BladeServer.SwitchGroup));
        #endregion

        #region Public Methods

        /// <summary>
        /// Synchronizes the server.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SyncServer(BladeServer model)
        {
            // 存在则更新
            if (ExsitsBladeServer(model.DeviceId))
            {
                this.UpdateBlade(model, true);
            }
            else
            {
                this.InsertDetails(model);
            }
        }

        /// <summary>
        /// The insert detials.
        /// </summary>
        /// <param name="model">The model.</param>
        private void InsertDetails(BladeServer model)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Insert Blade:{model.DN}");
                var discoveryData = new IncrementalDiscoveryData();

                #region BladeServer

                var bladeServer = this.CreateBladeServer(model);
                bladeServer[this.HuaweiServerKey].Value = model.DeviceId;

                discoveryData.Add(bladeServer);

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

                #region Switch

                var switchGroup = this.CreateLogicalGroup(this.SwitchGroupClass, model.DeviceId);
                discoveryData.Add(switchGroup);
                model.SwitchList.ForEach(
                    x =>
                    {
                        var switchObject = this.CreateSwitch(x, model.ServerName);
                        switchObject[this.PartGroupKey].Value = switchGroup[this.PartGroupKey].Value;

                        switchObject[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(switchObject);
                    });

                #endregion

                #region HMM

                var hmmGroup = this.CreateLogicalGroup(this.HmmGroupClass, model.DeviceId);
                discoveryData.Add(hmmGroup);

                var hmm = this.CreateHmm(model.HmmInfo);
                hmm[this.PartGroupKey].Value = hmmGroup[this.PartGroupKey].Value;

                hmm[this.HuaweiServerKey].Value = model.DeviceId;
                discoveryData.Add(hmm);

                #endregion

                #region Child Blade

                var childBladeGroup =
                    this.CreateLogicalGroup(this.ChildBladeGroupClass, model.DeviceId);
                var childBladeGroupKey = childBladeGroup[this.PartGroupKey].Value.ToString();
                discoveryData.Add(childBladeGroup);

                model.ChildBlades.ForEach(
                    x =>
                    {
                        var childBlade = this.CreateChildBlade(x, model.ServerName);
                        childBlade[this.PartGroupKey].Value = childBladeGroupKey;

                        childBlade[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(childBlade);

                        var childBladeKey = this.ChildBladeClass.PropertyCollection["DN"];

                        #region CPU

                        var cpuGroup =
                            this.CreateLogicalChildGroup(this.CpuGroupClass, model.DeviceId, x.DeviceId);

                        cpuGroup[childBladeKey].Value = x.DeviceId;
                        cpuGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        discoveryData.Add(cpuGroup);

                        x.CPUList.ForEach(
                            y =>
                            {
                                var cpu = this.CreateCpu(y);
                                cpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                                cpu[childBladeKey].Value = x.DeviceId;
                                cpu[this.PartGroupKey].Value = childBladeGroupKey;

                                cpu[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(cpu);
                            });

                        #endregion

                        #region Memory

                        var memoryGroup = this.CreateLogicalChildGroup(this.MemoryGroupClass, model.DeviceId, x.DeviceId);
                        memoryGroup[childBladeKey].Value = x.DeviceId;
                        memoryGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        discoveryData.Add(memoryGroup);
                        x.MemoryList.ForEach(
                            y =>
                            {
                                var memory = this.CreateMemory(y);
                                memory[this.PartChildGroupKey].Value =
                                    memoryGroup[this.PartChildGroupKey].Value;
                                memory[childBladeKey].Value = x.DeviceId;
                                memory[this.PartGroupKey].Value = childBladeGroupKey;

                                memory[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(memory);
                            });

                        #endregion

                        #region Disk

                        var diskGroup =
                            this.CreateLogicalChildGroup(this.DiskGroupClass, model.DeviceId, x.DeviceId);
                        diskGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        diskGroup[childBladeKey].Value = x.DeviceId;
                        discoveryData.Add(diskGroup);
                        x.DiskList.ForEach(
                            y =>
                            {
                                var disk = this.CreateDisk(y);
                                disk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                                disk[childBladeKey].Value = x.DeviceId;
                                disk[this.PartGroupKey].Value = childBladeGroupKey;

                                disk[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(disk);
                            });

                        #endregion

                        #region Mezz

                        var mezzGroup =
                            this.CreateLogicalChildGroup(this.MezzGroupClass, model.DeviceId, x.DeviceId);
                        mezzGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        mezzGroup[childBladeKey].Value = x.DeviceId;
                        discoveryData.Add(mezzGroup);
                        x.MezzList.ForEach(
                            y =>
                            {
                                var mezz = this.CreateMezz(y);
                                mezz[this.PartChildGroupKey].Value = mezzGroup[this.PartChildGroupKey].Value;
                                mezz[childBladeKey].Value = x.DeviceId;
                                mezz[this.PartGroupKey].Value = childBladeGroupKey;


                                mezz[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(mezz);
                            });

                        #endregion

                        #region Raid

                        var raidGroup = this.CreateLogicalChildGroup(this.RaidGroupClass, model.DeviceId, x.DeviceId);
                        raidGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        raidGroup[childBladeKey].Value = x.DeviceId;
                        discoveryData.Add(raidGroup);
                        x.RaidList.ForEach(
                            y =>
                            {
                                var raid = this.CreateRaidControl(y);
                                raid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                                raid[childBladeKey].Value = x.DeviceId;
                                raid[this.PartGroupKey].Value = childBladeGroupKey;

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
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Insert Blade Error:{model.DN}", e);
            }
        }

        /// <summary>
        /// Updates the main with out related.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">是否是轮询</param>
        /// <exception cref="System.Exception">e</exception>
        public void UpdateBlade(BladeServer model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start UpdateBlade.[{model.DN}] [isPolling:{isPolling}]");
                var oldBlade = this.GetBladeServer(model.DeviceId);
                if (oldBlade == null)
                {
                    throw new Exception($"Can not find the server:{model.DN}");
                }

                var propertys = this.BladeClass.PropertyCollection; // 获取到class的属性
                var discoveryData = new IncrementalDiscoveryData();

                oldBlade[propertys["eSight"]].Value = model.ESight;
                if (model.Status != "-3")
                {
                    oldBlade[propertys["Status"]].Value = model.StatusTxt;
                }
                oldBlade[propertys["IPAddress"]].Value = model.IpAddress;
                oldBlade[propertys["Location"]].Value = model.Location;

                if (isPolling)
                {
                    oldBlade[propertys["Manufacturer"]].Value = model.Manufacturer;
                }

                // oldBlade[propertys["Vendor"]].Value = "HUAWEI";
                oldBlade[propertys["SystemPowerState"]].Value = string.Empty;
                oldBlade[propertys["ServerModel"]].Value = model.ServerModel;
                oldBlade[this.DisplayNameField].Value = model.ServerName;
                discoveryData.Add(oldBlade);
                #region Fan

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

                #endregion

                #region Switch

                var switchGroup = oldBlade.GetRelatedMonitoringObjects(this.SwitchGroupClass).First();
                discoveryData.Add(switchGroup);

                var relatedSwitchObjects = switchGroup.GetRelatedMonitoringObjects(this.SwitchClass);
                var deleteSwitch = relatedSwitchObjects.Where(
                    x => model.SwitchList.All(
                        y => y.UUID != x[this.SwitchClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deleteSwitch.ForEach(x => { discoveryData.Remove(x); });
                model.SwitchList.ForEach(
                    x =>
                    {
                        var oldSwitch = relatedSwitchObjects.FirstOrDefault(y => y[this.SwitchClass.PropertyCollection["UUID"]].Value.ToString() == x.UUID);
                        if (oldSwitch == null)
                        {
                            var newSwitch = this.CreateSwitch(x, model.ServerName);
                            newSwitch[this.PartGroupKey].Value = switchGroup[this.PartGroupKey].Value;
                            newSwitch[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newSwitch);
                        }
                        else
                        {
                            this.UpdateSwitch(x, oldSwitch, model.ServerName);
                            discoveryData.Add(oldSwitch);
                        }
                    });

                #endregion

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

                #region Hmm

                var hmmGroup = oldBlade.GetRelatedMonitoringObjects(this.HmmGroupClass).First();
                discoveryData.Add(hmmGroup);

                var relatedHmmObjects = hmmGroup.GetRelatedMonitoringObjects(this.HmmClass);
                //Hmm板只会有一个
                var oldHmm = relatedHmmObjects.FirstOrDefault();
                if (oldHmm != null)
                {
                    this.UpdateHmm(model.HmmInfo, oldHmm);
                    discoveryData.Add(oldHmm);
                }

                #endregion

                if (isPolling)
                {
                    #region ChildBlade

                    var childBladeGroup = oldBlade.GetRelatedMonitoringObjects(this.ChildBladeGroupClass).First();
                    var childBladeGroupKey = childBladeGroup[this.PartGroupKey].Value.ToString();
                    discoveryData.Add(childBladeGroup);

                    var relatedChildBladeObjects = childBladeGroup.GetRelatedMonitoringObjects(this.ChildBladeClass);
                    var deleteChildBlade = relatedChildBladeObjects.Where(
                            x => model.ChildBlades.All(y => y.DeviceId != x[this.ChildBladeClass.PropertyCollection["DN"]].Value.ToString()))
                        .ToList();
                    deleteChildBlade.ForEach(x => { discoveryData.Remove(x); });
                    if (deleteChildBlade.Count > 0)
                    {
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"new child blades:{string.Join(",", model.ChildBlades.Select(x => x.DeviceId))}");
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"old child blades:{string.Join(",", relatedChildBladeObjects.Select(x => x[this.ChildBladeClass.PropertyCollection["DN"]].Value.ToString()))}");
                        HWLogger.GetESightSdkLogger(model.ESight).Debug($"remove child blades:{deleteChildBlade.Count }");
                    }
                    model.ChildBlades.ForEach(
                        x =>
                        {
                            var oldChildServer = relatedChildBladeObjects.FirstOrDefault(y => y[this.ChildBladeClass.PropertyCollection["DN"]].Value.ToString() == x.DeviceId);
                            if (oldChildServer == null)
                            {
                                #region MyRegion

                                var newChildBlade = this.CreateChildBlade(x, model.ServerName);
                                newChildBlade[this.PartGroupKey].Value = childBladeGroupKey;
                                newChildBlade[this.HuaweiServerKey].Value = model.DeviceId;
                                discoveryData.Add(newChildBlade);
                                var childBladeKey = this.ChildBladeClass.PropertyCollection["DN"];

                                #region CPU

                                var cpuGroup =
                                        this.CreateLogicalChildGroup(this.CpuGroupClass, model.DeviceId, x.DeviceId);

                                cpuGroup[childBladeKey].Value = x.DeviceId;
                                cpuGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                discoveryData.Add(cpuGroup);

                                x.CPUList.ForEach(
                                    y =>
                                    {
                                        var cpu = this.CreateCpu(y);
                                        cpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                                        cpu[childBladeKey].Value = x.DeviceId;
                                        cpu[this.PartGroupKey].Value = childBladeGroupKey;

                                        cpu[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(cpu);
                                    });

                                #endregion

                                #region Memory

                                var memoryGroup = this.CreateLogicalChildGroup(this.MemoryGroupClass, model.DeviceId, x.DeviceId);
                                memoryGroup[childBladeKey].Value = x.DeviceId;
                                memoryGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                discoveryData.Add(memoryGroup);
                                x.MemoryList.ForEach(
                                    y =>
                                    {
                                        var memory = this.CreateMemory(y);
                                        memory[this.PartChildGroupKey].Value =
                                                memoryGroup[this.PartChildGroupKey].Value;
                                        memory[childBladeKey].Value = x.DeviceId;
                                        memory[this.PartGroupKey].Value = childBladeGroupKey;

                                        memory[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(memory);
                                    });

                                #endregion

                                #region Disk

                                var diskGroup =
                                        this.CreateLogicalChildGroup(this.DiskGroupClass, model.DeviceId, x.DeviceId);
                                diskGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                diskGroup[childBladeKey].Value = x.DeviceId;
                                discoveryData.Add(diskGroup);
                                x.DiskList.ForEach(
                                    y =>
                                    {
                                        var disk = this.CreateDisk(y);
                                        disk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                                        disk[childBladeKey].Value = x.DeviceId;
                                        disk[this.PartGroupKey].Value = childBladeGroupKey;

                                        disk[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(disk);
                                    });

                                #endregion

                                #region Mezz

                                var mezzGroup =
                                        this.CreateLogicalChildGroup(this.MezzGroupClass, model.DeviceId, x.DeviceId);
                                mezzGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                mezzGroup[childBladeKey].Value = x.DeviceId;
                                discoveryData.Add(mezzGroup);
                                x.MezzList.ForEach(
                                    y =>
                                    {
                                        var mezz = this.CreateMezz(y);
                                        mezz[this.PartChildGroupKey].Value = mezzGroup[this.PartChildGroupKey].Value;
                                        mezz[childBladeKey].Value = x.DeviceId;
                                        mezz[this.PartGroupKey].Value = childBladeGroupKey;


                                        mezz[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(mezz);
                                    });

                                #endregion

                                #region Raid

                                var raidGroup = this.CreateLogicalChildGroup(this.RaidGroupClass, model.DeviceId, x.DeviceId);
                                raidGroup[this.PartGroupKey].Value = childBladeGroupKey;
                                raidGroup[childBladeKey].Value = x.DeviceId;
                                discoveryData.Add(raidGroup);
                                x.RaidList.ForEach(
                                    y =>
                                    {
                                        var raid = this.CreateRaidControl(y);
                                        raid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                                        raid[childBladeKey].Value = x.DeviceId;
                                        raid[this.PartGroupKey].Value = childBladeGroupKey;

                                        raid[this.HuaweiServerKey].Value = model.DeviceId;
                                        discoveryData.Add(raid);
                                    });

                                #endregion
                                #endregion
                            }
                            else
                            {
                                this.UpdateChildBlade(x, true);
                            }
                        });
                    #endregion
                }

                // var relatedObjects = oldBlade.GetRelatedMonitoringObjects(ChildBladeClass);
                // relatedObjects.ToList().ForEach(x => discoveryData.Add(x));
                discoveryData.Overwrite(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update Blade Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// Updates the child blade.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">if set to <c>true</c> [is polling].</param>
        /// <exception cref="System.Exception"></exception>
        public void UpdateChildBlade(ChildBlade model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start Update ChildBlade. [{model.DN}] [isPolling:{isPolling}]");
                var oldObject = this.GetObject($"DN = '{model.DeviceId}'", this.ChildBladeClass);
                if (oldObject == null)
                {
                    throw new Exception($"Can not find the child blade server:{model.DN}");
                }

                var propertys = this.ChildBladeClass.PropertyCollection; // 获取到class的属性
                var discoveryData = new IncrementalDiscoveryData();

                var childServerKey = this.ChildBladeClass.PropertyCollection["DN"];

                oldObject[propertys["UUID"]].Value = model.UUID;
                oldObject[propertys["eSight"]].Value = model.ESight;

                if (model.Status != "-3")
                {
                    oldObject[propertys["Status"]].Value = model.StatusTxt;
                }
                oldObject[propertys["PresentState"]].Value = "Present";
                oldObject[propertys["BmcIP"]].Value = model.IpAddress;
                oldObject[propertys["BmcMask"]].Value = string.Empty;
                oldObject[propertys["ProductName"]].Value = model.Mode;
                oldObject[propertys["BladeVersion"]].Value = string.Empty;
                oldObject[propertys["BoardSerialNumber"]].Value = string.Empty;
                oldObject[propertys["BoardManufacturerDate"]].Value = string.Empty;
                oldObject[propertys["ProductDescription"]].Value = string.Empty;
                oldObject[propertys["ProductSerialNumber"]].Value = string.Empty;
                oldObject[propertys["SystemCPUUsage"]].Value = string.Empty;
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

                #region Mezz

                var mezzGroup = oldObject.GetRelatedMonitoringObjects(this.MezzGroupClass).First();
                discoveryData.Add(mezzGroup);

                var relatedMezzObjects = mezzGroup.GetRelatedMonitoringObjects(this.MezzClass);
                var deleteMezz = relatedMezzObjects.Where(
                        x => model.MezzList.All(
                            y => y.UUID != x[this.MezzClass.PropertyCollection["UUID"]].Value.ToString()))
                    .ToList();
                deleteMezz.ForEach(x => { discoveryData.Remove(x); });
                model.MezzList.ForEach(
                    y =>
                    {
                        var oldMezz = relatedMezzObjects.FirstOrDefault(z => z[this.MezzClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldMezz == null)
                        {
                            var newMezz = this.CreateMezz(y);
                            newMezz[this.PartChildGroupKey].Value = mezzGroup[this.PartChildGroupKey].Value;
                            newMezz[childServerKey].Value = model.DeviceId;
                            newMezz[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newMezz[this.HuaweiServerKey].Value = oldObject[this.HuaweiServerKey].Value;
                            discoveryData.Add(newMezz);
                        }
                        else
                        {
                            this.UpdateMezz(y, oldMezz);
                            discoveryData.Add(oldMezz);
                        }
                    });

                #endregion

                #region Raid

                var raidGroup = oldObject.GetRelatedMonitoringObjects(this.RaidGroupClass).First();
                discoveryData.Add(raidGroup);

                var relatedRaidObjects = oldObject.GetRelatedMonitoringObjects(this.RaidClass);
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
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update ChildBlade Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// 只更新Swith板
        /// 暂无用
        /// </summary>
        /// <param name="model">The model.</param>
        public void UpdateSwitchBoard(HWBoard model)
        {
            //HWLogger.GetESightSdkLogger(model.ESight).Debug("Start UpdateSwitch");
            var oldObject = this.GetObject($"DN = '{model.DN}'", this.SwitchClass);
            if (oldObject == null)
            {
                throw new Exception($"Can not find the Switch board:{model.DN}");
            }

            var propertys = this.ChildBladeClass.PropertyCollection; // 获取到class的属性
            var discoveryData = new IncrementalDiscoveryData();

            oldObject[propertys["DN"]].Value = model.DN;
            oldObject[propertys["ParentDN"]].Value = model.ParentDN;
            oldObject[propertys["IpAddress"]].Value = model.IpAddress;
            oldObject[propertys["BoardType"]].Value = model.BoardType;
            oldObject[propertys["SerialNumber"]].Value = model.SN;
            oldObject[propertys["PartNumber"]].Value = model.PartNumber;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["ManufacturerDate"]].Value = model.ManuTime;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["AssertTag"]].Value = model.AssertTag;
            oldObject[propertys["MoId"]].Value = model.MoId;
            oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3") { oldObject[propertys["Status"]].Value = model.HealthState; }

            var parent = this.GetParentServer(oldObject);
            if (parent != null)
            {
                oldObject[this.DisplayNameField].Value = $"{parent.DisplayName}-{model.Name}";
            }
            discoveryData.Add(oldObject);

            discoveryData.Overwrite(this.MontioringConnector);
            //HWLogger.GetESightSdkLogger(model.ESight).Debug("End UpdateSwitch");
        }

        /// <summary>
        /// The get blade server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetBladeServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.BladeClass);
        }

        /// <summary>
        /// The get child blade server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetChildBladeServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.ChildBladeClass);
        }

        /// <summary>
        /// Gets the switch board.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>Microsoft.EnterpriseManagement.Monitoring.MonitoringObject.</returns>
        public MonitoringObject GetSwitchBoard(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.SwitchClass);
        }

        /// <summary>
        /// Gets the parent dn.
        /// </summary>
        /// <param name="childDeviceId">The child device identifier.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">
        /// </exception>
        public string GetParentDn(string childDeviceId)
        {
            var oldObject = this.GetObject($"DN = '{childDeviceId}'", this.ChildBladeClass);
            if (oldObject == null)
            {
                throw new Exception($"Can not find the child blade server:{childDeviceId}");
            }
            var propertys = this.BladeClass.PropertyCollection; // 获取到class的属性

            var parent = this.GetFullParentServer(oldObject);
            if (parent == null)
            {
                throw new Exception($"Can not find the parent.The childServerDeviceId:{childDeviceId}");
            }
            var deviceId = parent[this.HuaweiServerKey].Value.ToString();
            var esight = parent[propertys["eSight"]].Value.ToString();
            return deviceId.Replace(esight + "-", string.Empty);
        }

        /// <summary>
        /// Gets the switch parent dn.
        /// </summary>
        /// <param name="childDeviceId">The child device identifier.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">
        /// </exception>
        public string GetSwitchParentDn(string childDeviceId)
        {
            var oldObject = this.GetObject($"DN = '{childDeviceId}'", this.SwitchClass);
            if (oldObject == null)
            {
                throw new Exception($"Can not find the child blade server:{childDeviceId}");
            }
            var propertys = this.BladeClass.PropertyCollection; // 获取到class的属性

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
        /// The exsits blade server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private bool ExsitsBladeServer(string deviceId)
        {
            return this.ExsitsDeviceId(deviceId, this.BladeClass);
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
                case ServerTypeEnum.Blade:
                    this.InsertEvent(this.BladeClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.ChildBlade:
                    this.InsertEvent(this.ChildBladeClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.Switch:
                    this.InsertEvent(this.SwitchClass, eventData, eSightIp);
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
                case ServerTypeEnum.Blade:
                    this.InsertDeviceChangeEvent(this.BladeClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.ChildBlade:
                    this.InsertDeviceChangeEvent(this.ChildBladeClass, eventData, eSightIp);
                    break;
                case ServerTypeEnum.Switch:
                    this.InsertDeviceChangeEvent(this.SwitchClass, eventData, eSightIp);
                    break;
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// The remove child blade.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="deviceId">The device identifier.</param>
        public void RemoveChildBlade(string eSightIp, string deviceId)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Remove ChildBlade.[deviceId:{deviceId}]");
                var existingObject = this.GetChildBladeServer(deviceId);
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
        /// The remove child Switch.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="deviceId">The device identifier.</param>
        public void RemoveChildSwitch(string eSightIp, string deviceId)
        {
            try
            {
                HWLogger.GetESightSdkLogger(eSightIp).Info($"Remove ChildSwitch.[deviceId:{deviceId}]");
                var existingObject = this.GetSwitchBoard(deviceId);
                if (existingObject != null)
                {
                    var discovery = new IncrementalDiscoveryData();
                    discovery.Remove(existingObject);
                    discovery.Commit(this.MontioringConnector);
                }
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(eSightIp).Error($"RemoveChildSwitch.[deviceId:{deviceId}]", e);
            }
        }

        /// <summary>
        /// Removes the e sight blade.
        /// </summary>
        /// <param name="esightIp">The e sight ip.</param>
        public void RemoveServerFromMGroup(string esightIp)
        {
            this.RemoverServersByESight(this.BladeClass, esightIp);
        }

        /// <summary>
        /// The remover all blade.
        /// </summary>
        public void RemoverAllBlade()
        {
            this.RemoverServers(this.BladeClass);
        }

        /// <summary>
        /// Removes the blade on synchronize.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="newDeviceIds">The new device ids.</param>
        public void RemoveBladeOnSync(string eSightIp, List<string> newDeviceIds)
        {
            this.RemoveServersOnSync(eSightIp, newDeviceIds, this.BladeClass);
        }
        #endregion

        #region Create Methods

        /// <summary>
        /// The create blade server.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateBladeServer(BladeServer model)
        {
            var propertys = this.BladeClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.BladeClass); // 实例化一个class


            obj[propertys["eSight"]].Value = model.ESight;
            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["IPAddress"]].Value = model.IpAddress;
            obj[propertys["Location"]].Value = model.Location;
            obj[propertys["Vendor"]].Value = "HUAWEI";
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["SystemPowerState"]].Value = string.Empty;
            obj[propertys["ServerModel"]].Value = model.ServerModel;

            obj[this.DisplayNameField].Value = model.ServerName;
            return obj;
        }

        /// <summary>
        /// Creates the child blade .
        /// </summary>
        /// <param name="model">The Child model.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <returns>Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject.</returns>
        private MPObject CreateChildBlade(ChildBlade model, string parentName)
        {
            var propertys = this.ChildBladeClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.ChildBladeClass); // 实例化一个class

            obj[propertys["DN"]].Value = model.DeviceId;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["eSight"]].Value = model.ESight;

            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["PresentState"]].Value = "Present";
            obj[propertys["BmcIP"]].Value = model.IpAddress;
            obj[propertys["BmcMask"]].Value = string.Empty;
            obj[propertys["ProductName"]].Value = model.Mode;
            obj[propertys["BladeVersion"]].Value = string.Empty;
            obj[propertys["BoardSerialNumber"]].Value = string.Empty;
            obj[propertys["BoardManufacturerDate"]].Value = string.Empty;
            obj[propertys["ProductDescription"]].Value = string.Empty;
            obj[propertys["ProductSerialNumber"]].Value = string.Empty;
            obj[propertys["SystemCPUUsage"]].Value = string.Empty;

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
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["CPUInfo"]].Value = model.Model;

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
            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["DiskLocation"]].Value = model.Location;
            obj[propertys["DiskSerialNumber"]].Value = string.Empty;
            obj[propertys["DiskINterfaceType"]].Value = string.Empty;
            obj[propertys["DiskCapcity"]].Value = string.Empty;
            obj[propertys["DiskManufacturer"]].Value = string.Empty;

            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// The create fan.
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
            obj[propertys["ControlModel"]].Value = model.ControlModel;
            obj[propertys["RotatePercent"]].Value = model.RotatePercent;
            obj[propertys["FanSpeed"]].Value = model.Rotate;

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
        private MPObject CreateHmm(HWHMM model)
        {
            var propertys = this.HmmClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.HmmClass); // 实例化一个class

            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["ProductName"]].Value = model.Type;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["ProductSerialNumber"]].Value = model.ProductSN;
            obj[propertys["InletTemp"]].Value = string.Empty;
            obj[propertys["OutletTemp"]].Value = string.Empty;
            obj[propertys["AmbientTemp"]].Value = string.Empty;
            obj[propertys["LSWTemp"]].Value = string.Empty;
            obj[propertys["SoftwareVersion"]].Value = string.Empty;
            obj[propertys["BoardSerialNumber"]].Value = string.Empty;
            obj[propertys["SmmOtherVersion"]].Value = string.Empty;
            obj[propertys["SmmHostname"]].Value = string.Empty;
            obj[propertys["SmmRedunancy"]].Value = string.Empty;

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
            obj[propertys["Frequency"]].Value = model.Frequency;
            obj[propertys["Capacity"]].Value = model.Capacity;

            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        /// <summary>
        /// Creates the mezz.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateMezz(HWMezz model)
        {
            var propertys = this.MezzClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.MezzClass); // 实例化一个class

            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["Status"]].Value = model.MezzHealthStatusTxt;

            obj[propertys["MezzInfo"]].Value = model.MezzInfo;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["MezzLocation"]].Value = model.MezzLocation;
            obj[propertys["MezzMac"]].Value = model.MezzMac;

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
            obj[propertys["PowerMode"]].Value = model.InputMode;
            obj[propertys["PowerRating"]].Value = model.RatePower;
            obj[propertys["RuntimePower"]].Value = model.InputPower;

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
            obj[this.DisplayNameField].Value = model.Name;
            return obj;
        }

        private MPObject CreateSwitch(ChildSwithBoard model, string parentName)
        {
            var propertys = this.SwitchClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.SwitchClass); // 实例化一个class

            obj[propertys["DN"]].Value = model.DeviceId;
            obj[propertys["eSight"]].Value = model.ESight;
            obj[propertys["ParentDN"]].Value = model.ParentDN;
            obj[propertys["IpAddress"]].Value = model.IpAddress;
            obj[propertys["BoardType"]].Value = model.BoardType;
            obj[propertys["SerialNumber"]].Value = model.SN;
            obj[propertys["PartNumber"]].Value = model.PartNumber;
            obj[propertys["Manufacturer"]].Value = model.Manufacturer;
            obj[propertys["ManufacturerDate"]].Value = model.ManuTime;
            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["AssertTag"]].Value = model.AssertTag;
            obj[propertys["MoId"]].Value = model.MoId;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["Status"]].Value = model.HealthStateTxt;
            obj[this.DisplayNameField].Value = $"{parentName}-{model.Name}";
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

            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["CPUInfo"]].Value = model.Model;

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
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["DiskLocation"]].Value = model.Location;
            oldObject[propertys["DiskSerialNumber"]].Value = string.Empty;
            oldObject[propertys["DiskINterfaceType"]].Value = string.Empty;
            oldObject[propertys["DiskCapcity"]].Value = string.Empty;
            oldObject[propertys["DiskManufacturer"]].Value = string.Empty;

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
            oldObject[propertys["ControlModel"]].Value = model.ControlModel;
            oldObject[propertys["RotatePercent"]].Value = model.RotatePercent;
            oldObject[propertys["FanSpeed"]].Value = model.Rotate;

            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the raid control.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateHmm(HWHMM model, MonitoringObject oldObject)
        {
            var propertys = this.HmmClass.PropertyCollection; // 获取到class的属性

            if (model.Status != "-3")
            {
                oldObject[propertys["Status"]].Value = model.StatusTxt;
            }

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["ProductName"]].Value = model.Type;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["ProductSerialNumber"]].Value = model.ProductSN;

            oldObject[propertys["InletTemp"]].Value = string.Empty;
            oldObject[propertys["OutletTemp"]].Value = string.Empty;
            oldObject[propertys["AmbientTemp"]].Value = string.Empty;
            oldObject[propertys["LSWTemp"]].Value = string.Empty;
            oldObject[propertys["SoftwareVersion"]].Value = string.Empty;
            oldObject[propertys["BoardSerialNumber"]].Value = string.Empty;
            oldObject[propertys["SmmOtherVersion"]].Value = string.Empty;
            oldObject[propertys["SmmHostname"]].Value = string.Empty;
            oldObject[propertys["SmmRedunancy"]].Value = string.Empty;

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
            oldObject[propertys["Frequency"]].Value = model.Frequency;
            oldObject[propertys["Capacity"]].Value = model.Capacity;

            oldObject[this.DisplayNameField].Value = model.Name;
        }

        /// <summary>
        /// Updates the mezz.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="oldObject">The old object.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        private void UpdateMezz(HWMezz model, MonitoringObject oldObject)
        {
            var propertys = this.MezzClass.PropertyCollection; // 获取到class的属性

            if (model.MezzHealthStatus != "-3")
            {
                oldObject[propertys["Status"]].Value = model.MezzHealthStatusTxt;
            }

            oldObject[propertys["MezzInfo"]].Value = model.MezzInfo;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["MezzLocation"]].Value = model.MezzLocation;
            oldObject[propertys["MezzMac"]].Value = model.MezzMac;

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
            oldObject[propertys["PowerMode"]].Value = model.InputMode;
            oldObject[propertys["PowerRating"]].Value = model.RatePower;
            oldObject[propertys["RuntimePower"]].Value = model.InputPower;

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

            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }

            oldObject[propertys["Type"]].Value = model.RaidType;
            oldObject[propertys["DeviceInterface"]].Value = model.InterfaceType;
            oldObject[propertys["BBUType"]].Value = model.BbuType;
            oldObject[this.DisplayNameField].Value = model.Name;
        }

        private void UpdateSwitch(ChildSwithBoard model, MonitoringObject oldObject, string parentName)
        {
            var propertys = this.SwitchClass.PropertyCollection; // 获取到class的属性
            oldObject[propertys["eSight"]].Value = model.ESight;
            oldObject[propertys["ParentDN"]].Value = model.ParentDN;
            oldObject[propertys["IpAddress"]].Value = model.IpAddress;
            oldObject[propertys["BoardType"]].Value = model.BoardType;
            oldObject[propertys["SerialNumber"]].Value = model.SN;
            oldObject[propertys["PartNumber"]].Value = model.PartNumber;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["ManufacturerDate"]].Value = model.ManuTime;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["AssertTag"]].Value = model.AssertTag;
            oldObject[propertys["MoId"]].Value = model.MoId;
            oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthStateTxt;
            }

            oldObject[this.DisplayNameField].Value = $"{parentName}-{model.Name}";
        }

        #endregion

    }
}