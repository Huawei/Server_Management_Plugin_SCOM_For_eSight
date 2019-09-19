//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseException.cs" company="">
//   
// </copyright>
// <summary>
//   自定义错误，基类Exception
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Huawei.SCOM.ESightPlugin.RESTeSightLib.Exceptions
{
    using System;

    /// <summary>
    ///     自定义错误，基类Exception
    /// </summary>
    [Serializable]
    public class BaseException : ApplicationException
    {
        /// <summary>
        /// The _error model.
        /// </summary>
        private string errorModel = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException" /> class.
        /// 错误类构造方法
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="eventSrcObject">错误对象</param>
        /// <param name="message">正文</param>
        public BaseException(string code, object eventSrcObject, string message)
            : base(message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException" /> class.
        /// 错误类构造方法
        /// </summary>
        /// <param name="errorModel">错误模块名</param>
        /// <param name="code">错误码</param>
        /// <param name="eventSrcObject">错误对象</param>
        /// <param name="message">正文</param>
        public BaseException(string errorModel, string code, object eventSrcObject, string message)
            : base(message)
        {
            this.ErrorModel = errorModel;
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        ///     错误码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error model.
        /// </summary>
        public string ErrorModel
        {
            get
            {
                return this.errorModel;
            }

            set
            {
                this.errorModel = value;
            }
        }

        /// <summary>
        ///     错误消息
        /// </summary>
        public string Message { get; set; }
    }
}