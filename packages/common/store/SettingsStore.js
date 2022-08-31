import { makeAutoObservable } from "mobx";
import api from "../api";
import { LANGUAGE, TenantStatus } from "../constants";
import { combineUrl } from "../utils";
import FirebaseHelper from "../utils/firebase";
import { AppServerConfig, ThemeKeys } from "../constants";
import { version } from "../package.json";
import SocketIOHelper from "../utils/socket";

import { Dark, Base } from "@docspace/components/themes";
import { initPluginStore } from "../../client/src/helpers/plugins";

const { proxyURL } = AppServerConfig;

const themes = {
  Dark: Dark,
  Base: Base,
};

const isDesktopEditors = window["AscDesktopEditor"] !== undefined;

class SettingsStore {
  isLoading = false;
  isLoaded = false;
  isBurgerLoading = false;

  checkedMaintenance = false;
  maintenanceExist = false;
  snackbarExist = false;
  currentProductId = "";
  culture = "en";
  cultures = [];
  theme = isDesktopEditors
    ? window.RendererProcessVariable?.theme?.type === "dark"
      ? Dark
      : Base
    : Base;
  trustedDomains = [];
  trustedDomainsType = 0;
  ipRestrictionEnable = false;
  ipRestrictions = [];
  sessionLifetime = "1440";
  timezone = "UTC";
  timezones = [];
  tenantAlias = "";
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
  urlOforms = "https://cmsoforms.onlyoffice.com/api/oforms";

  logoUrl = combineUrl(proxyURL, "/static/images/logo.docspace.react.svg");
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
  isDesktopClient = isDesktopEditors;
  //isDesktopEncryption: desktopEncryption;
  isEncryptionSupport = false;
  encryptionKeys = null;

  personal = false;
  docSpace = true;

  roomsMode = false;

  isHeaderVisible = false;
  isTabletView = false;

  showText = false;
  articleOpen = false;
  isMobileArticle = false;

  folderPath = [];

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
  buildVersionInfo = {
    appServer: version,
    documentServer: "6.4.1",
  };
  debugInfo = false;
  socketUrl = "";

