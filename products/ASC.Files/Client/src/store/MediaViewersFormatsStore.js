import { makeObservable } from "mobx";

class MediaViewersFormatsStore {
  images = [
    ".bmp",
    ".gif",
    ".jpeg",
    ".jpg",
    ".png",
    ".ico",
    ".tif",
    ".tiff",
    ".webp",
  ];
  media = [
    ".aac",
    ".flac",
    ".m4a",
    ".mp3",
    ".oga",
    ".ogg",
    ".wav",
    ".f4v",
    ".m4v",
    ".mov",
    ".mp4",
    ".ogv",
    ".webm",
    ".avi",
    ".mpg",
    ".mpeg",
    ".wmv",
  ];

  constructor() {
    makeObservable(this, {});
  }

  isMediaOrImage = (fileExst) => {
    if (this.media.includes(fileExst) || this.images.includes(fileExst)) {
      return true;
    }
    return false;
  };
}

export default MediaViewersFormatsStore;
