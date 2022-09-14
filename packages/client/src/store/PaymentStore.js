import {
  getPaymentSettings,
  setLicense,
  acceptLicense,
} from "@docspace/common/api/settings";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import toastr from "client/toastr";
import authStore from "@docspace/common/store/AuthStore";

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
  totalPrice = 30;
  managersCount = 1;
  maxAvailableManagersCount = 999;
  stepByQuotaForManager = 1;
  minAvailableManagersValue = 1;
  stepByQuotaForTotalSize = 107374182400;
  minAvailableTotalSizeValue = 107374182400;

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
      max,
    } = newSettings;

    this.buyUrl = buyUrl;
    this.salesEmail = salesEmail;
    this.helpUrl = helpUrl;
    this.standaloneMode = standaloneMode;
    this.maxAvailableManagersCount = max;

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

  setPaymentAccount = async () => {
    try {
      const res = await api.portal.getPaymentAccount();

      if (res) {
        if (res.indexOf("error") === -1) {
          this.accountLink = res;
        } else {
          toastr.error(res);
        }
      } else {
        console.error(res);
      }
    } catch (e) {
      console.error(e);
    }
  };

  setPaymentLink = async (link) => {
    this.paymentLink = link;
  };
  updatePayment = async (adminCount) => {
    try {
      const res = await api.portal.updatePayment(adminCount);
      console.log("updatePayment", res);
      if (res !== true) {
        toastr.error("error");
      } else {
        toastr.success("the changes will be applied soon");
      }
    } catch (e) {
      toastr.error(e);
    }
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  getTotalCostByFormula = (value) => {
    const costValuePerManager = authStore.paymentQuotasStore.planCost.value;
    return value * costValuePerManager;
  };

  initializeInfo = (isAlreadyPaid) => {
    this.initializeTotalPrice();
    this.initializeManagersCount();
    !isAlreadyPaid && this.setStartPaymentLink();
  };
  initializeTotalPrice = () => {
    const currentTotalPrice = authStore.currentQuotaStore.currentPlanCost;

    if (currentTotalPrice !== 0) {
      this.totalPrice = currentTotalPrice.value;
    } else {
      this.totalPrice = getTotalCostByFormula(this.minAvailableManagersValue);
    }
  };

  initializeManagersCount = () => {
    const currentPaidValueManagers =
      authStore.currentQuotaStore.maxCountManagersByQuota;

    if (currentPaidValueManagers !== 0) {
      this.managersCount = currentPaidValueManagers;
    } else {
      this.managersCount = this.minAvailableManagersValue;
    }
  };

  setStartPaymentLink = async () => {
    const link = await api.portal.getPaymentLink(this.managersCount);
    setPaymentLink(link);
  };

  setTotalPrice = (value) => {
    const price = this.getTotalCostByFormula(value);
    if (price !== this.totalPrice) this.totalPrice = price;
  };

  setManagersCount = (managers) => {
    this.managersCount = managers;
  };

  get isNeedRequest() {
    return this.managersCount > this.maxAvailableManagersCount;
  }

  get isLessCountThanAcceptable() {
    return this.managersCount < this.minAvailableManagersValue;
  }

  setRangeBound = () => {
    this.stepByQuotaForManager =
      authStore.paymentQuotasStore.stepAddingQuotaManagers;
    this.minAvailableManagersValue = this.stepByQuotaForManager;

    this.stepByQuotaForTotalSize =
      authStore.paymentQuotasStore.stepAddingQuotaTotalSize;
    this.minAvailableTotalSizeValue = this.stepByQuotaForManager;
  };

  sendPaymentRequest = async (email, userName, message) => {
    try {
      await api.portal.sendPaymentRequest(email, userName, message);
      toastr.success("Success");
    } catch (e) {
      toastr.error(e);
    }
  };
}

export default PaymentStore;
