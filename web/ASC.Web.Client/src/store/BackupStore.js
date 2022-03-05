import api from "@appserver/common/api";
import { getBackupProgress } from "@appserver/common/api/portal";
import { makeAutoObservable } from "mobx";
import {
  saveToSessionStorage,
  getFromSessionStorage,
} from "../components/pages/Settings/utils";
import toastr from "../helpers/toastr";

const DOCUMENT_MODULE_TYPE = 0;
const RESOURCES_MODULE_TYPE = 1;
const STORAGES_MODULE_TYPE = 5;

const EVERY_DAY_TYPE = "0";
const EVERY_WEEK_TYPE = "1";
const EVERY_MONTH_TYPE = "2";

class BackupStore {
  backupSchedule = {};
  backupStorage = {};

  defaultDay = "0";
  defaultHour = "12:00";
  defaultPeriodNumber = "0";
  defaultPeriodLabel = "Every day";
  defaultMaxCopiesNumber = "10";

  defaultWeekday = null;
  defaultWeekdayLabel = "";
  defaultStorageType = null;
  defaultFolderId = null;
  defaultMonthDay = "1";

  selectedDay = "0";
  selectedHour = "12:00";
  selectedPeriodNumber = "0";
  selectedPeriodLabel = "Every day";
  selectedMaxCopiesNumber = "10";

  selectedWeekday = null;
  selectedWeekdayLabel = "";
  selectedStorageType = null;
  selectedFolderId = null;
  selectedMonthDay = "1";

  selectedStorageId = null;
  defaultStorageId = null;

  thirdPartyStorage = [];

  preparationPortalDialogVisible = false;

  downloadingProgress = 100;

  temporaryLink = null;
  timerId = null;

  constructor() {
    makeAutoObservable(this);
  }

  deleteSchedule = (weekdayArr) => {
    this.backupSchedule = null;

    this.defaultDay = "0";
    this.defaultHour = "12:00";
    this.defaultPeriodNumber = "0";
    this.defaultPeriodLabel = "Every day";
    this.defaultMaxCopiesNumber = "10";

    this.defaultStorageType = "0";
    this.defaultFolderId = null;
    this.defaultMonthDay = "1";

    this.selectedDay = "0";
    this.selectedHour = "12:00";
    this.selectedPeriodNumber = "0";
    this.selectedPeriodLabel = "Every day";
    this.selectedMaxCopiesNumber = "10";

    this.selectedStorageType = "0";
    this.selectedFolderId = null;
    this.selectedMonthDay = "1";

    this.selectedStorageId = null;
    this.defaultStorageId = null;

    this.defaultWeekday = weekdayArr[0].key;
    this.defaultWeekdayLabel = weekdayArr[0].label;

    this.selectedWeekdayLabel = this.defaultWeekdayLabel;
    this.selectedWeekday = this.defaultWeekday;
  };
  get isChanged() {
    if (this.selectedHour !== this.defaultHour) {
      return true;
    }

    if (+this.selectedMaxCopiesNumber !== +this.defaultMaxCopiesNumber) {
      return true;
    }

    if (this.defaultPeriodNumber !== this.selectedPeriodNumber) {
      return true;
    }

    if (this.selectedStorageType !== this.defaultStorageType) {
      return true;
    }

    if (this.selectedPeriodNumber === "2") {
      if (this.selectedMonthDay !== this.defaultDay) {
        return true;
      }
    }

    if (this.selectedPeriodNumber === "1") {
      if (this.selectedWeekdayLabel !== this.defaultWeekdayLabel) {
        return true;
      }
    }
    console.log(
      "this.selectedFolderId",
      this.selectedFolderId,
      "this.defaultFolderId",
      this.defaultFolderId
    );
    if (this.selectedFolderId !== this.defaultFolderId) return true;

    if (this.selectedStorageId !== this.defaultStorageId) return true;

    return false;
  }

