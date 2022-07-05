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

class RoomsActionsStore {
  roomsStore = null;

  constructor(roomsStore) {
    makeAutoObservable();

    this.roomsStore = rooms;
  }
}

export default RoomsActionsStore;
