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
        /// The convert mezz health status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertMezzHealthStatus(string status)
        {
            switch (status)
            {
                case "1": return "0";
                case "-2":
                case "5": return "-1";
                default: return "-2";
            }
        }

        /// <summary>
        /// The convert status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertStatus(string status)
        {
            switch (status)
            {
                case "0": return "0";
                case "-1":
                case "-2": return "-1";
                default: return "-2";
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