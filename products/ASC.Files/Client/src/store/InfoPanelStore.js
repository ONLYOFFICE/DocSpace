import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;
  isShowRootFolder = false;

  constructor() {
    makeAutoObservable(this);
  }

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;
  };

  setIsToggleVisible = (bool) => {
    this.isToggleVisible = bool;
  };

  setIsShowRootFolder = (bool) => {
    this.isShowRootFolder = bool;
  };
}

export default InfoPanelStore;
