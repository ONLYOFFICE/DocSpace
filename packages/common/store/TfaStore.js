import { makeAutoObservable } from "mobx";
import api from "../api";

class TfaStore {
  tfaSettings = null;
  smsAvailable = null;
  appAvailable = null;
  backupCodes = [];
  tfaAndroidAppUrl =
    "https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2";
  tfaIosAppUrl = "https://apps.apple.com/app/google-authenticator/id388497605";
  tfaWinAppUrl =
    "https://www.microsoft.com/ru-ru/p/authenticator/9wzdncrfj3rj?rtc=1&activetab=pivot:overviewtab";

  constructor() {
    makeAutoObservable(this);
  }

  getTfaType = async () => {
    const res = await api.settings.getTfaSettings();
    const sms = res[0].enabled;
    const app = res[1].enabled;

    const type = sms ? "sms" : app ? "app" : "none";
    this.tfaSettings = type;
    this.smsAvailable = res[0].avaliable;
    this.appAvailable = res[1].avaliable;

    return type;
  };

  getTfaSettings = async () => {
    return await api.settings.getTfaSettings();
  };

  setTfaSettings = async (type) => {
    this.tfaSettings = type;
    return await api.settings.setTfaSettings(type);
  };

  setBackupCodes = (codes) => {
    this.backupCodes = codes;
  };

  getTfaConfirmLink = async () => {
    return await api.settings.getTfaConfirmLink();
  };

  getSecretKeyAndQR = async (confirmKey) => {
    return api.settings.getTfaSecretKeyAndQR(confirmKey);
  };

  loginWithCode = async (userName, passwordHash, code) => {
    return api.user.loginWithTfaCode(userName, passwordHash, code);
  };

  loginWithCodeAndCookie = async (code, confirmKey = null) => {
    return api.settings.validateTfaCode(code, confirmKey);
  };

  getBackupCodes = async () => {
    return api.settings.getTfaBackupCodes();
  };

  getNewBackupCodes = async () => {
    return api.settings.getTfaNewBackupCodes();
  };

  unlinkApp = async (id) => {
    return api.settings.unlinkTfaApp(id);
  };
}

export default TfaStore;
