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
// Assembly         : Huawei.SCOM.ESightPlugin.RESTeSightLib
// Author           : yayun
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="ESightSession.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.RESTeSightLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Web;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Devices;
    using Huawei.SCOM.ESightPlugin.Models.Server;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Exceptions;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;
    using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
    using LogUtil;

    /// <summary>
    /// Enum MySecurityProtocolType
    /// </summary>
    [Flags]
    public enum MySecurityProtocolType
    {
        /// <summary>
        ///  Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
        /// </summary>
        Ssl3 = 48,

        /// <summary>
        ///  Specifies the Transport Layer Security (TLS) 1.0 security protocol.
        /// </summary>
        Tls = 192,

        /// <summary>
        ///  Specifies the Transport Layer Security (TLS) 1.1 security protocol.
        /// </summary>
        Tls11 = 768,

        /// <summary>
        ///  Specifies the Transport Layer Security (TLS) 1.2 security protocol.
        /// </summary>
        Tls12 = 3072
    }

    /// <summary>
    /// The http helper.
    /// </summary>
    public class ESightSession : IDisposable
    {
        #region Field
        /// <summary>
        /// eSight服务器,http请求默认超时时间。
        /// </summary>
        private const int DefaultTimeoutSec = 30 * 60;

        /// <summary>
        /// The _disposed
        /// </summary>
        private bool disposed;

        private ESightLogger logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ESightSession"/> class.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        public ESightSession(HWESightHost eSight)
        {
            this.ESight = eSight;
            logger = new ESightLogger(eSight.HostIP);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ESightSession"/> class.
        /// </summary>
        ~ESightSession()
        {
            this.ReleaseUnmanagedResources();
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public HWESightHost ESight { get; set; }

        /// <summary>
        /// 缺省情况下，用户登录后，如果持续30分钟没有操作，openid会自动失效。
        /// </summary>
        /// <value><c>true</c> if this instance is time out; otherwise, <c>false</c>.</value>
        private bool IsTimeOut
        {
            get
            {
                TimeSpan tSpan = DateTime.Now - this.LatestConnectedTime;
                // 加上30秒的偏差。
                if (tSpan.TotalSeconds + 30 > DefaultTimeoutSec)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// LatestConnectedTime
        /// </summary>
        /// <value>The latest connected time.</value>
        private DateTime LatestConnectedTime { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        /// <value>The open identifier.</value>
        private string OpenId { get; set; }

        #endregion

        #region 登录、退出、测试连接

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the full URL.
        /// </summary>
        /// <param name="partUrl">The part URL.</param>
        /// <returns>System.String.</returns>
        public string GetFullUrl(string partUrl)
        {
            return $"https://{this.ESight.HostIP}:{this.ESight.HostPort}/{partUrl}";
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool IsCanConnect()
        {
            try
            {
                this.Login();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tests the connection.
        /// <exception cref="ESSessionExpceion">connect faild</exception>
        /// </summary>
        public void TestConnection()
        {
            string url = this.GetFullUrl("rest/openapi/sm/session");
            try
            {
                logger.Api.Info($"TestConnection url: {url}.");
                var param = new
                {
                    userid = this.ESight.LoginAccount,
                    value = RijndaelManagedCrypto.Instance.DecryptFromCS(ESight.LoginPd),
                    localIp = SystemUtil.GetLocalhostIP()
                };
                var content = new StringContent(JsonUtil.SerializeObject(param), Encoding.UTF8, "application/json");
                this.TrustCertificate();
                var httpClient = new HttpClient();
                var res = httpClient.PutAsync(url, content).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                logger.Api.Info($"TestConnection url: {url}.-result{resultStr}");

                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                this.CheckResult(url, res);
                var result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                if (result.Code != 0)
                {
                    throw new ESSessionExpceion(result.Code.ToString(), this, resultStr);
                }
                this.OpenId = result.Data;
                this.LatestConnectedTime = DateTime.Now;
                logger.Api.Info($"Test Login Success");
            }
            catch (ESSessionExpceion ex)
            {
                logger.Api.Error($"TestConnection Error.Url:{url} : ", ex);
                throw ex;
            }
            catch (AggregateException ae)
            {
                var esSessionExpceion = this.HandleException(ae);
                logger.Api.Error("TestConnection faild", esSessionExpceion);
                throw esSessionExpceion;
            }
        }

        /// <summary>
        /// 登录eSight（并记录状态到eSight）
        /// 登录成功后返回的openid，已经与调用该登录接口的客户端IP地址绑定。
        /// 此openid在有效期内不能在其他IP地址所在的客户端使用，否则认证时会被拒绝。
        /// </summary>
        /// <exception cref="Exception">Login Error: + result</exception>
        public void Login()
        {
            string url = this.GetFullUrl("rest/openapi/sm/session");
            try
            {
                logger.Api.Info($"Login url: {url}.");
                var param = new
                {
                    userid = this.ESight.LoginAccount,
                    value = RijndaelManagedCrypto.Instance.DecryptFromCS(this.ESight.LoginPd),
                    localIp = SystemUtil.GetLocalhostIP()
                };
                var content = new StringContent(JsonUtil.SerializeObject(param), Encoding.UTF8, "application/json");
                this.TrustCertificate();
                var httpClient = new HttpClient();
                var res = httpClient.PutAsync(url, content).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                logger.Api.Info($"Login url: {url}.-result{resultStr}");
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg =
                        $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                this.CheckResult(url, res);
                var result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                if (result.Code != 0)
                {
                    throw new ESSessionExpceion(result.Code.ToString(), this, result.Description);
                }
                this.OpenId = result.Data;
                this.LatestConnectedTime = DateTime.Now;

                // Login
               // ESightDal.Instance.UpdateESightConnectStatus(this.ESight.HostIP, ConstMgr.ESightConnectStatus.ONLINE, "connect success.");
                logger.Api.Info($"Login Success");
            }
            catch (ESSessionExpceion ex)
            {
                //this.HandleEsSessionException(ex);
                logger.Api.Error($"Login Error.Url:{url} : ", ex);
                throw ex;
            }
            catch (AggregateException ae)
            {
                var esSessionExpceion = this.HandleException(ae);
                //this.HandleEsSessionException(esSessionExpceion);
                logger.Api.Error($"Login Error.Url:{url} : ", ae);
                throw esSessionExpceion;
            }
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <exception cref="Exception">Exception</exception>
        /// <exception cref="ESSessionExpceion">HW_LOGIN_AUTH</exception>
        public void Logout()
        {
            if (string.IsNullOrEmpty(this.OpenId))
            {
                return;
            }
            string url = this.GetFullUrl($"rest/openapi/sm/session?openid={this.OpenId}");
            try
            {
                logger.Api.Info($"Logout url: {url}.");
                this.TrustCertificate();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.DeleteAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                this.CheckResult(url, res);
                var result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                if (result.Code != 0)
                {
                    throw new ESSessionExpceion(result.Code.ToString(), this, resultStr);
                }
                logger.Api.Info($"Logout Success.");
            }
            catch (Exception ex)
            {
                logger.Api.Error($"Logout Error.Url:{url} : ", ex);
                throw;
            }
        }

        #endregion

        #region KeepAlive
        public ESightResult SubscribeKeepAlive()
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                if (string.IsNullOrEmpty(this.ESight.SubscribeID))
                {
                    throw new Exception("SubscribeID can not be empty");
                }
                #region param
                var pluginConfig = ConfigHelper.GetPluginConfig();
                if (pluginConfig.InternetIp == "localhost")
                {
                    throw new Exception("Subscription ip can not be localhost");
                }
                var alramUrl = HttpUtility.UrlEncode(
                    $"https://{pluginConfig.InternetIp}:{pluginConfig.InternetPort}/SystemKeepAlive/{this.ESight.SubscribeID}");
                var param = new
                {
                    systemID = HttpUtility.UrlEncode(this.ESight.SystemID),
                    openID = this.ESight.OpenID,
                    url = alramUrl,
                    dataType = "JSON",
                    desc = "ESightSCOM.ESightPlugin"
                };
                #endregion

                string urlAlarm = "rest/openapi/notification/common/systemKeepAlive";
                string url = this.GetFullUrl(urlAlarm);

                var paramJson = JsonUtil.SerializeObject(param);
                logger.Api.Info($"SubscribeKeepAlive url:{url} param: {paramJson}");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var content = new StringContent(paramJson, Encoding.UTF8, "application/json");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.PutAsync(url, content).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                logger.Api.Info($"SubscribeKeepAlive url:{url} result: {resultStr}");
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error("SubscribeKeepAlive Error: ", ex);
            }
            return result;
        }

        public ESightResult UnSubscribeKeepAlive(string systemId)
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                string url = this.GetFullUrl($"rest/openapi/notification/common/systemKeepAlive?systemID={systemId}&desc=ESightSCOM.ESightPlugin");
                logger.Api.Info($"UnSubscribeKeepAlive url:{url} ");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.DeleteAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                logger.Api.Info($"UnSubscribeKeepAlive url:{url} result: {resultStr}");
                if (result.Code != 0)
                {
                    throw new Exception("UnSubscribeKeepAlive Error:" + resultStr);
                }
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error("UnSubscribeKeepAlive Error: ", ex);
            }
            return result;
        }

        #endregion

        #region Subscribe

        /// <summary>
        /// 订阅告警信息
        /// </summary>
        /// <returns>The <see cref="ESightResult" />.</returns>
        /// <exception cref="Exception">SubscribeAlarm Error: + resultStr</exception>
        public ESightResult SubscribeAlarm()
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                if (string.IsNullOrEmpty(this.ESight.SubscribeID))
                {
                    throw new Exception("SubscribeID can not be empty");
                }
                #region param
                var pluginConfig = ConfigHelper.GetPluginConfig();
                if (pluginConfig.InternetIp == "localhost")
                {
                    throw new Exception("Subscription ip can not be localhost");
                }
                var alramUrl = HttpUtility.UrlEncode(
                    $"https://{pluginConfig.InternetIp}:{pluginConfig.InternetPort}/AlarmNotification/{this.ESight.SubscribeID}");
                var param = new
                {
                    systemID = HttpUtility.UrlEncode(this.ESight.SystemID),
                    openID = this.ESight.OpenID,
                    url = alramUrl,
                    dataType = "JSON",
                    desc = "ESightSCOM.ESightPlugin"
                };
                #endregion

                string urlAlarm = "rest/openapi/notification/common/alarm";
                string url = this.GetFullUrl(urlAlarm);

                var paramJson = JsonUtil.SerializeObject(param);
                logger.Api.Info($"SubscribeAlarm url:{url} param: {paramJson}");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var content = new StringContent(paramJson, Encoding.UTF8, "application/json");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.PutAsync(url, content).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                logger.Api.Info($"SubscribeAlarm url:{url} result: {resultStr}");
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error(ex, "SubscribeAlarm Error: ");
            }
            return result;
        }

        /// <summary>
        /// 订阅设备变更信息
        /// </summary>
        /// <returns>Huawei.SCOM.ESightPlugin.Models.NotificationResultModel.</returns>
        /// <exception cref="Exception">Subscription ip can not be localhost
        /// or
        /// or</exception>
        public ESightResult SubscribeNeDevice()
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                if (string.IsNullOrEmpty(this.ESight.SubscribeID))
                {
                    throw new Exception("SubscribeID can not be empty");
                }
                #region param
                var pluginConfig = ConfigHelper.GetPluginConfig();
                if (pluginConfig.InternetIp == "localhost")
                {
                    throw new Exception("Subscription ip can not be localhost");
                }
                var alramUrl = HttpUtility.UrlEncode(
                    $"https://{pluginConfig.InternetIp}:{pluginConfig.InternetPort}/NeDeviceNotification/{this.ESight.SubscribeID}");
                var param = new
                {
                    systemID = HttpUtility.UrlEncode(this.ESight.SystemID),
                    openID = this.OpenId,
                    url = alramUrl,
                    dataType = "JSON",
                    desc = "ESightSCOM.ESightPlugin"
                };
                #endregion

                string urlAlarm = "rest/openapi/notification/common/nedevice";
                string url = this.GetFullUrl(urlAlarm);

                var paramJson = JsonUtil.SerializeObject(param);
                logger.Api.Info($"SubscribeNeDevice url:{url} param: {paramJson}");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var content = new StringContent(paramJson, Encoding.UTF8, "application/json");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.PutAsync(url, content).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                logger.Api.Info($"SubscribeNeDevice url:{url} result: {resultStr}");
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error("SubscribeNeDevice Error: ", ex);
            }
            return result;
        }

        /// <summary>
        /// 取消订阅告警信息
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.Exception">UnSubscribeAlarm Error: + resultStr</exception>
        public ESightResult UnSubscribeAlarm(string systemId)
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                string url = this.GetFullUrl($"rest/openapi/notification/common/alarm?systemID={systemId}&desc=ESightSCOM.ESightPlugin");
                logger.Api.Info($"UnSubscribeAlarm url:{url} ");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.DeleteAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                logger.Api.Info($"UnSubscribeAlarm url:{url} result: {resultStr}");
                if (result.Code != 0)
                {
                    throw new Exception("UnSubscribeAlarm Error:" + resultStr);
                }
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error("UnSubscribeAlarm Error: ", ex);
            }
            return result;
        }

        /// <summary>
        /// 取消订阅设备变更消息
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.Exception">UnSubscribeNeDevice Error: + resultStr</exception>
        /// <exception cref="Exception">UnSubscribeNeDevice Error: + resultStr</exception>
        public ESightResult UnSubscribeNeDevice(string systemId)
        {
            var result = new ESightResult { Code = -1, Description = string.Empty };
            try
            {
                string url = this.GetFullUrl($"rest/openapi/notification/common/nedevice?systemID={systemId}&desc=ESightSCOM.ESightPlugin");
                logger.Api.Info($"UnSubscribeNeDevice url:{url} ");

                this.CheckAndReLogin();
                this.TrustCertificate();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.DeleteAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                {
                    var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                    throw new Exception(errMsg);
                }
                logger.Api.Info($"UnSubscribeNeDevice url:{url} result: {resultStr}");
                result = JsonUtil.DeserializeObject<ESightResult>(resultStr);
                if (result.Code != 0)
                {
                    throw new Exception("UnSubscribeNeDevice Error:" + resultStr);
                }
            }
            catch (Exception ex)
            {
                result.Description = ex.Message;
                logger.Api.Error("UnSubscribeNeDevice Error: ", ex);
            }
            return result;
        }

        /// <summary>
        /// Gets the alarm history.
        /// </summary>
        /// <param name="page">The page of the pagination.</param>
        /// <returns>QueryPageResult&lt;AlarmHistory&gt;.</returns>
        /// <exception cref="ESSessionExpceion">e</exception>
        public AlarmHistoryList GetOpenAlarms(int page)
        {
            var result = new AlarmHistoryList();
            string url = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder($"rest/openapi/alarm?clearStatus=0&ackStatus=0&pageNo={page}&pageSize=100");
                url = this.GetFullUrl(sb.ToString());
                logger.Api.Info($"Get open alarms from esight, request URL: {url}");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var resp = httpClient.GetAsync(url).Result;
                var respContent = resp.Content.ReadAsStringAsync().Result;

                logger.Api.Debug($"Get open alarms from esight successfully. Response content:{respContent}");

                respContent = respContent.Replace("\"data\":\"null\"", "\"data\":[]");
                result = JsonUtil.DeserializeObject<AlarmHistoryList>(respContent);
                if (result.Code != 0)
                {
                    throw new ESSessionExpceion(result.Code.ToString(), this, respContent);
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Description = ex.Message;
                logger.Api.Error(ex, $"Get open alarms from esight failed, request URL: {url}.");
                throw;
            }
            return result;
        }

        #endregion

        #region 服务器列表、服务器详情

        /// <summary>
        /// Gets the server list.
        /// </summary>
        /// <param name="queryDeviceParam">The query device parameter.</param>
        /// <returns>QueryPageResult&lt;HWDevice&gt;.</returns>
        public QueryPageResult<HWDevice> GetServerList(DeviceParam queryDeviceParam)
        {
            var result = new QueryPageResult<HWDevice>();
            string url = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder("rest/openapi/server/device");
                sb.Append("?servertype=").Append(queryDeviceParam.ServerType);
                if (queryDeviceParam.StartPage > 0)
                {
                    sb.Append("&start=").Append(queryDeviceParam.StartPage);
                }
                if (queryDeviceParam.PageSize > 0)
                {
                    sb.Append("&size=").Append(queryDeviceParam.PageSize);
                }
                if (string.IsNullOrEmpty(queryDeviceParam.PageOrder))
                {
                    sb.Append("&orderby=").Append(queryDeviceParam.PageOrder);
                }
                if (string.IsNullOrEmpty(queryDeviceParam.PageOrder))
                {
                    sb.Append("&desc=").Append(queryDeviceParam.OrderDesc);
                }
                url = this.GetFullUrl(sb.ToString());

                //logger.Api.Info($"GetServerList start.Url:{url}");

                this.CheckAndReLogin();
                this.TrustCertificate();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.GetAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                logger.Api.Debug($"GetServerList success.Url:{url} \r\nresult:{resultStr}");
                resultStr = resultStr.Replace("\"data\":\"null\"", "\"data\":[]");
                result = JsonUtil.DeserializeObject<QueryPageResult<HWDevice>>(resultStr);
                if (result.Code != 0)
                {
                    throw new ESSessionExpceion(result.Code.ToString(), this, resultStr);
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Description = ex.Message;
                logger.Api.Error($"GetServerList Error.Url:{url} : ", ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Gets the server details.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns>QueryListResult&lt;HWDeviceDetail&gt;.</returns>
        public HWDeviceDetail GetServerDetails(string dn)
        {
            string url = string.Empty;
            try
            {
                url = this.GetFullUrl($"rest/openapi/server/device/detail?dn={dn}");
                //logger.Api.Info($"GetServerDetails success.Url:{url}");

                this.CheckAndReLogin();
                this.TrustCertificate();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var res = httpClient.GetAsync(url).Result;
                var resultStr = res.Content.ReadAsStringAsync().Result;
                logger.Api.Debug($"GetServerDetails success.Url:{url} \r\nresult:{resultStr}");
                resultStr = resultStr.Replace("\"data\":\"null\"", "\"data\":[]");
                var result = JsonUtil.DeserializeObject<QueryListResult<HWDeviceDetail>>(resultStr);
                if (result.Code != 0)
                {
                    throw new Exception($"GetServerDetails Error. Url:{url} \r\nResult:{ resultStr}");
                }
                if (!result.Data.Any())
                {
                    throw new Exception($"GetServerDetails Error. Url:{url} \r\nResult:{ resultStr}");
                }
                return result.Data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Api.Error($"GetServerDetails Error.Url:{url} : ", ex);
                throw;
            }
        }

        /// <summary>
        /// 查询刀片列表
        /// </summary>
        /// <param name="startPage">The start page.</param>
        /// <returns>刀片列表</returns>
        public ApiServerList<BladeServer> QueryBladeServer(int startPage)
        {
            var result = new ApiServerList<BladeServer>();
            var queryDeviceParam = new DeviceParam() { PageSize = 100, StartPage = startPage, ServerType = "blade" };
            var reqResult = this.GetServerList(queryDeviceParam);
            reqResult.Data.ForEach(x =>
                {
                    BladeServer bladeServer = new BladeServer(x);
                    x.ChildBlades.ForEach(m =>
                    {
                        ChildBlade childBlade = new ChildBlade(m, this.ESight.HostIP);
                        bladeServer.ChildBlades.Add(childBlade);
                    });
                    result.Data.Add(bladeServer);
                });
            result.TotalSize = reqResult.TotalSize;
            result.TotalPage = reqResult.TotalPage;
            return result;
        }

        /// <summary>
        /// 查询高密列表
        /// </summary>
        /// <param name="startPage">The start page.</param>
        /// <returns>高密列表</returns>
        public ApiServerList<HighdensityServer> QueryHighDesentyServer(int startPage)
        {
            var result = new ApiServerList<HighdensityServer>();
            var queryDeviceParam = new DeviceParam() { PageSize = 100, StartPage = startPage, ServerType = "highdensity" };
            var reqResult = this.GetServerList(queryDeviceParam);
            reqResult.Data.ForEach(x =>
                {
                    var highDesentyServer = new HighdensityServer(x);
                    x.ChildBlades.ForEach(m =>
                        {
                            var childHighdensity = new ChildHighdensity(m, this.ESight.HostIP);
                            highDesentyServer.ChildHighdensitys.Add(childHighdensity);
                        });
                    result.Data.Add(highDesentyServer);
                });
            result.TotalSize = reqResult.TotalSize;
            result.TotalPage = reqResult.TotalPage;
            return result;
        }

        /// <summary>
        /// 查询机架列表
        /// </summary>
        /// <param name="startPage">The start page.</param>
        /// <returns>机架列表</returns>
        public ApiServerList<RackServer> QueryRackServer(int startPage)
        {
            var result = new ApiServerList<RackServer>();
            var queryDeviceParam = new DeviceParam() { PageSize = 100, StartPage = startPage, ServerType = "rack" };
            var reqResult = this.GetServerList(queryDeviceParam);
            reqResult.Data.ForEach(x =>
                {
                    RackServer rackServer = new RackServer(x);
                    result.Data.Add(rackServer);
                });
            result.TotalSize = reqResult.TotalSize;
            result.TotalPage = reqResult.TotalPage;
            return result;
        }

        /// <summary>
        /// 查询昆仑列表
        /// </summary>
        /// <param name="startPage">The start page.</param>
        /// <returns>机架列表</returns>
        public ApiServerList<KunLunServer> QueryKunLunServer(int startPage)
        {
            var result = new ApiServerList<KunLunServer>();
            var queryDeviceParam = new DeviceParam() { PageSize = 100, StartPage = startPage, ServerType = "kunlun" };
            var reqResult = this.GetServerList(queryDeviceParam);
            reqResult.Data.ForEach(x =>
                {
                    KunLunServer kunlunServer = new KunLunServer(x);
                    result.Data.Add(kunlunServer);
                });
            result.TotalSize = reqResult.TotalSize;
            result.TotalPage = reqResult.TotalPage;
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 检查openId是否失效并重连
        /// </summary>
        private void CheckAndReLogin()
        {
            if (this.IsTimeOut)
            {
                this.Login();
            }
        }

        /// <summary>
        /// Trusts the certificate.
        /// </summary>
        private void TrustCertificate()
        {
            // 默认忽略证书
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            // 兼容所有ssl协议
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls | MySecurityProtocolType.Ssl3);
            ServicePointManager.DefaultConnectionLimit = 1000;
        }

        /// <summary>
        /// 根据Response解析result结果
        /// </summary>
        /// <param name="url">提交eSight的url</param>
        /// <param name="res">eSight返回的http result</param>
        /// <exception cref="ESSessionExpceion">ESSessionExpceion</exception>
        private void CheckResult(string url, HttpResponseMessage res)
        {
            if (!res.IsSuccessStatusCode)
            {
                string webErrorCode = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                int statusCode = CoreUtil.GetObjTranNull<int>(res.StatusCode);
                if (statusCode >= 400 && statusCode <= 600)
                {
                    webErrorCode = "-50" + statusCode;
                }
                var resultStr = res.Content.ReadAsStringAsync().Result;
                var errMsg = $"Accessing[{url}] ,StatusCode:[{res.StatusCode}],ReasonPhrase:[{res.ReasonPhrase}], Error occurred: [{resultStr}]";
                logger.Api.Error(errMsg);
                throw new ESSessionExpceion(webErrorCode, this, errMsg);
            }
        }

        #region Exception
        /// <summary>
        /// 解析复合Exception里的innerException,返回innerException
        /// </summary>
        /// <param name="ae">复合Exception</param>
        /// <returns>inner Exception 列表</returns>
        private List<Exception> GetFlattenAggregateException(AggregateException ae)
        {
            // Initialize a collection to contain the flattened exceptions.
            List<Exception> flattenedExceptions = new List<Exception>();

            // Create a list to remember all aggregates to be flattened, this will be accessed like a FIFO queue
            List<AggregateException> exceptionsToFlatten = new List<AggregateException>();
            exceptionsToFlatten.Add(ae);
            int nDequeueIndex = 0;

            // Continue removing and recursively flattening exceptions, until there are no more.
            while (exceptionsToFlatten.Count > nDequeueIndex)
            {
                // dequeue one from exceptionsToFlatten
                IList<Exception> currentInnerExceptions = exceptionsToFlatten[nDequeueIndex++].InnerExceptions;

                for (int i = 0; i < currentInnerExceptions.Count; i++)
                {
                    Exception currentInnerException = currentInnerExceptions[i];

                    if (currentInnerException == null)
                    {
                        continue;
                    }

                    AggregateException currentInnerAsAggregate = currentInnerException as AggregateException;

                    // If this exception is an aggregate, keep it around for later.  Otherwise,
                    // simply add it to the list of flattened exceptions to be returned.
                    if (currentInnerAsAggregate != null)
                    {
                        exceptionsToFlatten.Add(currentInnerAsAggregate);
                    }
                    else
                    {
                        flattenedExceptions.Add(currentInnerException);
                        flattenedExceptions.AddRange(this.GetFlattenException(currentInnerException));
                    }
                }
            }
            return flattenedExceptions;
        }

        /// <summary>
        /// 解析Exception里的innerException,返回innerException
        /// </summary>
        /// <param name="se">Exception</param>
        /// <returns>inner Exception 列表</returns>
        private List<Exception> GetFlattenException(Exception se)
        {
            List<Exception> exs = new List<Exception>();

            Exception currentInnerException = se.InnerException;
            if (currentInnerException == null)
            {
                return new List<Exception>();
            }
            else
            {
                exs.Add(currentInnerException);
                exs.AddRange(this.GetFlattenException(currentInnerException));
            }
            return exs;
        }

        /// <summary>
        /// 解析复合Exception里的innerException,返回解析过的内部Exception，方便前台判断。
        /// </summary>
        /// <param name="ae">复合Exception</param>
        /// <returns>解析过的内部Exception，方便前台判断。</returns>
        /// <exception cref="ESSessionExpceion">ESSessionExpceion</exception>
        private ESSessionExpceion HandleException(AggregateException ae)
        {
            StringBuilder sb = new StringBuilder();
            logger.Api.Error(ae);
            List<Exception> flattenedExceptions = this.GetFlattenAggregateException(ae);
            foreach (var ex in flattenedExceptions)
            {
                if (ex is WebException)
                {
                    WebException we = (WebException)ex;
                    logger.Api.Error(we);
                    if (we.Response != null)
                    {
                        HttpWebResponse response = (HttpWebResponse)we.Response;
                        StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                        string backstr = sr.ReadToEnd();
                        sr.Close();
                        response.Close();
                        logger.Api.Error(backstr);
                    }
                }
                sb.AppendLine(ex.Message);
            }
            logger.Api.Error(sb.ToString());
            int errCnt = flattenedExceptions.Count;
            for (int i = errCnt - 1; i >= 0; i--)
            {
                var ex = flattenedExceptions[i];
                // 是否socket连接错误
                if (ex is SocketException)
                {
                    var sex = ex as SocketException;
                    // 是否socket拒绝连接
                    if (sex.NativeErrorCode == 10061)
                    {
                        return new ESSessionExpceion(ConstMgr.ErrorCode.NET_SOCKET_REFUSED, this, ex.Message);
                    }
                    else
                    {
                        return new ESSessionExpceion(ConstMgr.ErrorCode.NET_SOCKET_UNKNOWN, this, ex.Message);
                    }
                }
                else if (ex is WebException)
                {
                    return new ESSessionExpceion(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, this, ex.Message);
                }
            }
            return new ESSessionExpceion(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, this, sb.ToString());
        }
        #endregion

        /// <summary>
        /// Releases the unmanaged resources.
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                try
                {
                    this.Logout();
                }
                catch (Exception se)
                {
                    logger.Api.Warn("There was an error clearing the connection", se);
                }
                // If disposing is false, only the following code is executed.
            }
            this.disposed = true;
        }

        #endregion
    }
}