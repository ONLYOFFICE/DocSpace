import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";

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
    providerId
  ) => {
    return api.files.saveThirdParty(
      url,
      login,
      password,
      token,
      isCorporate,
      customerTitle,
      providerKey,
      providerId
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
      newWindow = modal ? newWindow : window.open(url, "Authorization", params);
    } catch (err) {
      newWindow = modal ? newWindow : window.open(url, "Authorization");
    }

    return newWindow;
  };

  openConnectWindow = (serviceName, modal) => {
    const service = this.convertServiceName(serviceName);
    return api.files.openConnectWindow(service).then((link) => {
      return this.oAuthPopup(link, modal);
    });
  };

  getThirdPartyIcon = (iconName) => {
    switch (iconName) {
      case "Box":
        return "images/icon_box.react.svg";
      case "DropboxV2":
        return "images/icon_dropbox.react.svg";
      case "GoogleDrive":
        return "images/icon_google_drive.react.svg";
      case "OneDrive":
        return "images/icon_onedrive.react.svg";
      case "SharePoint":
        return "images/icon_sharepoint.react.svg";
      case "kDrive":
        return "images/icon_kdrive.react.svg";
      case "Yandex":
        return "images/icon_yandex_disk.react.svg";
      case "OwnCloud":
        return "images/icon_owncloud.react.svg";
      case "NextCloud":
        return "images/icon_nextcloud.react.svg";
      case "OneDriveForBusiness":
        return "images/icon_onedrive.react.svg";
      case "WebDav":
        return "images/icon_webdav.react.svg";

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
}

export default new ThirdPartyStore();
