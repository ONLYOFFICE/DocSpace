import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButton from "@appserver/components/radio-button";
import moment from "moment";
import Button from "@appserver/components/button";
import {
  deleteBackupSchedule,
  getBackupProgress,
  getBackupSchedule,
  createBackupSchedule,
} from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import SelectFolderDialog from "files/SelectFolderDialog";
import Loader from "@appserver/components/loader";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { StyledModules, StyledAutoBackup } from "../StyledBackup";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import DocumentsModule from "./sub-components/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";
import ToggleButton from "@appserver/components/toggle-button";
import { getBackupStorage } from "@appserver/common/api/settings";

const { proxyURL } = AppServerConfig;

const DOCUMENT_MODULE_TYPE = 0;
const RESOURCES_MODULE_TYPE = 1;
const STORAGES_MODULE_TYPE = 5;

const EVERY_DAY_TYPE = 0;
const EVERY_WEEK_TYPE = 1;
const EVERY_MONTH_TYPE = 2;
class AutomaticBackup extends React.PureComponent {
  constructor(props) {
    super(props);
    const { t, language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this._isMounted = false;

    this.state = {
      isCheckedDocuments: false,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,

      defaultMonthlySchedule: false,
      defaultDailySchedule: false,
      defaultWeeklySchedule: false,

      selectedMonthlySchedule: false,
      selectedDailySchedule: false,
      selectedWeeklySchedule: false,

      isEnable: false,

      defaultStorageTypeNumber: "",
      defaultHour: "",
      defaultPeriodNumber: "",
      defaultDay: "",
      defaultMaxCopiesNumber: "",
      defaultWeekdayLabel: "",
      defaultPeriodLabel: "",
      defaultMonthDay: "",
      defaultWeekday: "",

      selectedPeriodLabel: "",
      selectedWeekdayLabel: "",
      selectedWeekday: "",
      selectedHour: "",
      selectedMonthDay: "",
      selectedMaxCopiesNumber: "",
      selectedStorageTypeNumber: "",

      isLoadingData: false,
      isInitialLoading: true,
      isChanged: false,
      isChangedInStorage: false,
      isReset: false,
      isSuccessSave: false,
      downloadingProgress: 100,
    };

    this.periodsObject = [
      {
        key: 0,
        label: t("DailyPeriodSchedule"),
      },
      {
        key: 1,
        label: t("WeeklyPeriodSchedule"),
      },
      {
        key: 2,
        label: t("MonthlyPeriodSchedule"),
      },
    ];

    this._isMounted = false;
    this.hoursArray = [];
    this.monthNumbersArray = [];
    this.maxNumberCopiesArray = [];
    this.weekdaysLabelArray = [];
    this.timerId = null;
    this.formSettings = "";
    this.selectedFolder = "";
    this.defaultSelectedFolder = "";
    this.selectedSchedule = false;
    this.storageId = "";
    this.defaultStorageId = "";
    this.storageInfo = "";

    this.getTime();
    this.getMonthNumbers();
    this.getMaxNumberCopies();
  }

  setBasicSettings = async () => {
    const { downloadingProgress } = this.state;
    try {
      const [
        commonThirdPartyList,
        backupProgress,
        backupSchedule,
        storageInfo,
      ] = await Promise.all([
        SelectFolderDialog.getCommonThirdPartyList(),
        getBackupProgress(),
        getBackupSchedule(),
        getBackupStorage(),
      ]);

      this.commonThirdPartyList = commonThirdPartyList;
      this.storageInfo = storageInfo;

      if (backupProgress && !backupProgress.error) {
        const { progress } = backupProgress;
        this._isMounted && downloadingProgress !== progress;
        this.setState({
          downloadingProgress: progress,
        });

        if (progress !== 100 && !this.timerId) {
          this.timerId = setInterval(() => this.getProgress(), 1000);
        }
      }

      if (backupSchedule) {
        this.onSetDefaultOptions(backupSchedule);
        this.selectedSchedule = true;
      } else {
        this.onSetDefaultOptions();
      }
    } catch (error) {
      toastr.error(error);
    }
  };

  componentDidMount() {
    this._isMounted = true;
    this.getWeekdays();
    this.setBasicSettings();
  }

  componentDidUpdate(prevProps, prevState) {
    const {
      isChangedInStorage,
      isChanged,
      isSuccessSave,
      isReset,
    } = this.state;

    if (
      (isChangedInStorage !== prevState.isChangedInStorage ||
        isChanged !== prevState.isChanged) &&
      isSuccessSave
    ) {
      this.setState({ isSuccessSave: false });
    }
    if (
      (isChangedInStorage !== prevState.isChangedInStorage ||
        isChanged !== prevState.isChanged) &&
      isReset
    ) {
      this.setState({ isReset: false });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  onSetDefaultOptions = (selectedSchedule, savingProcess = false) => {
    let checkedStorage = {};
    let defaultOptions, selectedOptions;

    if (selectedSchedule) {
      const {
        storageType,
        cronParams,
        backupsStored,
        storageParams,
      } = selectedSchedule;
      const { folderId } = storageParams;
      const { period, day, hour } = cronParams;

      const isThirdPartyStorage = storageType === STORAGES_MODULE_TYPE;

      this.defaultSelectedFolder = !isThirdPartyStorage ? `${folderId}` : "";
      this.selectedFolder = this.defaultSelectedFolder;

      defaultOptions = {
        defaultStorageTypeNumber: `${storageType}`,
        defaultHour: `${hour}:00`,
        defaultPeriodNumber: `${period}`,
        defaultDay: `${day}`,
        defaultMaxCopiesNumber: `${backupsStored}`,
        isEnable: true,
      };

      selectedOptions = {
        selectedStorageTypeNumber: defaultOptions.defaultStorageTypeNumber,
        selectedHour: defaultOptions.defaultHour,
        selectedMaxCopiesNumber: defaultOptions.defaultMaxCopiesNumber,
      };
    } else {
      this.defaultSelectedFolder = "";
      this.selectedFolder = this.defaultSelectedFolder;

      defaultOptions = {
        defaultStorageTypeNumber: "0",
        defaultHour: "12:00",
        defaultPeriodNumber: "0",
        defaultDay: "0",
        defaultMaxCopiesNumber: "10",
      };

      selectedOptions = {
        selectedStorageTypeNumber: defaultOptions.defaultStorageTypeNumber,
        selectedHour: defaultOptions.defaultHour,
        selectedMaxCopiesNumber: defaultOptions.defaultMaxCopiesNumber,
      };
    }

    const { defaultStorageTypeNumber } = defaultOptions;

    if (+defaultStorageTypeNumber === DOCUMENT_MODULE_TYPE) {
      // Documents Module
      checkedStorage.isCheckedDocuments = true;
    } else {
      if (+defaultStorageTypeNumber === RESOURCES_MODULE_TYPE) {
        // ThirdPartyResource Module
        checkedStorage.isCheckedThirdParty = true;
      } else {
        if (+defaultStorageTypeNumber === STORAGES_MODULE_TYPE) {
          // ThirdPartyStorage Module
          checkedStorage.isCheckedThirdPartyStorage = true;
        }
      }
    }

    this.onSetDefaultPeriodOption(
      checkedStorage,
      defaultOptions,
      selectedOptions,
      savingProcess
    );
  };

  onSetDefaultPeriodOption = (
    checkedStorage,
    defaultOptions,
    selectedOptions,
    savingProcess
  ) => {
    const { defaultPeriodNumber, defaultDay, isEnable } = defaultOptions;

    const defaultPeriodSettings = this.setDefaultPeriodSettings(
      +defaultPeriodNumber
    );

    const resetOptions =
      savingProcess && this._isMounted
        ? {
            isLoadingData: false,
            isChanged: false,
            isChangedInStorage: false,
            isError: false,
            isErrorsFields: false,
            isSuccessSave: true,
          }
        : {};

    if (+defaultPeriodNumber === EVERY_WEEK_TYPE) {
      //Every Week option
      let weekDay;
      const periodLabel = this.periodsObject[EVERY_WEEK_TYPE].label;

      for (let i = 0; i < this.weekdaysLabelArray.length; i++) {
        if (+this.weekdaysLabelArray[i].key === +defaultDay) {
          weekDay = i;
        }
      }

      const weekDayLabel = this.weekdaysLabelArray[defaultDay ? weekDay : 0]
        .label;

      this._isMounted &&
        this.setState({
          ...checkedStorage,
          defaultPeriodLabel: periodLabel,
          selectedPeriodLabel: periodLabel,

          defaultWeekdayLabel: weekDayLabel,
          selectedWeekdayLabel: weekDayLabel,
          selectedWeekday: defaultDay,
          defaultWeekday: defaultDay,

          selectedMonthDay: "1",
          defaultMonthDay: "1",

          isInitialLoading: false,
          ...defaultPeriodSettings,
          ...defaultOptions,
          ...selectedOptions,
          ...resetOptions,
        });
    } else {
      const weekDay = this.weekdaysLabelArray[0].key;
      const weekDayLabel = this.weekdaysLabelArray[0].label;

      if (+defaultPeriodNumber === EVERY_MONTH_TYPE) {
        //Every Month option
        const periodLabel = this.periodsObject[EVERY_MONTH_TYPE].label;

        this._isMounted &&
          this.setState({
            ...checkedStorage,
            defaultPeriodLabel: periodLabel,
            selectedPeriodLabel: periodLabel,

            defaultWeekdayLabel: weekDayLabel,
            selectedWeekdayLabel: weekDayLabel,
            selectedWeekday: weekDay,
            defaultWeekday: weekDay,

            selectedMonthDay: defaultDay,
            defaultMonthDay: defaultDay,

            isInitialLoading: false,
            ...defaultPeriodSettings,
            ...defaultOptions,
            ...selectedOptions,
            ...resetOptions,
          });
      } else {
        //Every Day option
        const periodLabel = this.periodsObject[EVERY_DAY_TYPE].label;

        this._isMounted &&
          this.setState({
            ...checkedStorage,
            defaultPeriodLabel: periodLabel,
            selectedPeriodLabel: periodLabel,

            defaultWeekdayLabel: weekDayLabel,
            selectedWeekdayLabel: weekDayLabel,
            selectedWeekday: weekDay,
            defaultWeekday: weekDay,

            selectedMonthDay: "1",
            defaultMonthDay: "1",

            isInitialLoading: false,
            ...defaultPeriodSettings,
            ...defaultOptions,
            ...selectedOptions,
            ...resetOptions,
          });
      }
    }
  };

  getProgress = () => {
    const { t } = this.props;
    const { downloadingProgress } = this.state;
    getBackupProgress()
      .then((res) => {
        if (res) {
          const { error, progress } = res;
          if (error.length > 0 && progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${error}`);
            this.timerId = null;
            this.setState({
              downloadingProgress: 100,
            });
            return;
          }
          this._isMounted &&
            downloadingProgress !== progress &&
            this.setState({
              downloadingProgress: progress,
            });

          if (progress === 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;
          }
        } else {
          clearInterval(this.timerId);
          this.timerId && toastr.success(`${t("SuccessCopied")}`);
          this.timerId = null;

          this._isMounted &&
            this.setState({
              downloadingProgress: 100,
            });
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(err);
        this.timerId = null;

        this._isMounted &&
          this.setState({
            downloadingProgress: 100,
          });
      });
  };

  getTime = () => {
    for (let item = 0; item < 24; item++) {
      let obj = {
        key: item,
        label: `${item}:00`,
      };
      this.hoursArray.push(obj);
    }
  };

  getMonthNumbers = () => {
    for (let item = 1; item <= 31; item++) {
      let obj = {
        key: item,
        label: `${item}`,
      };
      this.monthNumbersArray.push(obj);
    }
  };

  getMaxNumberCopies = () => {
    const { t } = this.props;
    for (let item = 1; item <= 30; item++) {
      let obj = {
        key: `${item}`,
        label: `${item} ${t("MaxCopies")}`,
      };
      this.maxNumberCopiesArray.push(obj);
    }
  };
  getWeekdays = () => {
    const gettingWeekdays = moment.weekdays();

    for (let item = 0; item < gettingWeekdays.length; item++) {
      let obj = {
        key: `${item + 1}`,
        label: `${gettingWeekdays[item]}`,
      };
      this.weekdaysLabelArray.push(obj);
    }

    const isEnglishLanguage = this.lng === "en";

    if (!isEnglishLanguage) {
      this.weekdaysLabelArray.push(this.weekdaysLabelArray.shift());
    }
  };

  onClickPermissions = () => {
    const { isEnable } = this.state;

    const resetModuleSettings = this.resetModuleSettings(
      isEnable ? -1 : DOCUMENT_MODULE_TYPE.toString()
    );

    console.log("onClickPermissions resetModuleSettings", resetModuleSettings);
    this.setState(
      {
        isEnable: !isEnable,
        ...resetModuleSettings,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectMaxCopies = (options) => {
    const key = options.key;

    this.setState(
      {
        selectedMaxCopiesNumber: key,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectWeekDay = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState(
      {
        selectedWeekday: key,
        selectedWeekdayLabel: label,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectMonthNumber = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState(
      {
        selectedMonthDay: label,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectTime = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState({ selectedHour: label }, function () {
      this.checkChanges();
    });
  };

  setDefaultPeriodSettings = (key = -1) => {
    const titleArray = [
      [EVERY_DAY_TYPE, "Daily"],
      [EVERY_WEEK_TYPE, "Weekly"],
      [EVERY_MONTH_TYPE, "Monthly"],
    ];

    let resultObj = {};

    for (let i = 0; i < titleArray.length; i++) {
      if (+key === titleArray[i][0]) {
        resultObj[`default${titleArray[i][1]}Schedule`] = true;
        resultObj[`selected${titleArray[i][1]}Schedule`] = true;
      } else {
        resultObj[`default${titleArray[i][1]}Schedule`] = false;
        resultObj[`selected${titleArray[i][1]}Schedule`] = false;
      }
    }
    return resultObj;
  };

  resetPeriodSettings = (key = -1) => {
    const titleArray = [
      [EVERY_DAY_TYPE, "Daily"],
      [EVERY_WEEK_TYPE, "Weekly"],
      [EVERY_MONTH_TYPE, "Monthly"],
    ];

    let resultObj = {};

    for (let i = 0; i < titleArray.length; i++) {
      console.log("key", key, "i", i);
      if (+key === titleArray[i][0]) {
        resultObj[`selected${titleArray[i][1]}Schedule`] = true;
      } else {
        resultObj[`selected${titleArray[i][1]}Schedule`] = false;
        if (+key === -1)
          resultObj[`default${titleArray[i][1]}Schedule`] = false;
      }
    }
    return resultObj;
  };

  onSelectPeriod = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState({ selectedPeriodLabel: label });

    const resetOptions = this.resetPeriodSettings(key);

    console.log("onSelectPeriod resetOptions", resetOptions);

    this.setState(
      {
        ...resetOptions,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  checkChanges = () => {
    const {
      isChanged,
      defaultStorageTypeNumber,
      selectedStorageTypeNumber,
      isEnable,
    } = this.state;

    if (
      defaultStorageTypeNumber !== selectedStorageTypeNumber ||
      (this.selectedSchedule && !isEnable) ||
      (!this.selectedSchedule && isEnable)
    ) {
      !isChanged &&
        this.setState({
          isChanged: true,
        });
      return;
    } else {
      const changed = this.checkOptions();
      isChanged !== changed &&
        this.setState({
          isChanged: changed,
        });
      return;
    }
  };
  checkOptions = () => {
    const {
      defaultHour,
      defaultDay,
      defaultMaxCopiesNumber,
      defaultWeekdayLabel,

      defaultWeeklySchedule,
      defaultDailySchedule,
      defaultMonthlySchedule,

      selectedMaxCopiesNumber,
      selectedHour,
      selectedMonthDay,
      selectedWeekdayLabel,
      selectedMonthlySchedule,
      selectedDailySchedule,
      selectedWeeklySchedule,
    } = this.state;

    if (selectedHour !== defaultHour) {
      return true;
    }

    if (+selectedMaxCopiesNumber !== +defaultMaxCopiesNumber) {
      return true;
    }

    if (selectedDailySchedule !== defaultDailySchedule) {
      return true;
    }

    if (selectedMonthlySchedule) {
      if (selectedMonthlySchedule !== defaultMonthlySchedule) return true;
      if (selectedMonthDay !== defaultDay) {
        return true;
      }
    }

    if (selectedWeeklySchedule) {
      if (selectedWeeklySchedule !== defaultWeeklySchedule) return true;
      if (selectedWeekdayLabel !== defaultWeekdayLabel) {
        return true;
      }
    }
    if (this.selectedFolder !== this.defaultSelectedFolder) return true;

    return false;
  };

  resetModuleSettings = (key = -1) => {
    console.log("key", key);
    const arrayType = [
      [DOCUMENT_MODULE_TYPE, "Documents"],
      [RESOURCES_MODULE_TYPE, "ThirdParty"],
      [STORAGES_MODULE_TYPE, "ThirdPartyStorage"],
    ];
    const lastElem = arrayType.length - 1;
    let resultObj = {};

    for (let i = 0; i < arrayType.length; i++) {
      if (i === lastElem && +key === STORAGES_MODULE_TYPE) {
        resultObj[`isChecked${arrayType[lastElem][1]}`] = true;
        resultObj["selectedStorageTypeNumber"] = key.toString();
      } else {
        if (+key === arrayType[i][0]) {
          resultObj[`isChecked${arrayType[i][1]}`] = true;
          resultObj["selectedStorageTypeNumber"] = key.toString();
        } else {
          resultObj[`isChecked${arrayType[i][1]}`] = false;
        }
      }
    }

    return resultObj;
  };
  onClickShowStorage = (e) => {
    const key = e.target.name;

    const options = this.resetModuleSettings(key);
    console.log("onClickShowStorage resetModuleSettings", options);
    this.selectedFolder = "";
    this.setState(
      {
        ...options,
        isError: false,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onCancelModuleSettings = () => {
    const {
      isError,
      defaultStorageTypeNumber,
      defaultMonthlySchedule,
      defaultWeeklySchedule,
      defaultHour,
      defaultPeriodLabel,
      defaultWeekdayLabel,
      defaultMaxCopiesNumber,
      defaultDailySchedule,

      defaultWeekday,
      defaultMonthDay,
      isEnable,
      isErrorsFields,
    } = this.state;

    let storageState = {};

    if (!this.selectedSchedule && isEnable) {
      this.setState({ isEnable: false });
    }
    if (this.selectedSchedule && !isEnable) {
      this.setState({ isEnable: true });
    }
    storageState = this.resetModuleSettings(defaultStorageTypeNumber);
    console.log("onCancelModuleSettings storageObj", storageState);
    this.selectedFolder = this.defaultSelectedFolder;
    this.setState({
      selectedMonthlySchedule: defaultMonthlySchedule,
      selectedWeeklySchedule: defaultWeeklySchedule,
      selectedDailySchedule: defaultDailySchedule,
      selectedHour: defaultHour,
      selectedPeriodLabel: defaultPeriodLabel,
      selectedWeekdayLabel: defaultWeekdayLabel,
      selectedMaxCopiesNumber: defaultMaxCopiesNumber,
      selectedStorageTypeNumber: defaultStorageTypeNumber,
      selectedMonthDay: defaultMonthDay,
      selectedWeekday: defaultWeekday,
      ...(isError && { isError: false }),
      ...(isErrorsFields && { isErrorsFields: false }),
      isChanged: false,
      isChangedInStorage: false,
      isReset: true,
      ...storageState,
    });
  };

  canSave = () => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    if (
      (isCheckedDocuments && !this.selectedFolder) ||
      (isCheckedThirdParty && !this.selectedFolder)
    ) {
      this.setState({
        isError: true,
      });
      return false;
    }

    if (isCheckedThirdPartyStorage) {
      let errors = {};
      let firstError = false;

      for (let key of this.formNames) {
        const field = this.formSettings[key];

        if (!field) {
          if (!firstError) {
            firstError = true;
          }
          errors[key] = true;
        } else {
          if (!firstError && !field.trim()) {
            firstError = true;
          }
          errors[key] = !field.trim();
          console.log("errors[key]", errors[key]);
        }
      }

      if (firstError) {
        this.setState({
          isErrorsFields: errors,
        });
        return false;
      }
      return true;
    }
    return true;
  };
  onSaveModuleSettings = async () => {
    const {
      selectedMaxCopiesNumber,
      selectedMonthlySchedule,
      selectedWeeklySchedule,
      selectedWeekday,
      selectedHour,
      selectedMonthDay,

      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isEnable,
    } = this.state;

    if (!isEnable) {
      this.deleteSchedule();
      return;
    }

    if (!this.canSave()) return;

    this.setState({ isLoadingData: true }, function () {
      let day, period;

      if (selectedWeeklySchedule) {
        period = EVERY_WEEK_TYPE;
        day = selectedWeekday;
      } else {
        if (selectedMonthlySchedule) {
          period = EVERY_MONTH_TYPE;
          day = selectedMonthDay;
        } else {
          period = EVERY_DAY_TYPE;
          day = null;
        }
      }

      let time = selectedHour.substring(0, selectedHour.indexOf(":"));

      const storageType = isCheckedDocuments
        ? DOCUMENT_MODULE_TYPE
        : isCheckedThirdParty
        ? RESOURCES_MODULE_TYPE
        : STORAGES_MODULE_TYPE;

      const storageParams = [
        {
          key: isCheckedThirdPartyStorage ? "module" : "folderId",
          value: isCheckedThirdPartyStorage
            ? this.storageId
            : this.selectedFolder,
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

      this.createSchedule(
        storageType.toString(),
        storageParams,
        selectedMaxCopiesNumber,
        period.toString(),
        time,
        day?.toString()
      );
    });
  };

  createSchedule = async (
    storageType,
    storageParams,
    selectedMaxCopiesNumber,
    period,
    time,
    day
  ) => {
    const { t } = this.props;

    try {
      await createBackupSchedule(
        storageType,
        storageParams,
        selectedMaxCopiesNumber,
        period,
        time,
        day
      );

      const [selectedSchedule, storageInfo] = await Promise.all([
        getBackupSchedule(),
        getBackupStorage(),
      ]);
      this.storageInfo = storageInfo;
      this.selectedSchedule = true;

      this.onSetDefaultOptions(selectedSchedule, true);
      toastr.success(t("SuccessfullySaveSettingsMessage"));
    } catch (e) {
      toastr.error(e);

      this._isMounted &&
        this.setState({
          isLoadingData: false,
        });
    }
  };

  deleteSchedule = () => {
    const { t } = this.props;
    this.setState({ isLoadingData: true }, () => {
      deleteBackupSchedule()
        .then(() => {
          this.onSetDefaultOptions(null, true);
          this.selectedSchedule = false;
          toastr.success(t("SuccessfullySaveSettingsMessage"));
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({
            isLoadingData: false,
            isChanged: false,
            isChangedInStorage: false,
          });
        });
    });
  };

  onSelectFolder = (folderId) => {
    this.selectedFolder = folderId;
    this.checkChanges();
  };

  onSetLoadingData = (loading) => {
    const { isLoadingData } = this.state;

    isLoadingData !== loading &&
      this.setState({
        isLoadingData: loading,
      });
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(proxyURL, "/settings/datamanagement/backup/manual-backup")
    );
  };

  onSetIsChanged = (changed) => {
    this.setState({
      isChanged: changed,
    });
  };

  onSetIsChangedInStorage = (changed) => {
    this.setState({
      isChangedInStorage: changed,
    });
  };
  onSetFormSettings = (name, value, initialObj) => {
    if (!initialObj) {
      this.formSettings = {
        ...this.formSettings,
        ...{ [name]: value },
      };
    } else {
      this.formSettings = {
        ...initialObj,
      };
    }
    console.log("this.formSettings", this.formSettings);
  };

  onSetStorageId = (storageId) => {
    this.storageId = storageId;
  };

  onSetRequiredFormNames = (namesArray) => {
    this.formNames = namesArray;
  };

  render() {
    const { t } = this.props;
    const {
      isInitialLoading,
      isChanged,
      downloadingProgress,
      isEnable,
      isReset,

      isCheckedDocuments,
      isCheckedThirdParty,
      isLoadingData,
      defaultStorageTypeNumber,
      isCheckedThirdPartyStorage,
      isError,
      isSuccessSave,
      isErrorsFields,
      isChangedInStorage,
    } = this.state;

    const isThirdPartyDefault =
      +defaultStorageTypeNumber === RESOURCES_MODULE_TYPE;

    const isDisabledThirdPartyList =
      this.commonThirdPartyList && this.commonThirdPartyList.length === 0;

    const commonProps = {
      selectedMonthlySchedule: this.state.selectedMonthlySchedule,
      selectedWeeklySchedule: this.state.selectedWeeklySchedule,
      selectedHour: this.state.selectedHour,
      selectedPeriodLabel: this.state.selectedPeriodLabel,
      selectedMonthDay: this.state.selectedMonthDay,
      selectedWeekdayLabel: this.state.selectedWeekdayLabel,
      selectedMaxCopiesNumber: this.state.selectedMaxCopiesNumber,
      isLoadingData,
      isReset,
      isSuccessSave,
      monthNumbersArray: this.monthNumbersArray,
      hoursArray: this.hoursArray,
      maxNumberCopiesArray: this.maxNumberCopiesArray,
      periodsObject: this.periodsObject,
      weekdaysLabelArray: this.weekdaysLabelArray,

      onSelectPeriod: this.onSelectPeriod,
      onSelectWeekDay: this.onSelectWeekDay,
      onSelectMonthNumber: this.onSelectMonthNumber,
      onSelectTime: this.onSelectTime,
      onSelectMaxCopies: this.onSelectMaxCopies,
      onSelectFolder: this.onSelectFolder,
      onSetLoadingData: this.onSetLoadingData,
    };

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };
    console.log("render auto ", this.state);

    const isCopyingToLocal = downloadingProgress !== 100;

    return isInitialLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledAutoBackup>
        <div className="backup_toggle-wrapper">
          <ToggleButton
            className="backup_toggle-btn"
            label={t("EnableAutomaticBackup")}
            onChange={this.onClickPermissions}
            isChecked={isEnable}
            isDisabled={isLoadingData}
          />
          <Text className="backup_toggle-btn-description">
            {t("DataRestoreDescription")}
          </Text>
        </div>
        {isEnable && (
          <div className="backup_modules">
            <StyledModules>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("DocumentsModule")}
                name={`${DOCUMENT_MODULE_TYPE}`}
                key={0}
                isChecked={isCheckedDocuments}
                isDisabled={isLoadingData}
              />
              <Text className="backup-description">
                {t("DocumentsModuleDescription")}
              </Text>
              {isCheckedDocuments && (
                <DocumentsModule
                  {...commonProps}
                  isThirdPartyDefault={isThirdPartyDefault}
                  isError={isError}
                  defaultSelectedFolder={this.defaultSelectedFolder}
                  onSetDefaultFolderPath={this.onSetDefaultFolderPath}
                />
              )}
            </StyledModules>

            <StyledModules isDisabled={isDisabledThirdPartyList}>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("ThirdPartyResource")}
                name={`${RESOURCES_MODULE_TYPE}`}
                isChecked={isCheckedThirdParty}
                isDisabled={isLoadingData || isDisabledThirdPartyList}
              />
              <Text className="backup-description">
                {t("ThirdPartyResourceDescription")}
              </Text>

              {isCheckedThirdParty && (
                <ThirdPartyModule
                  {...commonProps}
                  isThirdPartyDefault={isThirdPartyDefault}
                  isError={isError}
                  defaultSelectedFolder={this.defaultSelectedFolder}
                  onSetDefaultFolderPath={this.onSetDefaultFolderPath}
                />
              )}
            </StyledModules>

            <StyledModules>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("ThirdPartyStorage")}
                name={`${STORAGES_MODULE_TYPE}`}
                isChecked={isCheckedThirdPartyStorage}
                isDisabled={isLoadingData}
              />
              <Text className="backup-description">
                {t("ThirdPartyStorageDescription")}
              </Text>

              {isCheckedThirdPartyStorage && (
                <ThirdPartyStorageModule
                  {...commonProps}
                  onSetIsChanged={this.onSetIsChangedInStorage}
                  onSetFormSettings={this.onSetFormSettings}
                  onSetStorageId={this.onSetStorageId}
                  onSetRequiredFormNames={this.onSetRequiredFormNames}
                  isErrorsFields={isErrorsFields}
                  storageInfo={this.storageInfo}
                />
              )}
            </StyledModules>
          </div>
        )}

        {(isChangedInStorage || isChanged) && (
          <>
            <Button
              label={t("Common:SaveButton")}
              onClick={this.onSaveModuleSettings}
              primary
              isDisabled={isCopyingToLocal || isLoadingData}
              size="medium"
              className="save-button"
            />

            <Button
              label={t("Common:CancelButton")}
              isDisabled={isLoadingData}
              onClick={this.onCancelModuleSettings}
              size="medium"
            />
          </>
        )}
        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
            onClick={this.onClickFloatingButton}
          />
        )}
      </StyledAutoBackup>
    );
  }
}
export default inject(({ auth }) => {
  const { language } = auth;

  return {
    language,
  };
})(withTranslation(["Settings", "Common"])(observer(AutomaticBackup)));
