using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    [Serializable]
    public class HWDisk
    {
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
        /// 槽位信息
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public int Location { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }

        /// <summary>
        /// 唯一标识.
        /// </summary>
        /// <value>The UUID.</value>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        private string _presentState { get; set; }

        /// <summary>
        /// 设备在位信息：“0”：不在位 “1”：在位
        /// </summary>
        /// <value>The state of the present.</value>
        [JsonProperty(PropertyName = "presentState")]
        public string PresentState
        {
            get { return StatusHelper.GetPresentState(_presentState); }
            set { _presentState = value; }
        }
    }
}
