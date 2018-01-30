// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 12-19-2017
//
// Last Modified By : yayun
// Last Modified On : 12-19-2017
// ***********************************************************************
// <copyright file="CustomData.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core.Models
{
    using System;

    /// <summary>
    /// Class CustomData.
    /// </summary>
    public class CustomData
    {
        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>The dn.</value>
        public string Dn { get; set; }

        /// <summary>
        /// Gets or sets the alarm sn.
        /// </summary>
        /// <value>The alarm sn.</value>
        public int AlarmSn { get; set; }

        /// <summary>
        /// 1-新增告警 2-清除告警 3-确认告警 4-反确认告警 5-变更告警 6-新增事件
        /// </summary>
        /// <value>The type of the opt.</value>
        public string OptType { get; set; }

        /// <summary>
        /// Gets or sets the event time.
        /// </summary>
        /// <value>The event time.</value>
        public string EventTime { get; set; }

        /// <summary>
        /// Gets or sets the type of the ne.
        /// </summary>
        /// <value>The type of the ne.</value>
        public string NeType { get; set; }

        /// <summary>
        /// Gets or sets the object instance.
        /// </summary>
        /// <value>The object instance.</value>
        public string ObjectInstance { get; set; }

        /// <summary>
        /// Gets or sets the proposed repair actions.
        /// </summary>
        /// <value>The proposed repair actions.</value>
        public string ProposedRepairActions { get; set; }

        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>The additional information.</value>
        public string AdditionalInformation { get; set; }
    }
}