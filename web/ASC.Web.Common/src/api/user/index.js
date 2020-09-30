import { request, setAuthorizationToken } from "../client";

export function login(userName, hash) {
  const data = {
    userName,
    passwordHash: hash
  };

  return request({
    method: "post",
    url: "/authentication.json",
    data
  }).then(tokenData => {
    setAuthorizationToken(true);
    return Promise.resolve(tokenData);
  });
}

export function logout() {
  return request({
    method: "post",
    url: "/authentication/logout"
  }).then(() => {
    setAuthorizationToken();
    return Promise.resolve();
  });
}

export function checkConfirmLink(data) {
  return request({
    method: "post",
    url: "/authentication/confirm.json",
    data
  });
}
