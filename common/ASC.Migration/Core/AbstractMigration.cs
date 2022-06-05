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

namespace ASC.Migration.Core;

public abstract class AbstractMigration<TMigrationInfo, TUser, TContacts, TCalendar, TFiles, TMail> : IMigration
    where TMigrationInfo : IMigrationInfo
{
    private readonly MigrationLogger _logger;
    protected CancellationToken _cancellationToken;
    protected TMigrationInfo _migrationInfo;
    private double _lastProgressUpdate;
    private string _lastStatusUpdate;
    protected List<Guid> _importedUsers;

    public event Action<double, string> OnProgressUpdate;

    public AbstractMigration(MigrationLogger migrationLogger)
    {
        _logger = migrationLogger;
    }

    protected void ReportProgress(double value, string status)
    {
        _lastProgressUpdate = value;
        _lastStatusUpdate = status;
        OnProgressUpdate?.Invoke(value, status);
        _logger.Log($"{value:0.00} progress: {status}");
    }

    public double GetProgress() => _lastProgressUpdate;
    public string GetProgressStatus() => _lastStatusUpdate;

    public abstract void Init(string path, CancellationToken cancellationToken);

    public abstract Task<MigrationApiInfo> Parse();

    public abstract Task Migrate(MigrationApiInfo migrationInfo);

    public void Log(string msg, Exception exception = null)
    {
        _logger.Log(msg, exception);
    }
    public virtual void Dispose()
    {
        _logger.Dispose();
    }

    public Stream GetLogs()
    {
        return _logger.GetStream();
    }

    public virtual List<Guid> GetGuidImportedUsers()
    {
        return _importedUsers;
    }
}
