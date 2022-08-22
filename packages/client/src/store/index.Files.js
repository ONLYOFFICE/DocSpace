import FilesStore from "./FilesStore";
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
import store from "client/store";
import selectFileDialogStore from "./SelectFileDialogStore";
import TagsStore from "./TagsStore";
import PeopleStore from "./PeopleStore";

const peopleStore = new PeopleStore(store.auth.infoPanelStore);

const tagsStore = new TagsStore();

const selectedFolderStore = new SelectedFolderStore(store.auth.settingsStore);

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const settingsStore = new SettingsStore(thirdPartyStore, treeFoldersStore);
const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
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
const versionHistoryStore = new VersionHistoryStore(filesStore);
const dialogsStore = new DialogsStore(
  store.auth,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  versionHistoryStore
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
  selectFileDialogStore,

  tagsStore,

  peopleStore,
};

export default stores;
