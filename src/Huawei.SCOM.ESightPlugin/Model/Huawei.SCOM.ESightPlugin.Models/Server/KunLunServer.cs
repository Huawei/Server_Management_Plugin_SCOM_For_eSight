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
// Assembly         : Huawei.SCOM.ESightPlugin.Models
// Author           : yayun
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-04-2018
// ***********************************************************************
// <copyright file="KunLunServer.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The kun lun server.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System.Collections.Generic;

    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The kun lun server.
    /// </summary>
    public class KunLunServer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KunLunServer" /> class.
        /// </summary>
        public KunLunServer()
        {
            this.DiskList = new List<HWDisk>();
            this.PowerSupplyList = new List<HWPSU>();
            this.FanList = new List<HWFAN>();
            this.BoardList = new List<HWBoard>();
            this.RaidList = new List<HWRAID>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KunLunServer"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public KunLunServer(HWDevice device)
        {
            this.DN = device.DN;
            this.Manufacturer = device.Manufacturer;
            this.ServerName = device.ServerName;
            this.Version = device.Version;

            this.DiskList = new List<HWDisk>();
            this.PowerSupplyList = new List<HWPSU>();
            this.FanList = new List<HWFAN>();
            this.BoardList = new List<HWBoard>();
            this.RaidList = new List<HWRAID>();
        }

        /// <summary>
        /// Gets or sets the bmc mac addr.
        /// </summary>
        public string BmcMacAddr { get; set; }

        /// <summary>
        ///     板信息，刀片服务器：交换板；机架、高密服务器、刀片：主板；
        /// </summary>
        public List<HWBoard> BoardList { get; set; }

        /// <summary>
        ///     cpu总核数
        /// </summary>
        public string CPUCores { get; set; }

        /// <summary>
        ///     cpu数量
        /// </summary>
        public string CPUNums { get; set; }

        /// <summary>
        ///     描述，属性字符串直接显示，非枚举值
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     磁盘信息
        /// </summary>
        public List<HWDisk> DiskList { get; set; }

        /// <summary>
        /// 服务器唯一标识，格式为eSightIp-Dn
        /// "192.168.1.1-NE=xxx"
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 服务器唯一标识，例如：
        /// "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        ///     Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }

        /// <summary>
        ///     风扇信息
        /// </summary>
        public List<HWFAN> FanList { get; set; }

        /// <summary>
        ///     服务器IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        ///     内存容量
        /// </summary>
        public string MemoryCapacity { get; set; }

        /// <summary>
        ///     刀片型号    说明    该字段在查询刀片单板时才返回。
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        ///     设备名称，属性字符串直接显示，非枚举值
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     电源信息
        /// </summary>
        public List<HWPSU> PowerSupplyList { get; set; }

        /// <summary>
        ///     务器序列号，属性字符串直接显示，非枚举值
        /// </summary>
        /// <value>The product sn.</value>
        public string ProductSN { get; set; }

        /// <summary>
        ///     Gets or sets the raid list.
        /// </summary>
        /// <value>The raid list.</value>
        public List<HWRAID> RaidList { get; set; }

        /// <summary>
        ///     Gets or sets the real time power.
        /// </summary>
        /// <value>The real time power.</value>
        public string RealTimePower { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        ///     Gets or sets the SMM mac addr.
        /// </summary>
        /// <value>The SMM mac addr.</value>
        public string SmmMacAddr { get; set; }

        /// <summary>
        ///     服务器状态，含义如下：
        ///     “0”：正常
        ///     “-1”：离线
        ///     “-2”：未知
        ///     其他：故障
        /// </summary>
        public string Status { get; set; }

        public string StatusTxt
        {
            get
            {
                if (Status == "0" || Status == "-3")
                {
                    return "OK";
                }
                else if (Status == "-1")
                {
                    return "Warning";
                }
                else if (Status == "-2")
                {
                    return "Critical";
                }
                else
                {
                    return Status;
                }
            }
        }

        /// <summary>
        ///     设备型号，属性字符串直接显示，非枚举值
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     服务器唯一标识，属性字符串直接显示，非枚举值
        ///     备注：存储型服务器和第三方服务器不支持
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KunLunServer" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void MakeDetail(HWDeviceDetail x, string eSightIp)
        {
            this.ESight = eSightIp;
            this.CPUNums = x.CPUNums;
            this.Description = x.Description;
            this.DN = x.DN;
            this.DeviceId = $"{eSightIp}-{ x.DN}";
            this.IpAddress = x.IpAddress;
            this.MemoryCapacity = x.MemoryCapacity;
            this.Mode = x.Mode;
            this.Name = $"{eSightIp}-{ x.Name}";
            this.ProductSN = x.ProductSN;

            this.Status = x.Status;
            this.Type = x.Type;
            this.UUID = x.UUID;
            this.SmmMacAddr = x.SmmMacAddr;
            this.RealTimePower = x.RealTimePower;
            this.CPUCores = x.CPUCores;
            this.BmcMacAddr = x.BmcMacAddr;
            this.DiskList = x.DiskList ?? new List<HWDisk>();
            this.PowerSupplyList = x.PSUList ?? new List<HWPSU>();
            this.FanList = x.FANList ?? new List<HWFAN>();
            this.BoardList = x.BoardList ?? new List<HWBoard>();
            this.RaidList = x.RaidList ?? new List<HWRAID>();
        }
    }
}