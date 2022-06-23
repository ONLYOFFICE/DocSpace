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
        `/filter?${urlFilter}`
      )
    );
  };
}

export default RoomsStore;
