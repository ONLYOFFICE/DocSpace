import { makeAutoObservable } from "mobx";

class MediaViewerDataStore {
  filesStore;

  id = null;
  visible = false;

  constructor(filesStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;
  };

  get playlist() {
    const playlist = [];
    let id = 0;

    if (this.filesStore.filesList) {
      this.filesStore.filesList.forEach((file) => {
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

export default MediaViewerDataStore;
