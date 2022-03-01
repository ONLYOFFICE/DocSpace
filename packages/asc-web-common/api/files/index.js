import { request } from "../client";
import axios from "axios";
import FilesFilter from "./filter";
import { FolderType } from "../../constants";
import find from "lodash/find";

export function openEdit(fileId, version, doc, view) {
  const params = []; // doc ? `?doc=${doc}` : "";

  if (view) {
    params.push(`view=${view}`);
  }

  if (version) {
    params.push(`version=${version}`);
  }

  if (doc) {
    params.push(`doc=${doc}`);
  }

  const paramsString = params.length > 0 ? `?${params.join("&")}` : "";

  const options = {
    method: "get",
    url: `/files/file/${fileId}/openedit${paramsString}`,
  };

  return request(options);
}

export function getFolderInfo(folderId) {
  const options = {
    method: "get",
    url: `/files/folder/${folderId}`,
  };

  return request(options);
}

export function getFolderPath(folderId) {
  const options = {
    method: "get",
    url: `/files/folder/${folderId}/path`,
  };

  return request(options);
}

export function getFolder(folderId, filter) {
  if (folderId && typeof folderId === "string") {
    folderId = encodeURIComponent(folderId.replace(/\\\\/g, "\\"));
  }

  const params =
    filter && filter instanceof FilesFilter
      ? `${folderId}?${filter.toApiUrlParams()}`
      : folderId;
  const options = {
    method: "get",
    url: `/files/${params}`,
  };

  return request(options);
}

const getFolderClassNameByType = (folderType) => {
  switch (folderType) {
    case FolderType.USER:
      return "tree-node-my";
    case FolderType.SHARE:
      return "tree-node-share";
    case FolderType.COMMON:
      return "tree-node-common";
    case FolderType.Projects:
      return "tree-node-projects";
    case FolderType.Favorites:
      return "tree-node-favorites";
    case FolderType.Recent:
      return "tree-node-recent";
    case FolderType.Privacy:
      return "tree-node-privacy";
    case FolderType.TRASH:
      return "tree-node-trash";
    default:
      return "";
  }
};

const sortInDisplayOrder = (folders) => {
  const sorted = [];

  const myFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.USER
  );
  myFolder && sorted.push(myFolder);

  const shareFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.SHARE
  );
  shareFolder && sorted.push(shareFolder);

  const favoritesFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.Favorites
  );
  favoritesFolder && sorted.push(favoritesFolder);

  const recentFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.Recent
  );
  recentFolder && sorted.push(recentFolder);

  const privateFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.Privacy
  );
  privateFolder && sorted.push(privateFolder);

  const commonFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.COMMON
  );
  commonFolder && sorted.push(commonFolder);

  const projectsFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.Projects
  );
  projectsFolder && sorted.push(projectsFolder);

  const trashFolder = find(
    folders,
    (folder) => folder.current.rootFolderType == FolderType.TRASH
  );
  trashFolder && sorted.push(trashFolder);

  return sorted;
};

export function getFoldersTree() {
  return request({ method: "get", url: "/files/@root?filterType=2" }).then(
    (response) => {
      const folders = sortInDisplayOrder(response);
      return folders.map((data, index) => {
        const type = +data.current.rootFolderType;
        const name = getFolderClassNameByType(type);
        const isRecycleBinFolder = type === FolderType.TRASH;
        return {
          id: data.current.id,
          key: `0-${index}`,
          parentId: data.current.parentId,
          title: data.current.title,
          rootFolderType: type,
          folderClassName: name,
          // folders: !isRecycleBinFolder
          //   ? data.folders.map((folder) => {
          //       return {
          //         id: folder.id,
          //         title: folder.title,
          //         access: folder.access,
          //         foldersCount: folder.foldersCount,
          //         rootFolderType: folder.rootFolderType,
          //         providerKey: folder.providerKey,
          //         newItems: folder.new,
          //       };
          //     })
          //   : null,
          folders: null,
          pathParts: data.pathParts,
          foldersCount: !isRecycleBinFolder
            ? data.current.foldersCount || data.folders.length
            : null,
          newItems: data.new,
        };
      });
    }
  );
}

