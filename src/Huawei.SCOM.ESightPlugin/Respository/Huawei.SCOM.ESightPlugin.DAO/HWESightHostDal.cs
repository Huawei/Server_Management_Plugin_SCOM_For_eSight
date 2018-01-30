// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.DAO
// Author           : suxiaobo
// Created          : 12-08-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-08-2017
// ***********************************************************************
// <copyright file="HWESightHostDal.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>eSight数据库管理类</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.DAO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using Huawei.SCOM.ESightPlugin.Models;

    using LogUtil;

    /// <summary>
    ///     eSight数据库管理类
    /// </summary>
    public class HWESightHostDal : /*BaseRepository<HWESightHost>, */IHWESightHostDal
    {
#if DEBUG

        /// <summary>
        /// The file path.
        /// </summary>
        public string FilePath = Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN") + "//KN";
#else
        public string FilePath = System.Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN") + "//KN";
#endif

        /// <summary>
        ///     单例
        /// </summary>
        public static HWESightHostDal Instance => SingletonProvider<HWESightHostDal>.Instance;

        /// <summary>
        /// 根据IP查找ESight实体。
        /// </summary>
        /// <param name="eSightIp">
        /// IP地址
        /// </param>
        /// <returns>
        /// The <see cref="HWESightHost"/>ex
        /// </returns>
        public HWESightHost FindByIP(string eSightIp)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                }
                var model = list.FirstOrDefault(x => x.HostIP == eSightIp);
                return model;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("FindByIP Error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 删除eSight
        /// </summary>
        /// <param name="eSightId">
        /// eSight Id
        /// </param>
        public void DeleteESight(int eSightId)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }

                    var oldModel = list.FirstOrDefault(x => x.ID == eSightId);
                    if (oldModel != null)
                    {
                        list.Remove(oldModel);
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            var bf = new BinaryFormatter();
                            bf.Serialize(fs, list);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("DeleteESight Error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// The insert entity.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int InsertEntity(HWESightHost model)
        {
            var id = 0;
            try
            {
                HWLogger.UI.Info("FilePath" + FilePath);
                var filePath = FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                    if (list.Count > 0)
                    {
                        id = list.Max(x => x.ID);
                    }
                }
                model.ID = id + 1;
                list.Add(model);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, list);
                }

                return model.ID;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("FilePath" + FilePath);
                HWLogger.UI.Error("InsertEntity Error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// The update entity.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int UpdateEntity(HWESightHost model)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }

                    if (list != null)
                    {
                        var oldModel = list.FirstOrDefault(x => x.HostIP == model.HostIP);
                        if (oldModel == null)
                        {
                            return -1;
                        }
                        list.Remove(oldModel);
                        model.ID = oldModel.ID;
                        list.Add(model);
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            var bf = new BinaryFormatter();
                            bf.Serialize(fs, list);
                        }
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("UpdateEntity Error:" + ex);
                throw;
            }

            return -1;
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
            var list = new List<HWESightHost>();
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                    if (list == null)
                    {
                        list = new List<HWESightHost>();
                    }
                }
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("GetList Error:" + ex);
                throw;
            }
            return list.OrderByDescending(x => x.ID).ToList();
        }

        /// <summary>
        /// The get entity by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HWESightHost"/>.
        /// </returns>
        public HWESightHost GetEntityById(int id)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                }
                var model = list.FirstOrDefault(x => x.ID == id);
                return model;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("GetEntityById Error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// The update subscription ne device status.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="status">The status.</param>
        /// <param name="error">The error.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int UpdateSubscriptionNeDeviceStatus(string hostIp, int status, string error)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                }
                var model = list.FirstOrDefault(x => x.HostIP == hostIp);
                model.SubscriptionNeDeviceStatus = status;
                model.SubscripeNeDeviceError = error;
                return this.UpdateEntity(model);
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("UpdateSubscriptionStatus Error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// The update subscription alarm status.
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="status">The status.</param>
        /// <param name="error">The error.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int UpdateSubscriptionAlarmStatus(string hostIp, int status, string error)
        {
            try
            {
                var filePath = this.FilePath + "\\eSight.bin";
                var list = new List<HWESightHost>();
                if (File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var bf = new BinaryFormatter();
                        list = bf.Deserialize(fs) as List<HWESightHost>;
                    }
                }
                var model = list.FirstOrDefault(x => x.HostIP == hostIp);
                model.SubscriptionAlarmStatus = status;
                model.SubscripeAlarmError = error;
                return this.UpdateEntity(model);
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("UpdateSubscriptionStatus Error:" + ex);
                throw;
            }
        }
    }
}