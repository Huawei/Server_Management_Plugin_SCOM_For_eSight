using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Huawei.SCOM.ESightPlugin.Models.Devices
{
    public class HWHMM
    {
        /// <summary>
        /// 服务器唯一标识，例如：
        ///"NE=xxx"
        /// </summary>
        [JsonProperty(PropertyName = "dn")]
        public string DN { get; set; }

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 设备名称，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 设备型号，属性字符串直接显示，非枚举值
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// 服务器唯一标识，属性字符串直接显示，非枚举值
        ///备注：存储型服务器和第三方服务器不支持
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 原始状态
        /// </summary>
        /// <value>The ori status.</value>
        [JsonProperty(PropertyName = "status")]
        public string OriStatus { get; set; }

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        public string Status => StatusHelper.ConvertStatus(OriStatus);

        /// <summary>
        /// Gets or sets the SMM mac addr.
        /// </summary>
        /// <value>The SMM mac addr.</value>
        [JsonProperty(PropertyName = "smmMacAddr")]
        public string SmmMacAddr { get; set; }

        /// <summary>
        /// Gets or sets the real time power.
        /// </summary>
        /// <value>The real time power.</value>
        [JsonProperty(PropertyName = "realTimePower")]
        public string RealTimePower { get; set; }

        /// <summary>
        /// 务器序列号，属性字符串直接显示，非枚举值
        /// </summary>
        /// <value>The product sn.</value>
        [JsonProperty(PropertyName = "productSn")]
        public string ProductSN { get; set; }

        /// <summary>
        /// 设备在位信息：“0”：不在位 “1”：在位
        /// </summary>
        /// <value>The state of the present.</value>
        [JsonProperty(PropertyName = "presentState")]
        public string PresentState => "Present";


    }
}
