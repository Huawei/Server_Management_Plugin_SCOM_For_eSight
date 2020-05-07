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
// Assembly         : Huawei.SCOM.ESightPlugin.RESTeSightLib
// Author           : yayun
// Created          : 01-02-2018
//
// Last Modified By : yayun
// Last Modified On : 01-02-2018
// ***********************************************************************
// <copyright file="ConfigHelper.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************


namespace Huawei.SCOM.ESightPlugin.Models
{
    using System;
    using System.IO;
    using CommonUtil;

    /// <summary>
    /// Class ConfigHelper.
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// The env variable.
        /// </summary>
        private static string path =Path.Combine($"{Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN")}","Configuration/PluginConfig.xml");

        /// <summary>
        ///     Gets the plugin configuration.
        /// </summary>
        /// <returns>Huawei.SCOM.ESightPlugin.Models.PluginConfig.</returns>
        public static PluginConfig GetPluginConfig()
        {
            if (!File.Exists(path))
            {
                throw new Exception("can not find config file:");
            }

            var config = XmlHelper.Load(typeof(PluginConfig), path) as PluginConfig;
            if (config == null)
            {
                throw new Exception("config is null");
            }
            return config;
        }

        /// <summary>
        /// Saves the plugin configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void SavePluginConfig(PluginConfig config)
        {
            XmlHelper.Save(config, path);
        }

        /// <summary>
        /// Saves the plugin configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="installPath">The install path.</param>
        public static void SavePluginConfig(PluginConfig config, string installPath)
        {
            XmlHelper.Save(config, installPath);
        }
    }
}
