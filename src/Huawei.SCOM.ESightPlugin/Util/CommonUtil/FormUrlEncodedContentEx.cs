using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CommonUtil
{
    /// <summary>
    /// 做FormUrlEncoded http提交的转码工具类。
    /// </summary>
    public class FormUrlEncodedContentEx : ByteArrayContent
    {
        /// <summary>
        /// Initializes a new instance of the FormUrlEncodedContentEx class with a specific collection of name/value pairs.
        /// </summary>
        /// <param name="nameValueCollection">A collection of name/value pairs.</param>
        public FormUrlEncodedContentEx(IEnumerable<KeyValuePair<string, object>> nameValueCollection) : base(FormUrlEncodedContentEx.GetContentByteArray(nameValueCollection))
        {
            base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 转换key/value数组为byte数组。
        /// </summary>
        /// <param name="nameValueCollection">key/value数组</param>
        /// <returns></returns>
        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, object>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> current in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }
                stringBuilder.Append(FormUrlEncodedContentEx.Encode(current.Key));
                stringBuilder.Append('=');
                if (current.Value is string)
                {
                    stringBuilder.Append(StringifyValue(current.Value));
                }
                else
                {
                    stringBuilder.Append(CoreUtil.GetObjTranNull<string>(current.Value).ToLower());
                }
            }
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }
        /// <summary>
        /// 对字符串进行url处理。
        /// </summary>
        /// <param name="data">需要编码的字符</param>
        /// <returns></returns>
        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return Uri.EscapeDataString(data).Replace("%20", "+");
        }
        /// <summary>
        /// 对象转字符
        /// </summary>
        /// <param name="value">对象实例</param>
        /// <returns></returns>
        private static string StringifyValue(object value)
        {
            if (value is string)
            {
                return FormUrlEncodedContentEx.Encode((string)value);
            }
            else if (value is IEnumerable<string>)//集合对象类型。
            {
                StringBuilder sb = new StringBuilder();
                bool firstElement = true;
                sb.Append("[");
                foreach (string item in (IEnumerable<string>)value)
                {
                    if (firstElement)
                    {
                        firstElement = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append("\"");
                    sb.Append(FormUrlEncodedContentEx.Encode(item));
                    sb.Append("\"");
                }
                sb.Append("]");
                return sb.ToString();
            }
            else
            {
                throw new InvalidOperationException("FormUrlEncodedContentEx cannot handle " + value.GetType().ToString() + " values");
            }
        }
    }
 
}
