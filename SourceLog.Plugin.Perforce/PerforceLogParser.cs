﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SourceLog.Core;
using SourceLog.Core.Models;

namespace SourceLog.Plugin.Perforce
{
    public static class PerforceLogParser
    {
        internal static LogEntryDto Parse(string changesetString)
        {
            SourceLogLogger.LogInformation("Parsing changeset: " + changesetString, "Plugin.Perforce");

            var logEntry = new LogEntryDto();
            const string pattern = @"Change\s(?<revision>\d+)\son\s(?<datetime>\d{4}/\d{2}/\d{2}\s\d{2}:\d{2}:\d{2})\sby\s(?<author>[^@]+)@\w+\n\n(?<message>.*?(?=\n([^\s]|$)))";
            var r = new Regex(pattern, RegexOptions.Singleline);
            var match = r.Match(changesetString);
            if (match.Success)
            {
                int revision;
                if (Int32.TryParse(match.Groups["revision"].Value, out revision))
                    logEntry.Revision = revision.ToString(CultureInfo.InvariantCulture);

                DateTime datetime;
                if (DateTime.TryParse(match.Groups["datetime"].Value, out datetime))
                    logEntry.CommittedDate = datetime;

                logEntry.Author = match.Groups["author"].Value;
                var message = match.Groups["message"].Value;
                logEntry.Message = message.Trim().Replace("\n\t", "\n");

            }
            else
            {
                SourceLogLogger.LogError("Parsing changeset: " + changesetString, "Plugin.Perforce");
            }

            return logEntry;
        }

        internal static ChangedFileDto ParseP4File(string file)
        {
            SourceLogLogger.LogInformation("Parsing file: " + file, "Plugin.Perforce");

            var changedFile = new ChangedFileDto();
            const string pattern = @"(?<filename>[^#]*)#(?<revision>\d+)\s-\s(?<action>\w+)\schange\s(?<changeNumber>\d+)\s\((?<filetype>\w+(\+\w+)?)\)";
            var r = new Regex(pattern);
            var match = r.Match(file);
            if (match.Success)
            {
                changedFile.FileName = match.Groups["filename"].Value;
                switch (match.Groups["action"].Value)
                {
                    case "add":
                        changedFile.ChangeType = ChangeType.Added;
                        break;
                    case "edit":
                        changedFile.ChangeType = ChangeType.Modified;
                        break;
                    case "delete":
                        changedFile.ChangeType = ChangeType.Deleted;
                        break;
                    case "branch":
                        changedFile.ChangeType = ChangeType.Copied;
                        break;
                    case "integrate":
                        changedFile.ChangeType = ChangeType.Modified;
                        break;
                }
            }
            else
            {
                SourceLogLogger.LogError("Parsing file failed: " + file, "Plugin.Perforce");
            }

            return changedFile;
        }
    }
}
