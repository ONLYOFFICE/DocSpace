import axios from "axios";
import * as fakeApi from "./fakeApi";

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

export function getUser() {
  return IS_FAKE
    ? fakeApi.getUser()
    : axios.get(`${API_URL}/people/@self.json`);
}

export function getSettings() {
  return IS_FAKE
    ? fakeApi.getSettings()
    : axios.get(`${API_URL}/settings.json`);
}

export function getPasswordSettings(key) {
  return IS_FAKE
    ? fakeApi.getPasswordSettings()
    : axios.get(`${API_URL}/settings/security/password`, {
        headers: { confirm: key }
      });
}

export function createUser(data, key) {
  return IS_FAKE
    ? fakeApi.createUser()
    : axios.post(`${API_URL}/people`, data, { headers: { confirm: key } });
}

export function validateActivatingEmail(data, key) {
  return fakeApi.validateActivatingEmail(data, key);
}

export function validateConfirmLink(link) {
  return fakeApi.validateConfirmLink(link);
}

export function changePassword(userId, password, key) {
  const IS_FAKE = true;
  return IS_FAKE
    ? fakeApi.changePassword()
    : axios.put(`${API_URL}/${userId}/password`, {password}, {
        headers: { confirm: key }
    });
}
