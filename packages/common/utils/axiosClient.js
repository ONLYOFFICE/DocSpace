import axios from "axios";
import combineUrl from "./combineUrl";
import defaultConfig from "PUBLIC_DIR/scripts/config.json";

let { api: apiConf, proxy: proxyConf } = defaultConfig;
let { orign: apiOrigin, prefix: apiPrefix, timeout: apiTimeout } = apiConf;
let { url: proxyURL } = proxyConf;

class AxiosClient {
  constructor() {
    if (typeof window !== "undefined") this.initCSR();
  }

  initCSR = () => {
    this.isSSR = false;
    const origin =
      window.DocSpaceConfig?.api?.origin || apiOrigin || window.location.origin;
    const proxy = window.DocSpaceConfig?.proxy?.url || proxyURL;
    const prefix = window.DocSpaceConfig?.api?.prefix || apiPrefix;

    let headers = null;

    if (apiOrigin !== "") {
      headers = {
        "Access-Control-Allow-Credentials": true,
      };
    }

    const apiBaseURL = combineUrl(origin, proxy, prefix);
    const paymentsURL = combineUrl(
      proxy,
      "/portal-settings/payments/portal-payments"
    );
    this.paymentsURL = paymentsURL;

    const apxiosConfig = {
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout, // default is `0` (no timeout)
      withCredentials: true,
    };

    if (headers) {
      apxiosConfig.headers = headers;
    }

    console.log("initCSR", {
      defaultConfig,
      apxiosConfig,
      DocSpaceConfig: window.DocSpaceConfig,
      paymentsURL,
    });

    this.client = axios.create(apxiosConfig);
  };

  initSSR = (headers) => {
    this.isSSR = true;

    const proto = headers["x-forwarded-proto"]?.split(",").shift();
    const host = headers["x-forwarded-host"]?.split(",").shift();

    const origin = apiOrigin || `${proto}://${host}`;

    const apiBaseURL = combineUrl(origin, proxyURL, apiPrefix);

    const axiosConfig = {
      baseURL: apiBaseURL,
      responseType: "json",
      timeout: apiTimeout,
      headers: headers,
    };

    console.log("initSSR", {
      defaultConfig,
      axiosConfig,
    });

    this.client = axios.create(axiosConfig);
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

      if (error?.response?.status === 401 && this.isSSR) {
        error.response.data = {
          ...error?.response?.data,
          error: { ...error?.response?.data?.error, message: 401 },
        };
      }

      const loginURL = combineUrl(proxyURL, "/login");
      if (!this.isSSR) {
        switch (error.response?.status) {
          case 401:
            if (options.skipUnauthorized) return Promise.resolve();
            if (options.skipLogout) return Promise.reject(error);

            this.request({
              method: "post",
              url: "/authentication/logout",
            }).then(() => {
              this.setWithCredentialsStatus(false);
              window.location.href = `${loginURL}?authError=true`;
            });
            break;
          case 402:
            if (!window.location.pathname.includes("payments")) {
              // window.location.href = this.paymentsURL;
            }
            break;
          case 403:
            const pathname = window.location.pathname;
            const isArchived = pathname.indexOf("/rooms/archived") !== -1;

            const isRooms =
              pathname.indexOf("/rooms/shared") !== -1 || isArchived;

            if (!isRooms) return;

            setTimeout(() => {
              window.DocSpace.navigate(isArchived ? "/archived" : "/");
            }, 1000);

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

        return Promise.reject(error);
      }
    };
    return this.client(options).then(onSuccess).catch(onError);
  };
}

export default AxiosClient;
