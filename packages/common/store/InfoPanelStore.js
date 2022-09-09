import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;

  selection = null;
  selectionParentRoom = null;

  roomsView = "members";
  personalView = "history";

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
    this.personalView = view === "members" ? "history" : view;
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
}

export default InfoPanelStore;
