﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using SourceLog.Core.Models;

namespace SourceLog.Model.Tests
{
    [TestClass]
    public class FlowDocumentTests
    {
        [TestMethod]
        public void LeadingTabsAreDisplayed()
        {
            var changedFileDto = new ChangedFileDto
                {
                    OldVersion = System.Text.Encoding.UTF8.GetBytes("\t@ ,"),
                    NewVersion = System.Text.Encoding.UTF8.GetBytes("\t @ ")
                };

            var logEntryDto = new LogEntryDto { ChangedFiles = new List<ChangedFileDto> { changedFileDto } };

            //var mockContext = new Mock<ISourceLogContext>();
            //var logSubscription = new LogSubscription(() => mockContext.Object)
            //{
            //    LogSubscriptionId = 1,
            //    Log = new TrulyObservableCollection<LogEntry>()
            //};

            //var fakeLogSubscriptionDbSet = new FakeLogSubscriptionDbSet { logSubscription };
            //mockContext.Setup(m => m.LogSubscriptions).Returns(fakeLogSubscriptionDbSet);

            //var logEntriesDbSet = new FakeDbSet<LogEntry>();
            //mockContext.Setup(m => m.LogEntries).Returns(logEntriesDbSet);

            //logSubscription.AddNewLogEntry(this, new NewLogEntryEventArgs { LogEntry = logEntry });

            var logEntry = new LogEntry(logEntryDto);

            logEntry.GenerateFlowDocuments();

            var changedFile = logEntry.ChangedFiles.First();

            //var changedFile = logEntriesDbSet.First().ChangedFiles.First();
            var textRange = new TextRange(changedFile.LeftFlowDocument.ContentStart, changedFile.LeftFlowDocument.ContentEnd);
            Assert.IsTrue(textRange.Text.StartsWith("\t"));
        }
    }

    public class FakeLogSubscriptionDbSet : FakeDbSet<LogSubscription>
    {
        public override LogSubscription Find(params object[] keyValues)
        {
            return this.Single(ls => ls.LogSubscriptionId == (int)(keyValues[0]));
        }
    }
}
