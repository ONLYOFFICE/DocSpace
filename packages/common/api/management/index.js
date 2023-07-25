import { request, setWithCredentialsStatus } from "../client";
import axios from "axios";

const baseURL = "/apisystem";

export const deletePortal = (data) => {
  const options = {
    baseURL,
    method: "delete",
    url: `/portal/remove`,
    params: data,
  };

  const res = request(options).then((res) => {
    return res;
  });

  return res;
};

export const getPortalName = async () => {
  const res = await request({
    baseURL,
    method: "get",
    url: `/settings/get?tenantId=-1&key=portalName`,
  });
  return res;
};

export const getDomainName = async () => {
  const options = {
    baseURL,
    method: "get",
    url: `/settings/get?tenantId=-1&key=baseDomain`,
  };
  const res = request(options).then((res) => {
    return res;
  });

  // const res = await axios
  //   .get("/apisystem/settings/get?tenantId=-1&key=baseDomain", {
  //     withCredentials: true,
  //     headers: {
  //       "Access-Control-Allow-Credentials": true,
  //     },
  //   })
  //   .then((res) => res);

  return res;
};

export const setDomainName = async (domainName) => {
  const data = {
    key: "BaseDomain",
    tenantId: -1,
    value: domainName,
  };

  const res = await request({
    baseURL,
    method: "post",
    url: `/settings/save`,
    data,
  });

  return res;
};

export const setPortalName = (portalName) => {
  const data = {
    key: "portalName",
    tenantId: -1,
    value: portalName,
  };

  const res = request({
    baseURL,
    method: "post",
    url: `/settings/save`,
    data,
  });

  return res;
};

export const getPortalStatus = async (portalName) => {
  const data = {
    portalName,
  };

  const res = await request({
    baseURL,
    method: "put",
    url: `/portal/status`,
    data,
  });

  return res;
};

export const createNewPortal = async (data) => {
  const res = await request({
    baseURL,
    method: "post",
    url: `/portal/register`,
    data,
  });

  return res;
};

export const getAllPortals = async () => {
  const res = await request({
    baseURL,
    method: "get",
    url: `/portal/get`,
  });
  return res;
};
