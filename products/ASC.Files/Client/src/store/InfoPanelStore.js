import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;
  showCurrentFolder = false;

  constructor() {
    makeAutoObservable(this);
  }

  setShowCurrentFolder = (bool) => {
    this.showCurrentFolder = bool;
  };

  get showCurrentFolder() {
    return this.showCurrenFolder;
  }

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
  };

  setVisible = () => {
    this.isVisible = true;
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;
  };

  get isVisible() {
    return this.isVisible;
  }
}

export default InfoPanelStore;
