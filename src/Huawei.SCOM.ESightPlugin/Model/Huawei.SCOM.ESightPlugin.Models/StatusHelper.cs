// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The status helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Huawei.SCOM.ESightPlugin.Models
{
    /// <summary>
    /// The status helper.
    /// </summary>
    public class StatusHelper
    {

        /// <summary>
        /// 20180625 统一健康状态
        /// Success  0
        /// Warnning  2 / 3 / 5
        /// Critical  4 / 6 / 7 / 8
        /// 与上次状态等级保持一致  -1 / -2 / Others
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ConvertStatus(string status)
        {
            switch (status)
            {
                case "0":
                    return "0";
                case "2":
                case "3":
                case "5":
                    return "-1";
                case "4":
                case "6":
                case "7":
                case "8":
                    return "-2";
                default://其他返回-3 标识健康状态本次不做更新
                    return "-3";
            }
        }

        /// <summary>
        /// The get present state. 
        /// CPU、Memory、Disk、电源、风扇、Board六种部件的在位状态,按照0展示为不在位、-2和2位未知其余为在位 ，未知显示为： Unkown 
        /// </summary>
        /// <param name="presentState">
        /// The present state.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPresentState(string presentState)
        {
            if (presentState == null)
            {
                return "Present";
            }
            if (presentState == "0")
            {
                return "Absent";
            }
            if (presentState == "-2" || presentState == "2")
            {
                return "Unkown";
            }
            return "Present";
        }
    }
}