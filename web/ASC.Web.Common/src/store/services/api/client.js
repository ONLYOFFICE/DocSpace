import axios from "axios";
import Cookies from "universal-cookie";
import history from "../../../history";
import { AUTH_KEY } from "../../../constants";

const PREFIX = "api";
const VERSION = "2.0";
const baseURL = `${window.location.origin}/${PREFIX}/${VERSION}`;


/**
 * @description axios instance for ajax requests
 */

const client = axios.create({
  baseURL: baseURL,
  responseType: 'json',
  timeout: 30000, // default is `0` (no timeout)
});

setAuthorizationToken(localStorage.getItem(AUTH_KEY));

/**
  * @description if any of the API gets 401 status code, this method 
   calls getAuthToken method to renew accessToken
  * updates the error configuration and retries all failed requests 
  again
*/
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
  const cookies = new Cookies();

  if (token) {
    client.defaults.headers.common['Authorization'] = token;
    localStorage.setItem(AUTH_KEY, token);

    const current = new Date();
    const nextYear = new Date();

    nextYear.setFullYear(current.getFullYear() + 1);

    cookies.set(AUTH_KEY, token, {
      path: "/",
      expires: nextYear
    });
  } else {
    localStorage.clear();
    delete client.defaults.headers.common['Authorization'];
    cookies.remove(AUTH_KEY, {
      path: "/"
    });
  }
}

const checkResponseError = (res) => {
  if (res && res.data && res.data.error) {
      console.error(res.data.error);
      throw new Error(res.data.error.message);
  }
}

/**
 * @description wrapper for making ajax requests
 * @param {object} object with method,url,data etc.
 */
export const request = function(options) {
  const onSuccess = function(response) {
    checkResponseError(response);
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
