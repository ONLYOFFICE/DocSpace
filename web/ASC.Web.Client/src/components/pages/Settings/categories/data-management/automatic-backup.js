import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import RadioButton from "@appserver/components/radio-button";
import styled from "styled-components";
import moment from "moment";
import ScheduleComponent from "./sub-components-automatic-backup/scheduleComponent";
import {
  createBackupSchedule,
  deleteBackupSchedule,
  getBackupProgress,
  getBackupSchedule,
} from "@appserver/common/api/portal";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import toastr from "@appserver/components/toast/toastr";
import OperationsDialog from "files/OperationsDialog";
import ThirdPartyStorageModule from "./sub-components-automatic-backup/thirdPartyStorageModule";
import Loader from "@appserver/components/loader";
import { getFolderPath } from "@appserver/common/api/files";

const StyledComponent = styled.div`
  ${commonSettingsStyles}
  .manual-backup_buttons {
    margin-top: 16px;
  }
  .backup-include_mail,
  .backup_combobox {
    margin-top: 16px;
    margin-bottom: 16px;
  }
  .inherit-title-link {
    margin-bottom: 8px;
  }
  .note_description {
    margin-top: 8px;
  }
  .radio-button_text {
    font-size: 19px;
  }
  .automatic-backup_main {
    margin-bottom: 30px;
    .radio-button_text {
      font-size: 13px;
    }
  }
  .radio-button_text {
    margin-right: 7px;
    font-size: 19px;
    font-weight: 600;
  }
  .automatic-backup_radio-button {
    margin-bottom: 8px;
  }
  .backup_combobox {
    display: inline-block;
    margin-right: 8px;
  }
`;
const StyledModules = styled.div`
  margin-bottom: 40px;
`;

let folderDocumentsModulePath = "";
let folderThirdPartyModulePath = "";

let defaultStorageType = "";
let defaultHour = "";
let defaultPeriod = "";
let defaultDay = 1;
let defaultMaxCopies = "10";

let defaultSelectedOption = "";
let defaultSelectedWeekdayOption = "";
let defaultMonthly = false;
let defaultWeekly = false;
let defaultDaily = false;

let moduleName = "";
let storageFiledValue = "";
class AutomaticBackup extends React.Component {
  constructor(props) {
    super(props);
    const { t, language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      isShowedStorageType: false, //if current automatic storage not choose

      isShowDocuments: false,
      isShowThirdParty: false,
      isShowThirdPartyStorage: false,

      isCheckedDocuments: false,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,

      monthlySchedule: false,
      dailySchedule: false,
      weeklySchedule: false,

      selectedOption: t("DailyPeriodSchedule"),
      selectedWeekdayOption: "",
      selectedNumberWeekdayOption: "2",
      selectedTimeOption: "12:00",
      selectedMonthOption: "1",
      selectedMaxCopies: "10",
      selectedNumberMaxCopies: "10",
      selectedPermission: "disable",
      weekOptions: [],

      isCopyingToLocal: true,
      isLoadingData: false,
      selectedFolder: "",
      isPanelVisible: false,
      isLoading: false,
      isChanged: false,
      isSetDefaultFolderPath: false,
      isError: false,
    };

    this.periodOptions = [
      {
        key: 1,
        label: t("DailyPeriodSchedule"),
      },
      {
        key: 2,
        label: t("WeeklyPeriodSchedule"),
      },
      {
        key: 3,
        label: t("MonthlyPeriodSchedule"),
      },
    ];

    this.timeOptionsArray = [];
    this.getTimeOptions();
    this.monthNumberOptionsArray = [];
    this.getMonthNumbersOption();

    this.maxNumberCopiesArray = [];
    this.getMaxNumberCopies();

    this.weekdaysOptions = [];
    this.arrayWeekdays = moment.weekdays();
  }

