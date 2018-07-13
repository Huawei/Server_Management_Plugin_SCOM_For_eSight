// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 11-20-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="RackConnector.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The rack connector.</summary>
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
    ///     The rack connector.
    /// </summary>
    public class RackConnector : BaseConnector
    {
        #region Fields

        /// <summary>
        ///     The connector guid.
        /// </summary>
        private static Guid connectorGuid = new Guid("{528C8486-2E62-42FB-9AFB-96CB8C089862}");

        /// <summary>
        ///     The instance.
        /// </summary>
        private static RackConnector instance;

        /// <summary>
        ///     The cpu class.
        /// </summary>
        private ManagementPackClass cpuClass;

        /// <summary>
        ///     The cpu group class.
        /// </summary>
        private ManagementPackClass cpuGroupClass;

        /// <summary>
        ///     The disk class.
        /// </summary>
        private ManagementPackClass diskClass;

        /// <summary>
        ///     The disk group class.
        /// </summary>
        private ManagementPackClass diskGroupClass;

        /// <summary>
        ///     The fan class.
        /// </summary>
        private ManagementPackClass fanClass;

        /// <summary>
        ///     The fan group class.
        /// </summary>
        private ManagementPackClass fanGroupClass;

        /// <summary>
        ///     The memory class.
        /// </summary>
        private ManagementPackClass memoryClass;

        /// <summary>
        ///     The memory group class.
        /// </summary>
        private ManagementPackClass memoryGroupClass;

        /// <summary>
        ///     The power supply class.
        /// </summary>
        private ManagementPackClass powerSupplyClass;

        /// <summary>
        ///     The power supply group class.
        /// </summary>
        private ManagementPackClass powerSupplyGroupClass;

        /// <summary>
        ///     The rack class.
        /// </summary>
        private ManagementPackClass rackClass;

        /// <summary>
        ///     The raid class.
        /// </summary>
        private ManagementPackClass raidClass;

        /// <summary>
        ///     The raid group class.
        /// </summary>
        private ManagementPackClass raidGroupClass;
        #endregion

        #region Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        public static RackConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RackConnector
                    {
                        ConnectorName = "RackServer.Connector",
                        ConnectorGuid = connectorGuid,
                        ConnectorInfo = new ConnectorInfo
                        {
                            Description = "RackServer Connector Description",
                            DisplayName = "RackServer Connector",
                            Name = "RackServer.Connector",
                            DiscoveryDataIsManaged = true
                        }
                    };
                    instance.Install();
                }
                return instance;
            }
        }

        /// <summary>
        ///     Gets the cpu class.
        /// </summary>
        public ManagementPackClass CpuClass => this.cpuClass
                                               ?? (this.cpuClass =
                                                       MGroup.Instance.GetManagementPackClass(
                                                           EntityTypeConst.RackServer.Cpu));

        /// <summary>
        ///     Gets the cpu group class.
        /// </summary>
        public ManagementPackClass CpuGroupClass => this.cpuGroupClass
                                                    ?? (this.cpuGroupClass =
                                                            MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.RackServer.CpuGroup));

        /// <summary>
        ///     Gets the fan class.
        /// </summary>
        public ManagementPackClass FanClass => this.fanClass
                                               ?? (this.fanClass =
                                                       MGroup.Instance.GetManagementPackClass(
                                                           EntityTypeConst.RackServer.Fan));

        /// <summary>
        ///     Gets the fan group class.
        /// </summary>
        public ManagementPackClass FanGroupClass => this.fanGroupClass
                                                    ?? (this.fanGroupClass =
                                                            MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.RackServer.FanGroup));

        /// <summary>
        ///     Gets the memory class.
        /// </summary>
        public ManagementPackClass MemoryClass => this.memoryClass
                                                  ?? (this.memoryClass =
                                                          MGroup.Instance.GetManagementPackClass(
                                                              EntityTypeConst.RackServer.Memory));

        /// <summary>
        ///     Gets the memory group class.
        /// </summary>
        public ManagementPackClass MemoryGroupClass => this.memoryGroupClass
                                                       ?? (this.memoryGroupClass =
                                                               MGroup.Instance.GetManagementPackClass(
                                                                   EntityTypeConst.RackServer.MemoryGroup));

        /// <summary>
        ///     Gets the physical disk class.
        /// </summary>
        public ManagementPackClass PhysicalDiskClass => this.diskClass
                                                        ?? (this.diskClass = MGroup.Instance.GetManagementPackClass(
                                                                EntityTypeConst.RackServer.PhysicalDisk));

        /// <summary>
        ///     Gets the physical disk group class.
        /// </summary>
        public ManagementPackClass PhysicalDiskGroupClass => this.diskGroupClass
                                                             ?? (this.diskGroupClass =
                                                                     MGroup.Instance.GetManagementPackClass(
                                                                         EntityTypeConst.RackServer.PhysicalDiskGroup));

        /// <summary>
        ///     Gets the power supply class.
        /// </summary>
        public ManagementPackClass PowerSupplyClass => this.powerSupplyClass
                                                       ?? (this.powerSupplyClass =
                                                               MGroup.Instance.GetManagementPackClass(
                                                                   EntityTypeConst.RackServer.PowerSupply));

        /// <summary>
        ///     Gets the power supply group class.
        /// </summary>
        public ManagementPackClass PowerSupplyGroupClass => this.powerSupplyGroupClass
                                                            ?? (this.powerSupplyGroupClass =
                                                                    MGroup.Instance.GetManagementPackClass(
                                                                        EntityTypeConst.RackServer.PowerSupplyGroup));

        /// <summary>
        ///     Gets the rack class.
        /// </summary>
        public ManagementPackClass RackClass => this.rackClass
                                                ?? (this.rackClass =
                                                        MGroup.Instance.GetManagementPackClass(
                                                            EntityTypeConst.RackServer.MainName));

        /// <summary>
        ///     Gets the raid class.
        /// </summary>
        public ManagementPackClass RaidClass => this.raidClass
                                                ?? (this.raidClass =
                                                        MGroup.Instance.GetManagementPackClass(
                                                            EntityTypeConst.RackServer.RaidController));

        /// <summary>
        ///     Gets the raid group class.
        /// </summary>
        public ManagementPackClass RaidGroupClass => this.raidGroupClass
                                                     ?? (this.raidGroupClass =
                                                             MGroup.Instance.GetManagementPackClass(
                                                                 EntityTypeConst.RackServer.RaidControllerGroup));

        #endregion

        #region Public Methods

        /// <summary>
        /// Synchronizes the server.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SyncServer(RackServer model)
        {
            // 存在则更新
            if (ExsitsRackServer(model.DeviceId))
            {
                this.UpdateRack(model, true);
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
        private void InsertDetials(RackServer model)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Insert Rack:{model.DN}");
                var discoveryData = new IncrementalDiscoveryData();

                var rackServer = this.CreateRackServer(model);
                discoveryData.Add(rackServer);

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

                #region CPU

                var cpuGroup = this.CreateLogicalGroup(this.CpuGroupClass, model.DeviceId);
                discoveryData.Add(cpuGroup);
                model.CPUList.ForEach(
                    x =>
                        {
                            var cpu = this.CreateCpu(x);
                            cpu[this.PartGroupKey].Value = cpuGroup[this.PartGroupKey].Value;
                            cpu[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(cpu);
                        });

                #endregion

                #region Memory

                var memoryGroup = this.CreateLogicalGroup(this.MemoryGroupClass, model.DeviceId);
                discoveryData.Add(memoryGroup);
                model.MemoryList.ForEach(
                    x =>
                        {
                            var memory = this.CreateMemory(x);
                            memory[this.PartGroupKey].Value = memoryGroup[this.PartGroupKey].Value;
                            memory[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(memory);
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
        /// The update rack.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isPolling">是否是轮询</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="Exception"></exception>
        public void UpdateRack(RackServer model, bool isPolling)
        {
            try
            {
                HWLogger.GetESightSdkLogger(model.ESight).Debug($"Start UpdateRack. [{model.DN}] [isPolling:{isPolling}]");
                var oldBlade = this.GetRackServer(model.DeviceId);
                if (oldBlade == null)
                {
                    throw new Exception($"Can not find the server:{model.DN}");
                }
                var propertys = this.RackClass.PropertyCollection; // 获取到class的属性
                var discoveryData = new IncrementalDiscoveryData();
                oldBlade[this.DisplayNameField].Value = model.Name;
                if (model.Status != "-3")
                {
                    oldBlade[propertys["Status"]].Value = model.Status;
                }
                oldBlade[propertys["Vendor"]].Value = "HUAWEI";
                oldBlade[propertys["UUID"]].Value = model.UUID;
                oldBlade[propertys["IPAddress"]].Value = model.IpAddress;

                oldBlade[propertys["CPLDVersion"]].Value = string.Empty;
                oldBlade[propertys["UbootVersion"]].Value = string.Empty;
                oldBlade[propertys["ProductSn"]].Value = model.ProductSN;
                oldBlade[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
                oldBlade[propertys["CPUNums"]].Value = model.CPUNums;
                oldBlade[propertys["CPUCores"]].Value = model.CPUCores;
                if (isPolling)
                {
                    oldBlade[propertys["Manufacturer"]].Value = model.Manufacturer;
                    oldBlade[propertys["iBMCVersion"]].Value = model.Version;
                }

                // oldBlade[propertys["ServerName"]].Value = model.ServerName;
                oldBlade[propertys["BMCMacAddr"]].Value = model.BmcMacAddr;
                oldBlade[propertys["eSight"]].Value = model.ESight;
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

                #region CPU

                var cpuGroup = oldBlade.GetRelatedMonitoringObjects(this.CpuGroupClass).First();
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
                            newCpu[this.HuaweiServerKey].Value = model.DeviceId;
                            newCpu[this.PartGroupKey].Value = cpuGroup[this.PartGroupKey].Value;
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

                var memoryGroup = oldBlade.GetRelatedMonitoringObjects(this.MemoryGroupClass).First();
                discoveryData.Add(memoryGroup);

                var relatedMemoryObjects = memoryGroup.GetRelatedMonitoringObjects(this.MemoryClass);
                var deleteMemory = relatedMemoryObjects.Where(x => model.MemoryList.All(y => y.UUID != x[this.MemoryClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
                deleteMemory.ForEach(x => { discoveryData.Remove(x); });
                model.MemoryList.ForEach(
                    y =>
                    {
                        var oldMemory = relatedMemoryObjects.FirstOrDefault(z => z[this.MemoryClass.PropertyCollection["UUID"]].Value.ToString() == y.UUID);
                        if (oldMemory == null)
                        {
                            var newMemory = this.CreateMemory(y);
                            newMemory[this.HuaweiServerKey].Value = model.DeviceId;
                            newMemory[this.PartGroupKey].Value = memoryGroup[this.PartGroupKey].Value;
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

                var diskGroup = oldBlade.GetRelatedMonitoringObjects(this.PhysicalDiskGroupClass).First();
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

                var raidGroup = oldBlade.GetRelatedMonitoringObjects(this.RaidGroupClass).First();
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
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Update UpdateRack Error.[{model.DN}] [isPolling:{isPolling}]", e);
            }
        }

        /// <summary>
        /// The exsits rack server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool ExsitsRackServer(string deviceId)
        {
            return this.ExsitsDeviceId(deviceId, this.RackClass);
        }

        /// <summary>
        /// The get rack server.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetRackServer(string deviceId)
        {
            return this.GetObject($"DN = '{deviceId}'", this.RackClass);
        }

        #endregion

        #region Event

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertEvent(EventData eventData, string eSightIp)
        {
            this.InsertEvent(this.RackClass, eventData, eSightIp);
        }

        /// <summary>
        /// Inserts the history event.
        /// </summary>
        /// <param name="eventDatas">The event datas.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void InsertHistoryEvent(List<EventData> eventDatas, string eSightIp)
        {
            this.InsertHistoryEvent(this.RackClass, eventDatas, eSightIp);
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        public void InsertDeviceChangeEvent(DeviceChangeEventData deviceChangeEventData, string eSightIp)
        {
            this.InsertDeviceChangeEvent(this.RackClass, deviceChangeEventData, eSightIp);
        }
        #endregion

        #region Remove
        /// <summary>
        /// Deletes the servers on synchronize.
        /// </summary>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="newDeviceIds">The new device ids.</param>
        public void DeleteRackOnSync(string eSightIp, List<string> newDeviceIds)
        {
            this.RemoveServersOnSync(eSightIp, newDeviceIds, this.RackClass);
        }

        /// <summary>
        /// Removes the e sight rack.
        /// </summary>
        /// <param name="eSightIp">
        /// The e sight ip.
        /// </param>
        public void RemoveServerFromMGroup(string eSightIp)
        {
            this.RemoverServersByESight(this.RackClass, eSightIp);
        }

        /// <summary>
        ///     The remover all rack.
        /// </summary>
        public void RemoverAllRack()
        {
            this.RemoverServers(this.RackClass);
        }

        #endregion

        #region Create Methods

        /// <summary>
        /// The create rack server.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MPObject"/>.
        /// </returns>
        private MPObject CreateRackServer(RackServer model)
        {
            var propertys = this.RackClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.RackClass); // 实例化一个class
            obj[this.HuaweiServerKey].Value = model.DeviceId;
            obj[propertys["Status"]].Value = model.Status == "-3" ? "0" : model.Status;
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
        /// Creates the cpu.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="MPObject" />.</returns>
        private MPObject CreateCpu(HWCPU model)
        {
            var propertys = this.CpuClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.CpuClass); // 实例化一个class
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
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
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="MPObject" />.</returns>
        private MPObject CreateDisk(HWDisk model)
        {
            var propertys = this.PhysicalDiskClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.PhysicalDiskClass); // 实例化一个class
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["PresentState"]].Value = model.PresentState;
            obj[propertys["Speed"]].Value = model.Rotate;
            obj[propertys["RotatePercent"]].Value = model.GetRotatePercent("rack");
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
            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
            obj[propertys["UUID"]].Value = model.UUID;

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

            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
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

            obj[propertys["Status"]].Value = model.HealthState == "-3" ? "0" : model.HealthState;
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
                oldObject[propertys["Status"]].Value = model.HealthState;
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
            var propertys = this.PhysicalDiskClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthState;
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
                oldObject[propertys["Status"]].Value = model.HealthState;
            }
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Speed"]].Value = model.Rotate;
            oldObject[propertys["RotatePercent"]].Value = model.GetRotatePercent("rack");
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

            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthState;
            }

            // oldObject[propertys["UUID"]].Value = model.UUID;
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

            // oldObject[propertys["UUID"]].Value = model.UUID;
            if (model.HealthState != "-3")
            {
                oldObject[propertys["Status"]].Value = model.HealthState;
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
                oldObject[propertys["Status"]].Value = model.HealthState;
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