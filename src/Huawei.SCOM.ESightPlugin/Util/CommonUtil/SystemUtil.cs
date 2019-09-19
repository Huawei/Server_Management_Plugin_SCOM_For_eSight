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
using System.Management;
using System.Net;
using System.Text;
using Huawei.SCOM.ESightPlugin.LogUtil;

namespace CommonUtil
{

    /// <summary>
    /// 系统相关工具类
    /// 如： 系统环境，系统IP等。
    /// </summary>
    public class SystemUtil
    {
        /// <summary>
        /// 获得本地计算机IP
        /// </summary>
        /// <returns>本地计算机IP</returns>
        public static string GetLocalhostIP()
        {
            try
            {
                string stringIP = "";
                ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
                foreach (ManagementObject managementObject in managementObjectCollection)
                {
                    if ((bool)managementObject["IPEnabled"] == true)
                    {
                        string[] IPAddresses = (string[])managementObject["IPAddress"];
                        if (IPAddresses.Length > 0)
                        {
                            stringIP = IPAddresses[0];
                        }
                    }
                }
                return stringIP;
            }
            catch (Exception se)
            {
                HWLogger.Default.Warn("GetLocalhostIP", se);
                return "";
            }
        }
    }
}
