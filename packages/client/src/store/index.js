import authStore from "./AuthStore";
import PaymentStore from "./PaymentStore";
import WizardStore from "./WizardStore";
import SettingsSetupStore from "./SettingsSetupStore";
import ConfirmStore from "./ConfirmStore";
import BackupStore from "./BackupStore";
import CommonStore from "./CommonStore";
import BannerStore from "./BannerStore";
import ProfileActionsStore from "./ProfileActionsStore";
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
import selectFileDialogStore from "./SelectFileDialogStore";
import TagsStore from "./TagsStore";
import PeopleStore from "./PeopleStore";

const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore(authStore);
const confirmStore = new ConfirmStore();
const backupStore = new BackupStore();
const commonStore = new CommonStore();
const bannerStore = new BannerStore();

const peopleStore = new PeopleStore(authStore);

const tagsStore = new TagsStore();

const selectedFolderStore = new SelectedFolderStore(authStore.settingsStore);

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const settingsStore = new SettingsStore(thirdPartyStore, treeFoldersStore);
const filesStore = new FilesStore(
  authStore,
  authStore.settingsStore,
  authStore.userStore,
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
  authStore,
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
  authStore,
  uploadDataStore,
  treeFoldersStore,
  filesStore,
  selectedFolderStore,
  settingsStore,
  dialogsStore,
  mediaViewerDataStore
);

const contextOptionsStore = new ContextOptionsStore(
  authStore,
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

const profileActionsStore = new ProfileActionsStore(
  authStore,
  filesStore,
  peopleStore
);

const store = {
  auth: authStore,
  payments: paymentStore,
  wizard: wizardStore,
  setup: setupStore,
  confirm: confirmStore,
  backup: backupStore,
  common: commonStore,
  bannerStore: bannerStore,
  profileActionsStore: profileActionsStore,

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

export default store;
