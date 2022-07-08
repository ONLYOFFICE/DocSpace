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
    this.roomsStore = roomsStore;

    makeAutoObservable(this);
  }

  onSelectTag = (tag) => {
    const { filterRooms, filter } = this.roomsStore;

    const tags = filter.tags ? [...filter.tags] : [];

    if (tags.length > 0) {
      const idx = tags.findIndex((item) => item === tag);

      if (idx > -1) {
        //TODO: remove tag here if already selected
        return;
      } else {
        tags.push(tag);
      }
    } else {
      tags.push(tag);
    }

    filterRooms(null, null, tags);
  };
}

export default RoomsActionsStore;
