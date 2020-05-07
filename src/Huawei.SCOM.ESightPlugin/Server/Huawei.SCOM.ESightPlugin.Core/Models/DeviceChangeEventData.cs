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
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 12-19-2017
//
// Last Modified By : yayun
// Last Modified On : 12-19-2017
// ***********************************************************************
// <copyright file="DeviceChangeEventData.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The device change event data.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core.Models
{
    using System;
    using CommonUtil;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Server;

    using Microsoft.EnterpriseManagement.Monitoring;

    /// <summary>
    /// The device change event data.
    /// </summary>
    public class DeviceChangeEventData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceChangeEventData" /> class.
        /// Initializes a new instance of the <see cref="EventData" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public DeviceChangeEventData(NedeviceData data, string eSightIp, ServerTypeEnum serverType)
        {
            this.DeviceId = $"{eSightIp}-{data.DeviceId}";

            this.LoggingComputer = eSightIp;
            this.ESightIp = eSightIp;
            this.Message = data.Description;
            this.NedeviceData = data;
        }

        /// <summary>
        ///     Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel => "Device change notification";

        /// <summary>
        ///     Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets the e sight ip.
        /// </summary>
        /// <value>The e sight ip.</value>
        public string ESightIp { get; set; }

        /// <summary>
        ///     Gets or sets the logging computer.
        /// </summary>
        /// <value>The logging computer.</value>
        public string LoggingComputer { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the nedevice data.
        /// </summary>
        /// <value>The nedevice data.</value>
        public NedeviceData NedeviceData { get; set; }

        /// <summary>
        ///  To the custom monitoring event.
        /// </summary>
        /// <returns>CustomMonitoringEvent.</returns>
        public CustomMonitoringEvent ToCustomMonitoringEvent()
        {
            var monitorEvent = new CustomMonitoringEvent(this.DeviceId, 0)
            {
                LoggingComputer = this.LoggingComputer,
                Channel = this.Channel,
                TimeGenerated = DateTime.Now,

                // 默认为info
                LevelId = 4,
                EventData = this.ToEventData()
                // Message = new CustomMonitoringEventMessage(this.Message)
            };
            return monitorEvent;
        }

        /// <summary>
        ///  To the event data.
        /// </summary>
        /// <returns>System.String.</returns>
        private string ToEventData()
        {
            return XmlHelper.SerializeToXmlStr(this.NedeviceData, true);
        }
    }
}