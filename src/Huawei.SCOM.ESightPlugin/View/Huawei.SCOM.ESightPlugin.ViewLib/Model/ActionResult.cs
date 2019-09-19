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

namespace Huawei.SCOM.ESightPlugin.ViewLib.Model
{
    public class Result
    {
        public const int DEFAULT_FAILIRE_CODE = 100;

        /// <summary>
        /// whether action succeed
        /// </summary>
        public bool Success => this.Code == 0;

        /// <summary>
        /// failed code when action failed
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// failed message when action failed
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// failed message when action failed
        /// </summary>
        public Exception Cause { get; set; }

        /// <summary>
        /// Action result if provided
        /// </summary>
        public Object Data { get; set; }

        public static Result Done(Object data)
        {
            Result result = new Result
            {
                Code = 0,
                Message = String.Empty,
                Data = data
            };
            return result;
        }

        public static Result Done(string message, Object data)
        {
            Result result = new Result
            {
                Code = 0,
                Message = message,
                Data = data,
            };
            return result;
        }

        public static Result Done()
        {
            Result result = new Result
            {
                Code = 0,
                Message = String.Empty,
            };
            return result;
        }

        public static Result Failed(String message)
        {
            return Result.Failed(DEFAULT_FAILIRE_CODE, message);
        }

        public static Result Failed(int code, String message)
        {
            Result result = new Result
            {
                Message = message,
                Code = code
            };
            return result;
        }

        public static Result Failed(int code, String message, Exception cause)
        {
            Result result = new Result
            {
                Message = message,
                Code = code,
                Cause = cause,
            };
            return result;
        }

        public static Result Failed(int code, String message, Object data)
        {
            Result result = new Result
            {
                Message = message,
                Code = code,
                Data = data
            };
            return result;
        }

    }

    /// <summary>
    /// Class Result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; set; }

        /// <summary>
        /// Dones the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Result.</returns>
        public static Result<T> Done(T data)
        {
            Result<T> result = new Result<T>
            {
                Code = 0,
                Message = string.Empty,
                Data = data
            };
            return result;
        }

        /// <summary>
        /// Faileds the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cause">The cause.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static Result<T> Failed(string message, Exception cause)
        {
            Result<T> result = new Result<T>
            {
                Message = message,
                Code = DEFAULT_FAILIRE_CODE,
                Cause = cause,
                Data = default(T)
            };
            return result;
        }

        /// <summary>
        /// Faileds the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="cause">The cause.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static new Result<T> Failed(int code, string message, Exception cause)
        {
            Result<T> result = new Result<T>
            {
                Message = message,
                Code = code,
                Cause = cause,
                Data = default(T)
            };
            return result;
        }
    }
}
