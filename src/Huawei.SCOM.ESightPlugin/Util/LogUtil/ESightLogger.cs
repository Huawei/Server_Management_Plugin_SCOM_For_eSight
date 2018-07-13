using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Huawei.SCOM.ESightPlugin.LogUtil
{
    public class ESightLogger
    {
        public ESightLogger(string eSightIp) 
        {
            this.ESightIp = eSightIp;
        }

        public string ESightIp { get; set; }

        public Logger Polling => LogManager.GetLogger($"{this.ESightIp}.Polling");

        public Logger Subscribe => LogManager.GetLogger($"{this.ESightIp}.Subscribe");

        public Logger NotifyProcess => LogManager.GetLogger($"{this.ESightIp}.NotifyProcess");

        public Logger Api => LogManager.GetLogger($"{this.ESightIp}.Api");

        public Logger Sdk => LogManager.GetLogger($"{this.ESightIp}.Sdk");

    }
}
