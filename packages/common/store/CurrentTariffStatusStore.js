import { makeAutoObservable, runInAction } from "mobx";
import api from "../api";
import { TariffState } from "../constants";
import { getUserByEmail } from "../api/people";
import moment from "moment";
import { getDaysRemaining } from "@docspace/common/utils";
class CurrentTariffStatusStore {
  portalTariffStatus = {};
  isLoaded = false;
  payerInfo = null;

  paymentDate = "";
  gracePeriodEndDate = "";
  delayDaysCount = "";

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
      if (!this.customerId || !this.customerId?.length) return;

      const result = await getUserByEmail(this.customerId);
      if (!result) return;

      this.payerInfo = result;
    } catch (e) {
      console.error(e);
    }
  };
  setTariffDates = () => {
    const setGracePeriodDays = () => {
      const delayDueDateByMoment = moment(this.delayDueDate);

      this.gracePeriodEndDate = delayDueDateByMoment.format("LL");

      this.delayDaysCount = getDaysRemaining(delayDueDateByMoment);
    };

    this.paymentDate = moment(this.dueDate).format("LL");

    (this.isGracePeriod || this.isNotPaidPeriod) && setGracePeriodDays();
  };

  setPortalTariff = async () => {
    const res = await api.portal.getPortalTariff();

    if (!res) return;

    runInAction(() => {
      this.portalTariffStatus = res;

      this.setTariffDates();
    });
  };
}

export default CurrentTariffStatusStore;
