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
// <copyright file="ChildBlade.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The child blade.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System.Collections.Generic;

    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The child blade.
    /// </summary>
    public class ChildBlade
    {
        /// <summary>
        /// The _ cpu list.
        /// </summary>
        private List<HWCPU> cpuList;

        /// <summary>
        ///     磁盘信息
        /// </summary>
        private List<HWDisk> diskList;

        /// <summary>
        ///     内存信息
        /// </summary>
        private List<HWMemory> memoryList;

        /// <summary>
        ///     Gets or sets the mezz list.
        /// </summary>
        /// <value>The mezz list.</value>
        private List<HWMezz> mezzList;

        /// <summary>
        ///     Gets or sets the raid list.
        /// </summary>
        /// <value>The raid list.</value>
        private List<HWRAID> raidList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildBlade" /> class.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        public ChildBlade(string eSight)
        {
            this.ESight = eSight;
            this.CPUList = new List<HWCPU>();
            this.MemoryList = new List<HWMemory>();
            this.DiskList = new List<HWDisk>();
            this.MezzList = new List<HWMezz>();
            this.RaidList = new List<HWRAID>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildBlade" /> class.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="eSight">The e sight.</param>
        public ChildBlade(Blade m, string eSight)
        {
            this.DeviceId = $"{eSight}-{m.DN}";
            this.DN = m.DN;
            this.ESight = eSight;
            this.CPUList = new List<HWCPU>();
            this.MemoryList = new List<HWMemory>();
            this.DiskList = new List<HWDisk>();
            this.MezzList = new List<HWMezz>();
            this.RaidList = new List<HWRAID>();
        }

        /// <summary>
        ///     CPU 信息
        /// </summary>
        public List<HWCPU> CPUList
        {
            get
            {
                return this.cpuList ?? (this.cpuList = new List<HWCPU>());
            }

            set
            {
                this.cpuList = value;
            }
        }

        /// <summary>
        /// Gets or sets the disk list.
        /// </summary>
        public List<HWDisk> DiskList
        {
            get
            {
                return this.diskList ?? (this.diskList = new List<HWDisk>());
            }

            set
            {
                this.diskList = value;
            }
        }

        /// <summary>
        ///  服务器唯一标识，例如：
        ///  "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// 服务器唯一标识，格式为eSightIp-Dn
        /// "192.168.1.1-NE=xxx"
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// eSight Ip
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }

        /// <summary>
        /// 服务器IP地址
        /// SCOM:BmcIP
        /// </summary>
        /// <value>The ip address.</value>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the memory list.
        /// </summary>
        public List<HWMemory> MemoryList
        {
            get
            {
                return this.memoryList ?? (this.memoryList = new List<HWMemory>());
            }

            set
            {
                this.memoryList = value;
            }
        }

        /// <summary>
        /// Gets or sets the mezz list.
        /// </summary>
        public List<HWMezz> MezzList
        {
            get
            {
                return this.mezzList ?? (this.mezzList = new List<HWMezz>());
            }

            set
            {
                this.mezzList = value;
            }
        }

        /// <summary>
        ///     刀片型号
        /// </summary>
        /// <value>The mode.</value>
        public string Mode { get; set; }

        /// <summary>
        ///     刀片名称（管理板IP+刀片槽位）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     能查询到就是在位(暂默认为1)
        /// </summary>
        /// <value>The presence.</value>
        public string Presence => "1";

        /// <summary>
        /// Gets or sets the raid list.
        /// </summary>
        public List<HWRAID> RaidList
        {
            get
            {
                return this.raidList ?? (this.raidList = new List<HWRAID>());
            }

            set
            {
                this.raidList = value;
            }
        }

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
        ///     服务器唯一标识，属性字符串直接显示，非枚举值
        ///     备注：存储型服务器和第三方服务器不支持
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// The make child blade detail.
        /// </summary>
        /// <param name="detail">
        /// The detail.
        /// </param>
        public void MakeChildBladeDetail(HWDeviceDetail detail)
        {
            this.DN = detail.DN;
            this.Name = detail.Name;
            this.IpAddress = detail.IpAddress;
            this.DeviceId = $"{this.ESight}-{ detail.DN}";
            this.CPUList = detail.CPUList;
            this.DiskList = detail.DiskList;
            this.MemoryList = detail.MemoryList;
            this.MezzList = detail.MezzList;
            this.RaidList = detail.RaidList;
            this.UUID = detail.UUID;
            this.Status = detail.Status;
            this.Mode = detail.Mode;
        }
    }
}