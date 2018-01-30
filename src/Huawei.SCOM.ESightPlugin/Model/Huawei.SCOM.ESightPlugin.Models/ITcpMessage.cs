// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Models
// Author           : yayun
// Created          : 01-12-2018
//
// Last Modified By : yayun
// Last Modified On : 01-12-2018
// ***********************************************************************
// <copyright file="ITcpMessage.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The TcpMessage interface.</summary>
// ***********************************************************************


namespace Huawei.SCOM.ESightPlugin.Models
{
    /// <summary>
    /// The TcpMessage interface.
    /// </summary>
    public interface ITcpMessage
    {
        /// <summary>
        ///     Gets or sets the e sight ip.
        /// </summary>
        string ESightIp { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     Gets or sets the msg type.
        /// </summary>
        TcpMessageType MsgType { get; set; }
    }
}