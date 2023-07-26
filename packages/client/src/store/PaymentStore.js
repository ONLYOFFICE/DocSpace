import {
  getPaymentSettings,
  setLicense,
  acceptLicense,
} from "@docspace/common/api/settings";
import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";
import toastr from "@docspace/components/toast/toastr";
import authStore from "@docspace/common/store/AuthStore";
import { getPaymentLink } from "@docspace/common/api/portal";
import axios from "axios";
import { combineUrl } from "@docspace/common/utils";

class PaymentStore {
  salesEmail = "";
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
  isUpdatingBasicSettings = false;
  totalPrice = 30;
  managersCount = 1;
  maxAvailableManagersCount = 999;
  stepByQuotaForManager = 1;
  minAvailableManagersValue = 1;
  stepByQuotaForTotalSize = 107374182400;
  minAvailableTotalSizeValue = 107374182400;

  isInitPaymentPage = false;
  isLicenseCorrect = false;

  constructor() {
    makeAutoObservable(this);
  }

  get isAlreadyPaid() {
    const { currentQuotaStore, currentTariffStatusStore } = authStore;
    const { customerId } = currentTariffStatusStore;
    const { isFreeTariff } = currentQuotaStore;

    return customerId?.length !== 0 || !isFreeTariff;
  }

  setIsInitPaymentPage = (value) => {
    this.isInitPaymentPage = value
  }

  setIsUpdatingBasicSettings = (isUpdatingBasicSettings) => {
    this.isUpdatingBasicSettings = isUpdatingBasicSettings;
  };
  basicSettings = async () => {
    const { currentTariffStatusStore, currentQuotaStore } = authStore;
    const { setPortalTariff, setPayerInfo } = currentTariffStatusStore;
    const { addedManagersCount } = currentQuotaStore;

    this.setIsUpdatingBasicSettings(true);

    const requests = [setPortalTariff()];

    this.isAlreadyPaid
      ? requests.push(this.setPaymentAccount())
      : requests.push(this.getBasicPaymentLink(addedManagersCount));

    try {
      await Promise.all(requests);
      this.setBasicTariffContainer();
    } catch (error) {
      toastr.error(t("Common:UnexpectedError"));
      console.error(error);
    }

    if (this.isAlreadyPaid) await setPayerInfo();

    this.setIsUpdatingBasicSettings(false);
  };

  init = async (t) => {
    if (this.isInitPaymentPage) {
      this.basicSettings();

      return;
    }

    const {
      paymentQuotasStore,
      currentTariffStatusStore,
      currentQuotaStore,
    } = authStore;
    const { setPayerInfo } = currentTariffStatusStore;
    const { addedManagersCount } = currentQuotaStore;
    const { setPortalPaymentQuotas } = paymentQuotasStore;

    const requests = [this.getSettingsPayment(), setPortalPaymentQuotas()];

    this.isAlreadyPaid
      ? requests.push(this.setPaymentAccount())
      : requests.push(this.getBasicPaymentLink(addedManagersCount));

    try {
      await Promise.all(requests);
      this.setRangeStepByQuota();
      this.setBasicTariffContainer();

      if (!this.isAlreadyPaid) this.setIsInitPaymentPage(true);
    } catch (error) {
      toastr.error(t("Common:UnexpectedError"));
      console.error(error);
      return;
    }

    if (this.isAlreadyPaid) await setPayerInfo();

    this.setIsInitPaymentPage(true);
  };

  getBasicPaymentLink = async (managersCount) => {
    const backUrl = combineUrl(
      window.location.origin,
      "/portal-settings/payments/portal-payments?complete=true"
    );

    try {
      const link = await getPaymentLink(managersCount, backUrl);

      if (!link) return;
      this.setPaymentLink(link);
    } catch (err) {
      console.error(err);
    }
  };
  getPaymentLink = async (token = undefined) => {
    const backUrl = combineUrl(
      window.location.origin,
      "/portal-settings/payments/portal-payments?complete=true"
    );

    await getPaymentLink(this.managersCount, backUrl, token)
      .then((link) => {
        if (!link) return;
        this.setPaymentLink(link);
      })
      .catch((err) => {
        if (axios.isCancel(err)) {
          console.log("Request canceled", err.message);
        } else {
          console.error(err);
          if (err?.response?.status === 402) {
            return;
          }
          this.isInitPaymentPage && toastr.error(err);
        }
      });
  };

