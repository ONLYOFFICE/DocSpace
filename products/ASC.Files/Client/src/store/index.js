import FilesStore from "./FilesStore";
import fileActionStore from "./FileActionStore";
import selectedFolderStore from "./SelectedFolderStore";
import TreeFoldersStore from "./TreeFoldersStore";
import InitFilesStore from "./InitFilesStore";
import thirdPartyStore from "./ThirdPartyStore";
import SettingsStore from "./SettingsStore";
import FilesActionsStore from "./FilesActionsStore";
import FormatsStore from "./FormatsStore";
import iconFormatsStore from "./IconFormatsStore";
import mediaViewersFormatsStore from "./MediaViewersFormatsStore";
import docserviceStore from "./DocserviceStore";
import MediaViewerDataStore from "./MediaViewerDataStore";

import versionHistoryStore from "./VersionHistoryStore";
import uploadDataStore from "./UploadDataStore";
import dialogsStore from "./DialogsStore";

import store from "studio/store";

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  fileActionStore,
  selectedFolderStore,
  treeFoldersStore
);
const initFilesStore = new InitFilesStore(
  store.auth,
  store.auth.settingsStore,
  filesStore,
  treeFoldersStore
);
const settingsStore = new SettingsStore(thirdPartyStore);
const filesActionsStore = new FilesActionsStore(
  store.auth,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  initFilesStore,
  settingsStore,
  dialogsStore
);
const formatsStore = new FormatsStore(
  iconFormatsStore,
  mediaViewersFormatsStore,
  docserviceStore
);
const mediaViewerDataStore = new MediaViewerDataStore(filesStore);

const stores = {
  initFilesStore,
  filesStore,
  settingsStore,
  mediaViewerDataStore,
  formatsStore,
  versionHistoryStore,
  uploadDataStore,
  dialogsStore,
  treeFoldersStore,
  selectedFolderStore,
  filesActionsStore,
};

export default stores;
