// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 11-21-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 11-23-2017
// ***********************************************************************
// <copyright file="ApiResult.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>Ajax请求返回结果</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer.Model
{
    using System.Collections.Generic;

    using Huawei.SCOM.ESightPlugin.Const;

    using Newtonsoft.Json;

    /// <summary>
    ///     Ajax请求返回结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        /// <param name="success">
        /// The success.
        /// </param>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        public ApiResult(bool success, string errCode, string msg, string exmsg)
        {
            this.Code = !success && errCode != "0" || success && errCode == "0"
                            ? errCode
                            : ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
            this.Success = success;
            this.Msg = msg;
            this.ExceptionMsg = exmsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        public ApiResult(string msg)
            : this(true, "0", msg, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        public ApiResult(string errCode, string exmsg)
            : this(false, errCode, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        /// <value>The error code.</value>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        ///     Exception消息.
        /// </summary>
        public string ExceptionMsg { get; set; }

        /// <summary>
        ///     提示消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        ///     成功 失败
        /// </summary>
        public bool Success { get; set; }
    }

    /// <summary>
    /// 请求接口返回结果
    /// </summary>
    /// <typeparam name="T">T
    /// </typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
        /// </summary>
        /// <param name="success">
        /// The success.
        /// </param>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiResult(bool success, string errCode, string msg, string exmsg, T data)
        {
            this.Code = !success && errCode != "0" || success && errCode == "0"
                            ? errCode
                            : ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
            this.Success = success;
            this.Msg = msg;
            this.ExceptionMsg = exmsg;
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiResult(string msg, T data)
            : this(true, "0", msg, string.Empty, data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
        /// </summary>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        public ApiResult(string errCode, string exmsg)
            : this(false, errCode, string.Empty, exmsg, default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiResult(T data)
            : this(true, "0", string.Empty, string.Empty, data)
        {
        }

        /// <summary>
        ///     错误代码
        /// </summary>
        /// <value>The error code.</value>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        ///     返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///     Exception消息.
        /// </summary>
        public string ExceptionMsg { get; set; }

        /// <summary>
        ///     提示消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        ///     成功 失败
        /// </summary>
        public bool Success { get; set; }
    }

    /// <summary>
    /// The api list result.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class ApiListResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiListResult{T}"/> class.
        /// </summary>
        /// <param name="success">
        /// The success.
        /// </param>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiListResult(bool success, string errCode, string msg, string exmsg, IEnumerable<T> data)
        {
            this.Code = !success && errCode != "0" || success && errCode == "0"
                            ? errCode
                            : ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
            this.Success = success;
            this.Msg = msg;
            this.ExceptionMsg = exmsg;
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiListResult{T}"/> class.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiListResult(string msg, IEnumerable<T> data)
            : this(true, "0", msg, string.Empty, data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiListResult{T}"/> class.
        /// </summary>
        /// <param name="errCode">
        /// The err code.
        /// </param>
        /// <param name="exmsg">
        /// The exmsg.
        /// </param>
        public ApiListResult(string errCode, string exmsg)
            : this(false, errCode, string.Empty, exmsg, default(IEnumerable<T>))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiListResult{T}"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public ApiListResult(IEnumerable<T> data)
            : this(true, "0", string.Empty, string.Empty, data)
        {
        }

        /// <summary>
        ///     错误代码
        /// </summary>
        /// <value>The error code.</value>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        ///     返回数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        ///     Exception消息.
        /// </summary>
        public string ExceptionMsg { get; set; }

        /// <summary>
        ///     提示消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        ///     成功 失败
        /// </summary>
        public bool Success { get; set; }
    }
}