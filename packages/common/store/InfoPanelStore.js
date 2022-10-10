import { makeAutoObservable } from "mobx";

import { getUserRole } from "@docspace/client/src/helpers/people-helpers";
import { getUserById } from "@docspace/common/api/people";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import Filter from "../api/people/filter";

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
    const isContextMenuSelection = selection.isContextMenuSelection;
    return {
      ...selection,
      isRoom: selection.isRoom || !!selection.roomType,
      icon: this.getItemIcon(selection, 32),
      isContextMenuSelection: false,
      wasContextMenuSelection: !!isContextMenuSelection,
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

  openAccounts = async (history, openSelfProfile = false) => {
    const path = [AppServerConfig.proxyURL, config.homepage, "/accounts"];
    if (openSelfProfile) path.push("/view/@self");
    this.selectedFolderStore.setSelectedFolder(null);

    const { filter } = this.peopleStore.filterStore;
    const { getUsersList } = this.peopleStore.usersStore;

    const newFilter = Filter.getDefault();
    newFilter.page = 0;
    newFilter.search = "afiltnessc@wsj.com";
    await getUsersList(newFilter);
    path.push(`filter?${newFilter.toUrlParams()}`);

    this.treeFoldersStore.setSelectedNode(["accounts", newFilter]);
    history.push(combineUrl(...path));
  };

  filterUser = async (user) => {
    const { filter } = this.peopleStore.filterStore;
    const { getUsersList } = this.peopleStore.usersStore;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.search = user.email;
    await getUsersList(newFilter);
  };

  openSelfProfile = (history) => {
    const path = [
      AppServerConfig.proxyURL,
      config.homepage,
      "/accounts",
      "/view/@self",
    ];
    this.selectedFolderStore.setSelectedFolder(null);
    this.treeFoldersStore.setSelectedNode(["accounts", "filter"]);
    history.push(combineUrl(...path));
  };

  fetchUser = async (userId) => {
    const {
      getStatusType,
      getUserContextOptions,
    } = this.peopleStore.usersStore;

    const fetchedUser = await getUserById(userId);
    fetchedUser.role = getUserRole(fetchedUser);
    fetchedUser.statusType = getStatusType(fetchedUser);
    fetchedUser.options = getUserContextOptions(
      false,
      fetchedUser.isOwner,
      fetchedUser.statusType,
      fetchedUser.status
    );

    return fetchedUser;
  };

  openAccountsWithSelectedUser = async (user, history) => {
    const { getUsersList } = this.peopleStore.usersStore;
    const { selectUser } = this.peopleStore.selectionStore;

    const path = [AppServerConfig.proxyURL, config.homepage, "/accounts"];

    const newFilter = Filter.getDefault();
    newFilter.page = 0;
    newFilter.search = user.email;
    await getUsersList(newFilter);
    path.push(`filter?${newFilter.toUrlParams()}`);

    this.selectedFolderStore.setSelectedFolder(null);
    this.treeFoldersStore.setSelectedNode(["accounts"]);
    history.push(combineUrl(...path));

    selectUser(user);
  };

  openUser = async (userId, history) => {
    if (userId === this.authStore.userStore.user.id) {
      this.openSelfProfile(history);
      return;
    }

    const fetchedUser = await this.fetchUser(userId);
    this.openAccountsWithSelectedUser(fetchedUser, history);
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
