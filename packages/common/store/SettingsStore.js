import { makeAutoObservable } from "mobx";
import api from "../api";
import { combineUrl, setCookie, getCookie } from "../utils";
import FirebaseHelper from "../utils/firebase";
import {
  ThemeKeys,
  COOKIE_EXPIRATION_YEAR,
  LANGUAGE,
  TenantStatus,
} from "../constants";
import { version } from "../package.json";
import SocketIOHelper from "../utils/socket";
import { Dark, Base } from "@docspace/components/themes";
import { initPluginStore } from "../../client/src/helpers/plugins";
import { wrongPortalNameUrl } from "@docspace/common/constants";

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
    : window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
    ? Dark
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

  logoUrl = "";

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
  withPaging = false;

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
    docspace: version,
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

  additionalResourcesData = null;
  additionalResourcesIsDefault = true;
  companyInfoSettingsData = null;
  companyInfoSettingsIsDefault = true;

  whiteLabelLogoUrls = [];
  standalone = false;

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

  setGreetingSettings = (greetingSettings) => {
    this.greetingSettings = greetingSettings;
  };

  getSettings = async (withPassword) => {
    let newSettings = null;

    if (window?.__ASC_INITIAL_EDITOR_STATE__?.portalSettings)
      newSettings = window.__ASC_INITIAL_EDITOR_STATE__.portalSettings;
    else newSettings = await api.settings.getSettings(withPassword);

    if (window["AscDesktopEditor"] !== undefined || this.personal) {
      const dp = combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        "/products/files/"
      );
      this.setDefaultPage(dp);
    }

    Object.keys(newSettings).map((key) => {
      if (key in this) {
        this.setValue(
          key,
          key === "defaultPage"
            ? combineUrl(window.DocSpaceConfig?.proxy?.url, newSettings[key])
            : newSettings[key]
        );
        if (key === "culture") {
          if (newSettings.wizardToken) return;
          const language = getCookie(LANGUAGE);
          if (!language || language == "undefined") {
            setCookie(LANGUAGE, newSettings[key], {
              "max-age": COOKIE_EXPIRATION_YEAR,
            });
          }
        }
      } else if (key === "passwordHash") {
        this.setValue("hashSettings", newSettings[key]);
      }
    });

    this.setGreetingSettings(newSettings.greetingSettings);

    return newSettings;
  };

  getFolderPath = async (id) => {
    this.folderPath = await api.files.getFolderPath(id);
  };

  getCustomSchemaList = async () => {
    this.customSchemaList = await api.settings.getCustomSchemaList();
  };

  getPortalSettings = async () => {
    const origSettings = await this.getSettings().catch((err) => {
      if (err?.response?.status === 404) {
        // portal not found
        return window.location.replace(
          `${wrongPortalNameUrl}?url=${window.location.hostname}`
        );
      }
    });

    if (origSettings?.plugins?.enabled) {
      initPluginStore();

      this.enablePlugins = origSettings.plugins.enabled;
      this.pluginOptions = origSettings.plugins.allow;
    }

    if (origSettings.tenantAlias) {
      this.setTenantAlias(origSettings.tenantAlias);
    }
  };

  init = async () => {
    this.setIsLoading(true);
    const requests = [];

    requests.push(
      this.getPortalSettings(),
      this.getAppearanceTheme(),
      this.getWhiteLabelLogoUrls(),
      this.getBuildVersionInfo()
    );

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

  setAdditionalResourcesData = (data) => {
    this.additionalResourcesData = data;
  };

  setAdditionalResourcesIsDefault = (additionalResourcesIsDefault) => {
    this.additionalResourcesIsDefault = additionalResourcesIsDefault;
  };

  setAdditionalResources = async (
    feedbackAndSupportEnabled,
    videoGuidesEnabled,
    helpCenterEnabled
  ) => {
    return await api.settings.setAdditionalResources(
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
      helpCenterEnabled
    );
  };

  getAdditionalResources = async () => {
    const res = await api.settings.getAdditionalResources();

    this.setAdditionalResourcesData(res);
    this.setAdditionalResourcesIsDefault(res.isDefault);
  };

  restoreAdditionalResources = async () => {
    return await api.settings.restoreAdditionalResources();
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

  setCompanyInfoSettingsData = (data) => {
    this.companyInfoSettingsData = data;
  };

  setCompanyInfoSettingsIsDefault = (companyInfoSettingsIsDefault) => {
    this.companyInfoSettingsIsDefault = companyInfoSettingsIsDefault;
  };

  setCompanyInfoSettings = async (address, companyName, email, phone, site) => {
    return api.settings.setCompanyInfoSettings(
      address,
      companyName,
      email,
      phone,
      site
    );
  };

  setLogoUrl = (url) => {
    this.logoUrl = url[0];
  };

  setLogoUrls = (urls) => {
    this.whiteLabelLogoUrls = urls;
  };

  getCompanyInfoSettings = async () => {
    const res = await api.settings.getCompanyInfoSettings();

    this.setCompanyInfoSettingsData(res);
    this.setCompanyInfoSettingsIsDefault(res.isDefault);
  };

  getWhiteLabelLogoUrls = async () => {
    const res = await api.settings.getLogoUrls();

    this.setLogoUrls(Object.values(res));
    this.setLogoUrl(Object.values(res));
  };

  restoreCompanyInfoSettings = async () => {
    return await api.settings.restoreCompanyInfoSettings();
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
    return combineUrl(
      window.DocSpaceConfig?.proxy?.url,
      `/login.ashx?p=${token}&code=${code}`
    );
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
    return timezones;
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
      docspace: version,
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
    let res = null;
    if (window?.__ASC_INITIAL_EDITOR_STATE__?.appearanceTheme)
      res = window.__ASC_INITIAL_EDITOR_STATE__.appearanceTheme;
    else res = await api.settings.getAppearanceTheme();

    const currentColorScheme = res.themes.find((theme) => {
      return res.selected === theme.id;
    });

    this.setAppearanceTheme(res.themes);
    this.setSelectThemeId(res.selected);
    this.setCurrentColorScheme(currentColorScheme);
  };

  sendAppearanceTheme = async (data) => {
    return api.settings.sendAppearanceTheme(data);
  };

  deleteAppearanceTheme = async (id) => {
    return api.settings.deleteAppearanceTheme(id);
  };
}

export default SettingsStore;
