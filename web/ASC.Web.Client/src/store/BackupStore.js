import api from "@appserver/common/api";
import { makeAutoObservable } from "mobx";

const DOCUMENT_MODULE_TYPE = 0;
const RESOURCES_MODULE_TYPE = 1;
const STORAGES_MODULE_TYPE = 5;

const EVERY_DAY_TYPE = "0";
const EVERY_WEEK_TYPE = "1";
const EVERY_MONTH_TYPE = "2";

class BackupStore {
  backupSchedule = {};
  backupStorage = {};
  commonThirdPartyList = {};

  defaultDay = "0";
  defaultHour = "12:00";
  defaultPeriodNumber = "0";
  defaultPeriodLabel = "Every day";
  defaultMaxCopiesNumber = "10";

  defaultWeekday = null;
  defaultWeekdayLabel = "";
  defaultStorageType = null;
  defaultFolderId = null;

  selectedDay = "0";
  selectedHour = "12:00";
  selectedPeriodNumber = "0";
  selectedPeriodLabel = "Every day";
  selectedMaxCopiesNumber = "10";

  selectedWeekday = null;
  selectedWeekdayLabel = "";
  selectedStorageType = null;
  selectedFolderId = null;

  thirdPartyStorage = [];

  preparationPortalDialogVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

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
        //Every Week option
        let weekDay;

        for (let i = 0; i < weekdayArr.length; i++) {
          if (+weekdayArr[i].key === +this.defaultDay) {
            weekDay = i;
          }
        }

        this.defaultWeekdayLabel = weekdayArr[defaultDay ? weekDay : 0].label;
        this.selectedWeekdayLabel = this.defaultWeekdayLabel;

        this.defaultWeekday = this.defaultDay;
        this.selectedWeekday = this.defaultWeekday;
      } else {
        this.defaultWeekday = weekdayArr[0].key;
        this.defaultWeekdayLabel = weekdayArr[0].label;

        this.selectedWeekdayLabel = this.defaultWeekdayLabel;
        this.selectedWeekday = this.defaultWeekday;
      }
    }
  };

  setThirdPartyStorage = (list) => {
    this.thirdPartyStorage = list;
  };

  setPreparationPortalDialogVisible = (visible) => {
    this.preparationPortalDialogVisible = visible;
  };
}

export default BackupStore;
