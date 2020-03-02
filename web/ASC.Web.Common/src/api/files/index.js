import { request } from "../client";
//import axios from "axios";
import Filter from "./filter";
import * as fakeFiles from "./fake";

export function getFolder(folderId, filter = Filter.getDefault(), fake = false) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "Fake folder");
  }

  return request({
    method: "get",
    url: `/files/${folderId}.json`
  });
}

export function getFolderInfo(folderId) {

  return request({
    method: "get",
    url: `/files/folder/${folderId}`
  });
}

export function getFolderPath(folderId) {

  return request({
    method: "get",
    url: `/files/folder/${folderId}/path`
  });
}

export function getMyFolderList(filter = Filter.getDefault(), fake = false) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "My Documents");
  }

  return request({
    method: "get",
    url: `/files/@my.json`
  });
}

export function getCommonFolderList(filter = Filter.getDefault(), fake = false) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "Common Documents");
  }

  return request({
    method: "get",
    url: `/files/@common.json`
  });
}

export function getProjectsFolderList(filter = Filter.getDefault(), fake = false) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "Project Documents");
  }

  return request({
    method: "get",
    url: `/files/@projects.json`
  });
}

export function getTrashFolderList(filter = Filter.getDefault(), fake = false) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "Recycle Bin");
  }

  return request({
    method: "get",
    url: `/files/@trash.json`
  });
}

export function getSharedFolderList(filter = Filter.getDefault(), fake = true) {

  if (fake) {
    return fakeFiles.getFakeElements(filter, "Shared with Me");
  }

  return request({
    method: "get",
    url: `/files/@share.json`
  });
}

export function createTextFileInMy(title) {
  const options = {
    method: "post",
    url: "/files/@my/file",
    title: title
  };

  return request(options);
}

export function createTextFileInCommon(title) {
  const options = {
    method: "post",
    url: "/files/@common/file",
    title: title
  };

  return request(options);
}

export function createTextFile(folderId, title) {
  const options = {
    method: "post",
    url: `/files/${folderId}/file`,
    title: title
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

export function updateFile(fileId, title /*, lastVersion */) {
  const options = {
    method: "put",
    url: `/files/file/${fileId}`,
    title: title,
    /* lastVersion: lastVersion */ //TODO: not found file with last version
  };

  return request(options);
}

export function createFolder(folderId, title) {
  const options = {
    method: "post",
    url: `/files/folder/${folderId}`,
    title: title
  };

  return request(options);
}

export function renameFolder(folderId, title) {
  const options = {
    method: "put",
    url: `/files/folder/${folderId}`,
    title: title
  };

  return request(options);
}

//TODO: must work after add operations

/* export function deleteFolder(folderId, deleteAfter, immediately) {
  const options = {
    method: "delete",
    url: `/files/folder/${folderId}`,
    deleteAfter: deleteAfter,
    immediately: immediately
  };

  return request(options);
}

export function deleteFile(folderId, deleteAfter, immediately) {
  const options = {
    method: "delete",
    url: `/files/file/${folderId}`,
    deleteAfter: deleteAfter,
    immediately: immediately
  };

  return request(options);
} */