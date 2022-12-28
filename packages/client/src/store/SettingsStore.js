import api from "@docspace/common/api";
import {
  setFavoritesSetting,
  setRecentSetting,
} from "@docspace/common/api/files";
import { RoomsType } from "@docspace/common/constants";
import axios from "axios";
import { makeAutoObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";
import {
  iconSize24,
  iconSize32,
  iconSize64,
  iconSize96,
} from "@docspace/common/utils/image-helpers";

class SettingsStore {
  thirdPartyStore;
  treeFoldersStore;

  isErrorSettings = null;
  expandedSetting = null;

  confirmDelete = null;
  enableThirdParty = null;
  forcesave = null;
  storeForcesave = null;
  storeOriginalFiles = null;
  updateIfExist = null;
  favoritesSection = null;
  recentSection = null;
  hideConfirmConvertSave = null;
  chunkUploadSize = 1024 * 1023; // 1024 * 1023; //~0.999mb

  settingsIsLoaded = false;

  extsImagePreviewed = [];
  extsMediaPreviewed = [];
  extsWebPreviewed = [];
  extsWebEdited = [];
  extsWebEncrypt = [];
  extsWebReviewed = [];
  extsWebCustomFilterEditing = [];
  extsWebRestrictedEditing = [];
  extsWebCommented = [];
  extsWebTemplate = [];
  extsCoAuthoring = [];
  extsMustConvert = [];
  extsConvertible = [];
  extsUploadable = [];
  extsArchive = [];
  extsVideo = [];
  extsAudio = [];
  extsImage = [];
  extsSpreadsheet = [];
  extsPresentation = [];
  extsDocument = [];
  internalFormats = {};
  masterFormExtension = "";

  html = [".htm", ".mht", ".html"];
  ebook = [".fb2", ".ibk", ".prc", ".epub"];

  constructor(thirdPartyStore, treeFoldersStore) {
    makeAutoObservable(this);

    this.thirdPartyStore = thirdPartyStore;
    this.treeFoldersStore = treeFoldersStore;
  }

  setIsLoaded = (isLoaded) => {
    this.settingsIsLoaded = isLoaded;
  };

  get isLoadedSettingsTree() {
    return (
      this.confirmDelete !== null &&
      this.enableThirdParty !== null &&
      this.forcesave !== null &&
      this.storeForcesave !== null &&
      this.storeOriginalFiles !== null &&
      this.updateIfExist !== null
    );
  }

  setFilesSettings = (settings) => {
    const settingsItems = Object.keys(settings);
    for (let key of settingsItems) {
      this[key] = settings[key];
    }
  };

  setIsErrorSettings = (isError) => {
    this.isErrorSettings = isError;
  };

  setExpandSettingsTree = (expandedSetting) => {
    this.expandedSetting = expandedSetting;
  };

  getFilesSettings = () => {
    if (this.isLoadedSettingsTree) return Promise.resolve();

    return api.files
      .getSettingsFiles()
      .then((settings) => {
        this.setFilesSettings(settings);
        this.setIsLoaded(true);

        if (!settings.enableThirdParty) return;

        return axios
          .all([
            api.files.getThirdPartyCapabilities(),
            api.files.getThirdPartyList(),
          ])
          .then(([capabilities, providers]) => {
            for (let item of capabilities) {
              item.splice(1, 1);
            }
            this.thirdPartyStore.setThirdPartyCapabilities(capabilities); //TODO: Out of bounds read: 1
            this.thirdPartyStore.setThirdPartyProviders(providers);
          });
      })
      .catch(() => this.setIsErrorSettings(true));
  };

  setFilesSetting = (setting, val) => {
    this[setting] = val;
  };

  setUpdateIfExist = (data, setting) =>
    api.files
      .updateIfExist(data)
      .then((res) => this.setFilesSetting(setting, res));

  setStoreOriginal = (data, setting) =>
    api.files
      .storeOriginal(data)
      .then((res) => this.setFilesSetting(setting, res));

  setConfirmDelete = (data, setting) =>
    api.files
      .changeDeleteConfirm(data)
      .then((res) => this.setFilesSetting(setting, res));

  setStoreForceSave = (data) =>
    api.files.storeForceSave(data).then((res) => this.setStoreForcesave(res));

  setStoreForcesave = (val) => (this.storeForcesave = val);

  setEnableThirdParty = async (data, setting) => {
    const res = await api.files.thirdParty(data);
    this.setFilesSetting(setting, res);

    if (data) {
      return axios
        .all([
          api.files.getThirdPartyCapabilities(),
          api.files.getThirdPartyList(),
        ])
        .then(([capabilities, providers]) => {
          for (let item of capabilities) {
            item.splice(1, 1);
          }
          this.thirdPartyStore.setThirdPartyCapabilities(capabilities); //TODO: Out of bounds read: 1
          this.thirdPartyStore.setThirdPartyProviders(providers);
        });
    } else {
      return Promise.resolve();
    }
  };

  setForceSave = (data) =>
    api.files.forceSave(data).then((res) => this.setForcesave(res));

  setForcesave = (val) => (this.forcesave = val);

  updateRootTreeFolders = () => {
    const { getFoldersTree, setTreeFolders } = this.treeFoldersStore;
    getFoldersTree().then((root) => setTreeFolders(root));
  };

  setFavoritesSetting = (set, setting) => {
    return setFavoritesSetting(set).then((res) => {
      this.setFilesSetting(setting, res);
      this.updateRootTreeFolders();
    });
  };

  setRecentSetting = (set, setting) => {
    return setRecentSetting(set).then((res) => {
      this.setFilesSetting(setting, res);
      this.updateRootTreeFolders();
    });
  };

  hideConfirmConvert = async (save = true) => {
    const hideConfirmConvertSave = await api.files.hideConfirmConvert(save);
    this.hideConfirmConvertSave = hideConfirmConvertSave;
  };

  canConvert = (extension) => presentInArray(this.extsMustConvert, extension);

  isArchive = (extension) => presentInArray(this.extsArchive, extension);

  isImage = (extension) => presentInArray(this.extsImage, extension);

  isSound = (extension) => presentInArray(this.extsAudio, extension);

  isHtml = (extension) => presentInArray(this.html, extension);

  isEbook = (extension) => presentInArray(this.ebook, extension);

  isDocument = (extension) => presentInArray(this.extsDocument, extension);

  isMasterFormExtension = (extension) => this.masterFormExtension === extension;

  isPresentation = (extension) =>
    presentInArray(this.extsPresentation, extension);

  isSpreadsheet = (extension) =>
    presentInArray(this.extsSpreadsheet, extension);

  getIcon = (
    size = 24,
    fileExst = null,
    providerKey = null,
    contentLength = null,
    roomType = null,
    isArchive = null
  ) => {
    if (fileExst || contentLength) {
      const isArchiveItem = this.isArchive(fileExst);
      const isImageItem = this.isImage(fileExst);
      const isSoundItem = this.isSound(fileExst);
      const isHtmlItem = this.isHtml(fileExst);

      const icon = this.getFileIcon(
        fileExst,
        size,
        isArchiveItem,
        isImageItem,
        isSoundItem,
        isHtmlItem
      );
      return icon;
    } else if (roomType) {
      return this.getRoomsIcon(roomType, isArchive, 32);
    } else {
      return this.getFolderIcon(providerKey, size);
    }
  };

  getIconBySize = (size, path) => {
    switch (+size) {
      case 24:
        return iconSize24.get(path);
      case 32:
        return iconSize32.get(path);
      case 64:
        return iconSize64.get(path);
      case 96:
        return iconSize96.get(path);
    }
  };

  getRoomsIcon = (roomType, isArchive, size = 32) => {
    let path = "";

    if (isArchive) {
      path = "archive.svg";
    } else {
      switch (roomType) {
        case RoomsType.CustomRoom:
          path = "custom.svg";
          break;
        case RoomsType.FillingFormsRoom:
          path = "filling.form.svg";
          break;
        case RoomsType.EditingRoom:
          path = "editing.svg";
          break;
        case RoomsType.ReadOnlyRoom:
          path = "view.only.svg";
          break;
        case RoomsType.ReviewRoom:
          path = "review.svg";
          break;
      }
    }

    return this.getIconBySize(size, path);
  };

  getFolderIcon = (providerKey, size = 32) => {
    let path = "";

    switch (providerKey) {
      case "Box":
      case "BoxNet":
        path = "box.svg";
        break;
      case "DropBox":
      case "DropboxV2":
        path = "dropbox.svg";
        break;
      case "Google":
      case "GoogleDrive":
        path = "google.svg";
        break;
      case "OneDrive":
        path = "onedrive.svg";
        break;
      case "SharePoint":
        path = "sharepoint.svg";
        break;
      case "Yandex":
        path = "yandex.svg";
        break;
      case "kDrive":
        path = "kdrive.svg";
        break;
      case "WebDav":
        path = "webdav.svg";
        break;
      default:
        path = "folder.svg";
        break;
    }

    return this.getIconBySize(size, path);
  };

  getIconUrl = (extension, size) => {
    let path = "";

    switch (extension) {
      case ".avi":
        path = "avi.svg";
      case ".csv":
        path = "csv.svg";
      case ".djvu":
        path = "djvu.svg";
      case ".doc":
        path = "doc.svg";
      case ".docm":
        path = "docm.svg";
      case ".docx":
        path = "docx.svg";
      case ".dotx":
        path = "dotx.svg";
      case ".dvd":
        path = "dvd.svg";
      case ".epub":
        path = "epub.svg";
      case ".pb2":
      case ".fb2":
        path = "fb2.svg";
      case ".flv":
        path = "flv.svg";
      case ".fodt":
        path = "fodt.svg";
      case ".iaf":
        path = "iaf.svg";
      case ".ics":
        path = "ics.svg";
      case ".m2ts":
        path = "m2ts.svg";
      case ".mht":
        path = "mht.svg";
      case ".mkv":
        path = "mkv.svg";
      case ".mov":
        path = "mov.svg";
      case ".mp4":
        path = "mp4.svg";
      case ".mpg":
        path = "mpg.svg";
      case ".odp":
        path = "odp.svg";
      case ".ods":
        path = "ods.svg";
      case ".odt":
        path = "odt.svg";
      case ".otp":
        path = "otp.svg";
      case ".ots":
        path = "ots.svg";
      case ".ott":
        path = "ott.svg";
      case ".pdf":
        path = "pdf.svg";
      case ".pot":
        path = "pot.svg";
      case ".pps":
        path = "pps.svg";
      case ".ppsx":
        path = "ppsx.svg";
      case ".ppt":
        path = "ppt.svg";
      case ".pptm":
        path = "pptm.svg";
      case ".pptx":
        path = "pptx.svg";
      case ".rtf":
        path = "rtf.svg";
      case ".svg":
        path = "svg.svg";
      case ".txt":
        path = "txt.svg";
      case ".webm":
        path = "webm.svg";
      case ".xls":
        path = "xls.svg";
      case ".xlsm":
        path = "xlsm.svg";
      case ".xlsx":
        path = "xlsx.svg";
      case ".xps":
        path = "xps.svg";
      case ".xml":
        path = "xml.svg";
      case ".oform":
        path = "oform.svg";
      case ".docxf":
        path = "docxf.svg";
      default:
        path = "file.svg";
    }

    return this.getIconBySize(size, path);
  };

  getFileIcon = (
    extension,
    size = 32,
    archive = false,
    image = false,
    sound = false,
    html = false
  ) => {
    let path = "";

    if (archive) path = "file_archive.svg";

    if (image) path = "image.svg";

    if (sound) path = "sound.svg";

    if (html) path = "html.svg";

    if (path) return this.getIconBySize(size, path);

    return this.getIconUrl(extension, size);
  };

  getIconSrc = (ext, size = 24) => {
    let path = "";

    if (presentInArray(this.extsArchive, ext, true)) path = "file_archive.svg";

    if (presentInArray(this.extsImage, ext, true)) path = "image.svg";

    if (presentInArray(this.extsAudio, ext, true)) path = "sound.svg";

    if (presentInArray(this.html, ext, true)) path = "html.svg";

    if (path) return this.getIconBySize(size, path);

    const extension = ext.toLowerCase();

    return this.getIconUrl(extension, size);
  };
}

export default SettingsStore;
