
import { request } from "../client";
//import axios from "axios";
import Filter from "./filter";

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

  export function getUser(userId="") {
    return request({
      method: "get",
      url: `/people/${userId || '@self.json'}`
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