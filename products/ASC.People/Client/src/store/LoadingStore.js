import { makeAutoObservable } from "mobx";

class LoadingStore {
  isLoading = false;
  isLoaded = false;
  isRefresh = false;
  firstLoad = true;

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
}

export default LoadingStore;