  componentDidMount() {
    const { getCommonThirdPartyList } = this.props;

    this.getWeekdaysOptions();

    getBackupProgress().then((res) => {
      if (res) {
        if (res.progress === 100)
          this.setState({
            isCopyingToLocal: false,
          });
        if (res.progress !== 100)
          this.timerId = setInterval(() => this.getProgress(), 1000);
      } else {
        this.setState({
          isCopyingToLocal: false,
        });
      }
    });

    this.setState({ isLoading: true }, function () {
      getCommonThirdPartyList()
        .then(() => getBackupSchedule())

        .then((selectedSchedule) => {
          if (selectedSchedule) {
            const folderId = selectedSchedule.storageParams.folderId;
            defaultStorageType = `${selectedSchedule.storageType}`;
            defaultHour = `${selectedSchedule.cronParams.hour}:00`;
            defaultPeriod = `${selectedSchedule.cronParams.period}`;
            defaultDay = selectedSchedule.cronParams.day;
            defaultMaxCopies = `${selectedSchedule.backupsStored}`;

            if (defaultStorageType === "5") {
              this.setState({
                selectedPermission: "enable",
                isShowedStorageTypes: true,
              });

              this.onSetDefaultOptions();
            } else {
              getFolderPath(folderId)
                .then((folderPath) =>
                  defaultStorageType === "0"
                    ? (folderDocumentsModulePath = folderPath)
                    : (folderThirdPartyModulePath = folderPath)
                )
                .then(() => {
                  this.setState({
                    selectedPermission: "enable",
                    isShowedStorageTypes: true,
                  });

                  this.onSetDefaultOptions();
                });
            }
          }
        })
        .finally(() =>
          this.setState({
            isLoading: false,
          })
        );
    });
  }
  componentDidUpdate(prevState) {
    const { isChanged, isSetDefaultFolderPath } = this.state;

    if (isChanged !== prevState.isChanged && isSetDefaultFolderPath) {
      this.setState({
        isSetDefaultFolderPath: false,
      });
    }
  }
  componentWillUnmount() {
    clearInterval(this.timerId);
  }

  onSetDefaultOptions = () => {
    if (defaultStorageType === "0") {
      // Documents Module
      this.setState({
        isShowDocuments: true,
        isCheckedDocuments: true,
        selectedTimeOption: defaultHour,
        selectedMaxCopies: defaultMaxCopies,
        selectedNumberMaxCopies: defaultMaxCopies,
      });
    }
    if (defaultStorageType === "1") {
      // ThirdPartyResource Module
      this.setState({
        isShowThirdParty: true,
        isCheckedThirdParty: true,

        selectedTimeOption: defaultHour,
        selectedMaxCopies: defaultMaxCopies,
        selectedNumberMaxCopies: defaultMaxCopies,
      });
    }

    if (defaultStorageType === "5") {
      // ThirdPartyStorage Module
      this.setState({
        isShowThirdPartyStorage: true,
        isCheckedThirdPartyStorage: true,
        selectedTimeOption: defaultHour,
        selectedMaxCopies: defaultMaxCopies,
        selectedNumberMaxCopies: defaultMaxCopies,
      });
    }

    if (+defaultPeriod === 1) {
      //Every Week option
      const arrayIndex = this.lng === "en" ? defaultDay - 1 : defaultDay - 2; //selected number of week
      defaultSelectedOption = this.periodOptions[1].label;
      defaultSelectedWeekdayOption = defaultDay;
      defaultWeekly = true;
      this.setState({
        selectedOption: defaultSelectedOption,
        weeklySchedule: true,
        selectedWeekdayOption: this.weekdaysOptions[arrayIndex].label,
        selectedNumberWeekdayOption: defaultDay,
      });
    } else {
      if (+defaultPeriod === 2) {
        //Every Month option
        defaultSelectedOption = this.periodOptions[2].label;
        defaultMonthly = true;
        this.setState({
          selectedOption: defaultSelectedOption,
          monthlySchedule: true,
          selectedMonthOption: `${defaultDay}`, //selected day of month
        });
      } else {
        defaultDaily = true;
        defaultSelectedOption = this.periodOptions[0].label;
        this.setState({
          selectedOption: defaultSelectedOption,
        });
      }
    }
  };
  getProgress = () => {
    getBackupProgress().then((res) => {
      if (res) {
        if (res.error.length > 0 && res.progress !== 100) {
          clearInterval(this.timerId);
          console.log("error", res.error);
          this.setState({
            isCopyingToLocal: true,
          });
          return;
        }
        if (res.progress === 100) {
          clearInterval(this.timerId);
          this.setState({
            isCopyingToLocal: false,
          });
        }
      }
    });
  };

  getTimeOptions = () => {
    for (let item = 0; item < 24; item++) {
      let obj = {
        key: item,
        label: `${item}:00`,
      };
      this.timeOptionsArray.push(obj);
    }
  };

