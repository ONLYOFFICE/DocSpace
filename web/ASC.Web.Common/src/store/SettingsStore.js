import { makeAutoObservable } from "mobx";
import api from "../api";
import { LANGUAGE } from "../constants";

const desktop = window["AscDesktopEditor"] !== undefined;
const desktopEncryption =
  desktop && typeof window.AscDesktopEditor.cloudCryptoCommand === "function";
const lang = localStorage["language"]
  ? localStorage
      .getItem("language")
      .split("-")
      .find((el) => el[0])
  : "en";

class SettingsStore {
  settings = {
    currentProductId: "",
    culture: "en-US",
    cultures: [],
    trustedDomains: [],
    trustedDomainsType: 1,
    timezone: "UTC",
    timezones: [],
    utcOffset: "00:00:00",
    utcHoursOffset: 0,
    defaultPage: "/products/files",
    homepage: "", //config.homepage,
    datePattern: "M/d/yyyy",
    datePatternJQ: "00/00/0000",
    dateTimePattern: "dddd, MMMM d, yyyy h:mm:ss tt",
    datepicker: {
      datePattern: "mm/dd/yy",
      dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
      timePattern: "h:mm tt",
    },
    organizationName: "ONLYOFFICE",
    greetingSettings: "Web Office Applications",
    enableAdmMess: false,
    urlLicense: "https://gnu.org/licenses/gpl-3.0.html",
    urlSupport: "https://helpdesk.onlyoffice.com/",
    urlAuthKeys: `https://helpcenter.onlyoffice.com/${lang}/installation/groups-authorization-keys.aspx`,
    logoUrl: "",
    customNames: {
      id: "Common",
      userCaption: "User",
      usersCaption: "Users",
      groupCaption: "Group",
      groupsCaption: "Groups",
      userPostCaption: "Title",
      regDateCaption: "Registration Date",
      groupHeadCaption: "Head",
      guestCaption: "Guest",
      guestsCaption: "Guests",
    },
    isDesktopClient: desktop,
    //isDesktopEncryption: desktopEncryption,
    isEncryptionSupport: false,
    encryptionKeys: null,
    isTabletView: false,
  };

  constructor() {
    makeAutoObservable(this);
  }

  getSettings() {
    return api.settings.getSettings();
  }

  getCurrentCustomSchema(id) {
    return api.settings.getCurrentCustomSchema(id);
  }

  async getPortalSettings() {
    const settingsData = await this.getSettings();
    let customNames = { ...this.settings.customNames };
    const { passwordHash: hashSettings, ...otherSettings } = settingsData;
    const logoSettings = { logoUrl: "images/nav.logo.opened.react.svg" };

    const settings = hashSettings
      ? { ...logoSettings, ...otherSettings, hashSettings }
      : { ...logoSettings, ...otherSettings };

    if (!localStorage.getItem(LANGUAGE)) {
      localStorage.setItem(LANGUAGE, settings.culture);
    }

    if (otherSettings.nameSchemaId) {
      customNames = await this.getCurrentCustomSchema(
        otherSettings.nameSchemaId
      );
    }

    settings.customNames = customNames;
    this.settings = { ...this.settings, ...settings };
  }
}

export default SettingsStore;
