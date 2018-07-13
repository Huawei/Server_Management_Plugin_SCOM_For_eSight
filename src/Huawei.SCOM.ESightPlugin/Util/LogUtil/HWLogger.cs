using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Huawei.SCOM.ESightPlugin.LogUtil
{
    public class HWLogger
    {
        public static Logger Default => LogManager.GetLogger($"Default");

        public static Logger Service => LogManager.GetLogger($"Service");

        public static Logger Install => LogManager.GetLogger($"Install");

        public static Logger UI => LogManager.GetLogger($"UI");

        public static Logger NotifyRecv => LogManager.GetLogger($"NotifyRecv");

        public static Logger GetESightSdkLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.Sdk");
        }

        public static Logger GetESightNotifyLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.NotifyProcess");
        }

        public static Logger GetESightSubscribeLogger(string eSightIp)
        {
            return LogManager.GetLogger($"{eSightIp}.Subscribe");
        }
    }
}
