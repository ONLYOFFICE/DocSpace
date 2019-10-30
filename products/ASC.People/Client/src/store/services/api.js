import { request } from "./client";
import axios from "axios";
import Filter from "../people/filter";

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules"
  }).then(modules => {
    return axios.all(
      modules.map(m =>
        request({
          method: "get",
          url: `${window.location.origin}/${m}`
        })
      )
    );
  });
}

export function getSettings() {
  return request({
    method: "get",
    url: "/settings.json"
  });
}

export function getPortalCultures() {
  return request({
    method: "get",
    url: "/settings/cultures.json"
  });
}

export function getPortalPasswordSettings() {
  return request({
    method: "get",
    url: "/settings/security/password"
  });
}

export function getUser(userId) {
  return request({
    method: "get",
    url: "/people/@self.json"
  });
}

export function getSelectorUserList() {
  return request({
    method: "get",
    url: "/people/filter.json?fields=id,displayName,groups"
  });
}

export function getUserList(filter = Filter.getDefault()) {
  const params =
    filter && filter instanceof Filter
      ? `/filter.json?${filter.toUrlParams()}`
      : "";

  return request({
    method: "get",
    url: `/people${params}`
  });
}

export function getGroupList() {
  return request({
    method: "get",
    url: "/group"
  });
}

export function createUser(data) {
  return request({
    method: "post",
    url: "/people",
    data
  });
}

export function updateUser(data) {
  return request({
    method: "put",
    url: `/people/${data.id}`,
    data
  });
}

export function updateUserCulture(id, cultureName) {
  return request({
    method: "put",
    url: `/people/${id}/culture`,
    data: { cultureName }
  });
}
export function loadAvatar(profileId, data) {
  return request({
    method: "post",
    url: `/people/${profileId}/photo`,
    data
  });
}
export function createThumbnailsAvatar(profileId, data) {
  return request({
    method: "post",
    url: `/people/${profileId}/photo/thumbnails.json`,
    data
  });
}
export function deleteAvatar(profileId) {
  return request({
    method: "delete",
    url: `/people/${profileId}/photo`
  });
}

export function getInitInfo() {
  return axios
    .all([
      getUser(),
      getModulesList(),
      getSettings(),
      getPortalPasswordSettings(),
      getPortalCultures()
    ])
    .then(
      axios.spread((user, modules, settings, passwordSettings, cultures) => {
        const info = {
          user,
          modules,
          settings
        };

        info.settings.passwordSettings = passwordSettings;
        info.settings.cultures = cultures || [];

        return Promise.resolve(info);
      })
    );
}

export function getInvitationLinks() {
  const isGuest = true;
  return axios.all([getInvitationLink(), getInvitationLink(isGuest)]).then(
    axios.spread((userInvitationLinkResp, guestInvitationLinkResp) => {
      const links = {
        inviteLinks: {
          userLink: userInvitationLinkResp,
          guestLink: guestInvitationLinkResp
        }
      };

      return Promise.resolve(links);
    })
  );
}

export function updateUserStatus(status, userIds) {
  return request({
    method: "put",
    url: `/people/status/${status}`,
    data: { userIds }
  });
}

export function updateUserType(type, userIds) {
  return request({
    method: "put",
    url: `/people/type/${type}`,
    data: { userIds }
  });
}

export function resendUserInvites(userIds) {
  return request({
    method: "put",
    url: "/people/invite",
    data: { userIds }
  });
}

export function sendInstructionsToDelete() {
  return request({
    method: "put",
    url: "/people/self/delete.json"
  });
}

export function sendInstructionsToChangePassword(email) {
  return request({
    method: "post",
    url: "/people/password.json",
    data: { email }
  });
}

export function sendInstructionsToChangeEmail(userId, email) {
  return request({
    method: "post",
    url: "/people/email.json",
    data: { userId, email }
  });
}

export function deleteUser(userId) {
  return request({
    method: "delete",
    url: `/people/${userId}.json`
  });
}

export function deleteUsers(userIds) {
  return request({
    method: "put",
    url: "/people/delete.json",
    data: { userIds }
  });
}

export function getGroup(groupId) {
  return request({
    method: "get",
    url: `/group/${groupId}.json`
  });
}

const GUEST_INVITE_LINK = "guestInvitationLink";
const USER_INVITE_LINK = "userInvitationLink";
const INVITE_LINK_TTL = "localStorageLinkTtl";
const LINKS_TTL = 6 * 3600 * 1000;

export function getInvitationLink(isGuest) {
  const curLinksTtl = localStorage.getItem(INVITE_LINK_TTL);
  const now = +new Date();

  if (!curLinksTtl) {
    localStorage.setItem(INVITE_LINK_TTL, now);
  } else if (now - curLinksTtl > LINKS_TTL) {
    localStorage.removeItem(GUEST_INVITE_LINK);
    localStorage.removeItem(USER_INVITE_LINK);
    localStorage.setItem(INVITE_LINK_TTL, now);
  }

  const link = localStorage.getItem(
    isGuest ? GUEST_INVITE_LINK : USER_INVITE_LINK
  );

  return link
    ? Promise.resolve(link)
    : request({
        method: "get",
        url: `/portal/users/invite/${isGuest ? 2 : 1}.json`
      }).then(link => {
        localStorage.setItem(
          isGuest ? GUEST_INVITE_LINK : USER_INVITE_LINK,
          link
        );
        return Promise.resolve(link);
      });
}

export function getShortenedLink(link) {
  return request({
    method: "put",
    url: "/portal/getshortenlink.json",
    data: link
  });
}

export function createGroup(groupName, groupManager, members) {
  const data = { groupName, groupManager, members };
  return request({
    method: "post",
    url: "/group.json",
    data
  });
}

export function updateGroup(id, groupName, groupManager, members) {
  const data = { groupId: id, groupName, groupManager, members };
  return request({
    method: "put",
    url: `/group/${id}.json`,
    data
  });
}

export function deleteGroup(id) {
  return request({
    method: "delete",
    url: `/group/${id}.json`
  });
}
