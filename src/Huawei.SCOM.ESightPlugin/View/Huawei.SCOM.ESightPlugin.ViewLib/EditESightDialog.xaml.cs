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
using Huawei.SCOM.ESightPlugin.ViewLib.Repo;
using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Huawei.SCOM.ESightPlugin.ViewLib
{
    /// <summary>
    /// EditESightDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditESightDialog : Window, INotifyPropertyChanged
    {
        private ESightApplianceRepo ESightApplianceRepo { get; set; }

        private bool _updateCredentialChecked = false;
        private ESightAppliance _item = new ESightAppliance();
        private Result _actionResult = Result.Done();

        public bool UpdateCredentialChecked
        {
            get
            {
                return this._updateCredentialChecked;
            }
            set
            {
                if (value != this._updateCredentialChecked)
                {
                    this._updateCredentialChecked = value;
                    this.OnPropertyChanged("UpdateCredentialChecked");
                }
            }
        }

        public ESightAppliance Item
        {
            get
            {
                return this._item;
            }

            set
            {
                if (_item != value)
                {
                    _item = value;
                    this.OnPropertyChanged("ActionResult");
                }
            }
        }
        public Result ActionResult
        {
            get
            {
                return _actionResult;
            }

            set
            {

                if (_actionResult != value)
                {
                    _actionResult = value;
                    this.OnPropertyChanged("ActionResult");
                }
            }
        }


        public EditESightDialog(ESightApplianceRepo repo)
        {
            InitializeComponent();
            this.ESightApplianceRepo = repo;
            this.DataContext = this;
            this.ShowInTaskbar = false;
        }

        public void SetItem(ESightAppliance item)
        {
            this.Item = item;
            // using binding instead?
            txtHost.Text = item.Host;
            txtAlias.Text = item.AliasName;
            txtPort.Text = item.Port;
            txtSystemId.Text = item.SystemId;
            txtAccount.Text = item.LoginAccount;
            // txtPassword.Password = item.LoginPd;
        }


        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                LogHelper.Error("OnMouseLeftButtonDown Error:",ex);
            }
        }
        private async void OnSaveBtnClicked(object sender, RoutedEventArgs e)
        {
            this.ActionResult = Result.Done();
            ESightAppliance appliance = new ESightAppliance
            {
                Host = txtHost.Text,
                Port = txtPort.Text,
                SystemId = txtSystemId.Text,
                AliasName = txtAlias.Text,
                LoginAccount = txtAccount.Text,
                LoginPassword = txtPassword.Password,
                UpdateCredential = UpdateCredentialChecked,
            };
            LogHelper.Info($"Update eSight:{txtHost.Text}、{txtPort.Text}、{txtSystemId.Text}、{txtAlias.Text}、{txtAccount.Text}");

            this.ActionResult = await ESightApplianceRepo.Update(appliance);
            this.btnSave.IsEnabled = true;
            this.btnSave.Content = "Save";
            if (this.ActionResult.Success)
            {
                LogHelper.Info($"Update eSight ({appliance.Host}) Success.");
                this.Close();
            }
            else
            {
                LogHelper.Error(this.ActionResult.Cause, $"Update eSight ({appliance.Host}) Faild:" + this.ActionResult.Message);
            }
        }

        private async void OnTestBtnClicked(object sender, RoutedEventArgs e)
        {
            this.ActionResult = Result.Done();
            ESightAppliance appliance = new ESightAppliance
            {
                Host = txtHost.Text,
                Port = txtPort.Text,
                SystemId = txtSystemId.Text,
                AliasName = txtAlias.Text,
                LoginAccount = txtAccount.Text,
                LoginPassword = txtPassword.Password,
                UpdateCredential = UpdateCredentialChecked,
            };
            this.ActionResult = await ESightApplianceRepo.Test(appliance);
            this.btnTest.IsEnabled = true;
            this.btnTest.Content = "Test";
            if (this.ActionResult.Success)
            {
                LogHelper.Info($"Test eSight ({appliance.Host}) Success.");
            }
            else
            {
                LogHelper.Error(this.ActionResult.Cause, $"Test eSight ({appliance.Host}) Faild:" + this.ActionResult.Message);
            }
        }
       
        private void OnCloseBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region NotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion //NotifyPropertyChanged 
    }
}
