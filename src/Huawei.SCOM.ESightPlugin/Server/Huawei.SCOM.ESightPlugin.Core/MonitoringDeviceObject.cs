using Huawei.SCOM.ESightPlugin.Models.Server;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Core
{
    public class MonitoringDeviceObject
    {
        public string DeviceId { get; set; }

        public MonitoringObject Device { get; set; }

        public ServerTypeEnum ServerType { get; set; }

        Func<string, MonitoringObject> LoadFunc { get; set; }

        public MonitoringDeviceObject(string deviceId, MonitoringObject Device, ServerTypeEnum serverType, Func<string, MonitoringObject> LoadFunc)
        {
            this.DeviceId = deviceId;
            this.Device = Device;
            this.LoadFunc = LoadFunc;
            this.ServerType = serverType;
        }

        public MonitoringDeviceObject Reload()
        {
            this.Device = this.LoadFunc(DeviceId);
            return this;
        }

        public ManagementPackClass GetMPClazz()
        {
            switch(this.ServerType)
            {
                case ServerTypeEnum.Blade:
                    return BladeConnector.Instance.BladeClass;
                case ServerTypeEnum.ChildBlade:
                    return BladeConnector.Instance.ChildBladeClass;
                case ServerTypeEnum.Switch:
                    return BladeConnector.Instance.SwitchClass;

                case ServerTypeEnum.Highdensity:
                    return HighdensityConnector.Instance.HighdensityClass;
                case ServerTypeEnum.ChildHighdensity:
                    return HighdensityConnector.Instance.ChildHighdensityClass;
                case ServerTypeEnum.Rack:
                    return RackConnector.Instance.RackClass;
                case ServerTypeEnum.KunLun:
                    return KunLunConnector.Instance.KunLunClass;
                default:
                    return null;
            }
        }

    }
}