export function getMyFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@my`,
  };

  return request(options);
}

export function getCommonFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@common`,
  };

  return request(options);
}

export function getFavoritesFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@favorites`,
  };

  return request(options);
}

export function getProjectsFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@projects`,
  };

  return request(options);
}

export function getTrashFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@trash`,
  };

  return request(options);
}

export function getSharedFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@share`,
  };

  return request(options);
}

export function getRecentFolderList(filter = FilesFilter.getDefault()) {
  const options = {
    method: "get",
    url: `/files/@recent`,
  };

  return request(options);
}

export function createFolder(parentFolderId, title) {
  const data = { title };
  const options = {
    method: "post",
    url: `/files/folder/${parentFolderId}`,
    data,
  };

  return request(options);
}

export function renameFolder(folderId, title) {
  const data = { title };
  const options = {
    method: "put",
    url: `/files/folder/${folderId}`,
    data,
  };

  return request(options);
}

export function deleteFolder(folderId, deleteAfter, immediately) {
  const data = { deleteAfter, immediately };
  const options = {
    method: "delete",
    url: `/files/folder/${folderId}`,
    data,
  };

  return request(options);
}

export function createFile(folderId, title, templateId) {
  const data = { title, templateId };
  const options = {
    method: "post",
    url: `/files/${folderId}/file`,
    data,
  };

  return request(options);
}

export function createTextFile(folderId, title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: `/files/${folderId}/text`,
    data,
  };

  return request(options);
}

export function createTextFileInMy(title) {
  const data = { title };
  const options = {
    method: "post",
    url: "/files/@my/file",
    data,
  };

  return request(options);
}

export function createTextFileInCommon(title) {
  const data = { title };
  const options = {
    method: "post",
    url: "/files/@common/file",
    data,
  };

  return request(options);
}

export function createHtmlFile(folderId, title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: `/files/${folderId}/html`,
    data,
  };

  return request(options);
}

export function createHtmlFileInMy(title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: "/files/@my/html",
    data,
  };

  return request(options);
}

export function createHtmlFileInCommon(title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: "/files/@common/html",
    data,
  };

  return request(options);
}

export function getFileInfo(fileId) {
  const options = {
    method: "get",
    url: `/files/file/${fileId}`,
  };

  return request(options);
}

export function updateFile(fileId, title, lastVersion) {
  const data = { title, lastVersion };
  const options = {
    method: "put",
    url: `/files/file/${fileId}`,
    data,
  };

  return request(options);
}

export function addFileToRecentlyViewed(fileId) {
  const data = { fileId };
  const options = {
    method: "post",
    url: `/files/file/${fileId}/recent`,
    data,
  };

  return request(options);
}

export function deleteFile(fileId, deleteAfter, immediately) {
  const data = { deleteAfter, immediately };
  const options = {
    method: "delete",
    url: `/files/file/${fileId}`,
    data,
  };

  return request(options);
}

export function emptyTrash() {
  return request({ method: "put", url: "/files/fileops/emptytrash" });
}

export function removeFiles(folderIds, fileIds, deleteAfter, immediately) {
  const data = { folderIds, fileIds, deleteAfter, immediately };
  return request({ method: "put", url: "/files/fileops/delete", data });
}

export function getShareFiles(fileIds, folderIds) {
  const data = { fileIds, folderIds };
  return request({
    method: "post",
    url: "/files/share",
    data,
  });
}

export function setExternalAccess(fileId, accessType) {
  const data = { share: accessType };
  return request({
    method: "put",
    url: `/files/${fileId}/setacelink`,
    data,
  });
}

export function setShareFiles(
  fileIds,
  folderIds,
  share,
  notify,
  sharingMessage
) {
  const data = { fileIds, folderIds, share, notify, sharingMessage };

  return request({
    method: "put",
    url: "/files/share",
    data,
  });
}

export function removeShareFiles(fileIds, folderIds) {
  const data = { fileIds, folderIds };
  return request({
    method: "delete",
    url: "/files/share",
    data,
  });
}

