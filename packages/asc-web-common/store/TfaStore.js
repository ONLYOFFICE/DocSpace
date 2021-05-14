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
    console.log("getTfaSettings");
    const res = await api.settings.getTfaSettings();
    const sms = res[0].enabled;
    const app = res[1].enabled;

    const type = sms ? "sms" : app ? "app" : "none";
    this.tfaSettings = type;

    return type;
  };

  setTfaSettings = async (type) => {
    console.log("setTfaSettings");
    const res = await api.settings.setTfaSettings(type);
    console.log(res);
    this.getTfaConfirmLink(res, type);
  };

  getTfaConfirmLink = async (res, type) => {
    console.log("getTfaConfirmLink");

    if (res && type !== "none") {
      const link = await api.settings.getTfaConfirmLink();
      console.log(link);
      document.location.href = link;
    }
  };

  getSecretKeyAndQR = async (confirmKey) => {
    return api.settings.getTfaSecretKeyAndQR(confirmKey);
  };

  getBackupCodes = async () => {
    console.log("getBackupCodes");
    const backupCodes = await api.settings.getTfaNewBackupCodes();
    console.log(backupCodes);
    this.backupCodes = backupCodes;
  };
}

export default TfaStore;
