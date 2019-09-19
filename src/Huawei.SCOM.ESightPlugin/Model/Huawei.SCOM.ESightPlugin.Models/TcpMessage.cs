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
// Created          : 01-05-2018
//
// Last Modified By : yayun
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="TcpMessage.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The tcp messge type.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models
{
    using System;
    using Newtonsoft.Json;
    /// <summary>
    /// The tcp messge type.
    /// </summary>
    public enum TcpMessageType
    {

        /// <summary>
        /// alarm.
        /// </summary>
        Alarm,

        /// <summary>
        /// nedevice.
        /// </summary>
        NeDevice,

        /// <summary>
        /// The keep alive
        /// </summary>
        KeepAlive
    }

    /// <summary>
    ///     The tcp message.
    /// </summary>
    public class TcpMessage<T> : ITcpMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpMessage" /> class.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="msgType">Type of the MSG.</param>
        /// <param name="data">The data.</param>
        public TcpMessage(string subscribeId, TcpMessageType msgType, T data)
        {
            this.Id = Guid.NewGuid().ToString();
            this.SubscribeId = subscribeId;
            this.MsgType = msgType;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the e sight ip.
        /// </summary>
        [JsonProperty("subscribeId")]
        public string SubscribeId { get; set; }

        /// <summary>
        /// Gets or sets the msg type.
        /// </summary>
        [JsonProperty("tcpMessageType")]
        public TcpMessageType MsgType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value>The desc.</value>
        public string Desc => MsgType.ToString();

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public T Data { get; set; }
    }
}