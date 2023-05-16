import { makeAutoObservable, runInAction } from "mobx";
import moment from "moment";

import { getDaysLeft, getDaysRemaining } from "@docspace/common/utils";

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
    return moment(this.dueDate).format("LL");
  }

  get gracePeriodEndDate() {
    moment.locale(authStore.language);
    return moment(this.delayDueDate).format("LL");
  }

  get delayDaysCount() {
    moment.locale(authStore.language);
    return getDaysRemaining(this.delayDueDate);
  }

  get isLicenseDateExpires() {
    if (!this.dueDate) return;

    return !moment(this.dueDate).isAfter();
  }
  get isLicenseExpiring() {
    if (!this.dueDate || !authStore.isEnterprise) return;

    const days = getDaysLeft(this.dueDate);

    if (days <= 7) return true;

    return false;
  }
  get trialDaysLeft() {
    if (!this.dueDate) return;

    return getDaysLeft(this.dueDate);
  }

  setPortalTariffValue = async (res) => {
    this.portalTariffStatus = res;

    this.setIsLoaded(true);
  };

  setPortalTariff = async () => {
    const res = await api.portal.getPortalTariff();

    if (!res) return;

    runInAction(() => {
      this.portalTariffStatus = res;
    });
  };
}

export default CurrentTariffStatusStore;
