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
// Created          : 02-08-2018
//
// Last Modified By : yayun
// Last Modified On : 02-08-2018
// ***********************************************************************
// <copyright file="AlarmHistory.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The alarm history.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The alarm history.
    /// </summary>
    public class AlarmHistory
    {
        /// <summary>
        /// 是否已确认
        /// </summary>
        /// <value>The acked.</value>
        [JsonProperty(PropertyName = "acked")]
        public bool Acked { get; set; }

        /// <summary>
        /// 确认时间（网管服务器的UTC时间，精确到毫秒），当告警未被确认时为null。
        /// </summary>
        /// <value>The ack time.</value>
        [JsonProperty(PropertyName = "ackTime")]
        public long? AckTime { get; set; }

        /// <summary>
        /// 确认用户
        /// </summary>
        /// <value>The ack user.</value>
        [JsonProperty(PropertyName = "ackUser")]
        public string AckUser { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        /// <value>The additional information.</value>
        [JsonProperty(PropertyName = "additionalInformation")]
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// 附加文本
        /// </summary>
        /// <value>The additional text.</value>
        [JsonProperty(PropertyName = "additionalText")]
        public string AdditionalText { get; set; }

        /// <summary>
        /// 告警分组标识
        /// </summary>
        /// <value>The alarm group identifier.</value>
        [JsonProperty(PropertyName = "alarmGroupId")]
        public int AlarmGroupId { get; set; }

        /// <summary>
        /// 告警标识
        /// </summary>
        /// <value>The alarm identifier.</value>
        [JsonProperty(PropertyName = "alarmId")]
        public int AlarmId { get; set; }

        /// <summary>
        /// 告警名称
        /// </summary>
        /// <value>The name of the alarm.</value>
        [JsonProperty(PropertyName = "alarmName")]
        public string AlarmName { get; set; }

        /// <summary>
        /// 告警流水号
        /// </summary>
        /// <value>The alarm sn.</value>
        [JsonProperty(PropertyName = "alarmSN")]
        public int AlarmSn { get; set; }

        /// <summary>
        /// 告警到达网管时间，为UTC时间，精确到毫秒。
        /// </summary>
        /// <value>The arrived time.</value>
        [JsonProperty(PropertyName = "arrivedTime")]
        public long ArrivedTime { get; set; }

        /// <summary>
        /// 是否已清除
        /// </summary>
        /// <value>The cleared.</value>
        [JsonProperty(PropertyName = "cleared")]
        public bool Cleared { get; set; }

        /// <summary>
        /// 告警清除时间，为UTC时间，精确到毫秒。如果告警被手工清除，该时间为网管服务器的时间；如果告警自动清除，该时间为设备的时间。当告警未被清除时为null。
        /// </summary>
        /// <value>The cleared time.</value>
        [JsonProperty(PropertyName = "clearedTime")]
        public long? ClearedTime { get; set; }

        /// <summary>
        /// 清除类别。可以是如下值之一：
        /// 1：ADAC（Automatically Detected and Automatically Cleared），自动检测自动清除
        /// 2：ADMC（Automatically Detected and Manually Cleared），自动检测手动清除
        /// </summary>
        /// <value>The type of the cleared.</value>
        [JsonProperty(PropertyName = "clearedType")]
        public int ClearedType { get; set; }

        /// <summary>
        /// 告警清除用户
        /// </summary>
        /// <value>The clear user.</value>
        [JsonProperty(PropertyName = "clearUser")]
        public string ClearUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <value>The comments.</value>
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }

        /// <summary>
        /// 备注时间
        /// </summary>
        /// <value>The comment time.</value>
        [JsonProperty(PropertyName = "commentTime")]
        public long CommentTime { get; set; }

        /// <summary>
        /// 备注用户
        /// </summary>
        /// <value>The comment user.</value>
        [JsonProperty(PropertyName = "commentUser")]
        public string CommentUser { get; set; }

        /// <summary>
        /// 设备流水号
        /// </summary>
        /// <value>The dev CSN.</value>
        [JsonProperty(PropertyName = "devCsn")]
        public long? DevCsn { get; set; }

        /// <summary>
        /// 告警产生时间的UTC毫秒数
        /// </summary>
        /// <value>The event time.</value>
        [JsonProperty(PropertyName = "eventTime")]
        public long EventTime { get; set; }

        /// <summary>
        /// 告警类型。可以是如下值之一：
        /// 1：通信告警
        /// 2：设备告警
        /// 3：处理出错告警
        /// 4：业务质量告警
        /// 5：环境告警
        /// 6：完整性告警
        /// 7：操作告警
        /// 8：物理资源告警
        /// 9：安全告警
        /// 10：时间域告警
        /// </summary>
        /// <value>The type of the event.</value>
        [JsonProperty(PropertyName = "eventType")]
        public int EventType { get; set; }

        /// <summary>
        /// 最近一次发生时间，为UTC时间
        /// </summary>
        /// <value>The latest log time.</value>
        [JsonProperty(PropertyName = "latestLogTime")]
        public long LatestLogTime { get; set; }

        /// <summary>
        /// 管理对象
        /// </summary>
        /// <value>The mo dn.</value>
        [JsonProperty(PropertyName = "moDN")]
        public string MoDn { get; set; }

        /// <summary>
        /// 管理对象名称
        /// </summary>
        /// <value>The name of the mo.</value>
        [JsonProperty(PropertyName = "moName")]
        public string MoName { get; set; }

        /// <summary>
        /// 告警源标识
        /// </summary>
        /// <value>The ne dn.</value>
        [JsonProperty(PropertyName = "neDN")]
        public string NeDn { get; set; }

        /// <summary>
        /// 网元名称
        /// </summary>
        /// <value>The name of the ne.</value>
        [JsonProperty(PropertyName = "neName")]
        public string NeName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        /// <value>The type of the ne.</value>
        [JsonProperty(PropertyName = "neType")]
        public string NeType { get; set; }

        /// <summary>
        /// 定位信息
        /// </summary>
        /// <value>The object instance.</value>
        [JsonProperty(PropertyName = "objectInstance")]
        public string ObjectInstance { get; set; }

        /// <summary>
        /// 告警级别。可以是如下值之一：
        /// 0：不确定
        /// 1：紧急
        /// 2：重要
        /// 3：次要
        /// 4：提示
        /// </summary>
        /// <value>The perceived severity.</value>
        [JsonProperty(PropertyName = "perceivedSeverity")]
        public int PerceivedSeverity { get; set; }


        /// <summary>
        /// 告警可能原因，可能为null
        /// </summary>
        /// <value>The probable cause.</value>
        [JsonProperty(PropertyName = "probableCause")]
        public int? ProbableCause { get; set; }

        /// <summary>
        /// 告警可能原因描述信息
        /// </summary>
        /// <value>The probable cause string.</value>
        [JsonProperty(PropertyName = "probableCauseStr")]
        public string ProbableCauseStr { get; set; }

        /// <summary>
        /// 修复建议
        /// </summary>
        /// <value>The proposed repair actions.</value>
        [JsonProperty(PropertyName = "proposedRepairActions")]
        public string ProposedRepairActions { get; set; }
    }

    /// <summary>
    /// Class AlarmHistoryList.
    /// </summary>
    public class AlarmHistoryList
    {
        public AlarmHistoryList()
        {
            this.Data = new List<AlarmHistory>();
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [JsonProperty(PropertyName = "data")]
        public List<AlarmHistory> Data { get; set; }


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }


        /// <summary>
        /// Gets or sets the total page.
        /// </summary>
        /// <value>The total page.</value>
        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }


        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        [JsonProperty(PropertyName = "currentPage")]
        public int CurrentPage { get; set; }
    }
}