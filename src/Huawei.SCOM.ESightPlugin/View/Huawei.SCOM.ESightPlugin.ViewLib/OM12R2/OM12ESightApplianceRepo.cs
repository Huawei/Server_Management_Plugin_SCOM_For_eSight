//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Huawei.SCOM.ESightPlugin.ViewLib.Model;
using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Huawei.SCOM.ESightPlugin.ViewLib.Model.Constants;
using Result = Huawei.SCOM.ESightPlugin.ViewLib.Model.Result;

namespace Huawei.SCOM.ESightPlugin.ViewLib.OM12R2
{
    public class OM12ESightApplianceRepo
    {

        private static OM12ESightApplianceRepo instance;

        public static OM12ESightApplianceRepo Instance => instance ?? (instance = new OM12ESightApplianceRepo());
        public static ManagementPackClass GetMPClass()
        {
            return OM12Connection.GetManagementPackClass(ESightAppliance.EntityClassName);
        }

        public async Task<Result<List<EnterpriseManagementObject>>> All()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var objects = OM12Connection.All<EnterpriseManagementObject>(ESightAppliance.EntityClassName);
                    return Result<List<EnterpriseManagementObject>>.Done(objects.ToList());
                }
                catch (Exception e)
                {
                    return Result<List<EnterpriseManagementObject>>.Failed("Get List Error", e);
                }
            });
        }

        public async Task<Result<EnterpriseManagementObject>> FindByHost(string host)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var objects = OM12Connection.Query<EnterpriseManagementObject>(ESightAppliance.EntityClassName, $"Host='{host}'");
                    return Result<EnterpriseManagementObject>.Done(objects.FirstOrDefault());
                }
                catch (Exception e)
                {
                    return Result<EnterpriseManagementObject>.Failed(100, $"Internal error caused by {e.Message}", e);
                }
            });
        }

        public async Task<Result> Add(ESightAppliance appliance)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (OM12Connection.Exsits(ESightAppliance.EntityClassName, $"Host='{appliance.Host}'"))
                    {
                        return Result.Failed(101, $"ESight {appliance.Host} already exsits.");
                    }

                    IncrementalDiscoveryData incrementalDiscoveryData = new IncrementalDiscoveryData();

                    // add appliance record
                    ManagementPackClass MPClass = GetMPClass();
                    CreatableEnterpriseManagementObject EMOAppliance =
                        new CreatableEnterpriseManagementObject(OM12Connection.HuaweiESightMG, MPClass);
                    IDictionary<string, ManagementPackProperty> props =
                        OM12Connection.GetManagementPackProperties(EMOAppliance);
                    EMOAppliance[props["Host"]].Value = appliance.Host;
                    EMOAppliance[props["Port"]].Value = appliance.Port;
                    EMOAppliance[props["AliasName"]].Value = appliance.AliasName;
                    EMOAppliance[props["SystemId"]].Value = appliance.SystemId;
                    EMOAppliance[props["LoginAccount"]].Value = appliance.LoginAccount;
                    EMOAppliance[props["LoginPassword"]].Value = RijndaelManagedCrypto.Instance
                        .EncryptForCS(appliance.LoginPassword);
                    EMOAppliance[props["LastModifiedOn"]].Value = DateTime.UtcNow;
                    EMOAppliance[props["CreatedOn"]].Value = DateTime.UtcNow;

                    EMOAppliance[props["OpenID"]].Value = Guid.NewGuid().ToString("D");
                    EMOAppliance[props["SubscribeID"]].Value = Guid.NewGuid().ToString("D");
                    EMOAppliance[props["SubKeepAliveStatus"]].Value = 0;
                    EMOAppliance[props["SubscriptionAlarmStatus"]].Value = 0;
                    EMOAppliance[props["SubscriptionNeDeviceStatus"]].Value = 0;

                    EMOAppliance[props["SubKeepAliveError"]].Value = string.Empty;
                    EMOAppliance[props["SubscripeAlarmError"]].Value = string.Empty;
                    EMOAppliance[props["SubscripeNeDeviceError"]].Value = string.Empty;
                    //EMOAppliance[props["LatestConnectInfo"]].Value = string.Empty;

                    //EMOAppliance[props["LatestStatus"]].Value = Constants.ESightConnectionStatus.NONE;

                    ManagementPackClass baseEntity = OM12Connection.GetManagementPackClass("System.Entity");
                    EMOAppliance[baseEntity, "DisplayName"].Value = appliance.Host;
                    incrementalDiscoveryData.Add(EMOAppliance);
                    incrementalDiscoveryData.Commit(OM12Connection.HuaweiESightMG);
                    return Result.Done();
                }
                catch (Exception e)
                {
                    return Result.Failed(100, $"Internal error caused by {e.Message}", e);
                }
            });
        }


        public async Task<Result> Update(ESightAppliance appliance)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var obj = await this.FindByHost(appliance.Host);
                    var exsitObj = obj.Data;
                    if (exsitObj == null)
                    {
                        return Result.Failed(104, $"ESight {appliance.Host} can not find.");
                    }
                    IncrementalDiscoveryData incrementalDiscoveryData = new IncrementalDiscoveryData();

                    // update appliance record
                    ManagementPackClass MPClass = GetMPClass();
                    EnterpriseManagementObject managementObject = obj.Data;
                    var props = OM12Connection.GetManagementPackProperties(managementObject);
                    managementObject[props["Port"]].Value = appliance.Port;
                    managementObject[props["AliasName"]].Value = appliance.AliasName;
                    string currentSystemId = managementObject[props["SystemId"]].Value as string;
                    bool IsSystemIdChanged = currentSystemId != appliance.SystemId;
                    if (IsSystemIdChanged)
                    {
                        managementObject[props["SystemId"]].Value = appliance.SystemId;
                        managementObject[props["SubscribeID"]].Value = Guid.NewGuid().ToString("D");
                        managementObject[props["SubKeepAliveStatus"]].Value = 0;
                        managementObject[props["SubscriptionAlarmStatus"]].Value = 0;
                        managementObject[props["SubscriptionNeDeviceStatus"]].Value = 0;
                        managementObject[props["SubKeepAliveError"]].Value = string.Empty;
                        managementObject[props["SubscripeAlarmError"]].Value = string.Empty;
                        managementObject[props["SubscripeNeDeviceError"]].Value = string.Empty;
                    }
                    if (appliance.UpdateCredential)
                    {
                        managementObject[props["LoginAccount"]].Value = appliance.LoginAccount;
                        managementObject[props["LoginPassword"]].Value = RijndaelManagedCrypto.Instance
                            .EncryptForCS(appliance.LoginPassword);
                    }
                    managementObject[props["LastModifiedOn"]].Value = DateTime.UtcNow;
                    incrementalDiscoveryData.Add(managementObject);
                    incrementalDiscoveryData.Commit(OM12Connection.HuaweiESightMG);
                    return Result.Done();
                }
                catch (Exception e)
                {
                    return Result.Failed(100, $"Internal error caused by {e.Message}", e);
                }
            });
        }

        public async Task<Result> Delete(ESightAppliance appliance)
        {
            return await Task.Run(async () =>
            {
                IncrementalDiscoveryData incrementalDiscoveryData = new IncrementalDiscoveryData();
                try
                {
                    var obj = await this.FindByHost(appliance.Host);
                    var exsitObj = obj.Data;
                    if (exsitObj == null)
                    {
                        return Result.Failed(104, $"{appliance.Host} does not exists, delete failed.");
                    }
                    incrementalDiscoveryData.Remove(obj.Data);
                    incrementalDiscoveryData.Commit(OM12Connection.HuaweiESightMG);
                    return Result.Done();
                }
                catch (Exception e)
                {
                    return Result.Failed(100, $"Internal error caused by {e.Message}", e);
                }
            });
        }
    }
}
