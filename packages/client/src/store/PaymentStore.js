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
  accountLink = null;
  isLoading = false;
  totalPrice = null;

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

  setPaymentAccount = async () => {
    try {
      const res = await api.portal.getPaymentAccount();

      if (res) {
        if (res.indexOf("error") === -1) {
          this.accountLink = res;
        } else {
          toastr.error(res);
        }
      }
    } catch (e) {
      toastr.error(e);
    }
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
  updatePayment = async (adminCount) => {
    try {
      const res = await api.portal.updatePayment(adminCount);
      console.log("updatePayment", res);
    } catch (e) {
      toastr.error(e);
    }
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setTotalPrice = (price) => {
    if (price > 0 && price !== this.totalPrice) this.totalPrice = price;
  };
}

export default PaymentStore;
