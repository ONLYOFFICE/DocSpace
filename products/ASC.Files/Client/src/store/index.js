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
import SocketStore from "./SocketStore";

import VersionHistoryStore from "./VersionHistoryStore";
import DialogsStore from "./DialogsStore";
import selectedFilesStore from "./SelectedFilesStore";
import store from "studio/store";

const formatsStore = new FormatsStore(
  iconFormatsStore,
  mediaViewersFormatsStore,
  docserviceStore
);
const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);

const settingsStore = new SettingsStore(thirdPartyStore, treeFoldersStore);

const filesStore = new FilesStore(
  store.auth,
  store.auth.settingsStore,
  store.auth.userStore,
  fileActionStore,
  selectedFolderStore,
  treeFoldersStore,
  formatsStore,
  settingsStore,
  selectedFilesStore
);
const mediaViewerDataStore = new MediaViewerDataStore(filesStore, formatsStore);

const secondaryProgressDataStore = new SecondaryProgressDataStore();
const primaryProgressDataStore = new PrimaryProgressDataStore();

const dialogsStore = new DialogsStore(
  treeFoldersStore,
  filesStore,
  selectedFolderStore
);
const uploadDataStore = new UploadDataStore(
  formatsStore,
  treeFoldersStore,
  selectedFolderStore,
  filesStore,
  secondaryProgressDataStore,
  primaryProgressDataStore,
  dialogsStore,
  settingsStore
);

const socketStore = new SocketStore(filesStore);

const filesActionsStore = new FilesActionsStore(
  store.auth,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  settingsStore,
  dialogsStore,
  mediaViewerDataStore,
  socketStore
);

const versionHistoryStore = new VersionHistoryStore(filesStore);

//const selectedFilesStore = new SelectedFilesStore(selectedFilesStore);
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
  selectedFilesStore,
  socketStore,
};

export default stores;
