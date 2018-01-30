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
            if (presentState == "1")
            {
                return "Present";
            }
            if (presentState == "0")
            {
                return "Absent";
            }
            return presentState;
        }
    }
}