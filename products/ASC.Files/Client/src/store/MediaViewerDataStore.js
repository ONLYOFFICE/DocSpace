import { makeAutoObservable } from "mobx";

class MediaViewerDataStore {
  filesStore;
  settingsStore;

  id = null;
  visible = false;
  previewFile = null;

  constructor(filesStore, settingsStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.settingsStore = settingsStore;
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;
  };

  setToPreviewFile = (file, visible) => {
    if (file === null) {
      this.previewFile = null;
      this.id = null;
      this.visible = false;
      return;
    }

    if (!file.canOpenPlayer) return;

    this.previewFile = file;
    this.id = file.id;
    this.visible = visible;
  };

  get playlist() {
    const { isMediaOrImage } = this.settingsStore;
    const { files } = this.filesStore;

    const filesList = [...files];
    const playlist = [];
    let id = 0;

    if (filesList.length > 0) {
      filesList.forEach((file) => {
        const canOpenPlayer = isMediaOrImage(file.fileExst);
        if (canOpenPlayer) {
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
