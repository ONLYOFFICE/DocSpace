import { request } from "../client";
import { decodeDisplayName } from "../../utils";
import { FolderType } from "../../constants";

export function getRooms(filter) {
  const options = {
    method: "get",
    url: `/files/rooms?${filter.toApiUrlParams()}`,
  };

  return request(options).then((res) => {
    res.files = decodeDisplayName(res.files);
    res.folders = decodeDisplayName(res.folders);

    if (res.current.rootFolderType === FolderType.Archive) {
      res.folders.forEach((room) => (room.isArchive = true));
    }

    return res;
  });
}

export function getRoomInfo(id) {
  const options = {
    method: "get",
    url: `/files/rooms/${id}`,
  };

  return request(options).then((res) => {
    res.files = decodeDisplayName(res.files);
    res.folders = decodeDisplayName(res.folders);
    return res;
  });
}

export function createRoom(data) {
  const options = { method: "post", url: `/files/rooms`, data };

  return request(options).then((res) => {
    return res;
  });
}

export function pinRoom(id) {
  const options = { method: "put", url: `/files/rooms/${id}/pin` };

  return request(options).then((res) => {
    return res;
  });
}

export function unpinRoom(id) {
  const options = { method: "put", url: `/files/rooms/${id}/unpin` };

  return request(options).then((res) => {
    return res;
  });
}

export function deleteRoom(id, deleteAfter = true) {
  const data = { deleteAfter };

  const options = {
    method: "delete",
    url: `/files/rooms/${id}`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function archiveRoom(id, deleteAfter = true) {
  const data = { deleteAfter };

  const options = {
    method: "put",
    url: `/files/rooms/${id}/archive`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function unarchiveRoom(id, deleteAfter = true) {
  const data = { deleteAfter };
  const options = {
    method: "put",
    url: `/files/rooms/${id}/unarchive`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function getTags() {
  const options = {
    method: "get",
    url: "/files/tags",
  };

  return request(options).then((res) => {
    return res;
  });
}
