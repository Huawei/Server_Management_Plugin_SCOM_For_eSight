//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    [Serializable]
    public class HWDevice
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
        /// 服务器名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "serverName")]
        public string ServerName { get; set; }

        /// <summary>
        /// 服务器型号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "serverModel")]
        public string ServerModel { get; set; }

        /// <summary>
        /// 服务器序列号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "productSN")]
        public string ProductSN { get; set; }

        /// <summary>
        /// 服务器位置信息，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// 服务器描述，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// 服务器唯一标识，属性字符串直接显示，非枚举值
        ///备注：存储型服务器和第三方服务器不支持
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 原始状态
        /// </summary>
        /// <value>The ori status.</value>
        [JsonProperty(PropertyName = "status")]
        public string OriStatus { get; set; }

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        public string Status => StatusHelper.ConvertStatus(OriStatus);

        private string _manufacturer;
        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer
        {
            //华为返回此字段有问题 有时返回manufacturer 有时返回manufacture
            get { return !string.IsNullOrEmpty(_manufacturer) ? _manufacturer : Manufacture; }
            set { _manufacturer = value; }
        }

        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacture")]
        public string Manufacture { get; set; }

        /// <summary>
        /// BMC版本信息。
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        /// <summary>
        /// 刀片服务器或者高密服务器的刀片列表，其他服务器类型此字段为null；
        ///获取刀片详细信息需要通过“查询服务器详细信息”接口查询。
        /// </summary>
        [JsonProperty(PropertyName = "childBlades")]
        public List<Blade> ChildBlades { get; set; }
    }
}
