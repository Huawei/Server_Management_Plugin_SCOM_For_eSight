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
// Created          : 11-14-2017
//
// Last Modified By : yayun
// Last Modified On : 01-02-2018
// ***********************************************************************
// <copyright file="ESightEngine.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSight管理类。（eSight引擎)</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.RESTeSightLib
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Exceptions;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;

    using LogUtil;

    /// <summary>
    ///     eSight管理类。（eSight引擎)
    /// </summary>
    public class ESightEngine
    {
        #region Field

        /// <summary>
        /// The _lock instance.
        /// </summary>
        private static readonly object LockInstance = new object();
     
        /// <summary>
        /// The instance
        /// </summary>
        private static ESightEngine instance;

        /// <summary>
        ///     eSight Session的Dictionary存储向量。
        /// </summary>
        private readonly Dictionary<string, ESightSession> eSightSessions = new Dictionary<string, ESightSession>();

        /// <summary>
        ///     单例模式，保证在内存中仅有一个管理类的实例。
        /// </summary>
        public static ESightEngine Instance
        {
            get
            {
                // 是否没有初始化
                if (instance == null)
                {
                    try
                    {
                        System.Threading.Monitor.Enter(LockInstance);
                        // 打开线程锁，再判断一次
                        // 两次判断是为了提高单例获取时的效率.
                        // 开锁后，相对判断会比较满。
                        if (instance == null)
                        {
                            instance = new ESightEngine();
                            instance.InitEsSessions();
                        }
                    }
                    finally
                    {
                        // 释放
                        Monitor.Exit(LockInstance);
                    }
                }
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// 初始化所有的eSight连接的配置信息。
        /// 注意，并没有open。
        /// </summary>
        public void InitEsSessions()
        {
            var hostList = ESightDal.Instance.GetList(); // 获取eSight
            foreach (var hwESightHost in hostList)
            {
                lock (this.eSightSessions)
                {
                    this.eSightSessions.Clear();
                    ESightSession iEsSession = new ESightSession(hwESightHost);
                    this.eSightSessions[hwESightHost.HostIP.ToUpper()] = iEsSession;
                }
            }
        }

        /// <summary>
        /// Checks all eSight connection.
        /// </summary>
        public void CheckAllESightConnection()
        {
            foreach (var eSightSession in this.eSightSessions)
            {
                eSightSession.Value.IsCanConnect();
            }
        }

        /// <summary>
        /// 删除一个已有的eSight.
        /// </summary>
        /// <param name="hostIp">eSight IP</param>
        /// <returns>默认返回成功</returns>
        public bool RemoveEsSession(string hostIp)
        {
            var iEsSession = this.FindEsSession(hostIp);
            if (iEsSession != null)
            {
                this.eSightSessions.Remove(hostIp.ToUpper());
            }
            return true;
        }

        /// <summary>
        /// Saves the new es session.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        /// <returns>IESSession.</returns>
        public ESightSession SaveSession(HWESightHost eSight)
        {
            // 测试连接...
            using (var eSSession = new ESightSession(eSight))
            {
                eSSession.TestConnection();
            }
            // 查找已有的eSesssion,防止重复
            var iEsSession = this.FindEsSession(eSight.HostIP);
            if (iEsSession == null)
            {
                // 没有找到已有的eSight.
                iEsSession = new ESightSession(eSight);
            }
            iEsSession.ESight = eSight;
            lock (this.eSightSessions)
            {
                // 锁定向量，防止并发
                this.eSightSessions[iEsSession.ESight.HostIP.ToUpper()] = iEsSession; // 存储到缓存。
            }

            HWLogger.UI.Info("SaveSession eSight" + eSight.HostIP);
            return iEsSession;
        }

        /// <summary>
        /// 测试eSight是否能够连通。
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        /// <returns>失败返回错误码，成功返回为空字符</returns>
        public string TestEsSession(HWESightHost eSight)
        {
            try
            {
                using (var eSSession = new ESightSession(eSight))
                {
                    eSSession.TestConnection();
                }
            }
            catch (ESSessionExpceion ess)
            {
                HWLogger.UI.Error(ess);
                if (ess.Code == "1")
                {
                    return ConstMgr.ErrorCode.SYS_USER_LOGING;
                }
                return ess.Code;
            }
            catch (Exception se)
            {
                HWLogger.UI.Error(se);
                return ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
            }

            return string.Empty;
        }

        /// <summary>
        /// 是否相同的eSight实体。
        /// </summary>
        /// <param name="host1">
        /// host1
        /// </param>
        /// <param name="host2">
        /// host2
        /// </param>
        /// <returns>
        /// bool
        /// </returns>
        public bool IsSameESightHost(HWESightHost host1, HWESightHost host2)
        {
            if (host1.HostIP != host2.HostIP)
            {
                return false;
            }
            if (host1.HostPort != host2.HostPort)
            {
                return false;
            }
            if (host1.AliasName != host2.AliasName)
            {
                return false;
            }
            if (host1.LoginAccount != host2.LoginAccount)
            {
                return false;
            }
            if (host1.LoginPd != host2.LoginPd)
            {
                return false;
            }
            if (host1.CertPath != host2.CertPath)
            {
                return false;
            }
            if (host1.SubscriptionAlarmStatus != host2.SubscriptionAlarmStatus)
            {
                return false;
            }
            if (host1.SubscripeAlarmError != host2.SubscripeAlarmError)
            {
                return false;
            }
            if (host1.SubKeepAliveStatus != host2.SubKeepAliveStatus)
            {
                return false;
            }
            if (host1.SubKeepAliveError != host2.SubKeepAliveError)
            {
                return false;
            }
            if (host1.SubscriptionNeDeviceStatus != host2.SubscriptionNeDeviceStatus)
            {
                return false;
            }
            if (host1.SubscripeNeDeviceError != host2.SubscripeNeDeviceError)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找eSight主机信息
        /// </summary>
        /// <param name="hostIp">主机IP</param>
        /// <returns>返回查找到的eSight，没有找到返回为null</returns>
        public ESightSession FindEsSession(string hostIp)
        {
            if (this.eSightSessions.ContainsKey(hostIp.ToUpper()))
            {
                return this.eSightSessions[hostIp.ToUpper()];
            }
            return null;
        }

    }
}