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


using System;
using System.Collections.Generic;
using System.IO;

using ASC.Files.Core;
using ASC.Web.Files.Core.Entries;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.WCFService.FileOperations;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Web.Files.Services.WCFService
{
    public interface IFileStorageService<T>
    {
        #region Folder Manager

        Folder<T> GetFolder(T folderId);

        ItemList<Folder<T>> GetFolders(T parentId);

        ItemList<object> GetPath(T folderId);

        Folder<T> CreateNewFolder(T parentId, string title);

        Folder<T> FolderRename(T folderId, string title);

        DataWrapper<T> GetFolderItems(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy);

        object GetFolderItemsXml(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy);

        ItemList<FileEntry<T>> GetItems(ItemList<string> items, FilterType filter, bool subjectGroup, string subjectID, string searchText);

        ItemDictionary<string, string> MoveOrCopyFilesCheck(ItemList<string> items, string destFolderId);

        ItemList<FileOperationResult> MoveOrCopyItems(ItemList<string> items, string destFolderId, FileConflictResolveType resolveType, bool isCopyOperation, bool deleteAfter = false);

        ItemList<FileOperationResult> DeleteItems(string action, ItemList<string> items, bool ignoreException = false, bool deleteAfter = false, bool immediately = false);

        void ReassignStorage(Guid userFromId, Guid userToId);

        void DeleteStorage(Guid userId);

        #endregion

        #region File Manager

        File GetFile(string fileId, int version);

        File<T> CreateNewFile(FileModel<T> fileWrapper);

        File FileRename(string fileId, string title);

        KeyValuePair<File<T>, ItemList<File<T>>> UpdateToVersion(T fileId, int version);

        KeyValuePair<File<T>, ItemList<File<T>>> CompleteVersion(T fileId, int version, bool continueVersion);

        string UpdateComment(T fileId, int version, string comment);

        ItemList<File<T>> GetFileHistory(T fileId);

        ItemList<File<T>> GetSiblingsFile(T fileId, T folderId, FilterType filter, bool subjectGroup, string subjectID, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy);

        KeyValuePair<bool, string> TrackEditFile(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish);

        ItemDictionary<string, string> CheckEditing(ItemList<string> filesId);

        File SaveEditing(string fileId, string fileExtension, string fileuri, Stream stream, string doc, bool forcesave);

        File UpdateFileStream(string fileId, Stream stream, bool encrypted, bool forcesave);

        string StartEdit(string fileId, bool editingAlone, string doc);

        ItemList<FileOperationResult> CheckConversion(ItemList<ItemList<string>> filesIdVersion);

        File LockFile(string fileId, bool lockFile);

        ItemList<EditHistory> GetEditHistory(string fileId, string doc);

        EditHistoryData GetEditDiffUrl(string fileId, int version, string doc = null);

        ItemList<EditHistory> RestoreVersion(string fileId, int version, string url, string doc = null);

        Web.Core.Files.DocumentService.FileLink GetPresignedUri(string fileId);

        #endregion

        #region Favorites Manager

        ItemList<FileEntry<T>> AddToFavorites(ItemList<T> foldersId, ItemList<T> filesId);

        ItemList<FileEntry<T>> DeleteFavorites(ItemList<T> foldersId, ItemList<T> filesId);

        #endregion

        #region Templates Manager

        ItemList<FileEntry<T>> AddToTemplates(ItemList<T> filesId);

        ItemList<FileEntry<T>> DeleteTemplates(ItemList<T> filesId);

        ItemList<FileEntry<T>> GetTemplates(FilterType filter, int from, int count, bool subjectGroup, string ssubject, string searchText, bool searchInContent);

        #endregion


        #region Utils

        ItemList<FileEntry<T>> ChangeOwner(ItemList<string> items, Guid userId);

        ItemList<FileOperationResult> BulkDownload(Dictionary<string, string> items);

        ItemList<FileOperationResult> GetTasksStatuses();

        ItemList<FileOperationResult> EmptyTrash();

        ItemList<FileOperationResult> TerminateTasks();

        string GetShortenLink(string fileId);

        bool StoreOriginal(bool store);

        bool HideConfirmConvert(bool isForSave);

        bool UpdateIfExist(bool update);

        bool Forcesave(bool value);

        bool StoreForcesave(bool value);

        bool DisplayRecent(bool value);

        bool DisplayFavorite(bool value);

        bool DisplayTemplates(bool value);

        bool ChangeDeleteConfrim(bool update);

        string GetHelpCenter();

        #endregion

        #region Ace Manager

        ItemList<AceWrapper> GetSharedInfo(ItemList<string> objectId);

        ItemList<AceShortWrapper> GetSharedInfoShort(string objectId);

        List<T> SetAceObject(AceCollection<T> aceCollection, bool notify);

        void RemoveAce(ItemList<string> items);

        ItemList<FileOperationResult> MarkAsRead(ItemList<string> items);

        object GetNewItems(string folderId);

        bool SetAceLink(string fileId, FileShare share);

        ItemList<MentionWrapper> SharedUsers(string fileId);

        ItemList<AceShortWrapper> SendEditorNotify(string fileId, MentionMessageWrapper mentionMessage);

        ItemList<EncryptionKeyPair> GetEncryptionAccess(T fileId);

        #endregion

        #region ThirdParty

        ItemList<ThirdPartyParams> GetThirdParty();

        ItemList<Folder<T>> GetThirdPartyFolder(int folderType);

        Folder<T> SaveThirdParty(ThirdPartyParams thirdPartyParams);

        object DeleteThirdParty(string providerId);

        bool ChangeAccessToThirdparty(bool enableThirdpartySettings);

        bool SaveDocuSign(string code);

        object DeleteDocuSign();

        string SendDocuSign(string fileId, DocuSignData docuSignData);

        #endregion

        #region MailMerge

        ItemList<string> GetMailAccounts();

        #endregion
    }
}