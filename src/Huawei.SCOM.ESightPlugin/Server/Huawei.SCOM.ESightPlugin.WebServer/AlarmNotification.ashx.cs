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
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 12-12-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="AlarmNotification.ashx.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>接受告警通知处理程序</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.WebServer.Helper;

    using LogUtil;

    /// <summary>
    ///  接受告警通知处理程序
    /// </summary>
    public class AlarmNotification : IHttpHandler
    {
        /// <summary>
        /// The is reusable.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="Exception">ex
        /// </exception>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var url = context.Request.Url;
            var formData = context.Request.Form;
            try
            {
                // todo 校验header中的OpenId

                var subscribeId = context.Request.QueryString["subscribeID"];
                var data = context.Request.Form["data"];
                var msgType = context.Request.Form["msgType"];

                var datas = JsonUtil.DeserializeObject<List<AlarmData>>(data);
                if (!datas.Any())
                {
                    throw new Exception($"The message do not contain data param.");
                }
                var alarmData = datas.FirstOrDefault();
                if (alarmData == null)
                {
                    throw new Exception($"alarmData is null");
                }
                HWLogger.NotifyRecv.Info($"url :{url},[msgType:{msgType}], data:{JsonUtil.SerializeObject(alarmData)}");
                if (string.IsNullOrWhiteSpace(alarmData.NeDN))
                {
                    throw new Exception($"The message do not contain DN param.");
                }
                if (alarmData.OptType == 3 || alarmData.OptType == 4 || alarmData.OptType == 6)
                {
                    // 过滤掉3：确认告警4：反确认告警 6：新增事件，
                    HWLogger.NotifyRecv.Info($"alarmData OptType is {alarmData.OptType}. Do not need handle");
                }
                else
                {
                    var alarm = new NotifyModel<AlarmData>
                    {
                        SubscribeId = subscribeId,
                        ResourceURI = context.Request.Form["resourceURI"],
                        MsgType = Convert.ToInt32(msgType),
                        Timestamp = context.Request.Form["timestamp"],
                        Description = context.Request.Form["description"],
                        ExtendedData = context.Request.Form["extendedData"],
                        Data = alarmData
                    };

                    Task.Run(() =>
                    {
                        var message = new TcpMessage<NotifyModel<AlarmData>>(subscribeId, TcpMessageType.Alarm, alarm);
                        NotifyClient.Instance.SendMsg(message);
                    });
                }



            }
            catch (Exception ex)
            {
                HWLogger.NotifyRecv.Error($"Alarm Notification Error:{url} formData:{formData}", ex);
                context.Response.Write($"Alarm Notification Error: { ex }");
            }
            context.Response.End();
        }
    }
}