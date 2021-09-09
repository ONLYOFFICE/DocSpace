import { makeAutoObservable } from "mobx";

class MediaViewerDataStore {
  filesStore;

  id = null;
  visible = false;
  previewFile = null;

  constructor(filesStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;
  };

  setToPreviewFile = (file, visible) => {
    if (!file.canOpenPlayer) return;
    this.previewFile = file;
    this.id = file.id;
    this.visible = visible;
  };

  get playlist() {
    const playlist = [];
    let id = 0;

    if (this.filesStore.filesList.length > 0) {
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
    } else if (this.previewFile) {
      playlist.push({
        id: id,
        fileId: this.previewFile.id,
        src: this.previewFile.viewUrl,
        title: this.previewFile.title,
      });
    }
    return playlist;
  }
}

export default MediaViewerDataStore;
