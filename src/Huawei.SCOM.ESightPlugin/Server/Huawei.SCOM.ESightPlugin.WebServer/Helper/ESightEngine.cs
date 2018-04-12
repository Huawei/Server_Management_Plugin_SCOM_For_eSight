// ***********************************************************************
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

namespace Huawei.SCOM.ESightPlugin.WebServer.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
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
        /// The _lock refresh pwds.
        /// </summary>
        private static readonly object LockRefreshPwds = new object();

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
                            instance.CheckAndUpgradeKey();
                            // 2017-10-11
                            // 将30天刷新一次（RefreshPwdsThirtyDay）
                            // 改为检测如果是兼容密钥则30天刷新一次
                            // 否则则升级密钥。
                        }
                    }
                    finally
                    {
                        // 释放
                        Monitor.Exit(LockInstance);
                    }
                }
                else
                {
                    instance.CheckAndUpgradeKey();
                    // 2017-10-11
                    // 将30天刷新一次（RefreshPwdsThirtyDay）
                    // 改为检测如果是兼容密钥则30天刷新一次
                    // 否则则升级密钥。
                }
                return instance;
            }
        }
        #endregion

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

            HWLogger.UI.Info("iESSession" + iEsSession.ESight.SystemID);
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
                HWLogger.API.Error(ess);
                if (ess.Code == "1")
                {
                    return ConstMgr.ErrorCode.SYS_USER_LOGING;
                }
                return ess.Code;
            }
            catch (Exception se)
            {
                HWLogger.API.Error(se);
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

        #region RefreshPwds
        /// <summary>
        ///     2017-10-11 检查并升级密钥
        /// </summary>
        private void CheckAndUpgradeKey()
        {
            if (!EncryptUtil.IsCompatibleVersion())
            {
                this.InitEsSessions();
                this.RefreshPwds();
            }
            else
            {
                this.RefreshPwdsThirtyDay();
            }
        }

        /// <summary>
        ///     刷新密码。时机是每次启动时，这里会加在这个单例的初始化。
        ///     重置密钥，并且更新密码。
        ///     规则1：密钥须支持可更新，并明确更新周期，在一次性可编程的芯片中保存的密钥除外
        ///     说明：工作密钥及密钥加密密钥在使用过程中，都应保证其可以更新。对于根密钥暂不要求必须支持可更新。
        /// </summary>
        private void RefreshPwds()
        {
            try
            {
                HWLogger.DEFAULT.InfoFormat("Refresh eSightpwd with encryption...");
                lock (LockRefreshPwds)
                {
                    lock (this.eSightSessions)
                    {
                        using (var mutex = new Mutex(false, "huawei.SCOM.ESightPlugin.engine"))
                        {
                            if (mutex.WaitOne(TimeSpan.FromSeconds(60), false))
                            {
                                string oldMainKey;

                                #region 检查是否需要升级的密钥
                                // 2017-10-11 检查是否需要升级的密钥。
                                if (!EncryptUtil.IsCompatibleVersion())
                                {
                                    oldMainKey = EncryptUtil.GetMainKey1060();
                                    HWLogger.DEFAULT.InfoFormat("oldMainKey:{0}", oldMainKey);
                                    if (string.IsNullOrEmpty(oldMainKey))
                                    {
                                        return;
                                    }
                                    EncryptUtil.ClearAndUpgradeKey();
                                }
                                else
                                {
                                    // 旧的key
                                    oldMainKey = EncryptUtil.GetMainKeyWithoutInit();
                                    if (string.IsNullOrEmpty(oldMainKey))
                                    {
                                        return;
                                    }

                                    // 重新初始化主密钥。
                                    EncryptUtil.InitMainKey();
                                }
                                #endregion

                                var newMainKey = EncryptUtil.GetMainKeyFromPath();

                                #region 刷新密码
                                // LogUtil.HWLogger.DEFAULT.InfoFormat("Change key,oldMainKey={1},newMainKey={1}",oldMainKey,newMainKey);
                                // 遍历所有session.
                                var hostlist = ESightDal.Instance.GetList();
                                HWLogger.DEFAULT.InfoFormat($"Start Refresh eSightpwd !eSight Count:{hostlist.Count}");
                                foreach (var eSightHost in hostlist)
                                {
                                    var pwd = EncryptUtil.DecryptWithKey(oldMainKey, eSightHost.LoginPd);
                                    var enPwd = EncryptUtil.EncryptWithKey(newMainKey, pwd);

                                    var iEsSession = this.FindEsSession(eSightHost.HostIP);
                                    iEsSession.ESight.LoginPd = enPwd;

                                    this.eSightSessions[eSightHost.HostIP.ToUpper()] = iEsSession;
                                    ESightDal.Instance.UpdateESightPwd(eSightHost.HostIP, enPwd);
                                    HWLogger.DEFAULT.InfoFormat($"Refresh eSightpwd :{eSightHost.HostIP}!");
                                }
                                #endregion
                            }
                        }
                    }
                }

                HWLogger.DEFAULT.InfoFormat("Refresh eSightpwd with encryption successful!");
            }
            catch (Exception ex)
            {
                HWLogger.DEFAULT.Error("Refresh eSightpwd  Error!", ex);
            }
        }

        /// <summary>
        ///     隔天更新密钥。
        ///     1. 当启动时更新。
        ///     2. 启动后，距离上次更新超过一天时更新。
        /// </summary>
        private void RefreshPwdsThirtyDay()
        {
            var now = DateTime.Now;
            var d = now.Subtract(EncryptUtil.GetLatestKeyChangeDate());
            if (d.Days > 30)
            {
                this.InitEsSessions();
                this.RefreshPwds();
            }
        }
        #endregion
    }
}