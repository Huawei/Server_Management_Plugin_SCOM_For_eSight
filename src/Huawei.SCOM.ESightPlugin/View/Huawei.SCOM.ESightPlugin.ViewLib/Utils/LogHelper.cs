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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.ViewLib.Utils
{
    public static class LogHelper
    {
        /// <summary>
        /// The object
        /// </summary>
        private static object obj = new object();

        /// <summary>
        /// The file name
        /// </summary>
        private static string fileName;

        /// <summary>
        /// Initializes static members of the <see cref="LogHelper"/> class.
        /// </summary>
        static LogHelper()
        {
            var path = Path.Combine(Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN"), "Logs");
            fileName = Path.Combine(path, "Huawei.UI.log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Informations the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="p">The p.</param>
        public static void Info(string format, params object[] p)
        {
            try
            {
                lock (obj)
                {
                    File.AppendAllText(fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + string.Format(format, p) + "\r\n");
                }
            }
            catch (Exception ex)
            {
                Error("WriteLog", ex);
            }
        }

        /// <summary>
        /// 记录程序异常
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(Exception ex, string format, params object[] args)
        {
            Error(args.Length > 0 ? string.Format(format, args) : format, ex);
        }

        /// <summary>
        /// 记录程序异常
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public static void Error(string message, Exception ex)
        {
            lock (obj)
            {
                var log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}\r\n";
                if (ex != null)
                {
                    log += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Exception:{ex }\r\n";
                }
                File.AppendAllText(fileName, log);
            }
        }
    }
}
