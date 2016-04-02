using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System;

namespace SourceLog
{
    public static class SourceLogLogger
    {
        public static void LogInformation(string message, string category = null)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                Severity = TraceEventType.Information,
            };
            if (!string.IsNullOrEmpty(category))
                logEntry.Categories.Add(category);

            Logger.Write(logEntry);
        }
        public static void LogError(string message, string category = null)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                Severity = TraceEventType.Error,
            };
            if (!string.IsNullOrEmpty(category))
                logEntry.Categories.Add(category);

            Logger.Write(logEntry);
        }
        public static void LogWarning(string message, string category = null)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                Severity = TraceEventType.Warning,
            };
            if (!string.IsNullOrEmpty(category))
                logEntry.Categories.Add(category);

            Logger.Write(logEntry);
        }

        public static void Log(object ex)
        {
            Logger.Write(ex);
        }
    }
}
