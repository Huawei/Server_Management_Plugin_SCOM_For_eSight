using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.Models
{
    public class TaskResult
    {

        public TaskResult(string message)
        {
            this.Message = message;
        }

        public TaskResult()
        {
            this.SyncResults = new List<SyncResult>();
        }
        public string Message { get; set; }

        public List<SyncResult> SyncResults { get; set; } 
    }

    public class SyncResult
    {
        public Exception Exception { get; set; }

        public string DN { get; set; }

        public string ServerType { get; set; }
        public bool Success { get; set; }
    }
}
