import { makeAutoObservable } from "mobx";

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
      thumbnailUrl:
        selection.isRoom && selection.logo?.large
          ? selection.logo.large
          : selection.thumbnailUrl || this.getItemIcon(selection, 96),
      isContextMenuSelection: false,
    };
  };

  getItemIcon = (item, size) => {
    return item.isRoom || !!item.roomType
      ? item.logo && item.logo.medium
        ? item.logo.medium
        : item.icon
        ? item.icon
        : this.settingsStore.getIcon(size, null, null, null, item.roomType)
      : item.isFolder
      ? this.settingsStore.getFolderIcon(item.providerKey, size)
      : this.settingsStore.getIcon(size, item.fileExst || ".file");
  };

  getIsFiles = (givenPathName) => {
    const pathname = givenPathName || window.location.pathname.toLowerCase();
    return (
      pathname.indexOf("files") !== -1 || pathname.indexOf("personal") !== -1
    );
  };

  getIsRooms = (givenPathName) => {
    const pathname = givenPathName || window.location.pathname.toLowerCase();
    return (
      pathname.indexOf("rooms") !== -1 && !(pathname.indexOf("personal") !== -1)
    );
  };

  getIsAccounts = (givenPathName) => {
    const pathname = givenPathName || window.location.pathname.toLowerCase();
    return (
      pathname.indexOf("accounts") !== -1 && !(pathname.indexOf("view") !== -1)
    );
  };

  getIsGallery = (givenPathName) => {
    const pathname = givenPathName || window.location.pathname.toLowerCase();
    return pathname.indexOf("form-gallery") !== -1;
  };

  getCanDisplay = () => {
    const pathname = window.location.pathname.toLowerCase();
    const isFiles = this.getIsFiles(pathname);
    const isRooms = this.getIsRooms(pathname);
    const isAccounts = this.getIsAccounts(pathname);
    const isGallery = this.getIsGallery(pathname);
    return isRooms || isFiles || isGallery || isAccounts;
  };
}

export default InfoPanelStore;
