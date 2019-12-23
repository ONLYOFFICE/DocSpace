
import { request } from "../client";
//import axios from "axios";
import Filter from "./filter";
import * as fakePeople from "./fake";

export function getUserList(filter = Filter.getDefault(), fake = false) {
    if(fake) {
      return fakePeople.getUserList(filter);
    }

    const params =
      filter && filter instanceof Filter
        ? `/filter.json?${filter.toUrlParams()}`
        : "";
  
    return request({
      method: "get",
      url: `/people${params}`
    });
  }

  export function getUser(userName = null) {
    return request({
      method: "get",
      url: `/people/${userName || '@self'}.json`
    });
  }
  export function getUserPhoto(userId) {
    return request({
      method: "get",
      url: `/people/${userId}/photo`
    });
  }

  export function createUser(data, confirmKey = null) {
    const options = {
      method: "post",
      url: "/people",
      data: data
    };

    if(confirmKey)
      options.headers = { confirm: confirmKey };

    return request(options);
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

  export function resendUserInvites(userIds) {
    return request({
      method: "put",
      url: "/people/invite",
      data: { userIds }
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

  export function sendInstructionsToDelete() {
    return request({
      method: "put",
      url: "/people/self/delete.json"
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

  export function getSelectorUserList() {
    return request({
      method: "get",
      url: "/people/filter.json?fields=id,displayName,groups"
    });
  }