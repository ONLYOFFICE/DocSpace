import FilesStore from "./FilesStore";
import fileActionStore from "./FileActionStore";
import selectedFolderStore from "./SelectedFolderStore";
import TreeFoldersStore from "./TreeFoldersStore";
import thirdPartyStore from "./ThirdPartyStore";
import SettingsStore from "./SettingsStore";
import FilesActionsStore from "./FilesActionsStore";
import FormatsStore from "./FormatsStore";
import iconFormatsStore from "./IconFormatsStore";
import mediaViewersFormatsStore from "./MediaViewersFormatsStore";
import docserviceStore from "./DocserviceStore";
import MediaViewerDataStore from "./MediaViewerDataStore";
import UploadDataStore from "./UploadDataStore";
import SecondaryProgressDataStore from "./SecondaryProgressDataStore";
import PrimaryProgressDataStore from "./PrimaryProgressDataStore";
import ContextOptionsStore from "./ContextOptionsStore";

import versionHistoryStore from "./VersionHistoryStore";
import dialogsStore from "./DialogsStore";

import store from "studio/store";

const formatsStore = new FormatsStore(
  iconFormatsStore,
  mediaViewersFormatsStore,
  docserviceStore
);
const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  fileActionStore,
  selectedFolderStore,
  treeFoldersStore,
  formatsStore
);
const settingsStore = new SettingsStore(thirdPartyStore);
const secondaryProgressDataStore = new SecondaryProgressDataStore();
const primaryProgressDataStore = new PrimaryProgressDataStore();
const uploadDataStore = new UploadDataStore(
  formatsStore,
  treeFoldersStore,
  selectedFolderStore,
  filesStore,
  secondaryProgressDataStore,
  primaryProgressDataStore
);
const filesActionsStore = new FilesActionsStore(
  store.auth,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  settingsStore,
  dialogsStore
);

const mediaViewerDataStore = new MediaViewerDataStore(filesStore);

const contextOptionsStore = new ContextOptionsStore(
  filesStore,
  fileActionStore,
  selectedFolderStore,
  filesActionsStore,
  dialogsStore,
  versionHistoryStore,
  mediaViewerDataStore
);

const stores = {
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
  contextOptionsStore,
};

export default stores;
