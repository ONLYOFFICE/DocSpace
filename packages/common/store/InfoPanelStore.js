import { makeAutoObservable } from "mobx";
import { Events } from "@docspace/common/constants";

import { getCategoryType } from "@docspace/client/src/helpers/utils";
import { CategoryType } from "@docspace/client/src/helpers/constants";

class InfoPanelStore {
  isVisible = false;

  selection = null;
  selectionParentRoom = null;

  roomsView = "members";
  fileView = "history";

  settingsStore = null;

  constructor() {
    makeAutoObservable(this);
  }

  setSelection = (selection) => (this.selection = selection);
  setSelectionParentRoom = (obj) => (this.selectionParentRoom = obj);
  setIsVisible = (bool) => (this.isVisible = bool);
  setSettingsStore = (settingsStore) => (this.settingsStore = settingsStore);
  setView = (view) => {
    this.roomsView = view;
    this.fileView = view === "members" ? "history" : view;
  };

  normalizeSelection = (selection) => {
    return {
      ...selection,
      isRoom: selection.isRoom || !!selection.roomType,
      icon: this.getItemIcon(selection, 32),
      hasCustonThumbnail: !!selection.thumbnailUrl,
      thumbnailUrl: selection.thumbnailUrl || this.getItemIcon(selection, 96),
      isContextMenuSelection: false,
    };
  };

  getItemIcon = (item, size) => {
    return item.isRoom || !!item.roomType
      ? item.logo && item.logo.big
        ? item.logo.big
        : item.icon
        ? item.icon
        : this.settingsStore.getIcon(size, null, null, null, item.roomType)
      : item.isFolder
      ? this.settingsStore.getFolderIcon(item.providerKey, size)
      : this.settingsStore.getIcon(size, item.fileExst || ".file");
  };

  getIsFileCategory = () => {
    const categoryType = getCategoryType(location);
    return (
      categoryType == CategoryType.Personal ||
      categoryType == CategoryType.Favorite ||
      categoryType == CategoryType.Recent ||
      categoryType == CategoryType.Trash
    );
  };

  getIsRoomCategory = () => {
    const categoryType = getCategoryType(location);
    return (
      categoryType == CategoryType.Shared ||
      categoryType == CategoryType.SharedRoom ||
      categoryType == CategoryType.Archive ||
      categoryType == CategoryType.ArchivedRoom
    );
  };

  getIsGallery = () => {
    const pathname = window.location.pathname.toLowerCase();
    return pathname.indexOf("form-gallery") !== -1;
  };

  getCanDisplay = () => {
    const pathname = window.location.pathname.toLowerCase();

    const isRooms = pathname.indexOf("rooms") !== -1;
    const isFiles = pathname.indexOf("files") !== -1;
    const isGallery = pathname.indexOf("form-gallery") !== -1;
    const isAccountsHome =
      pathname.indexOf("accounts") !== -1 && !(pathname.indexOf("view") !== -1);

    return isRooms || isFiles || isGallery || isAccountsHome;
  };
}

export default InfoPanelStore;