  getMonthNumbersOption = () => {
    for (let item = 1; item <= 31; item++) {
      let obj = {
        key: item + 24,
        label: `${item}`,
      };
      this.monthNumberOptionsArray.push(obj);
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
  getWeekdaysOptions = () => {
    for (let item = 0; item < this.arrayWeekdays.length; item++) {
      let obj = {
        key: `${item + 1}`,
        label: `${this.arrayWeekdays[item]}`,
      };
      this.weekdaysOptions.push(obj);
    }
    const isEnglishLanguage = this.lng === "en";

    if (!isEnglishLanguage) {
      const startWeek = this.weekdaysOptions[0];
      this.weekdaysOptions.shift();
      this.weekdaysOptions.push(startWeek);
    }

    this.setState({
      weekOptions: this.weekdaysOptions,
      selectedWeekdayOption: this.weekdaysOptions[0].label,
    });
  };

  onSelectPeriod = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;
    //debugger;
    this.setState({ selectedOption: label });
    key === 1
      ? this.setState(
          {
            weeklySchedule: false,
            monthlySchedule: false,
            dailySchedule: true,
          },
          function () {
            this.checkChanges();
          }
        )
      : key === 2
      ? this.setState(
          {
            weeklySchedule: true,
            monthlySchedule: false,
            dailySchedule: false,
          },
          function () {
            this.checkChanges();
          }
        )
      : this.setState(
          {
            monthlySchedule: true,
            weeklySchedule: false,
            dailySchedule: false,
          },
          function () {
            this.checkChanges();
          }
        );
  };

  onSelectWeedDay = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;
    //debugger;
    this.setState(
      {
        selectedNumberWeekdayOption: key,
        selectedWeekdayOption: label,
      },
      function () {
        this.checkChanges();
      }
    );
  };
  onSelectMonthNumberAndTimeOptions = (options) => {
    const key = options.key;
    const label = options.label;

    if (key <= 24) {
      this.setState({ selectedTimeOption: label }, function () {
        this.checkChanges();
      });
    } else {
      this.setState(
        {
          selectedMonthOption: label,
        },
        function () {
          this.checkChanges();
        }
      );
    }
  };

