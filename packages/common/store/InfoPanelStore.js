import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;
  roomState = "members";
  isRoom = false;

  constructor() {
    makeAutoObservable(this);
  }

  setIsRoom = (isRoom) => {
    this.isRoom = isRoom;
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

  setRoomState = (str) => {
    this.roomState = str;
  };
}

export default InfoPanelStore;
