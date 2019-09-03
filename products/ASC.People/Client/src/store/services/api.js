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

export function getInitInfo() {
  return axios.all([getUser(), getModulesList(), getSettings()]).then(
    axios.spread(function(userResp, modulesResp, settingsResp) {
      return Promise.resolve({
        user: userResp.data.response,
        modules: modulesResp.data.response,
        settings: settingsResp.data.response
      });
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
    ? fakeApi.sendInstructionsToChangePassword(email)
    : axios.post(`${API_URL}/people/password.json`, { email });
}

export function deleteUser(userId) {
  return IS_FAKE
    ? fakeApi.deleteUser(userId)
    : axios.delete(`${API_URL}/people/${userId}.json`);
}

export function deleteUsers(userIds) {
  return IS_FAKE
    ? fakeApi.deleteUsers(userIds)
    : axios.delete(`${API_URL}/people/delete.json`, { userIds });
}