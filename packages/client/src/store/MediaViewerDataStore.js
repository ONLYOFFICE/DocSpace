import { makeAutoObservable } from "mobx";

class MediaViewerDataStore {
  filesStore;
  settingsStore;

  id = null;
  visible = false;
  previewFile = null;
  currentItem = null;

  constructor(filesStore, settingsStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.settingsStore = settingsStore;
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;

    if (!mediaData.visible) this.setCurrentItem(null);
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

  setCurrentItem = (item) => {
    this.currentItem = item;
  };

  setCurrentId = (id) => {
    this.id = id;
  };

  get playlist() {
    const { files } = this.filesStore;

    const filesList = [...files];
    const playlist = [];
    let id = 0;

    if (this.currentItem) {
      playlist.push({
        id: id,
        fileId: this.currentItem.fileId,
        src: this.currentItem.fileInfo.viewUrl,
        title: this.currentItem.fileInfo.title,
      });

      return playlist;
    }

    if (filesList.length > 0) {
      filesList.forEach((file) => {
        const canOpenPlayer =
          file.viewAccessability.ImageView || file.viewAccessability.MediaView;
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