  onSelectMaxCopies = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState(
      {
        selectedNumberMaxCopies: key,
        selectedMaxCopies: label,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  onSelectFolder = (folderId) => {
    console.log("folderId", folderId);
    this.setState({
      selectedFolder: folderId,
      isChanged: true,
    });
  };

  onSaveModuleSettings = () => {
    const {
      selectedFolder,
      weeklySchedule,
      selectedTimeOption,
      monthlySchedule,
      selectedMonthOption,
      selectedNumberWeekdayOption,
      selectedNumberMaxCopies,
      isShowDocuments,
      isShowThirdParty,
      isShowThirdPartyStorage,
      isError,
    } = this.state;
    const { t } = this.props;
    let storageParams = [];

    if (!selectedFolder && !isShowThirdPartyStorage) {
      this.setState({
        isError: true,
      });
      return;
    }
    this.setState({ isLoadingData: true }, function () {
      let period = weeklySchedule ? "1" : monthlySchedule ? "2" : "0";

      let day = weeklySchedule
        ? `${selectedNumberWeekdayOption}`
        : monthlySchedule
        ? `${selectedMonthOption}`
        : null;

      let time = selectedTimeOption.substring(
        0,
        selectedTimeOption.indexOf(":")
      );
      let storageType = isShowDocuments ? "0" : isShowThirdParty ? "1" : "5";

      //console.log("storageFiledValue", this.storageFiledValue);
      if (!isShowThirdPartyStorage) {
        storageParams = [
          {
            key: "folderId",
            value: selectedFolder[0],
          },
        ];
      } else {
        storageParams = [
          {
            key: "module",
            value: this.moduleName,
          },
        ];
        let obj = {};

        for (let i = 0; i < this.storageFiledValue.length; i++) {
          obj = {
            key: this.storageFiledValue[i].key,
            value: this.storageFiledValue[i].value,
          };
          storageParams.push(obj);
        }
      }
      //debugger;

      createBackupSchedule(
        storageType,
        storageParams,
        selectedNumberMaxCopies,
        period,
        time,
        day
      )
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .then(() => getBackupSchedule())

        .then((selectedSchedule) => {
          if (selectedSchedule) {
            const folderId = selectedSchedule.storageParams.folderId;
            defaultStorageType = `${selectedSchedule.storageType}`;
            defaultHour = `${selectedSchedule.cronParams.hour}:00`;
            defaultPeriod = `${selectedSchedule.cronParams.period}`;
            defaultDay = selectedSchedule.cronParams.day;
            defaultMaxCopies = `${selectedSchedule.backupsStored}`;

            !isShowThirdPartyStorage && this.onSelectFolder([`${folderId}`]);
            if (!isShowThirdPartyStorage) {
              getFolderPath(folderId)
                .then((folderPath) => {
                  if (+defaultStorageType === 0) {
                    folderDocumentsModulePath = folderPath;
                    folderThirdPartyModulePath = "";
                  }
                  if (+defaultStorageType === 1) {
                    folderThirdPartyModulePath = folderPath;
                    folderDocumentsModulePath = "";
                  }
                })
                .then(() => {
                  this.setState({
                    selectedPermission: "enable",
                    isShowedStorageTypes: true,
                  });

                  this.onSetDefaultOptions();
                });
            } else {
              this.setState({
                selectedPermission: "enable",
                isShowedStorageTypes: true,
              });

              this.onSetDefaultOptions();
            }
          }
        })
        .catch((error) => console.log("error", error))
        .finally(() => {
          if (isError) this.setState({ isError: false });
          this.setState({
            isLoadingData: false,
            isChanged: false,
            selectedFolder: "",
          });
        });
    });
  };

  onCancelModuleSettings = () => {
    const {
      isShowDocuments,
      isShowThirdParty,
      selectedPermission,
      isCheckedDocuments,
      monthlySchedule,
      weeklySchedule,
      isError,
    } = this.state;

    this.setState({
      isChanged: false,
      isSetDefaultFolderPath: true,
      selectedFolder: "",
    });

    if (isError) this.setState({ isError: false });
    if (defaultStorageType) {
      selectedPermission === "disable" &&
        this.setState({
          selectedPermission: "enable",
          isShowedStorageTypes: true,
        });

      this.setState({
        selectedOption: defaultSelectedOption,
      });

      if (
        monthlySchedule === defaultWeekly ||
        monthlySchedule === defaultDaily
      ) {
        this.setState({
          monthlySchedule: false,
        });
      }

      if (
        weeklySchedule === defaultMonthly ||
        weeklySchedule === defaultDaily
      ) {
        this.setState({
          weeklySchedule: false,
        });
      }

      this.onSetDefaultOptions();

      if (+defaultStorageType === 0) {
        // Documents Module
        isShowThirdParty
          ? this.setState({
              isShowThirdParty: false,
              isCheckedThirdParty: false,
            })
          : this.setState({
              isShowThirdPartyStorage: false,
              isCheckedThirdPartyStorage: false,
            });
      }

      if (+defaultStorageType === 1) {
        // ThirdPartyResource Module
        isShowDocuments || isCheckedDocuments
          ? this.setState({
              isShowDocuments: false,
              isCheckedDocuments: false,
            })
          : this.setState({
              isShowThirdPartyStorage: false,
              isCheckedThirdPartyStorage: false,
            });
      }

      if (+defaultStorageType === 5) {
        // ThirdPartyStorage Module
        isShowDocuments
          ? this.setState({
              isShowDocuments: false,
              isCheckedDocuments: false,
            })
          : this.setState({
              isShowThirdParty: false,
              isCheckedThirdParty: false,
            });
      }
    } else {
      this.setState({
        selectedPermission: "disable",
        isShowedStorageTypes: false,

        isCheckedDocuments: false,
        isCheckedThirdParty: false,
        isCheckedThirdPartyStorage: false,

        isShowDocuments: false,
        isShowThirdParty: false,
        isShowThirdPartyStorage: false,
      });
    }
  };
  onClickShowStorage = (e) => {
    const name = e.target.name;

    //debugger;
    +name === 0
      ? this.setState(
          {
            isShowDocuments: true,
            isCheckedDocuments: true,
            isShowThirdParty: false,
            isCheckedThirdParty: false,
            isShowThirdPartyStorage: false,
            isCheckedThirdPartyStorage: false,
          },
          function () {
            this.checkChanges();
          }
        )
      : +name === 1
      ? this.setState(
          {
            isShowDocuments: false,
            isCheckedDocuments: false,
            isShowThirdParty: true,
            isCheckedThirdParty: true,
            isShowThirdPartyStorage: false,
            isCheckedThirdPartyStorage: false,
          },
          function () {
            this.checkChanges();
          }
        )
      : this.setState(
          {
            isShowDocuments: false,
            isCheckedDocuments: false,
            isShowThirdParty: false,
            isCheckedThirdParty: false,
            isShowThirdPartyStorage: true,
            isCheckedThirdPartyStorage: true,
          },
          function () {
            this.checkChanges();
          }
        );
  };

  checkChanges = () => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    let changed;
    //debugger;
    if (isCheckedDocuments) {
      //debugger;
      if (+defaultStorageType === 0) {
        changed = this.checkOptions();
        this.setState({
          isChanged: changed,
        });
        return;
      } else {
        this.setState({
          isChanged: true,
        });
        return;
      }
    }
    //debugger;
    if (isCheckedThirdParty) {
      if (+defaultStorageType === 1) {
        changed = this.checkOptions();
        this.setState({
          isChanged: changed,
        });
        return;
      } else {
        this.setState({
          isChanged: true,
        });
        return;
      }
    }
    if (isCheckedThirdPartyStorage) {
      if (+defaultStorageType === 5) {
        changed = this.checkOptions();
        this.setState({
          isChanged: changed,
        });
        return;
      } else {
        this.setState({
          isChanged: true,
        });
        return;
      }
    }
  };

  checkOptions = () => {
    const {
      selectedTimeOption,
      monthlySchedule,
      weeklySchedule,
      selectedMonthOption,
      selectedNumberMaxCopies,
      selectedNumberWeekdayOption,
      dailySchedule,
    } = this.state;

    //debugger;
    if (selectedTimeOption !== defaultHour) {
      return true;
    }
    if (+selectedNumberMaxCopies !== +defaultMaxCopies) {
      return true;
    }
    if (+defaultPeriod === 0 && (monthlySchedule || weeklySchedule)) {
      return true;
    }

    if (+defaultPeriod === 1 && (monthlySchedule || dailySchedule)) {
      return true;
    }
    if (+defaultPeriod === 2 && (weeklySchedule || dailySchedule)) {
      return true;
    }
    if (monthlySchedule) {
      if (+selectedMonthOption !== defaultDay) {
        return true;
      }
    }
    //debugger;
    if (weeklySchedule) {
      if (+selectedNumberWeekdayOption !== defaultSelectedWeekdayOption) {
        return true;
      }
    }

    return false;
  };
  onClickPermissions = (e) => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    const name = e.target.defaultValue;
    if (name === "enable") {
      this.setState({
        isShowedStorageTypes: true,

        selectedPermission: "enable",
      });

      this.checkChanges();

      if (isCheckedDocuments) {
        this.setState({
          isShowDocuments: true,
        });
      } else {
        if (isCheckedThirdParty) {
          this.setState({
            isShowThirdParty: true,
          });
        } else {
          if (isCheckedThirdPartyStorage) {
            this.setState({
              isShowThirdPartyStorage: true,
            });
          } else {
            this.setState({
              isCheckedDocuments: true,
              isShowDocuments: true,
              isChanged: true,
            });
          }
        }
      }
    } else {
      this.setState({
        isShowedStorageTypes: false,
        //isCheckedDocuments: false,
        //isCheckedThirdParty: false,
        //isCheckedThirdPartyStorage: false,

        isShowDocuments: false,
        isShowThirdParty: false,
        isShowThirdPartyStorage: false,
        isChanged: true,
        selectedPermission: "disable",
      });
    }
  };
  onClickDeleteSchedule = () => {
    const { t } = this.props;
    this.setState({ isLoadingData: true }, function () {
      deleteBackupSchedule()
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .then(() => getBackupSchedule())
        .then((selectedSchedule) => {
          if (selectedSchedule) {
            defaultStorageType = `${selectedSchedule.storageType}`;
          }
        })
        .catch((error) => toastr.error(error))
        .finally(() =>
          this.setState({ isLoadingData: false, isChanged: false })
        );
    });
  };

