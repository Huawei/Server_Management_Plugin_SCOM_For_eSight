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
        public EventData(AlarmData data, string eSightIp, ServerTypeEnum serverType)
        {
            this.AlarmData = data;
            this.OptType = data.OptType;
            if (serverType == ServerTypeEnum.ChildBlade || serverType == ServerTypeEnum.ChildHighdensity)
            {
                this.DeviceId = data.MoDN;
            }
            else
            {
                this.DeviceId = $"{eSightIp}-{data.MoDN}";
            }

            this.AlarmSn = data.AlarmSN;
            this.Channel = data.AlarmName;
            this.LevelId = this.GetLevel(data.PerceivedSeverity, data.OptType);
            this.LoggingComputer = data.MoDN;
            this.Message = data.AlarmName;
            this.CustomData = new CustomData()
            {
                Dn = data.MoDN,
                AlarmSn = data.AlarmSN,
                OptType = this.OptTypeTxt,
                EventTime = TimeHelper.StampToDateTime(data.EventTime.ToString()).ToString(),
                NeType = string.IsNullOrEmpty(data.NeType) ? string.Empty : data.NeType,
                ObjectInstance = string.IsNullOrEmpty(data.ObjectInstance) ? string.Empty : data.ObjectInstance,
                ProposedRepairActions = string.IsNullOrEmpty(data.ProposedRepairActions) ? string.Empty : data.ObjectInstance,
                AdditionalInformation = string.IsNullOrEmpty(data.AdditionalInformation) ? string.Empty : data.ObjectInstance
            };
        }

        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        public string DeviceId { get; set; }

        /// <summary>
        /// 1-新增告警 2-清除告警 3-确认告警 4-反确认告警 5-变更告警 6-新增事件
        /// </summary>
        /// <value>The type of the opt.</value>
        public int OptType { get; set; }

        /// <summary>
        /// Gets the alarm data.
        /// </summary>
        /// <value>The alarm data.</value>
        public AlarmData AlarmData { get; }

        /// <summary>
        /// Gets or sets the alarm sn.
        /// </summary>
        /// <value>The alarm sn.</value>
        public int AlarmSn { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel { get; set; }

        /// <summary>
        /// 1-Error, 2-Warning, 4-Information, 8-Success Audit, 16-Failure Audit.
        /// </summary>
        /// <value>The level identifier.</value>
        public int LevelId { get; set; }

        /// <summary>
        /// Gets or sets the logging computer.
        /// </summary>
        /// <value>The logging computer.</value>
        public string LoggingComputer { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// 0-information, 1-warning, 2-error
        /// </summary>
        /// <value>The severity.</value>
        public int Severity
        {
            get
            {
                if (this.LevelId == 1)
                {
                    return 2;
                }
                if (this.LevelId == 2)
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
                if (this.LevelId == 1)
                {
                    return 2;
                }
                if (this.LevelId == 2)
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
        /// Gets or sets the custom data.
        /// </summary>
        /// <value>The custom data.</value>
        public CustomData CustomData { get; set; }

        /// <summary>
        /// To the custom monitoring event.
        /// </summary>
        /// <returns>CustomMonitoringEvent.</returns>
        public CustomMonitoringEvent ToCustomMonitoringEvent()
        {
            var customMonitoringEvent = new CustomMonitoringEvent(this.DeviceId, this.AlarmSn)
            {
                LoggingComputer = this.LoggingComputer,
                Channel = this.Channel,
                TimeGenerated = DateTime.Now,
                LevelId = this.LevelId,
                EventData = this.ToEventData(),
                Message = new CustomMonitoringEventMessage(this.Message),
            };

            customMonitoringEvent.Parameters.Add($"{this.Severity}{this.MantissaNumber}");
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


        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <param name="perceivedSeverity">The perceived severity.</param>
        /// <param name="optType">Type of the opt.</param>
        /// <returns>System.Int32.</returns>
        private int GetLevel(int perceivedSeverity, int optType)
        {
            if (optType == 2 || optType == 6)
            {
                return 4;
            }
            if (perceivedSeverity == 1 || perceivedSeverity == 2)
            {
                return 1;
            }
            if (perceivedSeverity == 3)
            {
                return 2;
            }
            if (perceivedSeverity == 0 || perceivedSeverity == 4 || perceivedSeverity == 5)
            {
                return 4;
            }
            return 4;
        }

        /// <summary>
        /// To the event data.
        /// </summary>
        /// <returns>System.String.</returns>
        private string ToEventData()
        {
            return XmlHelper.SerializeToXmlStr(this.CustomData, true);
        }
    }
}