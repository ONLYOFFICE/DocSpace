import { makeAutoObservable, runInAction } from "mobx";
import api from "@appserver/common/api";
import { AppServerConfig, RoomsType } from "@appserver/common/constants";

import toastr from "studio/toastr";

import history from "@appserver/common/history";

import { combineUrl } from "@appserver/common/utils";

import config from "../../package.json";

const { RoomsFilter } = api;

class RoomsStore {
  authStore;
  settingsStore;
  userStore;
  filesStore;
  selectedFolderStore;
  treeFoldersStore;
  filesSettingsStore;

  rooms = [];
  tags = [];

  selection = [];
  bufferSelection = [];

  filter = RoomsFilter.getDefault();

  constructor(
    authStore,
    settingsStore,
    userStore,
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
    filesSettingsStore
  ) {
    makeAutoObservable(this);

    this.authStore = authStore;
    this.settingsStore = settingsStore;
    this.userStore = userStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesSettingsStore = filesSettingsStore;
  }

  setRoom = (room) => {
    const idx = this.rooms.findIndex((item) => item.id === room.id);

    this.rooms[idx] = room;
  };

  setRooms = (rooms) => {
    this.rooms = rooms;
  };

  setTags = (tags) => {
    this.tags = tags;
  };

  setRoomsFilter = (filter) => {
    this.filter = filter;
  };

  setFilterUrl = (filter) => {
    this.filter = filter;
    const urlFilter = filter.toUrlParams();
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/rooms?${urlFilter}`
      )
    );
  };

  setSelection = (items) => {
    this.selection = items;
  };

  setBufferSelection = (item) => {
    this.bufferSelection = item;
  };

  setHeaderVisible = (isHeaderVisible) => {
    this.isHeaderVisible = isHeaderVisible;
  };

  sortRooms = (sortBy, sortOrder) => {
    const newFilter = this.filter.clone();

    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;

    return this.fetchRooms(newFilter.searchArea, newFilter);
  };

  filterRooms = (types, subjectId, tags) => {
    const newFilter = this.filter.clone();

    newFilter.page = 0;
    newFilter.types = types ? types : null;
    newFilter.subjectId = subjectId ? subjectId : null;
    newFilter.tags = tags ? tags : null;

    return this.fetchRooms(newFilter.searchArea, newFilter);
  };

  searchRooms = (filterValue) => {
    const newFilter = this.filter.clone();

    newFilter.page = 0;
    newFilter.filterValue = filterValue;

    return this.fetchRooms(newFilter.searchArea, newFilter);
  };

  fetchRooms = (searchArea, filter) => {
    const { setSelectedNode } = this.treeFoldersStore;

    const filterData = !!filter ? filter.clone() : RoomsFilter.getDefault();

    if (searchArea && filterData.searchArea !== searchArea) {
      filterData.searchArea = searchArea;
    }

    const request = () =>
      api.rooms
        .getRooms(filterData)
        .then(async (data) => {
          const folderId = data.current.id;

          setSelectedNode([folderId + ""]);

          filterData.total = data.total;

          if (data.total > 0) {
            const lastPage = filterData.getLastPage();

            if (filterData.page > lastPage) {
              filterData.page = lastPage;

              return this.fetchFiles(searchArea, filterData);
            }
          }

          this.setFilterUrl(filterData);

          runInAction(() => {
            this.setRooms(data.folders);
            this.fetchTags();
          });

          this.selectedFolderStore.setSelectedFolder({
            folders: data.folders,
            ...data.current,
            pathParts: data.pathParts,
            navigationPath: [],
            ...{ new: data.new },
          });
        })
        .catch((err) => {
          toastr.error(err);
        });

    return request();
  };

  fetchRoomInfo = (id) => api.rooms.getRoom(id);

  selectRoom = (checked, item) => {
    this.setBufferSelection(null);

    if (checked) {
      this.selection.push(item);
    } else {
      const idx = this.selection.findIndex((room) => room.id === item.id);

      this.selection.splice(idx, 1);
    }
  };

  setSelected = (selected) => {
    if (selected === "none") {
      this.setBufferSelection(null);
    }

    const newSelection = [];

    this.rooms.forEach((room) => {
      const checked = this.getRoomChecked(room, selected);

      if (checked) newSelection.push(room);
    });

    this.selection = newSelection;
  };

  openContextMenu = (item) => {
    if (this.selection.length > 0) return;

    this.setBufferSelection(item);
  };

  closeContextMenu = () => {
    this.bufferSelection = null;
  };

  createRoom = (data) => {
    const isInThirdparty = !!data.storageLocation;
    return isInThirdparty
      ? api.rooms.createRoomInThirdpary(data.storageLocation.id, data)
      : api.rooms.createRoom(data);
  };

  deleteRoom = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.deleteRoom(selectedRoom.id);
  };

  pinRoom = (id) => {
    return api.rooms.pinRoom(id);
  };

  unpinRoom = (id) => {
    return api.rooms.unpinRoom(id);
  };

  moveToArchive = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.archiveRoom(selectedRoom.id);
  };

  moveFromArchive = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.unarchiveRoom(selectedRoom.id);
  };

  fetchTags = () => {
    const request = () =>
      api.rooms.getTags().then((res) => {
        this.setTags(res);
        return res;
      });

    return request();
  };

  getRoomCheckboxTitle = (t, key) => {
    switch (key) {
      case "all":
        return t("All");
      case RoomsType.FillingFormsRoom:
        return "Filling form rooms";
      case RoomsType.CustomRoom:
        return "Custom rooms";
      case RoomsType.EditingRoom:
        return "Editing rooms";
      case RoomsType.ReviewRoom:
        return "Review rooms";
      case RoomsType.ReadOnlyRoom:
        return "Read-only rooms";
      default:
        return "";
    }
  };

  getRoomChecked = (room, selected) => {
    const type = room.roomType;

    switch (selected) {
      case "all":
        return true;
      case RoomsType.FillingFormsRoom:
        return type === RoomsType.FillingFormsRoom;
      case RoomsType.CustomRoom:
        return type === RoomsType.CustomRoom;
      case RoomsType.EditingRoom:
        return type === RoomsType.EditingRoom;
      case RoomsType.ReviewRoom:
        return type === RoomsType.ReviewRoom;
      case RoomsType.ReadOnlyRoom:
        return type === RoomsType.ReadOnlyRoom;
      default:
        return false;
    }
  };

  get isHeaderVisible() {
    return this.selection.length > 0;
  }

  get isHeaderIndeterminate() {
    return this.isHeaderVisible && this.selection.length
      ? this.selection.length < this.rooms.length
      : false;
  }

  get isHeaderChecked() {
    return this.isHeaderVisible && this.selection.length === this.rooms.length;
  }

  get checkboxMenuItems() {
    let cbMenu = ["all"];

    for (const item of this.rooms) {
      switch (item.roomType) {
        case RoomsType.FillingFormsRoom:
          cbMenu.push(RoomsType.FillingFormsRoom);
          break;
        case RoomsType.CustomRoom:
          cbMenu.push(RoomsType.CustomRoom);
          break;
        case RoomsType.EditingRoom:
          cbMenu.push(RoomsType.EditingRoom);
          break;
        case RoomsType.ReviewRoom:
          cbMenu.push(RoomsType.ReviewRoom);
          break;
        case RoomsType.ReadOnlyRoom:
          cbMenu.push(RoomsType.ReadOnlyRoom);
          break;
      }
    }

    cbMenu = cbMenu.filter((item, index) => cbMenu.indexOf(item) === index);

    return cbMenu;
  }
}

export default RoomsStore;
