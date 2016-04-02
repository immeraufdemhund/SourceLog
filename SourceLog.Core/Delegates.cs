using SourceLog.Core.EventArguments;

namespace SourceLog.Core
{

    public delegate void NewLogEntryEventHandler(object sender, NewLogEntryEventArgs e);

    public delegate void PluginExceptionEventHandler(object sender, PluginExceptionEventArgs args);
}
