import store from "studio/store";

import DialogsStore from "./DialogsStore";
import docserviceStore from "./DocserviceStore";
import fileActionStore from "./FileActionStore";
import FilesActionsStore from "./FilesActionsStore";
import FilesStore from "./FilesStore";
import FormatsStore from "./FormatsStore";
import iconFormatsStore from "./IconFormatsStore";
import InfoPanelStore from "./InfoPanelStore";
import MediaViewerDataStore from "./MediaViewerDataStore";
import mediaViewersFormatsStore from "./MediaViewersFormatsStore";
import PrimaryProgressDataStore from "./PrimaryProgressDataStore";
import SecondaryProgressDataStore from "./SecondaryProgressDataStore";
import selectedFilesStore from "./SelectedFilesStore";
import selectedFolderStore from "./SelectedFolderStore";
import SettingsStore from "./SettingsStore";
import thirdPartyStore from "./ThirdPartyStore";
import TreeFoldersStore from "./TreeFoldersStore";
import UploadDataStore from "./UploadDataStore";
import VersionHistoryStore from "./VersionHistoryStore";

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
  formatsStore,
  settingsStore,
  selectedFilesStore
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

//const selectedFilesStore = new SelectedFilesStore(selectedFilesStore);
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
  selectedFilesStore,
  infoPanelStore,
};

export default stores;
