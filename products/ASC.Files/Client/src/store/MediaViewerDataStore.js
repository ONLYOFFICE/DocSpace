import { makeAutoObservable } from "mobx";
import filesStore from "./FilesStore";

class MediaViewerDataStore {
  id = null;
  visible = false;

  constructor() {
    makeAutoObservable(this);
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;
  };

  get playlist() {
    const playlist = [];
    let id = 0;

    if (filesStore.filesList) {
      filesStore.filesList.forEach((file) => {
        if (file.canOpenPlayer) {
          playlist.push({
            id: id,
            fileId: file.id,
            src: file.viewUrl,
            title: file.title,
          });
          id++;
        }
      });
    }
    return playlist;
  }
}

export default new MediaViewerDataStore();
