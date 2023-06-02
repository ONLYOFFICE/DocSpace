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

namespace ASC.Web.Files.Services.DocumentService;

[Scope(Additional = typeof(ConfigurationExtention))]
public class DocumentServiceHelper
{
    private readonly IDaoFactory _daoFactory;
    private readonly FileShareLink _fileShareLink;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly FileSecurity _fileSecurity;
    private readonly SetupInfo _setupInfo;
    private readonly FileUtility _fileUtility;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly Global _global;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly LockerManager _lockerManager;
    private readonly FileTrackerHelper _fileTracker;
    private readonly EntryStatusManager _entryStatusManager;
    private readonly IServiceProvider _serviceProvider;

    public DocumentServiceHelper(
        IDaoFactory daoFactory,
        FileShareLink fileShareLink,
        UserManager userManager,
        AuthContext authContext,
        FileSecurity fileSecurity,
        SetupInfo setupInfo,
        FileUtility fileUtility,
        MachinePseudoKeys machinePseudoKeys,
        Global global,
        DocumentServiceConnector documentServiceConnector,
        LockerManager lockerManager,
        FileTrackerHelper fileTracker,
        EntryStatusManager entryStatusManager,
        IServiceProvider serviceProvider)
    {
        _daoFactory = daoFactory;
        _fileShareLink = fileShareLink;
        _userManager = userManager;
        _authContext = authContext;
        _fileSecurity = fileSecurity;
        _setupInfo = setupInfo;
        _fileUtility = fileUtility;
        _machinePseudoKeys = machinePseudoKeys;
        _global = global;
        _documentServiceConnector = documentServiceConnector;
        _lockerManager = lockerManager;
        _fileTracker = fileTracker;
        _entryStatusManager = entryStatusManager;
        _serviceProvider = serviceProvider;
    }

    public async Task<(File<T> File, Configuration<T> Configuration, bool LocatedInPrivateRoom)> GetParamsAsync<T>(T fileId, int version, string doc, bool editPossible, bool tryEdit, bool tryCoauth)
    {
        var lastVersion = true;
        FileShare linkRight;

        var fileDao = _daoFactory.GetFileDao<T>();

        var fileOptions = await _fileShareLink.CheckAsync(doc, fileDao);
        var file = fileOptions.File;
        linkRight = fileOptions.FileShare;

        if (file == null)
        {
            var curFile = await fileDao.GetFileAsync(fileId);

            if (curFile != null && 0 < version && version < curFile.Version)
            {
                file = await fileDao.GetFileAsync(fileId, version);
                lastVersion = false;
            }
            else
            {
                file = curFile;
            }
        }

        return await GetParamsAsync(file, lastVersion, linkRight, true, true, editPossible, tryEdit, tryCoauth);
    }

    public async Task<(File<T> File, Configuration<T> Configuration, bool LocatedInPrivateRoom)> GetParamsAsync<T>(File<T> file, bool lastVersion, FileShare linkRight, bool rightToRename, bool rightToEdit, bool editPossible, bool tryEdit, bool tryCoauth)
    {
        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (!string.IsNullOrEmpty(file.Error))
        {
            throw new Exception(file.Error);
        }

        var rightToReview = rightToEdit;
        var reviewPossible = editPossible;

        var rightToFillForms = rightToEdit;
        var fillFormsPossible = editPossible;

        var rightToComment = rightToEdit;
        var commentPossible = editPossible;

        var rightModifyFilter = rightToEdit;

        rightToEdit = rightToEdit
                      && (linkRight == FileShare.ReadWrite || linkRight == FileShare.CustomFilter
                          || await _fileSecurity.CanEditAsync(file) || await _fileSecurity.CanCustomFilterEditAsync(file));
        if (editPossible && !rightToEdit)
        {
            editPossible = false;
        }

        rightModifyFilter = rightModifyFilter
            && (linkRight == FileShare.ReadWrite
                || await _fileSecurity.CanEditAsync(file));

        rightToRename = rightToRename && rightToEdit && await _fileSecurity.CanRenameAsync(file);

        rightToReview = rightToReview
                        && (linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                            || await _fileSecurity.CanReviewAsync(file));
        if (reviewPossible && !rightToReview)
        {
            reviewPossible = false;
        }

        rightToFillForms = rightToFillForms
                           && (linkRight == FileShare.FillForms || linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                               || await _fileSecurity.CanFillFormsAsync(file));
        if (fillFormsPossible && !rightToFillForms)
        {
            fillFormsPossible = false;
        }

        rightToComment = rightToComment
                         && (linkRight == FileShare.Comment || linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                             || await _fileSecurity.CanCommentAsync(file));
        if (commentPossible && !rightToComment)
        {
            commentPossible = false;
        }

        if (linkRight == FileShare.Restrict
            && !(editPossible || reviewPossible || fillFormsPossible || commentPossible)
            && !await _fileSecurity.CanReadAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        }

        if (file.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        if (file.ContentLength > _setupInfo.AvailableFileSize)
        {
            throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeEdit, FileSizeComment.FilesSizeToString(_setupInfo.AvailableFileSize)));
        }

