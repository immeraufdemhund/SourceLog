using System;

namespace SourceLog.Core
{
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Represents plugin specific repo connection information
        /// </summary>
        string SettingsXml { get; set; }

        DateTime MaxDateTimeRetrieved { get; set; }

        /// <summary>
        /// Create an interval function that uses uses SettingsXml and MaxDateTimeRetrieved
        /// to check for new log entries.  Calls NewLogEntry for each new entry.
        /// </summary>
        void Initialise();

        event NewLogEntryEventHandler NewLogEntry;

        event PluginExceptionEventHandler PluginException;
    }
}