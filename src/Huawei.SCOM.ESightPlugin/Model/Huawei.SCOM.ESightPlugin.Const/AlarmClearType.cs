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

namespace Huawei.SCOM.ESightPlugin.Const
{
    public partial class ConstMgr
    {
        /// <summary>
        /// 前台错误代码，由后台返回
        /// 只有一些后台抛出错误代码定义。
        /// </summary>
        public class AlarmClearType : ConstBase
        {
            public const string ADAC = "Automatically Detected and Automatically Cleared";
            public const string ADMC = "Automatically Detected and Manually Cleared";

            public static string GetClearReason(int clearType)
            {
                return clearType == 1 ? ADAC : ADMC;
            }
        }
    }
}
