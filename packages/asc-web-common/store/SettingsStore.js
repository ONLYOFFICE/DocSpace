import { makeAutoObservable } from "mobx";
import api from "../api";
import { ARTICLE_PINNED_KEY, LANGUAGE } from "../constants";
import { combineUrl } from "../utils";
import FirebaseHelper from "../utils/firebase";
import { AppServerConfig } from "../constants";
const { proxyURL } = AppServerConfig;

class SettingsStore {
  isLoading = false;
  isLoaded = false;

  currentProductId = "";
  culture = "en-US";
  cultures = [];
  trustedDomains = [];
  trustedDomainsType = 0;
  trustedDomains = [];
  timezone = "UTC";
  timezones = [];
  utcOffset = "00:00:00";
  utcHoursOffset = 0;
  defaultPage = "/";
  homepage = "";
  datePattern = "M/d/yyyy";
  datePatternJQ = "00/00/0000";
  dateTimePattern = "dddd, MMMM d, yyyy h:mm:ss tt";
  datepicker = {
    datePattern: "mm/dd/yy",
    dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
    timePattern: "h:mm tt",
  };
  organizationName = "ONLYOFFICE";
  greetingSettings = "Web Office Applications";
  enableAdmMess = false;
  enabledJoin = false;
  urlLicense = "https://gnu.org/licenses/gpl-3.0.html";
  urlSupport = "https://helpdesk.onlyoffice.com/";
  logoUrl = combineUrl(proxyURL, "/static/images/nav.logo.opened.react.svg");
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

  personal = false;

  isHeaderVisible = false;
  isTabletView = false;
  isArticlePinned =
    localStorage.getItem(ARTICLE_PINNED_KEY) === "true" || false;
  isArticleVisible = false;
  isBackdropVisible = false;

  isArticleVisibleOnUnpin = false;

  hashSettings = null;
  title = "";
  ownerId = null;
  nameSchemaId = null;
  owner = {};
  wizardToken = null;
  passwordSettings = null;
  hasShortenService = false;

  customSchemaList = [];
  firebase = {
    apiKey: "",
    authDomain: "",
    projectId: "",
    storageBucket: "",
    messagingSenderId: "",
    appId: "",
    measurementId: "",
  };
  version = "";

  constructor() {
    makeAutoObservable(this);
  }

  get urlAuthKeys() {
    const splitted = this.culture.split("-");
    const lang = splitted.length > 0 ? splitted[0] : "en";
    return `https://helpcenter.onlyoffice.com/${lang}/installation/groups-authorization-keys.aspx`;
  }

  get wizardCompleted() {
    return this.isLoaded && !this.wizardToken;
  }

  get helpUrlCommonSettings() {
    const substring = this.culture.substring(0, this.culture.indexOf("-"));
    const lang = substring.length > 0 ? substring : "en";

    return `https://helpcenter.onlyoffice.com/${lang}/administration/configuration.aspx#CustomizingPortal_block`;
  }

  setIsArticleVisible = (visible) => {
    this.isArticleVisible = this.isArticlePinned ? true : visible;
  };

  setIsBackdropVisible = (visible) => {
    this.isBackdropVisible = visible;
  };

  hideArticle = () => {
    this.setIsArticleVisible(false);
    this.setIsBackdropVisible(false);
  };

  setValue = (key, value) => {
    this[key] = value;
  };

  setDefaultPage = (defaultPage) => {
    this.defaultPage = defaultPage;
  };

  getSettings = async () => {
    const newSettings = await api.settings.getSettings();

    if (window["AscDesktopEditor"] !== undefined || this.personal) {
      const dp = combineUrl(proxyURL, "/products/files/");
      this.setDefaultPage(dp);
    }

    Object.keys(newSettings).map((key) => {
      if (key in this) {
        this.setValue(
          key,
          key === "defaultPage"
            ? combineUrl(proxyURL, newSettings[key])
            : newSettings[key]
        );
        if (key === "culture") {
          const language = localStorage.getItem(LANGUAGE);
          if (!language || language == "undefined") {
            localStorage.setItem(LANGUAGE, newSettings[key]);
          }
        }
      } else if (key === "passwordHash") {
        this.setValue("hashSettings", newSettings[key]);
      }
    });

    return newSettings;
  };

