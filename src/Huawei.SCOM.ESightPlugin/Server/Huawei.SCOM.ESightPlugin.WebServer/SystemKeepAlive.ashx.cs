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
using System.Threading.Tasks;
using System.Web;
using CommonUtil;
using Huawei.SCOM.ESightPlugin.LogUtil;
using Huawei.SCOM.ESightPlugin.Models;
using Huawei.SCOM.ESightPlugin.WebServer.Helper;

namespace Huawei.SCOM.ESightPlugin.WebServer
{
    /// <summary>
    /// SystemKeepAlive 的摘要说明
    /// </summary>
    public class SystemKeepAlive : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var url = context.Request.Url;
            var formData = context.Request.Form;
            try
            {
                // todo 校验header中的OpenId
                var subscribeId = context.Request.QueryString["subscribeID"];
                var msgType = context.Request.Form["msgType"];
                var data = context.Request.Form["data"];
                var datas = JsonUtil.DeserializeObject<List<KeepAliveData>>(data);
                if (!datas.Any())
                {
                    throw new Exception($"The message do not contain \"data\" param .");
                }
                HWLogger.NotifyRecv.Info($"url :{url},[msgType:{msgType}], data:{JsonUtil.SerializeObject(datas)}");

                Task.Run(() =>
                {
                    var aliveData = new NotifyModel<KeepAliveData>
                    {
                        SubscribeId = subscribeId,
                        MsgType = Convert.ToInt32(msgType),
                        ResourceURI = context.Request.Form["resourceURI"],
                        Timestamp = context.Request.Form["timestamp"],
                        Description = context.Request.Form["description"],
                        ExtendedData = context.Request.Form["extendedData"],
                        Data = datas.FirstOrDefault()
                    };
                    var message = new TcpMessage<NotifyModel<KeepAliveData>>(subscribeId, TcpMessageType.KeepAlive, aliveData);
                    NotifyClient.Instance.SendMsg(message);
                });

            }
            catch (Exception ex)
            {
                HWLogger.NotifyRecv.Error($"SystemKeepAlive Notification Error:{url} formData:{formData}", ex);
                context.Response.Write($"SystemKeepAlive Notification Error:{ ex } ");
            }
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}