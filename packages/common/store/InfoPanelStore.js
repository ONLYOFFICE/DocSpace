import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;

  selection = null;

  currentRoomTitle = null;
  currentRoomMembers = null;

  roomsView = "members";
  personalView = "history";

  settingsStore = null;
  setSettingsStore = (settingsStore) => (this.settingsStore = settingsStore);

  constructor() {
    makeAutoObservable(this);
  }

  setSelection = (selection) => {
    this.selection = selection;
  };

  getItemIcon = (item, size) => {
    return item.isRoom
      ? item.logo && item.logo.big
        ? item.logo.big
        : item.icon
      : item.isFolder
      ? this.settingsStore.getFolderIcon(item.providerKey, size)
      : this.settingsStore.getIcon(size, item.fileExst || ".file");
  };

  normalizeSelection = (selection) => {
    return {
      ...selection,
      icon: this.getItemIcon(selection, 32),
      hasCustonThumbnail: !!selection.thumbnailUrl,
      thumbnailUrl: selection.thumbnailUrl || this.getItemIcon(selection, 96),
    };
  };

  setCurrentRoomTitle = (currentRoomTitle) => {
    this.currentRoomTitle = currentRoomTitle;
  };

  setCurrentRoomMembers = (currentRoomMembers) => {
    this.currentRoomMembers = currentRoomMembers;
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

  setView = (view) => {
    this.roomsView = view;
    this.personalView = view === "members" ? "history" : view;
  };
}

export default InfoPanelStore;
