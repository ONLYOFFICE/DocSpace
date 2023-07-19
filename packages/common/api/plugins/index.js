import { request } from "../client";

export const getPlugins = async (enabled) => {
  return request({
    method: "GET",
    url: `/settings/webplugins?enabled=${enabled}`,
  });
};

export const addPlugin = async (data) => {
  return request({
    method: "POST",
    url: `/settings/webplugins`,
    data,
  });
};

export const getPlugin = async (id) => {
  return request({
    method: "GET",
    url: `/settings/webplugins/${id}`,
  });
};

export const activatePlugin = async (id, enabled) => {
  console.log(id, enabled);
  return request({
    method: "PUT",
    url: `/settings/webplugins/${id}`,
    data: { enabled },
  });
};

export const deletePlugin = async (id) => {
  request({
    method: "DELETE",
    url: `/settings/webplugins/${id}`,
  });
};
