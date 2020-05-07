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
    [Serializable]
    public class HWFAN
    {
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
        /// 转速
        /// </summary>
        [JsonProperty(PropertyName = "rotate")]
        public string Rotate { get; set; }

        private string rotatePercent;


        /// <summary>
        /// 转百分比(%)
        /// </summary>
        [JsonProperty(PropertyName = "rotatePercent")]
        public string RotatePercent
        {
            get
            {
                if (string.IsNullOrEmpty(this.rotatePercent))
                {
                    return "--";
                }
                if (this.rotatePercent.ToLower() == "unknown")
                {
                    return "--";
                }
                if (this.rotatePercent.ToLower() == "255")
                {
                    return "--";
                }
                return this.rotatePercent;
            }

            set
            {
                this.rotatePercent = value;
            }
        }

        /// <summary>
        /// Gets the rotate percent.
        /// </summary>
        /// <param name="serverType">Type of the server.</param>
        /// <returns>System.String.</returns>
        public string GetRotatePercent(string serverType)
        {
            if (serverType == "rack" || serverType == "highdensity")
            {
                if (this.ControlModel == "0")
                {
                    return "--";
                }
            }
            return this.RotatePercent;
        }

        private string _controlModel { get; set; }
        /// <summary>
        /// 风扇模式，含义如下：	“0”：自动	“1”：手动
        /// </summary>
        [JsonProperty(PropertyName = "controlModel")]
        public string ControlModel { 
            get 
            { 
                if (this._controlModel == "0")
                {
                    return "Auto";
                }
                if (this._controlModel == "1")
                {
                    return "Manual";
                }
                return "";
            }
            set { this._controlModel = value; }
        }
        /// <summary>
        ///索引
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }

        /// <summary>
        /// 唯一标识.
        /// </summary>
        /// <value>The UUID.</value>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }


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
