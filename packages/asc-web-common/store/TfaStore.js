import { makeAutoObservable } from "mobx";
import api from "../api";

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

    return sms ? "sms" : app ? "app" : "none";
  };

  setTfaSettings = async (type) => {
    console.log("setTfaSettings");
    const res = await api.settings.setTfaSettings(type);
    console.log(res);

    if (res && type !== "none") {
      const link = await api.settings.getTfaConfirmLink();
      console.log(link);
      this.tfaSettings = type;
      return link;
    }
  };

  getBackupCodes = async () => {
    console.log("getBackupCodes");
    const backupCodes = await api.settings.getTfaNewBackupCodes();
    console.log(backupCodes);
    this.backupCodes = backupCodes;
  };
}

export default TfaStore;
