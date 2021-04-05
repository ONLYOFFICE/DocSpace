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
  });
}

export function logout() {
  return request({
    method: "post",
    url: "/authentication/logout",
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
    return state;
  });
}
