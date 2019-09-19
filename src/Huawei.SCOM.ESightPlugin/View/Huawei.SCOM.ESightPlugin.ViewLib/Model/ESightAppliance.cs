//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Huawei.SCOM.ESightPlugin.ViewLib.Model.Constants;

namespace Huawei.SCOM.ESightPlugin.ViewLib.Model
{
    public class ESightAppliance
    {

        public const string EntityClassName = "ESight.Appliance";

        string _default_port = "32102";

        //[Key]
        //public long ID { get; set; }

        public string Host { get; set; }
        public string Port
        {
            get { return _default_port; }
            set { _default_port = value; }
        }

        public string AliasName { get; set; }

        public string LoginAccount { get; set; }

        public string LoginPassword { get; set; }

        public string SystemId { get; set; }

        public DateTime LastModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool UpdateCredential { get; set; }


        public string LatestStatus { get; set; }

        public string OpenID { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription alarm status.</value>
        public int SubKeepAliveStatus { get; set; }

        /// <summary>
        /// 订阅保活错误信息
        /// </summary>
        /// <value>The subscripe alarm error.</value>
        public string SubKeepAliveError { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription alarm status.</value>
        public int SubscriptionAlarmStatus { get; set; }

        public string AlarmSubscriptionStatusDisplay
        {
            get
            {
                switch (SubscriptionAlarmStatus)
                {
                    case 0:
                        return ESightSubscriptionStatus.NotSubscribed;
                    case -1:
                        return ESightSubscriptionStatus.Failed;
                    case 1:
                        return ESightSubscriptionStatus.Success;
                    default:
                        return ESightSubscriptionStatus.NotSubscribed;
                }
            }
        }

        /// <summary>
        /// 订阅告警错误信息
        /// </summary>
        /// <value>The subscripe alarm error.</value>
        public string SubscripeAlarmError { get; set; }

        /// <summary>
        /// -1 出错，0-未订阅 1-订阅成功
        /// </summary>
        /// <value>The subscription ne device status.</value>
        public int SubscriptionNeDeviceStatus { get; set; }
        public string DeviceSubscriptionStatusDisplay
        {
            get
            {
                switch (SubscriptionNeDeviceStatus)
                {
                    case 0:
                        return ESightSubscriptionStatus.NotSubscribed;
                    case -1:
                        return ESightSubscriptionStatus.Failed;
                    case 1:
                        return ESightSubscriptionStatus.Success;
                    default:
                        return ESightSubscriptionStatus.NotSubscribed;
                }
            }
        }

        /// <summary>
        /// 订阅设备变更错误信息
        /// </summary>
        /// <value>The subscripe Ne Device error.</value>
        public string SubscripeNeDeviceError { get; set; }

        /// <summary>
        /// Gets or sets the subscribe identifier.
        /// </summary>
        /// <value>The subscribe identifier.</value>
        public string SubscribeID { get; set; }


        /// <summary>
        /// Gets or sets the latest connect information.
        /// </summary>
        /// <value>The latest connect information.</value>
        public string LatestConnectInfo { get; set; }


        public override bool Equals(object obj)
        {
            var appliance = obj as ESightAppliance;
            return appliance != null &&
                   Host == appliance.Host;
        }

        public override int GetHashCode()
        {
            return 1485843175 + EqualityComparer<string>.Default.GetHashCode(Host);
        }
    }


    public class ESightApplianceOperation
    {

        public const string EntityClassName = "ESight.ApplianceOperation";

        public string Id { get; set; }
        public string Host { get; set; }
        public Int32 OperationType { get; set; }
        public bool IsSystemIdChanged { get; set; }
        public string OldSystemId { get; set; }
        public DateTime CreatedOn { get; set; }

    }

    public class Constants
    {
        /// <summary>
        /// The e sight connect status.
        /// </summary>
        public class ESightConnectionStatus
        {
            /// <summary>
            /// The lates t_ statu s_ disconnect.
            /// </summary>
            public const string DISCONNECT = "DISCONNECT";

            /// <summary>
            /// The lates t_ statu s_ failed.
            /// </summary>
            public const string FAILED = "Offline";

            /// <summary>
            /// The lates t_ statu s_ none.
            /// </summary>
            public const string NONE = "Ready";

            /// <summary>
            /// The lates t_ statu s_ online.
            /// </summary>
            public const string ONLINE = "Online";
        }

        public class ESightSubscriptionStatus
        {

            public const string NotSubscribed = "Not subscribed";

            public const string Failed = "Error";

            public const string Success = "Success";

        }



        public enum ESightApplianceOperationType
        {
            Add = 1,
            Update = 2,
            Delete = 4,
        }
    }
}
