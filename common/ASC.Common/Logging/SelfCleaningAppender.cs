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

namespace ASC.Common.Logging;

public class SelfCleaningAppender : RollingFileAppender
{
    private static DateTime _lastCleanDate;
    private static int? _cleanPeriod;

    protected override void Append(LoggingEvent loggingEvent)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Append(loggingEvent);
    }

    protected override void Append(LoggingEvent[] loggingEvents)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Append(loggingEvents);
    }

    private static int GetCleanPeriod()
    {
        if (_cleanPeriod != null)
        {
            return _cleanPeriod.Value;
        }

        const string key = "CleanPeriod";

        var value = 30;

        var repo = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
        if (repo != null && repo.Properties.GetKeys().Contains(key))
        {
            int.TryParse(repo.Properties[key].ToString(), out value);
        }

        _cleanPeriod = value;

        return value;
    }

    private void Clean()
    {
        try
        {
            if (string.IsNullOrEmpty(File))
            {
                return;
            }

            var fileInfo = new FileInfo(File);
            if (!fileInfo.Exists)
            {
                return;
            }

            var directory = fileInfo.Directory;
            if (directory == null || !directory.Exists)
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
            LogLog.Error(GetType(), err.Message, err);
        }
    }
}
