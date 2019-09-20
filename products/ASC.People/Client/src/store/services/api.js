import axios from "axios";
import * as fakeApi from "./fakeApi";
import Filter from "../people/filter";

const PREFIX = "api";
const VERSION = "2.0";
const API_URL = `${window.location.origin}/${PREFIX}/${VERSION}`;

const IS_FAKE = false;

export function login(data) {
  return axios.post(`${API_URL}/authentication`, data);
}

export function getModulesList() {
  return IS_FAKE
    ? fakeApi.getModulesList()
    : axios
        .get(`${API_URL}/modules`)
        .then(res => {
          const modules = res.data.response;
          return axios.all(
            modules.map(m => axios.get(`${window.location.origin}/${m}`))
          );
        })
        .then(res => {
          const response = res.map(d => d.data.response);
          return Promise.resolve({ data: { response } });
        });
}

export function getSettings() {
  return IS_FAKE
    ? fakeApi.getSettings()
    : axios.get(`${API_URL}/settings.json`);
}

export function getPortalPasswordSettings() {
  return IS_FAKE
    ? fakeApi.getPortalPasswordSettings()
    : axios.get(`${API_URL}/settings/security/password`);
}

export function getUser(userId) {
  return IS_FAKE
    ? fakeApi.getUser()
    : axios.get(`${API_URL}/people/${userId || "@self"}.json`);
}

export function getUserList(filter = Filter.getDefault()) {
  const params =
    filter && filter instanceof Filter
      ? `/filter.json?${filter.toUrlParams()}`
      : "";
  return IS_FAKE ? fakeApi.getUsers() : axios.get(`${API_URL}/people${params}`);
}

export function getGroupList() {
  return IS_FAKE ? fakeApi.getGroups() : axios.get(`${API_URL}/group`);
}

export function createUser(data) {
  return IS_FAKE ? fakeApi.createUser() : axios.post(`${API_URL}/people`, data);
}

export function updateUser(data) {
  return IS_FAKE
    ? fakeApi.updateUser()
    : axios.put(`${API_URL}/people/${data.id}`, data);
}
export function updateAvatar(profileId, data) {
  return IS_FAKE
    ? fakeApi.updateAvatar()
    : axios.post(`${API_URL}/people/${profileId}/photo/cropped`, data);
}
export function deleteAvatar(profileId) {

  return IS_FAKE
    ? fakeApi.deleteAvatar()
    : axios.delete(`${API_URL}/people/${profileId}/photo`, profileId);
}

export function getInitInfo() {
  return axios.all([getUser(), getModulesList(), getSettings(), getPortalPasswordSettings()]).then(
    axios.spread(function(userResp, modulesResp, settingsResp, passwordSettingsResp) {
      let info = {
        user: userResp.data.response,
        modules: modulesResp.data.response,
        settings: settingsResp.data.response
      };

      info.settings.passwordSettings = passwordSettingsResp.data.response;

      return Promise.resolve(info);
    })
  );
}

export function updateUserStatus(status, userIds) {
  return IS_FAKE
    ? fakeApi.updateUserStatus(status, userIds)
    : axios.put(`${API_URL}/people/status/${status}`, { userIds });
}

export function updateUserType(type, userIds) {
  return IS_FAKE
    ? fakeApi.updateUserType(type, userIds)
    : axios.put(`${API_URL}/people/type/${type}`, { userIds });
}

export function resendUserInvites(userIds) {
  return IS_FAKE
    ? fakeApi.resendUserInvites(userIds)
    : axios.put(`${API_URL}/people/invite`, { userIds });
}

export function sendInstructionsToDelete() {
  return IS_FAKE
    ? fakeApi.sendInstructionsToDelete()
    : axios.put(`${API_URL}/people/self/delete.json`);
}

export function sendInstructionsToChangePassword(email) {
  return IS_FAKE
    ? fakeApi.sendInstructionsToChangePassword()
    : axios.post(`${API_URL}/people/password.json`, { email });
}

export function sendInstructionsToChangeEmail(userId, email) {
  return IS_FAKE
    ? fakeApi.sendInstructionsToChangeEmail()
    : axios.post(`${API_URL}/people/email.json`, { userId, email });
}

export function deleteUser(userId) {
  return IS_FAKE
    ? fakeApi.deleteUser(userId)
    : axios.delete(`${API_URL}/people/${userId}.json`);
}

export function deleteUsers(userIds) {
  return IS_FAKE
    ? fakeApi.deleteUsers(userIds)
    : axios
        .put(`${API_URL}/people/delete.json`, { userIds })
        .then(CheckError);
}

export function getGroup(groupId) {
  return IS_FAKE
    ? fakeApi.getGroup(groupId)
    : axios.get(`${API_URL}/group/${groupId}.json`);
}

export function getInvitationLink(isGuest) {
  return IS_FAKE
    ? fakeApi.getInvitationLink(isGuest)
    : isGuest 
      ? axios.get(`${API_URL}/portal/users/invite/2.json`)
      : axios.get(`${API_URL}/portal/users/invite/1.json`);
}

export function getShortenedLink(link) {
  return IS_FAKE
    ? fakeApi.getShortenedLink(link)
    : axios.put(`${API_URL}/portal/getshortenlink.json`, link);
}

function CheckError(res) {
  if (res.data && res.data.error) {
    const error = res.data.error.message || "Unknown error has happened";
    console.trace(error);
    throw error;
  }
  return Promise.resolve(res);
}

export function createGroup(groupName, groupManager, members) {
  const group = {groupName, groupManager, members};
  return axios.post(`${API_URL}/group.json`, group);
}

export function updateGroup(id, groupName, groupManager, members) {
  const group = {id, groupName, groupManager, members};
  return axios.put(`${API_URL}/group/${id}.json`, group);
}
