import { makeAutoObservable } from "mobx";

class AboutDialogStore {
  isAboutDialogVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  setIsAboutDialogVisible = (visible) => {
    this.isAboutDialogVisible = visible;
  };
}

export default AboutDialogStore;
