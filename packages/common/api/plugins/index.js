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

export const uploadPlugin = async (formData) => {
  return request({
    method: "POST",
    url: `/plugins/upload`,
    data: formData,
  });
};

export const deletePlugin = async (id) => {
  request({
    method: "DELETE",
    url: `/plugins/delete/${id}`,
  });
};
