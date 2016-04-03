namespace SourceLog.Core.Models
{
    public class LogSubscription
    {
        public int LogSubscriptionId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string PluginTypeName { get; set; }
    }
}
