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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Huawei.SCOM.ESightPlugin.ViewLib.Repo;
using Huawei.SCOM.ESightPlugin.ViewLib.Model;
using Huawei.SCOM.ESightPlugin.ViewLib.Utils;
using System.Windows.Threading;

namespace Huawei.SCOM.ESightPlugin.ViewLib
{

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public partial class ESightConfigDashboard : UserControl, INotifyPropertyChanged
    {
        private Result _actionResult = Result.Done();


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

        public ESightApplianceRepo ESightApplianceRepo { get; set; }

        public ESightConfigDashboard()
        {
            InitializeComponent();
            this.ESightApplianceRepo = this.Resources["ESightApplianceRepo"] as ESightApplianceRepo;
            this.Loaded += ESightConfigDashboard_Loaded;
            this.dispatcherTimer_Tick(null, null);//首先触发一次
        }

        private void ESightConfigDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            // Start dispatcher timer
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();
        }

        //Refreshes grid data on timer tick
        protected void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                var result = await this.ESightApplianceRepo.LoadAll();
                if (result.Success)
                {
                    LogHelper.Info("Load eSight list success!");
                }
                else
                {
                    LogHelper.Error("Load eSight list failed! errorInfo:{0}", new Exception(result.Message));
                }
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    this.ActionResult = result;
                    add_btn.IsEnabled = true;
                }));
            });
        }

        public void OnSearchESight()
        {
            string keyword = txtSearchKeyword.Text;
            this.ESightApplianceRepo.Filter(keyword);
            // this.UpdateGridItemSource();
        }

        private void OnGridLoaded(object sender, RoutedEventArgs e)
        {
            // ... Assign ItemsSource of DataGrid.
            // grid = sender as DataGrid;
            // OnSearchESight();
        }


        private async void OnDeleteESight(object sender, RoutedEventArgs e)
        {
            int selectedIndex = Grid.SelectedIndex;
            if (Grid.SelectedIndex > -1 && selectedIndex < this.ESightApplianceRepo.FilteredItems.Count)
            {
                ESightAppliance appliance = this.ESightApplianceRepo.FilteredItems[Grid.SelectedIndex];
                MessageBoxResult confirmResult =
                    MessageBox.Show("Are you sure you want to delete the eSight?",
                                        "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    this.ActionResult = await this.ESightApplianceRepo.Delete(appliance);
                    if (!this.ActionResult.Success)
                    {
                        LogHelper.Error(this.ActionResult.Cause, $"Delete eSight ({appliance.Host}) Faild:" + this.ActionResult.Message);
                        MessageBox.Show(this.ActionResult.Message);
                    }
                    else
                    {
                        LogHelper.Info($"Delete eSight ({appliance.Host}) Success.");
                    }
                }
            }
        }
        private void ShowEditESightDialog(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedIndex > -1 && Grid.SelectedIndex < this.ESightApplianceRepo.FilteredItems.Count)
            {
                this.Effect = new BlurEffect();
                EditESightDialog dialog = new EditESightDialog(this.ESightApplianceRepo);

                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.SetItem(this.ESightApplianceRepo.FilteredItems[Grid.SelectedIndex]);
                dialog.ShowDialog();
                this.Effect = null;
            }
        }

        private void ShowAddESightDialog(object sender, RoutedEventArgs e)
        {
            this.Effect = new BlurEffect();
            AddESightDialog dialog = new AddESightDialog(this.ESightApplianceRepo);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.ShowDialog();
            this.Effect = null;
        }

        public void OnEditESight(string host, string alias, string port, string systemId, string account, string password)
        {
            if (Grid.SelectedIndex > -1 && Grid.SelectedIndex < this.ESightApplianceRepo.FilteredItems.Count)
            {
                // Items[grid.SelectedIndex] = new ServerData(host, alias, port, systemId, account, password);
                OnSearchESight();
            }
        }

        public void OnAddESight(string host, string alias, string port, string systemId, string account, string password)
        {
            // Items.Add(new ServerData(host, alias, port, systemId, account, password));
            OnSearchESight();
        }


        private void OnSearchKeywordDataContextChanged(object Sender, DependencyPropertyChangedEventArgs e)
        {
            OnSearchESight();
        }

        private void OnSearchKeywordChanged(object sender, TextChangedEventArgs e)
        {
            OnSearchESight();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
