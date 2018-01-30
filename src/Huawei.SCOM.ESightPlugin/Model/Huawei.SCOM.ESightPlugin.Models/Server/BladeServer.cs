// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BladeServer.cs" company="">
//   
// </copyright>
// <summary>
//   The blade server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Huawei.SCOM.ESightPlugin.Models.Devices;

    /// <summary>
    /// The blade server.
    /// </summary>
    [Serializable]
    public class BladeServer
    {
        /// <summary>
        /// The _child blades.
        /// </summary>
        private List<ChildBlade> _childBlades;

        /// <summary>
        /// The _fan list.
        /// </summary>
        private List<HWFAN> _fanList;

        /// <summary>
        /// The _power supply list.
        /// </summary>
        private List<HWPSU> _powerSupplyList;

        /// <summary>
        /// The _switch list.
        /// </summary>
        private List<HWBoard> _switchList;

        /// <summary>
        /// Initializes a new instance of the <see cref="BladeServer"/> class.
        /// </summary>
        public BladeServer()
        {
            this.FanList = new List<HWFAN>();
            this.PowerSupplyList = new List<HWPSU>();
            this.ChildBlades = new List<ChildBlade>();
            this.SwitchList = new List<HWBoard>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BladeServer"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public BladeServer(HWDevice device)
        {
            this.DN = device.DN;
            this.ServerName = device.ServerName;
            this.Manufacturer = device.Manufacturer;
            this.ServerModel = device.ServerModel;
            this.IpAddress = device.IpAddress;
            this.Location = device.Location;
            this.Status = device.Status;
            this.FanList = new List<HWFAN>();
            this.PowerSupplyList = new List<HWPSU>();
            this.ChildBlades = new List<ChildBlade>();
            this.SwitchList = new List<HWBoard>();
        }

        /// <summary>
        ///     刀片服务器或者高密服务器的刀片列表，其他服务器类型此字段为null；
        ///     获取刀片详细信息需要通过“查询服务器详细信息”接口查询。
        /// </summary>
        public List<ChildBlade> ChildBlades
        {
            get
            {
                return this._childBlades ?? (this._childBlades = new List<ChildBlade>());
            }

            set
            {
                this._childBlades = value;
            }
        }

        /// <summary>
        ///     服务器唯一标识，例如：
        ///     "NE=xxx"
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// Gets or sets the e sight.
        /// </summary>
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
        ///     Gets or sets the power supply.
        /// </summary>
        /// <value>The power supply.</value>
        public List<HWBoard> SwitchList
        {
            get
            {
                return this._switchList ?? (this._switchList = new List<HWBoard>());
            }

            set
            {
                this._switchList = value;
            }
        }

        /// <summary>
        ///     待确认： esihgt 需要提供北向接口
        /// </summary>
        /// <value>The state of the system power.</value>
        public string SystemPowerState { get; set; }

        /// <summary>
        ///     默认为"HUAWEI"
        /// </summary>
        /// <value>The vendor.</value>
        public string Vendor => "HUAWEI";

        /// <summary>
        /// The make detail.
        /// </summary>
        /// <param name="detail">
        /// The detail.
        /// </param>
        public void MakeDetail(HWDeviceDetail detail)
        {
            var hmm = new HWHMM
                          {
                              DN = detail.DN,
                              IpAddress = detail.IpAddress,
                              Name = detail.Name,
                              Type = detail.Type,
                              UUID = detail.UUID,
                              Status = detail.Status,
                              SmmMacAddr = detail.SmmMacAddr,
                              RealTimePower = detail.RealTimePower,
                              ProductSN = detail.ProductSN
                          };
            this.HmmInfo = hmm;
            this.FanList = detail.FANList;
            this.SwitchList = detail.BoardList.Where(x => x.BoardType == 1).ToList();
            this.PowerSupplyList = detail.PSUList;
        }

        // public string Version { get; set; }
        ///// </summary>
        ///// BMC版本信息。

        ///// <summary>
        // public string UUID { get; set; }
        ///// </summary>
        /////备注：存储型服务器和第三方服务器不支持
        ///// 服务器唯一标识，属性字符串直接显示，非枚举值
        ///// <summary>
        // public string Description { get; set; }
        ///// </summary>
        ///// 服务器描述，属性字符串直接显示，非枚举值

        ///// <summary>
    }
}