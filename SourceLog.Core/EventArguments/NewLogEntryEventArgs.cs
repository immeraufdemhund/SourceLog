using System;
using SourceLog.Core.Models;

namespace SourceLog.Core.EventArguments
{
	public class NewLogEntryEventArgs : EventArgs
	{
		public LogEntryDto LogEntry { get; set; }
	}
}