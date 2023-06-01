import IconBoxSmallReactSvgUrl from "PUBLIC_DIR/images/icon_box_small.react.svg?url";
import IconBoxReactSvgUrl from "PUBLIC_DIR/images/icon_box.react.svg?url";
import IconDropboxSmallReactSvgUrl from "PUBLIC_DIR/images/icon_dropbox_small.react.svg?url";
import IconDropboxReactSvgUrl from "PUBLIC_DIR/images/icon_dropbox.react.svg?url";
import IconGoogleDriveSmallReactSvgUrl from "PUBLIC_DIR/images/icon_google_drive_small.react.svg?url";
import IconGoogleDriveReactSvgUrl from "PUBLIC_DIR/images/icon_google_drive.react.svg?url";
import IconOnedriveSmallReactSvgUrl from "PUBLIC_DIR/images/icon_onedrive_small.react.svg?url";
import IconOnedriveReactSvgUrl from "PUBLIC_DIR/images/icon_onedrive.react.svg?url";
import IconSharepointSmallReactSvgUrl from "PUBLIC_DIR/images/icon_sharepoint_small.react.svg?url";
import IconSharepointReactSvgUrl from "PUBLIC_DIR/images/icon_sharepoint.react.svg?url";
import IconKdriveSmallReactSvgUrl from "PUBLIC_DIR/images/icon_kdrive_small.react.svg?url";
import IconKdriveReactSvgUrl from "PUBLIC_DIR/images/icon_kdrive.react.svg?url";
import IconYandexDiskSmallReactSvgUrl from "PUBLIC_DIR/images/icon_yandex_disk_small.react.svg?url";
import IconYandexDiskReactSvgUrl from "PUBLIC_DIR/images/icon_yandex_disk.react.svg?url";
import IconOwncloudSmallReactSvgUrl from "PUBLIC_DIR/images/icon_owncloud_small.react.svg?url";
import IconOwncloudReactSvgUrl from "PUBLIC_DIR/images/icon_owncloud.react.svg?url";
import IconNextcloudSmallReactSvgUrl from "PUBLIC_DIR/images/icon_nextcloud_small.react.svg?url";
import IconNextcloudReactSvgUrl from "PUBLIC_DIR/images/icon_nextcloud.react.svg?url";
import IconWebdavSmallReactSvgUrl from "PUBLIC_DIR/images/icon_webdav_small.react.svg?url";
import IconWebdavReactSvgUrl from "PUBLIC_DIR/images/icon_webdav.react.svg?url";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import i18n from "../helpers/i18n";

class ThirdPartyStore {
  capabilities = null;
  providers = [];

  constructor() {
    makeAutoObservable(this);
  }

  setThirdPartyProviders = (providers) => {
    this.providers = providers;
  };

  setThirdPartyCapabilities = (capabilities) => {
    this.capabilities = capabilities;
  };

  deleteThirdParty = (id) => api.files.deleteThirdParty(id);

  fetchThirdPartyProviders = async () => {
    const list = await api.files.getThirdPartyList();
    this.setThirdPartyProviders(list);
  };

  saveThirdParty = (
    url,
    login,
    password,
    token,
    isCorporate,
    customerTitle,
    providerKey,
    providerId,
    isRoomsStorage
  ) => {
    return api.files.saveThirdParty(
      url,
      login,
      password,
      token,
      isCorporate,
      customerTitle,
      providerKey,
      providerId,
      isRoomsStorage
    );
  };

  convertServiceName = (serviceName) => {
    //Docusign, OneDrive, Wordpress
    switch (serviceName) {
      case "GoogleDrive":
        return "google";
      case "Box":
        return "box";
      case "DropboxV2":
        return "dropbox";
      case "OneDrive":
        return "onedrive";
      default:
        return "";
    }
  };

  oAuthPopup = (url, modal) => {
    let newWindow = modal;

    if (modal) {
      newWindow.location = url;
    }

    try {
      let params =
        "height=600,width=1020,resizable=0,status=0,toolbar=0,menubar=0,location=1";
      newWindow = modal
        ? newWindow
        : window.open(url, i18n.t("Common:Authorization"), params);
    } catch (err) {
      newWindow = modal
        ? newWindow
        : window.open(url, i18n.t("Common:Authorization"));
    }

    return newWindow;
  };

