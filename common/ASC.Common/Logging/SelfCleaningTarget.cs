// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using LogLevel = NLog.LogLevel;

namespace ASC.Common.Logging;

[Target("SelfCleaning")]
public class SelfCleaningTarget : FileTarget
{
    private static DateTime _lastCleanDate;
    private static int? _cleanPeriod;

    protected override void Write(IList<AsyncLogEventInfo> logEvents)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        var buffer = new List<AsyncLogEventInfo>();

        foreach (var logEvent in logEvents)
        {
            buffer.Add(logEvent);
            if (buffer.Count < 10)
            {
                continue;
            }

            base.Write(buffer);
            buffer.Clear();
        }

        base.Write(buffer);
    }

    protected override void Write(LogEventInfo logEvent)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Write(logEvent);
    }

    private static int GetCleanPeriod()
    {
        if (_cleanPeriod != null)
        {
            return _cleanPeriod.Value;
        }

        var value = 30;

        const string key = "cleanPeriod";

            if (LogManager.Configuration.Variables.TryGetValue(key, out var variable))
        {
            if (variable != null && !string.IsNullOrEmpty(variable.Text))
            {
                int.TryParse(variable.Text, out value);
            }
        }

        _cleanPeriod = value;

        return value;
    }

    private void Clean()
    {
        var filePath = string.Empty;
        var dirPath = string.Empty;

        try
        {
            if (FileName == null)
            {
                return;
            }

            filePath = ((NLog.Layouts.SimpleLayout)FileName).Text;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            dirPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }
            if (!Path.IsPathRooted(dirPath))
            {
                dirPath = CrossPlatform.PathCombine(AppDomain.CurrentDomain.BaseDirectory, dirPath);
            }

            var directory = new DirectoryInfo(dirPath);
            if (!directory.Exists)
            {
                return;
            }

            var files = directory.GetFiles();
            var cleanPeriod = GetCleanPeriod();

            foreach (var file in files.Where(file => (DateTime.UtcNow.Date - file.CreationTimeUtc.Date).Days > cleanPeriod))
            {
                file.Delete();
            }
        }
        catch (Exception err)
        {
            base.Write(new LogEventInfo
            {
                Exception = err,
                Level = LogLevel.Error,
                    Message = $"file: {filePath}, dir: {dirPath}, mess: {err.Message}",
                LoggerName = "SelfCleaningTarget"
            });
        }
    }
}
