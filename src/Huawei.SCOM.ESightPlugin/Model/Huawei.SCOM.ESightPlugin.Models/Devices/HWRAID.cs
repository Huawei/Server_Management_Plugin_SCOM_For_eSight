using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    public class HWRAID
    {
        /// <summary>
        /// 名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 原始状态
        /// </summary>
        /// <value>The ori status.</value>
        [JsonProperty(PropertyName = "healthState")]
        public string OriStatus { get; set; }

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        public string HealthState => StatusHelper.ConvertStatus(OriStatus);

        /// <summary>
        /// raid类型
        /// </summary>
        [JsonProperty(PropertyName = "raidType")]
        public string RaidType { get; set; }

        private string interfaceType;

        /// <summary>
        /// 接口类型：“1”：SPI、“2”：SAS-3G、“3”：SATA-1.5G、“4”：SATA-3G、“5”：SAS-6G、“6”：SAS-12G、“255”/其它：未知
        /// </summary>
        /// <value>The type of the interface.</value>
        [JsonProperty(PropertyName = "interfaceType")]
        public string InterfaceType
        {
            get
            {
                switch (this.interfaceType)
                {
                    case "1": return "SPI";
                    case "2": return "SAS-3G";
                    case "3": return "SATA-1.5G";
                    case "4": return "SATA-3G";
                    case "5": return "SAS-6G";
                    case "6": return "SAS-12G";
                    case "255": return "Unknown";
                    default: return this.interfaceType;
                }
            }

            set
            {
                this.interfaceType = value;
            }
        }

        /// <summary>
        /// BBU类型
        /// </summary>
        [JsonProperty(PropertyName = "bbuType")]
        public string BbuType { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }
        /// <summary>
        /// RAID唯一标识，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }
    }
}
