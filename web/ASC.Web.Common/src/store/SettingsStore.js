import { action, computed, makeObservable, observable } from "mobx";
import api from "../api";
import { LANGUAGE } from "../constants";

class SettingsStore {
  isLoading = false;
  isLoaded = false;

  currentProductId = "";
  culture = "en-US";
  cultures = [];
  trustedDomains = [];
  trustedDomainsType = 1;
  timezone = "UTC";
  timezones = [];
  utcOffset = "00:00:00";
  utcHoursOffset = 0;
  defaultPage = "/"; //"/products/files";
  homepage = ""; //config.homepage;
  datePattern = "M/d/yyyy";
  datePatternJQ = "00/00/0000";
  dateTimePattern = "dddd, MMMM d, yyyy h:mm:ss tt";
  datepicker = {
    datePattern: "mm/dd/yy",
    dateTimePattern: "DD: mm dd: yy h:mm:ss tt",
    timePattern: "h:mm tt",
  };
  organizationName = "ONLYOFFICE";
  greetingSettings = "Web Office Applications";
  enableAdmMess = false;
  urlLicense = "https://gnu.org/licenses/gpl-3.0.html";
  urlSupport = "https://helpdesk.onlyoffice.com/";
  logoUrl = "images/nav.logo.opened.react.svg";
  customNames = {
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
  };
  isDesktopClient = window["AscDesktopEditor"] !== undefined;
  //isDesktopEncryption: desktopEncryption;
  isEncryptionSupport = false;
  encryptionKeys = null;
  isTabletView = false;
  hashSettings = null;
  title = "";
  ownerId = null;
  nameSchemaId = null;

  constructor() {
    makeObservable(this, {
      currentProductId: observable,
      culture: observable,
      cultures: observable,
      trustedDomains: observable,
      trustedDomainsType: observable,
      timezone: observable,
      timezones: observable,
      utcOffset: observable,
      utcHoursOffset: observable,
      defaultPage: observable,
      homepage: observable,
      datePattern: observable,
      datePatternJQ: observable,
      dateTimePattern: observable,
      datepicker: observable,
      organizationName: observable,
      greetingSettings: observable,
      enableAdmMess: observable,
      urlLicense: observable,
      urlSupport: observable,
      urlAuthKeys: computed,
      logoUrl: observable,
      customNames: observable,
      isDesktopClient: observable,
      isEncryptionSupport: observable,
      encryptionKeys: observable,
      isTabletView: observable,
      hashSettings: observable,
      ownerId: observable,
      nameSchemaId: observable,
      getSettings: action,
      getCurrentCustomSchema: action,
      getPortalSettings: action,
      init: action,
      isLoaded: observable,
      isLoading: observable,
      setIsLoading: action,
      setIsLoaded: action,
    });
  }

  get urlAuthKeys() {
    const splitted = this.culture.split("-");
    const lang = splitted.length > 0 ? splitted[0] : "en";
    return `https://helpcenter.onlyoffice.com/${lang}/installation/groups-authorization-keys.aspx`;
  }

  getSettings = async () => {
    const newSettings = await api.settings.getSettings();

    Object.keys(newSettings).map((key) => {
      if (key in this) {
        this[key] = newSettings[key];

        if (key === "culture" && !localStorage.getItem(LANGUAGE)) {
          localStorage.setItem(LANGUAGE, newSettings[key]);
        }
      } else if (key === "passwordHash") {
        this.hashSettings = newSettings[key];
      }
    });

    return newSettings;
  };

  getCurrentCustomSchema = async (id) => {
    this.customNames = await api.settings.getCurrentCustomSchema(id);
  };

  getPortalSettings = async () => {
    const origSettings = await this.getSettings();

    if (origSettings.nameSchemaId) {
      this.getCurrentCustomSchema(origSettings.nameSchemaId);
    }
  };

  init = async () => {
    this.setIsLoading(true);

    await this.getPortalSettings();

    this.setIsLoading(false);
    this.setIsLoaded(true);
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default SettingsStore;
