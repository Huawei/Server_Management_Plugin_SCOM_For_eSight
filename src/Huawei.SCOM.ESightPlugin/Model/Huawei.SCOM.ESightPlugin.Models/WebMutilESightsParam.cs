//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huawei.SCOM.ESightPlugin.Models
{
    /// <summary>
    /// 前台选择多个eSight时的参数类，用于反序列化前台提交json
    /// </summary>
    /// <typeparam name="T">具体的对象类型，不同情况可能不同。</typeparam>
    public class WebMutilESightsParam<T>
    {
        /// <summary>
        /// eSight列表
        /// </summary>
        [JsonProperty(PropertyName = "esights")]
        public IList<string> ESights { get; set; }
        /// <summary>
        /// 参数正文，对应提交的具体对象类型。
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