  userFormValidation = /^[\p{L}\p{M}'\-]+$/gu;
  folderFormValidation = new RegExp('[*+:"<>?|\\\\/]', "gim");

  tenantStatus = null;
  helpLink = null;
  hotkeyPanelVisible = false;
  frameConfig = null;


  appearanceTheme = [];
  selectedThemeId = null;
  currentColorScheme = null;

  enablePlugins = false;
  pluginOptions = [];


  constructor() {
    makeAutoObservable(this);
  }

  setTenantStatus = (tenantStatus) => {
    this.tenantStatus = tenantStatus;
  };

  get urlAuthKeys() {
    return `${this.helpLink}/installation/groups-authorization-keys.aspx`;
  }

  get wizardCompleted() {
    return this.isLoaded && !this.wizardToken;
  }

  get helpUrlCommonSettings() {
    return `${this.helpLink}/administration/configuration.aspx#CustomizingPortal_block`;
  }

  get helpUrlCreatingBackup() {
    return `${this.helpLink}/administration/configuration.aspx#CreatingBackup_block`;
  }

  setValue = (key, value) => {
    this[key] = value;
  };

  setCheckedMaintenance = (checkedMaintenance) => {
    this.checkedMaintenance = checkedMaintenance;
  };

  setMaintenanceExist = (maintenanceExist) => {
    this.maintenanceExist = maintenanceExist;
  };

  setSnackbarExist = (snackbar) => {
    this.snackbarExist = snackbar;
  };

  setDefaultPage = (defaultPage) => {
    this.defaultPage = defaultPage;
  };

  getSettings = async () => {
    let newSettings = null;

    if (window?.__ASC_INITIAL_EDITOR_STATE__?.portalSettings)
      newSettings = window.__ASC_INITIAL_EDITOR_STATE__.portalSettings;
    else newSettings = await api.settings.getSettings();

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
        // if (key === "personal") {
        //   window.AppServer = {
        //     ...window.AppServer,
        //     personal: newSettings[key],
        //   };
        // }
      } else if (key === "passwordHash") {
        this.setValue("hashSettings", newSettings[key]);
      }
    });

    return newSettings;
  };

  getFolderPath = async (id) => {
    this.folderPath = await api.files.getFolderPath(id);
  };

  getCurrentCustomSchema = async (id) => {
    let customNames = null;
    if (window?.__ASC_INITIAL_EDITOR_STATE__?.customNames) {
      customNames = window.__ASC_INITIAL_EDITOR_STATE__.customNames;
      window.__ASC_INITIAL_EDITOR_STATE__.customNames = null;
    } else customNames = await api.settings.getCurrentCustomSchema(id);
    this.customNames = customNames;
  };

  getCustomSchemaList = async () => {
    this.customSchemaList = await api.settings.getCustomSchemaList();
  };

  getPortalSettings = async () => {
    const origSettings = await this.getSettings();

    if (origSettings?.plugins?.enabled) {
      initPluginStore();

      this.enablePlugins = origSettings.plugins.enabled;
      this.pluginOptions = origSettings.plugins.allow;
    }

    if (
      origSettings.nameSchemaId &&
      this.tenantStatus !== TenantStatus.PortalRestore
    ) {
      this.getCurrentCustomSchema(origSettings.nameSchemaId);
    }

    if (origSettings.tenantAlias) {
      this.setTenantAlias(origSettings.tenantAlias);
    }
  };

  init = async () => {
    this.setIsLoading(true);
    const requests = [];

    requests.push(this.getPortalSettings(), this.getAppearanceTheme());

    this.tenantStatus !== TenantStatus.PortalRestore &&
      requests.push(this.getBuildVersionInfo());

    await Promise.all(requests);

    this.setIsLoading(false);
    this.setIsLoaded(true);
  };

  setRoomsMode = (mode) => {
    this.roomsMode = mode;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setCultures = (cultures) => {
    this.cultures = cultures;
  };

  getPortalCultures = async () => {
    const cultures = await api.settings.getPortalCultures();
    this.setCultures(cultures);
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
        } catch (e) {
          clearInterval(interval);
          reject(e);
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

  setPortalPasswordSettings = async (
    minLength,
    upperCase,
    digits,
    specSymbols
  ) => {
    const settings = await api.settings.setPortalPasswordSettings(
      minLength,
      upperCase,
      digits,
      specSymbols
    );
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

  setShowText = (showText) => {
    this.showText = showText;
  };

  toggleShowText = () => {
    this.showText = !this.showText;
  };

  setArticleOpen = (articleOpen) => {
    this.articleOpen = articleOpen;
  };

  toggleArticleOpen = () => {
    this.articleOpen = !this.articleOpen;
  };

  setIsMobileArticle = (isMobileArticle) => {
    this.isMobileArticle = isMobileArticle;
  };

  get firebaseHelper() {
    window.firebaseHelper = new FirebaseHelper(this.firebase);
    return window.firebaseHelper;
  }

  get socketHelper() {
    return new SocketIOHelper(this.socketUrl);
  }

  getBuildVersionInfo = async () => {
    let versionInfo = null;
    if (window?.__ASC_INITIAL_EDITOR_STATE__?.versionInfo)
      versionInfo = window.__ASC_INITIAL_EDITOR_STATE__.versionInfo;
    else versionInfo = await api.settings.getBuildVersion();
    this.setBuildVersionInfo(versionInfo);
  };

  setBuildVersionInfo = (versionInfo) => {
    this.buildVersionInfo = {
      ...this.buildVersionInfo,
      appServer: version,
      ...versionInfo,
    };

    if (!this.buildVersionInfo.documentServer)
      this.buildVersionInfo.documentServer = "6.4.1";
  };

  setTheme = (key) => {
    let theme = null;
    switch (key) {
      case ThemeKeys.Base:
      case ThemeKeys.BaseStr:
        theme = ThemeKeys.BaseStr;
        break;
      case ThemeKeys.Dark:
      case ThemeKeys.DarkStr:
        theme = ThemeKeys.DarkStr;
        break;
      case ThemeKeys.System:
      case ThemeKeys.SystemStr:
      default:
        theme =
          window.matchMedia &&
          window.matchMedia("(prefers-color-scheme: dark)").matches
            ? ThemeKeys.DarkStr
            : ThemeKeys.BaseStr;
    }

    this.theme = themes[theme];
  };

  setMailDomainSettings = async (data) => {
    const res = await api.settings.setMailDomainSettings(data);
    this.trustedDomainsType = data.type;
    this.trustedDomains = data.domains;
    return res;
  };

  setTenantAlias = (tenantAlias) => {
    this.tenantAlias = tenantAlias;
  };

  getIpRestrictions = async () => {
    const res = await api.settings.getIpRestrictions();
    this.ipRestrictions = res?.map((el) => el.ip);
  };

  setIpRestrictions = async (ips) => {
    const data = {
      ips: ips,
    };
    const res = await api.settings.setIpRestrictions(data);
    this.ipRestrictions = res;
  };

  getIpRestrictionsEnable = async () => {
    const res = await api.settings.getIpRestrictionsEnable();
    this.ipRestrictionEnable = res.enable;
  };

  setIpRestrictionsEnable = async (enable) => {
    const data = {
      enable: enable,
    };
    const res = await api.settings.setIpRestrictionsEnable(data);
    this.ipRestrictionEnable = res.enable;
  };

  setMessageSettings = async (turnOn) => {
    await api.settings.setMessageSettings(turnOn);
    this.enableAdmMess = turnOn;
  };

  getSessionLifetime = async () => {
    const res = await api.settings.getCookieSettings();
    this.sessionLifetime = res;
  };

  setSessionLifetimeSettings = async (lifeTime) => {
    const res = await api.settings.setCookieSettings(lifeTime);
    this.sessionLifetime = lifeTime;
  };

  setIsBurgerLoading = (isBurgerLoading) => {
    this.isBurgerLoading = isBurgerLoading;
  };

  setHotkeyPanelVisible = (hotkeyPanelVisible) => {
    this.hotkeyPanelVisible = hotkeyPanelVisible;
  };

  setFrameConfig = (frameConfig) => {
    this.frameConfig = frameConfig;
    return frameConfig;
  };

  get isFrame() {
    return this.frameConfig?.name === window.name;
  }

  setAppearanceTheme = (theme) => {
    this.appearanceTheme = theme;
  };

  setSelectThemeId = (selected) => {
    this.selectedThemeId = selected;
  };

  setCurrentColorScheme = (currentColorScheme) => {
    this.currentColorScheme = currentColorScheme;
  };

  getAppearanceTheme = async () => {
    const res = await api.settings.getAppearanceTheme();

    const currentColorScheme = res.themes.find((theme) => {
      return res.selected === theme.id;
    });

    this.setAppearanceTheme(res.themes);
    this.setSelectThemeId(res.selected);
    this.setCurrentColorScheme(currentColorScheme);
  };

  sendAppearanceTheme = async (data) => {
    const res = await api.settings.sendAppearanceTheme(data);
  };
}

export default SettingsStore;