  standaloneBasicSettings = async (t) => {
    const { getTenantExtra } = authStore;

    this.setIsUpdatingBasicSettings(true);

    try {
      await getTenantExtra();
    } catch (e) {
      toastr.error(t("Common:UnexpectedError"));

      return;
    }

    this.setIsUpdatingBasicSettings(false);
  };

  standaloneInit = async (t) => {
    const { getTenantExtra } = authStore;

    if (this.isInitPaymentPage) {
      this.standaloneBasicSettings(t);

      return;
    }

    try {
      await Promise.all([this.getSettingsPayment(), getTenantExtra()]);
    } catch (error) {
      toastr.error(t("Common:UnexpectedError"));
      console.error(error);
      return;
    }

    this.isInitPaymentPage = true;
  };
  getSettingsPayment = async () => {
    try {
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
    } catch (e) {
      console.error(e);
    }
  };

  setIsLicenseCorrect = (isLicenseCorrect) => {
    this.isLicenseCorrect = isLicenseCorrect;
  };
  setPaymentsLicense = async (confirmKey, data) => {
    try {
      const message = await setLicense(confirmKey, data);
      this.setIsLicenseCorrect(true);

      toastr.success(message);
    } catch (e) {
      toastr.error(e);
      this.setIsLicenseCorrect(false);
    }
  };

  acceptPaymentsLicense = async (t) => {
    try {
      const { getTenantExtra } = authStore;

      await acceptLicense();

      toastr.success(t("ActivateLicenseActivated"));
      localStorage.removeItem("enterpriseAlertClose");

      await getTenantExtra();
    } catch (e) {
      toastr.error(e);
    }
  };

  setPaymentAccount = async () => {
    const res = await api.portal.getPaymentAccount();

    if (res) {
      if (res.indexOf("error") === -1) {
        this.accountLink = res;
      } else {
        toastr.error(res);
      }
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
    if (this.managersCount > this.maxAvailableManagersCount)
      return this.maxAvailableManagersCount * this.stepByQuotaForTotalSize;

    return this.managersCount * this.stepByQuotaForTotalSize;
  }

  resetTariffContainerToBasic = () => {
    this.setBasicTariffContainer();
  };
  setBasicTariffContainer = () => {
    const { currentQuotaStore } = authStore;
    const {
      currentPlanCost,
      maxCountManagersByQuota,
      addedManagersCount,
    } = currentQuotaStore;
    const currentTotalPrice = currentPlanCost.value;

    if (this.isAlreadyPaid) {
      const countOnRequest =
        maxCountManagersByQuota > this.maxAvailableManagersCount;

      this.managersCount = countOnRequest
        ? this.maxAvailableManagersCount + 1
        : maxCountManagersByQuota;

      this.totalPrice = currentTotalPrice;

      return;
    }

    this.managersCount = addedManagersCount;
    this.totalPrice = this.getTotalCostByFormula(addedManagersCount);
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

  get isPayer() {
    const { userStore, currentTariffStatusStore } = authStore;
    const { user } = userStore;

    const { customerId } = currentTariffStatusStore;

    if (!user) return false;

    return user.email === customerId;
  }

  get isStripePortalAvailable() {
    const { userStore } = authStore;
    const { user } = userStore;

    if (!user) return false;

    return user.isOwner || this.isPayer;
  }

  get canUpdateTariff() {
    const { currentQuotaStore, userStore } = authStore;
    const { user } = userStore;
    const { isFreeTariff } = currentQuotaStore;

    if (!user) return false;

    if (isFreeTariff) return true;

    return this.isPayer;
  }

  get canPayTariff() {
    const { currentQuotaStore } = authStore;
    const { addedManagersCount } = currentQuotaStore;

    if (this.managersCount >= addedManagersCount) return true;

    return false;
  }

  get canDowngradeTariff() {
    const { currentQuotaStore } = authStore;
    const { addedManagersCount, usedTotalStorageSizeCount } = currentQuotaStore;

    if (addedManagersCount > this.managersCount) return false;
    if (usedTotalStorageSizeCount > this.allowedStorageSizeByQuota)
      return false;

    return true;
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
