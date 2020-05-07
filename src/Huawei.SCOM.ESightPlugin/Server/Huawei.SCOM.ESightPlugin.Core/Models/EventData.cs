//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 12-19-2017
//
// Last Modified By : yayun
// Last Modified On : 12-19-2017
// ***********************************************************************
// <copyright file="EventData.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core.Models
{
    using System;
    using CommonUtil;
    using Huawei.SCOM.ESightPlugin.Models;
    using Microsoft.EnterpriseManagement.Monitoring;
    using ESightPlugin.Models.Server;
    using System.Diagnostics;

    /// <summary>
    /// Class EventModel.
    /// </summary>
    public class EventData
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EventData" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="serverType">Type of the server.</param>
        public EventData(AlarmData data, string eSightIp)
        {
            this.AlarmData = data;
            this.ESightIp = eSightIp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventData" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        /// <param name="serverType">Type of the server.</param>
        public EventData(AlarmData data, string eSightIp, ServerTypeEnum serverType)
        {
            this.AlarmData = data;
            this.ESightIp = eSightIp;
            this.ServerType = serverType;
        }


        /// <summary>
        /// EventId used as SCOM event-id
        /// </summary>
        public string EventId 
        {
            get { return $"{(int)this.GetLevel()}{this.MantissaNumber}"; }
        }

        /// <summary>
        /// Event source device type.
        /// </summary>
        public ServerTypeEnum ServerType { get; set; }

        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        public string DeviceId
        {
            get
            {
                return $"{this.ESightIp}-{this.AlarmData.NeDN}";
            }
        }

        /// <summary>
        /// Gets or sets the e sight ip.
        /// </summary>
        /// <value>The e sight ip.</value>
        public string ESightIp { get; set; }

        /// <summary>
        /// 1-新增告警 2-清除告警 3-确认告警 4-反确认告警 5-变更告警 6-新增事件
        /// </summary>
        /// <value>The type of the opt.</value>
        public int OptType
        {
            get
            {
                return AlarmData.OptType;
            }

            set
            {
                OptType = value;
            }
        }

        /// <summary>
        /// Gets the alarm data.
        /// </summary>
        /// <value>The alarm data.</value>
        public AlarmData AlarmData 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the alarm sn.
        /// </summary>
        /// <value>The alarm sn.</value>
        public int AlarmSn
        {
            get
            {
                return this.AlarmData.AlarmSN;
            }
        }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel
        {
            get
            {
                return this.AlarmData.AlarmName;
            }
        }

        /// <summary>
        /// 1-Error, 2-Warning, 4-Information, 8-Success Audit, 16-Failure Audit.
        /// </summary>
        /// <value>The level identifier.</value>
        public EventLogEntryType LevelId
        {
            get
            {
                return this.GetLevel();
            }
        }

        /// <summary>
        /// Gets or sets the logging computer.
        /// </summary>
        /// <value>The logging computer.</value>
        public string LoggingComputer
        {
            get
            {
                return this.ESightIp;
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get
            {
                return this.AlarmData.AlarmName;
            }
        }

        public bool Cleared
        {
            get
            {
                return this.AlarmData.Cleared;
            }
        }

        /// <summary>
        /// 0-information, 1-warning, 2-error
        /// </summary>
        /// <value>The severity.</value>
        public int Severity
        {
            get
            {
                if (this.LevelId == EventLogEntryType.Error)
                {
                    return 2;
                }
                if (this.LevelId == EventLogEntryType.Warning)
                {
                    return 1;
                }
                return 0;
            }
        }

        /// <summary>
        /// 0-low, 1-medium, 2-high
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get
            {
                if (this.LevelId == EventLogEntryType.Error)
                {
                    return 2;
                }
                if (this.LevelId == EventLogEntryType.Warning)
                {
                    return 1;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the opt type text.
        /// </summary>
        /// <value>The opt type text.</value>
        public string OptTypeTxt
        {
            get
            {
                switch (this.OptType)
                {
                    case 1: return "add an alarm";
                    case 2: return "clear an alarm";
                    case 5: return "change an alarm";
                    case 6: return "new add an event";
                    default: return this.OptType.ToString();
                }
            }
        }

        /// <summary>
        /// 事件编号的尾数
        /// </summary>
        /// <value>The mantissa number.</value>
        public string MantissaNumber
        {
            get
            {
                switch (this.AlarmSn.ToString().Substring(this.AlarmSn.ToString().Length - 1, 1))
                {
                    case "1":
                    case "6": return "16";
                    case "2":
                    case "7": return "27";
                    case "3":
                    case "8": return "38";
                    case "4":
                    case "9": return "49";
                    case "5":
                    case "0": return "05";
                    default: return "16";
                }
            }
        }

        /// <summary>
        /// To the custom monitoring event.
        /// </summary>
        /// <returns>CustomMonitoringEvent.</returns>
        public CustomMonitoringEvent ToCustomMonitoringEvent()
        {

            // <Description>The {0}（{3}）occur  a alert ({6}) at {2} ,it caused by {4} and we suggest the repair actions is {5}</Description>
            var customMonitoringEvent = new CustomMonitoringEvent(this.DeviceId, this.AlarmSn)
            {
                LoggingComputer = this.LoggingComputer,
                Channel = this.Channel,
                TimeGenerated = DateTime.Now,
                LevelId = Convert.ToInt32(this.LevelId),
                EventData = this.ToEventData(),
                Message = new CustomMonitoringEventMessage(this.Message),
            };

            customMonitoringEvent.Parameters.Add(this.EventId);
            customMonitoringEvent.Parameters.Add(this.Priority.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.AdditionalText ?? string.Empty);
            customMonitoringEvent.Parameters.Add(this.AlarmData.AlarmId.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.AlarmName);
            customMonitoringEvent.Parameters.Add(this.AlarmData.AlarmSN.ToString());
            customMonitoringEvent.Parameters.Add(TimeHelper.StampToDateTime(this.AlarmData.ArrivedTime.ToString()).ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.DevCsn.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.EventType.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.MoName);
            return customMonitoringEvent;
        }


        /**
        public CustomMonitoringEvent ToCustomMonitoringInitEvent()
        {
            var customMonitoringEvent = new CustomMonitoringEvent(this.DeviceId, this.AlarmSn)
            {
                LoggingComputer = this.LoggingComputer,
                Channel = "scom plugin for eSight warning initialization",
                TimeGenerated = DateTime.Now,
                LevelId = 2,
                EventData = this.ToEventData(),
                Message = new CustomMonitoringEventMessage(this.Message),
            };

            customMonitoringEvent.Parameters.Add($"1{this.MantissaNumber}");
            customMonitoringEvent.Parameters.Add("1");
            customMonitoringEvent.Parameters.Add(this.AlarmData.AdditionalText ?? string.Empty);
            customMonitoringEvent.Parameters.Add(this.AlarmData.AlarmId.ToString());
            customMonitoringEvent.Parameters.Add("scom plugin for eSight warning initialization");
            customMonitoringEvent.Parameters.Add(this.AlarmData.AlarmSN.ToString());
            customMonitoringEvent.Parameters.Add(TimeHelper.StampToDateTime(this.AlarmData.ArrivedTime.ToString()).ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.DevCsn.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.EventType.ToString());
            customMonitoringEvent.Parameters.Add(this.AlarmData.MoName);
            return customMonitoringEvent;
        }*/

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private EventLogEntryType GetLevel()
        {
            // When OptType is 2(Clear) or 6(New Event), it's processed BaseConnector#InsertEvent
            // This kind of Event will not be insert into "EventProvider"
            if (OptType == 2 || OptType == 6)
            {
                return EventLogEntryType.Information;
            }

            switch (this.AlarmData.PerceivedSeverity)
            {
                case 1:
                case 2:
                    return EventLogEntryType.Error; // 1：紧急 2：重要
                case 3:
                    return EventLogEntryType.Warning; // 3：次要
                case 0:
                case 4:
                case 5:
                    return EventLogEntryType.Information; // 0：不确定 4：提示 5：已清除
                default:
                    return EventLogEntryType.Information;
            }
        }

        /// <summary>
        /// To the event data.
        /// </summary>
        /// <returns>System.String.</returns>
        private string ToEventData()
        {
            return XmlHelper.SerializeToXmlStr(this.AlarmData, true);
        }

        public string Description
        {
            get
            {
                var eventTimeString = TimeHelper.StampToDateTime(AlarmData.EventTime.ToString()).ToString();
                return $@"Alert ""{AlarmData.AlarmName}"" was reported by ""{AlarmData.ObjectInstance}"" of {DeviceId}({AlarmData.NeType}) at {eventTimeString}.
It's caused by ""{AlarmData.ProbableCauseStr ?? string.Empty}"" and the suggested repair action is ""{AlarmData.ProposedRepairActions ?? string.Empty}"".";
            }
        }
    }
}