import { api } from "asc-web-common";
import { makeAutoObservable } from "mobx";

class PaymentStore {
  salesEmail = "sales@onlyoffice.com";
  helpUrl = "https://helpdesk.onlyoffice.com";
  buyUrl =
    "https://www.onlyoffice.com/enterprise-edition.aspx?type=buyenterprise";
  standaloneMode = true;
  currentLicense = {
    expiresDate: new Date(),
    trialMode: true,
  };

  constructor() {
    makeAutoObservable(this);
  }

  getSettingsPayment = async () => {
    const newSettings = await api.settings.getPaymentSettings();
    const {
      buyUrl,
      salesEmail,
      currentLicense,
      standalone: standaloneMode,
      feedbackAndSupportUrl: helpUrl,
    } = newSettings;

    this.buyUrl = buyUrl;
    this.salesEmail = salesEmail;
    this.helpUrl = helpUrl;
    this.standaloneMode = standaloneMode;
    if (currentLicense) {
      if (currentLicense.date)
        this.currentLicense.expiresDate = new Date(currentLicense.date);

      if (currentLicense.trial)
        this.currentLicense.trialMode = currentLicense.trial;
    }

    return newSettings;
  };

  setPaymentsLicense = async (confirmKey, data) => {
    const response = await api.settings.setLicense(confirmKey, data);

    this.acceptPaymentsLicense();
    this.getSettingsPayment();

    return response;
  };

  acceptPaymentsLicense = async () => {
    const response = await api.settings
      .acceptLicense()
      .then((res) => console.log(res));

    return response;
  };
}

export default PaymentStore;
