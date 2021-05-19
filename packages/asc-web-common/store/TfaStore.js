import { makeAutoObservable } from "mobx";
import api from "../api";
import history from "../history";

class TfaStore {
  tfaSettings = null;

  constructor() {
    makeAutoObservable(this);
  }

  getTfaSettings = async () => {
    const res = await api.settings.getTfaSettings();
    const sms = res[0].enabled;
    const app = res[1].enabled;

    const type = sms ? "sms" : app ? "app" : "none";
    this.tfaSettings = type;

    return type;
  };

  setTfaSettings = async (type) => {
    return await api.settings.setTfaSettings(type);
  };

  getTfaConfirmLink = async (res, type) => {
    if (res && type !== "none") {
      return await api.settings.getTfaConfirmLink();
    }
  };

  getSecretKeyAndQR = async (confirmKey) => {
    return api.settings.getTfaSecretKeyAndQR(confirmKey);
  };

  loginWithCode = async (userName, passwordHash, code) => {
    return api.user.loginWithTfaCode(userName, passwordHash, code);
  };

  getBackupCodes = async () => {
    return api.settings.getTfaNewBackupCodes();
  };

  unlinkApp = async () => {
    return api.settings.unlinkTfaApp();
  };
}

export default TfaStore;