  getCurrentCustomSchema = async (id) => {
    this.customNames = await api.settings.getCurrentCustomSchema(id);
  };

  getCustomSchemaList = async () => {
    this.customSchemaList = await api.settings.getCustomSchemaList();
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

  getPortalCultures = async () => {
    this.cultures = await api.settings.getPortalCultures();
  };

  setIsEncryptionSupport = (isEncryptionSupport) => {
    this.isEncryptionSupport = isEncryptionSupport;
  };

  getIsEncryptionSupport = async () => {
    const isEncryptionSupport = await api.files.getIsEncryptionSupport();
    this.setIsEncryptionSupport(isEncryptionSupport);
  };

  updateEncryptionKeys = (encryptionKeys) => {
    this.encryptionKeys = encryptionKeys ?? {};
  };

  setEncryptionKeys = async (keys) => {
    await api.files.setEncryptionKeys(keys);
    this.updateEncryptionKeys(keys);
  };

  getEncryptionKeys = async () => {
    const encryptionKeys = await api.files.getEncryptionKeys();
    this.updateEncryptionKeys(encryptionKeys);
  };

  getOAuthToken = (tokenGetterWin) => {
    return new Promise((resolve, reject) => {
      localStorage.removeItem("code");
      let interval = null;
      interval = setInterval(() => {
        try {
          const code = localStorage.getItem("code");

          if (code) {
            localStorage.removeItem("code");
            clearInterval(interval);
            resolve(code);
          } else if (tokenGetterWin && tokenGetterWin.closed) {
            clearInterval(interval);
            reject();
          }
        } catch {
          return;
        }
      }, 500);
    });
  };

  getLoginLink = (token, code) => {
    return combineUrl(proxyURL, `/login.ashx?p=${token}&code=${code}`);
  };

  setModuleInfo = (homepage, productId) => {
    if (this.homepage === homepage || this.currentProductId === productId)
      return;

    console.log(`setModuleInfo('${homepage}', '${productId}')`);

    this.homepage = homepage;
    this.setCurrentProductId(productId);

    const baseElm = document.getElementsByTagName("base");
    if (baseElm && baseElm.length === 1) {
      const baseUrl = homepage
        ? homepage[homepage.length - 1] === "/"
          ? homepage
          : `${homepage}/`
        : "/";

      console.log("SET base URL", baseUrl);

      baseElm[0].setAttribute("href", baseUrl);
    }
  };

  setCurrentProductId = (currentProductId) => {
    this.currentProductId = currentProductId;
  };

  getPortalOwner = async () => {
    const owner = await api.people.getUserById(this.ownerId);
    this.owner = owner;
    return owner;
  };

  setWizardComplete = () => {
    this.wizardToken = null;
  };

  setPasswordSettings = (passwordSettings) => {
    this.passwordSettings = passwordSettings;
  };

  getPortalPasswordSettings = async (confirmKey = null) => {
    const settings = await api.settings.getPortalPasswordSettings(confirmKey);
    this.setPasswordSettings(settings);
  };

  setTimezones = (timezones) => {
    this.timezones = timezones;
  };

  getPortalTimezones = async (token = undefined) => {
    const timezones = await api.settings.getPortalTimezones(token);
    this.setTimezones(timezones);
  };

  setHeaderVisible = (isHeaderVisible) => {
    this.isHeaderVisible = isHeaderVisible;
  };

  setIsTabletView = (isTabletView) => {
    this.isTabletView = isTabletView;
  };

  setArticlePinned = (isPinned) => {
    isPinned
      ? localStorage.setItem(ARTICLE_PINNED_KEY, isPinned)
      : localStorage.removeItem(ARTICLE_PINNED_KEY);
    this.isArticlePinned = isPinned;
  };

  setArticleVisibleOnUnpin = (visible) => {
    this.isArticleVisibleOnUnpin = visible;
  };

  get firebaseHelper() {
    return new FirebaseHelper(this.firebase);
  }
}

export default SettingsStore;
