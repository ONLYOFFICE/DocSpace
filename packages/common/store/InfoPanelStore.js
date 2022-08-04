import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;
  roomState = "members";

  constructor() {
    makeAutoObservable(this);
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

  setRoomState = (str) => {
    this.roomState = str;
  };
}

export default InfoPanelStore;
