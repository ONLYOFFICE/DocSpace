import { makeAutoObservable, runInAction } from "mobx";
import api from "../api";
import { getConvertedSize } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";

const MANAGER = "manager";
const TOTAL_SIZE = "total_size";

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
      (obj) => obj.id === MANAGER
    );
    return result.value;
  }

  get stepAddingQuotaTotalSize() {
    const result = this.portalPaymentQuotasFeatures.find(
      (obj) => obj.id === TOTAL_SIZE
    );
    return result.value;
  }

  get tariffTitle() {
    return this.portalPaymentQuotas?.title;
  }

  setReplacingValuesInTranslation = (t) => {
    this.replaceTotalSizeValue(t);
  };

  replaceTotalSizeValue = (t) => {
    const totalSizeObj = this.portalPaymentQuotasFeatures.find(
      (el) => el.id === TOTAL_SIZE
    );
    const replacedValue = totalSizeObj.title.replace(
      "{0}",
      getConvertedSize(t, totalSizeObj.value)
    );

    totalSizeObj.title = replacedValue;
  };

  get usedTotalStorageSizeTitle() {
    const result = this.portalPaymentQuotasFeatures.find(
      (obj) => obj.id === TOTAL_SIZE
    );
    return result.priceTitle;
  }
  get addedManagersCountTitle() {
    const result = this.portalPaymentQuotasFeatures.find(
      (obj) => obj.id === MANAGER
    );
    return result.priceTitle;
  }

  get tariffPlanTitle() {
    return this.portalPaymentQuotas.title;
  }
  setPortalPaymentQuotas = async (t) => {
    if (this.isLoaded) return;

    try {
      const res = await api.portal.getPortalPaymentQuotas();

      if (!res) return;

      runInAction(() => {
        this.portalPaymentQuotas = res[0];

        this.portalPaymentQuotasFeatures = res[0].features;
      });

      this.setIsLoaded(true);
    } catch (e) {
      toastr.error(e);
    }
  };
}

export default PaymentQuotasStore;
