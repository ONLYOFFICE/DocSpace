import authStore from "@docspace/common/store/AuthStore";
import PaymentStore from "./PaymentStore";
import WizardStore from "./WizardStore";
import SettingsSetupStore from "./SettingsSetupStore";
import ConfirmStore from "./ConfirmStore";
import BackupStore from "./BackupStore";
import CommonStore from "./CommonStore";

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
import filesSelectorInput from "./FilesSelectorInput";
import ContextOptionsStore from "./ContextOptionsStore";
import HotkeyStore from "./HotkeyStore";

import TagsStore from "./TagsStore";
import PeopleStore from "./PeopleStore";
import OformsStore from "./OformsStore";

import AccessRightsStore from "./AccessRightsStore";
import TableStore from "./TableStore";
import CreateEditRoomStore from "./CreateEditRoomStore";
import PublicRoomStore from "./PublicRoomStore";

import WebhooksStore from "./WebhooksStore";
import ClientLoadingStore from "./ClientLoadingStore";

const oformsStore = new OformsStore(authStore);

const selectedFolderStore = new SelectedFolderStore(authStore.settingsStore);

const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore();
const confirmStore = new ConfirmStore();
const backupStore = new BackupStore();
const commonStore = new CommonStore();

const ssoStore = new SsoFormStore();

const tagsStore = new TagsStore();

const treeFoldersStore = new TreeFoldersStore(selectedFolderStore, authStore);

const publicRoomStore = new PublicRoomStore();

const clientLoadingStore = new ClientLoadingStore();

const settingsStore = new SettingsStore(
  thirdPartyStore,
  treeFoldersStore,
  publicRoomStore
);

const accessRightsStore = new AccessRightsStore(authStore, selectedFolderStore);

const filesStore = new FilesStore(
  authStore,
  selectedFolderStore,
  treeFoldersStore,
  settingsStore,
  thirdPartyStore,
  accessRightsStore,
  clientLoadingStore,
  publicRoomStore
);

const mediaViewerDataStore = new MediaViewerDataStore(
  filesStore,
  settingsStore,
  publicRoomStore
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

const peopleStore = new PeopleStore(
  authStore,
  setupStore,
  accessRightsStore,
  dialogsStore
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
  accessRightsStore,
  clientLoadingStore,
  publicRoomStore
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
  selectedFolderStore,
  publicRoomStore
);

const hotkeyStore = new HotkeyStore(
  filesStore,
  dialogsStore,
  settingsStore,
  filesActionsStore,
  treeFoldersStore,
  uploadDataStore,
  selectedFolderStore
);

const profileActionsStore = new ProfileActionsStore(
  authStore,
  filesStore,
  peopleStore,
  treeFoldersStore,
  selectedFolderStore
);

peopleStore.profileActionsStore = profileActionsStore;

const tableStore = new TableStore(authStore, treeFoldersStore);

authStore.infoPanelStore.authStore = authStore;
authStore.infoPanelStore.settingsStore = settingsStore;
authStore.infoPanelStore.filesStore = filesStore;
authStore.infoPanelStore.peopleStore = peopleStore;
authStore.infoPanelStore.selectedFolderStore = selectedFolderStore;
authStore.infoPanelStore.treeFoldersStore = treeFoldersStore;

const createEditRoomStore = new CreateEditRoomStore(
  filesStore,
  filesActionsStore,
  selectedFolderStore,
  tagsStore,
  thirdPartyStore,
  authStore.settingsStore,
  authStore.infoPanelStore,
  authStore.currentQuotaStore,
  clientLoadingStore
);

const webhooksStore = new WebhooksStore();

const store = {
  auth: authStore,
  payments: paymentStore,
  wizard: wizardStore,
  setup: setupStore,
  confirm: confirmStore,
  backup: backupStore,
  common: commonStore,

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
  filesSelectorInput,
  contextOptionsStore,
  hotkeyStore,

  oformsStore,
  tableStore,

  tagsStore,

  peopleStore,

  accessRightsStore,
  createEditRoomStore,

  webhooksStore,
  clientLoadingStore,
  publicRoomStore,
};

export default store;
