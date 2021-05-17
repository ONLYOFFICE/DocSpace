import { makeAutoObservable } from "mobx";
import api from "../api";
import history from "../history";
import { combineUrl, isAdmin } from "../utils";

class TfaStore {
  tfaSettings = null;
  backupCodes = null;

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
    const res = await api.settings.setTfaSettings(type);
    this.getTfaConfirmLink(res, type);
  };

  getTfaConfirmLink = async (res, type) => {
    if (res && type !== "none") {
      const link = await api.settings.getTfaConfirmLink();
      document.location.href = link;
    }
  };

  getSecretKeyAndQR = async (confirmKey) => {
    return api.settings.getTfaSecretKeyAndQR(confirmKey);
  };

  getBackupCodes = async () => {
    const backupCodes = await api.settings.getTfaNewBackupCodes();
    this.backupCodes = backupCodes;
  };

  loginWithCode = async (userName, passwordHash, code) => {
    return api.user.loginWithTfaCode(userName, passwordHash, code);
  };
}

export default TfaStore;
