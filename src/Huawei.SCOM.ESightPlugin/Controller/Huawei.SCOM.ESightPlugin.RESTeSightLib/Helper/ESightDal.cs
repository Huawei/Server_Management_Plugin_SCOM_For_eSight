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
// Created          : 12-08-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 01-05-2018
// ***********************************************************************
// <copyright file="HWESightHostDal.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSight数据库管理类</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.ViewLib.Model;
    using Huawei.SCOM.ESightPlugin.ViewLib.OM12R2;
    using Microsoft.EnterpriseManagement.Common;
    using Microsoft.EnterpriseManagement.Monitoring;

    /// <summary>
    /// Class ESightDal.
    /// </summary>
    public class ESightDal
    {
        /// <summary>
        /// The file path
        /// </summary>
        private readonly string filePath = Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN") + "//KN//eSight.bin";

        /// <summary>
        ///     单例
        /// </summary>
        public static ESightDal Instance => SingletonProvider<ESightDal>.Instance;

        /// <summary>
        /// Deletes the e sight by host ip.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        public void DeleteESightByHostIp(string hostIp)
        {
            // TODO should never call here??
            var entity = this.GetEntityByHostIp(hostIp);
            if (entity == null)
            {
                throw new Exception($"can not find eSight:{hostIp}");
            }

        }

        /// <summary>
        /// 根据订阅ID查询eSight
        /// </summary>
        /// <param name="subscribeId">The subscribe identifier.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.Models.HWESightHost.</returns>
        public HWESightHost GetEntityBySubscribeId(string subscribeId)
        {
            IObjectReader<EnterpriseManagementObject> items =
               OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"SubscribeID='{subscribeId}'");
            return items.Select(ConvertMonitoringObjectToESightHost()).FirstOrDefault();
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public void InsertEntity(HWESightHost model)
        {
            // TODO should never call here??
            var list = this.GetList();
            list.Add(model);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public void UpdateEntity(HWESightHost model)
        {
            // TODO should never call here??
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == model.HostIP);
            if (oldModel == null)
            {
                throw new Exception($"Can not find the eSight:{model.HostIP}");
            }
            list.Remove(oldModel);
            list.Add(model);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// The get list.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IList</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IList<HWESightHost> GetList(string condition = "1=1 ")
        {
            IObjectReader<EnterpriseManagementObject> monitoringObjects = OM12Connection.All<EnterpriseManagementObject>(ESightAppliance.EntityClassName);
            IEnumerable<HWESightHost> appliances = monitoringObjects.Select(ConvertMonitoringObjectToESightHost());
            return appliances.OrderByDescending(x => x.CreateTime).ToList();
        }


        /// <summary>
        /// Convert OM MonitoringObject to HWESightHost Entity Function
        /// </summary>
        /// <returns></returns>
        private static Func<EnterpriseManagementObject, HWESightHost> ConvertMonitoringObjectToESightHost()
        {
            return obj =>
            {
                var props = OM12Connection.GetManagementPackProperties(obj);
                HWESightHost appliance = new HWESightHost
                {
                    HostIP = obj[props["Host"]].Value as String,
                    HostPort = Convert.ToInt32(obj[props["Port"]].Value),
                    AliasName = obj[props["AliasName"]].Value as String,
                    SystemID = obj[props["SystemId"]].Value as String,
                    LoginAccount = obj[props["LoginAccount"]].Value as String,
                    LoginPd = obj[props["LoginPassword"]].Value as String,
                    CreateTime = ((DateTime)obj[props["CreatedOn"]].Value).ToLocalTime(),
                    LastModifyTime = ((DateTime)obj[props["LastModifiedOn"]].Value).ToLocalTime(),

                    OpenID = obj[props["OpenID"]].Value as String,
                    SubscribeID = obj[props["SubscribeID"]].Value as String,
                    SubKeepAliveStatus = Convert.ToInt32(obj[props["SubKeepAliveStatus"]].Value),
                    SubscriptionAlarmStatus = Convert.ToInt32(obj[props["SubscriptionAlarmStatus"]].Value),
                    SubscriptionNeDeviceStatus = Convert.ToInt32(obj[props["SubscriptionNeDeviceStatus"]].Value),
                    SubKeepAliveError = obj[props["SubKeepAliveError"]].Value as String,
                    SubscripeAlarmError = obj[props["SubscripeAlarmError"]].Value as String,
                    SubscripeNeDeviceError = obj[props["SubscripeNeDeviceError"]].Value as String,
                    //LatestConnectInfo = obj[props["LatestConnectInfo"]].Value as String,
                    //LatestStatus = obj[props["LatestStatus"]].Value as string,
                };

                return appliance;
            };
        }

        /// <summary>
        /// 根据IP查找ESight实体。
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <returns>The <see cref="HWESightHost" />.</returns>
        public HWESightHost GetEntityByHostIp(string hostIp)
        {
            IObjectReader<EnterpriseManagementObject> items =
                OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{hostIp}'");
            return items.Select(ConvertMonitoringObjectToESightHost()).FirstOrDefault();
        }
        #region UpdateESight

        /// <summary>
        /// Updates the esight alarm.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="alarmStatus">The alarm status.</param>
        /// <param name="error">The error.</param>
        public void UpdateESightKeepAlive(string hostIp, int alarmStatus, string error)
        {
            IObjectReader<EnterpriseManagementObject> items =
                OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{hostIp}'");
            EnterpriseManagementObject managementObject = items.FirstOrDefault();
            if (managementObject == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }

            var props = OM12Connection.GetManagementPackProperties(managementObject);
            managementObject[props["LastModifiedOn"]].Value = DateTime.UtcNow;
            managementObject[props["SubKeepAliveError"]].Value = error;
            managementObject[props["SubKeepAliveStatus"]].Value = alarmStatus;

            managementObject.Overwrite();
        }


        /// <summary>
        /// Updates the esight alarm.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="alarmStatus">The alarm status.</param>
        /// <param name="error">The error.</param>
        public void UpdateESightAlarm(string hostIp, int alarmStatus, string error)
        {
            IObjectReader<EnterpriseManagementObject> items =
                OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{hostIp}'");
            EnterpriseManagementObject managementObject = items.FirstOrDefault();
            if (managementObject == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }

            var props = OM12Connection.GetManagementPackProperties(managementObject);
            managementObject[props["LastModifiedOn"]].Value = DateTime.UtcNow;
            managementObject[props["SubscripeAlarmError"]].Value = error;
            managementObject[props["SubscriptionAlarmStatus"]].Value = alarmStatus;

            managementObject.Overwrite();
        }

        /// <summary>
        /// Updates the esight alarm.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="alarmStatus">The alarm status.</param>
        /// <param name="error">The error.</param>
        public void UpdateESightNeDevice(string hostIp, int alarmStatus, string error)
        {
            IObjectReader<EnterpriseManagementObject> items =
                OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{hostIp}'");
            EnterpriseManagementObject managementObject = items.FirstOrDefault();
            if (managementObject == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }

            var props = OM12Connection.GetManagementPackProperties(managementObject);
            managementObject[props["LastModifiedOn"]].Value = DateTime.UtcNow;
            managementObject[props["SubscripeNeDeviceError"]].Value = error;
            managementObject[props["SubscriptionNeDeviceStatus"]].Value = alarmStatus;

            managementObject.Overwrite();
        }

        /// <summary>
        /// Updates the esight password.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="enPwd">The pwd.</param>
        public void UpdateESightPwd(string hostIp, string enPwd)
        {

            //  TODO remove later?
            //var list = this.GetList();
            //var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            //if (oldModel == null)
            //{
            //    throw new Exception($"Can not find the eSight:{hostIp}");
            //}
            //list.Remove(oldModel);
            //oldModel.LastModifyTime = DateTime.Now;
            //oldModel.LoginPd = enPwd;
            //list.Add(oldModel);
            //using (var fs = new FileStream(this.filePath, FileMode.Create))
            //{
            //    var bf = new BinaryFormatter();
            //    bf.Serialize(fs, list);
            //}
        }


        /// <summary>
        /// Updates  esight connect status.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="latestStatus">The latest status.</param>
        /// <param name="latestConnectInfo">The latest connect information.</param>
        public void UpdateESightConnectStatus(string hostIp, string latestStatus, string latestConnectInfo)
        {
            IObjectReader<EnterpriseManagementObject> items =
                OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{hostIp}'");
            EnterpriseManagementObject managementObject = items.FirstOrDefault();
            if (managementObject == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }

            var props = OM12Connection.GetManagementPackProperties(managementObject);
            managementObject[props["LastModifiedOn"]].Value = DateTime.UtcNow;
            managementObject[props["LatestStatus"]].Value = latestStatus;
            managementObject[props["LatestConnectInfo"]].Value = latestConnectInfo;

            managementObject.Overwrite();
        }

        #endregion

    }
}