        string strError = null;
        if ((editPossible || reviewPossible || fillFormsPossible || commentPossible)
            && await _lockerManager.FileLockedForMeAsync(file.Id))
        {
            if (tryEdit)
            {
                strError = FilesCommonResource.ErrorMassage_LockedFile;
            }

            rightToRename = false;
            rightToEdit = editPossible = false;
            rightToReview = reviewPossible = false;
            rightToFillForms = fillFormsPossible = false;
            rightToComment = commentPossible = false;
        }

        if (editPossible
            && !_fileUtility.CanWebEdit(file.Title))
        {
            rightToEdit = editPossible = false;
        }

        var locatedInPrivateRoom = false;

        if (file.RootFolderType == FolderType.VirtualRooms)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();

            locatedInPrivateRoom = await DocSpaceHelper.LocatedInPrivateRoomAsync(file, folderDao);
        }

        if (file.Encrypted
            && file.RootFolderType != FolderType.Privacy && !locatedInPrivateRoom)
        {
            rightToEdit = editPossible = false;
            rightToReview = reviewPossible = false;
            rightToFillForms = fillFormsPossible = false;
            rightToComment = commentPossible = false;
        }


        if (!editPossible && !_fileUtility.CanWebView(file.Title))
        {
            throw new Exception($"{FilesCommonResource.ErrorMassage_NotSupportedFormat} ({FileUtility.GetFileExtension(file.Title)})");
        }

        if (reviewPossible &&
            !_fileUtility.CanWebReview(file.Title))
        {
            rightToReview = reviewPossible = false;
        }

        if (fillFormsPossible &&
            !_fileUtility.CanWebRestrictedEditing(file.Title))
        {
            rightToFillForms = fillFormsPossible = false;
        }

        if (commentPossible &&
            !_fileUtility.CanWebComment(file.Title))
        {
            rightToComment = commentPossible = false;
        }

        var rightChangeHistory = rightToEdit && !file.Encrypted;

        if (_fileTracker.IsEditing(file.Id))
        {
            rightChangeHistory = false;

            bool coauth;
            if ((editPossible || reviewPossible || fillFormsPossible || commentPossible)
                && tryCoauth
                && (!(coauth = _fileUtility.CanCoAuhtoring(file.Title)) || _fileTracker.IsEditingAlone(file.Id)))
            {
                if (tryEdit)
                {
                    var editingBy = _fileTracker.GetEditingBy(file.Id).FirstOrDefault();
                    strError = string.Format(!coauth
                                                 ? FilesCommonResource.ErrorMassage_EditingCoauth
                                                 : FilesCommonResource.ErrorMassage_EditingMobile,
                                             await _global.GetUserNameAsync(editingBy, true));
                }
                rightToEdit = editPossible = reviewPossible = fillFormsPossible = commentPossible = false;
            }
        }

        var fileStable = file;
        if (lastVersion && file.Forcesave != ForcesaveType.None && tryEdit)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            fileStable = await fileDao.GetFileStableAsync(file.Id, file.Version);
        }

        var docKey = await GetDocKeyAsync(fileStable);
        var modeWrite = (editPossible || reviewPossible || fillFormsPossible || commentPossible) && tryEdit;

        if (file.ParentId != null)
        {
            await _entryStatusManager.SetFileStatusAsync(file);
        }

        var rightToDownload = await CanDownloadAsync(_fileSecurity, file, linkRight);

        var configuration = new Configuration<T>(file, _serviceProvider)
        {
            Document =
                {
                    Key = docKey,
                    Permissions =
                    {
                        Edit = rightToEdit && lastVersion,
                        Rename = rightToRename && lastVersion && !file.ProviderEntry,
                        Review = rightToReview && lastVersion,
                        FillForms = rightToFillForms && lastVersion,
                        Comment = rightToComment && lastVersion,
                        ChangeHistory = rightChangeHistory,
                        ModifyFilter = rightModifyFilter,
                        Print = rightToDownload,
                        Download = rightToDownload
                    }
                },
            EditorConfig =
                {
                    ModeWrite = modeWrite,
                },
            ErrorMessage = strError,
        };

        if (!lastVersion)
        {
            configuration.Document.Title += $" ({file.CreateOnString})";
        }

        if (_fileUtility.CanWebRestrictedEditing(file.Title))
        {
            var linkDao = _daoFactory.GetLinkDao();
            var sourceId = await linkDao.GetSourceAsync(file.Id.ToString());
            configuration.Document.IsLinkedForMe = !string.IsNullOrEmpty(sourceId);
        }

        return (file, configuration, locatedInPrivateRoom);
    }

    private async Task<bool> CanDownloadAsync<T>(FileSecurity fileSecurity, File<T> file, FileShare linkRight)
    {
        if (!file.DenyDownload)
        {
            return true;
        }

        var canDownload = linkRight != FileShare.Restrict && linkRight != FileShare.Read && linkRight != FileShare.Comment;

        if (canDownload || _authContext.CurrentAccount.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID))
        {
            return canDownload;
        }

        if (linkRight == FileShare.Read || linkRight == FileShare.Comment)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            file = await fileDao.GetFileAsync(file.Id); // reset Access prop
        }

        canDownload = await fileSecurity.CanDownloadAsync(file);

        return canDownload;
    }

    public string GetSignature(object payload)
    {
        if (string.IsNullOrEmpty(_fileUtility.SignatureSecret))
        {
            return null;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        var encoder = new JwtEncoder(new HMACSHA256Algorithm(),
                                     new JwtSerializer(),
                                     new JwtBase64UrlEncoder());
#pragma warning restore CS0618 // Type or member is obsolete


        return encoder.Encode(payload, _fileUtility.SignatureSecret);
    }


    public async Task<string> GetDocKeyAsync<T>(File<T> file)
    {
        return await GetDocKeyAsync(file.Id, file.Version, file.ProviderEntry ? file.ModifiedOn : file.CreateOn);
    }

    public async Task<string> GetDocKeyAsync<T>(T fileId, int fileVersion, DateTime modified)
    {
        var str = $"teamlab_{fileId}_{fileVersion}_{modified.GetHashCode()}_{await _global.GetDocDbKeyAsync()}";

        var keyDoc = Encoding.UTF8.GetBytes(str)
                             .ToList()
                             .Concat(_machinePseudoKeys.GetMachineConstant())
                             .ToArray();

        return DocumentServiceConnector.GenerateRevisionId(Hasher.Base64Hash(keyDoc, HashAlg.SHA256));
    }

    public async Task CheckUsersForDropAsync<T>(File<T> file)
    {
        var sharedLink =
            await _fileSecurity.CanEditAsync(file, FileConstant.ShareLinkId)
            || await _fileSecurity.CanCustomFilterEditAsync(file, FileConstant.ShareLinkId)
            || await _fileSecurity.CanReviewAsync(file, FileConstant.ShareLinkId)
            || await _fileSecurity.CanFillFormsAsync(file, FileConstant.ShareLinkId)
            || await _fileSecurity.CanCommentAsync(file, FileConstant.ShareLinkId);

        var usersDrop = new List<string>();

        foreach (var uid in _fileTracker.GetEditingBy(file.Id))
        {
            if (!await _userManager.UserExistsAsync(uid) && !sharedLink)
            {
                usersDrop.Add(uid.ToString());
                continue;
            }

            if (!await _fileSecurity.CanEditAsync(file, uid)
                && !await _fileSecurity.CanCustomFilterEditAsync(file, uid)
                && !await _fileSecurity.CanReviewAsync(file, uid)
                && !await _fileSecurity.CanFillFormsAsync(file, uid)
                && !await _fileSecurity.CanCommentAsync(file, uid))
            {
                usersDrop.Add(uid.ToString());
            }
        }

        if (usersDrop.Count == 0)
        {
            return;
        }

        var fileStable = file;
        if (file.Forcesave != ForcesaveType.None)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            fileStable = await fileDao.GetFileStableAsync(file.Id, file.Version);
        }

        var docKey = await GetDocKeyAsync(fileStable);

        await DropUserAsync(docKey, usersDrop.ToArray(), file.Id);
    }

    public async Task<bool> DropUserAsync(string docKeyForTrack, string[] users, object fileId = null)
    {
        return await _documentServiceConnector.CommandAsync(CommandMethod.Drop, docKeyForTrack, fileId, null, users);
    }

    public async Task<bool> RenameFileAsync<T>(File<T> file, IFileDao<T> fileDao)
    {
        if (!_fileUtility.CanWebView(file.Title)
            && !_fileUtility.CanWebCustomFilterEditing(file.Title)
            && !_fileUtility.CanWebEdit(file.Title)
            && !_fileUtility.CanWebReview(file.Title)
            && !_fileUtility.CanWebRestrictedEditing(file.Title)
            && !_fileUtility.CanWebComment(file.Title))
        {
            return true;
        }

        var fileStable = file.Forcesave == ForcesaveType.None ? file : await fileDao.GetFileStableAsync(file.Id, file.Version);
        var docKeyForTrack = await GetDocKeyAsync(fileStable);

        var meta = new MetaData { Title = file.Title };

        return await _documentServiceConnector.CommandAsync(CommandMethod.Meta, docKeyForTrack, file.Id, meta: meta);
    }
}