  openConnectWindow = (serviceName, modal) => {
    const service = this.convertServiceName(serviceName);
    return api.files.openConnectWindow(service).then((link) => {
      return this.oAuthPopup(link, modal);
    });
  };

  getThirdPartyIcon = (iconName, size = "big") => {
    switch (iconName) {
      case "Box":
        if (size === "small") return IconBoxSmallReactSvgUrl;
        return IconBoxReactSvgUrl;
      case "DropboxV2":
        if (size === "small") return IconDropboxSmallReactSvgUrl;
        return IconDropboxReactSvgUrl;
      case "GoogleDrive":
        if (size === "small") return IconGoogleDriveSmallReactSvgUrl;
        return IconGoogleDriveReactSvgUrl;
      case "OneDrive":
        if (size === "small") return IconOnedriveSmallReactSvgUrl;
        return IconOnedriveReactSvgUrl;
      case "SharePoint":
        if (size === "small") return IconSharepointSmallReactSvgUrl;
        return IconSharepointReactSvgUrl;
      case "kDrive":
        if (size === "small") return IconKdriveSmallReactSvgUrl;
        return IconKdriveReactSvgUrl;
      case "Yandex":
        if (size === "small") return IconYandexDiskSmallReactSvgUrl;
        return IconYandexDiskReactSvgUrl;
      case "OwnCloud":
        if (size === "small") return IconOwncloudSmallReactSvgUrl;
        return IconOwncloudReactSvgUrl;
      case "NextCloud":
        if (size === "small") return IconNextcloudSmallReactSvgUrl;
        return IconNextcloudReactSvgUrl;
      case "OneDriveForBusiness":
        if (size === "small") return IconOnedriveSmallReactSvgUrl;
        return IconOnedriveReactSvgUrl;
      case "WebDav":
        if (size === "small") return IconWebdavSmallReactSvgUrl;
        return IconWebdavReactSvgUrl;

      default:
        return "";
    }
  };

  get googleConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "GoogleDrive")
    );
  }

  get boxConnectItem() {
    return this.capabilities && this.capabilities.find((x) => x[0] === "Box");
  }

  get dropboxConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "DropboxV2")
    );
  }
  get oneDriveConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "OneDrive")
    );
  }

  get sharePointConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "SharePoint")
    );
  }

  get kDriveConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "kDrive")
    );
  }

  get yandexConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "Yandex")
    );
  }

  get webDavConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "WebDav")
    );
  }

  // TODO: remove WebDav get NextCloud
  get nextCloudConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "WebDav")
    );
  }
  // TODO:remove WebDav get OwnCloud
  get ownCloudConnectItem() {
    return (
      this.capabilities && this.capabilities.find((x) => x[0] === "WebDav")
    );
  }

  get connectItems() {
    let nextCloudConnectItem = [],
      ownCloudConnectItem = [];

    if (this.nextCloudConnectItem) {
      nextCloudConnectItem.push(...this.nextCloudConnectItem, "Nextcloud");
    }

    if (this.ownCloudConnectItem) {
      ownCloudConnectItem.push(...this.ownCloudConnectItem, "ownCloud");
    }

    const connectItems = [
      this.googleConnectItem,
      this.boxConnectItem,
      this.dropboxConnectItem,
      this.oneDriveConnectItem,
      nextCloudConnectItem,
      this.kDriveConnectItem,
      this.yandexConnectItem,
      ownCloudConnectItem,
      this.webDavConnectItem,
      this.sharePointConnectItem,
    ]
      .map(
        (item) =>
          item && {
            isAvialable: !!item,
            id: item[0],
            providerName: item[0],
            isOauth: item.length > 1 && item[0] !== "WebDav",
            oauthHref: item.length > 1 && item[0] !== "WebDav" ? item[1] : "",
            ...(item[0] === "WebDav" && {
              category: item[item.length - 1],
            }),
          }
      )
      .filter((item) => !!item);

    return connectItems;
  }
}

export default new ThirdPartyStore();
