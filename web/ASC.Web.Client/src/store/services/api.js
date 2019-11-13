import { request, setAuthorizationToken } from "./client";
import axios from "axios";
import Filter from "../settings/filter";

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

export function logout() {
  return request({
    method: "post",
    url: "/authentication/logout"
  }).then((data) => {
    setAuthorizationToken();
    return Promise.resolve();
  });
}

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules"
  }).then(modules => {
    return modules && axios.all(
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
  return request({
    method: "get",
    url: "/settings/security/password",
    headers: {
      confirm: key
    }
  });
}

export function createUser(data, key) {
  return request({
    method: "post",
    url: "/people",
    data: data,
    headers: { confirm: key }
  });
}

export function changePassword(userId, password, key) {
  const data = { password };

  return request({
    method: "put",
    url: `/people/${userId}/password`,
    data,
    headers: { confirm: key }
  });
}

export function changeEmail(userId, email, key) {
  const data = { email };

  return request({
    method: "put",
    url: `/people/${userId}/password`,
    data,
    headers: { confirm: key }
  });
}
export function updateActivationStatus(activationStatus, userId, key) {
  return request({
    method: "put",
    url: `/people/activationstatus/${activationStatus}.json`,
    data: { userIds: [userId] },
    headers: { confirm: key }
  });
}

export function updateUser(data) {
  return request({
    method: "put",
    url: `/people/${data.id}`,
    data
  });
}

export function checkConfirmLink(data) {
  return request({
    method: "post",
    url: "/authentication/confirm.json",
    data
  });
}

export function deleteSelf(key) {
  return request({
    method: "delete",
    url: "/people/@self",
    headers: { confirm: key }
  });
}
export function sendInstructionsToChangePassword(email) {
  return request({
    method: "post",
    url: "/people/password.json",
    data: { email }
  });
}

export function getPortalCultures() {
  return request({
    method: "get",
    url: "/settings/cultures.json"
  });
}

export function setLanguageAndTime(lng, timeZoneID) {
  return request({
    method: "put",
    url: "/settings/timeandlanguage.json",
    data: { lng, timeZoneID }
  });
}

export function getPortalTimezones() {
  return request({
    method: "get",
    url: "/settings/timezones.json"
  });
}

export function getUserList() {
  return request({
    method: "get",
    url: `/people`
  });
}

export function getListAdmins(filter = Filter.getDefault()) {
  const filterParams = filter.toUrlParams();
  const params =
    "fields=id,displayName,groups,name,avatar,avatarSmall,isOwner,isAdmin,profileUrl,listAdminModules";
  return request({
    method: "get",
    url: `/people/filter.json?${filterParams}&${params}`
  });
}

export function getAdmins(isParams) {
  let params = "&fields";
  if (isParams) {
    params =
      "fields=id,displayName,groups,name,avatar,avatarSmall,isOwner,isAdmin,profileUrl,listAdminModules";
  }
  return request({
    method: "get",
    url: `/people/filter.json?isadministrator=true&${params}`
  });
}

export function changeProductAdmin(userId, productId, administrator) {
  return request({
    method: "put",
    url: "/settings/security/administrator",
    data: {
      productId,
      userId,
      administrator
    }
  });
}

export function getUserById(userId) {
  return request({
    method: "get",
    url: `/people/${userId}`
  });
}

/* export function getGreetingSettings() {
  return request({
    method: "get",
    url: `/settings/greetingsettings.json`,
  });
} */

export function setGreetingSettings(title) {
  return request({
    method: "post",
    url: `/settings/greetingsettings.json`,
    data: { title }
  });
}

export function restoreGreetingSettings() {
  return request({
    method: "post",
    url: `/settings/greetingsettings/restore.json`
  });
}

export function getLogoText() {
  return request({
    method: "get",
    url: `/settings/whitelabel/logotext.json`
  });
}
export function getLogoSizes() {
  return request({
    method: "get",
    url: `/settings/whitelabel/sizes.json`
  });
}

export function getLogoUrls() {
  return request({
    method: "get",
    url: `/settings/whitelabel/logos.json`
  });
}