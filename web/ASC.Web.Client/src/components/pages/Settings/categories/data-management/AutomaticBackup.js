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
import { StyledModules, StyledAutoBackup } from "./StyledBackup";
import ThirdPartyModule from "./sub-components-automatic-backup/ThirdPartyModule";
import DocumentsModule from "./sub-components-automatic-backup/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components-automatic-backup/ThirdPartyStorageModule";
import ToggleButton from "@appserver/components/toggle-button";

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

      defaultSelectedFolder: "",
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

      isCopyingToLocal: true,
      isLoadingData: false,
      isLoading: false,
      isChanged: false,

      downloadingProgress: 100,

      selectedFolder: "",
    };
    this.selectedSchedule = false;
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

    this.hoursArray = [];
    this.getTime();
    this.monthNumbersArray = [];
    this.getMonthNumbers();
    this.maxNumberCopiesArray = [];
    this.getMaxNumberCopies();
    this.weekdaysLabelArray = [];

    this._isMounted = false;
    this.timerId = null;
  }

  componentDidMount() {
    this._isMounted = true;
    this.getWeekdays();

    getBackupProgress().then((response) => {
      if (response) {
        if (!response.error) {
          if (response.progress === 100)
            this.setState({
              isCopyingToLocal: false,
            });
          if (response.progress !== 100)
            this.timerId = setInterval(() => this.getProgress(), 1000);
        } else {
          this.setState({
            isCopyingToLocal: false,
          });
        }
      } else {
        this._isMounted &&
          this.setState({
            isCopyingToLocal: false,
          });
      }
    });

    this.setState({ isLoading: true }, function () {
      SelectFolderDialog.getCommonThirdPartyList()
        .then(
          (thirdPartyArray) => (this.commonThirdPartyList = thirdPartyArray)
        )
        .then(() => getBackupSchedule())

        .then((selectedSchedule) => {
          if (selectedSchedule) {
            this.onSetDefaultOptions(selectedSchedule);
            this.selectedSchedule = true;
            this.setState({ isEnable: true });
          } else {
            this.onSetDefaultOptions();
            this._isMounted &&
              this.setState({
                isLoading: false,
              });
          }
        });
    });
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  onSetDefaultOptions = (selectedSchedule) => {
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

      defaultOptions = {
        defaultSelectedFolder: !isThirdPartyStorage ? `${folderId}` : "",
        defaultStorageTypeNumber: `${storageType}`,
        defaultHour: `${hour}:00`,
        defaultPeriodNumber: `${period}`,
        defaultDay: `${day}`,
        defaultMaxCopiesNumber: `${backupsStored}`,
      };

      selectedOptions = {
        selectedFolder: defaultOptions.defaultSelectedFolder,
        selectedStorageTypeNumber: defaultOptions.defaultStorageTypeNumber,
        selectedHour: defaultOptions.defaultHour,
        selectedMaxCopiesNumber: defaultOptions.defaultMaxCopiesNumber,
      };
    } else {
      defaultOptions = {
        defaultSelectedFolder: "",
        defaultStorageTypeNumber: "0",
        defaultHour: "12:00",
        defaultPeriodNumber: "0",
        defaultDay: "0",
        defaultMaxCopiesNumber: "10",
      };

      selectedOptions = {
        selectedFolder: defaultOptions.defaultSelectedFolder,
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
      selectedOptions
    );
  };

  onSetDefaultPeriodOption = (
    checkedStorage,
    defaultOptions,
    selectedOptions
  ) => {
    const { defaultPeriodNumber, defaultDay } = defaultOptions;

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

          defaultWeeklySchedule: true,
          selectedWeeklySchedule: true,

          selectedMonthDay: "1",
          defaultMonthDay: "1",

          isLoading: false,
          ...defaultOptions,
          ...selectedOptions,
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

            defaultMonthlySchedule: true,
            selectedMonthlySchedule: true,

            isLoading: false,
            ...defaultOptions,
            ...selectedOptions,
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

            defaultDailySchedule: true,
            selectedDailySchedule: true,

            isLoading: false,
            ...defaultOptions,
            ...selectedOptions,
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
          if (res.error.length > 0 && res.progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${res.error}`);
            console.log("error", res.error);
            this.timerId = null;
            this.setState({
              isCopyingToLocal: false,
              downloadingProgress: 100,
            });
            return;
          }
          if (this._isMounted) {
            downloadingProgress !== res.progress &&
              this.setState({
                downloadingProgress: res.progress,
              });
          }
          if (res.progress === 100) {
            clearInterval(this.timerId);

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;
            if (this._isMounted) {
              this.setState({
                isCopyingToLocal: false,
              });
            }
          }
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(err);
        console.log("err", err);

        this.timerId = null;
        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
            isCopyingToLocal: false,
          });
        }
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
      isEnable ? -1 : `${DOCUMENT_MODULE_TYPE}`
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

  resetPeriodSettings = (key = -1) => {
    const titleArray = [
      [EVERY_DAY_TYPE, "Daily"],
      [EVERY_WEEK_TYPE, "Weekly"],
      [EVERY_MONTH_TYPE, "Monthly"],
    ];

    let resultObj = {};

    for (let i = 0; i < titleArray.length; i++) {
      console.log("key", key, "i", i);
      if (key === titleArray[i][0]) {
        resultObj[`selected${titleArray[i][1]}Schedule`] = true;
      } else {
        resultObj[`selected${titleArray[i][1]}Schedule`] = false;
        if (key === -1) resultObj[`default${titleArray[i][1]}Schedule`] = false;
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
      defaultSelectedFolder,

      selectedMaxCopiesNumber,
      selectedHour,
      selectedMonthDay,
      selectedWeekdayLabel,
      selectedMonthlySchedule,
      selectedDailySchedule,
      selectedWeeklySchedule,
      selectedFolder,
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
    if (selectedFolder !== defaultSelectedFolder) return true;

    return false;
  };

  resetModuleSettings = (key = -1) => {
    const arrayType = [
      [`${DOCUMENT_MODULE_TYPE}`, "Documents"],
      [`${RESOURCES_MODULE_TYPE}`, "ThirdParty"],
      [`${STORAGES_MODULE_TYPE}`, "ThirdPartyStorage"],
    ];
    let resultObj = {};

    for (let i = 0; i < 3; i++) {
      if (key === arrayType[i][0]) {
        resultObj[`isChecked${arrayType[i][1]}`] = true;
        resultObj["selectedStorageTypeNumber"] = `${key}`;
      } else {
        resultObj[`isChecked${arrayType[i][1]}`] = false;
      }
    }

    return resultObj;
  };
  onClickShowStorage = (e) => {
    const key = e.target.name;
    let options = this.resetModuleSettings(key);
    console.log("onClickShowStorage resetModuleSettings", options);
    this.setState(
      {
        ...options,
        selectedFolder: "",
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
    } = this.state;

    let storageObj = {};

    if (!this.selectedSchedule && isEnable) {
      this.setState({ isEnable: false });
    }
    if (this.selectedSchedule && !isEnable) {
      this.setState({ isEnable: true });
    }

    storageObj = this.resetModuleSettings(defaultStorageTypeNumber);
    console.log("onCancelModuleSettings storageObj", storageObj);
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
      isChanged: false,
      isReset: true,
      ...storageObj,
    });
  };
  onSaveModuleSettings = async (selectedId, inputValueArray) => {
    const {
      selectedMaxCopiesNumber,
      selectedMonthlySchedule,
      selectedWeeklySchedule,
      selectedWeekday,
      selectedHour,
      selectedMonthDay,

      isCheckedDocuments,
      isCheckedThirdParty,
      isEnable,
      selectedFolder,
    } = this.state;

    if (!isEnable) {
      this.deleteSchedule();
      return;
    }

    if (
      (isCheckedDocuments && !selectedFolder) ||
      (isCheckedThirdParty && !selectedFolder)
    ) {
      this.setState({
        isError: true,
      });
      return;
    }

    this.setState({ isLoadingData: true }, function () {
      let day, period;

      if (selectedWeeklySchedule) {
        period = `${EVERY_WEEK_TYPE}`;
        day = `${selectedWeekday}`;
      } else {
        if (selectedMonthlySchedule) {
          period = `${EVERY_MONTH_TYPE}`;
          day = `${selectedMonthDay}`;
        } else {
          period = `${EVERY_DAY_TYPE}`;
          day = null;
        }
      }

      let time = selectedHour.substring(0, selectedHour.indexOf(":"));

      const storageType = isCheckedDocuments
        ? `${DOCUMENT_MODULE_TYPE}`
        : isCheckedThirdParty
        ? `${RESOURCES_MODULE_TYPE}`
        : `${STORAGES_MODULE_TYPE}`;

      let storageParams = [];

      if (isCheckedDocuments || isCheckedThirdParty) {
        storageParams.push({
          key: "folderId",
          value: selectedFolder,
        });
      } else {
        storageParams.push({
          key: "module",
          value: selectedId,
        });

        let obj = {};

        for (let i = 0; i < inputValueArray.length; i++) {
          obj = {
            key: inputValueArray[i].key,
            value: inputValueArray[i].value,
          };
          storageParams.push(obj);
        }
      }

      this.setState(
        {
          isLoadingData: true,
        },
        function () {
          this.createSchedule(
            storageType,
            storageParams,
            selectedMaxCopiesNumber,
            period,
            time,
            day
          );
        }
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
      const selectedSchedule = await getBackupSchedule();
      if (selectedSchedule) {
        const { storageType } = selectedSchedule;
        this.selectedSchedule = true;
        const resetOptions = this.resetPeriodSettings();
        const resetModuleSettings = this.resetModuleSettings(storageType);
        console.log("createSchedule resetModuleSettings", resetModuleSettings);

        this.setState({
          ...resetOptions,
          ...resetModuleSettings,
        });

        this.onSetDefaultOptions(selectedSchedule);
      } else {
        this.selectedSchedule = false;
        this.onSetDefaultOptions();
      }
      toastr.success(t("SuccessfullySaveSettingsMessage"));
    } catch (e) {
      console.error(e);
    }

    this._isMounted &&
      this.setState({
        isLoadingData: false,
        isChanged: false,
        isError: false,
      });
  };

  deleteSchedule = () => {
    const { t } = this.props;
    this.setState({ isLoadingData: true }, function () {
      deleteBackupSchedule()
        .then(() => {
          this.selectedSchedule = false;
          toastr.success(t("SuccessfullySaveSettingsMessage"));
        })
        .then(() => {
          const resetOptions = this.resetPeriodSettings();
          console.log("createSchedule resetOptions", resetOptions);
          this.setState({
            ...resetOptions,
          });
          this.onSetDefaultOptions();
        })
        .catch((error) => toastr.error(error))
        .finally(() =>
          this.setState({ isLoadingData: false, isChanged: false })
        );
    });
  };
  onSelectFolder = (folderId) => {
    this.setState(
      {
        selectedFolder: folderId,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSetLoadingData = (loading) => {
    this.setState({
      isLoadingData: loading,
      isReset: false,
    });
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(proxyURL, "/settings/datamanagement/backup/manual-backup")
    );
  };
  render() {
    const { t } = this.props;
    const {
      isLoading,
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
      isCopyingToLocal,
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
      isLoadingData: isLoadingData,

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
      isDisabled: isCheckedThirdParty
        ? isLoadingData || isDisabledThirdPartyList
        : isLoadingData,
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledAutoBackup>
        <ToggleButton
          className="backup_toggle-btn"
          label={t("EnableAutomaticBackup")}
          onChange={this.onClickPermissions}
          isChecked={isEnable}
          isDisabled={isLoadingData}
        />
        <Text className="backup_toggle-btn-description">
          {t("DocumentsModuleDescription")}
        </Text>
        {isEnable && (
          <div className="backup_modules">
            <StyledModules>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("DocumentsModule")}
                name={"0"}
                key={0}
                isChecked={isCheckedDocuments}
              />
              <Text className="backup-description">
                {t("DocumentsModuleDescription")}
              </Text>
              {isCheckedDocuments && (
                <DocumentsModule
                  {...commonProps}
                  isReset={isReset}
                  isThirdPartyDefault={isThirdPartyDefault}
                  isError={isError}
                />
              )}
            </StyledModules>

            <StyledModules isDisabled={isDisabledThirdPartyList}>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("ThirdPartyResource")}
                name={"1"}
                isChecked={isCheckedThirdParty}
              />
              <Text className="backup-description">
                {t("ThirdPartyResourceDescription")}
              </Text>

              {isCheckedThirdParty && (
                <ThirdPartyModule
                  {...commonProps}
                  isReset={isReset}
                  isThirdPartyDefault={isThirdPartyDefault}
                  isError={isError}
                />
              )}
            </StyledModules>

            <StyledModules>
              <RadioButton
                {...commonRadioButtonProps}
                label={t("ThirdPartyStorage")}
                name={"2"}
                isChecked={isCheckedThirdPartyStorage}
              />
              <Text className="backup-description">
                {t("ThirdPartyStorageDescription")}
              </Text>

              {isCheckedThirdPartyStorage && (
                <ThirdPartyStorageModule
                  {...commonProps}
                  onCancelModuleSettings={this.onCancelModuleSettings}
                  checkChanges={this.checkChanges}
                  onSaveModuleSettings={this.onSaveModuleSettings}
                  isChanged={isChanged}
                  isCopyingToLocal={isCopyingToLocal}
                />
              )}
            </StyledModules>
          </div>
        )}

        {isChanged && !isCheckedThirdPartyStorage && (
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
