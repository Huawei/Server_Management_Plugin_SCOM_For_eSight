//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using CommonUtil;
using Huawei.SCOM.ESightPlugin.Core;
using Huawei.SCOM.ESightPlugin.Core.Models;
using Huawei.SCOM.ESightPlugin.Models;
using Huawei.SCOM.ESightPlugin.Models.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Service
{
    public partial class ESightSyncInstance
    {
        /// <summary>
        /// Gets or sets the alarm datas.
        /// </summary>
        /// <value>The alarm datas.</value>
        public Queue<AlarmData> AlarmDatas { get; set; }


        #region 启用插入事件的任务
        /// <summary>
        ///启用插入事件的任务
        /// </summary>
        /// <returns>Task.</returns>
        private void RunInsertEventTask()
        {
            Task.Run(() =>
            {
                while (this.IsRunning)
                {
                    if (AlarmDatas.Count > 0)
                    {
                        logger.Polling.Debug($"RunInsertEventTask :[{AlarmDatas.Count}]");
                        var alarm = AlarmDatas.Dequeue();
                        try
                        {
                            var dn = alarm.NeDN;
                            var serverType = this.GetServerTypeByDn(dn);
                            var eventData = new EventData(alarm, this.ESightIp, serverType);
                            logger.Polling.Debug($"RunInsertEventTask ESightIp:[{this.ESightIp}] serverType:[{serverType}] AlarmSn :[{eventData.AlarmSn}]");
                            switch (serverType)
                            {
                                case ServerTypeEnum.Blade:
                                case ServerTypeEnum.ChildBlade:
                                case ServerTypeEnum.Switch:
                                    BladeConnector.Instance.InsertEvent(eventData, serverType, this.ESightIp);
                                    break;
                                case ServerTypeEnum.Highdensity:
                                case ServerTypeEnum.ChildHighdensity:
                                    HighdensityConnector.Instance.InsertEvent(eventData, serverType, this.ESightIp);
                                    break;
                                case ServerTypeEnum.Rack:
                                    RackConnector.Instance.InsertEvent(eventData, this.ESightIp);
                                    break;
                                case ServerTypeEnum.KunLun:
                                    KunLunConnector.Instance.InsertEvent(eventData, this.ESightIp);
                                    break;
                            }
                            logger.Polling.Debug($"End InsertEvent :[{eventData.AlarmSn}]");
                        }
                        catch (Exception e)
                        {
                            OnPollingError($"Insert Event Error.AlarmId:{alarm.AlarmId}.", e);
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            });
        }
        #endregion

        /// <summary>
        /// Synchronizes the history alarm.
        /// </summary>
        public void SyncHistoryAlarm()
        {
            Task.Run(async () =>
            {
                logger.Polling.Debug($"Start Sync History Alarm");
                await Task.Delay(3 * 1000);
                var existAlarmData = new List<AlarmData>();
                var existBladeAlarmData = BladeConnector.Instance.GetExistAlarmDatas(BladeConnector.Instance.BladeClass, this.ESightIp);
                var existChildBladeAlarmData = BladeConnector.Instance.GetExistAlarmDatas(BladeConnector.Instance.ChildBladeClass, this.ESightIp);
                var existBladeSwitchAlarmData = BladeConnector.Instance.GetExistAlarmDatas(BladeConnector.Instance.SwitchClass, this.ESightIp);

                var existHighdensityAlarmData = HighdensityConnector.Instance.GetExistAlarmDatas(HighdensityConnector.Instance.HighdensityClass, this.ESightIp);
                var existChildHighdensityAlarmData = HighdensityConnector.Instance.GetExistAlarmDatas(HighdensityConnector.Instance.ChildHighdensityClass, this.ESightIp);

                var existKunLunAlarmData = KunLunConnector.Instance.GetExistAlarmDatas(KunLunConnector.Instance.KunLunClass, this.ESightIp);
                var existRackAlarmData = RackConnector.Instance.GetExistAlarmDatas(RackConnector.Instance.RackClass, this.ESightIp);

                existAlarmData.AddRange(existBladeAlarmData);
                existAlarmData.AddRange(existChildBladeAlarmData);
                existAlarmData.AddRange(existBladeSwitchAlarmData);
                existAlarmData.AddRange(existHighdensityAlarmData);
                existAlarmData.AddRange(existChildHighdensityAlarmData);
                existAlarmData.AddRange(existKunLunAlarmData);
                existAlarmData.AddRange(existRackAlarmData);
                OnPollingInfo($"SyncHistoryAlarm ExistAlarmData Count:{existAlarmData.Count}]");
                int totalPage = 1;
                int startPage = 0;
                var allVaildEvent = new List<AlarmHistory>();
                while (startPage < totalPage)
                {
                    try
                    {
                        startPage++;
                        var result = this.Session.GetAlarmHistory(startPage);
                        totalPage = result.TotalPage;
                        if (result.Code != 0)
                        {
                            OnPollingError($"SyncHistoryAlarm faild .pageNo:{startPage}.result:[{JsonUtil.SerializeObject(result)}");
                        }
                        else
                        {
                            var deviceEvents = result.Data.Where(x => x.EventType == 2).ToList();
                            OnPollingInfo($"SyncHistoryAlarm Success:[TotalCount:{result.Data.Count} EventType2Count:{deviceEvents.Count}]");
                            allVaildEvent.AddRange(deviceEvents);
                            deviceEvents.ForEach(item =>
                            {
                                var alarm = new AlarmData(item);
                                var existAlarm = existAlarmData.FirstOrDefault(x => x.AlarmSN == alarm.AlarmSN);
                                if (existAlarm == null)//数据库中不存在时，从队列中查询
                                {
                                    existAlarm = AlarmDatas.FirstOrDefault(x => x.AlarmSN == alarm.AlarmSN);
                                }
                                //对比本次列表中的告警和已存在的告警，如果不同再去插入
                                if (existAlarm == null || !CompareAlarmData(alarm, existAlarm))
                                {
                                    logger.Polling.Debug($"SyncHistoryAlarm Enqueue AlarmSN:[{alarm.AlarmSN}]");
                                    AlarmDatas.Enqueue(alarm);//加入队列
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPollingError($"SyncHistoryAlarm Error.eSight:{this.ESightIp} pageNo:{startPage}.", ex);
                    }
                }


                // 插入历史告警完成后调用订阅接口
                this.Subscribe();
                //检查未关闭的告警，在本次历史告警查询中是否存在，不存在则关闭
                BladeConnector.Instance.CheckUnclosedAlert(BladeConnector.Instance.BladeClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
                BladeConnector.Instance.CheckUnclosedAlert(BladeConnector.Instance.ChildBladeClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
                BladeConnector.Instance.CheckUnclosedAlert(BladeConnector.Instance.SwitchClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());

                HighdensityConnector.Instance.CheckUnclosedAlert(HighdensityConnector.Instance.HighdensityClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
                HighdensityConnector.Instance.CheckUnclosedAlert(HighdensityConnector.Instance.ChildHighdensityClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
                KunLunConnector.Instance.CheckUnclosedAlert(KunLunConnector.Instance.KunLunClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
                RackConnector.Instance.CheckUnclosedAlert(RackConnector.Instance.RackClass, this.ESightIp, allVaildEvent.Select(x => x.AlarmSn.ToString()).ToList());
            });
        }

        /// <summary>
        /// 对比告警
        /// </summary>
        /// <param name="preData">The pre data.</param>
        /// <param name="nowData">The now data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CompareAlarmData(AlarmData preData, AlarmData nowData)
        {
            if (preData.AlarmName != nowData.AlarmName) { return false; }
            if (preData.AlarmId != nowData.AlarmId) { return false; }
            //if (preData.ArrivedTime != nowData.ArrivedTime) { return false; }
            //if (preData.MoDN != nowData.MoDN) { return false; }
            //if (preData.MoName != nowData.MoName) { return false; }
            //if (preData.NeDN != nowData.NeDN) { return false; }
            //if (preData.NeName != nowData.NeName) { return false; }
            //if (preData.NeType != nowData.NeType) { return false; }
            if (preData.PerceivedSeverity != nowData.PerceivedSeverity) { return false; }
            if (preData.ProbableCause != nowData.ProbableCause) { return false; }
            return true;
        }

        /// <summary>
        /// 处理推送过来的告警
        /// </summary>
        /// <param name="data">The data.</param>
        public void DealNewAlarm(AlarmData data)
        {
            logger.Polling.Debug($"Enqueue AlarmSN:[{data.AlarmSN}]");
            this.AlarmDatas.Enqueue(data);
        }
    }
}
