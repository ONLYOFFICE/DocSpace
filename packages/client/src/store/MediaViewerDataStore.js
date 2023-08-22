import { makeAutoObservable, runInAction } from "mobx";
import {
  isNullOrUndefined,
  findNearestIndex,
  isVideo,
} from "@docspace/common/components/MediaViewer/helpers";
import { thumbnailStatuses } from "SRC_DIR/helpers/filesConstants";

const FirstUrlKey = "isFirstUrl";

class MediaViewerDataStore {
  filesStore;
  settingsStore;
  publicRoomStore;

  id = null;
  visible = false;
  previewFile = null;
  currentItem = null;
  prevPostionIndex = 0;

  constructor(filesStore, settingsStore, publicRoomStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.settingsStore = settingsStore;
    this.publicRoomStore = publicRoomStore;
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

    if (
      !file.canOpenPlayer &&
      !file.fileExst === ".pdf" &&
      window.DocSpaceConfig.pdfViewer
    )
      return;

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

  saveFirstUrl = (url) => {
    localStorage.setItem(FirstUrlKey, url);
  };

  getFirstUrl = () => {
    return localStorage.getItem(FirstUrlKey);
  };

  removeFirstUrl = () => {
    localStorage.removeItem(FirstUrlKey);
  };

  changeUrl = (id) => {
    if (this.publicRoomStore.isPublicRoom) return;

    const url = "/products/files/#preview/" + id;

    window.DocSpace.navigate(url);
  };

  nextMedia = () => {
    const { setBufferSelection, files } = this.filesStore;

    const postionIndex = (this.currentPostionIndex + 1) % this.playlist.length;

    if (postionIndex === 0) {
      return;
    }
    const currentFileId = this.playlist[postionIndex].fileId;

    const targetFile = files.find((item) => item.id === currentFileId);

    if (!isNullOrUndefined(targetFile)) setBufferSelection(targetFile);

    const fileId = this.playlist[postionIndex].fileId;
    this.setCurrentId(fileId);
    this.changeUrl(fileId);
  };

  prevMedia = () => {
    const { setBufferSelection, files } = this.filesStore;

    let currentPlaylistPos = this.currentPostionIndex - 1;

    if (currentPlaylistPos === -1) {
      return;
    }

    const currentFileId = this.playlist[currentPlaylistPos].fileId;

    const targetFile = files.find((item) => item.id === currentFileId);

    if (!isNullOrUndefined(targetFile)) setBufferSelection(targetFile);

    const fileId = this.playlist[currentPlaylistPos].fileId;
    this.setCurrentId(fileId);
    this.changeUrl(fileId);
  };

  get currentPostionIndex() {
    if (this.playlist.length === 0) {
      return 0;
    }

    let index = this.playlist.find((file) => file.fileId === this.id)?.id;

    if (isNullOrUndefined(index)) {
      index = findNearestIndex(this.playlist, this.prevPostionIndex);
    }

    runInAction(() => {
      this.prevPostionIndex = index;
    });

    return index;
  }

  get playlist() {
    const { files } = this.filesStore;

    const filesList = [...files];
    const playlist = [];
    const itemsWithoutThumb = [];
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
        const canOpenPlayer =
          file.viewAccessability.ImageView ||
          file.viewAccessability.MediaView ||
          (file.fileExst === ".pdf" && window.DocSpaceConfig.pdfViewer);

        if (canOpenPlayer) {
          playlist.push({
            id: id,
            fileId: file.id,
            src: file.viewUrl,
            title: file.title,
            fileExst: file.fileExst,
            fileStatus: file.fileStatus,
            canShare: file.canShare,
            version: file.version,
            thumbnailUrl: file.thumbnailUrl,
          });

          const thumbnailIsNotCreated =
            file.thumbnailStatus === thumbnailStatuses.WAITING;

          const isVideoOrImage =
            file.viewAccessability.ImageView || isVideo(file.fileExst);

          if (thumbnailIsNotCreated && isVideoOrImage)
            itemsWithoutThumb.push(file);

          id++;
        }
      });
      if (this.previewFile) {
        runInAction(() => {
          this.previewFile = null;
        });
      }
    } else if (this.previewFile) {
      playlist.push({
        ...this.previewFile,
        id: id,
        fileId: this.previewFile.id,
        src: this.previewFile.viewUrl,
        version: this.previewFile.version,
        thumbnailUrl: this.previewFile.thumbnailUrl,
      });

      if (this.previewFile.viewAccessability.ImageView) {
        itemsWithoutThumb.push(this.previewFile);
      }
    }

    if (itemsWithoutThumb.length > 0) {
      this.filesStore.createThumbnails(itemsWithoutThumb);
    }

    return playlist;
  }
}

export default MediaViewerDataStore;
