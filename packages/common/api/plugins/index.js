import { request } from "../client";

export const getPlugins = async (enabled) => {
  const url = enabled
    ? `/settings/webplugins?enabled=${enabled}`
    : `/settings/webplugins`;

  return request({
    method: "GET",
    url,
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

export const getSystemPlugin = async (name) => {
  return request({
    method: "GET",
    url: `/settings/webplugins/system/${name}`,
  });
};

export const activateSystemPlugin = async (name, enabled) => {
  return request({
    method: "PUT",
    url: `/settings/webplugins/system/${name}`,
    data: { enabled },
  });
};

export const deleteSystemPlugin = async (name) => {
  request({
    method: "DELETE",
    url: `/settings/webplugins/system/${name}`,
  });
};
