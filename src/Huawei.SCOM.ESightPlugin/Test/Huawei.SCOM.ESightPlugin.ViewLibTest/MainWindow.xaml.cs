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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Huawei.SCOM.ESightPlugin.ViewLibTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Guid _ = Guid.NewGuid();
            //Console.Write(_);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //HealthDashboard.AddBladeServerSeries(ViewLib.HealthStatus.OK, 10);
            //HealthDashboard.AddKunLunServerSeries(ViewLib.HealthStatus.OK, 10);
            //HealthDashboard.AddHighDensityServerSeries(ViewLib.HealthStatus.OK, 10);
            //HealthDashboard.AddRackServerSeries(ViewLib.HealthStatus.OK, 10);
        }

    }
}
