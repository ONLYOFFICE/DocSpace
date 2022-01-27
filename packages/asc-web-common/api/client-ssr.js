import axios from "axios";
import { AppServerConfig } from "../constants";
import combineUrl from "../utils/combineUrl";

const { proxyURL, apiPrefixURL, apiTimeout } = AppServerConfig;

const apiBaseURL = combineUrl(process.env.remoteApiUrl, proxyURL, apiPrefixURL);

const client = axios.create({
  baseURL: apiBaseURL,
  responseType: "json",
  timeout: apiTimeout,
});

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
    return res.message;
  }
};

export default function (options) {
  const onSuccess = function (response) {
    const error = getResponseError(response);
    if (error) throw new Error(error);

    if (!response || !response.data || response.isAxiosError) return null;

    if (response.data.hasOwnProperty("total"))
      return { total: +response.data.total, items: response.data.response };

    if (response.request.responseType === "text") return response.data;

    return response.data.response;
  };

  const onError = function (error) {
    //console.error("Request Failed:", error);

    const errorText = error.response
      ? getResponseError(error.response)
      : error.message;
    console.log(errorText);
    // switch (error.response?.status) {
    //   case 401:
    //     if (options.skipUnauthorized) return Promise.resolve();

    //     request({
    //       method: "post",
    //       url: "/authentication/logout",
    //     }).then(() => {
    //       setWithCredentialsStatus(false);
    //       window.location.href = window?.AppServer?.personal ? "/" : loginURL;
    //     });
    //     break;
    //   case 402:
    //     if (!window.location.pathname.includes("payments")) {
    //       window.location.href = paymentsURL;
    //     }
    //     break;
    //   default:
    //     break;
    // }

    return Promise.reject(errorText || error);
  };

  return client(options).then(onSuccess).catch(onError);
}
