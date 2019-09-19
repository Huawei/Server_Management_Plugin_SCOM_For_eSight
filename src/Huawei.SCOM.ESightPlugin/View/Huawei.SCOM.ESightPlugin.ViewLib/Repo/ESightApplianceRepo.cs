//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using Huawei.SCOM.ESightPlugin.ViewLib.Client;
using Huawei.SCOM.ESightPlugin.ViewLib.Model;
using Huawei.SCOM.ESightPlugin.ViewLib.OM12R2;
using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Monitoring;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Result = Huawei.SCOM.ESightPlugin.ViewLib.Model.Result;

namespace Huawei.SCOM.ESightPlugin.ViewLib.Repo
{

    public class ESightApplianceRepo : INotifyPropertyChanged
    {
        private const string PatternDigits = @"^[a-zA-Z0-9-_\.]{1,100}$";
        private const string PatternAccount = "[\">+<;\\\\#&?%=/'®©]+";

        #region Private Members
        private string keyword;
        #endregion //Private Members

        #region Public Members
        public List<ESightAppliance> Items { get; set; } = new List<ESightAppliance>();
        public ObservableCollection<ESightAppliance> FilteredItems { get; set; } = new ObservableCollection<ESightAppliance>();
        #endregion //Public Members

        #region Constructors
        public ESightApplianceRepo()
        {

        }
        #endregion //Constructor

        #region Load Appliance List
        public async Task<Result<List<ESightAppliance>>> LoadAll()
        {
            try
            {
                var getListResult = await OM12ESightApplianceRepo.Instance.All();
                if (!getListResult.Success)
                {
                    LogHelper.Error(getListResult.Cause, getListResult.Message);
                    return Result<List<ESightAppliance>>.Failed(getListResult.Code, getListResult.Message, getListResult.Cause);
                }
                this.Items = getListResult.Data.Select(x => GetModelFromMpObject(x)).ToList();
                foreach (var appliance in this.Items)
                {
                    if (PingFd(appliance.Host))
                    {
                        appliance.LatestStatus = Constants.ESightConnectionStatus.ONLINE;
                    }
                    else
                    {
                        appliance.LatestStatus = Constants.ESightConnectionStatus.FAILED;
                        LogHelper.Info("Can not connect the remote server.", $"PingFd Error:");
                    }
                }
                Filter();
                return Result<List<ESightAppliance>>.Done(this.Items);
            }
            catch (Exception ex)
            {

                LogHelper.Error(ex, "LoadAll");
                return Result<List<ESightAppliance>>.Failed("LoadAll", ex);
            }
        }
        private bool PingFd(string nameOrAddress)
        {
            var pingable = false;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                var reply = pinger.Send(nameOrAddress, 1000);
                if (reply != null)
                {
                    pingable = reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("PingFd Error:", ex);
            }
            finally
            {
                pinger?.Dispose();
            }
            return pingable;
        }
        private ESightAppliance GetModelFromMpObject(EnterpriseManagementObject obj)
        {
            var props = OM12Connection.GetManagementPackProperties(obj);
            var model = new ESightAppliance()
            {
                Host = obj[props["Host"]].Value as String,
                Port = obj[props["Port"]].Value.ToString(),
                AliasName = obj[props["AliasName"]].Value as String,
                SystemId = obj[props["SystemId"]].Value as String,
                LoginAccount = obj[props["LoginAccount"]].Value as String,
                LoginPassword = obj[props["LoginPassword"]].Value as String,
                CreatedOn = ((DateTime)obj[props["CreatedOn"]].Value).ToLocalTime(),
                LastModifiedOn = ((DateTime)obj[props["LastModifiedOn"]].Value).ToLocalTime(),
                SubscriptionAlarmStatus = Convert.ToInt32(obj[props["SubscriptionAlarmStatus"]].Value),
                SubscriptionNeDeviceStatus = Convert.ToInt32(obj[props["SubscriptionNeDeviceStatus"]].Value),
                //LatestStatus = obj[props["LatestStatus"]].Value as string,
                SubscribeID = obj[props["SubscribeID"]].Value as string,
            };
            LogHelper.Info($"Host[{model.Host}]  Port[{model.Port}] LoginAccount[{model.LoginAccount}] SubscribeID[{model.SubscribeID}] ");
            return model;
        }