export function setFileOwner(folderIds, fileIds, userId) {
  const data = { folderIds, fileIds, userId };
  return request({
    method: "post",
    url: "/files/owner",
    data,
  });
}

export function startUploadSession(
  folderId,
  fileName,
  fileSize,
  relativePath,
  encrypted
) {
  const data = { fileName, fileSize, relativePath, encrypted };
  return request({
    method: "post",
    url: `/files/${folderId}/upload/create_session.json`,
    data,
  });
}

export function uploadFile(url, data) {
  return axios.post(url, data);
}

export function downloadFiles(fileIds, folderIds) {
  const data = { fileIds, folderIds };
  return request({ method: "put", url: "/files/fileops/bulkdownload", data });
}

export function getProgress() {
  return request({ method: "get", url: "/files/fileops" });
}

export function checkFileConflicts(destFolderId, folderIds, fileIds) {
  let paramsString =
    folderIds.length > 0 ? `&folderIds=${folderIds.join("&folderIds=")}` : "";
  paramsString +=
    fileIds.length > 0 ? `&fileIds=${fileIds.join("&fileIds=")}` : "";

  return request({
    method: "get",
    url: `/files/fileops/move.json?destFolderId=${destFolderId}${paramsString}`,
  });
}

export function copyToFolder(
  destFolderId,
  folderIds,
  fileIds,
  conflictResolveType,
  deleteAfter
) {
  const data = {
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter,
  };
  return request({ method: "put", url: "/files/fileops/copy", data });
}

export function moveToFolder(
  destFolderId,
  folderIds,
  fileIds,
  conflictResolveType,
  deleteAfter
) {
  const data = {
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter,
  };
  return request({ method: "put", url: "/files/fileops/move", data });
}

export function getFileVersionInfo(fileId) {
  return request({
    method: "get",
    url: `/files/file/${fileId}/history`,
  });
}

export function markAsRead(folderIds, fileIds) {
  const data = { folderIds, fileIds };
  return request({ method: "put", url: "/files/fileops/markasread", data });
}

export function getNewFiles(folderId) {
  return request({
    method: "get",
    url: `/files/${folderId}/news`,
  });
}

export function convertFile(fileId, password = null, sync = false) {
  const data = { password, sync };

  return request({
    method: "put",
    url: `/files/file/${fileId}/checkconversion`,
    data,
  });
}

export function getFileConversationProgress(fileId) {
  return request({
    method: "get",
    url: `/files/file/${fileId}/checkconversion`,
  });
}

export function finalizeVersion(fileId, version, continueVersion) {
  const data = { fileId, version, continueVersion };
  return request({
    method: "put",
    url: `/files/file/${fileId}/history`,
    data,
  });
}

export function markAsVersion(fileId, continueVersion, version) {
  const data = { continueVersion, version };
  return request({ method: "put", url: `/files/file/${fileId}/history`, data });
}

export function versionEditComment(fileId, comment, version) {
  const data = { comment, version };
  return request({ method: "put", url: `/files/file/${fileId}/comment`, data });
}

export function versionRestore(fileId, lastversion) {
  const data = { lastversion };
  return request({ method: "put", url: `/files/file/${fileId}`, data });
}

export function lockFile(fileId, lockFile) {
  const data = { lockFile };
  return request({ method: "put", url: `/files/file/${fileId}/lock`, data });
}

export function updateIfExist(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/updateifexist", data });
}

export function storeOriginal(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/storeoriginal", data });
}

export function changeDeleteConfirm(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/changedeleteconfrim", data });
}

export function storeForceSave(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/storeforcesave", data });
}

export function forceSave(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/forcesave", data });
}

export function thirdParty(val) {
  const data = { set: val };
  return request({ method: "put", url: "files/thirdparty", data });
}

export function getThirdPartyList() {
  return request({ method: "get", url: "files/thirdparty" });
}

