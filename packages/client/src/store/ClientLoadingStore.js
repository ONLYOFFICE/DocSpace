import { makeAutoObservable } from "mobx";

class ClientLoadingStore {
  isLoaded = false;
  firstLoad = true;

  isArticleLoading = true;

  isSectionHeaderLoading = false;
  isSectionFilterLoading = false;
  isSectionBodyLoading = false;
  isProfileLoaded = false;

  // showArticleLoader = true;
  // showHeaderLoader = true;
  // showFilterLoader = true;
  // showBodyLoader = true;

  sectionHeaderTimer = null;
  sectionFilterTimer = null;
  sectionBodyTimer = null;

  pendingSectionLoaders = {
    header: false,
    filter: false,
    body: false,
  };

  constructor() {
    makeAutoObservable(this);
  }

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setFirstLoad = (firstLoad) => {
    this.firstLoad = firstLoad;
  };

  setIsArticleLoading = (isArticleLoading) => {
    this.isArticleLoading = isArticleLoading;
  };

  setIsSectionHeaderLoading = (isSectionHeaderLoading, withTimer = true) => {
    this.pendingSectionLoaders.header = isSectionHeaderLoading;
    if (isSectionHeaderLoading) {
      if (this.sectionHeaderTimer) {
        return;
      }
      if (withTimer) {
        return (this.sectionHeaderTimer = setTimeout(() => {
          this.isSectionHeaderLoading = isSectionHeaderLoading;
        }, 500));
      }
      this.isSectionHeaderLoading = isSectionHeaderLoading;
    } else {
      if (this.sectionHeaderTimer) {
        clearTimeout(this.sectionHeaderTimer);
        this.sectionHeaderTimer = null;
      }
      this.isSectionHeaderLoading = false;
    }
  };

  setIsSectionFilterLoading = (isSectionFilterLoading, withTimer = true) => {
    this.pendingSectionLoaders.filter = isSectionFilterLoading;
    if (isSectionFilterLoading) {
      if (this.sectionFilterTimer) {
        return;
      }
      if (withTimer) {
        return (this.sectionFilterTimer = setTimeout(() => {
          this.isSectionFilterLoading = isSectionFilterLoading;
        }, 500));
      }
      this.isSectionFilterLoading = isSectionFilterLoading;
    } else {
      if (this.sectionFilterTimer) {
        clearTimeout(this.sectionFilterTimer);
        this.sectionFilterTimer = null;
      }
      this.isSectionFilterLoading = false;
    }
  };

  setIsSectionBodyLoading = (isSectionBodyLoading, withTimer = true) => {
    this.pendingSectionLoaders.body = isSectionBodyLoading;
    if (isSectionBodyLoading) {
      if (this.sectionBodyTimer) {
        return;
      }
      if (withTimer) {
        return (this.sectionBodyTimer = setTimeout(() => {
          this.isSectionBodyLoading = isSectionBodyLoading;
        }, 500));
      }
      this.isSectionBodyLoading = isSectionBodyLoading;
    } else {
      if (this.sectionBodyTimer) {
        clearTimeout(this.sectionBodyTimer);
        this.sectionBodyTimer = null;
      }
      this.isSectionBodyLoading = false;
    }
  };

  setIsProfileLoaded = (isProfileLoaded) => {
    this.isProfileLoaded = isProfileLoaded;
  };

  hideLoaders = () => {
    this.clearTimers();
    this.showHeaderLoader = false;
    this.showFilterLoader = false;
    this.showBodyLoader = false;
    this.isSectionHeaderLoading = false;
    this.isSectionFilterLoading = false;
    this.isSectionBodyLoading = false;
  };

  clearTimers = () => {
    clearTimeout(this.sectionHeaderTimer);
    clearTimeout(this.sectionFilterTimer);
    clearTimeout(this.sectionBodyTimer);
  };

  get isLoading() {
    return (
      this.isArticleLoading ||
      this.pendingSectionLoaders.header ||
      this.pendingSectionLoaders.filter ||
      this.pendingSectionLoaders.body
    );
  }

  get showArticleLoader() {
    return this.isArticleLoading;
  }

  get showHeaderLoader() {
    return this.isSectionHeaderLoading || this.isArticleLoading;
  }

  get showFilterLoader() {
    return this.isSectionFilterLoading || this.showHeaderLoader;
  }

  get showBodyLoader() {
    return this.isSectionBodyLoading || this.showFilterLoader;
  }
}

export default ClientLoadingStore;
