import axios from "axios";
import { AppServerConfig } from "../constants";
import combineUrl from "./combineUrl";

const { proxyURL, apiPrefixURL, apiTimeout } = AppServerConfig;

class AxiosClient {
  constructor() {
    this.isSSR = false;
    this.headers = null;

    if (typeof window !== "undefined") this.CSRConstructor();
  }

  CSRConstructor = () => {
    const { proxyURL, apiPrefixURL, apiTimeout } = AppServerConfig;
    const origin = window.location.origin;

    const apiBaseURL = combineUrl(origin, proxyURL, apiPrefixURL);
    const paymentsURL = combineUrl(proxyURL, "/payments");
    this.loginURL = combineUrl(proxyURL, "/login");

    window.AppServer = {
      ...window.AppServer,
      origin,
      proxyURL,
      apiPrefixURL,
      apiBaseURL,
      apiTimeout,
      paymentsURL,
    };

    this.client = axios.create({
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout, // default is `0` (no timeout)
    });
  };

  SSRConstructor = (headers) => {
    const xRewriterUrl = headers["x-rewriter-url"];
    const apiBaseURL = combineUrl(xRewriterUrl, proxyURL, apiPrefixURL);

    this.client = axios.create({
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout,
      headers: headers,
    });
  };

  initSSR = (headers) => {
    this.isSSR = true;
    this.SSRConstructor(headers);
  };

  setWithCredentialsStatus = (state) => {
    this.client.defaults.withCredentials = state;
  };

  setClientBasePath = (path) => {
    if (!path) return;

    this.client.defaults.baseURL = path;
  };

  getResponseError = (res) => {
    if (!res) return;

    if (res.data && res.data.error) {
      return res.data.error.message;
    }

    if (res.isAxiosError && res.message) {
      //console.error(res.message);
      return res.message;
    }
  };

  request = (options) => {
    const onSuccess = (response) => {
      const error = this.getResponseError(response);
      if (error) throw new Error(error);

      if (!response || !response.data || response.isAxiosError) return null;

      if (response.data.hasOwnProperty("total"))
        return { total: +response.data.total, items: response.data.response };

      if (response.request.responseType === "text") return response.data;

      return response.data.response;
    };

    const onError = (error) => {
      //console.error("Request Failed:", error);

      const errorText = error.response
        ? this.getResponseError(error.response)
        : error.message;

      if (!this.isSSR) {
        switch (error.response?.status) {
          case 401:
            if (options.skipUnauthorized) return Promise.resolve();

            request({
              method: "post",
              url: "/authentication/logout",
            }).then(() => {
              this.setWithCredentialsStatus(false);
              window.location.href = window?.AppServer?.personal
                ? "/"
                : loginURL;
            });
            break;
          case 402:
            if (!window.location.pathname.includes("payments")) {
              window.location.href = paymentsURL;
            }
            break;
          default:
            break;
        }
      } // TODO: adapt for SSR

      return Promise.reject(errorText || error);
    };
    return this.client(options).then(onSuccess).catch(onError);
  };
}

export default AxiosClient;
