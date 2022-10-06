import { makeAutoObservable } from "mobx";

import { getUserRole } from "@docspace/client/src/helpers/people-helpers";
import { getUserById } from "@docspace/common/api/people";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";

class InfoPanelStore {
  isVisible = false;

  selection = null;
  selectionParentRoom = null;

  roomsView = "members";
  fileView = "history";

  authStore = null;
  settingsStore = null;
  peopleStore = null;
  selectedFolderStore = null;
  treeFoldersStore = null;

  constructor() {
    makeAutoObservable(this);
  }

  setIsVisible = (bool) => (this.isVisible = bool);

  setSelection = (selection) => (this.selection = selection);
  setSelectionParentRoom = (obj) => (this.selectionParentRoom = obj);

  setView = (view) => {
    this.roomsView = view;
    this.fileView = view === "members" ? "history" : view;
  };

  normalizeSelection = (selection) => {
    return {
      ...selection,
      isRoom: selection.isRoom || !!selection.roomType,
      icon: this.getItemIcon(selection, 32),
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

  openAccounts = (history, openSelfProfile = false) => {
    const path = [AppServerConfig.proxyURL, config.homepage, "/accounts"];
    if (openSelfProfile) path.push("/view/@self");
    this.selectedFolderStore.setSelectedFolder(null);
    this.treeFoldersStore.setSelectedNode(["accounts", "filter"]);
    history.push(combineUrl(...path));
  };

  openUser = async (userId, history) => {
    if (userId === this.authStore.userStore.user.id) {
      this.openAccounts(history, true);
      return;
    }

    const {
      getStatusType,
      getUserContextOptions,
    } = this.peopleStore.usersStore;
    const { selectUser } = this.peopleStore.selectionStore;

    this.openAccounts(history, false);
    const fetchedUser = await getUserById(userId);
    fetchedUser.role = getUserRole(fetchedUser);
    fetchedUser.statusType = getStatusType(fetchedUser);
    fetchedUser.options = getUserContextOptions(
      false,
      fetchedUser.isOwner,
      fetchedUser.statusType,
      fetchedUser.status
    );
    selectUser(fetchedUser);
  };

  getItemNoThumbnail = (item) => {
    this.getItemIcon(item, 96);
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
