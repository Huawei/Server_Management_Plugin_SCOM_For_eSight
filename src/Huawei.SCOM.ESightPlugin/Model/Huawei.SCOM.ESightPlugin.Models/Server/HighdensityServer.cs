// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Models
// Author           : yayun
// Created          : 01-04-2018
//
// Last Modified By : yayun
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="HighdensityServer.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The highdensity server.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System.Collections.Generic;

    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The highdensity server.
    /// </summary>
    public class HighdensityServer
    {
        /// <summary>
        /// The _child highdensitys.
        /// </summary>
        private List<ChildHighdensity> _childHighdensitys;

        /// <summary>
        /// The _fan list.
        /// </summary>
        private List<HWFAN> _fanList;

        /// <summary>
        /// The _power supply list.
        /// </summary>
        private List<HWPSU> _powerSupplyList;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighdensityServer"/> class.
        /// </summary>
        public HighdensityServer()
        {
            this.FanList = new List<HWFAN>();
            this.PowerSupplyList = new List<HWPSU>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighdensityServer"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public HighdensityServer(HWDevice device)
        {
            this.DN = device.DN;
            this.ServerName = device.ServerName;
            this.Manufacturer = device.Manufacturer;
            this.ServerModel = device.ServerModel;
            this.IpAddress = device.IpAddress;
            this.Location = device.Location;
            this.Status = device.Status;
            this.UUID = device.UUID;
            this.ProductSN = device.ProductSN;
            this.Version = device.Version;
            this.ChildHighdensitys = new List<ChildHighdensity>();
        }

        /// <summary>
        ///     刀片服务器或者高密服务器的刀片列表，其他服务器类型此字段为null；
        ///     获取刀片详细信息需要通过“查询服务器详细信息”接口查询。
        /// </summary>
        public List<ChildHighdensity> ChildHighdensitys
        {
            get
            {
                return this._childHighdensitys ?? (this._childHighdensitys = new List<ChildHighdensity>());
            }

            set
            {
                this._childHighdensitys = value;
            }
        }

        /// <summary>
        /// 服务器唯一标识，格式为eSightIp-Dn
        /// "192.168.1.1-NE=xxx"
        /// </summary>
        public string DeviceId { get; set; }


        /// <summary>
        /// 服务器唯一标识，例如：
        /// "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        ///     Gets or sets the e sight.
        /// </summary>
        /// <value>The e sight.</value>
        public string ESight { get; set; }

        /// <summary>
        ///     Gets or sets the fan.
        /// </summary>
        /// <value>The fan.</value>
        public List<HWFAN> FanList
        {
            get
            {
                return this._fanList ?? (this._fanList = new List<HWFAN>());
            }

            set
            {
                this._fanList = value;
            }
        }

        /// <summary>
        ///     Gets or sets the HMM information.
        /// </summary>
        /// <value>The HMM information.</value>
        public HWHMM HmmInfo { get; set; }

        /// <summary>
        ///     服务器IP地址
        ///     SCOM：DeviceKey
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///     服务器位置信息，属性字符串直接显示，非枚举值
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     厂商，属性字符串直接显示，非枚举值
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        ///     Gets or sets the power supply.
        /// </summary>
        /// <value>The power supply.</value>
        public List<HWPSU> PowerSupplyList
        {
            get
            {
                return this._powerSupplyList ?? (this._powerSupplyList = new List<HWPSU>());
            }

            set
            {
                this._powerSupplyList = value;
            }
        }

        /// <summary>
        /// Gets or sets the product sn.
        /// </summary>
        public string ProductSN { get; set; }

        /// <summary>
        ///     服务器型号，属性字符串直接显示，非枚举值
        /// </summary>
        public string ServerModel { get; set; }

        /// <summary>
        ///     服务器名称，属性字符串直接显示，非枚举值
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        ///     服务器状态，含义如下：
        ///     “0”：正常
        ///     “-1”：离线
        ///     “-2”：未知
        ///     其他：故障
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///     待确认： esihgt 需要提供北向接口
        /// </summary>
        /// <value>The state of the system power.</value>
        public string SystemPowerState { get; set; }

        /// <summary>
        ///     服务器唯一标识，属性字符串直接显示，非枚举值
        ///     备注：存储型服务器和第三方服务器不支持
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        ///     默认为"HUAWEI"
        /// </summary>
        /// <value>The vendor.</value>
        public string Vendor => "HUAWEI";

        /// <summary>
        ///     BMC版本信息。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The make detail.
        /// </summary>
        /// <param name="detail">The detail.</param>
        /// <param name="eSightIp">The e sight ip.</param>
        public void MakeDetail(HWDeviceDetail detail, string eSightIp)
        {
            //高密服务器根据管理板Dn查询详情返回的是第一个子刀片的dn因此不等于deatial的dn
            var hmm = new HWHMM
            {
                DN = this.DN,
                IpAddress = detail.IpAddress,
                Name = detail.Name,
                Type = detail.Type,
                UUID = detail.UUID,
                Status = detail.Status,
                SmmMacAddr = detail.SmmMacAddr,
                RealTimePower = detail.RealTimePower,
                ProductSN = detail.ProductSN
            };
            this.ESight = eSightIp;
            //this.DN = detail.DN;
            this.DeviceId = $"{eSightIp}-{ this.DN}";
            this.HmmInfo = hmm;
            this.FanList = detail.FANList;
            this.PowerSupplyList = detail.PSUList;
        }
    }
}