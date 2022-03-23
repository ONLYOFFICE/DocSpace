import AxiosClient from "../utils/axiosClient";

const client = new AxiosClient();

export const initSSR = (headers) => {
  client.initSSR(headers);
};

export const request = (options) => {
  return client.request(options);
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

    if (response.request.responseType === "text") return response.data;

    return response.data.response;
  };

  const onError = function (error) {
    //console.error("Request Failed:", error);

    const errorText = error.response
      ? getResponseError(error.response)
      : error.message;

    switch (error.response?.status) {
      case 401:
        if (options.skipUnauthorized) return Promise.resolve();
        if (options.skipLogout) return Promise.reject(errorText || error);

        request({
          method: "post",
          url: "/authentication/logout",
        }).then(() => {
          setWithCredentialsStatus(false);
          window.location.href = window?.AppServer?.personal ? "/" : loginURL;
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

    return Promise.reject(errorText || error);
  };

  return client(options).then(onSuccess).catch(onError);
export const setWithCredentialsStatus = (state) => {
  return client.setWithCredentialsStatus(state);
};
