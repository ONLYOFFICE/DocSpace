import {
  getPaymentSettings,
  setLicense,
  acceptLicense,
} from "@docspace/common/api/settings";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import toastr from "@docspace/components/toast/toastr";
import authStore from "@docspace/common/store/AuthStore";
import moment from "moment";
import { getUserByEmail } from "@docspace/common/api/people";

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

  isInitPaymentPage = false;

  paymentDate = "";

  gracePeriodStartDate = "";
  gracePeriodEndDate = "";
  delayDaysCount = "";

  payerInfo = null;

  constructor() {
    makeAutoObservable(this);
  }

  get isValidPaymentDateYear() {
    const { currentTariffStatusStore } = authStore;
    const { delayDueDate } = currentTariffStatusStore;
    const paymentTermYear = moment(delayDueDate).year();

    return paymentTermYear !== 9999;
  }

  get isAlreadyPaid() {
    const { currentQuotaStore, currentTariffStatusStore } = authStore;
    const { customerId } = currentTariffStatusStore;
    const { isFreeTariff } = currentQuotaStore;

    return customerId?.length !== 0 || !isFreeTariff;
  }

  setTariffDates = () => {
    const { currentTariffStatusStore } = authStore;
    const {
      isGracePeriod,
      isNotPaidPeriod,
      delayDueDate,
      dueDate,
    } = currentTariffStatusStore;

    const setGracePeriodDays = () => {
      const fromDateMoment = moment(dueDate);
      const byDateMoment = moment(delayDueDate);

      this.gracePeriodStartDate = fromDateMoment.format("LL");
      this.gracePeriodEndDate = byDateMoment.format("LL");

      this.delayDaysCount = fromDateMoment.to(byDateMoment, true);
    };

    this.paymentDate = moment(
      isGracePeriod || isNotPaidPeriod ? delayDueDate : dueDate
    ).format("LL");

    isGracePeriod && setGracePeriodDays();
  };

  init = async () => {
    if (this.isInitPaymentPage) return;

    const {
      currentQuotaStore,
      currentTariffStatusStore,
      paymentQuotasStore,
    } = authStore;
    const { customerId } = currentTariffStatusStore;
    const { setPortalPaymentQuotas, isLoaded } = paymentQuotasStore;
    const { setPortalQuota } = currentQuotaStore;

    const requests = [this.getSettingsPayment(), setPortalQuota()];

    if (!isLoaded) requests.push(setPortalPaymentQuotas());

    if (this.isAlreadyPaid) requests.push(this.setPaymentAccount());

    try {
      await Promise.all(requests);
      this.setRangeStepByQuota();
      this.setTariffDates();

      if (!this.isAlreadyPaid) this.isInitPaymentPage = true;
    } catch (error) {
      toastr.error(error);
      return;
    }

    try {
      if (this.isAlreadyPaid) this.payerInfo = await getUserByEmail(customerId);
      this.isInitPaymentPage = true;
    } catch (e) {
      console.error(e);
    }
  };

  getSettingsPayment = async () => {
    const newSettings = await getPaymentSettings();

    if (!newSettings) return;

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

  setRangeStepByQuota = () => {
    const { paymentQuotasStore } = authStore;
    const {
      stepAddingQuotaManagers,
      stepAddingQuotaTotalSize,
    } = paymentQuotasStore;

    this.stepByQuotaForManager = stepAddingQuotaManagers;
    this.minAvailableManagersValue = this.stepByQuotaForManager;

    this.stepByQuotaForTotalSize = stepAddingQuotaTotalSize;
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
