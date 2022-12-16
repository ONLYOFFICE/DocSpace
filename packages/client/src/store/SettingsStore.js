import api from "@docspace/common/api";
import {
  setFavoritesSetting,
  setRecentSetting,
} from "@docspace/common/api/files";
import { RoomsType } from "@docspace/common/constants";
import axios from "axios";
import { makeAutoObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

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

  getRoomsIcon = (roomType, isArchive, size = 32) => {
    const reviewPath = `images/icons/${size}`;

    if (isArchive) return `${reviewPath}/room/archive.svg`;

    switch (roomType) {
      case RoomsType.CustomRoom:
        return `${reviewPath}/room/custom.svg`;
      case RoomsType.FillingFormsRoom:
        return `${reviewPath}/room/filling.form.svg`;
      case RoomsType.EditingRoom:
        return `${reviewPath}/room/editing.svg`;
      case RoomsType.ReadOnlyRoom:
        return `${reviewPath}/room/view.only.svg`;
      case RoomsType.ReviewRoom:
        return `${reviewPath}/room/review.svg`;
    }
  };

  getFolderIcon = (providerKey, size = 32) => {
    const folderPath = `images/icons/${size}`;

    switch (providerKey) {
      case "Box":
      case "BoxNet":
        return `${folderPath}/folder/box.svg`;
      case "DropBox":
      case "DropboxV2":
        return `${folderPath}/folder/dropbox.svg`;
      case "Google":
      case "GoogleDrive":
        return `${folderPath}/folder/google.svg`;
      case "OneDrive":
        return `${folderPath}/folder/onedrive.svg`;
      case "SharePoint":
        return `${folderPath}/folder/sharepoint.svg`;
      case "Yandex":
        return `${folderPath}/folder/yandex.svg`;
      case "kDrive":
        return `${folderPath}/folder/kdrive.svg`;
      case "WebDav":
        return `${folderPath}/folder/webdav.svg`;
      default:
        return `${folderPath}/folder.svg`;
    }
  };

  getIconUrl = (extension, folderPath) => {
    switch (extension) {
      case ".avi":
        return `${folderPath}/avi.svg`;
      case ".csv":
        return `${folderPath}/csv.svg`;
      case ".djvu":
        return `${folderPath}/djvu.svg`;
      case ".doc":
        return `${folderPath}/doc.svg`;
      case ".docm":
        return `${folderPath}/docm.svg`;
      case ".docx":
        return `${folderPath}/docx.svg`;
      case ".dotx":
        return `${folderPath}/dotx.svg`;
      case ".dvd":
        return `${folderPath}/dvd.svg`;
      case ".epub":
        return `${folderPath}/epub.svg`;
      case ".pb2":
      case ".fb2":
        return `${folderPath}/fb2.svg`;
      case ".flv":
        return `${folderPath}/flv.svg`;
      case ".fodt":
        return `${folderPath}/fodt.svg`;
      case ".iaf":
        return `${folderPath}/iaf.svg`;
      case ".ics":
        return `${folderPath}/ics.svg`;
      case ".m2ts":
        return `${folderPath}/m2ts.svg`;
      case ".mht":
        return `${folderPath}/mht.svg`;
      case ".mkv":
        return `${folderPath}/mkv.svg`;
      case ".mov":
        return `${folderPath}/mov.svg`;
      case ".mp4":
        return `${folderPath}/mp4.svg`;
      case ".mpg":
        return `${folderPath}/mpg.svg`;
      case ".odp":
        return `${folderPath}/odp.svg`;
      case ".ods":
        return `${folderPath}/ods.svg`;
      case ".odt":
        return `${folderPath}/odt.svg`;
      case ".otp":
        return `${folderPath}/otp.svg`;
      case ".ots":
        return `${folderPath}/ots.svg`;
      case ".ott":
        return `${folderPath}/ott.svg`;
      case ".pdf":
        return `${folderPath}/pdf.svg`;
      case ".pot":
        return `${folderPath}/pot.svg`;
      case ".pps":
        return `${folderPath}/pps.svg`;
      case ".ppsx":
        return `${folderPath}/ppsx.svg`;
      case ".ppt":
        return `${folderPath}/ppt.svg`;
      case ".pptm":
        return `${folderPath}/pptm.svg`;
      case ".pptx":
        return `${folderPath}/pptx.svg`;
      case ".rtf":
        return `${folderPath}/rtf.svg`;
      case ".svg":
        return `${folderPath}/svg.svg`;
      case ".txt":
        return `${folderPath}/txt.svg`;
      case ".webm":
        return `${folderPath}/webm.svg`;
      case ".xls":
        return `${folderPath}/xls.svg`;
      case ".xlsm":
        return `${folderPath}/xlsm.svg`;
      case ".xlsx":
        return `${folderPath}/xlsx.svg`;
      case ".xps":
        return `${folderPath}/xps.svg`;
      case ".xml":
        return `${folderPath}/xml.svg`;
      case ".oform":
        return `${folderPath}/oform.svg`;
      case ".docxf":
        return `${folderPath}/docxf.svg`;
      default:
        return `${folderPath}/file.svg`;
    }
  };

  getFileIcon = (
    extension,
    size = 32,
    archive = false,
    image = false,
    sound = false,
    html = false
  ) => {
    const folderPath = `/static/images/icons/${size}`;

    if (archive) return `${folderPath}/file_archive.svg`;

    if (image) return `${folderPath}/image.svg`;

    if (sound) return `${folderPath}/sound.svg`;

    if (html) return `${folderPath}/html.svg`;

    return this.getIconUrl(extension, folderPath);
  };

  getIconSrc = (ext, size = 24) => {
    const folderPath = `/static/images/icons/${size}`;

    if (presentInArray(this.extsArchive, ext, true))
      return `${folderPath}/file_archive.svg`;

    if (presentInArray(this.extsImage, ext, true))
      return `${folderPath}/image.svg`;

    if (presentInArray(this.extsAudio, ext, true))
      return `${folderPath}/sound.svg`;

    if (presentInArray(this.html, ext, true)) return `${folderPath}/html.svg`;

    const extension = ext.toLowerCase();

    return this.getIconUrl(extension, folderPath);
  };
}

export default SettingsStore;
