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
// Last Modified On : 11-19-2017
// ***********************************************************************
// <copyright file="SingletonProvider.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The singleton provider.</summary>
// ***********************************************************************

namespace CommonUtil
{
    /// <summary>
    /// The singleton provider.
    /// </summary>
    /// <typeparam name="T">T
    /// </typeparam>
    public class SingletonProvider<T>
        where T : new()
    {
        /// <summary>
        /// The sync object.
        /// </summary>
        private static readonly object SyncObject = new object();

        /// <summary>
        /// The _singleton.
        /// </summary>
        private static T _singleton;


        /// <summary>
        /// Prevents a default instance of the <see cref="SingletonProvider{T}"/> class from being created.
        /// </summary>
        private SingletonProvider()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_singleton == null)
                {
                    lock (SyncObject)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new T();
                        }
                    }
                }
                return _singleton;
            }
        }
    }
}