import AxiosClient from "../utils/axiosClient";

const client = new AxiosClient();

export const initSSR = (headers) => {
  client.initSSR(headers);
};

export const request = (options) => {
  return client.request(options);
};

export const setWithCredentialsStatus = (state) => {
  return client.setWithCredentialsStatus(state);
};
