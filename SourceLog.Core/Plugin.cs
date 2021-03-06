﻿using System;
using System.Threading;
using SourceLog.Core.EventArguments;

namespace SourceLog.Core
{
    public class Plugin : IPlugin
    {
        protected Timer Timer;
        protected readonly object LockObject = new object();

        public string SettingsXml { get; set; }

        public DateTime MaxDateTimeRetrieved { get; set; }
        public virtual void Initialise()
        {
            SourceLogLogger.LogInformation("Plugin initialising","Plugin." + GetType().Name);

            Timer = new Timer(CheckForNewLogEntries);
            Timer.Change(0, 15000);
        }

        private void CheckForNewLogEntries(object state)
        {
            if (Monitor.TryEnter(LockObject))
            {
                try
                {
                    SourceLogLogger.LogInformation("Checking for new entries","Plugin." + GetType().Name);
                    CheckForNewLogEntriesImpl();
                }
                catch (Exception ex)
                {
                    var args = new PluginExceptionEventArgs { Exception = ex };
                    if (PluginException != null)
                        PluginException(this, args);
                }
                finally
                {
                    Monitor.Exit(LockObject);
                }
            }
        }

        protected virtual void CheckForNewLogEntriesImpl()
        {
            throw new NotImplementedException("Please implement an overriding method in a derived class.");
        }

        public event NewLogEntryEventHandler NewLogEntry;
        public event PluginExceptionEventHandler PluginException;

        protected void OnNewLogEntry(NewLogEntryEventArgs e)
        {
            if (NewLogEntry != null)
                NewLogEntry(this, e);
        }

        protected void OnLogProviderException(PluginExceptionEventArgs e)
        {
            if (PluginException != null)
                PluginException(this, e);
        }

        public void Dispose()
        {
            Timer.Dispose();
            Timer = null;
        }
    }
}
