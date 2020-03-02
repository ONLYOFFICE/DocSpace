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

export function createTextFileInMy(title) {
  const options = {
    method: "post",
    url: "/files/@my/file",
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