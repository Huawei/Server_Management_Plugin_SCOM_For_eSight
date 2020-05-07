using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESightPlugin.Unittest
{
    [TestClass]
    public class EventLogUnittest
    {

        protected const string ESightEventLogName = "HUAWEI ESight Events";
        protected const string ESightEventSource = "HUAWEI ESight Event Notification";

        private static bool EventSourceExists(string source)
        {
            if (System.Diagnostics.EventLog.SourceExists(source))
            {
                System.Diagnostics.EventLog evLog = new System.Diagnostics.EventLog { Source = source };
                if (evLog.Log != ESightEventLogName)
                {
                    System.Diagnostics.EventLog.DeleteEventSource(source);
                }
                evLog.Dispose();
            }

            if (!System.Diagnostics.EventLog.SourceExists(source))
            {
                System.Diagnostics.EventLog.CreateEventSource(source, ESightEventLogName);
            }

            return System.Diagnostics.EventLog.SourceExists(source);
        }

        static int CategoryCount = 3;

        static void CreateEventSourceSample(string sourceName, string logName, string messageFile)
        {

            // Create the event source if it does not exist.
            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
            {
                EventSourceCreationData mySourceData = new EventSourceCreationData(sourceName, logName);

                // Set the message resource file that the event source references.
                // All event resource identifiers correspond to text in this file.
                if (!System.IO.File.Exists(messageFile))
                {
                    Console.WriteLine("Input message resource file does not exist - {0}",
                        messageFile);
                    messageFile = "";
                }
                else
                {
                    // Set the specified file as the resource
                    // file for message text, category text, and 
                    // message parameter strings.  

                    mySourceData.MessageResourceFile = messageFile;
                    mySourceData.CategoryResourceFile = messageFile;
                    mySourceData.CategoryCount = CategoryCount;
                    mySourceData.ParameterResourceFile = messageFile;

                    Console.WriteLine("Event source message resource file set to {0}",
                        messageFile);
                }

                Console.WriteLine("Registering new source for event log.");
                System.Diagnostics.EventLog.CreateEventSource(mySourceData);
            }
            else
            {
                // Get the event log corresponding to the existing source.
                logName = System.Diagnostics.EventLog.LogNameFromSourceName(sourceName, ".");
            }

            // Register the localized name of the event log.
            // For example, the actual name of the event log is "myNewLog," but
            // the event log name displayed in the Event Viewer might be
            // "Sample Application Log" or some other application-specific
            // text.
            System.Diagnostics.EventLog myEventLog = new System.Diagnostics.EventLog(logName, ".", sourceName);

            if (messageFile.Length > 0)
            {
                myEventLog.RegisterDisplayName(messageFile, 5001);
            }
        }


        [TestMethod]
        public void InsertEventLog()
        {
            CreateEventSourceSample(ESightEventSource, ESightEventLogName, "ESightEventLogMessages.mc");

            // Define an informational event and a warning event.

            // The message identifiers correspond to the message text in the
            // message resource file defined for the source.
            EventInstance myInfoEvent = new EventInstance(1000, 1, EventLogEntryType.Information);
            EventInstance myWarningEvent = new EventInstance(1001, 1, EventLogEntryType.Warning);

            // Insert the method name into the event log message.
            string[] insertStrings = { "EventLogSamples.WriteEventSample2" };

            // Write the events to the event log.

            System.Diagnostics.EventLog.WriteEvent(ESightEventSource, myInfoEvent);

            // Append binary data to the warning event entry.
            byte[] binaryData = { 7, 8, 9, 10 };
            System.Diagnostics.EventLog.WriteEvent(ESightEventSource, myWarningEvent, binaryData, insertStrings);
        }

        [TestMethod]
        public void InsertEventLog2()
        {
            if (EventSourceExists(ESightEventSource))
            {
                //EventLog.WriteEntry(ESightEventSource, "this is a sample event", EventLogEntryType.Error);

                EventInstance instance = new EventInstance(1000, 1, EventLogEntryType.Information);
                EventLog.WriteEvent(ESightEventSource, instance, new string[] { "Description: this is desc", "User Action: parameter2" });
                //EventLog.WriteEntry("Saravanan", "Application logs an entry", EventLogEntryType.Information, 2, 0);
                //EventLog.WriteEvent("Saravanan", new EventInstance(2, 3),  "Application logs an event");
            }
        }

        [TestMethod]
        public void QueryEventLogWithParameterContent()
        {
            // EventLogQ
            String query = $@"Event[System/Provider/@Name=""{ESightEventSource}"" and EventData[Data[1] and Data=""User Action: parameter2""]]";
            EventLogQuery eventlogQuery = new EventLogQuery(ESightEventLogName, PathType.LogName, query);
            EventLogReader eventlogReader = new EventLogReader(eventlogQuery);

            // Loop through the events returned
            for (EventRecord eventRecord = eventlogReader.ReadEvent(); null != eventRecord; eventRecord = eventlogReader.ReadEvent())
            {
                // Get the description from the eventrecord. 
                string message = eventRecord.FormatDescription();
                Trace.WriteLine(message);
                // Do something cool with it :) 
            }

        }

        [TestMethod]
        public void QueryPowershellEventLog()
        {
            var LogName = "Windows PowerShell";
            var LogSource = "PowerShell";
            // EventLogQ
            String query = $@"*[System/Provider/@Name=""{LogSource}""]";
            EventLogQuery eventlogQuery = new EventLogQuery(LogName, PathType.LogName, query);
            EventLogReader eventlogReader = new EventLogReader(eventlogQuery);

            Console.WriteLine(eventlogReader.BatchSize);
            // Loop through the events returned
            for (EventRecord eventRecord = eventlogReader.ReadEvent(); null != eventRecord; eventRecord = eventlogReader.ReadEvent())
            {
                // Get the description from the eventrecord. 
                string message = eventRecord.FormatDescription();
                Trace.WriteLine(message);
                Console.WriteLine(message);
                // Do something cool with it :) 
            }

        }
    }
}
