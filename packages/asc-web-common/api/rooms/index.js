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

    res.folders.forEach((room) => (room.isRoom = !!room.roomType));

    if (res.current.rootFolderType === FolderType.Archive) {
      res.folders.forEach((room) => (room.isArchive = true));
    }

    return res;
  });
}

export function getRoom(id) {
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

export function deleteRoom(id) {
  const options = {
    method: "delete",
    url: `/files/rooms/${id}`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function archiveRoom(id) {
  const options = {
    method: "put",
    url: `/files/rooms/${id}/archive`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function unarchiveRoom(id) {
  const options = {
    method: "put",
    url: `/files/rooms/${id}/unarchive`,
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
