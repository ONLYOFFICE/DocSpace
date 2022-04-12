import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  onHeaderCrossClick = () => {
    this.isVisible = false;
  };

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
  };

  setVisible = () => {
    this.isVisible = true;
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;
  };
}

export default InfoPanelStore;
