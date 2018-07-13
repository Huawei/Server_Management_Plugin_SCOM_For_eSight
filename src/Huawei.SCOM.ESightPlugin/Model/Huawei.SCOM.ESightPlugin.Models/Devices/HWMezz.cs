using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    public class HWMezz
    {
        // <summary>
        /// 名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 原始状态
        /// </summary>
        /// <value>The ori status.</value>
        [JsonProperty(PropertyName = "mezzHealthStatus")]
        public string OriStatus { get; set; }

        /// <summary>
        /// 健康状态（1：:正常；-2、5：未知;其他故障；PresentState为-2时健康状态是为未知）
        /// </summary>
        public string MezzHealthStatus => StatusHelper.ConvertStatus(OriStatus);

        private string _presentState { get; set; }

        /// <summary>
        /// 在位状态（0：不在位；非0在位）
        /// </summary>
        [JsonProperty(PropertyName = "presentState")]
        public string PresentState
        {
            get { return StatusHelper.GetPresentState(_presentState); }
            set { _presentState = value; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty(PropertyName = "mezzInfo")]
        public string MezzInfo { get; set; }
        /// <summary>
        /// 位置信息
        /// </summary>
        [JsonProperty(PropertyName = "mezzLocation")]
        public string MezzLocation { get; set; }
        /// <summary>
        /// Mac地址
        /// </summary>
        [JsonProperty(PropertyName = "mezzMac")]
        public string MezzMac { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        [JsonProperty(PropertyName = "moId")]
        public string MoId { get; set; }
        /// <summary>
        /// 唯一标识，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }
    }
}
