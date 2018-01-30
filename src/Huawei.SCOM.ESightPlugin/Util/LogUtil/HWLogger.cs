using System;
using System.IO;

namespace LogUtil
{
    /// <summary>
    /// 日志管理类。
    /// </summary>
    public static class HWLogger
    {
        /// <summary>
        /// 缺省日志 输出: Huawei.SCOM.Plugin.For.eSight.log
        /// </summary>
        public static ILogger DEFAULT = new Logger("DEFAULT_LOG");
        /// <summary>
        /// UI层日志 输出: Huawei.UI.log
        /// </summary>
        public static ILogger UI = new Logger("UI_LOG");

        /// <summary>
        /// windows service日志
        /// </summary>
        public static ILogger SERVICE = new Logger("SERVICE_LOG");

        /// <summary>
        /// 通知消息日志
        /// </summary>
        public static ILogger NOTIFICATION = new Logger("NOTIFICATION_LOG");

        /// <summary>
        /// 业务类日志 输出：Huawei.API.log
        /// </summary>
        public static ILogger API = new Logger("API_LOG");
        /// <summary>
        /// 系统更新或者升级日志，目前暂时没有使用，保留属性 输出：Huawei.updater.log 
        /// </summary>
        public static ILogger UPDATER = new Logger("UPDATER_LOG");


        /// <summary>
        /// The notificatio n_ process
        /// </summary>
        public static ILogger NOTIFICATION_PROCESS = new Logger("NOTIFICATION_PROCESS_LOG");  
    }
}
