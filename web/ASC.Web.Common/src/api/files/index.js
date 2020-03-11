import { request } from "../client";
//import axios from "axios";
import Filter from "./filter";
import * as fakeFiles from "./fake";

export function getFolder(folderId, filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Fake folder");
  }

  const options = {
    method: "get",
    url: `/files/${folderId}`
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

export function getMyFolderList(filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "My Documents");
  }

  const options = {
    method: "get",
    url: `/files/@my`
  };
  
  return request(options);
}

export function getCommonFolderList(filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Common Documents");
  }

  const options = {
    method: "get",
    url: `/files/@common`
  };
  
  return request(options);
}

export function getProjectsFolderList(filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Project Documents");
  }
  
  const options = {
    method: "get",
    url: `/files/@projects`
  };
  
  return request(options);
}

export function getTrashFolderList(filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Recycle Bin");
  }

  const options = {
    method: "get",
    url: `/files/@trash`
  };
  
  return request(options);
}

export function getSharedFolderList(filter = Filter.getDefault(), fake = false) {
  if (fake) {
    return fakeFiles.getFakeElements(filter, "Shared with Me");
  }

  const options = {
    method: "get",
    url: `/files/@share`
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

export function createTextFile(folderId, title) {
  const data = { title };
  const options = {
    method: "post",
    url: `/files/${folderId}/file`,
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

export function deleteFile(fileId, deleteAfter, immediately) {
  const data = { deleteAfter, immediately };
  const options = {
    method: "delete",
    url: `/files/file/${fileId}`,
    data
  };

  return request(options);
}