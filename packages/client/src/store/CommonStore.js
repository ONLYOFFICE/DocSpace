import { makeAutoObservable, runInAction } from "mobx";
import authStore from "@docspace/common/store/AuthStore";
import api from "@docspace/common/api";

class CommonStore {
  whiteLabelLogoSizes = [];
  whiteLabelLogoText = null;

  isInit = false;
  isLoaded = false;
  isLoadedArticleBody = false;
  isLoadedArticleHeader = false;
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
      this.getWhiteLabelLogoText(),
      this.getWhiteLabelLogoSizes()
    );

    return Promise.all(requests).finally(() => this.setIsLoaded(true));
  };

  setLogoText = (text) => {
    this.whiteLabelLogoText = text;
  };

  setLogoSizes = (sizes) => {
    this.whiteLabelLogoSizes = sizes;
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
    runInAction(() => {
      this.isLoadedArticleBody = isLoadedArticleBody;
    });
  };

  setIsLoadedArticleHeader = (isLoadedArticleHeader) => {
    runInAction(() => {
      this.isLoadedArticleHeader = isLoadedArticleHeader;
    });
  };

  setIsLoadedSectionHeader = (isLoadedSectionHeader) => {
    runInAction(() => {
      this.isLoadedSectionHeader = isLoadedSectionHeader;
    });
  };

  setIsLoadedSubmenu = (isLoadedSubmenu) => {
    runInAction(() => {
      this.isLoadedSubmenu = isLoadedSubmenu;
    });
  };

  setIsLoadedLngTZSettings = (isLoadedLngTZSettings) => {
    runInAction(() => {
      this.isLoadedLngTZSettings = isLoadedLngTZSettings;
    });
  };

  setIsLoadedWelcomePageSettings = (isLoadedWelcomePageSettings) => {
    runInAction(() => {
      this.isLoadedWelcomePageSettings = isLoadedWelcomePageSettings;
    });
  };

  setIsLoadedPortalRenaming = (isLoadedPortalRenaming) => {
    runInAction(() => {
      this.isLoadedPortalRenaming = isLoadedPortalRenaming;
    });
  };

  setIsLoadedDNSSettings = (isLoadedDNSSettings) => {
    runInAction(() => {
      this.isLoadedDNSSettings = isLoadedDNSSettings;
    });
  };

  setIsLoadedCustomization = (isLoadedCustomization) => {
    runInAction(() => {
      this.isLoadedCustomization = isLoadedCustomization;
    });
  };

  setIsLoadedCustomizationNavbar = (isLoadedCustomizationNavbar) => {
    runInAction(() => {
      this.isLoadedCustomizationNavbar = isLoadedCustomizationNavbar;
    });
  };

  setIsLoadedAdditionalResources = (isLoadedAdditionalResources) => {
    runInAction(() => {
      this.isLoadedAdditionalResources = isLoadedAdditionalResources;
    });
  };

  setIsLoadedCompanyInfoSettingsData = (isLoadedCompanyInfoSettingsData) => {
    runInAction(() => {
      this.isLoadedCompanyInfoSettingsData = isLoadedCompanyInfoSettingsData;
    });
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default CommonStore;
