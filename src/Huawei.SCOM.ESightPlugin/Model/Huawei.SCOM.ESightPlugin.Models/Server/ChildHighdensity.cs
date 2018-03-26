// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Models
// Author           : yayun
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-04-2018
// ***********************************************************************
// <copyright file="ChildHighdensity.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The child highdensity.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System.Collections.Generic;

    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The child highdensity.
    /// </summary>
    public class ChildHighdensity
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
        ///     Gets or sets the raid list.
        /// </summary>
        /// <value>The raid list.</value>
        private List<HWRAID> raidList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildHighdensity" /> class.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        public ChildHighdensity(string eSight)
        {
            this.ESight = eSight;
            this.CPUList = new List<HWCPU>();
            this.MemoryList = new List<HWMemory>();
            this.DiskList = new List<HWDisk>();
            this.RaidList = new List<HWRAID>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildHighdensity" /> class.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="eSight">The e sight.</param>
        public ChildHighdensity(Blade m, string eSight)
        {
            this.DN = m.DN;
            this.ESight = eSight;
            this.Name = m.Name;
            this.IpAddress = m.IpAddress;
            this.CPUList = new List<HWCPU>();
            this.MemoryList = new List<HWMemory>();
            this.DiskList = new List<HWDisk>();
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
        ///     服务器唯一标识，例如：
        ///     "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }

        /// <summary>
        ///     服务器IP地址
        ///     SCOM:BmcIP
        /// </summary>
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
        /// Gets or sets the product sn.
        /// </summary>
        public string ProductSn { get; set; }

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

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

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
            this.CPUList = detail.CPUList;
            this.DiskList = detail.DiskList;
            this.MemoryList = detail.MemoryList;
            this.RaidList = detail.RaidList;
            this.UUID = detail.UUID;
            this.Status = detail.Status;
            this.Mode = detail.Mode;
            this.Type = detail.Type;
            this.ProductSn = detail.ProductSN;
        }

    }
}