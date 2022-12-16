import axios from "axios";
import { AppServerConfig } from "../constants";
import combineUrl from "./combineUrl";

const { proxyURL, apiOrigin, apiPrefix, apiTimeout } = AppServerConfig;

class AxiosClient {
  constructor() {
    if (typeof window !== "undefined") this.initCSR();
  }

  initCSR = () => {
    this.isSSR = false;
    const origin = apiOrigin || window.location.origin;
    let headers = null;

    if (apiOrigin !== "") {
      headers = {
        "Access-Control-Allow-Credentials": true,
      };
    }

    const apiBaseURL = combineUrl(origin, proxyURL, apiPrefix);
    const paymentsURL = combineUrl(
      proxyURL,
      "/portal-settings/payments/portal-payments"
    );
    this.paymentsURL = paymentsURL;

    // window.AppServer = {
    //   ...window.AppServer,
    //   origin,
    //   proxyURL,
    //   apiPrefix,
    //   apiBaseURL,
    //   apiTimeout,
    //   paymentsURL,
    // };

    const config = {
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout, // default is `0` (no timeout)
      withCredentials: true,
    };

    if (headers) {
      config.headers = headers;
    }

    this.client = axios.create(config);
  };

  initSSR = (headers) => {
    this.isSSR = true;
    const xRewriterUrl = headers["x-rewriter-url"];

    const origin = apiOrigin || xRewriterUrl;

    const apiBaseURL = combineUrl(origin, proxyURL, apiPrefix);

    this.client = axios.create({
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout,
      headers: headers,
    });
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
      console.log("Request Failed:", { error });

      // let errorText = error.response
      //   ? this.getResponseError(error.response)
      //   : error.message;

      if (error?.response?.status === 401 && this.isSSR) errorText = 401;

      const loginURL = combineUrl(proxyURL, "/login");
      if (!this.isSSR) {
        switch (error.response?.status) {
          case 401:
            if (options.skipUnauthorized) return Promise.resolve();
            if (options.skipLogout) return Promise.reject(errorText || error);

            this.request({
              method: "post",
              url: "/authentication/logout",
            }).then(() => {
              this.setWithCredentialsStatus(false);
              window.location.href = loginURL;
            });
            break;
          case 402:
            if (!window.location.pathname.includes("payments")) {
              // window.location.href = this.paymentsURL;
            }
            break;
          default:
            break;
        }

        return Promise.reject(error);
      } else {
        switch (error.response?.status) {
          case 401:
            return Promise.resolve();

          default:
            break;
        }

        return Promise.reject(errorText || error);
      }
    };
    return this.client(options).then(onSuccess).catch(onError);
  };
}

export default AxiosClient;
