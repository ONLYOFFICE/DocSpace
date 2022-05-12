import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
  };

  setInfoPanelIsVisible = (isVisible) => {
    this.isVisible = isVisible;
  };

  setVisible = () => {
    this.isVisible = true;
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;
  };
}

export default InfoPanelStore;
