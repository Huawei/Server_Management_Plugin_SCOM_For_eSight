//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Huawei.SCOM.ESightPlugin.WebServer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Huawei.SCOM.ESightPlugin.LogUtil;

namespace Huawei.SCOM.ESightPlugin.WebServer
{
 
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            NotifyClient.Instance.Init();
        }
        
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            RedirectNotificationURL();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 重定向URL，转义subscribeID
        /// </summary>
        protected void RedirectNotificationURL()
        {
            try
            {
                var requestUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                HWLogger.NotifyRecv.Info($"Receive New Request: {requestUrl}");
                if (requestUrl.Contains("AlarmNotification/"))
                {
                    var newUrl = "/AlarmNotification.ashx?subscribeID=" + requestUrl.Substring(requestUrl.LastIndexOf('/') + 1);
                    HttpContext.Current.Server.TransferRequest(newUrl, true);
                }
                else if (requestUrl.Contains("NeDeviceNotification/"))
                {
                    var newUrl = "/NeDeviceNotification.ashx?subscribeID=" + requestUrl.Substring(requestUrl.LastIndexOf('/') + 1);
                    HttpContext.Current.Server.TransferRequest(newUrl, true);
                }
                else if (requestUrl.Contains("SystemKeepAlive/"))
                {
                    var newUrl = "/SystemKeepAlive.ashx?subscribeID=" + requestUrl.Substring(requestUrl.LastIndexOf('/') + 1);
                    HttpContext.Current.Server.TransferRequest(newUrl, true);
                }
            }
            catch (Exception ex)
            {
                HWLogger.NotifyRecv.Error($"RedirectNotificationURL ERROR:", ex);
            }

        }
    }
}