import FilesStore from "./FilesStore";
import fileActionStore from "./FileActionStore";
import selectedFolderStore from "./SelectedFolderStore";
import treeFoldersStore from "./TreeFoldersStore";
import InitFilesStore from "./InitFilesStore";
import thirdPartyStore from "./ThirdPartyStore";

import SettingsStore from "./SettingsStore";
import mediaViewerDataStore from "./MediaViewerDataStore";
import formatsStore from "./FormatsStore";
import versionHistoryStore from "./VersionHistoryStore";
import uploadDataStore from "./UploadDataStore";
import dialogsStore from "./DialogsStore";

import filesActionsStore from "./FilesActionsStore";

import store from "studio/store";

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
