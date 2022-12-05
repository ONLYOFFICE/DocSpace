import authStore from "@docspace/common/store/AuthStore";
import PaymentStore from "./PaymentStore";
import WizardStore from "./WizardStore";
import SettingsSetupStore from "./SettingsSetupStore";
import ConfirmStore from "./ConfirmStore";
import BackupStore from "./BackupStore";
import CommonStore from "./CommonStore";
import BannerStore from "./BannerStore";
import ProfileActionsStore from "./ProfileActionsStore";
import SsoFormStore from "./SsoFormStore";

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
import OformsStore from "./OformsStore";
import AccessRightsStore from "./AccessRightsStore";

const oformsStore = new OformsStore(authStore);

const selectedFolderStore = new SelectedFolderStore(authStore.settingsStore);

const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore();
const confirmStore = new ConfirmStore();
const backupStore = new BackupStore();
const commonStore = new CommonStore();
const bannerStore = new BannerStore();

const ssoStore = new SsoFormStore();

const tagsStore = new TagsStore();

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore);
const settingsStore = new SettingsStore(thirdPartyStore, treeFoldersStore);

const accessRightsStore = new AccessRightsStore(authStore, selectedFolderStore);

const peopleStore = new PeopleStore(
  authStore,
  authStore.infoPanelStore,
  setupStore,
  accessRightsStore
);

const filesStore = new FilesStore(
  authStore,
  selectedFolderStore,
  treeFoldersStore,
  settingsStore,
  thirdPartyStore,
  accessRightsStore
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
  authStore,
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
  mediaViewerDataStore,
  accessRightsStore
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
  settingsStore,
  selectedFolderStore
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
  peopleStore,
  treeFoldersStore,
  selectedFolderStore
);

authStore.infoPanelStore.authStore = authStore;
authStore.infoPanelStore.settingsStore = settingsStore;
authStore.infoPanelStore.filesStore = filesStore;
authStore.infoPanelStore.peopleStore = peopleStore;
authStore.infoPanelStore.selectedFolderStore = selectedFolderStore;
authStore.infoPanelStore.treeFoldersStore = treeFoldersStore;

const store = {
  auth: authStore,
  payments: paymentStore,
  wizard: wizardStore,
  setup: setupStore,
  confirm: confirmStore,
  backup: backupStore,
  common: commonStore,
  bannerStore,
  ssoStore,
  profileActionsStore,

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
  oformsStore,

  tagsStore,

  peopleStore,

  accessRightsStore,
};

export default store;
