import axios from "axios";
import history from "../../history";
import { AUTH_KEY } from "../../helpers/constants.js";

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
      //place your reentry code
      history.push("/login/error=unauthorized");
    }

    if (error.response.status === 502) {
      //toastr.error(error.response);
      history.push(`/error/${error.response.status}`);
    }

    return error;
  }
);

export function setAuthorizationToken(token) {
  if (token) {
    client.defaults.withCredentials = true;
    localStorage.setItem(AUTH_KEY, true);
  } else {
    client.defaults.withCredentials = false;
    localStorage.clear();
  }
}

const checkResponseError = res => {
  if (res && res.data && res.data.error) {
    console.error(res.data.error);
    throw new Error(res.data.error.message);
  }
};

/**
 * @description wrapper for making ajax requests
 * @param {object} object with method,url,data etc.
 */
export const request = function(options) {
  const onSuccess = function(response) {
    checkResponseError(response);
    return response.data
      ? response.data.hasOwnProperty("total")
        ? { total: +response.data.total, items: response.data.response }
        : response.data.response
      : null;
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
