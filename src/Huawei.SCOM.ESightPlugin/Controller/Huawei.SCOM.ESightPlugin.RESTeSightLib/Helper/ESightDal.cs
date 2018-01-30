// ***********************************************************************
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
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            if (oldModel == null)
            {
                throw new Exception($"can not find eSight:{hostIp}");
            }
            list.Remove(oldModel);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// 根据订阅ID查询eSight
        /// </summary>
        /// <param name="subscribeId">The subscribe identifier.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.Models.HWESightHost.</returns>
        public HWESightHost GetEntityBySubscribeId(string subscribeId)
        {
            var list = this.GetList();
            return list.FirstOrDefault(x => x.SubscribeID == subscribeId);
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="model">The model.</param>
        public void InsertEntity(HWESightHost model)
        {
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
            this.CreateDir();
            var list = new List<HWESightHost>();
            if (File.Exists(this.filePath))
            {
                using (var fs = new FileStream(this.filePath, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    list = bf.Deserialize(fs) as List<HWESightHost>;
                }
                if (list == null)
                {
                    throw new Exception($"Get eSight list is null");
                }
            }
            else
            {
                using (var fs = new FileStream(this.filePath, FileMode.Create))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, list);
                }
            }
            return list.OrderByDescending(x => x.CreateTime).ToList();
        }

        /// <summary>
        /// 根据IP查找ESight实体。
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <returns>The <see cref="HWESightHost" />.</returns>
        public HWESightHost GetEntityByHostIp(string hostIp)
        {
            var list = this.GetList();
            return list.FirstOrDefault(x => x.HostIP == hostIp);
        }

        #region UpdateESight

        /// <summary>
        /// Updates the esight alarm.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="alarmStatus">The alarm status.</param>
        /// <param name="error">The error.</param>
        public void UpdateESightAlarm(string hostIp, int alarmStatus, string error)
        {
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            if (oldModel == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }
            list.Remove(oldModel);
            oldModel.LastModifyTime = DateTime.Now;
            oldModel.SubscripeAlarmError = error;
            oldModel.SubscriptionAlarmStatus = alarmStatus;
            list.Add(oldModel);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// Updates the esight alarm.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="alarmStatus">The alarm status.</param>
        /// <param name="error">The error.</param>
        public void UpdateESightNeDevice(string hostIp, int alarmStatus, string error)
        {
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            if (oldModel == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }
            list.Remove(oldModel);
            oldModel.LastModifyTime = DateTime.Now;
            oldModel.SubscripeNeDeviceError = error;
            oldModel.SubscriptionNeDeviceStatus = alarmStatus;
            list.Add(oldModel);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// Updates the esight password.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="enPwd">The pwd.</param>
        public void UpdateESightPwd(string hostIp, string enPwd)
        {
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            if (oldModel == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }
            list.Remove(oldModel);
            oldModel.LastModifyTime = DateTime.Now;
            oldModel.LoginPd = enPwd;
            list.Add(oldModel);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }


        /// <summary>
        /// Updates  esight connect status.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="latestStatus">The latest status.</param>
        /// <param name="latestConnectInfo">The latest connect information.</param>
        public void UpdateESightConnectStatus(string hostIp, string latestStatus, string latestConnectInfo)
        {
            var list = this.GetList();
            var oldModel = list.FirstOrDefault(x => x.HostIP == hostIp);
            if (oldModel == null)
            {
                throw new Exception($"Can not find the eSight:{hostIp}");
            }
            list.Remove(oldModel);
            oldModel.LastModifyTime = DateTime.Now;
            oldModel.LatestStatus = latestStatus;
            oldModel.LatestConnectInfo = latestConnectInfo;
            list.Add(oldModel);
            using (var fs = new FileStream(this.filePath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        #endregion

        /// <summary>
        /// 删除数据库文件
        /// </summary>
        public void DeleteDbFile()
        {
            if (File.Exists(this.filePath))
            {
                File.Delete(this.filePath);
            }
        }

        /// <summary>
        /// Creates the dir.
        /// </summary>
        private void CreateDir()
        {
            var path = Path.GetDirectoryName(this.filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}