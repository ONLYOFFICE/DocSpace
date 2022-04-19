import { makeAutoObservable, runInAction } from "mobx";

import authStore from "@appserver/common/store/AuthStore";

class CommonStore {
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
      authStore.settingsStore.getPortalCultures()
    );

    return Promise.all(requests).finally(() => this.setIsLoaded(true));
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
