import { makeAutoObservable, runInAction } from "mobx";
import api from "@appserver/common/api";
import {
  AppServerConfig,
  RoomsType,
  FolderType,
} from "@appserver/common/constants";

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
    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeAutoObservable(this);

    this.authStore = authStore;
    this.settingsStore = settingsStore;
    this.userStore = userStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesSettingsStore = filesSettingsStore;
  }

  setRooms = (rooms) => {
    this.rooms = rooms;
  };

  setRoomsFilter = (filter) => {
    this.filter = filter;
  };

  fetchRooms = (
    folderId,
    filter,
    clearFilter = true,
    withSubfolders = false
  ) => {
    const { setSelectedNode } = this.treeFoldersStore;

    const filterData = filter ? filter.clone() : RoomsFilter.getDefault();

    if (folderId) {
      setSelectedNode([folderId + ""]);
    }

    const request = () =>
      api.rooms
        .getRooms(filterData)
        .then(async (data) => {
          const id = data.current.id;

          if (!folderId) {
            setSelectedNode([id + ""]);
          }

          filterData.total = data.total;

          if (data.total > 0) {
            const lastPage = filterData.getLastPage();

            if (filterData.page > lastPage) {
              filterData.page = lastPage;

              return this.fetchFiles(
                folderId,
                filterData,
                clearFilter,
                withSubfolders
              );
            }
          }

          this.setFilterUrl(filterData);

          runInAction(() => {
            this.setRooms(data.folders);
          });

          const navigationPath = await Promise.all(
            data.pathParts.map(async (folder) => {
              const data = await api.files.getFolderInfo(folder);

              return { id: folder, title: data.title };
            })
          ).then((res) => {
            return res
              .filter((item, index) => index !== res.length - 1)
              .reverse();
          });

          this.selectedFolderStore.setSelectedFolder({
            folders: data.folders,
            ...data.current,
            pathParts: data.pathParts,
            navigationPath: navigationPath,
            ...{ new: data.new },
          });
        })
        .catch((err) => {
          toastr.error(err);
        });

    return request();
  };

  setFilterUrl = (filter) => {
    const urlFilter = filter.toUrlParams();
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/rooms?${urlFilter}`
      )
    );
  };

  selectRoom = (checked, item) => {
    this.bufferSelection = null;

    if (checked) {
      this.selection.push(item);
    } else {
      const idx = this.selection.findIndex((room) => room.id === item.id);

      this.selection.splice(idx, 1);
    }
  };

  openContextMenu = (item) => {
    this.bufferSelection = item;
  };

  closeContextMenu = () => {
    this.bufferSelection = null;
  };

  createRoom = (e, title = "Room 4", roomType = RoomsType.ReadOnlyRoom) => {
    const data = { title, roomType };

    const request = () =>
      api.rooms.createRoom(data).then((res) => {
        this.fetchRooms(null, this.filter).then(() => {
          toastr.success(`${res.title} was successful create`);
        });
      });

    return request();
  };

  pinRoom = () => {
    const selectedRoom =
      this.selection.length > 0 ? this.selection[0] : this.bufferSelection;

    if (selectedRoom.pinned) {
      return this.unpinRoom();
    }

    const request = () =>
      api.rooms.pinRoom(selectedRoom.id).then((res) => {
        this.fetchRooms(null, this.filter).then(() => {
          toastr.success(`${selectedRoom.title} was successful pin`);
        });
      });

    return request();
  };

  unpinRoom = (room) => {
    const selectedRoom = room
      ? room
      : this.selection.length > 0
      ? this.selection[0]
      : this.bufferSelection;

    if (!selectedRoom.pinned) {
      return this.pinRoom();
    }

    const request = () =>
      api.rooms.unpinRoom(selectedRoom.id).then((res) => {
        this.fetchRooms(null, this.filter).then(() => {
          toastr.success(`${selectedRoom.title} was successful unpin`);
        });
      });

    return request();
  };

  deleteRoom = () => {
    const selectedRoom =
      this.selection.length > 0 ? this.selection[0] : this.bufferSelection;

    const request = () =>
      api.rooms.deleteRoom(selectedRoom.id).then((res) => {
        //TODO: change setTimeout
        setTimeout(() => {
          this.fetchRooms(null, this.filter).then(() => {
            toastr.success(`${selectedRoom.title} was successful delete`);
          });
        }, 500);
      });

    return request();
  };
}

export default RoomsStore;
