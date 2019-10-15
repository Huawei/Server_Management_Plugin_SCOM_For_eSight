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
// Assembly         : ESightPlugin.TestClient
// Author           : yayun
// Created          : 11-14-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="FormMain.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>Class FormMain.</summary>
// ***********************************************************************

// ReSharper disable All

using System.Collections.Generic;

namespace ESightPlugin.TestClient
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Core;
    using Huawei.SCOM.ESightPlugin.Core.Models;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.Models.Devices;
    using Huawei.SCOM.ESightPlugin.Models.Server;
    using Huawei.SCOM.ESightPlugin.Service;

    /// <summary>
    ///     Class FormMain.
    /// </summary>
    public partial class FormMain : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class.
        /// </summary>
        public FormMain()
        {
            this.InitializeComponent();
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the blade test.
        /// </summary>
        /// <value>The blade test.</value>
        public BladeServer BladeTest { get; set; }

        /// <summary>
        ///     Gets or sets the high test.
        /// </summary>
        /// <value>The high test.</value>
        public HighdensityServer HighTest { get; set; }

        /// <summary>
        /// Gets or sets the kun lun test.
        /// </summary>
        public KunLunServer KunLunTest { get; set; }

        /// <summary>
        /// Gets or sets the rack test.
        /// </summary>
        public RackServer RackTest { get; set; }

        public EventData BladeEventData { get; set; }

        public EventData ChildBladeEventData { get; set; }

        public EventData SwitchEventData { get; set; }

        public EventData HighEventData { get; set; }

        public EventData ChildHighEventData { get; set; }

        public EventData KunLunEventData { get; set; }

        public EventData RackEventData { get; set; }

        public DeviceChangeEventData DeviceChangeEventData { get; set; }

        public string ESightIp => "192.168.0.1";
        #endregion

        #region Methods

        /// <summary>
        /// The get details.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="HWDeviceDetail"/>.
        /// </returns>
        private HWDeviceDetail GetDetails(string filename)
        {
            var path = Application.StartupPath + "//..//..//..//..//..//..//..//..//mockNew//ServerData//" + filename;

            var txt = File.ReadAllText(path).Replace("module.exports =", string.Empty);

            var da = JsonUtil.DeserializeObject<QueryListResult<HWDeviceDetail>>(txt);
            return da.Data.First();
        }

        /// <summary>
        /// The get main.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="HWDevice"/>.
        /// </returns>
        private HWDevice GetMain(string filename)
        {
            var path = Application.StartupPath + "//..//..//..//..//..//..//..//..//mockNew//ServerData//" + filename;

            var txt = File.ReadAllText(path).Replace("module.exports =", string.Empty);

            var da = JsonUtil.DeserializeObject<QueryPageResult<HWDevice>>(txt);

            return da.Data.First();
        }

        /// <summary>
        /// The form 1_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Reload();
        }

        /// <summary>
        /// The reload.
        /// </summary>
        public void Reload()
        {
            var bladeMain = this.GetMain("bladeList.js");
            var highdensityMain = this.GetMain("highdensityList.js");

            var bladeChildDetial = this.GetDetails("bladeChildDetial.js");
            var bladeDetial = this.GetDetails("bladeDetial.js");

            var highdensityChildDetial = this.GetDetails("highdensityChildDetial.js");
            var highdensityDetial = this.GetDetails("highdensityDetial.js");

            // var rackMain = this.GetMain("rackList.js");
            var rackDetail = this.GetDetails("rackDetail.js");

            // var kunLunMain = this.GetMain("kunlunList.js");
            var kunLunDetail = this.GetDetails("kunlunDetail.js");

            this.BladeTest = new BladeServer(bladeMain);
            bladeMain.ChildBlades.ForEach(
                m =>
                    {
                        var childBlade = new ChildBlade(m, this.ESightIp);
                        childBlade.MakeChildBladeDetail(bladeChildDetial);
                        this.BladeTest.ChildBlades.Add(childBlade);
                    });
            this.BladeTest.MakeDetail(bladeDetial, ESightIp);

            this.HighTest = new HighdensityServer(highdensityMain);
            highdensityMain.ChildBlades.ForEach(
                m =>
                    {
                        var childHighdensity = new ChildHighdensity(m, this.ESightIp);
                        childHighdensity.MakeChildBladeDetail(highdensityChildDetial);
                        this.HighTest.ChildHighdensitys.Add(childHighdensity);
                    });
            this.HighTest.MakeDetail(highdensityDetial, ESightIp);

            this.RackTest = new RackServer();
            this.RackTest.MakeDetail(rackDetail, ESightIp);

            this.KunLunTest = new KunLunServer();
            this.KunLunTest.MakeDetail(kunLunDetail, ESightIp);

            #region MyRegion
            var path = Application.StartupPath + "//..//..//..//..//..//..//..//..//mockNew//alarmData//alarmData.js";
            var data = File.ReadAllText(path).Replace("module.exports =", string.Empty);

            this.BladeEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "Blade")), ESightIp, ServerTypeEnum.Blade);
            this.ChildBladeEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "ChildBlade")), ESightIp, ServerTypeEnum.ChildBlade);
            this.SwitchEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "Switch")), ESightIp, ServerTypeEnum.Switch);
            this.HighEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "High")), ESightIp, ServerTypeEnum.Highdensity);
            this.ChildHighEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "ChildHigh")), ESightIp, ServerTypeEnum.ChildHighdensity);
            this.KunLunEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "KunLun")), ESightIp, ServerTypeEnum.KunLun);
            this.RackEventData = new EventData(JsonUtil.DeserializeObject<AlarmData>(data.Replace("Rack", "Rack")), ESightIp, ServerTypeEnum.Rack);

            path = Application.StartupPath + "//..//..//..//..//..//..//..//..//mockNew//alarmData//deviceChangeData.js";
            data = File.ReadAllText(path).Replace("module.exports =", string.Empty);
            var daTemp = JsonUtil.DeserializeObject<NedeviceData>(data);
            this.DeviceChangeEventData = new DeviceChangeEventData(daTemp, "192.168" + "0.1", ServerTypeEnum.Blade);

            #endregion
        }
        #endregion

        #region Blade
        /// <summary>
        /// The btn delete blade server_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void btnDeleteBladeServer_Click(object sender, EventArgs e)
        {
            var dn = ESightIp + "-NE=346039091x";
            BladeConnector.Instance.RemoveServerByDeviceId(this.ESightIp, dn);
        }

        /// <summary>
        /// The btn delete child blade_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void btnDeleteChildBlade_Click(object sender, EventArgs e)
        {
            var dn = "NE=34603911";
            BladeConnector.Instance.RemoveChildBlade(this.ESightIp, $"{this.ESightIp}-{ dn}");
        }


        /// <summary>
        /// The btn insert blade server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInsertBladeServer_Click(object sender, EventArgs e)
        {
            BladeConnector.Instance.SyncServer(this.BladeTest);
        }

        /// <summary>
        /// The btn insert child blade_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateChildBlade_Click(object sender, EventArgs e)
        {
            var childBlade = this.BladeTest.ChildBlades.First();
            childBlade.Name += new Random().Next(0, 100) +childBlade.Name;
            BladeConnector.Instance.UpdateChildBlade(childBlade, false);
        }
        /// <summary>
        /// The btn insert main blade_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateMainBlade_Click(object sender, EventArgs e)
        {
            this.BladeTest.Location = new Random().Next(0, 100) + this.BladeTest.Location;
            BladeConnector.Instance.UpdateBlade(this.BladeTest, false);
        }


        #endregion

        #region Highdensity

        /// <summary>
        /// The btn delete child high density_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeleteChildHighDensity_Click(object sender, EventArgs e)
        {
            var dn = string.Empty;
            HighdensityConnector.Instance.RemoveChildHighDensityServer(this.ESightIp, $"{this.ESightIp}-{ dn}");
        }

        /// <summary>
        /// The btn delete high density server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeleteHighDensityServer_Click(object sender, EventArgs e)
        {
            var dn = string.Empty;
            HighdensityConnector.Instance.RemoveServerByDeviceId(this.ESightIp, dn);
        }

        /// <summary>
        /// The btn insert child high_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateChildHigh_Click(object sender, EventArgs e)
        {
            var childBlade = this.HighTest.ChildHighdensitys.First();
            childBlade.Name += new Random().Next(0, 100) +childBlade.Name;
            HighdensityConnector.Instance.UpdateChildBoard(childBlade, false);
        }

        /// <summary>
        /// The btn insert high density server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInsertHighDensityServer_Click(object sender, EventArgs e)
        {
            HighdensityConnector.Instance.SyncServer(this.HighTest);
        }

        /// <summary>
        /// The btn insert main high_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateMainHigh_Click(object sender, EventArgs e)
        {
            this.HighTest.Location = new Random().Next(0, 100) + this.HighTest.Location;
            HighdensityConnector.Instance.UpdateMain(this.HighTest, false);
        }
        #endregion

        #region Rack
        /// <summary>
        /// The btn delete rack_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeleteRack_Click(object sender, EventArgs e)
        {
            var dn = string.Empty;
            RackConnector.Instance.RemoveServerByDeviceId(this.ESightIp, dn);
        }


        /// <summary>
        /// The btn insert rack_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateRack_Click(object sender, EventArgs e)
        {
            this.RackTest.iBMCIPv4Address = new Random().Next(0, 100) + this.RackTest.iBMCIPv4Address;
            RackConnector.Instance.UpdateRack(this.RackTest, false);
        }


        /// <summary>
        /// The btn insert rack server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInsertRackServer_Click(object sender, EventArgs e)
        {
            RackConnector.Instance.SyncServer(this.RackTest);
        }

        #endregion

        #region Kunlun

        /// <summary>
        /// The btn delete kun lun server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeleteKunLunServer_Click(object sender, EventArgs e)
        {
            var dn = string.Empty;
            KunLunConnector.Instance.RemoveServerByDeviceId(this.ESightIp, dn);
        }

        /// <summary>
        /// The btn insert kun lun server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInsertKunLunServer_Click(object sender, EventArgs e)
        {
            KunLunConnector.Instance.SyncServer(this.KunLunTest);
        }

        /// <summary>
        /// The btn update kun lun server_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUpdateKunLunServer_Click(object sender, EventArgs e)
        {
            this.KunLunTest.IpAddress = new Random().Next(0, 10) + this.KunLunTest.IpAddress;
            KunLunConnector.Instance.UpdateKunLun(this.KunLunTest, false);
        }
        #endregion

        /// <summary>
        /// Handles the Click event of the btnDeleteAllServer control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private void btnDeleteAllServer_Click(object sender, EventArgs e)
        {
            BladeConnector.Instance.RemoverAllBlade();
            RackConnector.Instance.RemoverAllRack();
            HighdensityConnector.Instance.RemoverAllHighdensity();
        }

        /// <summary>
        /// The btn refresh_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Reload();
        }

        /// <summary>
        /// The btn test service_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnTestService_Click(object sender, EventArgs e)
        {
            var service = new ESightPluginService();
            service.Debug();
        }

        #region Event
        private void btnInsertEvent_Click(object sender, EventArgs e)
        {
            BladeEventData.OptType = 1;
            HighEventData.OptType = 1;
            ChildBladeEventData.OptType = 1;
            SwitchEventData.OptType = 1;
            ChildHighEventData.OptType = 1;
            KunLunEventData.OptType = 1;
            RackEventData.OptType = 1;
            //for (int i = 0; i < 10; i++)
            //{
            //    BladeConnector.Instance.InsertSwitchEvent(SwitchEventData);
            //    SwitchEventData.AlarmSn += 1;
            //}

            //SwitchEventData.LevelId = 2;
            //for (int i = 0; i < 10; i++)
            //{
            //    BladeConnector.Instance.InsertSwitchEvent(SwitchEventData);
            //    SwitchEventData.AlarmSn += 1;
            //}

           // BladeConnector.Instance.InsertHistoryEvent(new List<EventData>() { BladeEventData }, ServerTypeEnum.Blade, this.ESightIp);
            //HighdensityConnector.Instance.InsertEvent(HighEventData);
            //BladeConnector.Instance.InsertChildBladeEvent(ChildBladeEventData);
            //HighdensityConnector.Instance.InsertChildBladeEvent(ChildHighEventData);
            //KunLunConnector.Instance.InsertEvent(KunLunEventData);
            RackConnector.Instance.InsertEvent(RackEventData,this.ESightIp);
        }

        private void btnUpdateAlert_Click(object sender, EventArgs e)
        {
            RackEventData.OptType = 5;
            RackConnector.Instance.InsertEvent(RackEventData, this.ESightIp);
        }

        private void btnCloseAlert_Click(object sender, EventArgs e)
        {
            RackEventData.OptType = 2;
            RackConnector.Instance.InsertEvent(RackEventData, this.ESightIp);
        }
        private void btnInsertDeviceChange_Click(object sender, EventArgs e)
        {
            RackConnector.Instance.InsertDeviceChangeEvent(DeviceChangeEventData, this.ESightIp);
        }

        #endregion

        private void btnEnqueue_Click(object sender, EventArgs e)
        {
            var a = "{'subscribeId':'ceb158a9-a73b-4e78-8dd4-ee16297fe39a','tcpMessageType':0,'Desc':'Alarm','Id':'239ebaf8-552d-49e7-bf18-5066796e3a88','Data':{'SubscribeId':'ceb158a9-a73b-4e78-8dd4-ee16297fe39a','resourceURI':'/rest/openapi/notification/common/alarm','msgType':1,'extendedData':'{}','description':'Alarm related notification.','timestamp':'2019-06-10 16:02:44','data':{'optType':1,'systemID':'HuaweiPlatform','ackTime':0,'ackUser':null,'acked':false,'additionalInformation':'The disk Disk4 predictive failure (SN:PFVXJKWE).','additionalText':'','alarmId':33554441,'alarmName':'Hard disk prewarning','alarmSN':24128,'arrivedTime':1560207764169,'clearUser':null,'cleared':false,'clearedTime':0,'clearedType':1,'commentTime':0,'commentUser':'','comments':'','devCsn':0,'eventTime':1560236563000,'eventType':2,'moDN':'NE=34603227','moName':'2488H V5-192.168.100.1','neDN':'NE=34603227','neName':'2488H V5-192.168.0.1','neType':'2488H V5','objectInstance':'Sensor name=HDDPlaneDisk4','perceivedSeverity':3,'probableCause':1014873,'proposedRepairActions':'The server can still operate properly. Replace the disk at an appropriate time and environment.'}}} ";
            var service = new ESightPluginService();
            service.AnalysisTcpMsg(a);
        }

        private void btnStartTask_Click(object sender, EventArgs e)
        {

        }
    }
}
