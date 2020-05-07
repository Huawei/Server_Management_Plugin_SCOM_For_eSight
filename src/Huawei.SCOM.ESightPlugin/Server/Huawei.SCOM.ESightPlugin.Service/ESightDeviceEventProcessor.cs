using Huawei.SCOM.ESightPlugin.Core;
using Huawei.SCOM.ESightPlugin.LogUtil;
using Huawei.SCOM.ESightPlugin.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Huawei.SCOM.ESightPlugin.Const.ConstMgr.ESightEventeLogSource;

namespace Huawei.SCOM.ESightPlugin.Service
{
    public partial class ESightSyncInstance
    {


        /// <summary>
        /// Gets or sets the alarm datas.
        /// </summary>
        /// <value>The alarm datas.</value>
        private Queue<NedeviceData> deviceEventQueue { get; set; } = new Queue<NedeviceData>();

        /// <summary>
        /// synchronize lock object for "Alarm" Queue
        /// </summary>
        private readonly object deviceEventQueuelocker = new object();

        /// <summary>
        /// processor working thread
        /// </summary>
        private Thread deviceEventProcessor;

        /// <summary>
        /// The new alarm received handle
        /// </summary>
        private readonly AutoResetEvent receiveDeviceEvent = new AutoResetEvent(false);

        private int idx = 0;
        private List<int> eventIdSuffixs = new List<int> { 5, 16, 27, 38, 49 };


        /// <summary>
        /// Start Device event processor
        /// </summary>
        private void StartNeDeviceEventProcessor()
        {
            if (deviceEventProcessor == null)
            {
                deviceEventProcessor = new Thread(delegate ()
                {
                    while (this.IsRunning) 
                    {
                        logger.Polling.Info($"Current Device Event Processing Queue amount: {deviceEventQueue.Count}.");

                        if (deviceEventQueue.Count > 0 || receiveDeviceEvent.WaitOne())
                        {
                            NedeviceData eventObject = null;
                            lock (deviceEventQueuelocker)
                            {
                                if (deviceEventQueue.Count > 0)
                                {
                                    eventObject = deviceEventQueue.Dequeue();
                                }
                            }

                            if (eventObject != null)
                            {
                                logger.Polling.Info($"[{eventObject.DeviceId}] Start processing new device event.");


                                // should we catagory alarm with server type ?
                                var deviceId = eventObject.DeviceId;
                                MonitoringDeviceObject monitoringObject = GetDeviceByDN(deviceId);
                                if (monitoringObject == null)
                                {
                                    logger.Polling.Error($"[{eventObject.DeviceId}] No MonitoringObject({deviceId}) exists, event will be ignore");
                                    continue;
                                }

                                // waiting for monitoring-object ready.
                                WaitForDeviceMonitored(monitoringObject);


                                // Create New EventLog for new events, and generate SCOM event through associated rule
                                CreateNewEventLogForDeviceEvent(eventObject);

                            }
                        }
                    }
                });
            }

            this.deviceEventProcessor.Start();
            logger.Polling.Info("Device event processor starts successfully.");

        }

        private void CreateNewEventLogForDeviceEvent(NedeviceData eventObject)
        {
            logger.Polling.Info($"[{eventObject.DeviceId}] Persist event to window EventLog now.");

            // Create new event log instance
            var eventId = 400 + eventIdSuffixs.ElementAt(idx++ % eventIdSuffixs.Count);
            EventInstance instance = new EventInstance(eventId, 0, EventLogEntryType.Information);

            var deviceId = $"{this.ESightIp}-{eventObject.DeviceId}";
            object[] values = new object[] {
                "Device changed notification",      // channel
                deviceId,                           // SCOM monitor object DN
                eventObject.Description,
            };

            EventLog.WriteEvent(EVENT_SOURCE, instance, values);
            this.logger.Polling.Info($"[{eventObject.DeviceId}] Persist event to window EventLog successfully.");
        }

        /// <summary>
        /// Submit device changed event to processing queue
        /// </summary>
        /// <param name="data">The data.</param>
        public void SubmitNewDeviceEvent(NedeviceData data)
        {
         
            logger.Polling.Info($"[{data.DeviceId}] Submit new eSight device event to processing queue.");
            lock (deviceEventQueuelocker)
            {
                deviceEventQueue.Enqueue(data);
            }

            this.receiveDeviceEvent.Set();
        }
    }
}
