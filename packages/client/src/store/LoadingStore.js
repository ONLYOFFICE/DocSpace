import { makeAutoObservable } from "mobx";

class LoadingStore {
  isLoading = false;
  isLoaded = false;
  isRefresh = false;
  firstLoad = true;
  profileLoaded = true;

  constructor() {
    makeAutoObservable(this);
  }

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setIsRefresh = (isRefresh) => {
    this.isRefresh = isRefresh;
  };

  setFirstLoad = (firstLoad) => {
    this.firstLoad = firstLoad;
  };

  setLoadedProfile = (profileLoaded) => {
    this.profileLoaded = profileLoaded;
  };

  reset = () => {
    this.isLoading = false;
    this.isLoaded = false;
    this.isRefresh = false;
    this.firstLoad = true;
    this.profileLoaded = true;
  };
}

export default LoadingStore;
