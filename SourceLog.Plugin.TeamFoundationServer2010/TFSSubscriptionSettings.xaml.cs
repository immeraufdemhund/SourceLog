using SourceLog.Core;
using System.Xml.Linq;

namespace SourceLog.Plugin.TeamFoundationServer2010
{
    public partial class TFSSubscriptionSettings : ISubscriptionSettings
    {
        public TFSSubscriptionSettings()
        {
            InitializeComponent();
        }

        public string SettingsXml
        {
            get
            {
                return new XDocument(
                    new XElement("Settings",
                        new XElement("CollectionURL", txtCollectionUrl.Text),
                        new XElement("SourceLocation", txtSourceLocation.Text))
                ).ToString();
            }
            set
            {
                var settingsXml = XDocument.Parse(value);
                txtCollectionUrl.Text = settingsXml.Root.Element("CollectionURL").Value;
                txtSourceLocation.Text = settingsXml.Root.Element("SourceLocation").Value;
            }
        }
    }
}
