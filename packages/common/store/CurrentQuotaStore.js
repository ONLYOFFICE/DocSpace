import { makeAutoObservable } from "mobx";

import api from "../api";
import { PortalFeaturesLimitations } from "../constants";

const MANAGER = "manager";
const TOTAL_SIZE = "total_size";
const FILE_SIZE = "file_size";
const ROOM = "room";
const USERS = "users";
const USERS_IN_ROOM = "usersInRoom";
class QuotasStore {
  currentPortalQuota = {};
  currentPortalQuotaFeatures = [];

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
    return this.currentPortalQuota.free;
  }

  get currentPlanCost() {
    if (this.currentPortalQuota.price) return this.currentPortalQuota.price;
    else return { value: 0, currencySymbol: "" };
  }

  get maxCountManagersByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === MANAGER
    );

    return result?.value;
  }

  get addedManagersCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === MANAGER
    );

    return result?.used?.value;
  }

  get maxTotalSizeByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === TOTAL_SIZE
    );

    if (!result?.value) return PortalFeaturesLimitations.Limitless;

    return result?.value;
  }

  get usedTotalStorageSizeCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === TOTAL_SIZE
    );
    return result?.used?.value;
  }

  get maxFileSizeByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === FILE_SIZE
    );

    return result?.value;
  }

  get maxCountUsersByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === USERS
    );
    if (!result || !result?.value) return PortalFeaturesLimitations.Limitless;
    return result?.value;
  }

  get maxCountRoomsByQuota() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === ROOM
    );
    if (!result || !result?.value) return PortalFeaturesLimitations.Limitless;
    return result?.value;
  }

  get usedRoomsCount() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === ROOM
    );

    return result?.used?.value;
  }

  get isBrandingAndCustomizationAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "whitelabel"
    );

    return result?.value;
  }

  get isSSOAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "sso"
    );

    return result?.value;
  }

  get isRestoreAndAutoBackupAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "restore"
    );

    return result?.value;
  }

  get isAuditAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "audit"
    );

    return result?.value;
  }

  get currentTariffPlanTitle() {
    return this.currentPortalQuota.title;
  }

  get quotaCharacteristics() {
    const result = [];

    this.currentPortalQuotaFeatures.forEach((elem) => {
      elem.id === ROOM && result?.splice(0, 0, elem);
      elem.id === MANAGER && result?.splice(1, 0, elem);
      elem.id === TOTAL_SIZE && result?.splice(2, 0, elem);
    });

    return result;
  }

  get maxUsersCountInRoom() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === USERS_IN_ROOM
    );

    if (!result || !result?.value) return PortalFeaturesLimitations.Limitless;

    return result?.value;
  }

  get showRoomQuotaBar() {
    return (
      (this.usedRoomsCount / this.maxCountRoomsByQuota) * 100 >= 90 ||
      this.maxCountRoomsByQuota - this.usedRoomsCount === 1
    );
  }

  get showStorageQuotaBar() {
    return (
      (this.usedTotalStorageSizeCount / this.maxTotalSizeByQuota) * 100 >= 90
    );
  }

  setPortalQuotaValue = (res) => {
    this.currentPortalQuota = res;
    this.currentPortalQuotaFeatures = res.features;
  };
  setPortalQuota = async () => {
    const res = await api.portal.getPortalQuota();
    if (!res) return;

    this.currentPortalQuota = res;
    this.currentPortalQuotaFeatures = res.features;
  };
}

export default QuotasStore;
