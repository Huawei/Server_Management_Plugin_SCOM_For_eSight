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
        /// The exsits rack server.
        /// </summary>
        /// <param name="dn">
        /// The dn.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExsitsRackServer(string dn)
        {
            return this.ExsitsDn(dn, this.RackClass);
        }

        /// <summary>
        /// The get rack server.
        /// </summary>
        /// <param name="dn">
        /// The dn.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        public MonitoringObject GetRackServer(string dn)
        {
            return this.GetObject($"DN = '{dn}'", this.RackClass);
        }

        /// <summary>
        /// The insert detials.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void InsertDetials(RackServer model)
        {
            var discoveryData = new IncrementalDiscoveryData();
            var baseComputer = this.GetComputerByDn(model.DN);
            if (baseComputer == null)
            {
                var newBaseComputer = this.CreateComputer(model.DN);
                discoveryData.Add(newBaseComputer);
            }
            else
            {
                discoveryData.Add(baseComputer);
            }

            var rackServer = this.CreateRackServer(model);
            rackServer[this.ComputerKey].Value = model.DN;
            discoveryData.Add(rackServer);

            #region Fan

            var fanGroup = this.CreateLogicalGroup(this.FanGroupClass, model.DN);
            discoveryData.Add(fanGroup);
            model.FanList.ForEach(
                x =>
                    {
                        var fan = this.CreateFan(x);
                        fan[this.PartGroupKey].Value = fanGroup[this.PartGroupKey].Value;

                        fan[this.HuaweiServerKey].Value = model.DN;
                        fan[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(fan);
                    });

            #endregion

            #region PSU

            var powerSupplyGroup = this.CreateLogicalGroup(this.PowerSupplyGroupClass, model.DN);
            discoveryData.Add(powerSupplyGroup);
            model.PowerSupplyList.ForEach(
                x =>
                    {
                        var powerSupply = this.CreatePowerSupply(x);
                        powerSupply[this.PartGroupKey].Value = powerSupplyGroup[this.PartGroupKey].Value;
                        powerSupply[this.HuaweiServerKey].Value = model.DN;
                        powerSupply[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(powerSupply);
                    });

            #endregion

            #region Raid

            var raidGroup = this.CreateLogicalGroup(this.RaidGroupClass, model.DN);
            discoveryData.Add(raidGroup);
            model.RaidList.ForEach(
                y =>
                    {
                        var raid = this.CreateRaidControl(y);
                        raid[this.PartGroupKey].Value = raidGroup[this.PartGroupKey].Value;
                        raid[this.HuaweiServerKey].Value = model.DN;
                        raid[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(raid);
                    });

            #endregion

            #region CPU

            var cpuGroup = this.CreateLogicalGroup(this.CpuGroupClass, model.DN);
            discoveryData.Add(cpuGroup);
            model.CPUList.ForEach(
                x =>
                    {
                        var cpu = this.CreateCpu(x);
                        cpu[this.PartGroupKey].Value = cpuGroup[this.PartGroupKey].Value;
                        cpu[this.HuaweiServerKey].Value = model.DN;
                        cpu[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(cpu);
                    });

            #endregion

            #region Memory

            var memoryGroup = this.CreateLogicalGroup(this.MemoryGroupClass, model.DN);
            discoveryData.Add(memoryGroup);
            model.MemoryList.ForEach(
                x =>
                    {
                        var memory = this.CreateMemory(x);
                        memory[this.PartGroupKey].Value = memoryGroup[this.PartGroupKey].Value;
                        memory[this.HuaweiServerKey].Value = model.DN;
                        memory[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(memory);
                    });

            #endregion

            #region Disk

            var diskGroup = this.CreateLogicalGroup(this.PhysicalDiskGroupClass, model.DN);
            discoveryData.Add(diskGroup);
            model.DiskList.ForEach(
                x =>
                    {
                        var disk = this.CreateDisk(x);
                        disk[this.PartGroupKey].Value = diskGroup[this.PartGroupKey].Value;
                        disk[this.HuaweiServerKey].Value = model.DN;
                        disk[this.ComputerKey].Value = model.DN;
                        discoveryData.Add(disk);
                    });

            #endregion

            if (!this.ExsitsRackServer(model.DN))
            {
                discoveryData.Commit(this.MontioringConnector);
            }
            else
            {
                discoveryData.Overwrite(this.MontioringConnector);
            }
        }

        /// <summary>
        /// The update rack.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <exception cref="Exception">
        /// Can not find the server 
        /// </exception>
        public void UpdateRack(RackServer model)
        {
            HWLogger.NOTIFICATION_PROCESS.Debug($"Start UpdateRack {model.DN}");
            var oldBlade = this.GetRackServer(model.DN);
            if (oldBlade == null)
            {
                throw new Exception($"Can not find the server:{model.DN}");
            }
            var propertys = this.RackClass.PropertyCollection; // 获取到class的属性
            var discoveryData = new IncrementalDiscoveryData();
            oldBlade[this.DisplayNameField].Value = model.Name;
            oldBlade[propertys["Status"]].Value = model.Status;
            oldBlade[propertys["Vendor"]].Value = "HUAWEI";
            oldBlade[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldBlade[propertys["UUID"]].Value = model.UUID;
            oldBlade[propertys["IPAddress"]].Value = model.IpAddress;

            // oldBlade[propertys["iBMCVersion"]].Value = model.Version;
            oldBlade[propertys["CPLDVersion"]].Value = string.Empty;
            oldBlade[propertys["UbootVersion"]].Value = string.Empty;
            oldBlade[propertys["ProductSn"]].Value = model.ProductSN;
            oldBlade[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
            oldBlade[propertys["CPUNums"]].Value = model.CPUNums;
            oldBlade[propertys["CPUCores"]].Value = model.CPUCores;

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
                        var fan = this.UpdateFan(x);
                        if (fan == null)
                        {
                            var newFan = this.CreateFan(x);
                            newFan[this.ComputerKey].Value = model.DN;
                            newFan[this.HuaweiServerKey].Value = model.DN;
                            newFan[this.PartGroupKey].Value = fanGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newFan);
                        }
                        else
                        {
                            discoveryData.Add(fan);
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
                        var psu = this.UpdatePowerSupply(x);
                        if (psu == null)
                        {
                            var newpsu = this.CreatePowerSupply(x);
                            newpsu[this.ComputerKey].Value = model.DN;
                            newpsu[this.HuaweiServerKey].Value = model.DN;
                            newpsu[this.PartGroupKey].Value = psuGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newpsu);
                        }
                        else
                        {
                            discoveryData.Add(psu);
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
                        var cpu = this.UpdateCpu(y);
                        if (cpu == null)
                        {
                            var newCpu = this.CreateCpu(y);
                            newCpu[this.ComputerKey].Value = model.DN;
                            newCpu[this.HuaweiServerKey].Value = model.DN;
                            newCpu[this.PartGroupKey].Value = cpuGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newCpu);
                        }
                        else
                        {
                            discoveryData.Add(cpu);
                        }
                    });

            #endregion

            #region Memory

            var memoryGroup = oldBlade.GetRelatedMonitoringObjects(this.MemoryGroupClass).First();
            discoveryData.Add(memoryGroup);

            var relatedMemoryObjects = memoryGroup.GetRelatedMonitoringObjects(this.MemoryClass);
            var deleteMemory = relatedMemoryObjects.Where(
                x => model.MemoryList.All(
                    y => y.UUID != x[this.MemoryClass.PropertyCollection["UUID"]].Value.ToString())).ToList();
            deleteMemory.ForEach(x => { discoveryData.Remove(x); });
            model.MemoryList.ForEach(
                y =>
                    {
                        var memory = this.UpdateMemory(y);
                        if (memory == null)
                        {
                            var newMemory = this.CreateMemory(y);
                            newMemory[this.ComputerKey].Value = model.DN;
                            newMemory[this.HuaweiServerKey].Value = model.DN;
                            newMemory[this.PartGroupKey].Value = memoryGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newMemory);
                        }
                        else
                        {
                            discoveryData.Add(memory);
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
                        var disk = this.UpdateDisk(y);
                        if (disk == null)
                        {
                            var newDisk = this.CreateDisk(y);
                            newDisk[this.ComputerKey].Value = model.DN;
                            newDisk[this.HuaweiServerKey].Value = model.DN;
                            newDisk[this.PartGroupKey].Value = diskGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newDisk);
                        }
                        else
                        {
                            discoveryData.Add(disk);
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
                        var raid = this.UpdateRaidControl(y);
                        if (raid == null)
                        {
                            var newRaid = this.CreateRaidControl(y);
                            newRaid[this.ComputerKey].Value = model.DN;
                            newRaid[this.HuaweiServerKey].Value = model.DN;
                            newRaid[this.PartGroupKey].Value = raidGroup[this.PartGroupKey].Value;
                            discoveryData.Add(newRaid);
                        }
                        else
                        {
                            discoveryData.Add(raid);
                        }
                    });

            #endregion

            discoveryData.Overwrite(this.MontioringConnector);
            HWLogger.NOTIFICATION_PROCESS.Debug($"End UpdateRack {model.DN}");
        }

        /// <summary>
        /// Inserts the event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertEvent(EventData eventData)
        {
            this.InsertEvent(this.RackClass, eventData);
        }

        /// <summary>
        /// Inserts the history event.
        /// </summary>
        /// <param name="eventDatas">The event datas.</param>
        public void InsertHistoryEvent(List<EventData> eventDatas)
        {
            this.InsertHistoryEvent(this.RackClass, eventDatas);
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="deviceChangeEventData">The device change event data.</param>
        public void InsertDeviceChangeEvent(DeviceChangeEventData deviceChangeEventData)
        {
            this.InsertDeviceChangeEvent(this.RackClass, deviceChangeEventData);
        }

        /// <summary>
        /// Removes the e sight rack.
        /// </summary>
        /// <param name="eSightIp">
        /// The e sight ip.
        /// </param>
        public void RemoveServerFromMGroup(string eSightIp)
        {
            this.RemoverServers(this.RackClass, eSightIp);
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
            obj[this.HuaweiServerKey].Value = model.DN;
            obj[propertys["Status"]].Value = model.Status;
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

            obj[propertys["Status"]].Value = model.HealthState;
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

            obj[propertys["Status"]].Value = model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState;
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

            obj[propertys["Status"]].Value = model.HealthState;
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

            obj[propertys["Status"]].Value = model.HealthState;
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
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        private MonitoringObject UpdateCpu(HWCPU model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.CpuClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.CpuClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Type"]].Value = model.Model;
            oldObject[propertys["Frequency"]].Value = model.Frequency;
            oldObject[propertys["CoreCount"]].Value = string.Empty;
            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the disk.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        private MonitoringObject UpdateDisk(HWDisk model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.PhysicalDiskClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.PhysicalDiskClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["Locator"]].Value = model.Location;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Diskcapacity"]].Value = string.Empty;
            oldObject[propertys["IndterfaceType"]].Value = string.Empty;
            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the fan.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// MPObject.
        /// </returns>
        private MonitoringObject UpdateFan(HWFAN model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.FanClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.FanClass.PropertyCollection; // 获取到class的属性
            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Speed"]].Value = model.Rotate;
            oldObject[propertys["RotatePercent"]].Value = model.GetRotatePercent("rack");
            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the memory.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        private MonitoringObject UpdateMemory(HWMemory model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.MemoryClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.MemoryClass.PropertyCollection; // 获取到class的属性

            oldObject[propertys["Status"]].Value = model.HealthState;

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["Size"]].Value = model.Capacity;
            oldObject[propertys["Frequency"]].Value = model.Frequency;

            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the power supply.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// MPObject.
        /// </returns>
        private MonitoringObject UpdatePowerSupply(HWPSU model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.PowerSupplyClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.PowerSupplyClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["InputMode"]].Value = model.InputMode;
            oldObject[propertys["Model"]].Value = model.Model;
            oldObject[propertys["PowerRating"]].Value = model.RatePower;
            oldObject[propertys["InputPower"]].Value = model.InputPower;

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the raid control.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        private MonitoringObject UpdateRaidControl(HWRAID model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.RaidClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.RaidClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["Type"]].Value = model.RaidType;
            oldObject[propertys["DeviceInterface"]].Value = model.InterfaceType;
            oldObject[propertys["BBUType"]].Value = model.BbuType;
            oldObject[propertys["FirmwareVersion"]].Value = string.Empty;
            oldObject[propertys["DirverVersion"]].Value = string.Empty;
            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }
        #endregion
    }
}