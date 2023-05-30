import { makeAutoObservable } from "mobx";
import toastr from "@docspace/components/toast/toastr";

import api from "../api";
import { PortalFeaturesLimitations } from "../constants";
import authStore from "./AuthStore";

const MANAGER = "manager";
const TOTAL_SIZE = "total_size";
const FILE_SIZE = "file_size";
const ROOM = "room";
const USERS = "users";
const USERS_IN_ROOM = "usersInRoom";

const COUNT_FOR_SHOWING_BAR = 2;
const PERCENTAGE_FOR_SHOWING_BAR = 90;

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

  get isOAuthAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "oauth"
    );

    return result?.value;
  }

  get isThirdPartyAvailable() {
    const result = this.currentPortalQuotaFeatures.find(
      (obj) => obj.id === "thirdparty"
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
      this.maxCountRoomsByQuota - this.usedRoomsCount <=
        COUNT_FOR_SHOWING_BAR &&
      this.maxCountRoomsByQuota > 0 &&
      this.maxCountRoomsByQuota >= this.usedRoomsCount
    );
  }

  get showStorageQuotaBar() {
    return (
      (this.usedTotalStorageSizeCount / this.maxTotalSizeByQuota) * 100 >=
      PERCENTAGE_FOR_SHOWING_BAR
    );
  }

  get showUserQuotaBar() {
    return (
      this.addedManagersCount > 1 &&
      this.maxCountManagersByQuota - this.addedManagersCount <=
        COUNT_FOR_SHOWING_BAR &&
      this.maxCountManagersByQuota >= this.addedManagersCount
    );
  }

  get isNonProfit() {
    return this.currentPortalQuota?.nonProfit;
  }

  setPortalQuotaValue = (res) => {
    this.currentPortalQuota = res;
    this.currentPortalQuotaFeatures = res.features;
  };

  updateQuotaUsedValue = (featureId, value) => {
    this.currentPortalQuotaFeatures.forEach((elem) => {
      if (elem.id === featureId && elem.used) elem.used.value = value;
    });
  };
  updateQuotaFeatureValue = (featureId, value) => {
    this.currentPortalQuotaFeatures.forEach((elem) => {
      if (elem.id === featureId) elem.value = value;
    });
  };
  setPortalQuota = async () => {
    try {
      let refresh = false;

      if (
        window.location.search === "?complete=true" &&
        !authStore.isUpdatingTariff
      ) {
        refresh = true;
      }

      const res = await api.portal.getPortalQuota(refresh);

      if (refresh) {
        window.history.replaceState(
          {},
          document.title,
          window.location.pathname
        );
      }

      if (!res) return;

      this.setPortalQuotaValue(res);

      this.setIsLoaded(true);
    } catch (e) {
      toastr.error(e);
    }
  };
}

export default QuotasStore;
