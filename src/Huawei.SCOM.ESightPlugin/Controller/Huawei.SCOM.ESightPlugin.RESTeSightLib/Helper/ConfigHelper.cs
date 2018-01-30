// ***********************************************************************
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


namespace Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper
{
    using System;
    using System.IO;
    using CommonUtil;
    using Huawei.SCOM.ESightPlugin.Models;

    using LogUtil;

    /// <summary>
    /// Class ConfigHelper.
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// The env variable.
        /// </summary>
        private static string path = $"{Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN")}/Configuration/PluginConfig.xml";

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
