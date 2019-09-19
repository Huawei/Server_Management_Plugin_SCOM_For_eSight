//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Const
// Author           : yayun
// Created          : 01-05-2018
//
// Last Modified By : yayun
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="ESightConnectStatus.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The e sight connect status.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Const
{
    /// <summary>
    /// Class ConstMgr.
    /// </summary>
    public partial class ConstMgr
    {
        /// <summary>
        /// The e sight connect status.
        /// </summary>
        public class ESightConnectStatus : ConstBase
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
    }
}