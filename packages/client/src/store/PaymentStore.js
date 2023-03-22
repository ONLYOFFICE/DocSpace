import {
  getPaymentSettings,
  setLicense,
  acceptLicense,
} from "@docspace/common/api/settings";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import toastr from "@docspace/components/toast/toastr";
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
      }
    } catch (e) {
      console.error(e);
    }
  };

  setPaymentLink = async (link) => {
    this.paymentLink = link;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  getTotalCostByFormula = (value) => {
    const costValuePerManager = authStore.paymentQuotasStore.planCost.value;
    return value * costValuePerManager;
  };

  get allowedStorageSizeByQuota() {
    return this.managersCount * this.stepByQuotaForTotalSize;
  }

  initializeInfo = () => {
    const currentTotalPrice = authStore.currentQuotaStore.currentPlanCost.value;

    this.initializeTotalPrice(currentTotalPrice);
    this.initializeManagersCount(currentTotalPrice);
  };
  initializeTotalPrice = (currentTotalPrice) => {
    if (currentTotalPrice !== 0) {
      this.totalPrice = currentTotalPrice;
    } else {
      this.totalPrice = this.getTotalCostByFormula(
        this.minAvailableManagersValue
      );
    }
  };

  initializeManagersCount = (currentTotalPrice) => {
    const maxCountManagersByQuota =
      authStore.currentQuotaStore.maxCountManagersByQuota;

    if (maxCountManagersByQuota !== 0 && currentTotalPrice !== 0) {
      this.managersCount =
        maxCountManagersByQuota > this.maxAvailableManagersCount
          ? this.maxAvailableManagersCount + 1
          : maxCountManagersByQuota;
    } else {
      this.managersCount = this.minAvailableManagersValue;
    }
  };

  setTotalPrice = (value) => {
    const price = this.getTotalCostByFormula(value);
    if (price !== this.totalPrice) this.totalPrice = price;
  };

  setManagersCount = (managers) => {
    if (managers > this.maxAvailableManagersCount)
      this.managersCount = this.maxAvailableManagersCount + 1;
    else this.managersCount = managers;
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
      toastr.success(t("SuccessfullySentMessage"));
    } catch (e) {
      toastr.error(e);
    }
  };
}

export default PaymentStore;
