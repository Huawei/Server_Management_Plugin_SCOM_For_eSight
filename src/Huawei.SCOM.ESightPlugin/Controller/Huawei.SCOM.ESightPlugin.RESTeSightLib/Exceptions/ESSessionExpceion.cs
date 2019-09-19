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
// Assembly         : Huawei.SCOM.ESightPlugin.RESTeSightLib
// Author           : yayun
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-04-2018
// ***********************************************************************
// <copyright file="ESSessionExpceion.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSession 错误类</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.RESTeSightLib.Exceptions
{
    using System;

    /// <summary>
    ///     eSession 错误类
    /// </summary>
    [Serializable]
    public class ESSessionExpceion : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ESSessionExpceion"/> class. 
        /// 错误类构造方法
        /// </summary>
        /// <param name="code">
        /// 错误码
        /// </param>
        /// <param name="esSession">
        /// 错误对象
        /// </param>
        /// <param name="message">
        /// 正文
        /// </param>
        public ESSessionExpceion(string code, ESightSession esSession, string message)
            : base(code, esSession, message)
        {
        }
    }
}