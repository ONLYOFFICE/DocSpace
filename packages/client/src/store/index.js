import authStore from "@docspace/common/store/AuthStore";
import PaymentStore from "./PaymentStore";
import WizardStore from "./WizardStore";
import SettingsSetupStore from "./SettingsSetupStore";
import ConfirmStore from "./ConfirmStore";
import BackupStore from "./BackupStore";
import CommonStore from "./CommonStore";
import BannerStore from "./BannerStore";
import ProfileActionsStore from "./ProfileActionsStore";

const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore();
const confirmStore = new ConfirmStore();
const backupStore = new BackupStore();
const commonStore = new CommonStore();
const bannerStore = new BannerStore();
const profileActionsStore = new ProfileActionsStore();

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
};

export default store;
