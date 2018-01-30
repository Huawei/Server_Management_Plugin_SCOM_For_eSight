using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    /// <summary>
    /// 板信息，刀片服务器：交换板；机架、高密服务器、刀片：主板；
    /// </summary>
    [Serializable]
    public class HWBoard
    {

        /// <summary>
        /// Gets or sets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        private string _healthState;

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        [JsonProperty(PropertyName = "healthState")]
        public string HealthState { get { return _healthState; } set { _healthState = StatusHelper.ConvertStatus(value); } }

        /// <summary>
        /// 单板类型，含义如下：	“0”：主板	“1”：交换板
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public int BoardType { get; set; }
        /// <summary>
        /// 单板序列号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "sn")]
        public string SN { get; set; }

        /// <summary>
        /// 单板部件号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "partNumber")]
        public string PartNumber { get; set; }

        private string _manufacturer;
        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer
        {
            //华为返回此字段有问题 有时返回manufacturer 有时返回manufacture
            get { return !string.IsNullOrEmpty(_manufacturer) ? _manufacturer : Manufacture; }
            set { _manufacturer = value; }
        }

        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manufacture")]
        public string Manufacture { get; set; }

        /// <summary>
        /// 制造日期，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "manuTime")]
        public string ManuTime { get; set; }

        /// <summary>
        ///设备唯一标识符
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }
    }
}
