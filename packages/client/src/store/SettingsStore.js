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
  publicRoomStore;

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
  keepNewFileName = null;
  thumbnails1280x720 = window.DocSpaceConfig?.thumbnails1280x720 || false;
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
  canSearchByContent = false;

  html = [".htm", ".mht", ".html"];
  ebook = [".fb2", ".ibk", ".prc", ".epub"];

  constructor(thirdPartyStore, treeFoldersStore, publicRoomStore) {
    makeAutoObservable(this);

    this.thirdPartyStore = thirdPartyStore;
    this.treeFoldersStore = treeFoldersStore;
    this.publicRoomStore = publicRoomStore;
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

        if (!settings.enableThirdParty || this.publicRoomStore.isPublicRoom)
          return;

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

  setThumbnails1280x720 = (enabled) => {
    this.thumbnails1280x720 = enabled;
  };

  setKeepNewFileName = (data) => {
    api.files
      .changeKeepNewFileName(data)
      .then((res) => this.setFilesSetting("keepNewFileName", res));
  };

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

  canViewedDocs = (extension) =>
    presentInArray(this.extsWebPreviewed, extension);

  canConvert = (extension) => presentInArray(this.extsMustConvert, extension);

  // isMediaOrImage = (fileExst) => { TODO: no need, use the data from item
  //   if (
  //     this.extsVideo.includes(fileExst) ||
  //     this.extsImage.includes(fileExst) ||
  //     this.extsAudio.includes(fileExst)
  //   ) {
  //     return true;
  //   }
  //   return false;
  // };

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
        case RoomsType.PublicRoom:
          path = "public.svg";
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
        break;
      case ".csv":
        path = "csv.svg";
        break;
      case ".djvu":
        path = "djvu.svg";
        break;
      case ".doc":
        path = "doc.svg";
        break;
      case ".docm":
        path = "docm.svg";
        break;
      case ".docx":
        path = "docx.svg";
        break;
      case ".dotx":
        path = "dotx.svg";
        break;
      case ".dvd":
        path = "dvd.svg";
        break;
      case ".epub":
        path = "epub.svg";
        break;
      case ".pb2":
      case ".fb2":
        path = "fb2.svg";
        break;
      case ".flv":
        path = "flv.svg";
        break;
      case ".fodt":
        path = "fodt.svg";
        break;
      case ".iaf":
        path = "iaf.svg";
        break;
      case ".ics":
        path = "ics.svg";
        break;
      case ".m2ts":
        path = "m2ts.svg";
        break;
      case ".mht":
        path = "mht.svg";
        break;
      case ".mkv":
        path = "mkv.svg";
        break;
      case ".mov":
        path = "mov.svg";
        break;
      case ".mp4":
        path = "mp4.svg";
        break;
      case ".mpg":
        path = "mpg.svg";
        break;
      case ".odp":
        path = "odp.svg";
        break;
      case ".ods":
        path = "ods.svg";
        break;
      case ".odt":
        path = "odt.svg";
        break;
      case ".otp":
        path = "otp.svg";
        break;
      case ".ots":
        path = "ots.svg";
        break;
      case ".ott":
        path = "ott.svg";
        break;
      case ".pdf":
        path = "pdf.svg";
        break;
      case ".pot":
        path = "pot.svg";
        break;
      case ".pps":
        path = "pps.svg";
        break;
      case ".ppsx":
        path = "ppsx.svg";
        break;
      case ".ppt":
        path = "ppt.svg";
        break;
      case ".pptm":
        path = "pptm.svg";
        break;
      case ".pptx":
        path = "pptx.svg";
        break;
      case ".rtf":
        path = "rtf.svg";
        break;
      case ".svg":
        path = "svg.svg";
        break;
      case ".txt":
        path = "txt.svg";
        break;
      case ".webm":
        path = "webm.svg";
        break;
      case ".xls":
        path = "xls.svg";
        break;
      case ".xlsm":
        path = "xlsm.svg";
        break;
      case ".xlsx":
        path = "xlsx.svg";
        break;
      case ".xps":
        path = "xps.svg";
        break;
      case ".xml":
        path = "xml.svg";
        break;
      case ".oform":
        path = "oform.svg";
        break;
      case ".docxf":
        path = "docxf.svg";
        break;
      default:
        path = "file.svg";
        break;
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
