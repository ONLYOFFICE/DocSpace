import { makeAutoObservable, runInAction } from "mobx";
import authStore from "@appserver/common/store/AuthStore";
import api from "@appserver/common/api";

class CommonStore {
  whiteLabel = {
    logoSizes: [],
    logoText: null,
    logoUrls: [],
  };

  isInit = false;
  isLoaded = false;
  isLoadedArticleBody = false;
  isLoadedArticleHeader = false;
  isLoadedSectionHeader = false;
  isLoadedSubmenu = false;
  isLoadedLngTZSettings = false;
  isLoadedPortalRenaming = false;
  isLoadedCustomization = false;
  isLoadedCustomizationNavbar = false;
  isLoadedWelcomePageSettings = false;

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
      this.getWhiteLabelLogoSizes(),
      this.getWhiteLabelLogoUrls()
    );

    return Promise.all(requests).finally(() => this.setIsLoaded(true));
  };

  setLogoText = (text) => {
    this.whiteLabel.logoText = text;
  };

  setLogoSizes = (sizes) => {
    this.whiteLabel.logoSizes = sizes;
  };

  setLogoUrls = (urls) => {
    this.whiteLabel.logoUrls = urls;
  };

  getWhiteLabelLogoText = async () => {
    const res = await api.settings.getLogoText();
    this.setLogoText(res);
  };

  getWhiteLabelLogoSizes = async () => {
    const res = await api.settings.getLogoSizes();
    this.setLogoSizes(res);
  };

  getWhiteLabelLogoUrls = async () => {
    const res = await api.settings.getLogoUrls();
    this.setLogoUrls(Object.values(res));
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

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };
}

export default CommonStore;
