import { makeAutoObservable, runInAction } from "mobx";
import { AppServerConfig, FolderType } from "@appserver/common/constants";

import toastr from "studio/toastr";

// import history from "@appserver/common/history";

import { combineUrl } from "@appserver/common/utils";

import config from "../../package.json";

import FilesFilter from "@appserver/common/api/files/filter";

class RoomsActionsStore {
  roomsStore = null;
  filesStore = null;

  constructor(roomsStore, filesStore) {
    this.roomsStore = roomsStore;
    this.filesStore = filesStore;

    makeAutoObservable(this);
  }

  onOpenRoom = (e, id, history) => {
    if (
      e.detail === 1 &&
      id &&
      !e.target.closest(".tag") &&
      !e.target.closest(".room-logo_icon-container") &&
      !e.target.closest(".room-logo_checkbox") &&
      e.target.nodeName !== "IMG" &&
      e.target.nodeName !== "INPUT" &&
      e.target.nodeName !== "rect" &&
      e.target.nodeName !== "path" &&
      e.target.nodeName !== "svg"
    ) {
      const { setIsLoading, fetchFiles } = this.filesStore;

      setIsLoading(true);

      fetchFiles(id, null, true, false)
        .then(async () => {
          const filter = FilesFilter.getDefault();

          filter.folder = id;

          const urlFilter = filter.toUrlParams();

          history.push(
            combineUrl(
              AppServerConfig.proxyURL,
              config.homepage,
              `/filter?${urlFilter}`
            )
          );
        })
        .catch((err) => toastr.error(err))
        .finally(() => {
          setIsLoading(false);
        });
    }
  };

  getHeaderMenu = (t) => {
    const { selection } = this.roomsStore;

    if (selection.length === 0) return;

    const pinOption =
      selection.findIndex((room) => !room.pinned) > -1
        ? this.getOption("pin", t)
        : this.getOption("unpin", t);

    const archiveOption =
      selection.findIndex((room) => room.rootFolderType === FolderType.Rooms) >
      -1
        ? this.getOption("archive", t)
        : this.getOption("unarchive", t);

    return [pinOption, archiveOption];
  };

  getOption = (option, t) => {
    switch (option) {
    }
  };

  onSelectTag = (tag) => {
    const { filterRooms, filter } = this.roomsStore;

    const tags = filter.tags ? [...filter.tags] : [];

    if (tags.length > 0) {
      const idx = tags.findIndex((item) => item === tag);

      if (idx > -1) {
        //TODO: remove tag here if already selected
        return;
      }
    } else {
      tags.push(tag);
    }

    filterRooms(null, null, tags);
  };
}

export default RoomsActionsStore;
