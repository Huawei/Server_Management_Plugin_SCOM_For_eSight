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
using NLog;

namespace Huawei.SCOM.ESightPlugin.LogUtil
{
    public class ESightLogger
    {
        public ESightLogger(string eSightIp) 
        {
            this.ESightIp = eSightIp;
        }

        public string ESightIp { get; set; }

        public Logger Polling => LogManager.GetLogger($"{this.ESightIp}.Polling");

        public Logger Subscribe => LogManager.GetLogger($"{this.ESightIp}.Subscribe");

        public Logger NotifyProcess => LogManager.GetLogger($"{this.ESightIp}.NotifyProcess");

        public Logger Api => LogManager.GetLogger($"{this.ESightIp}.Api");

        public Logger Sdk => LogManager.GetLogger($"{this.ESightIp}.Sdk");

        public Logger Alarm => LogManager.GetLogger($"{this.ESightIp}.alarm");

    }
}
