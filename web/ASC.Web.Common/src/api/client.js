import axios from "axios";
import { AUTH_KEY } from "../constants";

const PREFIX = "api";
const VERSION = "2.0";
const baseURL = `${window.location.origin}/${PREFIX}/${VERSION}`;

/**
 * @description axios instance for ajax requests
 */

const client = axios.create({
  baseURL: baseURL,
  responseType: "json",
  timeout: 30000 // default is `0` (no timeout)
});

setAuthorizationToken(localStorage.getItem(AUTH_KEY));

client.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    if (error.response.status === 401) {
      setAuthorizationToken();
      window.location.href = "/login/error=unauthorized";
    }

    if (error.response.status === 502) {
      window.location.href = `/error/${error.response.status}`;
    }

    return error;
  }
);

export function setAuthorizationToken(token) {
  client.defaults.withCredentials = true;
  if (token) {
    localStorage.setItem(AUTH_KEY, true);
  } else {
    localStorage.clear();
  }
}

export function setClientBasePath(path) {
  if (!path)
    return;

  client.defaults.baseURL = path;
}

const checkResponseError = res => {
  if(!res) return;

  if (res.data && res.data.error) {
    console.error(res.data.error);
    throw new Error(res.data.error.message);
  }

  if(res.isAxiosError && res.message) {
    console.error(res.message);
    //toastr.error(res.message);
    throw new Error(res.message);
  }
};

/**
 * @description wrapper for making ajax requests
 * @param {object} object with method,url,data etc.
 */
export const request = function(options) {
  const onSuccess = function(response) {
    checkResponseError(response);
    
    if(!response || !response.data || response.isAxiosError)
      return null;

    if(response.data.hasOwnProperty("total"))
      return { total: +response.data.total, items: response.data.response };

    return response.data.response;
  };
  const onError = function(error) {
    console.error("Request Failed:", error.config);
    if (error.response) {
      console.error("Status:", error.response.status);
      console.error("Data:", error.response.data);
      console.error("Headers:", error.response.headers);
    } else {
      console.error("Error Message:", error.message);
    }
    return Promise.reject(error.response || error.message);
  };

  return client(options)
    .then(onSuccess)
    .catch(onError);
};
