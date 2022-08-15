import { getBackupProgress } from "@docspace/common/api/portal";
import { makeAutoObservable } from "mobx";
import {
  saveToSessionStorage,
  getFromSessionStorage,
} from "../pages/PortalSettings/utils";
import toastr from "../helpers/toastr";
import { AutoBackupPeriod } from "@docspace/common/constants";
import api from "@docspace/common/api";

const { EveryDayType, EveryWeekType } = AutoBackupPeriod;

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
  commonThirdPartyList = [];

  preparationPortalDialogVisible = false;

  downloadingProgress = 100;

  temporaryLink = null;
  timerId = null;

  isThirdStorageChanged = false;

  formSettings = {};
  requiredFormSettings = {};
  defaultFormSettings = {};
  errorsFieldsBeforeSafe = {};

  selectedEnableSchedule = false;
  defaultEnableSchedule = false;

  isSavingProcess = false;
  isResetProcess = false;

  storageRegions = [];

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

    this.selectedEnableSchedule = false;
    this.defaultEnableSchedule = false;

    this.setIsThirdStorageChanged(false);
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

    if (this.selectedFolderId !== this.defaultFolderId) return true;

    if (this.selectedStorageId !== this.defaultStorageId) return true;

    if (this.selectedEnableSchedule !== this.defaultEnableSchedule) return true;

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

    this.selectedEnableSchedule = this.defaultEnableSchedule;

    if (this.defaultFormSettings) {
      this.setFormSettings({ ...this.defaultFormSettings });
    }

    this.setIsThirdStorageChanged(false);
  };

  setDefaultOptions = (t, periodObj, weekdayArr) => {
    if (this.backupSchedule) {
      const {
        storageType,
        cronParams,
        backupsStored,
        storageParams,
      } = this.backupSchedule;

      const { folderId, module } = storageParams;
      const { period, day, hour } = cronParams;

      let defaultFormSettings = {};
      for (let variable in storageParams) {
        console.log("variable", variable, "storageParams", storageParams);
        if (variable === "module") continue;
        defaultFormSettings[variable] = storageParams[variable];
      }
      if (defaultFormSettings) {
        this.setFormSettings({ ...defaultFormSettings });
        this.setDefaultFormSettings({ ...defaultFormSettings });
        this.isThirdStorageChanged && this.setIsThirdStorageChanged(false);
      }

      this.defaultEnableSchedule = true;
      this.selectedEnableSchedule = true;
      this.defaultDay = `${day}`;
      this.defaultHour = `${hour}:00`;
      this.defaultPeriodNumber = `${period}`;
      this.defaultMaxCopiesNumber = `${backupsStored}`;
      this.defaultStorageType = `${storageType}`;
      this.defaultFolderId = `${folderId}`;
      if (module) this.defaultStorageId = `${module}`;

      this.selectedDay = this.defaultDay;
      this.selectedHour = this.defaultHour;
      this.selectedPeriodNumber = this.defaultPeriodNumber;
      this.selectedMaxCopiesNumber = this.defaultMaxCopiesNumber;
      this.selectedStorageType = this.defaultStorageType;
      this.selectedFolderId = this.defaultFolderId;

      this.defaultPeriodLabel = periodObj[+this.defaultPeriodNumber].label;
      this.selectedPeriodLabel = this.defaultPeriodLabel;
      if (module) this.selectedStorageId = this.defaultStorageId;

      this.defaultMonthDay =
        +this.defaultPeriodNumber === +EveryWeekType ||
        +this.defaultPeriodNumber === +EveryDayType
          ? "1"
          : this.defaultDay;

      this.selectedMonthDay = this.defaultMonthDay;

      if (+this.defaultPeriodNumber === +EveryWeekType) {
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

    this.setIsThirdStorageChanged(false);
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

  setCommonThirdPartyList = (list) => {
    this.commonThirdPartyList = list;
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
    if (folderId !== this.selectedFolderId) this.selectedFolderId = folderId;
  };

  setStorageId = (selectedStorage) => {
    console.log("setStorageId", selectedStorage);

    this.selectedStorageId = selectedStorage;
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

      if (response) {
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
      }
    } catch (e) {
      toastr.error(t("BackupCreatedError"));
      this.clearSessionStorage();
    }
  };
  getIntervalProgress = (t) => {
    if (this.timerId) {
      return;
    }

    let isWaitRequest = false;
    this.timerId = setInterval(async () => {
      try {
        if (isWaitRequest) {
          return;
        }

        isWaitRequest = true;

        const response = await getBackupProgress();

        if (response) {
          const { progress, link, error } = response;

          if (error.length > 0 && progress !== 100) {
            clearInterval(timerId);
            this.timerId && toastr.error(t("BackupCreatedError"));
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

            this.timerId && toastr.success(t("BackupCreatedSuccess"));
            this.timerId = null;

            return;
          }
        } else {
          this.timerId && toastr.error(t("BackupCreatedError"));
          this.downloadingProgress = 100;
          clearInterval(this.timerId);
          this.clearSessionStorage();
          this.timerId = null;
          return;
        }

        isWaitRequest = false;
      } catch (e) {
        clearInterval(this.timerId);
        this.clearSessionStorage();
        this.downloadingProgress = 100;
        this.timerId && toastr.error(t("BackupCreatedError"));
        this.timerId = null;
      }
    }, 1000);
  };

  clearProgressInterval = () => {
    this.timerId && clearInterval(this.timerId);
    this.timerId = null;
  };

  setDownloadingProgress = (progress) => {
    this.downloadingProgress = progress;
  };

  setTemporaryLink = (link) => {
    this.temporaryLink = link;
  };

  setFormSettings = (obj) => {
    this.formSettings = obj;
    console.log(" this.formSettings", this.formSettings);
  };

  addValueInFormSettings = (name, value) => {
    this.setFormSettings({ ...this.formSettings, [name]: value });
  };

  deleteValueFormSetting = (key) => {
    delete this.formSettings[key];
    console.log(" this.formSettings", this.defaultFormSettings);
  };
  getStorageParams = (
    isCheckedThirdPartyStorage,
    selectedFolderId,
    selectedStorageId
  ) => {
    let storageParams = [
      {
        key: isCheckedThirdPartyStorage ? "module" : "folderId",
        value: isCheckedThirdPartyStorage
          ? selectedStorageId
          : selectedFolderId,
      },
    ];

    if (isCheckedThirdPartyStorage) {
      const arraySettings = Object.entries(this.formSettings);

      for (let i = 0; i < arraySettings.length; i++) {
        const tmpObj = {
          key: arraySettings[i][0],
          value: arraySettings[i][1],
        };

        storageParams.push(tmpObj);
      }
    }

    return storageParams;
  };

  setRequiredFormSettings = (array) => {
    this.requiredFormSettings = array;
  };

  setStorageRegions = (regions) => {
    this.storageRegions = regions;
  };
  setDefaultFormSettings = (obj) => {
    this.defaultFormSettings = obj;
  };

  get isValidForm() {
    if (Object.keys(this.requiredFormSettings).length == 0) return;

    for (let key of this.requiredFormSettings) {
      const elem = this.formSettings[key];
      if (!elem) return false;
      if (!elem.trim()) return false;
    }
    return true;
  }
  isFormReady = () => {
    let errors = {};
    let firstError = false;

    for (let key of this.requiredFormSettings) {
      const elem = this.formSettings[key];

      errors[key] = !elem.trim();
      // console.log("elem.trim()", elem.trim(), "firstError", firstError);
      if (!elem.trim() && !firstError) {
        firstError = true;
      }
    }
    this.setErrorsFormFields(errors);

    return !firstError;
  };

  setErrorsFormFields = (errors) => {
    this.errorsFieldsBeforeSafe = errors;
  };
  setCompletedFormFields = (values, module) => {
    let formSettingsTemp = {};

    if (module && module === this.defaultStorageId) {
      this.setFormSettings({ ...this.defaultFormSettings });
      return;
    }

    for (const [key, value] of Object.entries(values)) {
      formSettingsTemp[key] = value;
    }

    this.setFormSettings({ ...formSettingsTemp });
    this.setErrorsFormFields({});
  };

  setIsThirdStorageChanged = (changed) => {
    if (changed !== this.isThirdStorageChanged) {
      this.isThirdStorageChanged = changed;
    }
  };

  setSelectedEnableSchedule = () => {
    const isEnable = this.selectedEnableSchedule;
    this.selectedEnableSchedule = !isEnable;
  };
  setSavingProcess = (process) => {
    if (process !== this.isSavingProcess) this.isSavingProcess = process;
  };

  setResetProcess = (process) => {
    if (process !== this.isResetProcess) this.isResetProcess = process;
  };

  convertServiceName = (serviceName) => {
    //Docusign, OneDrive, Wordpress
    switch (serviceName) {
      case "GoogleDrive":
        return "google";
      case "Box":
        return "box";
      case "DropboxV2":
        return "dropbox";
      case "OneDrive":
        return "onedrive";
      default:
        return "";
    }
  };

  oAuthPopup = (url, modal) => {
    let newWindow = modal;

    if (modal) {
      newWindow.location = url;
    }

    try {
      let params =
        "height=600,width=1020,resizable=0,status=0,toolbar=0,menubar=0,location=1";
      newWindow = modal ? newWindow : window.open(url, "Authorization", params);
    } catch (err) {
      newWindow = modal ? newWindow : window.open(url, "Authorization");
    }

    return newWindow;
  };
  openConnectWindow = (serviceName, modal) => {
    const service = this.convertServiceName(serviceName);
    console.log("service", service);
    return api.files.openConnectWindow(service).then((link) => {
      console.log("link", link);
      return this.oAuthPopup(link, modal);
    });
  };

  getOAuthToken = (tokenGetterWin) => {
    return new Promise((resolve, reject) => {
      localStorage.removeItem("code");
      let interval = null;
      interval = setInterval(() => {
        try {
          const code = localStorage.getItem("code");
          console.log("code", code);
          if (code) {
            localStorage.removeItem("code");
            clearInterval(interval);
            resolve(code);
            console.log("code", code);
          } else if (tokenGetterWin && tokenGetterWin.closed) {
            clearInterval(interval);
            reject();
          }
        } catch (e) {
          clearInterval(interval);
          reject(e);
          console.log("code catch", code);
        }
      }, 500);
    });
  };
}

export default BackupStore;
