import FilesStore from "./FilesStore";
import fileActionStore from "./FileActionStore";
import SelectedFolderStore from "./SelectedFolderStore";
import TreeFoldersStore from "./TreeFoldersStore";
import thirdPartyStore from "./ThirdPartyStore";
import SettingsStore from "./SettingsStore";
import FilesActionsStore from "./FilesActionsStore";
import MediaViewerDataStore from "./MediaViewerDataStore";
import UploadDataStore from "./UploadDataStore";
import SecondaryProgressDataStore from "./SecondaryProgressDataStore";
import PrimaryProgressDataStore from "./PrimaryProgressDataStore";

import VersionHistoryStore from "./VersionHistoryStore";
import DialogsStore from "./DialogsStore";
import selectFolderDialogStore from "./SelectFolderDialogStore";
import ContextOptionsStore from "./ContextOptionsStore";
import HotkeyStore from "./HotkeyStore";
import store from "studio/store";
import InfoPanelStore from "./InfoPanelStore";
import selectFileDialogStore from "./SelectFileDialogStore";

const selectedFolderStore = new SelectedFolderStore(store.auth.settingsStore);

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const settingsStore = new SettingsStore(thirdPartyStore, treeFoldersStore);
const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  fileActionStore,
  selectedFolderStore,
  treeFoldersStore,
  settingsStore,
  selectFolderDialogStore,
  selectFileDialogStore
);
const mediaViewerDataStore = new MediaViewerDataStore(
  filesStore,
  settingsStore
);
const secondaryProgressDataStore = new SecondaryProgressDataStore();
const primaryProgressDataStore = new PrimaryProgressDataStore();
const dialogsStore = new DialogsStore(
  store.auth,
  treeFoldersStore,
  filesStore,
  selectedFolderStore
);
const uploadDataStore = new UploadDataStore(
  treeFoldersStore,
  selectedFolderStore,
  filesStore,
  secondaryProgressDataStore,
  primaryProgressDataStore,
  dialogsStore,
  settingsStore
);

const infoPanelStore = new InfoPanelStore();

const filesActionsStore = new FilesActionsStore(
  store.auth,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  settingsStore,
  dialogsStore,
  mediaViewerDataStore,
  infoPanelStore
);

const versionHistoryStore = new VersionHistoryStore(filesStore);
const contextOptionsStore = new ContextOptionsStore(
  store.auth,
  dialogsStore,
  filesActionsStore,
  filesStore,
  mediaViewerDataStore,
  treeFoldersStore,
  uploadDataStore,
  versionHistoryStore,
  settingsStore,
  infoPanelStore
);

const hotkeyStore = new HotkeyStore(
  filesStore,
  dialogsStore,
  settingsStore,
  filesActionsStore,
  treeFoldersStore,
  uploadDataStore
);

const stores = {
  filesStore,
  settingsStore,
  mediaViewerDataStore,
  versionHistoryStore,
  uploadDataStore,
  dialogsStore,
  treeFoldersStore,
  selectedFolderStore,
  filesActionsStore,
  selectFolderDialogStore,
  contextOptionsStore,
  hotkeyStore,
  infoPanelStore,
  selectFileDialogStore,
};

export default stores;
