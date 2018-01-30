// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
//   
// </copyright>
// <summary>
//   日志接口实现类
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogUtil
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using NLog;

    /// <summary>
    ///     日志接口实现类
    /// </summary>
    internal class Logger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class. 
        /// 日志类构造方法
        /// </summary>
        /// <param name="logname">
        /// 日志的名称,建议直接使用HWLogger类来写日志
        /// </param>
        public Logger(string logname)
        {
            this.RawLog = LogManager.GetLogger(logname);
        }

        /// <summary>
        ///     NLOG 日志类。
        /// </summary>
        public NLog.Logger RawLog { get; }

        /// <summary>
        /// 输出错误日志 Debug 级别。
        /// </summary>
        /// &gt;
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        public void Debug(string msg)
        {
            this.RawLog.Debug(this.GetCleanedMessage(msg));
        }

        /// <summary>
        /// 输出错误日志 Debug 级别。
        /// </summary>
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        /// <param name="ex">
        /// 错误类
        /// </param>
        public void Debug(string msg, Exception ex)
        {
            this.RawLog.DebugException(this.GetCleanedMessage(msg.ToString()), ex);
        }

        /// <summary>
        /// 输出错误日志 Debug 级别。
        ///     支持string.format 的占位符方式.
        ///     如：DebugFormat("title:{0}",titleVar)
        /// </summary>
        /// <param name="msg">
        /// 消息正文
        /// </param>
        /// <param name="args">
        /// 带入参数,数组类型
        /// </param>
        public void DebugFormat(string msg, params object[] args)
        {
            this.RawLog.Debug(this.GetCleanedMessage(msg), args);
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// &gt;
        public void Error(Exception ex)
        {
            this.RawLog.Error(ex);
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void Error(string msg)
        {
            this.RawLog.Error(this.GetCleanedMessage(msg));
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        /// </summary>
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        /// <param name="ex">
        /// 错误类
        /// </param>
        public void Error(string msg, Exception ex)
        {
            this.RawLog.ErrorException(this.GetCleanedMessage(msg.ToString()), ex);
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        ///     支持string.format 的占位符方式.
        ///     如：ErrorFormat("title:{0}",titleVar)
        /// </summary>
        /// <param name="msg">
        /// 消息正文
        /// </param>
        /// <param name="args">
        /// 带入参数,数组类型
        /// </param>
        public void ErrorFormat(string msg, params object[] args)
        {
            this.RawLog.Error(this.GetCleanedMessage(msg), args);
        }

        /// <summary>
        /// 输出错误日志 Fatal 级别。
        /// </summary>
        /// &gt;
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        public void Fatal(string msg)
        {
            this.RawLog.Fatal(this.GetCleanedMessage(msg));
        }

        /// <summary>
        /// 输出错误日志 Fatal 级别。
        /// </summary>
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        /// <param name="ex">
        /// 错误类
        /// </param>
        public void Fatal(string msg, Exception ex)
        {
            this.RawLog.FatalException(this.GetCleanedMessage(msg.ToString()), ex);
        }

        /// <summary>
        /// 输出错误日志 Fatal 级别。
        ///     支持string.format 的占位符方式.
        ///     如：FatalFormat("title:{0}",titleVar)
        /// </summary>
        /// <param name="msg">
        /// 消息正文
        /// </param>
        /// <param name="args">
        /// 带入参数,数组类型
        /// </param>
        public void FatalFormat(string msg, params object[] args)
        {
            this.RawLog.Fatal(this.GetCleanedMessage(msg), args);
        }

        /// <summary>
        /// 输出错误日志 Info 级别。
        /// </summary>
        /// &gt;
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        public void Info(string msg)
        {
            var cleanedMsg = this.GetCleanedMessage(msg);
            this.RawLog.Info(cleanedMsg);
        }

        /// <summary>
        /// 输出错误日志 Info 级别。
        /// </summary>
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        /// <param name="ex">
        /// 错误类
        /// </param>
        public void Info(string msg, Exception ex)
        {
            this.RawLog.InfoException(this.GetCleanedMessage(msg.ToString()), ex);
        }

        /// <summary>
        /// 输出错误日志 Info 级别。
        ///     支持string.format 的占位符方式.
        ///     如：InfoFormat("title:{0}",titleVar)
        /// </summary>
        /// <param name="msg">
        /// 消息正文
        /// </param>
        /// <param name="args">
        /// 带入参数,数组类型
        /// </param>
        public void InfoFormat(string msg, params object[] args)
        {
            this.RawLog.Info(this.GetCleanedMessage(msg), args);
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void Log(string msg)
        {
            this.RawLog.Error(this.GetCleanedMessage(msg));
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Log(Exception e)
        {
            this.RawLog.Error(e);
        }

        /// <summary>
        /// 输出错误日志 Error 级别。
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="e">The e.</param>
        /// &gt;
        public void Log(object msg, Exception e)
        {
            this.RawLog.ErrorException(this.GetCleanedMessage(msg.ToString()), e);
        }

        /// <summary>
        /// 输出错误日志 Warn 级别。
        /// </summary>
        /// &gt;
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        public void Warn(string msg)
        {
            var cleanedMsg = this.GetCleanedMessage(msg);
            this.RawLog.Warn(cleanedMsg);
        }

        /// <summary>
        /// 输出错误日志 Warn 级别。
        /// </summary>
        /// <param name="msg">
        /// 错误消息体
        /// </param>
        /// <param name="ex">
        /// 错误类
        /// </param>
        public void Warn(string msg, Exception ex)
        {
            this.RawLog.WarnException(this.GetCleanedMessage(msg.ToString()), ex);
        }

        /// <summary>
        /// 输出错误日志 Warn 级别。
        ///     支持string.format 的占位符方式.
        ///     如：InfoFormat("title:{0}",titleVar)
        /// </summary>
        /// <param name="msg">
        /// 消息正文
        /// </param>
        /// <param name="args">
        /// 带入参数,数组类型
        /// </param>
        public void WarnFormat(string msg, params object[] args)
        {
            this.RawLog.Warn(this.GetCleanedMessage(msg), args);
        }

        /// <summary>
        /// Getcleaneds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        public string GetCleanedMessage(string message)
        {
            if (message == null)
            {
                return string.Empty;
            }
            return message.ToString().Replace('\n', '_').Replace('\r', '_');
        }
    }
}