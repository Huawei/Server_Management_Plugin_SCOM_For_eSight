// ***********************************************************************
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
        /// The insert event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertEvent(EventData eventData)
        {
            this.InsertEvent(this.BladeClass, eventData);
        }

        /// <summary>
        /// Inserts the history event.
        /// </summary>
        /// <param name="eventDatas">The event datas.</param>
        public void InsertHistoryEvent(List<EventData> eventDatas)
        {
            this.InsertHistoryEvent(this.BladeClass, eventDatas);
        }

        /// <summary>
        /// Inserts the child history event.
        /// </summary>
        /// <param name="eventDatas">The event datas.</param>
        public void InsertChildHistoryEvent(List<EventData> eventDatas)
        {
            this.InsertHistoryEvent(this.ChildBladeClass, eventDatas);
        }

        /// <summary>
        /// The insert child blade event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertChildBladeEvent(EventData eventData)
        {
            this.InsertEvent(this.ChildBladeClass, eventData);
        }

        /// <summary>
        /// Inserts the device change event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertDeviceChangeEvent(DeviceChangeEventData eventData)
        {
            this.InsertDeviceChangeEvent(this.BladeClass, eventData);
        }

        /// <summary>
        /// Inserts the child device change event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void InsertChildDeviceChangeEvent(DeviceChangeEventData eventData)
        {
            this.InsertDeviceChangeEvent(this.ChildBladeClass, eventData);
        }

        /// <summary>
        /// The insert detials.
        /// </summary>
        /// <param name="model">The model.</param>
        public void InsertDetials(BladeServer model)
        {
            var discoveryData = new IncrementalDiscoveryData();

            var baseComputer = this.GetComputerByDeviceId(model.DeviceId);
            if (baseComputer == null)
            {
                var newBaseComputer = this.CreateComputer(model.DeviceId);
                discoveryData.Add(newBaseComputer);
            }
            else
            {
                discoveryData.Add(baseComputer);
            }

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
                        fan[this.ComputerKey].Value = model.DeviceId;
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
                        powerSupply[this.ComputerKey].Value = model.DeviceId;
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
                        var switchObject = this.CreateSwitch(x);
                        switchObject[this.PartGroupKey].Value = switchGroup[this.PartGroupKey].Value;
                        switchObject[this.ComputerKey].Value = model.DeviceId;
                        switchObject[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(switchObject);
                    });

            #endregion

            #region HMM

            var hmmGroup = this.CreateLogicalGroup(this.HmmGroupClass, model.DeviceId);
            discoveryData.Add(hmmGroup);

            var hmm = this.CreateHmm(model.HmmInfo);
            hmm[this.PartGroupKey].Value = hmmGroup[this.PartGroupKey].Value;
            hmm[this.ComputerKey].Value = model.DeviceId;
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
                        var childBlade = this.CreateChildBlade(x);
                        childBlade[this.PartGroupKey].Value = childBladeGroupKey;
                        childBlade[this.ComputerKey].Value = model.DeviceId;
                        childBlade[this.HuaweiServerKey].Value = model.DeviceId;
                        discoveryData.Add(childBlade);

                        var childBladeKey = this.ChildBladeClass.PropertyCollection["DN"];

                        #region CPU

                        var cpuGroup =
                            this.CreateLogicalChildGroup(this.CpuGroupClass, model.DeviceId, x.DN);

                        cpuGroup[childBladeKey].Value = x.DN;
                        cpuGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        discoveryData.Add(cpuGroup);

                        x.CPUList.ForEach(
                            y =>
                                {
                                    var cpu = this.CreateCpu(y);
                                    cpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                                    cpu[childBladeKey].Value = x.DN;
                                    cpu[this.PartGroupKey].Value = childBladeGroupKey;
                                    cpu[this.ComputerKey].Value = model.DeviceId;
                                    cpu[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(cpu);
                                });

                        #endregion

                        #region Memory

                        var memoryGroup = this.CreateLogicalChildGroup(this.MemoryGroupClass, model.DeviceId, x.DN);
                        memoryGroup[childBladeKey].Value = x.DN;
                        memoryGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        discoveryData.Add(memoryGroup);
                        x.MemoryList.ForEach(
                            y =>
                                {
                                    var memory = this.CreateMemory(y);
                                    memory[this.PartChildGroupKey].Value =
                                        memoryGroup[this.PartChildGroupKey].Value;
                                    memory[childBladeKey].Value = x.DN;
                                    memory[this.PartGroupKey].Value = childBladeGroupKey;
                                    memory[this.ComputerKey].Value = model.DeviceId;
                                    memory[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(memory);
                                });

                        #endregion

                        #region Disk

                        var diskGroup =
                            this.CreateLogicalChildGroup(this.DiskGroupClass, model.DeviceId, x.DN);
                        diskGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        diskGroup[childBladeKey].Value = x.DN;
                        discoveryData.Add(diskGroup);
                        x.DiskList.ForEach(
                            y =>
                                {
                                    var disk = this.CreateDisk(y);
                                    disk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                                    disk[childBladeKey].Value = x.DN;
                                    disk[this.PartGroupKey].Value = childBladeGroupKey;
                                    disk[this.ComputerKey].Value = model.DeviceId;
                                    disk[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(disk);
                                });

                        #endregion

                        #region Mezz

                        var mezzGroup =
                            this.CreateLogicalChildGroup(this.MezzGroupClass, model.DeviceId, x.DN);
                        mezzGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        mezzGroup[childBladeKey].Value = x.DN;
                        discoveryData.Add(mezzGroup);
                        x.MezzList.ForEach(
                            y =>
                                {
                                    var mezz = this.CreateMezz(y);
                                    mezz[this.PartChildGroupKey].Value = mezzGroup[this.PartChildGroupKey].Value;
                                    mezz[childBladeKey].Value = x.DN;
                                    mezz[this.PartGroupKey].Value = childBladeGroupKey;

                                    mezz[this.ComputerKey].Value = model.DeviceId;
                                    mezz[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(mezz);
                                });

                        #endregion

                        #region Raid

                        var raidGroup = this.CreateLogicalChildGroup(this.RaidGroupClass, model.DeviceId, x.DN);
                        raidGroup[this.PartGroupKey].Value = childBladeGroupKey;
                        raidGroup[childBladeKey].Value = x.DN;
                        discoveryData.Add(raidGroup);
                        x.RaidList.ForEach(
                            y =>
                                {
                                    var raid = this.CreateRaidControl(y);
                                    raid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                                    raid[childBladeKey].Value = x.DN;
                                    raid[this.PartGroupKey].Value = childBladeGroupKey;
                                    raid[this.ComputerKey].Value = model.DeviceId;
                                    raid[this.HuaweiServerKey].Value = model.DeviceId;
                                    discoveryData.Add(raid);
                                });

                        #endregion
                    });

            #endregion

            if (!this.ExsitsBladeServer(model.DeviceId))
            {
                discoveryData.Commit(this.MontioringConnector);
            }
            else
            {
                discoveryData.Overwrite(this.MontioringConnector);
            }
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
        /// <param name="dn">The dn.</param>
        /// <returns>The <see cref="MonitoringObject" />.</returns>
        public MonitoringObject GetChildBladeServer(string dn)
        {
            return this.GetObject($"DN = '{dn}'", this.ChildBladeClass);
        }

        /// <summary>
        /// The remove child blade.
        /// </summary>
        /// <param name="dn">The dn.</param>
        public void RemoveChildBlade(string dn)
        {
            var existingObject = this.GetChildBladeServer(dn);
            if (existingObject != null)
            {
                var discovery = new IncrementalDiscoveryData();
                discovery.Remove(existingObject);
                discovery.Commit(this.MontioringConnector);
            }
        }

        /// <summary>
        /// Removes the e sight blade.
        /// </summary>
        /// <param name="esightIp">The e sight ip.</param>
        public void RemoveServerFromMGroup(string esightIp)
        {
            this.RemoverServers(this.BladeClass, esightIp);
        }

        /// <summary>
        /// The remover all blade.
        /// </summary>
        public void RemoverAllBlade()
        {
            this.RemoverServers(this.BladeClass);
        }

        /// <summary>
        /// Updates the main with out related.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void UpdateMainWithOutChildBlade(BladeServer model)
        {
            HWLogger.NOTIFICATION_PROCESS.Debug($"Start UpdateMainWithOutChildBlade {model.DeviceId}");
            var oldBlade = this.GetBladeServer(model.DeviceId);
            if (oldBlade == null)
            {
                throw new Exception($"Can not find the server:{model.DeviceId}");
            }

            var propertys = this.BladeClass.PropertyCollection; // 获取到class的属性
            var discoveryData = new IncrementalDiscoveryData();

            oldBlade[propertys["eSight"]].Value = model.ESight;
            oldBlade[propertys["Status"]].Value = model.Status;
            oldBlade[propertys["IPAddress"]].Value = model.IpAddress;
            oldBlade[propertys["Location"]].Value = model.Location;

            // oldBlade[propertys["Vendor"]].Value = "HUAWEI";
            oldBlade[propertys["Manufacturer"]].Value = model.Manufacturer;
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
                        var fan = this.UpdateFan(x);
                        if (fan == null)
                        {
                            var newFan = this.CreateFan(x);
                            newFan[this.PartGroupKey].Value = fanGroup[this.PartGroupKey].Value;
                            newFan[this.ComputerKey].Value = model.DeviceId;
                            newFan[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newFan);
                        }
                        else
                        {
                            discoveryData.Add(fan);
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
                        var switchObj = this.UpdateSwitch(x);
                        if (switchObj == null)
                        {
                            var newSwitch = this.CreateSwitch(x);
                            newSwitch[this.PartGroupKey].Value = switchGroup[this.PartGroupKey].Value;
                            newSwitch[this.ComputerKey].Value = model.DeviceId;
                            newSwitch[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newSwitch);
                        }
                        else
                        {
                            discoveryData.Add(switchObj);
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
                        var psu = this.UpdatePowerSupply(x);
                        if (psu == null)
                        {
                            var newpsu = this.CreatePowerSupply(x);
                            newpsu[this.PartGroupKey].Value = psuGroup[this.PartGroupKey].Value;
                            newpsu[this.ComputerKey].Value = model.DeviceId;
                            newpsu[this.HuaweiServerKey].Value = model.DeviceId;
                            discoveryData.Add(newpsu);
                        }
                        else
                        {
                            discoveryData.Add(psu);
                        }
                    });

            #endregion

            #region Hmm

            var hmmGroup = oldBlade.GetRelatedMonitoringObjects(this.PowerSupplyGroupClass).First();
            discoveryData.Add(hmmGroup);

            var hmm = this.UpdateHmm(model.HmmInfo);
            discoveryData.Add(hmm);

            #endregion

            // var relatedObjects = oldBlade.GetRelatedMonitoringObjects(ChildBladeClass);
            // relatedObjects.ToList().ForEach(x => discoveryData.Add(x));
            discoveryData.Overwrite(this.MontioringConnector);
            HWLogger.NOTIFICATION_PROCESS.Debug($"End UpdateMainWithOutChildBlade {model.DeviceId}");
        }

        /// <summary>
        /// Updates the child blade.
        /// </summary>
        /// <param name="model">The model.</param>
        public void UpdateChildBlade(ChildBlade model)
        {
            HWLogger.NOTIFICATION_PROCESS.Debug("Start UpdateChildBlade");
            var oldObject = this.GetObject($"DN = '{model.DN}'", this.ChildBladeClass);
            if (oldObject == null)
            {
                throw new Exception($"Can not find the child blade server:{model.DN}");
            }

            var propertys = this.ChildBladeClass.PropertyCollection; // 获取到class的属性
            var discoveryData = new IncrementalDiscoveryData();

            var childHighdensityKey = this.ChildBladeClass.PropertyCollection["DN"];

            oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.Status;
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

            oldObject[this.DisplayNameField].Value = model.Name;
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
                        var cpu = this.UpdateCpu(y);
                        if (cpu == null)
                        {
                            var newCpu = this.CreateCpu(y);
                            newCpu[this.PartChildGroupKey].Value = cpuGroup[this.PartChildGroupKey].Value;
                            newCpu[childHighdensityKey].Value = model.DN;
                            newCpu[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newCpu[this.ComputerKey].Value = model.DN;
                            newCpu[this.HuaweiServerKey].Value = model.DN;
                            discoveryData.Add(newCpu);
                        }
                        else
                        {
                            discoveryData.Add(cpu);
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
                        var memory = this.UpdateMemory(y);
                        if (memory == null)
                        {
                            var newMemory = this.CreateMemory(y);
                            newMemory[this.PartChildGroupKey].Value = memoryGroup[this.PartChildGroupKey].Value;
                            newMemory[childHighdensityKey].Value = model.DN;
                            newMemory[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newMemory[this.ComputerKey].Value = model.DN;
                            newMemory[this.HuaweiServerKey].Value = model.DN;
                            discoveryData.Add(newMemory);
                        }
                        else
                        {
                            discoveryData.Add(memory);
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
                        var disk = this.UpdateDisk(y);
                        if (disk == null)
                        {
                            var newDisk = this.CreateDisk(y);
                            newDisk[this.PartChildGroupKey].Value = diskGroup[this.PartChildGroupKey].Value;
                            newDisk[childHighdensityKey].Value = model.DN;
                            newDisk[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newDisk[this.ComputerKey].Value = model.DN;
                            newDisk[this.HuaweiServerKey].Value = model.DN;
                            discoveryData.Add(newDisk);
                        }
                        else
                        {
                            discoveryData.Add(disk);
                        }
                    });

            #endregion

            #region Mezz

            var mezzGroup = oldObject.GetRelatedMonitoringObjects(this.MemoryGroupClass).First();
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
                        var mezz = this.UpdateMezz(y);
                        if (mezz == null)
                        {
                            var newMezz = this.CreateMezz(y);
                            newMezz[this.PartChildGroupKey].Value = mezzGroup[this.PartChildGroupKey].Value;
                            newMezz[childHighdensityKey].Value = model.DN;
                            newMezz[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newMezz[this.ComputerKey].Value = model.DN;
                            newMezz[this.HuaweiServerKey].Value = model.DN;
                            discoveryData.Add(newMezz);
                        }
                        else
                        {
                            discoveryData.Add(mezz);
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
                        var raid = this.UpdateRaidControl(y);
                        if (raid == null)
                        {
                            var newRaid = this.CreateRaidControl(y);
                            newRaid[this.PartChildGroupKey].Value = raidGroup[this.PartChildGroupKey].Value;
                            newRaid[childHighdensityKey].Value = model.DN;
                            newRaid[this.PartGroupKey].Value = oldObject[this.PartGroupKey].Value;
                            newRaid[this.ComputerKey].Value = model.DN;
                            newRaid[this.HuaweiServerKey].Value = model.DN;
                            discoveryData.Add(newRaid);
                        }
                        else
                        {
                            discoveryData.Add(raid);
                        }
                    });

            #endregion

            discoveryData.Overwrite(this.MontioringConnector);
            HWLogger.NOTIFICATION_PROCESS.Debug("End UpdateChildBlade");
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

            obj[this.ComputerKey].Value = model.DeviceId;
            obj[propertys["eSight"]].Value = model.ESight;
            obj[propertys["Status"]].Value = model.Status;
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
        /// <returns>Microsoft.EnterpriseManagement.Common.CreatableEnterpriseManagementObject.</returns>
        private MPObject CreateChildBlade(ChildBlade model)
        {
            var propertys = this.ChildBladeClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.ChildBladeClass); // 实例化一个class

            obj[propertys["DN"]].Value = model.DN;
            obj[propertys["UUID"]].Value = model.UUID;

            obj[propertys["Status"]].Value = model.Status;
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

            obj[this.DisplayNameField].Value = model.Name;

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
            obj[propertys["Status"]].Value = model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState;
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

            obj[propertys["Status"]].Value = model.Status;
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
            obj[propertys["Status"]].Value = model.HealthState;

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
            obj[propertys["Status"]].Value = model.MezzHealthStatus;

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

            obj[propertys["Status"]].Value = model.HealthState;
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
            obj[propertys["Status"]].Value = model.HealthState;

            obj[propertys["Type"]].Value = model.RaidType;
            obj[propertys["DeviceInterface"]].Value = model.InterfaceType;
            obj[propertys["BBUType"]].Value = model.BbuType;
            obj[propertys["BBUPresence"]].Value = "Present";

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
        private MPObject CreateSwitch(HWBoard model)
        {
            var propertys = this.SwitchClass.PropertyCollection; // 获取到class的属性
            var obj = new MPObject(MGroup.Instance, this.SwitchClass); // 实例化一个class

            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["Status"]].Value = model.HealthState;
            obj[propertys["BladeVersion"]].Value = string.Empty;
            obj[propertys["SwitchType"]].Value = string.Empty;
            obj[propertys["ProductName"]].Value = string.Empty;
            obj[propertys["BoardManufacturer"]].Value = model.Manufacturer;
            obj[propertys["BoardPartNumber"]].Value = model.PartNumber;
            obj[propertys["BoardSerialNumber"]].Value = model.SN;
            obj[propertys["BoardManufacturerDate"]].Value = model.ManuTime;

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

            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["CPUInfo"]].Value = model.Model;

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
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.DiskClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.DiskClass.PropertyCollection; // 获取到class的属性

            // obj[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["DiskLocation"]].Value = model.Location;
            oldObject[propertys["DiskSerialNumber"]].Value = string.Empty;
            oldObject[propertys["DiskINterfaceType"]].Value = string.Empty;
            oldObject[propertys["DiskCapcity"]].Value = string.Empty;
            oldObject[propertys["DiskManufacturer"]].Value = string.Empty;

            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the fan.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>MPObject.</returns>
        private MonitoringObject UpdateFan(HWFAN model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.FanClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.FanClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["ControlModel"]].Value = model.ControlModel;
            oldObject[propertys["RotatePercent"]].Value = model.RotatePercent;
            oldObject[propertys["FanSpeed"]].Value = model.Rotate;

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
        private MonitoringObject UpdateHmm(HWHMM model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.HmmClass);
            if (oldObject == null)
            {
                return null;
            }
            var propertys = this.HmmClass.PropertyCollection; // 获取到class的属性

            oldObject[propertys["Status"]].Value = model.Status;

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

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;

            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["Manufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["Frequency"]].Value = model.Frequency;
            oldObject[propertys["Capacity"]].Value = model.Capacity;

            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }

        /// <summary>
        /// Updates the mezz.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MonitoringObject"/>.
        /// </returns>
        private MonitoringObject UpdateMezz(HWMezz model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.MezzClass);
            if (oldObject == null)
            {
                return null;
            }

            var propertys = this.MezzClass.PropertyCollection; // 获取到class的属性

            // oldObject[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.MezzHealthStatus;

            oldObject[propertys["MezzInfo"]].Value = model.MezzInfo;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["MezzLocation"]].Value = model.MezzLocation;
            oldObject[propertys["MezzMac"]].Value = model.MezzMac;

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

            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["PresentState"]].Value = model.PresentState;
            oldObject[propertys["PowerMode"]].Value = model.InputMode;
            oldObject[propertys["PowerRating"]].Value = model.RatePower;
            oldObject[propertys["RuntimePower"]].Value = model.InputPower;

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

            // obj[propertys["UUID"]].Value = model.UUID;
            oldObject[propertys["Status"]].Value = model.HealthState;

            oldObject[propertys["Type"]].Value = model.RaidType;
            oldObject[propertys["DeviceInterface"]].Value = model.InterfaceType;
            oldObject[propertys["BBUType"]].Value = model.BbuType;
            oldObject[propertys["BBUPresence"]].Value = "Present";

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
        private MonitoringObject UpdateSwitch(HWBoard model)
        {
            var oldObject = this.GetObject($"UUID = '{model.UUID}'", this.SwitchClass);
            if (oldObject == null)
            {
                return null;
            }

            var propertys = this.SwitchClass.PropertyCollection; // 获取到class的属性

            oldObject[propertys["Status"]].Value = model.HealthState;
            oldObject[propertys["BladeVersion"]].Value = string.Empty;
            oldObject[propertys["SwitchType"]].Value = string.Empty;
            oldObject[propertys["ProductName"]].Value = string.Empty;
            oldObject[propertys["BoardManufacturer"]].Value = model.Manufacturer;
            oldObject[propertys["BoardPartNumber"]].Value = model.PartNumber;
            oldObject[propertys["BoardSerialNumber"]].Value = model.SN;
            oldObject[propertys["BoardManufacturerDate"]].Value = model.ManuTime;

            oldObject[this.DisplayNameField].Value = model.Name;

            return oldObject;
        }
        #endregion
    }
}