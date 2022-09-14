import { makeAutoObservable } from "mobx";
import api from "../api";

class PaymentQuotasStore {
  portalPaymentQuotas = {};
  portalPaymentQuotasFeatures = [];
  isLoaded = false;

  constructor() {
    makeAutoObservable(this);
  }

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  get planCost() {
    if (this.portalPaymentQuotas.price) return this.portalPaymentQuotas.price;
    else return { value: 0, currencySymbol: "" };
  }

  get stepAddingQuotaManagers() {
    const result = this.portalPaymentQuotasFeatures.find(
      (obj) => obj.id === "manager"
    );
    return result.value;
  }

  get stepAddingQuotaTotalSize() {
    const result = this.portalPaymentQuotasFeatures.find(
      (obj) => obj.id === "total_size"
    );
    return result.value;
  }

  get tariffTitle() {
    return this.portalPaymentQuotas.title;
  }
  setPortalPaymentQuotas = async () => {
    if (this.isLoaded) return;

    try {
      const res = await api.portal.getPortalPaymentQuotas();

      if (!res) return;

      this.portalPaymentQuotas = res[0];

      this.portalPaymentQuotasFeatures = res[0].features;

      this.setIsLoaded(true);
    } catch (e) {
      toastr.error(e);
    }
  };
}

export default PaymentQuotasStore;
