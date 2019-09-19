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
    /// <summary>
    /// Class HWPSU.
    /// </summary>
    [Serializable]
    public class HWPSU
    {

        /// <summary>
        /// 唯一标识.
        /// </summary>
        /// <value>The UUID.</value>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 原始状态
        /// </summary>
        /// <value>The ori status.</value>
        [JsonProperty(PropertyName = "healthState")]
        public string OriStatus { get; set; }

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        public string HealthState => StatusHelper.ConvertStatus(OriStatus);

        public string HealthStateTxt
        {
            get
            {
                if (HealthState == "0" || HealthState == "-3")
                {
                    return "OK";
                }
                else if (HealthState == "-1")
                {
                    return "Warning";
                }
                else if (HealthState == "-2")
                {
                    return "Critical";
                }
                else
                {
                    return HealthState;
                }
            }
        }
        /// <summary>
        /// 功耗
        /// </summary>
        [JsonProperty(PropertyName = "inputPower")]
        public string InputPower { get; set; }

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
        /// 电源版本信息，属性字符串直接显示，非枚举值；    备注：有的电源没有版本信息；
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// 电源型号
        /// </summary>
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }


        /// <summary>
        /// 额定功率
        /// </summary>
        [JsonProperty(PropertyName = "ratePower")]
        public string RatePower { get; set; }

        /// <summary>
        /// The input mode
        /// </summary>
        private string inputMode;

        /// <summary>
        /// 输入电源模式：acInput(1) dcInput(2) acInputDcInput(3)
        /// </summary>
        /// <value>The input mode.</value>
        [JsonProperty(PropertyName = "inputMode")]
        public string InputMode
        {
            get
            {
                switch (this.inputMode)
                {
                    case "1": return "AC";
                    case "2": return "DC";
                    case "3": return "AC/DC";
                    default: return this.inputMode;
                }
            }

            set
            {
                this.inputMode = value;
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }

        private string _presentState { get; set; }

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
    }
}
