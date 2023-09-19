import axios from "axios";

export function getOforms(url) {
  return axios.get(url);
}

export function submitToGallery(url, file, formName, language) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("formName", formName);
  formData.append("language", language);

  return axios.post(url, formData);
}
