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
using System.Web;

namespace Huawei.SCOM.ESightPlugin.Models
{

    /// <summary>
    /// Class NotifyModel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyModel<T>
    {
        /// <summary>
        /// Gets or sets the subscribe identifier.
        /// </summary>
        /// <value>The subscribe identifier.</value>
        public string SubscribeId { get; set; }

        /// <summary>
        /// 资源类型 URI，告警变更消息通知为 /rest/openapi/notification/common/alarm。
        /// </summary>
        [JsonProperty("resourceURI")]
        public string ResourceURI { get; set; }

        /// <summary>
        /// 消息类型。可能的取值为：1：创建2：删除3：修改
        /// </summary>
        [JsonProperty("msgType")]
        public int MsgType { get; set; }

        /// <summary>
        /// 以字符串表示的 JSON 附加数据对象，此处为空对象 “{}”。
        /// </summary>
        [JsonProperty("extendedData")]
        public string ExtendedData { get; set; }

        /// <summary>
        /// 消息通知描述。
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 消息发出的时间戳，eSight 服务器协调时间格式。
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    /// <summary>
    /// 警告消息Data实体
    /// </summary>
    public class AlarmData
    {
        public AlarmData()
        {
            
        }

        public AlarmData(AlarmHistory data)
        {
            this.OptType = 1;
            this.SystemID = string.Empty;
            this.AckTime = data.AckTime ?? 0;
            this.AckUser = data.AckUser;
            this.Acked = data.Acked;
            this.AdditionalInformation = data.AdditionalInformation;
            this.AdditionalText = data.AdditionalText;
            this.AlarmId = data.AlarmId;
            this.AlarmName = data.AlarmName;
            this.AlarmSN = data.AlarmSn;
            this.ArrivedTime = data.ArrivedTime;
            this.ClearUser = data.ClearUser;
            this.Cleared = data.Cleared;
            this.ClearedTime = data.ClearedTime ?? 0;
            this.ClearedType = data.ClearedType;
            this.CommentTime = data.CommentTime;
            this.CommentUser = data.CommentUser;
            this.Comments = data.Comments;
            this.DevCsn = data.DevCsn ?? 0;
            this.EventTime = data.EventTime;
            this.EventType = data.EventType;
            this.MoDN = data.MoDn;
            this.MoName = data.MoName;
            this.NeDN = data.NeDn;
            this.NeName = data.NeName;
            this.NeType = data.NeType;
            this.ObjectInstance = data.ObjectInstance;
            this.PerceivedSeverity = data.PerceivedSeverity;
            this.ProbableCause = data.ProbableCause ?? 0;
            this.ProbableCauseStr = data.ProbableCauseStr;
            this.ProposedRepairActions = data.ProposedRepairActions;
        }

        /// <summary>
        /// 1-新增告警 2-清除告警 3-确认告警 4-反确认告警 5-变更告警 6-新增事件
        /// </summary>
        [JsonProperty("optType")]
        public int OptType { get; set; }

        /// <summary>
        /// eSight 自己的系统 ID，通过配置文件可以修改。见使用OpenAPI接口前期准备章节。
        /// </summary>
        [JsonProperty("systemID")]
        public string SystemID { get; set; }

        /// <summary>
        /// 确认时间（网管服务器的UTC时间，精确到毫秒），当告警未被确认时为null。
        /// </summary>
        [JsonProperty("ackTime")]
        public long AckTime { get; set; }

        /// <summary>
        /// 确认用户。
        /// </summary>
        [JsonProperty("ackUser")]
        public string AckUser { get; set; }

        /// <summary>
        /// 是否已确认。
        /// </summary>
        [JsonProperty("acked")]
        public bool Acked { get; set; }

        /// <summary>
        /// 附加信息。
        /// </summary>
        [JsonProperty("additionalInformation")]
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// 附加文本
        /// </summary>
        [JsonProperty("additionalText")]
        public string AdditionalText { get; set; }

        /// <summary>
        /// 告警标识
        /// </summary>
        [JsonProperty("alarmId")]
        public int AlarmId { get; set; }

        /// <summary>
        /// 告警名称
        /// </summary>
        [JsonProperty("alarmName")]
        public string AlarmName { get; set; }

        /// <summary>
        /// 告警流水号。
        /// </summary>
        [JsonProperty("alarmSN")]
        public int AlarmSN { get; set; }

        /// <summary>
        /// 告警到达网管时间，为UTC时间，精确到毫秒
        /// </summary>
        [JsonProperty("arrivedTime")]
        public long ArrivedTime { get; set; }

        /// <summary>
        /// 告警清除用户
        /// </summary>
        [JsonProperty("clearUser")]
        public string ClearUser { get; set; }

        /// <summary>
        /// 是否已清除
        /// </summary>
        [JsonProperty("cleared")]
        public bool Cleared { get; set; }

        /// <summary>
        /// 告警清除时间，为UTC时间，精确到毫秒。如果告警被手工清除，该时间为网管服务器的时间；如果告警自动清除，该时间为设备的时间。当告警未被清除时为null。
        /// </summary>
        [JsonProperty("clearedTime")]
        public long ClearedTime { get; set; }

        /// <summary>
        /// 除类别。可以是如下值之一：1：ADAC（Automatically Detected and Automatically Cleared），自动检测自动清除2：ADMC（Automatically Detected and Manually Cleared），自动检测手动清除
        /// </summary>
        [JsonProperty("clearedType")]
        public int ClearedType { get; set; }

        /// <summary>
        /// 备注时间。
        /// </summary>
        [JsonProperty("commentTime")]
        public long CommentTime { get; set; }

        /// <summary>
        /// 备注用户
        /// </summary>
        [JsonProperty("commentUser")]
        public string CommentUser { get; set; }

        /// <summary>
        ///备注
        [JsonProperty("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// 设备流水号
        /// </summary>
        [JsonProperty("devCsn")]
        public long DevCsn { get; set; }

        /// <summary>
        /// 告警产生时间的UTC毫秒数。
        /// </summary>
        [JsonProperty("eventTime")]
        public long EventTime { get; set; }

        /// <summary>
        /// 告警类型。可以是如下值之一：1：通信告警2：设备告警3：处理出错告警4：业务质量告警5：环境告警6：完整性告警7：操作告警8：物理资源告警9：安全告警10：时间域告警
        /// </summary>
        [JsonProperty("eventType")]
        public int EventType { get; set; }

        /// <summary>
        /// 管理对象。
        /// </summary>
        [JsonProperty("moDN")]
        public string MoDN { get; set; }

        /// <summary>
        /// 管理对象名称。
        /// </summary>
        [JsonProperty("moName")]
        public string MoName { get; set; }

        /// <summary>
        /// 告警源标识。
        /// </summary>
        [JsonProperty("neDN")]
        public string NeDN { get; set; }

        /// <summary>
        /// 网元名称。
        /// </summary>
        [JsonProperty("neName")]
        public string NeName { get; set; }

        /// <summary>
        ///设备类型。
        /// </summary>
        [JsonProperty("neType")]
        public string NeType { get; set; }

        /// <summary>
        /// 定位信息。
        /// </summary>
        [JsonProperty("objectInstance")]
        public string ObjectInstance { get; set; }

        /// <summary>
        /// 告警级别。可以是如下值之一：0：不确定 1：紧急 2：重要 3：次要 4：提示 5：已清除
        /// </summary>
        [JsonProperty("perceivedSeverity")]
        public int PerceivedSeverity { get; set; }

        /// <summary>
        /// 告警可能原因，可能为null。划分范围如下：0~9999：OMS/iEMP、10000~19999：i2000、20000~29999：核心网、40000~49999：UC、50000~59999：eSight、60000~999999：第三方告警、1000000~1019999：IT、1020000~1021999：UC(TP)、1022000~1029999：UC(IVS)、1030000~1039999：安全、1040000~1049999：终端管理、1050000~1059999：企业应用管理
        /// </summary>
        [JsonProperty("probableCause")]
        public int ProbableCause { get; set; }

        [JsonProperty("probableCauseStr")]
        public string ProbableCauseStr { get; set; }

        /// <summary>
        ///修复建议。
        /// </summary>
        [JsonProperty("proposedRepairActions")]
        public string ProposedRepairActions { get; set; }

    }

    /// <summary>
    /// 设备消息Data实体
    /// </summary>
    public class NedeviceData
    {
        public NedeviceData()
        {

        }
        /// <summary>
        /// 设备唯一标识（DN）。
        /// </summary>
        /// <value></value>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称。
        /// </summary>
        /// <value></value>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备分类
        /// </summary>
        /// <value></value>
        public string Classification { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        /// <value></value>
        public string DeviceType { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        /// <value></value>
        public string VendorName { get; set; }

        /// <summary>
        /// 设备 IP 地址列表
        /// </summary>
        /// <value></value>
        public string[] DeviceIPList { get; set; }

        /// <summary>
        /// 设备 SYSOID
        /// </summary>
        /// <value></value>
        public string Sysoid { get; set; }

        /// <summary>
        /// 设备时区
        /// </summary>
        /// <value></value>
        public string Timezone { get; set; }

        /// <summary>
        /// 设备软件版本号
        /// </summary>
        /// <value></value>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// 管理状态。可以是如下值之一
        /// </summary>
        /// <value></value>
        public string AdminStatus { get; set; }

        /// <summary>
        /// The connect status
        /// </summary>
        private string connectStatus;

        /// <summary>
        /// 连接状态。可以是如下值之一
        /// </summary>
        /// <value>The connect status.</value>
        public string ConnectStatus
        {
            get
            {
                switch (this.connectStatus)
                {
                    case "0": return "undetected";
                    case "1": return "online";
                    case "2": return "offline";
                    case "3": return "invalid";
                    default: return this.connectStatus;
                }
            }
            set
            {
                this.connectStatus = value;
            }
        }

        /// <summary>
        /// 告警状态。可以是如下值之一
        /// </summary>
        /// <value></value>
        public string AlarmStatus { get; set; }

        /// <summary>
        /// 设备地理位置信息。
        /// </summary>
        /// <value></value>
        public string GeographicalLocation { get; set; }

        /// <summary>
        /// 设备描述或者备注。
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
    }


    /// <summary>
    /// Class KeepAliveData.
    /// </summary>
    public class KeepAliveData
    {
        /// <summary>
        /// 心跳保活消息发送间隔，单位分钟。
        /// 每周期系统检查如果消息缓存为空，则发送保活消息。间隔可以在配置文件中配置，见使用OpenAPI接口前期准备章节。
        /// </summary>
        [JsonProperty("heartBeatInterval")]
        public int HeartBeatInterval { get; set; }

        /// <summary>
        /// eSight 系统名称。
        ///  eSight 系统名称可以在配置文件中配置，见使用OpenAPI接口前期准备章节。
        /// </summary>
        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        /// <summary>
        /// eSight 系统唯一标识。
        /// eSight 系统唯一标识可以在配置文件中配置，见使用OpenAPI接口前期准备章节。
        /// </summary>
        [JsonProperty("systemID")]
        public string SystemID { get; set; }
    }

}