import axios from "axios";
import { AUTH_KEY } from "../constants";
//import history from "../history";

const PREFIX = "api";
const VERSION = "2.0";
const baseURL = `${window.location.origin}/${PREFIX}/${VERSION}`;

/**
 * @description axios instance for ajax requests
 */

const client = axios.create({
  baseURL: baseURL,
  responseType: "json",
  timeout: 30000, // default is `0` (no timeout)
});

setAuthorizationToken(localStorage.getItem(AUTH_KEY));

client.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    switch (true) {
      case error.response.status === 401:
        setAuthorizationToken();
        window.location.href = "/login";
        break;
      case error.response.status === 402:
        if (!window.location.pathname.includes("payments")) {
          window.location.href = "/payments";
        }
        break;
      default:
        break;
    }

    return Promise.reject(error);
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
  if (!path) return;

  client.defaults.baseURL = path;
}

const getResponseError = (res) => {
  if (!res) return;

  if (res.data && res.data.error) {
    return res.data.error.message;
  }

  if (res.isAxiosError && res.message) {
    console.error(res.message);
    return res.message;
  }
};

/**
 * @description wrapper for making ajax requests
 * @param {object} object with method,url,data etc.
 */
export const request = function (options) {
  const onSuccess = function (response) {
    const error = getResponseError(response);
    if (error) throw new Error(error);

    if (!response || !response.data || response.isAxiosError) return null;

    if (response.data.hasOwnProperty("total"))
      return { total: +response.data.total, items: response.data.response };

    return response.data.response;
  };

  const onError = function (errorResponse) {
    console.error("Request Failed:", errorResponse);

    const errorText = errorResponse.response
      ? getResponseError(errorResponse.response)
      : errorResponse.message;

    return Promise.reject(errorText || errorResponse);
  };

  return client(options).then(onSuccess).catch(onError);
};
