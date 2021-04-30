import authStore from "@appserver/common/store/AuthStore";
import PaymentStore from "./PaymentStore";
import WizardStore from "./WizardStore";
import SettingsSetupStore from "./SettingsSetupStore";

const paymentStore = new PaymentStore();
const wizardStore = new WizardStore();
const setupStore = new SettingsSetupStore();

const store = {
  auth: authStore,
  payments: paymentStore,
  wizard: wizardStore,
  setup: setupStore,
};

export default store;
