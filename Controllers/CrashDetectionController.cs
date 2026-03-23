using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SystemControlApp.Controllers
{
    public class AppCrashInfo
    {
        public string ApplicationName { get; set; } = "";
        public string FaultingModule { get; set; } = "";
        public string ExceptionCode { get; set; } = "";
        public DateTime Time { get; set; }
        public string RawMessage { get; set; } = "";
    }

    public static class CrashDetectionController
    {
        public static List<AppCrashInfo> GetRecentCrashes(int lastMinutes = 10)
        {
            var crashes = new List<AppCrashInfo>();

            using EventLog applicationLog = new EventLog("Application");

            foreach (EventLogEntry entry in applicationLog.Entries)
            {
                if (entry.EntryType != EventLogEntryType.Error)
                    continue;

                if (entry.InstanceId != 1000 && entry.InstanceId != 1001)
                    continue;

                if (entry.TimeGenerated < DateTime.Now.AddMinutes(-lastMinutes))
                    continue;

                var crash = ParseCrash(entry);
                if (crash != null)
                    crashes.Add(crash);
            }

            return crashes;
        }

        private static AppCrashInfo? ParseCrash(EventLogEntry entry)
        {
            try
            {
                var info = new AppCrashInfo
                {
                    Time = entry.TimeGenerated,
                    RawMessage = entry.Message
                };

                string message = entry.Message;

                info.ApplicationName = ExtractValue(message, "Faulting application name:");
                info.FaultingModule = ExtractValue(message, "Faulting module name:");
                info.ExceptionCode = ExtractValue(message, "Exception code:");

                return info;
            }
            catch
            {
                return null;
            }
        }

        private static string ExtractValue(string text, string key)
        {
            int index = text.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return "";

            int start = index + key.Length;
            int end = text.IndexOf('\n', start);

            if (end < 0) end = text.Length;

            return text.Substring(start, end - start).Trim();
        }
    }
}
