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
    public class HWLogger
    {
        public static Logger Default => LogManager.GetLogger($"Default");

        public static Logger Service => LogManager.GetLogger($"Service");

        public static Logger Install => LogManager.GetLogger($"Install");


        public static Logger UI => LogManager.GetLogger($"UI");

        public static Logger NotifyRecv => LogManager.GetLogger($"NotifyRecv");


        public static Logger GetInstallLogger()
        {
            return LogManager.GetLogger($"Install");
        }

        public static Logger GetESightSdkLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.Sdk");
        }

        public static Logger GetESightNotifyLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.NotifyProcess");
        }

        public static Logger GetESightSubscribeLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.Subscribe");
        }
    }
}
