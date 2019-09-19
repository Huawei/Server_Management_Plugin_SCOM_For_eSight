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
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="RackServer.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The rack server.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The rack server.
    /// </summary>
    public class RackServer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RackServer" /> class.
        /// </summary>
        public RackServer()
        {
            this.CPUList = new List<HWCPU>();
            this.BoardList = new List<HWBoard>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RackServer" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public RackServer(HWDevice device)
        {
            this.DN = device.DN;
            this.ServerName = device.ServerName;
            this.iBMCIPv4Address = device.IpAddress;
            this.BMCVersion = device.Version;
            this.CPUList = new List<HWCPU>();
            this.BoardList = new List<HWBoard>();
        }

        /// <summary>
        /// Gets or sets the bmc mac addr.
        /// </summary>
        public string iBMCIPv4Address { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 设备型号，属性字符串直接显示，非枚举值
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 服务器唯一标识，属性字符串直接显示，非枚举值
        /// 备注：存储型服务器和第三方服务器不支持
        /// </summary>
        public string UUID { get; set; }

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
        /// 平均功率
        /// </summary>
        public string AveragePower { get; set; }

        /// <summary>
        /// 峰值功率
        /// </summary>
        public string PeakPower { get; set; }

        /// <summary>
        /// 电源功耗
        /// </summary>
        public string PowerConsumption { get; set; }

        /// <summary>
        /// DNS IP
        /// </summary>
        public string DNSServerIP { get; set; }

        /// <summary>
        /// DNS name
        /// </summary>
        public string DNSName { get; set; }

        /// <summary>
        /// 务器序列号，属性字符串直接显示，非枚举值
        /// </summary>
        /// <value>The product sn.</value>
        public string ProductSN { get; set; }

        /// <summary>
        ///主机名
        /// </summary>
        public string HostName { get; set; }

        //// <summary>
        /// cpu数量
        /// </summary>
        public string CPUNums { get; set; }

        /// <summary>
        ///     cpu总核数
        /// </summary>
        public string CPUCores { get; set; }

        /// <summary>
        /// CPU 信息
        /// </summary>
        public List<HWCPU> CPUList { get; set; }

        /// <summary>
        /// CPU型号
        /// </summary>
        public string CPUModel
        {
            get
            {
                return CPUList.Count > 0 ? string.Join(",", CPUList.Select(x => x.Model).ToArray()) : "";
            }
        }

        /// <summary>
        /// 内存容量
        /// </summary>
        public string MemoryCapacity { get; set; }

        /// <summary>
        /// BMC版本信息
        /// </summary>
        public string BMCVersion { get; set; }

        /// <summary>
        ///     板信息，刀片服务器：交换板；机架、高密服务器、刀片：主板；
        /// </summary>
        public List<HWBoard> BoardList { get; set; }

        /// <summary>
        /// 资产标签
        /// </summary>
        public string AssertTag
        {
            get
            {
                return BoardList.Count > 0 ? string.Join(",", BoardList.Select(x => x.AssertTag).ToArray()) : "";
            }
        }
        /// <summary>
        /// 服务器唯一标识，例如：
        /// "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// 服务器唯一标识，格式为eSightIp-Dn
        /// "192.168.1.1-NE=xxx"
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        ///     Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="RackServer" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void MakeDetail(HWDeviceDetail x, string eSightIp)
        {
            this.ServerName = x.Name;
            this.iBMCIPv4Address = x.IpAddress;
            this.ESight = eSightIp;
            this.Type = x.Type;
            this.UUID = x.UUID;
            this.Status = x.Status;
            this.AveragePower = x.AveragePower;
            this.PeakPower = x.PeakPower;
            this.PowerConsumption = x.PowerConsumption;
            this.DNSServerIP = x.DNSServerIP;
            this.DNSName = x.DNSName;
            this.ProductSN = x.ProductSN;
            this.HostName = x.HostName;
            this.CPUNums = x.CPUNums;
            this.CPUCores = x.CPUCores;
            this.CPUList = x.CPUList ?? new List<HWCPU>();
            this.MemoryCapacity = x.MemoryCapacity;
            //this.BMCVersion = x.BMCVersion;
            this.DN = x.DN;
            this.BoardList = x.BoardList ?? new List<HWBoard>();
            this.DeviceId = $"{eSightIp}-{ x.DN}";
        }
    }
}