// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 11-21-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-08-2017
// ***********************************************************************
// <copyright file="eSight.ashx.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSight 的摘要说明</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer
{
    using System;
    using System.Threading.Tasks;
    using System.Web;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.WebServer.Helper;
    using Huawei.SCOM.ESightPlugin.WebServer.Model;

    /// <summary>
    ///     eSight 的摘要说明
    /// </summary>
    public class eSight : IHttpHandler
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
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var action = context.Request.QueryString["action"];
            switch (action)
            {
                case "list":
                    var query = new ParamPagingOfQueryESight();
                    query.PageNo = 1;
                    query.PageSize = 100000;

                    query.HostIp = context.Request.Form["hostIP"] ?? string.Empty;
                    var list = ESightHelper.GetList(query);
                    Task.Run(() =>
                    {
                        ESightEngine.Instance.CheckAllESightConnection();
                    });
                    context.Response.Write(JsonUtil.SerializeObject(list).Replace("null", "\"\""));
                    break;
                case "test":
                    var testModel = new HWESightHost();
                    testModel.HostIP = context.Request.Form["hostIP"];
                    testModel.HostPort = Convert.ToInt32(context.Request.Form["hostPort"]);
                    testModel.LoginAccount = context.Request.Form["loginAccount"];
                    testModel.LoginPd = context.Request.Form["LoginPd"];
                    context.Response.Write(JsonUtil.SerializeObject(ESightHelper.Test(testModel)));
                    break;
                case "add":
                    var addModel = new HWESightHost();
                    addModel.AliasName = context.Request.Form["aliasName"];
                    addModel.HostIP = context.Request.Form["hostIP"];
                    addModel.HostPort = Convert.ToInt32(context.Request.Form["hostPort"]);
                    addModel.LoginAccount = context.Request.Form["loginAccount"];
                    addModel.SystemID = context.Request.Form["systemId"];
                    addModel.LoginPd = context.Request.Form["LoginPd"];
                    addModel.SubscripeNeDeviceError = string.Empty;
                    addModel.SubscripeAlarmError = string.Empty;
                    context.Response.Write(JsonUtil.SerializeObject(ESightHelper.Add(addModel)));
                    break;
                case "update":
                    var updateModel = new HWESightHost();
                    updateModel.AliasName = context.Request.Form["aliasName"];
                    updateModel.HostIP = context.Request.Form["hostIP"];
                    updateModel.HostPort = Convert.ToInt32(context.Request.Form["hostPort"]);
                    updateModel.LoginAccount = context.Request.Form["loginAccount"];
                    updateModel.SystemID = context.Request.Form["systemId"];
                    updateModel.LoginPd = context.Request.Form["LoginPd"];
                    if (!string.IsNullOrWhiteSpace(updateModel.LoginAccount))
                    {
                        context.Response.Write(JsonUtil.SerializeObject(ESightHelper.Update(updateModel)));
                    }
                    else
                    {
                        context.Response.Write(JsonUtil.SerializeObject(ESightHelper.UpdateWithOutPass(updateModel)));
                    }
                    break;
                case "delete":
                    var ids = context.Request.Form["ids"];
                    var result = ESightHelper.Delete(ids);
                    context.Response.Write(JsonUtil.SerializeObject(result));
                    break;
                default:
                    var ret = new WebReturnLGResult<HWESightHost>
                    {
                        Code = CoreUtil.GetObjTranNull<int>(
                                          ConstMgr.ErrorCode.SYS_UNKNOWN_ERR),
                        Description = string.Empty
                    };
                    ret.Code = -999999;
                    context.Response.Write(JsonUtil.SerializeObject(ret));
                    break;
            }
        }
    }
}