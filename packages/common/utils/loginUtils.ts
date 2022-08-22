import api from "../api";
import { setWithCredentialsStatus } from "../api/client";
export async function login(
  user: string,
  hash: string,
  session = true
): Promise<string | object> {
  try {
    const response = await api.user.login(user, hash, session);

    if (!response || (!response.token && !response.tfa))
      throw response.error.message;

    if (response.tfa && response.confirmUrl) {
      const url = response.confirmUrl.replace(window.location.origin, "");
      return url;
    }

    setWithCredentialsStatus(true);

    // this.reset();

    // this.init();
    // const defaultPage = window["AscDesktopEditor"] !== undefined || IS_PERSONAL ? combineUrl(proxyURL, "/products/files/") : "/"
    // return this.settingsStore.defaultPage;
    return Promise.resolve(response);
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function thirdPartyLogin(SerializedProfile) {
  try {
    const response = await api.user.thirdPartyLogin(SerializedProfile);

    if (!response || !response.token) throw new Error("Empty API response");

    setWithCredentialsStatus(true);

    // this.reset();

    // this.init();

    return Promise.resolve(response);
  } catch (e) {
    return Promise.reject(e);
  }
}
