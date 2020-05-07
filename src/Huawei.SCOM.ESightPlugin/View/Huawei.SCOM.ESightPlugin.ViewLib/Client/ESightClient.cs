//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Huawei.SCOM.ESightPlugin.ViewLib.Model;
using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.ViewLib.Client
{
    class ESightClient : IDisposable
    {
        #region Field

        /// <summary>
        /// The _disposed
        /// </summary>
        private bool disposed;


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ESightSession"/> class.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        public ESightClient(ESightAppliance eSight)
        {
            this.Appliance = eSight;
            AcceptAllHTTPS();
        }



        /// <summary>
        /// Finalizes an instance of the <see cref="ESightSession"/> class.
        /// </summary>
        ~ESightClient()
        {
            this.ReleaseUnmanagedResources();
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public ESightAppliance Appliance { get; set; }


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
            return $"https://{this.Appliance.Host}:{this.Appliance.Port}/{partUrl}";
        }

        public T ReadAsJsonDataContract<T>(string JSONString)
        {
            try
            {
                DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(JSONString));
                return (T)((object)dataContractJsonSerializer.ReadObject(stream));
            }
            catch (SerializationException)
            {
                throw;
            }
        }
        private static void AcceptAllHTTPS()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            ServicePointManager.DefaultConnectionLimit = 1000;
        }


        /// <summary>
        /// Tests the connection.
        /// <exception cref="ESSessionExpceion">connect faild</exception>
        /// </summary>
        public async Task<Result> TestCredential()
        {
            string url = this.GetFullUrl("rest/openapi/sm/session");
            try
            {
                string payload = string.Format("{{ \"userid\":\"{0}\", \"value\":\"{1}\" }}",
                    this.Appliance.LoginAccount, this.Appliance.LoginPassword);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpClient().PutAsync(url, content).Result;
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failed(Convert.ToInt32(response.StatusCode), $"Failed to communicate with {Appliance.Host} -> {response.ReasonPhrase}.");
                }

                ESightAPIResult result = ReadAsJsonDataContract<ESightAPIResult>(responseContent);
                if (result.code == 0)
                {
                    this.OpenId = result.data;
                    return Result.Done("eSight authentication passed", null);
                }
                else
                {
                    return Result.Failed(result.code, $"Failed to communicate with {Appliance.Host} -> {result.description}.");
                }
            }
            catch (SerializationException ex)
            {
                return Result.Failed(100, $"Failed to connect communicate with {Appliance.Host} -> {ex.Message}", ex);
            }
            catch (AggregateException e)
            {
                List<string> reasons = GetExceptionReasons(e);
                string cause = String.Join(", ", reasons);
                return Result.Failed(100, $"Failed to connect to {Appliance.Host} -> {cause}", e);
            }
        }

        private static List<string> GetExceptionReasons(AggregateException e)
        {
            List<string> reasons = new List<string>();
            e.Handle((x) =>
            {
                //if (x is WebException || x is SocketException || x is IOException)
                //{
                //    reasons.Add(x.Message);
                //    return true;
                //}
                reasons.Add(x.Message);
                return true;
            });
            return reasons;
        }


        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <exception cref="Exception">Exception</exception>
        /// <exception cref="ESSessionExpceion">HW_LOGIN_AUTH</exception>
        public async Task<Result> Logout()
        {
            if (string.IsNullOrEmpty(this.OpenId))
            {
                return Result.Done();
            }

            string url = this.GetFullUrl($"rest/openapi/sm/session?openid={this.OpenId}");
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("openid", this.OpenId);
                var response = httpClient.DeleteAsync(url).Result;
                string responseContent =await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failed(Convert.ToInt32(response.StatusCode), $"Failed to logout from {Appliance.Host} -> {response.ReasonPhrase}.");
                }

                ESightAPIResult result = ReadAsJsonDataContract<ESightAPIResult>(responseContent);
                if (result.code == 0)
                {
                    return Result.Done();
                }
                else
                {
                    return Result.Failed(result.code, $"Failed to logout from {Appliance.Host} -> {result.description}.");
                }
            }
            catch (Exception e)
            {
                return Result.Failed(100, $"Failed to connect to {Appliance.Host} -> {e.Message}.", e);
            }
        }
        #endregion
        

        /// <summary>
        /// Releases the unmanaged resources.
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            if (!this.disposed)
            {
                this.Logout();
            }
            this.disposed = true;
        }
    }


    public class ESightAPIResult
    {
#pragma warning disable IDE1006 // 命名样式
        public int code { get; set; }


        public string description { get; set; }

        public string data { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