        #endregion // Load Appliance List

        #region Filter 
        public void Filter(String keyword)
        {
            this.keyword = keyword;
            this.Filter();
        }

        private void Filter()
        {

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                FilteredItems.Clear();
                foreach (var item in Items)
                {
                    if (this.keyword != null && this.keyword.Trim() != "")
                    {
                        if (item.Host.Contains(this.keyword) || item.AliasName.Contains(this.keyword))
                        {
                            FilteredItems.Add(item);
                        }
                    }
                    else
                    {
                        FilteredItems.Add(item);
                    }
                }

                OnPropertyChanged("FilteredItems");
            }));


        }
        #endregion //Filter 

        #region Test Appliance 
        internal async Task<Result> Test(ESightAppliance appliance)
        {
            try
            {
                Result validateResult = Validate(appliance);
                if (!validateResult.Success)
                {
                    return validateResult;

                }
                var client = new ESightClient(appliance);
                return await client.TestCredential();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Test");
                return Result.Failed(100, "Test", ex);
            }

        }

        #endregion //Test Appliance 

        #region Add Appliance 
        internal async Task<Result> Add(ESightAppliance appliance)
        {
            try
            {
                Result validateResult = Validate(appliance);
                if (!validateResult.Success)
                {
                    return validateResult;
                }
                var client = new ESightClient(appliance);
                var result = await client.TestCredential();
                if (!result.Success)
                {
                    return result;
                }
                var addResult = await OM12ESightApplianceRepo.Instance.Add(appliance);
                if (addResult.Success)
                {
                    await this.LoadAll();
                }
                return addResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Add");
                return Result.Failed(100, "Add", ex);
            }

        }

        #endregion //Add Appliance 

        #region Update Appliance 
        internal async Task<Result> Update(ESightAppliance appliance)
        {
            try
            {
                Result validateResult = Validate(appliance);
                if (!validateResult.Success)
                {
                    return validateResult;
                }
                if (appliance.UpdateCredential)//修改了密码
                {
                    var client = new ESightClient(appliance);
                    var result = await client.TestCredential();
                    if (!result.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    var oldESightObj = await OM12ESightApplianceRepo.Instance.FindByHost(appliance.Host);
                    if (oldESightObj.Data == null)
                    {
                        return Result.Failed(104, $"eSight {appliance.Host} can not find.");
                    }
                    var oldESight = GetModelFromMpObject(oldESightObj.Data);
                    if (oldESight.Port != appliance.Port) //修改了端口
                    {
                        appliance.LoginPassword = oldESight.LoginPassword;
                        var client = new ESightClient(appliance);
                        var result = await client.TestCredential();
                        if (!result.Success)
                        {
                            return result;
                        }
                    }

                }
                var updateResult = await OM12ESightApplianceRepo.Instance.Update(appliance);
                if (updateResult.Success)
                {
                    await this.LoadAll();
                }
                return updateResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Update");
                return Result.Failed(100, "Update", ex);
            }

        }

        #endregion //Update Appliance 

        #region Delete Appliance 
        public async Task<Result> Delete(ESightAppliance appliance)
        {
            try
            {
                var deleteResult = await OM12ESightApplianceRepo.Instance.Delete(appliance);
                if (deleteResult.Success)
                {
                    await this.LoadAll();
                }
                return deleteResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Test");
                return Result.Failed(100, "Test", ex);
            }
        }
        #endregion //Delete Appliance 

        #region Validate Appliance 
        public Result Validate(ESightAppliance appliance)
        {

            // host 
            if (string.IsNullOrEmpty(appliance.Host))
            {
                return Result.Failed(1000, "Host should not be null or empty.");
            }


            IPAddress Address;
            bool isValidIPAddr = IPAddress.TryParse(appliance.Host, out Address);
            bool isValidDomain = Uri.CheckHostName(appliance.Host) != UriHostNameType.Unknown;
            if (!isValidIPAddr && !isValidDomain)
            {
                return Result.Failed(1000, "Host should be a valid IP adress or domain name.");
            }
            if (isValidIPAddr)
            {
                if (Address.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    if (!Regex.IsMatch(appliance.Host, "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"))
                    {
                        return Result.Failed(1000, "Host should be a valid IP adress or domain name.");
                    }
                }
            }


            // port
            if (string.IsNullOrEmpty(appliance.Port))
            {
                return Result.Failed(1001, "Port should not be null or empty.");
            }

            int PortAsInt;
            bool isNumeric = int.TryParse(appliance.Port, out PortAsInt);
            if (isNumeric)
            {
                if (PortAsInt < 1 || PortAsInt > 65535)
                {
                    return Result.Failed(1001, "Port should be a digits between 1 and 65535.");
                }

            }
            else
            {
                return Result.Failed(1001, "Port should be a digits between 1 and 65535.");
            }

            // System Id
            if (string.IsNullOrEmpty(appliance.SystemId))
            {
                return Result.Failed(1001, "SystemId should not be null or empty.");
            }

            if (!Regex.IsMatch(appliance.SystemId, PatternDigits))
            {
                return Result.Failed(1001, "SystemId should contains 1 to 100 characters, which can include letters, digits, hyphens (-), underscores (_), and periods(.).");
            }


            if (appliance.UpdateCredential)
            {
                // Login Account
                if (string.IsNullOrEmpty(appliance.LoginAccount))
                {
                    return Result.Failed(1001, "Account should not be null or empty.");
                }
                if (appliance.LoginAccount.Length > 32 || appliance.LoginAccount.Length < 6)
                {
                    return Result.Failed(1001, "Account should contains 6 to 32 characters. The following special characters are not allowed: \"#%&'+/;<=>?\\©®");
                }
                if (Regex.Match(appliance.LoginAccount, PatternAccount).Success)
                {
                    return Result.Failed(1001, "Account should contains 6 to 32 characters. The following special characters are not allowed: \"#%&'+/;<=>?\\©®");
                }
                // Login Password
                if (string.IsNullOrEmpty(appliance.LoginPassword))
                {
                    return Result.Failed(1001, "Password should not be null or empty.");
                }
                if (appliance.LoginPassword.Length > 32 || appliance.LoginPassword.Length < 8)
                {
                    return Result.Failed(1001, "The password can contain only 8 to 32 characters");
                }
                char[] charArray = appliance.LoginAccount.ToCharArray();
                Array.Reverse(charArray);
                var reverseAccount = new string(charArray);
                if (Regex.Match(appliance.LoginPassword, appliance.LoginAccount).Success || Regex.Match(appliance.LoginPassword, reverseAccount).Success)
                {
                    return Result.Failed(1001, "The password cannot contain the user name or the reverse of the user name.");
                }
                var regexPwd = new Regex(@"
(?=.*[0-9])                              #必须包含数字
(?=.*[a-z])                              #必须包含小写字母
(?=.*[A-Z])                              #必须包含大写字母
.{8,32}                                  #至少8个字符，最多32个字符
", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

                if (!regexPwd.IsMatch(appliance.LoginPassword))
                {
                    return Result.Failed(1001, "The password must contain 1 uppercase letter, 1 lowercase letter, and 1 digit.");
                }
                char[] charArray1 = appliance.LoginPassword.ToCharArray();
                var count = charArray1.GroupBy(e => e).OrderByDescending(e => e.Count()).First().ToList().Count();
                if (count>2)
                {
                    return Result.Failed(1001, "Each character in the password cannot be used more than 2 times");
                }
            }
            return Result.Done();
        }
        #endregion //Validate Credential 

        #region NotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion //NotifyPropertyChanged 
    }


}
