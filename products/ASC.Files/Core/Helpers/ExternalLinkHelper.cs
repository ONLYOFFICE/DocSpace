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

using Status = ASC.Files.Core.Security.Status;

namespace ASC.Files.Core.Helpers;

[Scope]
public class ExternalLinkHelper
{
    private readonly ExternalShare _externalShare;
    private readonly RoomLogoManager _roomLogoManager;
    private readonly SecurityContext _securityContext;
    private readonly IDaoFactory _daoFactory;

    public ExternalLinkHelper(ExternalShare externalShare, RoomLogoManager roomLogoManager, SecurityContext securityContext, IDaoFactory daoFactory)
    {
        _externalShare = externalShare;
        _roomLogoManager = roomLogoManager;
        _securityContext = securityContext;
        _daoFactory = daoFactory;
    }

    public async Task<ValidationInfo> ValidateAsync(string key, string password = null)
    {
        var result = new ValidationInfo
        {
            Status = Status.Invalid, 
            Access = FileShare.Restrict
        };

        var linkId = await _externalShare.ParseShareKeyAsync(key);
        var securityDao = _daoFactory.GetSecurityDao<int>();

        var record = await securityDao.GetSharesAsync(new[] { linkId }).FirstOrDefaultAsync();

        if (record == null)
        {
            return result;
        }

        var status = await _externalShare.ValidateRecordAsync(record, password);
        result.Status = status;

        if (status != Status.Ok && status != Status.RequiredPassword)
        {
            return result;
        }

        var entryId = record.EntryId.ToString();

        var entry = int.TryParse(entryId, out var id)
            ? await GetEntryAndProcessAsync(id, result)
            : await GetEntryAndProcessAsync(entryId, result);

        if (entry == null || entry.RootFolderType is FolderType.TRASH or FolderType.Archive)
        {
            result.Status = Status.Invalid;
            return result;
        }

        if (status == Status.RequiredPassword)
        {
            return result;
        }
        
        result.Access = record.Share;
        result.TenantId = record.TenantId;

        if (_securityContext.IsAuthenticated)
        {
            if (entry.CreateBy.Equals(_securityContext.CurrentAccount.ID))
            {
                result.Shared = true;
            }
            else
            {
                var existedRecord = await securityDao.GetSharesAsync(new[] { _securityContext.CurrentAccount.ID })
                    .FirstOrDefaultAsync(r => r.EntryId.ToString() == entryId);

                result.Shared = existedRecord != null;
            }
        }

        if (_securityContext.IsAuthenticated || !string.IsNullOrEmpty(_externalShare.GetAnonymousSessionKey()))
        {
            return result;
        }
        
        await _externalShare.SetAnonymousSessionKeyAsync();

        return result;
    }

    private async Task<FileEntry> GetEntryAndProcessAsync<T>(T id, ValidationInfo info)
    {
        var folder = await _daoFactory.GetFolderDao<T>().GetFolderAsync(id);

        if (folder == null)
        {
            return null;
        }

        info.Id = folder.Id.ToString();
        info.Title = folder.Title;
        info.FolderType = folder.FolderType;
        
        var logo = await _roomLogoManager.GetLogoAsync(folder);

        if (!logo.IsDefault())
        {
            info.Logo = logo;
        }
        
        return folder;
    }
}