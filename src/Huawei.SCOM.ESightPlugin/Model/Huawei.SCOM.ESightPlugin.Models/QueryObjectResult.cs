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
  [Serializable]
  public class QueryObjectResult<T>
  {
    /// <summary>
    /// 操作返回码。可以是如下值之一：
    ///	0：成功
    ///	非0：失败
    /// </summary>
    [JsonProperty(PropertyName = "code")]
    public int Code { get; set; }
    //deploy.error.-1


        /// <summary>
        /// 服务器列表。
        /// </summary>
    [JsonProperty(PropertyName = "data")]
    public T Data { get; set; }

    /// <summary>
    /// 接口调用结果的描述信息。
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }
  }
}
