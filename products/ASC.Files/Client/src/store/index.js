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
import selectFileDialogStore from "./SelectFileDialogStore";
import RoomsStore from "./RoomsStore";

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

const filesActionsStore = new FilesActionsStore(
  store.auth,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  settingsStore,
  dialogsStore,
  mediaViewerDataStore
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
  settingsStore
);

const hotkeyStore = new HotkeyStore(
  filesStore,
  dialogsStore,
  settingsStore,
  filesActionsStore,
  treeFoldersStore,
  uploadDataStore
);

const roomsStore = new RoomsStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  filesStore,
  treeFoldersStore,
  settingsStore
);

const stores = {
  filesStore,
  roomsStore,
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
  selectFileDialogStore,
};

export default stores;
