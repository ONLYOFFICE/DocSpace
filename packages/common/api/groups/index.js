import { request } from "../client";
import * as fakeGroup from "./fake";

export function getGroupList(fake = false, searchValue) {
  const params = searchValue ? `?filtervalue=${searchValue}` : "";

  return fake
    ? fakeGroup.getGroupList()
    : request({
        method: "get",
        url: `/group${params}`,
      }).then((groups) => {
        return groups.sort((a, b) => a.name.localeCompare(b.name));
      });
}

export function getGroup(groupId) {
  return request({
    method: "get",
    url: `/group/${groupId}.json`,
  });
}

export function createGroup(groupName, groupManager, members) {
  const data = { groupName, groupManager, members };
  return request({
    method: "post",
    url: "/group.json",
    data,
  });
}

export function updateGroup(id, groupName, groupManager, members) {
  const data = { groupId: id, groupName, groupManager, members };
  return request({
    method: "put",
    url: `/group/${id}.json`,
    data,
  });
}

export function deleteGroup(id) {
  return request({
    method: "delete",
    url: `/group/${id}.json`,
  });
}

/*export function getGroupListFull() {
  return request({
    method: "get",
    url: "/group/full",
  });
}*/ //TODO: use after fixing problems on the server
