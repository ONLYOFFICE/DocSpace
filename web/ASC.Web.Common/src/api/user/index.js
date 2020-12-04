import { request, setWithCredentialsStatus } from "../client";

export function login(userName, passwordHash) {
  const data = {
    userName,
    passwordHash,
  };

  return request({
    method: "post",
    url: "/authentication.json",
    data,
  }).then((tokenData) => {
    setWithCredentialsStatus(true);
    return Promise.resolve(tokenData);
  });
}

export function logout() {
  return request({
    method: "post",
    url: "/authentication/logout",
  }).then(() => {
    setWithCredentialsStatus(false);
    return Promise.resolve();
  });
}

export function checkConfirmLink(data) {
  return request({
    method: "post",
    url: "/authentication/confirm.json",
    data,
  });
}

export function checkIsAuthenticated() {
  return request({
    method: "get",
    url: "/authentication",
    withCredentials: true,
  }).then((state) => {
    setWithCredentialsStatus(state);
    return Promise.resolve();
  });
}
