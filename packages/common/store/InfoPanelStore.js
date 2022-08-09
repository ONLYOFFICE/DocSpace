import { makeAutoObservable } from "mobx";

import { getCategoryType } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

class InfoPanelStore {
  isVisible = false;
  roomState = "members";

  constructor() {
    makeAutoObservable(this);
  }

  isRoom = (selectedItem) => {
    const categoryType = getCategoryType(location);
    const isRoomCategory =
      categoryType == CategoryType.Shared ||
      categoryType == CategoryType.SharedRoom ||
      categoryType == CategoryType.Archive ||
      categoryType == CategoryType.ArchivedRoom;
    const isRoomItem = selectedItem.isRoom;
    return isRoomCategory && isRoomItem;
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