  onClickInput = (e) => {
    this.setState({
      isPanelVisible: true,
    });
  };

  onClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };

  fillStorageFields = (moduleName = "", storageFiledValue = "") => {
    //debugger;
    this.moduleName = moduleName;
    this.storageFiledValue = storageFiledValue;
    this.onSaveModuleSettings();
  };
  render() {
    const { t, commonThirdPartyList } = this.props;
    const {
      isShowedStorageTypes,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isShowDocuments,
      isShowThirdParty,
      isShowThirdPartyStorage,
      weeklySchedule,
      monthlySchedule,
      weekOptions,
      selectedOption,
      selectedWeekdayOption,
      selectedTimeOption,
      selectedMonthOption,
      selectedMaxCopies,
      isCopyingToLocal,
      isLoadingData,
      isPanelVisible,
      selectedPermission,
      isLoading,
      isChanged,
      isSetDefaultFolderPath,
      isError,
    } = this.state;

    console.log("folderThirdPartyModulePath", folderThirdPartyModulePath);

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledComponent>
        <RadioButtonGroup
          className="automatic-backup_main "
          name={"0"}
          options={[
            {
              label: t("DisableAutomaticBackup"),
              value: "disable",
            },
            {
              label: t("EnableAutomaticBackup"),
              value: "enable",
            },
          ]}
          isDisabled={isLoadingData}
          onClick={this.onClickPermissions}
          orientation="vertical"
          selected={selectedPermission}
        />

        {isShowedStorageTypes && (
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
                value="value"
                className="automatic-backup_radio-button"
              />
              <Text className="category-item-description">
                {t("DocumentsModuleDescription")}
              </Text>
              {isShowDocuments && (
                <>
                  <OperationsDialog
                    onSelectFolder={this.onSelectFolder}
                    name={"common"}
                    onClose={this.onClose}
                    onClickInput={this.onClickInput}
                    isPanelVisible={isPanelVisible}
                    isCommonWithoutProvider
                    folderPath={folderDocumentsModulePath}
                    isSetDefaultFolderPath={isSetDefaultFolderPath}
                  />

                  <ScheduleComponent
                    weeklySchedule={weeklySchedule}
                    monthlySchedule={monthlySchedule}
                    weekOptions={weekOptions}
                    selectedOption={selectedOption}
                    selectedWeekdayOption={selectedWeekdayOption}
                    selectedTimeOption={selectedTimeOption}
                    selectedMonthOption={selectedMonthOption}
                    selectedMaxCopies={selectedMaxCopies}
                    isLoadingData={isLoadingData}
                    periodOptions={this.periodOptions}
                    monthNumberOptionsArray={this.monthNumberOptionsArray}
                    timeOptionsArray={this.timeOptionsArray}
                    maxNumberCopiesArray={this.maxNumberCopiesArray}
                    onClickCheckbox={this.onClickCheckbox}
                    onSelectMaxCopies={this.onSelectMaxCopies}
                    onSelectMonthNumberAndTimeOptions={
                      this.onSelectMonthNumberAndTimeOptions
                    }
                    onSelectWeedDay={this.onSelectWeedDay}
                    onSelectPeriod={this.onSelectPeriod}
                  />
                </>
              )}
            </StyledModules>

            <StyledModules>
              <RadioButton
                fontSize="13px"
                fontWeight="400"
                label={t("ThirdPartyResource")}
                name={"1"}
                onClick={this.onClickShowStorage}
                isChecked={isCheckedThirdParty}
                value="value"
                className="automatic-backup_radio-button"
              />
              <Text className="category-item-description">
                {t("ThirdPartyResourceDescription")}
              </Text>
              <Text className="category-item-description note_description">
                {t("ThirdPartyResourceNoteDescription")}
              </Text>
              {isShowThirdParty && (
                <>
                  <OperationsDialog
                    onSelectFolder={this.onSelectFolder}
                    name={"thirdParty"}
                    onClose={this.onClose}
                    onClickInput={this.onClickInput}
                    isPanelVisible={isPanelVisible}
                    folderList={commonThirdPartyList}
                    folderPath={folderThirdPartyModulePath}
                    isSetDefaultFolderPath={isSetDefaultFolderPath}
                    isError={isError}
                  />
                  <ScheduleComponent
                    weeklySchedule={weeklySchedule}
                    monthlySchedule={monthlySchedule}
                    weekOptions={weekOptions}
                    selectedOption={selectedOption}
                    selectedWeekdayOption={selectedWeekdayOption}
                    selectedTimeOption={selectedTimeOption}
                    selectedMonthOption={selectedMonthOption}
                    selectedMaxCopies={selectedMaxCopies}
                    isLoadingData={isLoadingData}
                    periodOptions={this.periodOptions}
                    monthNumberOptionsArray={this.monthNumberOptionsArray}
                    timeOptionsArray={this.timeOptionsArray}
                    maxNumberCopiesArray={this.maxNumberCopiesArray}
                    onClickCheckbox={this.onClickCheckbox}
                    onSelectMaxCopies={this.onSelectMaxCopies}
                    onSelectMonthNumberAndTimeOptions={
                      this.onSelectMonthNumberAndTimeOptions
                    }
                    onSelectWeedDay={this.onSelectWeedDay}
                    onSelectPeriod={this.onSelectPeriod}
                  />
                </>
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
                value="value"
                className="automatic-backup_radio-button"
              />
              <Text className="category-item-description">
                {t("ThirdPartyStorageDescription")}
              </Text>
              <Text className="category-item-description note_description">
                {t("ThirdPartyStorageNoteDescription")}
              </Text>
              {isShowThirdPartyStorage && (
                <>
                  <ThirdPartyStorageModule
                    fillStorageFields={this.fillStorageFields}
                    onCancelModuleSettings={this.onCancelModuleSettings}
                    isCopyingToLocal={isCopyingToLocal}
                    isLoadingData={isLoadingData}
                    isChanged={isChanged}
                  />
                  <ScheduleComponent
                    weeklySchedule={weeklySchedule}
                    monthlySchedule={monthlySchedule}
                    weekOptions={weekOptions}
                    selectedOption={selectedOption}
                    selectedWeekdayOption={selectedWeekdayOption}
                    selectedTimeOption={selectedTimeOption}
                    selectedMonthOption={selectedMonthOption}
                    selectedMaxCopies={selectedMaxCopies}
                    isLoadingData={isLoadingData}
                    periodOptions={this.periodOptions}
                    monthNumberOptionsArray={this.monthNumberOptionsArray}
                    timeOptionsArray={this.timeOptionsArray}
                    maxNumberCopiesArray={this.maxNumberCopiesArray}
                    onClickCheckbox={this.onClickCheckbox}
                    onSelectMaxCopies={this.onSelectMaxCopies}
                    onSelectMonthNumberAndTimeOptions={
                      this.onSelectMonthNumberAndTimeOptions
                    }
                    onSelectWeedDay={this.onSelectWeedDay}
                    onSelectPeriod={this.onSelectPeriod}
                  />
                </>
              )}
            </StyledModules>
            {isChanged && !isShowThirdPartyStorage && (
              <SaveCancelButtons
                className="team-template_buttons"
                onSaveClick={this.onSaveModuleSettings}
                onCancelClick={this.onCancelModuleSettings}
                showReminder={false}
                reminderTest={t("YouHaveUnsavedChanges")}
                saveButtonLabel={t("SaveButton")}
                cancelButtonLabel={t("CancelButton")}
                isDisabled={isCopyingToLocal || isLoadingData}
              />
            )}
          </>
        )}

        {/* {!isShowedStorageTypes && isChanged && (
          <Button
            label={t("SaveButton")}
            onClick={this.onClickDeleteSchedule}
            primary
            isDisabled={isCopyingToLocal || isLoadingData}
            size="medium"
            tabIndex={10}
          />
        )} */}

        {!isShowedStorageTypes && isChanged && (
          <SaveCancelButtons
            className="team-template_buttons"
            onSaveClick={this.onClickDeleteSchedule}
            onCancelClick={this.onCancelModuleSettings}
            showReminder={false}
            reminderTest={t("YouHaveUnsavedChanges")}
            saveButtonLabel={t("SaveButton")}
            cancelButtonLabel={t("CancelButton")}
            isDisabled={isCopyingToLocal || isLoadingData}
          />
        )}
      </StyledComponent>
    );
  }
}
export default inject(({ auth, setup }) => {
  const { language } = auth;
  const { panelVisible } = auth;
  const { getCommonThirdPartyList } = setup;
  const { commonThirdPartyList } = setup.dataManagement;
  return {
    language,
    panelVisible,
    getCommonThirdPartyList,
    commonThirdPartyList,
  };
})(withTranslation("Settings")(observer(AutomaticBackup)));
