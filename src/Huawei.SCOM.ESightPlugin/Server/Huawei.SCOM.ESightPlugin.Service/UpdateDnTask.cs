// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Service
// Author           : yayun
// Created          : 06-12-2018
//
// Last Modified By : yayun
// Last Modified On : 06-12-2018
// ***********************************************************************
// <copyright file="NeedUpdateDn.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Linq;
using CommonUtil;
using Huawei.SCOM.ESightPlugin.Models.Devices;

namespace Huawei.SCOM.ESightPlugin.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using LogUtil;

    /// <summary>
    /// Class UpdateDnTask.
    /// </summary>
    public class UpdateDnTask
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDnTask" /> class.
        /// </summary>
        /// <param name="eSightSyncInstance">The e sight synchronize instance.</param>
        /// <param name="dn">The dn.</param>
        /// <param name="alarmSn">The alarm sn.</param>
        public UpdateDnTask(ESightSyncInstance eSightSyncInstance, string dn, int alarmSn)
        {
            Dn = dn;
            ESightSyncInstance = eSightSyncInstance;
            AlarmSn = alarmSn;
        }

        /// <summary>
        /// Gets or sets the e sight synchronize instance.
        /// </summary>
        /// <value>The e sight synchronize instance.</value>
        public ESightSyncInstance ESightSyncInstance { get; set; }

        /// <summary>
        /// Dn
        /// </summary>
        /// <value>The dn.</value>
        public string Dn { get; set; }

        /// <summary>
        /// 告警Sn
        /// </summary>
        /// <value>The alarm sn.</value>
        public int AlarmSn { get; set; }

        /// <summary>
        /// Gets or sets the update action.
        /// </summary>
        /// <value>The update action.</value>
        public Action<HWDeviceDetail, int> UpdateAction { get; set; }

        /// <summary>
        /// Gets or sets the last refresh time.
        /// </summary>
        /// <value>The last refresh time.</value>
        public DateTime FirstRefreshTime { get; set; }

        /// <summary>
        /// Gets or sets the last refresh time.
        /// </summary>
        /// <value>The last refresh time.</value>
        public DateTime LastRefreshTime { get; set; }

        /// <summary>
        /// Gets or sets the device first.
        /// </summary>
        /// <value>The device first.</value>
        public HWDeviceDetail DeviceFirst { get; set; }

        /// <summary>
        /// 开启首次更新的任务
        /// </summary>
        public void StartFirstUpdateTask()
        {
            Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(60));
                DeviceFirst = ESightSyncInstance.Session.GetServerDetails(this.Dn);
                UpdateAction(DeviceFirst, this.AlarmSn);
            });
        }

        /// <summary>
        /// 开启延迟更新的任务
        /// 异常时最多重试3次
        /// </summary>
        public void StartLastUpdateTask(Action<bool, bool> lastUpdateFinish)
        {
            Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5 * 60));
                int i = 0;
                while (i < 3)
                {
                    try
                    {
                        if (DateTime.Now > this.LastRefreshTime)
                        {
                            var deviceLast = ESightSyncInstance.Session.GetServerDetails(this.Dn);
                            var isChange = CheckIsChange(DeviceFirst, deviceLast);
                            if (isChange)
                            {
                                UpdateAction(deviceLast, this.AlarmSn);
                            }
                            lastUpdateFinish.Invoke(true, isChange);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        i++;
                        if (i == 3)
                        {
                            lastUpdateFinish.Invoke(false, false);
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            });
        }

        public bool CheckIsChange(HWDeviceDetail firstDevice, HWDeviceDetail lastDevice)
        {
            return JsonUtil.SerializeObject(firstDevice) != JsonUtil.SerializeObject(lastDevice);
        }

    }
}
