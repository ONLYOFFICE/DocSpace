import { makeAutoObservable } from "mobx";

import api from "@appserver/common/api";

class FilesActionStore {
  filesStore;

  constructor(filesStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
  }

  createThumbnails = () => {
    const filesList = this.filesStore.filesList;
    const fileIds = [];
    const re = /\d*$/;

    filesList.map((file) => {
      if (
        !file.thumbnailUrl ||
        (file.thumbnailUrl && file.thumbnailUrl.match(re)[0] != file.version)
      ) {
        fileIds.push(file.id);
      }
    });

    return api.files.createThumbnails(fileIds);
  };
}

export default FilesActionStore;
