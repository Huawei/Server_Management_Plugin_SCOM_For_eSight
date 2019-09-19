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

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    /**
      "switchBoard": [
        {
          "dn": "NE=34603627",       ---------------------------------------交换板DN
          "parentDn": "NE=34603612",
          "ipAddress": "192.168.13.1",
          "name": "switch module1",
          "healthState": "-3",
          "type": "1",
          "sn": "210305618210G4000030",
          "partNumber": "0",
          "manufacture": "Huawei Technologies Co., Ltd.",
          "manuTime": "2016-04-17 12:14:00",
          "presentState": "1",
          "assertTag": "",
          "moId": "42846",
          "uuid": "ChassisServer210230102810G8000334chassisSwitchswitch module1"
        }
        */

    /// <summary>
    /// 板信息，刀片服务器：交换板；机架、高密服务器、刀片：主板；
    /// </summary>
    [Serializable]
    public class HWBoard
    {
        #region field
        private string _presentState { get; set; }

        /// <summary>
        /// The _health state
        /// </summary>
        private string _healthState;

        /// <summary>
        /// The _manufacturer
        /// </summary>
        private string _manufacturer;
        #endregion

        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        [JsonProperty(PropertyName = "dn")]
        public string DN { get; set; }

        /// <summary>
        /// Gets or sets the parent dn.
        /// </summary>
        /// <value>The parent dn.</value>
        [JsonProperty(PropertyName = "parentDn")]
        public string ParentDN { get; set; }

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <value>The ip address.</value>
        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
       
        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        [JsonProperty(PropertyName = "healthState")]
        public string HealthState { get { return _healthState; } set { _healthState = StatusHelper.ConvertStatus(value); } }

        /// <summary>
        /// 单板类型，含义如下：“0”：主板	“1”：交换板
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public int BoardType { get; set; }

        /// <summary>
        /// 单板序列号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "sn")]
        public string SN { get; set; }

        /// <summary>
        /// 单板部件号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "partNumber")]
        public string PartNumber { get; set; }

        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer
        {
            // 华为返回此字段有问题 有时返回manufacturer 有时返回manufacture
            get { return !string.IsNullOrEmpty(_manufacturer) ? _manufacturer : Manufacture; }
            set { _manufacturer = value; }
        }

        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacture")]
        public string Manufacture { get; set; }

        /// <summary>
        /// 制造日期，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manuTime")]
        public string ManuTime { get; set; }


        /// <summary>
        /// 设备在位信息：“0”：不在位 “1”：在位
        /// </summary>
        /// <value>The state of the present.</value>
        [JsonProperty(PropertyName = "presentState")]
        public string PresentState
        {
            get { return StatusHelper.GetPresentState(_presentState); }
            set { _presentState = value; }
        }

        /// <summary>
        /// Gets or sets the assert tag.
        /// </summary>
        /// <value>The assert tag.</value>
        [JsonProperty(PropertyName = "assertTag")]
        public string AssertTag { get; set; }

        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }

        /// <summary>
        /// Gets or sets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

    }
}
