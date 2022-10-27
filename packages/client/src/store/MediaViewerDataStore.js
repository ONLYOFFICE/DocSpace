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
    const { isMediaOrImage } = this.settingsStore;
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
        fileExst: this.currentItem.fileInfo.fileExst,
        fileStatus: this.currentItem.fileInfo.fileStatus,
        canShare: this.currentItem.fileInfo.canShare,
      });

      return playlist;
    }

    if (filesList.length > 0) {
      filesList.forEach((file) => {
        const canOpenPlayer = isMediaOrImage(file.fileExst);
        if (canOpenPlayer) {
          playlist.push({
            id: id,
            fileId: file.id,
            src: file.viewUrl,
            title: file.title,
            fileExst: file.fileExst,
            fileStatus: file.fileStatus,
            canShare: file.canShare,
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
        fileExst: this.previewFile.fileExst,
        fileStatus: this.previewFile.fileStatus,
        canShare: this.previewFile.canShare,
      });
    }

    return playlist;
  }
}

export default MediaViewerDataStore;
