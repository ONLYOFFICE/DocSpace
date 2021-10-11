import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";

import RadioButtonGroup from "@appserver/components/radio-button-group";
import RadioButton from "@appserver/components/radio-button";
import moment from "moment";

import Button from "@appserver/components/button";
import {
  deleteBackupSchedule,
  getBackupProgress,
  getBackupSchedule,
  createBackupSchedule,
} from "@appserver/common/api/portal";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import toastr from "@appserver/components/toast/toastr";
import SelectFolderDialog from "files/SelectFolderDialog";

import Loader from "@appserver/components/loader";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import FloatingButton from "@appserver/common/components/FloatingButton";

import { StyledModules, StyledAutoBackup } from "./styled-backup";
import ThirdPartyModule from "./sub-components-automatic-backup/thirdPartyModule";
import DocumentsModule from "./sub-components-automatic-backup/documentsModule";
import ThirdPartyStorageModule from "./sub-components-automatic-backup/thirdPartyStorageModule";
import { getFromSessionStorage, saveToSessionStorage } from "../../utils";
import ToggleButton from "@appserver/components/toggle-button";

const { proxyURL } = AppServerConfig;

// let defaultStorageType = "";

// let defaultSelectedOption = "";

// let defaultMonthly = false;
// let defaultWeekly = false;
// let defaultDaily = false;

// let numberPeriodFromSessionStorage = null;
// let dayFromSessionStorage = "";
// let timeFromSessionStorage = "";
// let maxCopiesFromSessionStorage = "";
// const settingNames = ["day", "time", "maxCopies", "numberPeriod"];

const DOCUMENT_TYPE = 0;
const RESOURCES_TYPE = 1;
const STORAGE_TYPE = 5;

