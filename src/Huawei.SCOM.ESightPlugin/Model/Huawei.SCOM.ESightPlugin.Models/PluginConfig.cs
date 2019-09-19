//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models
{
    public class PluginConfig
    {
        /// <summary>
        /// 轮询间隔
        /// </summary>
        /// <value>The polling interval.</value>
        public int PollingInterval { get; set; }

        /// <summary>
        /// 外网IP
        /// </summary>
        /// <value>The internet ip.</value>
        public string InternetIp { get; set; }

        /// <summary>
        /// 外网端口
        /// </summary>
        /// <value>The internet port.</value>
        public int InternetPort { get; set; }

        /// <summary>
        /// 临时Tcp端口
        /// </summary>
        /// <value>The internet port.</value>
        public int TempTcpPort { get; set; }

        /// <summary>
        /// 判断是否第一次插入事件
        /// </summary>
        /// <value>The internet port.</value>
        public bool IsFirstInsertEvent { get; set; }

        public int ThreadCount { get; set; }
    }
}
