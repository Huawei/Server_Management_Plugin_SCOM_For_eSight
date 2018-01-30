// ***********************************************************************
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

    /// <summary>
    /// The tcp messge type.
    /// </summary>
    public enum TcpMessageType
    {
        /// <summary>
        /// SyncESight
        /// </summary>
        SyncESight,

        /// <summary>
        /// alarm.
        /// </summary>
        Alarm,

        /// <summary>
        /// nedevice.
        /// </summary>
        NeDevice,

        /// <summary>
        ///  delete a esight
        /// </summary>
        DeleteESight
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
        public TcpMessage(string hostIp, TcpMessageType msgType, T data)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ESightIp = hostIp;
            this.MsgType = msgType;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the e sight ip.
        /// </summary>
        public string ESightIp { get; set; }

        /// <summary>
        /// Gets or sets the msg type.
        /// </summary>
        public TcpMessageType MsgType { get; set; }
    }
}