const EVERY_DAY = 0;
const EVERY_WEEK = 1;
const EVERY_MONTH = 2;
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
      isShowedStorageTypes: false,

      defaultMonthlySchedule: false,
      defaultDailySchedule: false,
      defaultWeeklySchedule: false,

      selectedMonthlySchedule: false,
      selectedDailySchedule: false,
      selectedWeeklySchedule: false,

      isEnable: false,
      //weekOptions: [],

      defaultSelectedFolder: "",
      defaultStorageTypeNumber: "",
      defaultHour: "",
      defaultPeriodNumber: "",
      defaultDay: "",
      defaultMaxCopiesNumber: "",
      defaultWeekdayLabel: "",
      defaultPeriodLabel: "",

      selectedPeriodLabel: "",
      selectedWeekdayLabel: "",
      selectedHour: "",
      selectedMonthDay: "",
      selectedMaxCopiesNumber: "",
      selectedFolderDocument: "",
      selectedFolderResources: "",
      selectedStorageTypeNumber: "",

      isSetDefaultFolderPath: false,

      isCopyingToLocal: true,
      isLoadingData: false,
      isLoading: false,
      isDisableOptions: false,
      folderPath: "",
      isChanged: false,
      isSetDefaultFolderPath: false,
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
    //debugger;

    let checkedStorage = {};

    const defaultOptions = {
      defaultSelectedFolder: selectedSchedule
        ? `${selectedSchedule.storageParams.folderId}`
        : "",
      defaultStorageTypeNumber: selectedSchedule
        ? `${selectedSchedule.storageType}`
        : "0",
      defaultHour: selectedSchedule
        ? `${selectedSchedule.cronParams.hour}:00`
        : "12:00",
      defaultPeriodNumber: selectedSchedule
        ? `${selectedSchedule.cronParams.period}`
        : "0",
      defaultDay: selectedSchedule ? `${selectedSchedule.cronParams.day}` : "0",
      defaultMaxCopiesNumber: selectedSchedule
        ? `${selectedSchedule.backupsStored}`
        : "10",

      isEnable: true,
    };

    const selectedOptions = {
      selectedFolder: selectedSchedule
        ? `${selectedSchedule.storageParams.folderId}`
        : "",
      selectedStorageTypeNumber: selectedSchedule
        ? `${selectedSchedule.storageType}`
        : "0",
      selectedHour: selectedSchedule
        ? `${selectedSchedule.cronParams.hour}:00`
        : "12:00",

      selectedMaxCopiesNumber: selectedSchedule
        ? `${selectedSchedule.backupsStored}`
        : "10",
    };
    if (+defaultOptions.defaultStorageTypeNumber === DOCUMENT_TYPE) {
      // Documents Module
      checkedStorage.isCheckedDocuments = true;
    } else {
      if (+defaultOptions.defaultStorageTypeNumber === RESOURCES_TYPE) {
        // ThirdPartyResource Module
        checkedStorage.isCheckedThirdParty = true;
      } else {
        if (+defaultOptions.defaultStorageTypeNumber === STORAGE_TYPE) {
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

  getProgress = () => {
    const { t } = this.props;

    getBackupProgress()
      .then((res) => {
        if (res) {
          if (res.error.length > 0 && res.progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${res.error}`);
            console.log("error", res.error);
            this.timerId = null;
            this.setState({
              isCopyingToLocal: true,
            });
            return;
          }
          if (this._isMounted) {
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
        clearInterval(timerId);
        timerId && toastr.error(err);
        console.log("err", err);

        timerId = null;
        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
          });
        }
      });
  };
  onSetDefaultPeriodOption = (
    checkedStorage,
    defaultOptions,
    selectedOptions
  ) => {
    if (+defaultOptions.defaultPeriodNumber === EVERY_WEEK) {
      //Every Week option
      //debugger;

      this._isMounted &&
        this.setState({
          ...checkedStorage,
          defaultPeriodLabel: this.periodsObject[EVERY_WEEK].label,
          selectedPeriodLabel: this.periodsObject[EVERY_WEEK].label,
          defaultWeekdayLabel: this.weekdaysLabelArray[
            defaultOptions.defaultDay ? defaultOptions.defaultDay - 1 : 0
          ].label,

          selectedWeekdayLabel: this.weekdaysLabelArray[
            defaultOptions.defaultDay ? defaultOptions.defaultDay - 1 : 0
          ].label,

          selectedWeekday: defaultOptions.defaultDay,
          defaultWeeklySchedule: true,
          selectMonthlySchedule: true,
          selectedMonthDay: "1",
          isLoading: false,
          ...defaultOptions,
          ...selectedOptions,
        });
    } else {
      if (+defaultOptions.defaultPeriodNumber === EVERY_MONTH) {
        //Every Month option

        this._isMounted &&
          this.setState({
            ...checkedStorage,
            defaultPeriodLabel: this.periodsObject[EVERY_MONTH].label,
            selectedPeriodLabel: this.periodsObject[EVERY_MONTH].label,
            defaultWeekdayLabel: this.weekdaysLabelArray[0].label,
            selectedWeekdayLabel: this.weekdaysLabelArray[0].label,
            selectedMonthDay: defaultOptions.defaultDay,
            selectedWeekday: "1",
            defaultMonthlySchedule: true,
            selectedMonthlySchedule: true,
            isLoading: false,
            ...defaultOptions,
            ...selectedOptions,
          });
      } else {
        this._isMounted &&
          this.setState({
            ...checkedStorage,
            defaultPeriodLabel: this.periodsObject[EVERY_DAY].label,
            selectedPeriodLabel: this.periodsObject[EVERY_DAY].label,
            defaultWeekdayLabel: this.weekdaysLabelArray[0].label,
            selectedWeekdayLabel: this.weekdaysLabelArray[0].label,
            defaultDailySchedule: true,
            selectedDailySchedule: true,
            selectedMonthDay: "1",
            selectedWeekday: "1",
            isLoading: false,
            ...defaultOptions,
            ...selectedOptions,
          });
      }
    }
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
    for (let item = 1; item <= 30; item++) {
      let obj = {
        key: `${item}`,
        label: `${item}`,
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

    if (!isEnable) {
      this.setState({
        isEnable: true,
        isCheckedDocuments: true,
        selectedStorageTypeNumber: `${DOCUMENT_TYPE}`,
        isChanged: true,
      });
    } else {
      this.setState({
        isEnable: false,
        isCheckedDocuments: false,
        isCheckedThirdParty: false,
        isCheckedThirdPartyStorage: false,
        isChanged: true,
      });
    }
  };

  onSelectMaxCopies = (options) => {
    const key = options.key;
    const label = options.label;

    //saveToSessionStorage("maxCopies", label);

    this.setState(
      {
        selectedMaxCopiesNumber: key,
        // selectedMaxCopies: label,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectWeekDay = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;
    //debugger;
    //saveToSessionStorage("day", key);

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
  onSelectTime = (options) => {
    const label = options.label;

    // saveToSessionStorage("time", label);
    this.setState({ selectedTimeOption: label }),
      function () {
        this.checkChanges();
      };
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

  onSelectPeriod = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;

    saveToSessionStorage("numberPeriod", key);

    this.setState({ selectedPeriodLabel: label });

    if (key === EVERY_DAY) {
      this.setState(
        {
          selectedWeeklySchedule: false,
          selectedMonthlySchedule: false,
          selectedDailySchedule: true,
        },
        function () {
          this.checkChanges();
        }
      );
    } else {
      if (key === EVERY_WEEK) {
        this.setState(
          {
            selectedWeeklySchedule: true,
            selectedMonthlySchedule: false,
            selectedDailySchedule: false,
          },
          function () {
            this.checkChanges();
          }
        );
      } else {
        this.setState(
          {
            selectedWeeklySchedule: false,
            selectedMonthlySchedule: true,
            selectedDailySchedule: false,
          },
          function () {
            this.checkChanges();
          }
        );
      }
    }
  };

  onSelectFolder = (folderId) => {
    this.setState({
      selectedFolder: folderId,
    });
  };

  checkChanges = () => {
    const {
      isChanged,
      defaultStorageTypeNumber,
      selectedStorageTypeNumber,
    } = this.state;

    if (defaultStorageTypeNumber !== selectedStorageTypeNumber) {
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

  setDocumentsModule = () => {
    return {
      isCheckedDocuments: true,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,
      selectedStorageTypeNumber: `${DOCUMENT_TYPE}`,
    };
  };

  setResourcesModule = () => {
    return {
      isCheckedDocuments: false,
      isCheckedThirdParty: true,
      isCheckedThirdPartyStorage: false,
      selectedStorageTypeNumber: `${RESOURCES_TYPE}`,
    };
  };

  setStorageModule = () => {
    return {
      isCheckedDocuments: false,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: true,
      selectedStorageTypeNumber: `${STORAGE_TYPE}`,
    };
  };
  onClickShowStorage = (e) => {
    const name = +e.target.name;

    //debugger;
    let options = {};
    if (name === DOCUMENT_TYPE) {
      options = this.setDocumentsModule();
      this.setState(
        {
          ...options,
          isError: false,
        },
        function () {
          this.checkChanges();
        }
      );
    } else {
      if (name === RESOURCES_TYPE) {
        options = this.setResourcesModule();
        this.setState(
          {
            ...options,
            selectedFolderResources: "",
            isError: false,
          },
          function () {
            this.checkChanges();
          }
        );
      } else {
        options = this.setStorageModule();
        this.setState(
          {
            ...options,
          },
          function () {
            this.checkChanges();
          }
        );
      }
    }
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
      defaultDay,
    } = this.state;

    let storageObj = {};

    if (+defaultStorageTypeNumber === DOCUMENT_TYPE) {
      storageObj = this.setDocumentsModule();
    } else {
      if (+defaultStorageTypeNumber === RESOURCES_TYPE) {
        storageObj = this.setResourcesModule();
      } else {
        storageObj = this.setStorageModule();
      }
    }
    console.log(
      "defaultMonthlySchedule",
      defaultMonthlySchedule,
      "defaultWeeklySchedule",
      defaultWeeklySchedule,
      "defaultDailySchedule",
      defaultDailySchedule
    );

    let dayOption = {};
    if (+defaultStorageTypeNumber === STORAGE_TYPE) {
      dayOption = {
        selectedWeekday: defaultDay,
      };
    } else if (+defaultStorageTypeNumber === RESOURCES_TYPE) {
      dayOption = {
        selectedMonthDay: defaultDay,
      };
    }

    this.setState({
      selectedMonthlySchedule: defaultMonthlySchedule,
      selectedWeeklySchedule: defaultWeeklySchedule,
      selectedDailySchedule: defaultDailySchedule,
      selectedHour: defaultHour,
      selectedPeriodLabel: defaultPeriodLabel,
      selectedMonthDay: defaultDay,
      selectedWeekdayLabel: defaultWeekdayLabel,
      selectedMaxCopiesNumber: defaultMaxCopiesNumber,
      selectedStorageTypeNumber: defaultStorageTypeNumber,
      ...(isError && { isError: false }),
      isChanged: false,
      isReset: true,
      ...storageObj,
      ...dayOption,
    });
  };
  onSaveModuleSettings = async () => {
    const {
      selectedFolderDocument,
      selectedFolderResources,
      selectedMaxCopiesNumber,
      selectedMonthlySchedule,
      selectedWeeklySchedule,
      selectedWeekday,
      selectedHour,
      selectedMonthDay,

      isCheckedDocuments,
      isCheckedThirdParty,
    } = this.state;

    const { t } = this.props;
    debugger;
    if (
      (isCheckedDocuments && !selectedFolderDocument) ||
      (isCheckedThirdParty && !selectedFolderResources)
    ) {
      this.setState({
        isError: true,
      });
      return;
    }

    this.setState({ isLoadingData: true }, function () {
      let day, period, storageType;

      if (selectedWeeklySchedule) {
        period = `${EVERY_WEEK}`;
        day = `${selectedWeekday}`;
      } else {
        if (selectedMonthlySchedule) {
          period = `${EVERY_MONTH}`;
          day = `${selectedMonthDay}`;
        } else {
          period = `${EVERY_DAY}`;
          day = null;
        }
      }

      let time = selectedHour.substring(0, selectedHour.indexOf(":"));

      let selectedFolder;

      if (isCheckedDocuments) {
        storageType = `${DOCUMENT_TYPE}`;
        selectedFolder = selectedFolderDocument;
      } else {
        if (isCheckedThirdParty) {
          storageType = `${RESOURCES_TYPE}`;
          selectedFolder = selectedFolderResources;
        } else {
          storageType = `${STORAGE_TYPE}`;
        }
      }
      const storageParams = [
        {
          key: "folderId",
          value: selectedFolder,
        },
      ];

      let folderId;

      createBackupSchedule(
        storageType,
        storageParams,
        selectedMaxCopiesNumber,
        period,
        time,
        day
      )
        .then(() => getBackupSchedule())
        .then((selectedSchedule) => {
          if (selectedSchedule) {
            folderId = selectedSchedule.storageParams.folderId;
            this.onSetDefaultOptions(selectedSchedule);
            toastr.success(t("SuccessfullySaveSettingsMessage"));
          } else this.onSetDefaultOptions();
          if (selectedSchedule.storageType === DOCUMENT_TYPE) {
            this.setState({
              defaultMonthlySchedule: false,
              defaultWeeklySchedule: false,
            });
          } else if (selectedSchedule.storageType === RESOURCES_TYPE) {
            this.setState({
              defaultMonthlySchedule: false,
              defaultDailySchedule: false,
            });
          } else
            this.setState({
              defaultDailySchedule: false,
              defaultWeeklySchedule: false,
            });
        })
        .catch((e) => console.error(e))
        .finally(
          () =>
            this._isMounted &&
            this.setState({
              isLoadingData: false,
              isChanged: false,
              isError: false,
            })
        );
    });
  };

  onSelectFolder = (folderId) => {
    const { isCheckedThirdParty } = this.state;
    let folderType = {};
    if (isCheckedThirdParty) {
      folderType = {
        selectedFolderResources: folderId,
      };
    } else {
      folderType = {
        selectedFolderDocument: folderId,
      };
    }

    this.setState(
      {
        ...folderType,
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
      defaultSelectedFolder,

      selectedMonthlySchedule,
      selectedWeeklySchedule,
      selectedHour,
      selectedPeriodLabel,
      selectedMonthDay,
      selectedWeekdayLabel,
      selectedMaxCopiesNumber,
      isReset,

      isCheckedDocuments,
      isCheckedThirdParty,
      isLoadingData,
      defaultStorageTypeNumber,
      isCheckedThirdPartyStorage,
      isError,
      isCopyingToLocal,
    } = this.state;

    const resourcesModule = +defaultStorageTypeNumber === RESOURCES_TYPE;
    console.log("auto backup render");
    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledAutoBackup>
        <ToggleButton
          className="backup_toggle-btn"
          label={t("EnableAutomaticBackup")}
          onChange={this.onClickPermissions}
          isChecked={isEnable}
        />

        {isEnable && (
          <>
            <StyledModules>
              <RadioButton
                fontSize="13px"
                fontWeight="400"
                label={t("DocumentsModule")}
                name={"0"}
                key={0}
                onClick={this.onClickShowStorage}
                isChecked={isCheckedDocuments}
                isDisabled={isLoadingData}
                value="value"
                className="backup_radio-button"
              />
              <Text className="backup-description">
                {t("DocumentsModuleDescription")}
              </Text>
              {isCheckedDocuments && (
                <DocumentsModule
                  selectedPeriodLabel={selectedPeriodLabel}
                  selectedWeekdayLabel={selectedWeekdayLabel}
                  selectedMonthDay={selectedMonthDay}
                  selectedHour={selectedHour}
                  selectedMaxCopies={selectedMaxCopiesNumber}
                  monthNumbersArray={this.monthNumbersArray}
                  hoursArray={this.hoursArray}
                  maxNumberCopiesArray={this.maxNumberCopiesArray}
                  periodsObject={this.periodsObject}
                  weekdaysLabelArray={this.weekdaysLabelArray}
                  onSelectPeriod={this.onSelectPeriod}
                  onSelectWeekDay={this.onSelectWeekDay}
                  onSelectMonthNumber={this.onSelectMonthNumber}
                  onSelectTime={this.onSelectTime}
                  onSelectMaxCopies={this.onSelectMaxCopies}
                  onSelectFolder={this.onSelectFolder}
                  onSetLoadingData={this.onSetLoadingData}
                  weeklySchedule={selectedWeeklySchedule}
                  monthlySchedule={selectedMonthlySchedule}
                  defaultSelectedFolder={defaultSelectedFolder}
                  isReset={isReset}
                  resourcesModule={resourcesModule}
                  isLoadingData={isLoadingData}
                  isError={isError}
                />
              )}
            </StyledModules>

            <StyledModules
              isDisabled={
                this.commonThirdPartyList &&
                this.commonThirdPartyList.length === 0
              }
            >
              <RadioButton
                fontSize="13px"
                fontWeight="400"
                label={t("ThirdPartyResource")}
                name={"1"}
                onClick={this.onClickShowStorage}
                isChecked={isCheckedThirdParty}
                isDisabled={
                  isLoadingData ||
                  (this.commonThirdPartyList &&
                    this.commonThirdPartyList.length === 0)
                }
                value="value"
                className="backup_radio-button"
              />
              <Text className="backup-description">
                {t("ThirdPartyResourceDescription")}
              </Text>

              {isCheckedThirdParty && (
                <ThirdPartyModule
                  selectedPeriodLabel={selectedPeriodLabel}
                  selectedWeekdayLabel={selectedWeekdayLabel}
                  selectedMonthDay={selectedMonthDay}
                  selectedHour={selectedHour}
                  selectedMaxCopies={selectedMaxCopiesNumber}
                  monthNumbersArray={this.monthNumbersArray}
                  hoursArray={this.hoursArray}
                  maxNumberCopiesArray={this.maxNumberCopiesArray}
                  periodsObject={this.periodsObject}
                  weekdaysLabelArray={this.weekdaysLabelArray}
                  onSelectPeriod={this.onSelectPeriod}
                  onSelectWeekDay={this.onSelectWeekDay}
                  onSelectMonthNumber={this.onSelectMonthNumber}
                  onSelectTime={this.onSelectTime}
                  onSelectMaxCopies={this.onSelectMaxCopies}
                  onSelectFolder={this.onSelectFolder}
                  onSetLoadingData={this.onSetLoadingData}
                  weeklySchedule={selectedWeeklySchedule}
                  monthlySchedule={selectedMonthlySchedule}
                  defaultSelectedFolder={defaultSelectedFolder}
                  isReset={isReset}
                  resourcesModule={resourcesModule}
                  isLoadingData={isLoadingData}
                  isError={isError}
                />
              )}
            </StyledModules>

            <StyledModules>
              <RadioButton
                fontSize="13px"
                fontWeight="400"
                label={t("ThirdPartyStorage")}
                name={"2"}
                onClick={this.onClickShowStorage}
                isChecked={isCheckedThirdPartyStorage}
                isDisabled={isLoadingData}
                value="value"
                className="backup_radio-button"
              />
              <Text className="backup-description">
                {t("ThirdPartyStorageDescription")}
              </Text>

              {isCheckedThirdPartyStorage && (
                <ThirdPartyStorageModule
                  isLoadingData={isLoadingData}
                  selectedPeriodLabel={selectedPeriodLabel}
                  selectedWeekdayLabel={selectedWeekdayLabel}
                  selectedMonthDay={selectedMonthDay}
                  selectedHour={selectedHour}
                  selectedMaxCopies={selectedMaxCopiesNumber}
                  monthNumbersArray={this.monthNumbersArray}
                  hoursArray={this.hoursArray}
                  maxNumberCopiesArray={this.maxNumberCopiesArray}
                  periodsObject={this.periodsObject}
                  weekdaysLabelArray={this.weekdaysLabelArray}
                  onSelectPeriod={this.onSelectPeriod}
                  onSelectWeekDay={this.onSelectWeekDay}
                  onSelectMonthNumber={this.onSelectMonthNumber}
                  onSelectTime={this.onSelectTime}
                  onSelectMaxCopies={this.onSelectMaxCopies}
                  onSelectFolder={this.onSelectFolder}
                  onSetLoadingData={this.onSetLoadingData}
                  weeklySchedule={selectedWeeklySchedule}
                  monthlySchedule={selectedMonthlySchedule}
                />
              )}
            </StyledModules>
          </>
        )}

        {isChanged && (
          <>
            <Button
              label={t("Common:Save")}
              onClick={this.onSaveModuleSettings}
              primary
              isDisabled={isCopyingToLocal}
              size="medium"
              tabIndex={10}
            />

            <Button
              label={t("Common: Cancel")}
              onClick={this.onCancelModuleSettings}
              primary
              isDisabled={isCopyingToLocal}
              size="medium"
              tabIndex={10}
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
