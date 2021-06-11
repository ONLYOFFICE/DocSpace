import axios from "axios";
import { AppServerConfig } from "../constants";
import { combineUrl } from "../utils";

const { proxyURL, apiPrefixURL, apiTimeout } = AppServerConfig;
const origin = window.location.origin;

const apiBaseURL = combineUrl(origin, proxyURL, apiPrefixURL);
const loginURL = combineUrl(proxyURL, "/login");
const paymentsURL = combineUrl(proxyURL, "/payments");

window.AppServer = {
  origin,
  proxyURL,
  apiPrefixURL,
  apiBaseURL,
  apiTimeout,
  loginURL,
  paymentsURL,
};

/**
 * @description axios instance for ajax requests
 */

const client = axios.create({
  baseURL: apiBaseURL,
  responseType: "json",
  timeout: apiTimeout, // default is `0` (no timeout)
});

client.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (!error.response) return Promise.reject(error);

    switch (true) {
      case error.response.status === 401:
        request({
          method: "post",
          url: "/authentication/logout",
        }).then((res) => {
          setWithCredentialsStatus(false);
          window.location.href = loginURL;
        });
        break;
      case error.response.status === 402:
        if (!window.location.pathname.includes("payments")) {
          window.location.href = paymentsURL;
        }
        break;
      default:
        break;
    }

    return Promise.reject(error);
  }
);

export function setWithCredentialsStatus(state) {
  client.defaults.withCredentials = state;
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
