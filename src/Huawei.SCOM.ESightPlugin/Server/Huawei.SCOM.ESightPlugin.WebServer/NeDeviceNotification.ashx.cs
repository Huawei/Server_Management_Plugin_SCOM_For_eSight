// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 12-11-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-11-2017
// ***********************************************************************
// <copyright file="NeDeviceNotification.ashx.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>接受告警通知处理程序</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;
    using Huawei.SCOM.ESightPlugin.WebServer.Helper;

    using LogUtil;

    /// <summary>
    ///     接受告警通知处理程序
    /// </summary>
    public class NeDeviceNotification : IHttpHandler
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
                var msgType = context.Request.Form["msgType"];
                var data = context.Request.Form["data"];
                var datas = JsonUtil.DeserializeObject<List<NedeviceData>>(data);
                if (!datas.Any())
                {
                    throw new Exception($"The message do not contain \"data\" param .");
                }
                var alarmData = datas.FirstOrDefault();
                if (alarmData == null)
                {
                    throw new Exception($"alarmData is null");
                }
                HWLogger.NOTIFICATION.Info($"url :{url},msgType{msgType}, data:{JsonUtil.SerializeObject(alarmData)}");
                if (string.IsNullOrWhiteSpace(alarmData.DeviceId))
                {
                    throw new Exception($"The message do not contain \"DeviceId\" param.");
                }
                var eSight = ESightDal.Instance.GetEntityBySubscribeId(subscribeId);
                if (eSight == null)
                {
                    HWLogger.NOTIFICATION.Warn($"can not find the eSight,subscribeID:{subscribeId}");
                }
                else
                {
                    var nedevice = new NotifyModel<NedeviceData>
                    {
                        SubscribeId = subscribeId,
                        MsgType = Convert.ToInt32(msgType),
                        ResourceURI = context.Request.Form["resourceURI"],
                        Timestamp = context.Request.Form["timestamp"],
                        Description = context.Request.Form["description"],
                        ExtendedData = context.Request.Form["extendedData"],
                        Data = datas.FirstOrDefault()
                    };
                    Task.Run(() =>
                   {
                       var message = new TcpMessage<NotifyModel<NedeviceData>>(eSight.HostIP, TcpMessageType.NeDevice, nedevice);
                       NotifyClient.Instance.SendMsg(message);
                   });
                }
            }
            catch (Exception ex)
            {
                HWLogger.NOTIFICATION.Error($"NeDevice Notification Error:{url} formData:{formData}", ex);
                context.Response.Write($"NeDevice Notification Error:{ ex } ");
            }
            context.Response.End();
        }
    }
}