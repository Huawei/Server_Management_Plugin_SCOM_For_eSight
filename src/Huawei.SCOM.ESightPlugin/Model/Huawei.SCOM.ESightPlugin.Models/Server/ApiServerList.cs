using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models.Server
{
    public class ApiServerList<T>
    {
        public ApiServerList()
        {
            this.Data = new List<T>();
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int TotalSize { get; set; }

        public int TotalPage { get; set; }

        public List<T> Data { get; set; }
    }
}
