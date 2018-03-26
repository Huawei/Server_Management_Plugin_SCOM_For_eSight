﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    [Serializable]
    public class HWDeviceDetail
    {
        /// <summary>
        /// 服务器唯一标识，例如：
        ///"NE=xxx"
        /// </summary>
        [JsonProperty(PropertyName = "dn")]
        public string DN { get; set; }

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 设备名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 设备型号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the SMM mac addr.
        /// </summary>
        /// <value>The SMM mac addr.</value>
        [JsonProperty(PropertyName = "smmMacAddr")]
        public string SmmMacAddr { get; set; }

        /// <summary>
        /// Gets or sets the real time power.
        /// </summary>
        /// <value>The real time power.</value>
        [JsonProperty(PropertyName = "realTimePower")]
        public string RealTimePower { get; set; }

        /// <summary>
        /// 务器序列号，属性字符串直接显示，非枚举值
        /// </summary>
        /// <value>The product sn.</value>
        [JsonProperty(PropertyName = "productSn")]
        public string ProductSN { get; set; }

        private string _status;
        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get { return _status; } set { _status = StatusHelper.ConvertStatus(value); } }

        /// <summary>
        /// 描述，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }

        /// <summary>
        /// 服务器唯一标识，属性字符串直接显示，非枚举值
        ///备注：存储型服务器和第三方服务器不支持
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// CPU 信息
        /// </summary>
        [JsonProperty(PropertyName = "CPU")]
        public List<HWCPU> CPUList { get; set; }

        /// <summary>
        /// 内存信息
        /// </summary>
        [JsonProperty(PropertyName = "Memory")]
        public List<HWMemory> MemoryList { get; set; }

        /// <summary>
        /// 磁盘信息
        /// </summary>
        [JsonProperty(PropertyName = "Disk")]
        public List<HWDisk> DiskList { get; set; }

        /// <summary>
        /// 电源信息
        /// </summary>
        [JsonProperty(PropertyName = "PSU")]
        public List<HWPSU> PSUList { get; set; }

        /// <summary>
        /// 风扇信息
        /// </summary>
        [JsonProperty(PropertyName = "FAN")]
        public List<HWFAN> FANList { get; set; }

        /// <summary>
        /// 板信息，刀片服务器：交换板；机架、高密服务器、刀片：主板；
        /// todo 此处接口有变动为 "SwitchBoard" 待确认
        /// </summary>
        [JsonProperty(PropertyName = "board")]
        public List<HWBoard> BoardList { get; set; }


        /// <summary>
        /// Gets or sets the mezz list.
        /// </summary>
        /// <value>The mezz list.</value>
        [JsonProperty(PropertyName = "Mezz")]
        public List<HWMezz> MezzList { get; set; }

        /// <summary>
        /// Gets or sets the raid list.
        /// </summary>
        /// <value>The raid list.</value>
        [JsonProperty(PropertyName = "RAID")]
        public List<HWRAID> RaidList { get; set; }

        /// <summary>
        /// cpu数量
        /// </summary>
        [JsonProperty(PropertyName = "cpuNums")]
        public string CPUNums { get; set; }

        /// <summary>
        /// cpu总核数
        /// </summary>
        [JsonProperty(PropertyName = "cpuCores")]
        public string CPUCores { get; set; }

        /// <summary>
        /// 内存容量
        /// </summary>
        [JsonProperty(PropertyName = "MemoryCapacity")]
        public string MemoryCapacity { get; set; }

        /// <summary>
        /// 刀片型号    说明    该字段在查询刀片单板时才返回。
        /// </summary>
        [JsonProperty(PropertyName = "mode")]
        public string Mode { get; set; }

        /// <summary>
        /// 网卡地址
        /// </summary>
        [JsonProperty(PropertyName = "bmcMacAddr")]
        public string BmcMacAddr { get; set; }
    }
}
