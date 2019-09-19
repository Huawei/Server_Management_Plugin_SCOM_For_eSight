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
// Assembly         : Huawei.SCOM.ESightPlugin.DAO
// Author           : suxiaobo
// Created          : 11-19-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="IHWESightHostDal.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSight数据库管理类</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.DAO
{
    using Huawei.SCOM.ESightPlugin.Models;

    /// <summary>
    ///     eSight数据库管理类
    /// </summary>
    public interface IHWESightHostDal : IBaseRepository<HWESightHost>
    {
        /// <summary>
        /// 删除eSight
        /// </summary>
        /// <param name="eSightId">
        /// eSight Id
        /// </param>
        void DeleteESight(int eSightId);

        /// <summary>
        /// 根据IP查找ESight实体。
        /// </summary>
        /// <param name="eSightIp">
        /// IP地址
        /// </param>
        /// <returns>
        /// The <see cref="HWESightHost"/>.
        /// </returns>
        HWESightHost FindByIP(string eSightIp);

        /// <summary>
        /// The update subscription alarm status.
        /// </summary>
        /// <param name="hostIp">
        /// The host ip.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int UpdateSubscriptionAlarmStatus(string hostIp, int status, string error);

        /// <summary>
        /// The update subscription ne device status.
        /// </summary>
        /// <param name="hostIp">
        /// The host ip.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int UpdateSubscriptionNeDeviceStatus(string hostIp, int status, string error);
    }
}