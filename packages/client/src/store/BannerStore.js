import { makeAutoObservable } from "mobx";

class BannerStore {
  isBannerVisible = false; //TODO: set to true after fix

  constructor() {
    makeAutoObservable(this);
  }

  setIsBannerVisible = (visible) => {
    this.isBannerVisible = visible;
  };
}

export default BannerStore;
