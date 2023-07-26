import { makeAutoObservable } from "mobx";

import { getUserById } from "@docspace/common/api/people";
import { combineUrl, getUserRole } from "@docspace/common/utils";
import { FolderType } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import Filter from "../api/people/filter";
import { getRoomInfo } from "../api/rooms";

const observedKeys = [
  "id",
  "title",
  "thumbnailStatus",
  "thumbnailUrl",
  "version",
  "comment",
];

class InfoPanelStore {
  isVisible = false;
  isMobileHidden = false;

  selection = null;
  selectionHistory = null;
  selectionParentRoom = null;
  selectionHistory = null;

  roomsView = "info_details";
  fileView = "info_history";

  updateRoomMembers = null;
  isScrollLocked = false;
  historyWithFileList = false;

  authStore = null;
  settingsStore = null;
  peopleStore = null;
  filesStore = null;
  selectedFolderStore = null;
  treeFoldersStore = null;

  constructor() {
    makeAutoObservable(this);
  }

  // Setters

  setIsVisible = (bool) => {
    this.setView("info_details");
    this.isVisible = bool;
    this.isScrollLocked = false;
  };

  setIsMobileHidden = (bool) => (this.isMobileHidden = bool);

  setSelection = (selection) => {
    if (this.getIsAccounts() && (!selection.email || !selection.displayName)) {
      this.selection = selection.length
        ? selection
        : { isSelectedFolder: true };
      return;
    }
    this.selection = selection;
    this.isScrollLocked = false;
  };

  setSelectionParentRoom = (obj) => (this.selectionParentRoom = obj);
  setSelectionHistory = (obj) => (this.selectionHistory = obj);

  setSelectionHistory = (obj) => {
    this.selectionHistory = obj;
    this.historyWithFileList = this.selection.isFolder || this.selection.isRoom;
  };

  setView = (view) => {
    this.roomsView = view;
    this.fileView = view === "info_members" ? "info_history" : view;
    this.isScrollLocked = false;
  };

  setUpdateRoomMembers = (updateRoomMembers) => {
    this.updateRoomMembers = updateRoomMembers;
  };

  setIsScrollLocked = (isScrollLocked) => {
    this.isScrollLocked = isScrollLocked;
  };

  // Selection helpers //

  getSelectedItems = () => {
    const {
      selection: filesStoreSelection,
      bufferSelection: filesStoreBufferSelection,
    } = this.filesStore;

    const {
      selection: peopleStoreSelection,
      bufferSelection: peopleStoreBufferSelection,
    } = this.peopleStore.selectionStore;

    return this.getIsAccounts()
      ? peopleStoreSelection.length
        ? [...peopleStoreSelection]
        : peopleStoreBufferSelection
        ? [peopleStoreBufferSelection]
        : []
      : filesStoreSelection?.length > 0
      ? [...filesStoreSelection]
      : filesStoreBufferSelection
      ? [filesStoreBufferSelection]
      : [];
  };

  getSelectedFolder = () => {
    const selectedFolderStore = { ...this.selectedFolderStore };
    return {
      ...selectedFolderStore,
      isFolder: true,
      isRoom: !!this.selectedFolderStore.roomType,
    };
  };

  calculateSelection = (
    props = { selectedItems: [], selectedFolder: null }
  ) => {
    const selectedItems = props.selectedItems.length
      ? props.selectedItems
      : this.getSelectedItems();

    const selectedFolder = props.selectedFolder
      ? props.selectedFolder
      : this.getSelectedFolder();

    return selectedItems.length === 0
      ? this.normalizeSelection({
          ...selectedFolder,
          isSelectedFolder: true,
          isSelectedItem: false,
        })
      : selectedItems.length === 1
      ? this.normalizeSelection({
          ...selectedItems[0],
          isSelectedFolder: false,
          isSelectedItem: true,
        })
      : [...Array(selectedItems.length).keys()];
  };

  normalizeSelection = (selection) => {
    const isContextMenuSelection = selection.isContextMenuSelection;
    return {
      ...selection,
      isRoom: selection.isRoom || !!selection.roomType,
      icon: this.getInfoPanelItemIcon(selection, 32),
      isContextMenuSelection: false,
      wasContextMenuSelection: !!isContextMenuSelection,
    };
  };

