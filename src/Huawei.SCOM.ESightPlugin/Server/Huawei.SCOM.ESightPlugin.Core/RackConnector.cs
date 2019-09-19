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
                discoveryData.Commit(this.MontioringConnector);
            }
            catch (Exception e)
            {
                HWLogger.GetESightSdkLogger(model.ESight).Error($"Insert (Rack Error:{model.DN}", e);
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
                oldBlade[this.DisplayNameField].Value = model.ServerName;
                if (model.Status != "-3")
                {
                    oldBlade[propertys["Status"]].Value = model.StatusTxt;
                }
                oldBlade[propertys["iBMCIPv4Address"]].Value = model.iBMCIPv4Address;
                oldBlade[propertys["Type"]].Value = model.Type;
                oldBlade[propertys["UUID"]].Value = model.UUID;
                oldBlade[propertys["AveragePower"]].Value = model.AveragePower;
                oldBlade[propertys["PeakPower"]].Value = model.PeakPower;
                oldBlade[propertys["PowerConsumption"]].Value = model.PowerConsumption;
                oldBlade[propertys["DNSServerIP"]].Value = model.DNSServerIP;
                oldBlade[propertys["DNSName"]].Value = model.DNSName;


                oldBlade[propertys["ProductSn"]].Value = model.ProductSN;
                oldBlade[propertys["HostName"]].Value = model.HostName;
                oldBlade[propertys["CPUNums"]].Value = model.CPUNums;
                oldBlade[propertys["CPUCores"]].Value = model.CPUCores;
                oldBlade[propertys["CPUModel"]].Value = model.CPUModel;
                oldBlade[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
                oldBlade[propertys["AssertTag"]].Value = model.AssertTag;
                if (isPolling)
                {
                    oldBlade[propertys["BMCVersion"]].Value = model.BMCVersion;
                }
                oldBlade[propertys["eSight"]].Value = model.ESight;
                discoveryData.Add(oldBlade);
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

            obj[propertys["Status"]].Value = model.StatusTxt;
            obj[propertys["iBMCIPv4Address"]].Value = model.iBMCIPv4Address;
            obj[propertys["Type"]].Value = model.Type;
            obj[propertys["UUID"]].Value = model.UUID;
            obj[propertys["AveragePower"]].Value = model.AveragePower;
            obj[propertys["PeakPower"]].Value = model.PeakPower;
            obj[propertys["PowerConsumption"]].Value = model.PowerConsumption;
            obj[propertys["DNSServerIP"]].Value = model.DNSServerIP;
            obj[propertys["DNSName"]].Value = model.DNSName;
            obj[propertys["ProductSn"]].Value = model.ProductSN;
            obj[propertys["HostName"]].Value = model.HostName;
            obj[propertys["CPUNums"]].Value = model.CPUNums;
            obj[propertys["CPUCores"]].Value = model.CPUCores;
            obj[propertys["CPUModel"]].Value = model.CPUModel;
            obj[propertys["MemoryCapacity"]].Value = model.MemoryCapacity;
            obj[propertys["BMCVersion"]].Value = model.BMCVersion;
            obj[propertys["AssertTag"]].Value = model.AssertTag;
            obj[propertys["ServerName"]].Value = model.ServerName;
            obj[propertys["eSight"]].Value = model.ESight;
            obj[this.DisplayNameField].Value = model.ServerName;
            return obj;
        }

        #endregion

   
    }
}