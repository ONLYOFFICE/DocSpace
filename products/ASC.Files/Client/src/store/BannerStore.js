import { makeAutoObservable } from "mobx";

class BannerStore {
  bannerTypes = ["Cloud", "Desktop", "Education", "Enterprise", "Integration"];

  constructor() {
    makeAutoObservable(this);
  }

  getBannerType = () => {
    return this.bannerTypes[
      Math.floor(Math.random() * this.bannerTypes.length)
    ];
  };
}

export default BannerStore;
