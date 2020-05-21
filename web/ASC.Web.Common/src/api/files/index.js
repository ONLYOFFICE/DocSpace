import { request } from "../client";
import axios from "axios";
import FilesFilter from "./filter";
import * as fakeFiles from "./fake";

export function openEdit(fileId) {
  const options = {
    method: "get",
    url: `/files/file/${fileId}/openedit`
  };

  return request(options);
}

export function getFolderInfo(folderId) {
  const options = {
    method: "get",
    url: `/files/folder/${folderId}`
  };

  return request(options);
}

export function getFolderPath(folderId) {
  const options = {
    method: "get",
    url: `/files/folder/${folderId}/path`
  };

  return request(options);
}

export function getFolder(folderId, filter, fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Fake folder");
  }

  const params =
    filter && filter instanceof FilesFilter
      ? `${folderId}?${filter.toUrlParams()}`
      : folderId;

  const options = {
    method: "get",
    url: `/files/${params}`
  };

  return request(options);
}

export function getFoldersTree() {
  const rootFoldersPaths = ['@my', '@share', '@common', /*'@projects',*/ '@trash']; //TODO: need get from settings
  const requestsArray = rootFoldersPaths.map(path => request({ method: "get", url: `/files/${path}` }));

  return axios.all(requestsArray)
    .then(axios.spread((...responses) =>
      responses.map((data, index) => {
        const trashIndex = 3;
        return {
          id: data.current.id,
          key: `0-${index}`,
          title: data.current.title,
          folders: index !== trashIndex ? data.folders.map(folder => {
            return {
              id: folder.id,
              title: folder.title,
              foldersCount: folder.foldersCount
            }
          }) : null,
          pathParts: data.pathParts,
          foldersCount: index !== trashIndex ? data.current.foldersCount : null
        }
      })
    ))
}

export function getMyFolderList(filter = FilesFilter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "My Documents");
  }

  const options = {
    method: "get",
    url: `/files/@my`
  };

  return request(options);
}

export function getCommonFolderList(filter = FilesFilter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Common Documents");
  }

  const options = {
    method: "get",
    url: `/files/@common`
  };

  return request(options);
}

export function getProjectsFolderList(filter = FilesFilter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Project Documents");
  }

  const options = {
    method: "get",
    url: `/files/@projects`
  };

  return request(options);
}

export function getTrashFolderList(filter = FilesFilter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Recycle Bin");
  }

  const options = {
    method: "get",
    url: `/files/@trash`
  };

  return request(options);
}

export function getSharedFolderList(filter = FilesFilter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Shared with Me");
  }

  const options = {
    method: "get",
    url: `/files/@share`
  };

  return request(options);
}

export function createFolder(parentFolderId, title) {
  const data = { title };
  const options = {
    method: "post",
    url: `/files/folder/${parentFolderId}`,
    data
  };

  return request(options);
}

export function renameFolder(folderId, title) {
  const data = { title };
  const options = {
    method: "put",
    url: `/files/folder/${folderId}`,
    data
  };

  return request(options);
}

export function deleteFolder(folderId, deleteAfter, immediately) {
  const data = { deleteAfter, immediately };
  const options = {
    method: "delete",
    url: `/files/folder/${folderId}`,
    data
  };

  return request(options);
}

export function createFile(folderId, title) {
  const data = { title };
  const options = {
    method: "post",
    url: `/files/${folderId}/file`,
    data
  };

  return request(options);
}

export function createTextFile(folderId, title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: `/files/${folderId}/text`,
    data
  };

  return request(options);
}

export function createTextFileInMy(title) {
  const data = { title };
  const options = {
    method: "post",
    url: "/files/@my/file",
    data
  };

  return request(options);
}

export function createTextFileInCommon(title) {
  const data = { title };
  const options = {
    method: "post",
    url: "/files/@common/file",
    data
  };

  return request(options);
}

export function createHtmlFile(folderId, title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: `/files/${folderId}/html`,
    data
  };

  return request(options);
}

export function createHtmlFileInMy(title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: "/files/@my/html",
    data
  };

  return request(options);
}

export function createHtmlFileInCommon(title, content) {
  const data = { title, content };
  const options = {
    method: "post",
    url: "/files/@common/html",
    data
  };

  return request(options);
}

export function getFileInfo(fileId) {
  const options = {
    method: "get",
    url: `/files/file/${fileId}`
  };

  return request(options);
}

export function updateFile(fileId, title, lastVersion) {
  const data = { title, lastVersion };
  const options = {
    method: "put",
    url: `/files/file/${fileId}`,
    data
  };

  return request(options);
}

export function deleteFile(fileId, deleteAfter, immediately) {
  const data = { deleteAfter, immediately };
  const options = {
    method: "delete",
    url: `/files/file/${fileId}`,
    data
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

export function getShareFolders(folderId) {
  return request({
    method: "get",
    url: `/files/folder/${folderId}/share`
  });
}

export function getShareFiles(fileId) {
  return request({
    method: "get",
    url: `/files/file/${fileId}/share`
  });
}

export function setShareFolder(folderId, share, notify, sharingMessage) {
  const data = { share, notify, sharingMessage };
  return request({ method: "put", url: `/files/folder/${folderId}/share`, data });
}

export function setShareFiles(fileId, share, notify, sharingMessage) {
  const data = { share, notify, sharingMessage };
  return request({ method: "put", url: `/files/file/${fileId}/share`, data });
}

export function startUploadSession(folderId, fileName, fileSize, relativePath) {
  const data = { fileName, fileSize, relativePath };
  return request({ method: "post", url: `/files/${folderId}/upload/create_session.json`, data });
}

export function uploadFile(url, data) {
  return axios.post(url, data);
}

export function downloadFiles(fileIds, folderIds) {
  const data = { fileIds, folderIds };
  return request({ method: "put", url: "/files/fileops/bulkdownload", data });
}

export function downloadFormatFiles(fileConvertIds, folderIds) {
  const data = { folderIds, fileConvertIds };
  return request({ method: "put", url: "/files/fileops/bulkdownload", data });
}

export function getProgress() {
  return request({ method: "get", url: "/files/fileops" });
}

export function copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) {
  const data = { destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter };
  return request({ method: "put", url: "/files/fileops/copy", data });
}

export function moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) {
  const data = { destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter };
  return request({ method: "put", url: "/files/fileops/move", data });
}
