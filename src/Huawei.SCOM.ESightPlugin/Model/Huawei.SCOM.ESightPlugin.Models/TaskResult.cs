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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models
{
    public class TaskResult
    {

        public TaskResult(string message)
        {
            this.Message = message;
        }

        public TaskResult()
        {
            this.SyncResults = new List<SyncResult>();
        }
        public string Message { get; set; }

        public List<SyncResult> SyncResults { get; set; } 
    }

    public class SyncResult
    {
        public Exception Exception { get; set; }

        public string DN { get; set; }

        public string ServerType { get; set; }
        public bool Success { get; set; }
    }
}
