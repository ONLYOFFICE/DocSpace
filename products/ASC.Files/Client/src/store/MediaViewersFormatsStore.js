import { makeObservable, observable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

class MediaViewersFormatsStore {
  images = [
    ".ai",
    ".bmp",
    ".cmx",
    ".cod",
    ".gif",
    ".ico",
    ".ief",
    ".jpe",
    ".jpeg",
    ".jpg",
    ".pbm",
    ".png",
    ".pnm",
    ".psd",
    ".rgb",
    ".svg",
    ".tif",
    ".tiff",
    ".webp",
    ".xbm",
    ".xwd",
  ];
  media = [
    ".aac",
    ".ac3",
    ".aiff",
    ".amr",
    ".ape",
    ".avi",
    ".cda",
    ".f4v",
    ".flac",
    ".m4a",
    ".m4v",
    ".mid",
    ".mka",
    ".mov",
    ".mp3",
    ".mp4",
    ".mpc",
    ".mpeg",
    ".mpg",
    ".oga",
    ".ogg",
    ".ogv",
    ".pcm",
    ".ra",
    ".raw",
    ".wav",
    ".webm",
    ".wma",
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

  isMediaOrImage = (fileExst) => {
    if (this.media.includes(fileExst) || this.images.includes(fileExst)) {
      return true;
    }
    return false;
  };
}

export default new MediaViewersFormatsStore();