  toDefault = () => {
    this.selectedMonthlySchedule = this.defaultMonthlySchedule;
    this.selectedWeeklySchedule = this.defaultWeeklySchedule;
    this.selectedDailySchedule = this.defaultDailySchedule;
    this.selectedHour = this.defaultHour;
    this.selectedPeriodLabel = this.defaultPeriodLabel;
    this.selectedPeriodNumber = this.defaultPeriodNumber;
    this.selectedWeekdayLabel = this.defaultWeekdayLabel;
    this.selectedMaxCopiesNumber = this.defaultMaxCopiesNumber;
    this.selectedStorageType = this.defaultStorageType;
    this.selectedMonthDay = this.defaultMonthDay;
    this.selectedWeekday = this.defaultWeekday;
    this.selectedStorageId = this.defaultStorageId;
    this.selectedFolderId = this.defaultFolderId;
  };

  setDefaultOptions = (t, periodObj, weekdayArr) => {
    if (this.backupSchedule) {
      const {
        storageType,
        cronParams,
        backupsStored,
        storageParams,
      } = this.backupSchedule;

      const { folderId } = storageParams;
      const { period, day, hour } = cronParams;

      this.defaultDay = `${day}`;
      this.defaultHour = `${hour}:00`;
      this.defaultPeriodNumber = `${period}`;
      this.defaultMaxCopiesNumber = `${backupsStored}`;
      this.defaultStorageType = `${storageType}`;
      this.defaultFolderId = `${folderId}`;

      this.selectedDay = this.defaultDay;
      this.selectedHour = this.defaultHour;
      this.selectedPeriodNumber = this.defaultPeriodNumber;
      this.selectedMaxCopiesNumber = this.defaultMaxCopiesNumber;
      this.selectedStorageType = this.defaultStorageType;
      this.selectedFolderId = this.defaultFolderId;

      this.defaultPeriodLabel = periodObj[+this.defaultPeriodNumber].label;
      this.selectedPeriodLabel = this.defaultPeriodLabel;

      this.defaultMonthDay =
        this.defaultPeriodNumber === EVERY_WEEK_TYPE ||
        this.defaultPeriodNumber === EVERY_DAY_TYPE
          ? "1"
          : this.defaultDay;

      this.selectedMonthDay = this.defaultMonthDay;

      if (this.defaultPeriodNumber === EVERY_WEEK_TYPE) {
        let weekDay;

        if (this.defaultDay) {
          for (let i = 0; i < weekdayArr.length; i++) {
            if (+weekdayArr[i].key === +this.defaultDay) {
              weekDay = i;
            }
          }
        }

        this.defaultWeekdayLabel =
          weekdayArr[this.defaultDay ? weekDay : 0].label;
        this.selectedWeekdayLabel = this.defaultWeekdayLabel;

        this.defaultWeekday = this.defaultDay;
        this.selectedWeekday = this.defaultWeekday;
      } else {
        this.defaultWeekday = weekdayArr[0].key;
        this.defaultWeekdayLabel = weekdayArr[0].label;

        this.selectedWeekdayLabel = this.defaultWeekdayLabel;
        this.selectedWeekday = this.defaultWeekday;
      }
    } else {
      this.defaultPeriodLabel = periodObj[+this.defaultPeriodNumber].label;
      this.selectedPeriodLabel = this.defaultPeriodLabel;

      this.defaultWeekday = weekdayArr[0].key;
      this.defaultWeekdayLabel = weekdayArr[0].label;

      this.selectedWeekdayLabel = this.defaultWeekdayLabel;
      this.selectedWeekday = this.defaultWeekday;
    }
  };

  setThirdPartyStorage = (list) => {
    this.thirdPartyStorage = list;
  };

  setPreparationPortalDialogVisible = (visible) => {
    this.preparationPortalDialogVisible = visible;
  };

  setBackupSchedule = (backupSchedule) => {
    this.backupSchedule = backupSchedule;
  };

  setPeriod = (options) => {
    const key = options.key;
    const label = options.label;

    this.selectedPeriodLabel = label;
    this.selectedPeriodNumber = `${key}`;
  };

