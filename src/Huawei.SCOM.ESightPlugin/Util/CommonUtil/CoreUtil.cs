//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Huawei.SCOM.ESightPlugin.LogUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtil
{
    /// <summary>
    /// 通用Util包，主要是一些比较常用的方法，比如对象的null转换等。
    /// </summary>
    public class CoreUtil
    {
        /// <summary>
        /// 1. 类型转换
        /// 2. 对象null值处理，方便一些对象为null时，返回缺省的类型初值。
        /// 如：string 为null时，返回空字符串。        
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">需要转换的对象</param>
        /// <returns></returns>
        public static T GetObjTranNull<T>(Object obj)
        {
            try
            {
                //处理数据库空值返回空字符串转换的结果;字符串为Null返回空字符.
                if ((obj == null || obj == DBNull.Value) && (typeof(T) == typeof(string)))
                {
                    return (T)Convert.ChangeType("", typeof(T));
                }
                //如果对象是转换类型时,直接返回.
                if (obj is T)
                {
                    return (T)obj;
                }
                //处理可空类型.
                if (typeof(T).IsGenericType)
                {
                    Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        return (T)Convert.ChangeType(obj, Nullable.GetUnderlyingType(typeof(T)));
                    }
                }
                //强制转换.
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (InvalidCastException)
            {
                try
                {
                    //自动转换类型失败时,强制转换.
                    if (obj != null)
                    {
                        return (T)obj;
                    }
                }
                catch (FormatException fe)
                {
                    HWLogger.Default.Error("Cast value failed, FormatException. Using default:", fe);
                }
                catch (Exception ex)
                {
                    HWLogger.Default.Error("Cast value failed,InvalidCastException. Using default:", ex);
                }
            }
            catch (FormatException fe)
            {
                HWLogger.Default.Error("Cast value failed, FormatException:", fe);
            }
            catch (OverflowException oe)
            {
                HWLogger.Default.Error("Cast value failed, OverflowException:", oe);
            }
            catch (ArgumentNullException ane)
            {
                HWLogger.Default.Error("Cast value failed, ArgumentNullException:", ane);
            }
            catch (Exception ex) { 
                HWLogger.Default.Error("Cast value failed, Exception:", ex);
            }
            //返回缺省值.
            return default(T);
        }
    }
}
