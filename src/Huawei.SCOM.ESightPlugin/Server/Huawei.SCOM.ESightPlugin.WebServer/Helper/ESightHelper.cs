// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.WebServer
// Author           : suxiaobo
// Created          : 12-12-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="ESightHelper.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>The e sight helper.</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.WebServer.Helper
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using CommonUtil;

    using Huawei.SCOM.ESightPlugin.Const;
    using Huawei.SCOM.ESightPlugin.Models;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Exceptions;
    using Huawei.SCOM.ESightPlugin.RESTeSightLib.Helper;
    using Huawei.SCOM.ESightPlugin.WebServer.Model;

    using LogUtil;

    /// <summary>
    /// The e sight helper.
    /// </summary>
    public class ESightHelper
    {
        /// <summary>
        /// Deletes the specified event data.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.WebServer.Model.ApiResult.</returns>
        public static ApiResult Delete(object eventData)
        {
            var ret = new ApiResult(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, string.Empty);
            try
            {
                var jsData = JsonUtil.SerializeObject(eventData);
                HWLogger.UI.Info("Deleting eSight, the param is [{0}]", jsData);
                var hostIps = eventData.ToString().TrimEnd(',').Split(',');
                foreach (var hostIp in hostIps)
                {
                    try
                    {
                        // 取消订阅（不再修改eSight的订阅状态）
                        var eSight = ESightDal.Instance.GetEntityByHostIp(hostIp);
                        if (eSight == null)
                        {
                            throw new Exception("can not find eSight:" + hostIp);
                        }

                        UnSubscribeESight(hostIp, eSight.SystemID, true);

                        // 从文件中删除
                        ESightDal.Instance.DeleteESightByHostIp(hostIp);

                        // 告诉服务（删除scom中的服务器）
                        Task.Run(() =>
                        {
                            var message = new TcpMessage<string>(hostIp, TcpMessageType.DeleteESight, "delete a ESight");
                            NotifyClient.Instance.SendMsg(message);
                        });
                    }
                    catch (Exception ex)
                    {
                        HWLogger.UI.Error($"Deleting eSight({hostIp}) error !", ex);
                    }
                }
                HWLogger.UI.Info("Deleting eSight successful!");
                ret.Code = "0";
                ret.Success = true;
                ret.Msg = "Deleting eSight successful!";
            }
            catch (BaseException ex)
            {
                HWLogger.UI.Error("Deleting eSight failed: ", ex);
                ret.Code = $"{ex.ErrorModel}{ex.Code}";
                ret.Success = false;
                ret.ExceptionMsg = ex.Message;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("Deleting eSight failed: ", ex);
                ret.Code = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                ret.Success = false;
                ret.ExceptionMsg = ex.InnerException?.Message ?? ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 获取eSight列表数据
        /// </summary>
        /// <param name="queryParam">
        /// The query Param.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>WebReturnLGResult</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static WebReturnLGResult<HWESightHost> GetList(ParamPagingOfQueryESight queryParam)
        {
            var ret = new WebReturnLGResult<HWESightHost>
            {
                Code = CoreUtil.GetObjTranNull<int>(
                                  ConstMgr.ErrorCode.SYS_UNKNOWN_ERR),
                Description = string.Empty
            };
            try
            {
                // 1. 处理参数
                var jsData = JsonUtil.SerializeObject(queryParam);
                HWLogger.UI.Info("Querying eSight list, the param is [{0}]", jsData);
                var pageSize = queryParam.PageSize;
                var pageNo = queryParam.PageNo;
                var hostIp = queryParam.HostIp;

                // 2. 获取数据 
                var hwESightHostList = ESightDal.Instance.GetList();
                var filterList = hwESightHostList.Where(x => x.HostIP.Contains(hostIp)).Skip((pageNo - 1) * pageSize)
                    .Take(pageSize).ToList();

                // 3. 返回数据
                ret.Code = 0;
                ret.Data = filterList;
                ret.TotalNum = hwESightHostList.Count();
                ret.Description = string.Empty;

                HWLogger.UI.Info("Querying eSight list successful, the ret is [{0}]", JsonUtil.SerializeObject(ret));
            }
            catch (BaseException ex)
            {
                ret.Code = CoreUtil.GetObjTranNull<int>(ex.Code);
                ret.ErrorModel = ex.ErrorModel;
                ret.Description = ex.Message;
                ret.Data = null;
                ret.TotalNum = 0;
                HWLogger.UI.Error("Querying eSight list failed: ", ex);
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error(ex);
                ret.Code = CoreUtil.GetObjTranNull<int>(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR);
                ret.Description = ex.InnerException?.Message ?? ex.Message;
                ret.Data = null;
                ret.TotalNum = 0;
                HWLogger.UI.Error("Querying eSight list failed: ", ex);
            }

            return ret;
        }

        /// <summary>
        /// Adds the specified event data.
        /// </summary>
        /// <param name="model">The new e sight.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.WebServer.Model.ApiResult.</returns>
        public static ApiResult Add(HWESightHost model)
        {
            var ret = new ApiResult(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, string.Empty);
            try
            {
                var oldeSight = ESightDal.Instance.GetEntityByHostIp(model.HostIP);
                // hostIp 已存在，则报错
                if (oldeSight != null)
                {
                    throw new BaseException("-90006", null, $"{model.HostIP} is already exsit");
                }
                #region 赋值

                model.CreateTime = DateTime.Now;
                model.LastModifyTime = DateTime.Now;
                model.OpenID = Guid.NewGuid().ToString("D");
                model.SubscribeID = Guid.NewGuid().ToString("D");
                model.SubKeepAliveStatus = 0;
                model.SubscriptionAlarmStatus = 0;
                model.SubscriptionNeDeviceStatus = 0;
                model.SubKeepAliveError = string.Empty;
                model.SubscripeAlarmError = string.Empty;
                model.SubscripeNeDeviceError = string.Empty;
                model.LatestStatus = ConstMgr.ESightConnectStatus.NONE;
                model.LatestConnectInfo = string.Empty;
                var encryptPwd = EncryptUtil.EncryptPwd(model.LoginPd);
                model.LoginPd = encryptPwd;

                #endregion
                var json = JsonUtil.SerializeObject(model);
                HWLogger.UI.Info("Add eSight, the param is [{0}]", HidePd(json));
                ESightEngine.Instance.SaveSession(model);
                ESightDal.Instance.InsertEntity(model);

                // 告诉服务
                Task.Run(() =>
                {
                    var message = new TcpMessage<string>(model.HostIP, TcpMessageType.SyncESight, "add new ESight");
                    NotifyClient.Instance.SendMsg(message);
                });

                HWLogger.UI.Info("Add eSight successful!");
                ret.Code = "0";
                ret.Success = true;
                ret.Msg = "Add eSight successful!";
            }
            catch (BaseException ex)
            {
                HWLogger.UI.Error("Add eSight failed: ", ex);
                ret.Code = $"{ex.ErrorModel}{ex.Code}";
                ret.Success = false;
                ret.ExceptionMsg = ex.Message;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("Add eSight failed: ", ex);
                ret.Code = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                ret.Success = false;
                ret.ExceptionMsg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return ret;
        }

        /// <summary>
        /// Updates the specified new e sight.
        /// </summary>
        /// <param name="model">The new e sight.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.WebServer.Model.ApiResult.</returns>
        public static ApiResult Update(HWESightHost model)
        {
            var ret = new ApiResult(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, string.Empty);
            try
            {
                var json = JsonUtil.SerializeObject(model);
                HWLogger.UI.Info("Update eSight , the param is [{0}]", HidePd(json));
                #region 赋值
                var eSight = ESightDal.Instance.GetEntityByHostIp(model.HostIP);
                if (eSight == null)
                {
                    ret.Code = "-81111";
                    ret.Success = false;
                    ret.Msg = $"can not find eSight: hostip {model.HostIP}";
                    return ret;
                }

                eSight.AliasName = model.AliasName;
                eSight.HostPort = model.HostPort;
                eSight.LoginAccount = model.LoginAccount;
                eSight.LatestStatus = ConstMgr.ESightConnectStatus.NONE;
                eSight.LatestConnectInfo = string.Empty;
                eSight.LastModifyTime = DateTime.Now;

                var encryptPwd = EncryptUtil.EncryptPwd(model.LoginPd);
                eSight.LoginPd = encryptPwd;
                var oldSystemId = eSight.SystemID;
                var isChangeSystemId = oldSystemId != model.SystemID;
                if (isChangeSystemId)
                {
                    eSight.SystemID = model.SystemID;
                    eSight.SubscribeID = Guid.NewGuid().ToString();
                    eSight.SubscripeNeDeviceError = string.Empty;
                    eSight.SubscripeAlarmError = string.Empty;
                    eSight.SubKeepAliveError = string.Empty;
                    eSight.SubscriptionNeDeviceStatus = 0;
                    eSight.SubscriptionAlarmStatus = 0;
                    eSight.SubKeepAliveStatus = 0;
                }
                #endregion
                ESightEngine.Instance.SaveSession(eSight);
                if (isChangeSystemId)
                {
                    // 修改了systemId，在session保存成功后重新订阅
                    UnSubscribeESight(eSight.HostIP, oldSystemId);
                }
                // Update
                ESightDal.Instance.UpdateEntity(eSight);

                // 告诉服务
                Task.Run(() =>
                {
                    var message = new TcpMessage<string>(eSight.HostIP, TcpMessageType.SyncESight, "Update ESight");
                    NotifyClient.Instance.SendMsg(message);
                });

                HWLogger.UI.Info("Update eSight successful!");
                ret.Code = "0";
                ret.Success = true;
                ret.Msg = "Update eSight successful!";
            }
            catch (BaseException ex)
            {
                HWLogger.UI.Error("Update eSight failed: ", ex);
                ret.Code = $"{ex.ErrorModel}{ex.Code}";
                ret.Success = false;
                ret.ExceptionMsg = ex.Message;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("Update eSight failed: ", ex);
                ret.Code = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                ret.Success = false;
                ret.ExceptionMsg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return ret;
        }

        /// <summary>
        /// Updates the with out pass.
        /// </summary>
        /// <param name="model">The new e sight.</param>
        /// <returns>Huawei.SCOM.ESightPlugin.WebServer.Model.ApiResult.</returns>
        public static ApiResult UpdateWithOutPass(HWESightHost model)
        {
            var ret = new ApiResult(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, string.Empty);
            try
            {
                var json = JsonUtil.SerializeObject(model);
                HWLogger.UI.Info("Update eSight , the param is [{0}]", HidePd(json));
                #region 赋值
                var eSight = ESightDal.Instance.GetEntityByHostIp(model.HostIP);
                if (eSight == null)
                {
                    ret.Code = "-81111";
                    ret.Success = false;
                    ret.Msg = $"can not find eSight: hostip {model.HostIP}";
                    return ret;
                }
                eSight.AliasName = model.AliasName;
                eSight.HostPort = model.HostPort;
                eSight.LastModifyTime = DateTime.Now;
                var oldSystemId = eSight.SystemID;
                var isChangeSystemId = oldSystemId != model.SystemID;
                if (isChangeSystemId)
                {
                    // 修改了systemId，需要重新订阅
                    eSight.SystemID = model.SystemID;
                    eSight.SubscribeID = Guid.NewGuid().ToString();
                    eSight.SubscripeNeDeviceError = string.Empty;
                    eSight.SubscripeAlarmError = string.Empty;
                    eSight.SubKeepAliveError = string.Empty;
                    eSight.SubscriptionNeDeviceStatus = 0;
                    eSight.SubscriptionAlarmStatus = 0;
                    eSight.SubKeepAliveStatus = 0;
                }

                #endregion
                ESightEngine.Instance.SaveSession(eSight);
                if (isChangeSystemId)
                {
                    // 修改了systemId，在session保存成功后重新订阅
                    UnSubscribeESight(eSight.HostIP, oldSystemId);
                }
                // UpdateWithOutPass
                ESightDal.Instance.UpdateEntity(eSight);

                // 告诉服务
                Task.Run(() =>
                {
                    var message = new TcpMessage<string>(eSight.HostIP, TcpMessageType.SyncESight, "Update ESight WithOut Pass");
                    NotifyClient.Instance.SendMsg(message);
                });

                HWLogger.UI.Info("Update eSight successful!");
                ret.Code = "0";
                ret.Success = true;
                ret.Msg = "Update eSight successful!";
            }
            catch (BaseException ex)
            {
                HWLogger.UI.Error("Update eSight failed: ", ex);
                ret.Code = $"{ex.ErrorModel}{ex.Code}";
                ret.Success = false;
                ret.ExceptionMsg = ex.Message;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("Update eSight failed: ", ex);
                ret.Code = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                ret.Success = false;
                ret.ExceptionMsg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return ret;
        }

        /// <summary>
        /// The test.
        /// </summary>
        /// <param name="eSight">The e sight.</param>
        /// <returns>The <see cref="ApiResult" />.</returns>
        public static ApiResult Test(HWESightHost eSight)
        {
            var ret = new ApiResult(ConstMgr.ErrorCode.SYS_UNKNOWN_ERR, string.Empty);
            try
            {
                var json = JsonUtil.SerializeObject(eSight);
                HWLogger.UI.Info("Testing eSight connect..., the param is [{0}]", HidePd(json));

                var LoginPd = eSight.LoginPd;
                eSight.LoginPd = EncryptUtil.EncryptPwd(LoginPd);

                var testResult = ESightEngine.Instance.TestEsSession(eSight);
                if (string.IsNullOrEmpty(testResult))
                {
                    HWLogger.UI.Info("Testing eSight connect successful!");
                    ret.Code = "0";
                    ret.Success = true;
                    ret.Msg = "Testing eSight connect successful!";
                }
                else
                {
                    HWLogger.UI.Info("Testing eSight connect failed!");
                    ret.Code = testResult;
                    ret.Success = false;
                    ret.ExceptionMsg = "Testing eSight connect failed!";
                }
            }
            catch (BaseException ex)
            {
                HWLogger.UI.Error("Testing eSight connect failed: ", ex);
                ret.Success = false;
                ret.Code = $"{ex.ErrorModel}{ex.Code}";
                ret.ExceptionMsg = ex.Message;
            }
            catch (Exception ex)
            {
                HWLogger.UI.Error("Testing eSight connect failed: ", ex);
                ret.Code = ConstMgr.ErrorCode.SYS_UNKNOWN_ERR;
                ret.Success = false;
                ret.Msg = "Testing eSight connect failed!";
                ret.ExceptionMsg = ex.InnerException?.Message ?? ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 隐藏Json字符串中的密码
        /// </summary>
        /// <param name="jsonData">
        /// The json data.
        /// </param>
        /// <returns>
        /// System.String.
        /// </returns>
        private static string HidePd(string jsonData)
        {
            var replacement = "\"${str}\":\"********\"";
            var pattern1 = "\"(?<str>([A-Za-z0-9_]*)loginPd)\":\"(.*?)\"";
            string newJsonData = Regex.Replace(
                 jsonData,
                 pattern1,
                 replacement,
                 RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            return newJsonData;
        }

        /// <summary>
        /// 取消订阅eSight.(需要用旧的systemId取消订阅)
        /// </summary>
        /// <param name="hostIp">The host ip.</param>
        /// <param name="oldSystemId">The system identifier.</param>
        /// <param name="isDeleteSession">The is delete session.</param>
        private static void UnSubscribeESight(string hostIp, string oldSystemId, bool isDeleteSession = false)
        {
            Task.Run(() =>
            {
                try
                {
                    // 取消订阅
                    var iEsSession = ESightEngine.Instance.FindEsSession(hostIp);

                    var resut = iEsSession.UnSubscribeKeepAlive(oldSystemId);
                    HWLogger.UI.Info($"UnSubscribeKeepAlive.eSight:{hostIp} result:{JsonUtil.SerializeObject(resut)}");
                    resut = iEsSession.UnSubscribeAlarm(oldSystemId);
                    HWLogger.UI.Info($"UnSubscribeAlarm. eSight:{hostIp} result:{JsonUtil.SerializeObject(resut)}");
                    resut = iEsSession.UnSubscribeNeDevice(oldSystemId);
                    HWLogger.UI.Info($"UnSubscribeNeDevice. eSight:{hostIp} result:{JsonUtil.SerializeObject(resut)}");
                }
                catch (Exception ex)
                {
                    HWLogger.UI.Error("UnSubscribeWhenUpdateSystemId Error", ex);
                }
                finally
                {
                    if (isDeleteSession)
                    {
                        // 从缓存中删除
                        ESightEngine.Instance.RemoveEsSession(hostIp);
                    }
                }
            });
        }
    }
}