export function saveThirdParty(
  url,
  login,
  password,
  token,
  isCorporate,
  customerTitle,
  providerKey,
  providerId
) {
  const data = {
    url,
    login,
    password,
    token,
    isCorporate,
    customerTitle,
    providerKey,
    providerId,
  };
  return request({ method: "post", url: "files/thirdparty", data });
}

export function deleteThirdParty(providerId) {
  return request({ method: "delete", url: `files/thirdparty/${providerId}` });
}

export function getThirdPartyCapabilities() {
  return request({ method: "get", url: "files/thirdparty/capabilities" });
}

export function openConnectWindow(service) {
  return request({ method: "get", url: `thirdparty/${service}` });
}

export function getSettingsFiles() {
  return request({ method: "get", url: `/files/settings` });
}

export function markAsFavorite(ids) {
  const data = { fileIds: ids };
  const options = {
    method: "post",
    url: "/files/favorites",
    data,
  };

  return request(options);
}

export function removeFromFavorite(ids) {
  const data = { fileIds: ids };
  const options = {
    method: "delete",
    url: "/files/favorites",
    data,
  };

  return request(options);
}

export function getDocServiceUrl() {
  return request({ method: "get", url: `/files/docservice` });
}

export function getIsEncryptionSupport() {
  return request({
    method: "get",
    url: "/files/@privacy/available",
  });
}

export function setEncryptionKeys(keys) {
  const data = {
    publicKey: keys.publicKey,
    privateKeyEnc: keys.privateKeyEnc,
    enable: keys.enable,
    update: keys.update,
  };
  return request({
    method: "put",
    url: "privacyroom/keys",
    data,
  });
}

export function getEncryptionKeys() {
  return request({
    method: "get",
    url: "privacyroom/keys",
  });
}

export function getEncryptionAccess(fileId) {
  return request({
    method: "get",
    url: `privacyroom/access/${fileId}`,
    data: fileId,
  });
}

export function updateFileStream(file, fileId, encrypted, forcesave) {
  let fd = new FormData();
  fd.append("file", file);
  fd.append("encrypted", encrypted);
  fd.append("forcesave", forcesave);

  return request({
    method: "put",
    url: `/files/${fileId}/update`,
    data: fd,
  });
}

export function setFavoritesSetting(set) {
  return request({
    method: "put",
    url: "/files/settings/favorites",
    data: { set },
  });
}

export function setRecentSetting(set) {
  return request({
    method: "put",
    url: "/files/displayRecent",
    data: { set },
  });
}

export function hideConfirmConvert(save) {
  return request({
    method: "put",
    url: "/files/hideconfirmconvert.json",
    data: { save },
  });
}

export function getSubfolders(folderId) {
  return request({
    method: "get",
    url: `files/${folderId}/subfolders`,
  });
}

export function createThumbnails(fileIds) {
  const options = {
    method: "post",
    url: "/files/thumbnails",
    data: { fileIds: fileIds },
  };

  return request(options);
}

export function getPresignedUri(fileId) {
  return request({
    method: "get",
    url: `files/file/${fileId}/presigned`,
  });
}

export function checkFillFormDraft(fileId) {
  return request({
    method: "post",
    url: `files/masterform/${fileId}/checkfillformdraft`,
    data: { fileId },
  });
}

export function fileCopyAs(
  fileId,
  destTitle,
  destFolderId,
  enableExternalExt,
  password
) {
  return request({
    method: "post",
    url: `files/file/${fileId}/copyas`,
    data: {
      destTitle,
      destFolderId,
      enableExternalExt,
      password,
    },
  });
}

export function getEditHistory(fileId, doc) {
  return request({
    method: "get",
    url: `files/file/${fileId}/edit/history?doc=${doc}`,
  });
}

export function getEditDiff(fileId, version, doc) {
  return request({
    method: "get",
    url: `files/file/${fileId}/edit/diff?version=${version}&doc=${doc}`,
  });
}

export function restoreDocumentsVersion(fileId, version, doc) {
  const options = {
    method: "get",
    url: `files/file/${fileId}/restoreversion?version=${version}&doc=${doc}`,
  };

  return request(options);
}
