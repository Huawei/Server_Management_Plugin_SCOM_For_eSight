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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models
{
    [Serializable]
    public class HWESightHost
    {
        int _hostPort = 32102;

        //[Key]
        //public long ID { get; set; }

        [JsonProperty(PropertyName = "hostIp")]

        public string HostIP { get; set; }
        [JsonProperty(PropertyName = "hostPort")]
        public int HostPort
        {
            get { return _hostPort; }
            set { _hostPort = value; }
        }
        [JsonProperty(PropertyName = "aliasName")]
        public string AliasName
        {
            get; set;
        }
        [JsonProperty(PropertyName = "loginAccount")]
        public string LoginAccount { get; set; }

        [JsonProperty(PropertyName = "loginPd")]
        public string LoginPd { get; set; }

        [JsonIgnore]
        public string CertPath { get; set; }
        [JsonProperty(PropertyName = "latestStatus")]

        public string LatestStatus { get; set; }
        [JsonProperty(PropertyName = "reservedInt1")]

        public int ReservedInt1 { get; set; }
        [JsonProperty(PropertyName = "reservedInt2")]

        public int ReservedInt2 { get; set; }

        [JsonProperty(PropertyName = "systemId")]
        public string SystemID { get; set; }

        [JsonProperty(PropertyName = "openID")]
        public string OpenID { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription alarm status.</value>
        [JsonProperty(PropertyName = "subKeepAliveStatus")]
        public int SubKeepAliveStatus { get; set; }

        /// <summary>
        /// 订阅保活错误信息
        /// </summary>
        /// <value>The subscripe alarm error.</value>
        [JsonProperty(PropertyName = "subKeepAliveError")]
        public string SubKeepAliveError { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription alarm status.</value>
        [JsonProperty(PropertyName = "subscriptionAlarmStatus")]
        public int SubscriptionAlarmStatus { get; set; }

        /// <summary>
        /// 订阅告警错误信息
        /// </summary>
        /// <value>The subscripe alarm error.</value>
        [JsonProperty(PropertyName = "subscripeAlarmError")]
        public string SubscripeAlarmError { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription ne device status.</value>
        [JsonProperty(PropertyName = "subscriptionNeDeviceStatus")]
        public int SubscriptionNeDeviceStatus { get; set; }

        /// <summary>
        /// 订阅设备变更错误信息
        /// </summary>
        /// <value>The subscripe alarm error.</value>
        [JsonProperty(PropertyName = "subscripeNeDeviceError")]
        public string SubscripeNeDeviceError { get; set; }

        /// <summary>
        /// Gets or sets the subscribe identifier.
        /// </summary>
        /// <value>The subscribe identifier.</value>
        [JsonProperty(PropertyName = "subscribeID")]
        public string SubscribeID { get; set; }

        /// <summary>
        /// Gets or sets the last modify time.
        /// </summary>
        /// <value>The last modify time.</value>
        [JsonConverter(typeof(CustomJsonDateTimeConverter))]
        [JsonProperty(PropertyName = "lastModify")]
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// Gets or sets the latest connect information.
        /// </summary>
        /// <value>The latest connect information.</value>
        [JsonProperty(PropertyName = "latestConnectInfo")]
        public string LatestConnectInfo { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        [JsonConverter(typeof(CustomJsonDateTimeConverter))]
        [JsonProperty(PropertyName = "createTime")]
        public DateTime CreateTime { get; set; }


        public string Summary()
        {
            return $"HostIP={this.HostIP},AliasName={this.AliasName},Port={this.HostPort},LoginAccount={this.LoginAccount}";
        }
    }
}
