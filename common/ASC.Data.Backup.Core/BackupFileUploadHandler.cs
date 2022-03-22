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

namespace ASC.Web.Studio.Core.Backup;

[Scope]
public class BackupFileUploadHandler
{
    private const long _maxBackupFileSize = 1024L * 1024L * 1024L;
    private const string _backupTempFolder = "backup";
    private const string _backupFileName = "backup.tmp";

    private readonly PermissionContext _permissionContext;
    private readonly TempPath _tempPath;
    private readonly TenantManager _tenantManager;

    public BackupFileUploadHandler(
        PermissionContext permissionContext,
        TempPath tempPath,
        TenantManager tenantManager)
    {
        _permissionContext = permissionContext;
        _tempPath = tempPath;
        _tenantManager = tenantManager;
    }

    public string ProcessUpload(IFormFile file)
    {
        if (file == null)
        {
            return "No files.";
        }

        if (!_permissionContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
        {
            return "Access denied.";
        }

        if (file.Length <= 0 || file.Length > _maxBackupFileSize)
        {
            return $"File size must be greater than 0 and less than {_maxBackupFileSize} bytes";
        }

        try
        {
            var filePath = GetFilePath();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }

            return string.Empty;
        }
        catch (Exception error)
        {
            return error.Message;
        }
    }

    internal string GetFilePath()
    {
        var folder = Path.Combine(_tempPath.GetTempPath(), _backupTempFolder, _tenantManager.GetCurrentTenant().Id.ToString());

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return Path.Combine(folder, _backupFileName);
    }
}
