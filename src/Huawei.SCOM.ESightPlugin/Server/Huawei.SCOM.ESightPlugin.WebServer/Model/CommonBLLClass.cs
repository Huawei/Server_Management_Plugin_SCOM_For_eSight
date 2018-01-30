// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 11-21-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 11-21-2017
// ***********************************************************************
// <copyright file="CommonBLLClass.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>分页参数对象，用于只有页码和页大小两个参数的UI</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer.Model
{
    using Newtonsoft.Json;

    /// <summary>
    ///     分页参数对象，用于只有页码和页大小两个参数的UI
    /// </summary>
    public class ParamOnlyPagingInfo
    {
        /// <summary>
        /// Gets or sets the page no.
        /// </summary>
        [JsonProperty("pageNo")]
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }

    /// <summary>
    ///     分页参数对象，用于查询eSight列表
    /// </summary>
    public class ParamPagingOfQueryESight
    {
        /// <summary>
        /// Gets or sets the host ip.
        /// </summary>
        [JsonProperty("hostIp")]
        public string HostIp { get; set; }

        /// <summary>
        /// Gets or sets the page no.
        /// </summary>
        [JsonProperty("pageNo")]
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }

    /// <summary>
    ///     用于服务器详细信息查询
    /// </summary>
    public class ParamOfQueryDeviceDetail
    {
        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        [JsonProperty("dn")]
        public string DN { get; set; }

        /// <summary>
        /// Gets or sets the e sight ip.
        /// </summary>
        [JsonProperty("ip")]
        public string ESightIP { get; set; }

        /// <summary>
        /// Gets or sets the server type.
        /// </summary>
        [JsonProperty("serverType")]
        public string ServerType { get; set; }
    }
}