// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Models
// Author           : yayun
// Created          : 03-26-2018
//
// Last Modified By : yayun
// Last Modified On : 03-26-2018
// ***********************************************************************
// <copyright file="ChildSwithBoard.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// Class ChildSwithBoard.
    /// </summary>
    public class ChildSwithBoard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildSwithBoard"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="eSight">The e sight.</param>
        public ChildSwithBoard(HWBoard model, string eSight)
        {
            this.DeviceId = $"{eSight}-{ model.DN}";
            this.DN = model.DN;
            this.ESight = eSight;
            this.ParentDN = model.ParentDN;
            this.IpAddress = model.IpAddress;
            this.Name = model.Name;
            this.HealthState = model.HealthState;
            this.BoardType = model.BoardType;
            this.SN = model.SN;
            this.PartNumber = model.PartNumber;
            this.Manufacturer = model.Manufacturer;
            this.ManuTime = model.ManuTime;
            this.PresentState = model.PresentState;
            this.AssertTag = model.AssertTag;
            this.MoId = model.MoId;
            this.UUID = model.UUID;
        }

        /// <summary>
        /// 服务器唯一标识，格式为eSightIp-Dn
        /// "192.168.1.1-NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }

        /// <summary>
        /// Gets or sets the parent dn.
        /// </summary>
        /// <value>The parent dn.</value>
        public string ParentDN { get; set; }

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <value>The ip address.</value>
        public string IpAddress { get; set; }

        /// <summary>
        /// 名称，属性字符串直接显示，非枚举值
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务器状态，含义如下：
        ///	“0”：正常
        ///	“-1”：离线
        ///	“-2”：未知
        ///	其他：故障
        /// </summary>
        public string HealthState { get; set; }

        /// <summary>
        /// 单板类型，含义如下：“0”：主板	“1”：交换板
        /// </summary>
        public int BoardType { get; set; }

        /// <summary>
        /// 单板序列号，属性字符串直接显示，非枚举值
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 单板部件号，属性字符串直接显示，非枚举值
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 厂商，属性字符串直接显示，非枚举值
        /// </summary>
        public string Manufacturer { get; set; }


        /// <summary>
        /// 制造日期，属性字符串直接显示，非枚举值
        /// </summary>
        public string ManuTime { get; set; }

        /// <summary>
        /// 设备在位信息：“0”：不在位 “1”：在位
        /// </summary>
        /// <value>The state of the present.</value>
        public string PresentState { get; set; }

        /// <summary>
        /// Gets or sets the assert tag.
        /// </summary>
        /// <value>The assert tag.</value>
        public string AssertTag { get; set; }

        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public string MoId { get; set; }

        /// <summary>
        /// Gets or sets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        public string UUID { get; set; }
    }
}