  setWeekday = (options) => {
    const key = options.key;
    const label = options.label;

    this.selectedWeekday = key;
    this.selectedWeekdayLabel = label;
  };

  setMonthNumber = (options) => {
    const key = options.key;
    const label = options.label;

    this.selectedMonthDay = label;
  };

  setTime = (options) => {
    const key = options.key;
    const label = options.label;

    this.selectedHour = label;
  };

  setMaxCopies = (options) => {
    const key = options.key;
    this.selectedMaxCopiesNumber = key;
  };

  seStorageType = (type) => {
    this.selectedStorageType = `${type}`;
  };

  setSelectedFolder = (folderId) => {
    console.log("setSelectedFolder");
    if (folderId !== this.selectedFolderId) this.selectedFolderId = folderId;
  };

  setStorageId = (selectedStorage, defaultStorage) => {
    if (defaultStorage) {
      this.defaultStorageId = defaultStorage;
      this.selectedStorageId = defaultStorage;
    } else {
      this.selectedStorageId = selectedStorage;
    }
    console.log("this.selectedStorageId", this.selectedStorageId);
  };

  clearSessionStorage = () => {
    saveToSessionStorage("LocalCopyStorageType", "");

    getFromSessionStorage("LocalCopyPath") &&
      saveToSessionStorage("LocalCopyPath", "");

    getFromSessionStorage("LocalCopyFolder") &&
      saveToSessionStorage("LocalCopyFolder", "");

    getFromSessionStorage("LocalCopyStorage") &&
      saveToSessionStorage("LocalCopyStorage", "");

    getFromSessionStorage("LocalCopyThirdPartyStorageType") &&
      saveToSessionStorage("LocalCopyThirdPartyStorageType", "");
  };

  getProgress = async (t) => {
    try {
      const response = await getBackupProgress();
      console.log(" this.downloadingProgress", this.downloadingProgress);
      const { progress, link, error } = response;
      if (!error) {
        this.downloadingProgress = progress;

        if (link && link.slice(0, 1) === "/") {
          this.temporaryLink = link;
        }

        if (progress !== 100) {
          this.getIntervalProgress(t);
        } else {
          this.clearSessionStorage();
        }
      }
    } catch (e) {
      toastr.error(`${t("BackupCreatedError")}`);
      this.clearSessionStorage();
    }
  };
  getIntervalProgress = (t) => {
    console.log("this.timerId", this.timerId);
    if (this.timerId) {
      return;
    }

    this.timerId = setInterval(async () => {
      try {
        const response = await getBackupProgress();
        const { progress, link, error } = response;
        console.log("here");
        if (error.length > 0 && progress !== 100) {
          clearInterval(timerId);
          this.timerId && toastr.error(`${t("BackupCreatedError")}`);
          this.timerId = null;
          this.clearSessionStorage();
          this.downloadingProgress = 100;
          return;
        }

        if (progress !== this.downloadingProgress) {
          this.downloadingProgress = progress;
        }

        if (progress === 100) {
          clearInterval(this.timerId);
          this.clearSessionStorage();

          if (link && link.slice(0, 1) === "/") {
            this.temporaryLink = link;
          }

          this.timerId && toastr.success(`${t("BackupCreatedSuccess")}`);
          this.timerId = null;
          console.log("this.timerId success", this.timerId);
          return;
        }
      } catch (e) {
        clearInterval(this.timerId);
        this.clearSessionStorage();
        this.downloadingProgress = 100;
        this.timerId && toastr.error(`${t("BackupCreatedError")}`);
        this.timerId = null;
      }
    }, 1000);
  };

  clearProgressInterval = () => {
    console.log("clearInterval", this.timerId);
    this.timerId && clearInterval(this.timerId);
    this.timerId = null;
  };

  setDownloadingProgress = (progress) => {
    this.downloadingProgress = progress;
  };

  setTemporaryLink = (link) => {
    this.temporaryLink = link;
  };
}

export default BackupStore;
