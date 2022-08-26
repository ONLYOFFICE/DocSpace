import { makeAutoObservable } from "mobx";

import { getCategoryType } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

class InfoPanelStore {
  isVisible = false;
  roomState = "members";
  isRoom = false;

  constructor() {
    makeAutoObservable(this);
  }

  calculateisRoom = (selectedItem) => {
    const categoryType = getCategoryType(location);
    const isRoomCategory =
      categoryType == CategoryType.Shared ||
      categoryType == CategoryType.SharedRoom ||
      categoryType == CategoryType.Archive ||
      categoryType == CategoryType.ArchivedRoom;
    const isRoomItem = selectedItem.isRoom;

    this.isRoom = isRoomCategory && isRoomItem;
  };

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
