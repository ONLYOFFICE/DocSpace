/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

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

    public async Task<(File<T> File, Configuration<T> Configuration)> GetParamsAsync<T>(T fileId, int version, string doc, bool editPossible, bool tryEdit, bool tryCoauth)
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

    public async Task<(File<T> File, Configuration<T> Configuration)> GetParamsAsync<T>(File<T> file, bool lastVersion, FileShare linkRight, bool rightToRename, bool rightToEdit, bool editPossible, bool tryEdit, bool tryCoauth)
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

        if (linkRight == FileShare.Restrict && _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            rightToEdit = false;
            rightToReview = false;
            rightToFillForms = false;
            rightToComment = false;
        }

        var fileSecurity = _fileSecurity;
        rightToEdit = rightToEdit
                      && (linkRight == FileShare.ReadWrite || linkRight == FileShare.CustomFilter
                          || await fileSecurity.CanEditAsync(file) || await fileSecurity.CanCustomFilterEditAsync(file));
        if (editPossible && !rightToEdit)
        {
            editPossible = false;
        }

        rightModifyFilter = rightModifyFilter
            && (linkRight == FileShare.ReadWrite
                || await fileSecurity.CanEditAsync(file));

        rightToRename = rightToRename && rightToEdit && await fileSecurity.CanEditAsync(file);

        rightToReview = rightToReview
                        && (linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                            || await fileSecurity.CanReviewAsync(file));
        if (reviewPossible && !rightToReview)
        {
            reviewPossible = false;
        }

        rightToFillForms = rightToFillForms
                           && (linkRight == FileShare.FillForms || linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                               || await fileSecurity.CanFillFormsAsync(file));
        if (fillFormsPossible && !rightToFillForms)
        {
            fillFormsPossible = false;
        }

        rightToComment = rightToComment
                         && (linkRight == FileShare.Comment || linkRight == FileShare.Review || linkRight == FileShare.ReadWrite
                             || await fileSecurity.CanCommentAsync(file));
        if (commentPossible && !rightToComment)
        {
            commentPossible = false;
        }

        if (linkRight == FileShare.Restrict
            && !(editPossible || reviewPossible || fillFormsPossible || commentPossible)
            && !await fileSecurity.CanReadAsync(file))
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
            && await _lockerManager.FileLockedForMeAsync(file.ID))
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

        if (file.Encrypted
            && file.RootFolderType != FolderType.Privacy)
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

        if (_fileTracker.IsEditing(file.ID))
        {
            rightChangeHistory = false;

            bool coauth;
            if ((editPossible || reviewPossible || fillFormsPossible || commentPossible)
                && tryCoauth
                && (!(coauth = _fileUtility.CanCoAuhtoring(file.Title)) || _fileTracker.IsEditingAlone(file.ID)))
            {
                if (tryEdit)
                {
                    var editingBy = _fileTracker.GetEditingBy(file.ID).FirstOrDefault();
                    strError = string.Format(!coauth
                                                 ? FilesCommonResource.ErrorMassage_EditingCoauth
                                                 : FilesCommonResource.ErrorMassage_EditingMobile,
                                             _global.GetUserName(editingBy, true));
                }
                rightToEdit = editPossible = reviewPossible = fillFormsPossible = commentPossible = false;
            }
        }

        var fileStable = file;
        if (lastVersion && file.Forcesave != ForcesaveType.None && tryEdit)
        {
            var fileDao = _daoFactory.GetFileDao<T>();
            fileStable = await fileDao.GetFileStableAsync(file.ID, file.Version);
        }

        var docKey = GetDocKey(fileStable);
        var modeWrite = (editPossible || reviewPossible || fillFormsPossible || commentPossible) && tryEdit;

        if (file.FolderID != null)
        {
            await _entryStatusManager.SetFileStatusAsync(file);
        }

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
                        ModifyFilter = rightModifyFilter
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

        return (file, configuration);
    }


    public string GetSignature(object payload)
    {
        if (string.IsNullOrEmpty(_fileUtility.SignatureSecret))
        {
            return null;
        }

        return JsonWebToken.Encode(payload, _fileUtility.SignatureSecret);
    }


    public string GetDocKey<T>(File<T> file)
    {
        return GetDocKey(file.ID, file.Version, file.ProviderEntry ? file.ModifiedOn : file.CreateOn);
    }

    public string GetDocKey<T>(T fileId, int fileVersion, DateTime modified)
    {
        var str = $"teamlab_{fileId}_{fileVersion}_{modified.GetHashCode()}_{_global.GetDocDbKey()}";

        var keyDoc = Encoding.UTF8.GetBytes(str)
                             .ToList()
                             .Concat(_machinePseudoKeys.GetMachineConstant())
                             .ToArray();

        return DocumentServiceConnector.GenerateRevisionId(Hasher.Base64Hash(keyDoc, HashAlg.SHA256));
    }

    public async Task CheckUsersForDropAsync<T>(File<T> file)
    {
        var fileSecurity = _fileSecurity;
        var sharedLink =
            await fileSecurity.CanEditAsync(file, FileConstant.ShareLinkId)
            || await fileSecurity.CanCustomFilterEditAsync(file, FileConstant.ShareLinkId)
            || await fileSecurity.CanReviewAsync(file, FileConstant.ShareLinkId)
            || await fileSecurity.CanFillFormsAsync(file, FileConstant.ShareLinkId)
            || await fileSecurity.CanCommentAsync(file, FileConstant.ShareLinkId);

        var usersDrop = new List<string>();

        foreach (var uid in _fileTracker.GetEditingBy(file.ID))
        {
            if (!_userManager.UserExists(uid) && !sharedLink)
            {
                usersDrop.Add(uid.ToString());
                continue;
            }

            if (!await fileSecurity.CanEditAsync(file, uid)
                && !await fileSecurity.CanCustomFilterEditAsync(file, uid)
                && !await fileSecurity.CanReviewAsync(file, uid)
                && !await fileSecurity.CanFillFormsAsync(file, uid)
                && !await fileSecurity.CanCommentAsync(file, uid))
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
            fileStable = await fileDao.GetFileStableAsync(file.ID, file.Version);
        }

        var docKey = GetDocKey(fileStable);

        await DropUserAsync(docKey, usersDrop.ToArray(), file.ID);
    }

    public Task<bool> DropUserAsync(string docKeyForTrack, string[] users, object fileId = null)
    {
        return _documentServiceConnector.CommandAsync(CommandMethod.Drop, docKeyForTrack, fileId, null, users);
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

        var fileStable = file.Forcesave == ForcesaveType.None ? file : await fileDao.GetFileStableAsync(file.ID, file.Version);
        var docKeyForTrack = GetDocKey(fileStable);

        var meta = new MetaData { Title = file.Title };

        return await _documentServiceConnector.CommandAsync(CommandMethod.Meta, docKeyForTrack, file.ID, meta: meta);
    }
}