  reloadSelection = () => {
    this.setSelection(this.calculateSelection());
  };

  updateRoomLogoCacheBreaker = () => {
    const logo = this.selection.logo;
    this.setSelection({
      ...this.selection,
      logo: {
        small: logo.small.split("?")[0] + "?" + new Date().getTime(),
        medium: logo.medium.split("?")[0] + "?" + new Date().getTime(),
        large: logo.large.split("?")[0] + "?" + new Date().getTime(),
        original: logo.original.split("?")[0] + "?" + new Date().getTime(),
      },
    });
  };

  reloadSelectionParentRoom = async () => {
    if (!this.getIsRooms) return;

    const currentFolderRoomId =
      this.selectedFolderStore.pathParts &&
      this.selectedFolderStore.pathParts[1];
    const prevRoomId = this.selectionParentRoom?.id;

    if (!currentFolderRoomId || currentFolderRoomId === prevRoomId) return;

    const newSelectionParentRoom = await getRoomInfo(currentFolderRoomId);

    if (prevRoomId === newSelectionParentRoom.id) return;

    this.setSelectionParentRoom(
      this.normalizeSelection(newSelectionParentRoom)
    );
  };

  isItemChanged = (oldItem, newItem) => {
    for (let i = 0; i < observedKeys.length; i++) {
      const value = observedKeys[i];
      if (oldItem[value] !== newItem[value]) return true;
    }
    return false;
  };

  // Icon helpers //

  getInfoPanelItemIcon = (item, size) => {
    return item.isRoom || !!item.roomType
      ? item.rootFolderType === FolderType.Archive
        ? this.settingsStore.getIcon(
            size,
            null,
            null,
            null,
            item.roomType,
            true
          )
        : item.logo && item.logo.medium
        ? item.logo.medium
        : item.icon
        ? item.icon
        : this.settingsStore.getIcon(size, null, null, null, item.roomType)
      : item.isFolder
      ? this.settingsStore.getFolderIcon(item.providerKey, size)
      : this.settingsStore.getIcon(size, item.fileExst || ".file");
  };

  // User link actions //

  openUser = async (user, navigate) => {
    if (user.id === this.authStore.userStore.user.id) {
      this.openSelfProfile(navigate);
      return;
    }

    const fetchedUser = await this.fetchUser(user.id);
    this.openAccountsWithSelectedUser(fetchedUser, navigate);
  };

  openSelfProfile = (navigate) => {
    const path = [
      window.DocSpaceConfig?.proxy?.url,
      config.homepage,
      "/accounts",
      "/profile",
    ];
    this.selectedFolderStore.setSelectedFolder(null);
    this.treeFoldersStore.setSelectedNode(["accounts", "filter"]);
    navigate(combineUrl(...path));
  };

  openAccountsWithSelectedUser = async (user, navigate) => {
    const { getUsersList } = this.peopleStore.usersStore;
    const { setSelection } = this.peopleStore.selectionStore;

    const path = [
      window.DocSpaceConfig?.proxy?.url,
      config.homepage,
      "/accounts",
    ];

    const newFilter = Filter.getDefault();
    newFilter.page = 0;
    newFilter.search = user.email;
    path.push(`filter?${newFilter.toUrlParams()}`);
    const userList = await getUsersList(newFilter);

    navigate(combineUrl(...path));
    this.selectedFolderStore.setSelectedFolder(null);
    this.treeFoldersStore.setSelectedNode(["accounts"]);
    setSelection([user]);
  };

  fetchUser = async (userId) => {
    const { getStatusType, getUserContextOptions } =
      this.peopleStore.usersStore;

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

  // Routing helpers //

  getCanDisplay = () => {
    const pathname = window.location.pathname.toLowerCase();
    const isFiles = this.getIsFiles(pathname);
    const isRooms = this.getIsRooms(pathname);
    const isAccounts = this.getIsAccounts(pathname);
    const isGallery = this.getIsGallery(pathname);
    return isRooms || isFiles || isGallery || isAccounts;
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

  getIsTrash = (givenPathName) => {
    const pathname = givenPathName || window.location.pathname.toLowerCase();
    return pathname.indexOf("files/trash") !== -1;
  };
}

export default InfoPanelStore;
