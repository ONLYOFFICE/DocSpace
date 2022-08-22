import {
  getPaymentSettings,
  setLicense,
  acceptLicense,
} from "@docspace/common/api/settings";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import toastr from "client/toastr";

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

  paymentLink = null;
  isLoading = false;

  constructor() {
    makeAutoObservable(this);
  }

  getSettingsPayment = async () => {
    const newSettings = await getPaymentSettings();
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
    const response = await setLicense(confirmKey, data);

    this.acceptPaymentsLicense();
    this.getSettingsPayment();

    return response;
  };

  acceptPaymentsLicense = async () => {
    const response = await acceptLicense().then((res) => console.log(res));

    return response;
  };

  // ------------ For docspace -----------

  getPaymentAccount = async () => {
    return await api.portal.getPaymentAccount();
  };

  setPaymentLink = async (link) => {
    this.paymentLink = link;
    // try {
    //   const res = await api.portal.getPaymentLink(adminCount, currency);
    //   console.log("getPaymentLink", res);
    //   if (res) this.paymentLink = res;
    // } catch (e) {
    //   toastr.error(e);
    // }
  };
  updatePayment = async (adminCount, currency) => {
    try {
      const res = await api.portal.updatePayment(adminCount, currency);
      console.log("updatePayment", res);
    } catch (e) {
      toastr.error(e);
    }
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };
}

export default PaymentStore;
