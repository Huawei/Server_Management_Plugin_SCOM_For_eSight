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
using CommonUtil;

namespace Huawei.SCOM.ESightPlugin.Models
{
    [Serializable]
    public class QueryPageResult<T>
    {
        /// <summary>
        /// 操作返回码。可以是如下值之一：
        ///	0：成功
        ///	非0：失败
        /// </summary>
        //[JsonProperty(PropertyName = "code")]
        [JsonIgnore]
        public int Code { get; set; }

        
        [JsonProperty(PropertyName = "code")]
        public string RetCode
        {
            get { return ErrorModel + CoreUtil.GetObjTranNull<string>(Code); }
        }
        private string _errorModel = "";

        /// <summary>
        /// deploy.error.
        /// 错误模块
        /// </summary>
        [JsonProperty(PropertyName = "errorModel")]
        public string ErrorModel
        {
            get { return _errorModel; }
            set { _errorModel = value; }
        }
        /// <summary>
        /// 服务器列表。
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public List<T> Data { get; set; }

        /// <summary>
        /// 接口调用结果的描述信息。
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }


        /// <summary>
        /// 符合查询条件的设备记录总数。
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public int TotalSize { get; set; }

        /// <summary>
        /// 分页查询的总页数：size/ pageSize，如果余数大于0说明有未满页的尾页，再+1。
        /// </summary>
        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }

    }
}
