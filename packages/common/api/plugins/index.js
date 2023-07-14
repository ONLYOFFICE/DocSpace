import { request } from "../client";

export const getPlugins = async () => {
  return request({
    method: "GET",
    url: `/plugins`,
  });
};

export const activatePlugin = async (id) => {
  return request({
    method: "PUT",
    url: `/plugins/activate/${id}`,
  });
};

export const addPlugin = async (plugin) => {
  return request({
    method: "POST",
    url: `/plugins/add`,
    data: plugin,
  });
};

export const uploadImage = async (id, formData) => {
  return request({
    method: "PUT",
    url: `/plugins/upload/image/${id}`,
    data: formData,
  });
};

export const uploadPlugin = async (id, formData) => {
  return request({
    method: "PUT",
    url: `/plugins/upload/plugin/${id}`,
    data: formData,
  });
};

export const deletePlugin = async (id) => {
  request({
    method: "DELETE",
    url: `/plugins/delete/${id}`,
  });
};
