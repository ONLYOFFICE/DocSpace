import { makeAutoObservable } from "mobx";

const SHOW_LOADER_TIMER = 500;
const MIN_LOADER_TIMER = 500;

class ClientLoadingStore {
  publicRoomStore;

  isLoaded = false;
  firstLoad = true;

  isArticleLoading = true;

  isSectionHeaderLoading = false;
  isSectionFilterLoading = false;
  isSectionBodyLoading = false;

  isProfileLoaded = false;

  sectionHeaderTimer = null;
  sectionFilterTimer = null;
  sectionBodyTimer = null;

  pendingSectionLoaders = {
    header: false,
    filter: false,
    body: false,
  };

  startLoadingTime = {
    header: null,
    filter: null,
    body: null,
  };

  constructor(publicRoomStore) {
    makeAutoObservable(this);

    this.publicRoomStore = publicRoomStore;
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

  updateIsSectionHeaderLoading = (param) => {
    this.isSectionHeaderLoading = param;
  };

  updateIsSectionFilterLoading = (param) => {
    this.isSectionFilterLoading = param;
  };

  updateIsSectionBodyLoading = (param) => {
    this.isSectionBodyLoading = param;
  };

  setIsSectionHeaderLoading = (isSectionHeaderLoading, withTimer = true) => {
    this.pendingSectionLoaders.header = isSectionHeaderLoading;
    if (isSectionHeaderLoading) {
      if (this.sectionHeaderTimer) {
        return;
      }
      this.startLoadingTime.header = new Date();
      if (withTimer) {
        return (this.sectionHeaderTimer = setTimeout(() => {
          this.updateIsSectionHeaderLoading(isSectionHeaderLoading);
        }, SHOW_LOADER_TIMER));
      }
      this.updateIsSectionHeaderLoading(isSectionHeaderLoading);
    } else {
      if (this.startLoadingTime.header) {
        const currentDate = new Date();

        const ms = Math.abs(
          this.startLoadingTime.header.getTime() - currentDate.getTime()
        );
        if (this.sectionHeaderTimer) {
          let ms = Math.abs(ms - SHOW_LOADER_TIMER);
          clearTimeout(this.sectionHeaderTimer);
          this.sectionHeaderTimer = null;
        }

        if (ms < MIN_LOADER_TIMER)
          return setTimeout(() => {
            this.updateIsSectionHeaderLoading(false);
            this.startLoadingTime.header = null;
          }, MIN_LOADER_TIMER - ms);
      }
      if (this.sectionHeaderTimer) {
        clearTimeout(this.sectionHeaderTimer);
        this.sectionHeaderTimer = null;
      }
      this.startLoadingTime.header = null;
      this.updateIsSectionHeaderLoading(false);
    }
  };

  setIsSectionFilterLoading = (isSectionFilterLoading, withTimer = true) => {
    this.pendingSectionLoaders.filter = isSectionFilterLoading;
    if (isSectionFilterLoading) {
      if (this.sectionFilterTimer) {
        return;
      }
      this.startLoadingTime.filter = new Date();
      if (withTimer) {
        return (this.sectionFilterTimer = setTimeout(() => {
          this.updateIsSectionFilterLoading(isSectionFilterLoading);
        }, SHOW_LOADER_TIMER));
      }
      this.updateIsSectionFilterLoading(isSectionFilterLoading);
    } else {
      if (this.startLoadingTime.filter) {
        const currentDate = new Date();

        let ms = Math.abs(
          this.startLoadingTime.filter.getTime() - currentDate.getTime()
        );

        if (this.sectionFilterTimer) {
          let ms = Math.abs(ms - SHOW_LOADER_TIMER);

          clearTimeout(this.sectionFilterTimer);
          this.sectionFilterTimer = null;
        }

        if (ms < MIN_LOADER_TIMER) {
          return setTimeout(() => {
            this.updateIsSectionFilterLoading(false);
            this.startLoadingTime.filter = null;
          }, MIN_LOADER_TIMER - ms);
        }
      }
      if (this.sectionFilterTimer) {
        clearTimeout(this.sectionFilterTimer);
        this.sectionFilterTimer = null;
      }

      this.startLoadingTime.filter = null;
      this.updateIsSectionFilterLoading(false);
    }
  };

  setIsSectionBodyLoading = (isSectionBodyLoading, withTimer = true) => {
    this.pendingSectionLoaders.body = isSectionBodyLoading;

    if (isSectionBodyLoading) {
      if (this.sectionBodyTimer) {
        return;
      }
      this.startLoadingTime.body = new Date();
      if (withTimer) {
        return (this.sectionBodyTimer = setTimeout(() => {
          this.updateIsSectionBodyLoading(isSectionBodyLoading);
        }, SHOW_LOADER_TIMER));
      }
      this.updateIsSectionBodyLoading(isSectionBodyLoading);
    } else {
      if (this.startLoadingTime.body) {
        const currentDate = new Date();

        let ms = Math.abs(
          this.startLoadingTime.body.getTime() - currentDate.getTime()
        );

        if (this.sectionBodyTimer) {
          let ms = Math.abs(ms - SHOW_LOADER_TIMER);

          clearTimeout(this.sectionBodyTimer);
          this.sectionBodyTimer = null;
        }

        if (ms < MIN_LOADER_TIMER)
          return setTimeout(() => {
            this.updateIsSectionBodyLoading(false);
            this.startLoadingTime.body = null;
          }, MIN_LOADER_TIMER - ms);
      }

      if (this.sectionBodyTimer) {
        clearTimeout(this.sectionBodyTimer);
        this.sectionBodyTimer = null;
      }

      this.startLoadingTime.body = null;
      this.updateIsSectionBodyLoading(false);
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
      (this.isArticleLoading && !this.publicRoomStore.isPublicRoom) ||
      this.pendingSectionLoaders.header ||
      this.pendingSectionLoaders.filter ||
      this.pendingSectionLoaders.body
    );
  }

  get showArticleLoader() {
    if (this.publicRoomStore.isPublicRoom) return false;
    return this.isArticleLoading;
  }

  get showHeaderLoader() {
    return this.isSectionHeaderLoading || this.showArticleLoader;
  }

  get showFilterLoader() {
    return this.isSectionFilterLoading || this.showHeaderLoader;
  }

  get showBodyLoader() {
    return this.isSectionBodyLoading || this.showFilterLoader;
  }
}

export default ClientLoadingStore;
