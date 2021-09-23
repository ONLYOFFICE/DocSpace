import { makeObservable, observable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

class MediaViewersFormatsStore {
  images = [
    ".bmp",
    ".gif",
    ".ico",
    ".jpeg",
    ".jpg",
    ".png",
    ".svg",
    ".tif",
    ".tiff",
    ".webp",
  ];
  media = [
    ".aac",
    ".avi",
    ".f4v",
    ".flac",
    ".m4v",
    ".mov",
    ".mp3",
    ".mp4",
    ".mpeg",
    ".mpg",
    ".oga",
    ".ogg",
    ".ogv",
    ".wav",
    ".webm",
    ".wmv",
  ];

  constructor() {
    makeObservable(this, {
      images: observable,
      media: observable,
    });
  }

  isVideo = (extension) => {
    return presentInArray(this.media, extension);
  };

  isImage = (extension) => {
    return presentInArray(this.images, extension);
  };

  isMediaOrImage = (fileExst) => {
    if (this.media.includes(fileExst) || this.images.includes(fileExst)) {
      return true;
    }
    return false;
  };
}

export default new MediaViewersFormatsStore();
