//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyClient.cs" company="">
//   
// </copyright>
// <summary>
//   The notify client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Huawei.SCOM.ESightPlugin.WebServer.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using CommonUtil;
    using Huawei.SCOM.ESightPlugin.Models;
    using LogUtil;

    /// <summary>
    ///     The notify client.
    /// </summary>
    public class NotifyClient
    {
        /// <summary>
        /// The client
        /// </summary>
        private static TcpClient client;

        /// <summary>
        /// 为保证线程安全，使用一个锁来保护_task的访问
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The wait handle
        /// </summary>
        private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static NotifyClient Instance => SingletonProvider<NotifyClient>.Instance;

        /// <summary>
        /// Gets or sets the dn queue.
        /// </summary>
        /// <value>The message queue.</value>
        public Queue<ITcpMessage> MessageQueue { get; set; }

        /// <summary>
        /// The remote TCP port
        /// </summary>
        /// <value>The remote TCP port.</value>
        private int RemoteTcpPort
        {
            get
            {
                var config = ConfigHelper.GetPluginConfig();
                return config.TempTcpPort;
            }
        }

        /// <summary>
        /// The init.
        /// </summary>
        public void Init()
        {
            HWLogger.NotifyRecv.Info($"Init NotifyClient");
            this.MessageQueue = new Queue<ITcpMessage>();
            var worker = new Thread(this.DoJob);
            worker.Start();
        }

        /// <summary>
        /// Sends the MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void SendMsg(ITcpMessage msg)
        {
            this.MessageQueue.Enqueue(msg);
            this.waitHandle.Set();
        }

        /// <summary>
        /// The do job.
        /// </summary>
        private void DoJob()
        {
            while (this.MessageQueue.Count > 0 || this.waitHandle.WaitOne())
            {
                lock (this.locker)
                {
                    if (this.MessageQueue.Count > 0)
                    {
                        try
                        {
                            var msg = this.MessageQueue.Dequeue(); // 有任务时，出列任务
                            HWLogger.NotifyRecv.Debug($" MSG ID:{msg?.Id}");
                            var json = JsonUtil.SerializeObject(msg);
                            HWLogger.NotifyRecv.Debug($"SendTcpMsg :{json}");
                            if (client == null || !client.Connected)
                            {
                                client = new TcpClient("127.0" + ".0.1", this.RemoteTcpPort);
                            }
                            var data = Encoding.UTF8.GetBytes(json);
                            using (var ns = client.GetStream())
                            {
                                ns.Write(data, 0, data.Length);

                                var resData = new byte[256];
                                var bytes = ns.Read(resData, 0, resData.Length);
                                var result = Encoding.UTF8.GetString(resData, 0, bytes);

                                //HwLogger.NotifyRecv.Debug($"SendMsgResult Id:{msg.Id} result:{result}");
                            }
                        }
                        catch (Exception ex)
                        {
                            HWLogger.NotifyRecv.Error($"SendMsgResult Error", ex);
                        }
                    }
                }
            }
        }
    }
}