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

  openContextMenu = (item) => {
    if (this.selection.length > 0) return;

    this.setBufferSelection(item);
  };

  closeContextMenu = () => {
    this.bufferSelection = null;
  };

  createRoom = (title = "Room 4", roomType = RoomsType.ReadOnlyRoom) => {
    const data = { title, roomType };

    return api.rooms.createRoom(data);
  };

  deleteRoom = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.deleteRoom(selectedRoom.id);
  };

  pinRoom = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.pinRoom(selectedRoom.id);
  };

  unpinRoom = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    return api.rooms.unpinRoom(selectedRoom.id);
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
}

export default RoomsStore;
