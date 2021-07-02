import { makeAutoObservable } from "mobx";

class BannerStore {
  bannerTypes = ["Cloud", "Desktop", "Education", "Enterprise", "Integration"];

  constructor() {
    makeAutoObservable(this);
  }

  getBannerType = (index) => {
    return this.bannerTypes[index];
  };
}

export default BannerStore;
