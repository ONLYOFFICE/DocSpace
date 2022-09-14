import { makeAutoObservable } from "mobx";

import api from "../api";
import { PortalFeaturesLimitations } from "../constants";

class QuotasStore {
  currentPortalQuota = {};
  currentPortalQuotaFeatures = [];
  portalTariffStatus = {};
  isLoaded = false;

  constructor() {
    makeAutoObservable(this);
  }

  init = async () => {
    if (this.isLoaded) return;

    await this.setPortalQuota();

    this.setIsLoaded(true);
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  get isFreeTariff() {
    return this.currentPortalQuota.trial || this.currentPortalQuota.free;
  }

  get currentPlanCost() {
    if (this.currentPortalQuota.price) return this.currentPortalQuota.price;
    else return { value: 0, currencySymbol: "" };
  }

  get maxCountManagersByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "manager"
    );

    return result.value;
  }

  get addedManagersCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "manager"
    );

    return result.used.value;
  }

  get maxTotalSizeByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "total_size"
    );

    if (!result.value) return PortalFeaturesLimitations.Limitless;

    return result.value;
  }

  get usedTotalStorageSizeCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "manager"
    );
    return result.used.value;
  }

  get usedTotalStorageSizeTitle() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "total_size"
    );
    return result.used.title;
  }

  get maxFileSizeByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "file_size"
    );

    return result.value;
  }

  get maxCountUsersByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "users"
    );
    if (!result || !result.value) return PortalFeaturesLimitations.Limitless;
    return result.value;
  }

  get maxCountRoomsByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "room"
    );
    if (!result || !result.value) return PortalFeaturesLimitations.Limitless;
    return result.value;
  }

  get usedRoomsCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "room"
    );
    return result.used.value;
  }

  get isWhiteLabelAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "whitelabel"
    );

    return result.value;
  }

  get isSSOAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "sso"
    );

    return result.value;
  }

  get isRestoreAndAutoBackupAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "restore"
    );

    return result.value;
  }

  get isAuditAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "restore"
    );

    return result.value;
  }

  get currentTariffPlan() {
    return this.currentPortalQuota.title;
  }

  get quotaCharacteristics() {
    const result = [];

    this.currentPortalQuotaFeatures.forEach((elem) => {
      elem.id === "room" && result.splice(0, 0, elem);
      elem.id === "manager" && result.splice(1, 0, elem);
      elem.id === "total_size" && result.splice(2, 0, elem);
    });

    return result;
  }

  get maxUsersCountInRoom() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "usersInRoom"
    );

    if (!result || !result.value) return PortalFeaturesLimitations.Limitless;

    return result.value;
  }
  setPortalQuota = async () => {
    const res = await api.portal.getPortalQuota();
    if (!res) return;

    this.currentPortalQuota = res;
    this.currentPortalQuotaFeatures = res.features;
  };
}

export default QuotasStore;
