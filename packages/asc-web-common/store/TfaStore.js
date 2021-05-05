import { makeAutoObservable } from "mobx";
import api from "../api";

class TfaStore {
  tfaSettings = null;
  backupCodes = null;

  constructor() {
    makeAutoObservable(this);
  }

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
