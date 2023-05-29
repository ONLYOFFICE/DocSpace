import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import { ValidationResult } from "../helpers/constants";

class PublicRoomStore {
  externalLinks = [];
  roomTitle = null;
  roomId = null;
  roomStatus = null;
  roomType = null;
  publicKey = null;

  isLoaded = false;
  isLoading = false;

  constructor() {
    makeAutoObservable(this);
  }

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setRoomData = (data) => {
    const { id, roomType, status, title } = data;

    this.roomTitle = title;
    this.roomId = id;
    this.roomStatus = status;
    this.roomType = roomType;

    if (status === ValidationResult.Ok) this.isLoaded = true;
  };

  getExternalLinks = async (roomId) => {
    const type = 1;
    const externalLinks = await api.rooms.getExternalLinks(roomId, type);

    this.externalLinks = externalLinks;
  };

  setExternalLink = (linkId, data) => {
    const linkIndex = this.externalLinks.findIndex(
      (l) => l.sharedTo.id === linkId
    );
    const dataLink = data.find((l) => l.sharedTo.id === linkId);
    this.externalLinks[linkIndex] = dataLink;
  };

  setExternalLinks = (links) => {
    const externalLinks = links.filter((t) => t.sharedTo.shareLink); // shareLink

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
    this.setIsLoading(true);
    api.rooms
      .validatePublicRoomKey(key)
      .then((res) => {
        this.publicKey = key;
        this.setRoomData(res);
      })
      .finally(() => this.setIsLoading(false));
  };

  validatePublicRoomPassword = (key, passwordHash) => {
    return api.rooms.validatePublicRoomPassword(key, passwordHash);
  };

  get isPublicRoom() {
    return this.isLoaded;
  }

  get roomLinks() {
    const a = this.externalLinks.filter(
      (l) => l.sharedTo.shareLink && !l.sharedTo.isTemplate
    );

    return this.externalLinks.filter(
      (l) => l.sharedTo.shareLink && !l.sharedTo.isTemplate
    );
  }
}

export default PublicRoomStore;
