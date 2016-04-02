﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using SourceLog.Core.EventArguments;
using SourceLog.Core.Models;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TFS = Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;
using System.Xml.Linq;
using CoreChangeType = SourceLog.Core.ChangeType;

namespace SourceLog.Plugin.TeamFoundationServer2010
{
    public class TeamFoundationServer2010Plugin : Core.Plugin
    {
        protected override void CheckForNewLogEntriesImpl()
        {
            string collectionUrl;
            string sourceLocation;
            GetTfsSettings(out collectionUrl, out sourceLocation);

            var tfsUri = new Uri(collectionUrl);
            var projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsUri);

            var vcs = projectCollection.GetService<VersionControlServer>();
            var history = vcs.QueryHistory(
                path: sourceLocation,
                version: VersionSpec.Latest,
                deletionId: 0,
                recursion: RecursionType.Full,
                user: null,
                versionFrom: null,
                versionTo: null,
                maxCount: 30,
                includeChanges: true,
                slotMode: false
            )
            .Cast<Changeset>()
            .ToList();

            foreach (var changeset in
                history.Where(c => c.CreationDate > MaxDateTimeRetrieved).OrderBy(c => c.CreationDate))
            {
                var changesetId = changeset.ChangesetId;
                Logger.Write(new LogEntry { Message = "Creating LogEntry for Changeset " + changesetId, Categories = { "Plugin.TFS2010" } });

                var logEntry = new LogEntryDto
                {
                    Author = changeset.Committer,
                    CommittedDate = changeset.CreationDate,
                    Message = changeset.Comment,
                    Revision = changesetId.ToString(CultureInfo.InvariantCulture),
                    ChangedFiles = new List<ChangedFileDto>()
                };

                foreach (var change in changeset.Changes)
                {
                    var changedFile = new ChangedFileDto { FileName = change.Item.ServerItem };
                    switch (change.Item.ItemType)
                    {
                        case ItemType.Folder:

                            // XamlReader.Load seems to require UTF8
                            var folderStringBytes = System.Text.Encoding.UTF8.GetBytes("[Folder]");
                            
                            if(change.ChangeType.HasFlag(TFS.ChangeType.Add))
                                changedFile.OldVersion = new byte[0];
                            else
                                changedFile.OldVersion = folderStringBytes;

                            if (change.ChangeType.HasFlag(TFS.ChangeType.Delete))
                                changedFile.NewVersion = new byte[0];
                            else
                                changedFile.NewVersion = folderStringBytes;
                            
                            break;

                        case ItemType.File:
                            
                            if (change.ChangeType.HasFlag(TFS.ChangeType.Delete))
                                changedFile.NewVersion = new byte[0];
                            else
                                using (var memoryStream = new MemoryStream())
                                {
                                    change.Item.DownloadFile().CopyTo(memoryStream);
                                    changedFile.NewVersion = memoryStream.ToArray();
                                }

                            var previousVersion = vcs.GetItem(change.Item.ItemId, changesetId - 1, true);
                            if (previousVersion != null)
                                using (var previousVersionMemoryStream = new MemoryStream())
                                {
                                    previousVersion.DownloadFile().CopyTo(previousVersionMemoryStream);
                                    changedFile.OldVersion = previousVersionMemoryStream.ToArray();
                                }
                            else
                                changedFile.OldVersion = new byte[0];
                            
                            break;

                        default:
                            continue;
                    }

                    SetChangeType(changedFile, change);
                    logEntry.ChangedFiles.Add(changedFile);
                }

                var args = new NewLogEntryEventArgs { LogEntry = logEntry };
                OnNewLogEntry(args);
            }
            MaxDateTimeRetrieved = history.Max(c => c.CreationDate);
        }

        private void GetTfsSettings(out string collectionUrl, out string sourceLocation)
        {
            var settingsXml = XDocument.Parse(SettingsXml);
            // ReSharper disable PossibleNullReferenceException
            collectionUrl = settingsXml.Root.Element("CollectionURL").Value;
            sourceLocation = settingsXml.Root.Element("SourceLocation").Value;
            // ReSharper restore PossibleNullReferenceException
        }

        private static void SetChangeType(ChangedFileDto changedFile, Change change)
        {
            if (change.ChangeType.HasFlag(ChangeType.Add))
                changedFile.ChangeType = CoreChangeType.Added;
            else if (change.ChangeType.HasFlag(ChangeType.Delete))
                changedFile.ChangeType = CoreChangeType.Deleted;
            else
                changedFile.ChangeType = CoreChangeType.Modified;
        }
    }
}
