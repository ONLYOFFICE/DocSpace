import { makeAutoObservable } from "mobx";

class BannerStore {
  isBannerVisible = true;

  constructor() {
    makeAutoObservable(this);
  }

  setIsBannerVisible = (visible) => {
    this.isBannerVisible = visible;
  };
}

export default BannerStore;
