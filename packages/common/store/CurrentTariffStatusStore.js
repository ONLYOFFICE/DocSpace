import { makeAutoObservable, runInAction } from "mobx";
import moment from "moment";

import { getDaysRemaining } from "@docspace/common/utils";

import api from "../api";
import { TariffState } from "../constants";
import { getUserByEmail } from "../api/people";
import authStore from "./AuthStore";
class CurrentTariffStatusStore {
  portalTariffStatus = {};
  isLoaded = false;
  payerInfo = null;

  constructor() {
    makeAutoObservable(this);
  }

  init = async () => {
    if (this.isLoaded) return;

    await this.setPortalTariff();

    this.setIsLoaded(true);
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  get isGracePeriod() {
    return this.portalTariffStatus.state === TariffState.Delay;
  }

  get isPaidPeriod() {
    return this.portalTariffStatus.state === TariffState.Paid;
  }

  get isNotPaidPeriod() {
    return this.portalTariffStatus.state === TariffState.NotPaid;
  }

  get dueDate() {
    return this.portalTariffStatus.dueDate;
  }

  get delayDueDate() {
    return this.portalTariffStatus.delayDueDate;
  }

  get customerId() {
    return this.portalTariffStatus.customerId;
  }

  get portalStatus() {
    return this.portalTariffStatus.portalStatus;
  }

  setPayerInfo = async () => {
    try {
      if (!this.customerId || !this.customerId?.length) {
        this.payerInfo = null;
        return;
      }

      const result = await getUserByEmail(this.customerId);
      if (!result) {
        this.payerInfo = null;
        return;
      }

      this.payerInfo = result;
    } catch (e) {
      this.payerInfo = null;
      console.error(e);
    }
  };

  get paymentDate() {
    moment.locale(authStore.language);
    if (this.dueDate === null) return "";
    return moment(this.dueDate).format("LL");
  }

  get isPaymentDateValid() {
    moment.locale(authStore.language);
    if (this.dueDate === null) return "";
    return moment(this.dueDate).year() !== 9999;
  }

  get gracePeriodEndDate() {
    moment.locale(authStore.language);
    if (this.delayDueDate === null) return "";
    return moment(this.delayDueDate).format("LL");
  }

  get delayDaysCount() {
    moment.locale(authStore.language);
    if (this.delayDueDate === null) return "";
    return getDaysRemaining(this.delayDueDate);
  }

  setPortalTariff = async () => {
    let refresh = false;

    if (window.location.search === "?complete=true") {
      refresh = true;
    }

    const res = await api.portal.getPortalTariff(refresh);

    if (refresh)
      window.history.replaceState({}, document.title, window.location.pathname);

    if (!res) return;

    runInAction(() => {
      this.portalTariffStatus = res;
    });
  };
}

export default CurrentTariffStatusStore;
