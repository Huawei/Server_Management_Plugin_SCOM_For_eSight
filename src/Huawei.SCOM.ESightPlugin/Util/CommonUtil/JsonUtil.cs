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
// Assembly         : CommonUtil
// Author           : yayun
// Created          : 11-14-2017
//
// Last Modified By : yayun
// Last Modified On : 01-10-2018
// ***********************************************************************
// <copyright file="JsonUtil.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>Json工具类.</summary>
// ***********************************************************************

namespace CommonUtil
{
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    ///     Json工具类.
    /// </summary>
    public class JsonUtil
    {
        static JsonUtil()
        {
            InitJsonConvert();
        }

        /// <summary>
        /// 反序列化一个json字符串为T类型的实例对象。
        /// </summary>
        /// <typeparam name="T">泛型类型，传入目标类型</typeparam>
        /// <param name="s">json字符串</param>
        /// <returns>The <see cref="T" />.</returns>
        public static T DeserializeObject<T>(string s)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(s);
            using (var stream = new MemoryStream(jsonBytes))
            {
                using (var sr = new StreamReader(stream))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            NullValueHandling = NullValueHandling.Include
                        };
                        return serializer.Deserialize<T>(reader);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化json设置。
        /// 目前需要做的只有日期，下面备注的是以后需要的部分。
        /// -空值处理
        /// setting.NullValueHandling = NullValueHandling.Ignore;
        /// -高级用法中的Bool类型转换 设置
        /// setting.Converters.Add(new BoolConvert("是,否"));
        /// </summary>
        public static void InitJsonConvert()
        {
            var setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = () =>
                {
                    // 日期类型默认格式化处理
                    setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    return setting;
                };
        }

        /// <summary>
        /// 序列化对象, 序列化一个json字符串。
        /// </summary>
        /// <param name="obj">
        /// json字符串
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}