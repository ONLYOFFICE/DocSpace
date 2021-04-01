import FilesStore from "./FilesStore";
import fileActionStore from "./FileActionStore";
import selectedFolderStore from "./SelectedFolderStore";
import treeFoldersStore from "./TreeFoldersStore";

import settingsStore from "./SettingsStore";
import mediaViewerDataStore from "./MediaViewerDataStore";
import formatsStore from "./FormatsStore";
import versionHistoryStore from "./VersionHistoryStore";
import uploadDataStore from "./UploadDataStore";
import dialogsStore from "./DialogsStore";

import filesActionsStore from "./FilesActionsStore";
import initFilesStore from "./InitFilesStore";
import store from "studio/store";

const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  fileActionStore,
  selectedFolderStore,
  treeFoldersStore
);

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
