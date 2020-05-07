using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class WindowEventLogHelper
    {

        /// <summary>
        /// Create a new event source and bind to the EventLog named "logName".
        /// If source has been bind to other EventLog, will delete the source first.
        /// </summary>
        /// <param name="source">event source</param>
        /// <param name="logName">event log name</param>
        /// <returns>true if success else false</returns>
        public static bool CreateEventSourceIfNotExists(string source, string logName)
        {
            if (EventLog.SourceExists(source))
            {
                EventLog eventlog = new EventLog { Source = source };
                if (eventlog.Log != logName)
                {
                    EventLog.DeleteEventSource(source);
                }
                eventlog.Dispose();
            }

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }

            if (EventLog.SourceExists(source))
            {
                EventLog log = new EventLog(logName);
                log.MaximumKilobytes = 1024 * 256;
                log.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
                log.Dispose();
            }

            return EventLog.SourceExists(source);
        }

        public static bool CreateEventSourceIfNotExists(object eVENT_SOURCE, string eVENT_LOG_NAME)
        {
            throw new NotImplementedException();
        }
    }
}
