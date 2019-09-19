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
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-04-2018
// ***********************************************************************
// <copyright file="LoginResult.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>Class LoginResult.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models
{
    using Newtonsoft.Json;

    /// <summary>
    ///     Class LoginResult.
    /// </summary>
    public class ESightResult
    {
        /// <summary>
        /// 操作返回码。可以是如下值之一：
        /// 0：成功
        /// 非0：失败
        /// </summary>
        /// <value>The code.</value>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        /// <summary>
        ///  接口调用结果的描述信息。
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the open identifier.
        /// </summary>
        /// <value>The open identifier.</value>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}