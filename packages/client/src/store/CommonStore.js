import { makeAutoObservable } from "mobx";
import authStore from "@docspace/common/store/AuthStore";
import api from "@docspace/common/api";

class CommonStore {
  whiteLabelLogoUrls = [];
  whiteLabelLogoSizes = [];
  whiteLabelLogoText = null;

  isInit = false;
  isLoaded = false;
  isLoadedArticleBody = false;
  isLoadedSectionHeader = false;
  isLoadedSubmenu = false;
  isLoadedLngTZSettings = false;
  isLoadedDNSSettings = false;
  isLoadedPortalRenaming = false;
  isLoadedCustomization = false;
  isLoadedCustomizationNavbar = false;
  isLoadedWelcomePageSettings = false;
  isLoadedAdditionalResources = false;
  isLoadedCompanyInfoSettingsData = false;

  greetingSettingsIsDefault = true;

  constructor() {
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  initSettings = async () => {
    if (this.isInit) return;
    this.isInit = true;

    const requests = [];
    requests.push(
      authStore.settingsStore.getPortalTimezones(),
      authStore.settingsStore.getPortalCultures(),
      this.getWhiteLabelLogoUrls(),
      this.getWhiteLabelLogoText(),
      this.getWhiteLabelLogoSizes(),
      this.getGreetingSettingsIsDefault()
    );

    return Promise.all(requests).finally(() => this.setIsLoaded(true));
  };

  setLogoUrls = (urls) => {
    this.whiteLabelLogoUrls = urls;
  };

  setLogoText = (text) => {
    this.whiteLabelLogoText = text;
  };

  setLogoSizes = (sizes) => {
    this.whiteLabelLogoSizes = sizes;
  };

  getGreetingSettingsIsDefault = async () => {
    this.greetingSettingsIsDefault = await api.settings.getGreetingSettingsIsDefault();
  };

  getWhiteLabelLogoUrls = async () => {
    const res = await api.settings.getLogoUrls();
    this.setLogoUrls(Object.values(res));
  };

  getWhiteLabelLogoText = async () => {
    const res = await api.settings.getLogoText();
    this.setLogoText(res);
  };

  getWhiteLabelLogoSizes = async () => {
    const res = await api.settings.getLogoSizes();
    this.setLogoSizes(res);
  };

  setIsLoadedArticleBody = (isLoadedArticleBody) => {
    this.isLoadedArticleBody = isLoadedArticleBody;
  };

  setIsLoadedSectionHeader = (isLoadedSectionHeader) => {
    this.isLoadedSectionHeader = isLoadedSectionHeader;
  };

  setIsLoadedSubmenu = (isLoadedSubmenu) => {
    this.isLoadedSubmenu = isLoadedSubmenu;
  };

  setIsLoadedLngTZSettings = (isLoadedLngTZSettings) => {
    this.isLoadedLngTZSettings = isLoadedLngTZSettings;
  };

  setIsLoadedWelcomePageSettings = (isLoadedWelcomePageSettings) => {
    this.isLoadedWelcomePageSettings = isLoadedWelcomePageSettings;
  };

  setIsLoadedPortalRenaming = (isLoadedPortalRenaming) => {
    this.isLoadedPortalRenaming = isLoadedPortalRenaming;
  };

  setIsLoadedDNSSettings = (isLoadedDNSSettings) => {
    this.isLoadedDNSSettings = isLoadedDNSSettings;
  };

  setIsLoadedCustomization = (isLoadedCustomization) => {
    this.isLoadedCustomization = isLoadedCustomization;
  };

  setIsLoadedCustomizationNavbar = (isLoadedCustomizationNavbar) => {
    this.isLoadedCustomizationNavbar = isLoadedCustomizationNavbar;
  };

  setIsLoadedAdditionalResources = (isLoadedAdditionalResources) => {
    this.isLoadedAdditionalResources = isLoadedAdditionalResources;
  };

  setIsLoadedCompanyInfoSettingsData = (isLoadedCompanyInfoSettingsData) => {
    this.isLoadedCompanyInfoSettingsData = isLoadedCompanyInfoSettingsData;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default CommonStore;
