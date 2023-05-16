import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";

class PublicRoomStore {
  externalLinks = [];
  roomTitle = null;
  roomId = null;
  isLoaded = false;

  constructor() {
    makeAutoObservable(this);
  }

  setRoomData = (data) => {
    const { id, roomType, status, title } = data;

    this.roomTitle = title;
    this.roomId = id;
    this.isLoaded = true;
  };

  getExternalLinks = async (roomId) => {
    const type = 1;
    const externalLinks = await api.rooms.getExternalLinks(roomId, type);
    // this.externalLinks = externalLinks;
  };

  setExternalLink = (linkId, data) => {
    const linkIndex = this.externalLinks.findIndex(
      (l) => l.sharedTo.id === linkId
    );
    const dataLink = data.find((l) => l.sharedTo.id === linkId);
    this.externalLinks[linkIndex] = dataLink;
  };

  setExternalLinks = (links) => {
    const externalLinks = links.filter((t) => t.sharedTo.shareLink);
    this.externalLinks = externalLinks;
  };

  editExternalLink = (options) => {
    const {
      roomId,
      linkId,
      title,
      access = 2,
      expirationDate,
      linkType = 1,
      password,
      disabled,
      denyDownload,
    } = options;

    return api.rooms.editExternalLink(
      roomId,
      linkId,
      title,
      access,
      expirationDate,
      linkType,
      password,
      disabled,
      denyDownload
    );
  };

  validatePublicRoomKey = (key) => {
    return api.rooms.validatePublicRoomKey(key);
  };

  validatePublicRoomPassword = (key, password) => {
    return api.rooms.validatePublicRoomPassword(key, password);
  };
}

export default PublicRoomStore;
