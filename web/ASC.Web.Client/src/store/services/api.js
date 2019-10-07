import { request, setAuthorizationToken } from "./client";
import axios from "axios";

export function login(userName, password) {
  const data = {
    userName,
    password
  };

  return request({
    method: "post",
    url: "/authentication.json",
    data
  }).then(tokenData => {
    setAuthorizationToken(tokenData.token);
    return Promise.resolve(tokenData);
  });
}

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

export function getUser() {
  return request({
    method: "get",
    url: "/people/@self.json"
  });
}

export function getSettings() {
  return request({
    method: "get",
    url: "/settings.json"
  });
}

export function getPasswordSettings(key) {
  /*return IS_FAKE
    ? fakeApi.getPasswordSettings()
    : axios.get(`${API_URL}/settings/security/password`, {
      headers: { confirm: key }
    });*/

  return request({
    method: "get",
    url: "/settings/security/password",
    headers: {
      confirm: key
    }
  });
}

export function createUser(data, key) {
  /*return IS_FAKE
    ? fakeApi.createUser()
    : axios.post(`${API_URL}/people`, data, { headers: { confirm: key } });*/
  return request({
    method: "post",
    url: "/people",
    data: data,
    headers: { confirm: key }
  });
}

export function changePassword(userId, password, key) {
  /* return IS_FAKE
    ? fakeApi.changePassword()
    : axios.put(`${API_URL}/people/${userId}/password`, password, {
      headers: { confirm: key }
    });*/

    const data = { password };

  return request({
    method: "put",
    url: `/people/${userId}/password`,
    data,
    headers: { confirm: key }
  });
}

export function changeEmail(userId, email, key) {
  /* return IS_FAKE
    ? fakeApi.changePassword()
    : axios.put(`${API_URL}/people/${userId}/password`, password, {
      headers: { confirm: key }
    });*/

    const data = { email };

  return request({
    method: "put",
    url: `/people/${userId}/password`,
    data,
    headers: { confirm: key }
  });
}
export function updateActivationStatus(activationStatus, userId, key) {
  /*return IS_FAKE
    ? fakeApi.updateActivationStatus()
    : axios.put(`${API_URL}/people/activationstatus/${activationStatus}.json`, { userIds: [userId] }, {
      headers: { confirm: key }
    });*/

  return request({
    method: "put",
    url: `/people/activationstatus/${activationStatus}.json`,
    data: { userIds: [userId] },
    headers: { confirm: key }
  });
}

export function updateUser(data) {
  /*return IS_FAKE
    ? fakeApi.updateUser()
    : axios.put(`${API_URL}/people/${data.id}`, data);*/
  return request({
    method: "put",
    url: `/people/${data.id}`,
    data
  });
}

export function checkConfirmLink(data) {
  /*return IS_FAKE
    ? fakeApi.checkConfirmLink()
    : axios.post(`${API_URL}/authentication/confirm.json`, data);*/
  return request({
    method: "post",
    url: "/authentication/confirm.json",
    data
  });
}

export function deleteSelf(key) {
  /*return IS_FAKE
    ? fakeApi.deleteUser(key)
    : axios.delete(`${API_URL}/people/@self`,  {
      headers: { confirm: key }
    });*/
  return request({
    method: "delete",
    url: "/people/@self",
    headers: { confirm: key }
  });
}
export function sendInstructionsToChangePassword(email) {
  /*return IS_FAKE
    ? fakeApi.sendInstructionsToChangePassword()
    : axios.post(`${API_URL}/people/password.json`, { email });*/
  return request({
    method: "post",
    url: "/people/password.json",
    data: { email }
  });
}
