//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    public enum ServerTypeEnum
    {
        Blade = 0,
        ChildBlade = 1,
        Highdensity = 2,
        ChildHighdensity = 3,
        Rack = 4,
        Default = 5,
        KunLun = 6,
        /// <summary>
        /// 刀片的swith
        /// </summary>
        Switch = 7
    }
}