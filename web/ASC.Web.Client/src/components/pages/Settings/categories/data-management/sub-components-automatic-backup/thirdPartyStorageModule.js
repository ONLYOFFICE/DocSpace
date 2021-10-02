import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";
import toastr from "@appserver/components/toast/toastr";
import { startBackup } from "@appserver/common/api/portal";
import {
  createBackupSchedule,
  getBackupSchedule,
} from "@appserver/common/api/portal";

import GoogleCloudStorage from "./googleCloudStorage";
import RackspaceStorage from "./rackspaceStorage";
import SelectelStorage from "./selectelStorage";
import AmazonStorage from "./amazonStorage";
import { saveToSessionStorage, getFromSessionStorage } from "../.././../utils";
import ScheduleComponent from "../sub-components-automatic-backup/scheduleComponent";
import { ThirdPartyStorages } from "@appserver/common/constants";
import { StyledAutoBackup } from "../styled-backup";

let googleStorageId = "GoogleCloud";
let inputValueArray;

let numberPeriodFromSessionStorage = null;
let dayFromSessionStorage = "";
let timeFromSessionStorage = "";
let maxCopiesFromSessionStorage = "";

const settingNames = ["day", "time", "maxCopies", "numberPeriod"];
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const {
      t,
      selectedWeekdayOption,
      defaultSelectedOption,
      defaultDay,
      defaultHour,
      defaultMaxCopies,
      dailySchedule,
      monthlySchedule,
      weeklySchedule,
      periodOptions,
      weekOptions,
    } = this.props;

    this.isSetDefaultIdStorage = false;

    dayFromSessionStorage = getFromSessionStorage("day");
    timeFromSessionStorage = getFromSessionStorage("time");
    maxCopiesFromSessionStorage = getFromSessionStorage("maxCopies");
    numberPeriodFromSessionStorage = getFromSessionStorage("numberPeriod");
    const weekName =
      (numberPeriodFromSessionStorage === 2 && dayFromSessionStorage) ||
      (weeklySchedule && dayFromSessionStorage)
        ? weekOptions[+dayFromSessionStorage - 1].label
        : "";
    const periodName = numberPeriodFromSessionStorage
      ? periodOptions[+numberPeriodFromSessionStorage - 1].label
      : "";
    const numberMaxCopies = maxCopiesFromSessionStorage
      ? maxCopiesFromSessionStorage.substring(
          0,
          maxCopiesFromSessionStorage.indexOf(" ")
        )
      : "";

    const monthNumber = monthlySchedule
      ? dayFromSessionStorage || `${defaultDay}`
      : dayFromSessionStorage || "1";

    const weekdayNumber = weeklySchedule
      ? dayFromSessionStorage || defaultDay
      : dayFromSessionStorage || "2";

    const weekdayOption = numberPeriodFromSessionStorage
      ? numberPeriodFromSessionStorage === 2
      : weeklySchedule || false;
    const dayOption = numberPeriodFromSessionStorage
      ? numberPeriodFromSessionStorage === 1
      : dailySchedule || false;

    const monthOption = numberPeriodFromSessionStorage
      ? numberPeriodFromSessionStorage === 3
      : monthlySchedule || false;
    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      defaultSelectedStorage: "",
      selectedId: "",
      isLoading: false,
      isChanged: false,

      monthlySchedule: monthOption,
      dailySchedule: dayOption,
      weeklySchedule: weekdayOption,

      selectedOption:
        periodName || defaultSelectedOption || t("DailyPeriodSchedule"),
      selectedWeekdayOption: weekName || selectedWeekdayOption,
      selectedNumberWeekdayOption: weekdayNumber,
      selectedTimeOption: timeFromSessionStorage || defaultHour || "12:00",
      selectedMonthOption: monthNumber,
      selectedMaxCopies:
        maxCopiesFromSessionStorage || defaultMaxCopies || "10",
      selectedNumberMaxCopies: numberMaxCopies || defaultMaxCopies || "10",
    };
    this.isFirstSet = false;
    this.firstSetId = "";
    this._isMounted = false;
  }
  componentDidMount() {
    this._isMounted = true;
    const { onSetLoadingData } = this.props;

    onSetLoadingData && onSetLoadingData(true);
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupStorage()
          .then((storageBackup) => this.getOptions(storageBackup))
          .then(() => this.checkChanges())
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);
            this.setState({ isLoading: false });
          });
      }
    );
  }

  componentWillUnmount() {
    this._isMounted = false;
  }
  getOptions = (storageBackup) => {
    this.setState({
      isLoading: true,
    });
    let options = [];
    let availableStorage = {};

    //debugger;
    for (let item = 0; item < storageBackup.length; item++) {
      // debugger;
      let obj = {
        [storageBackup[item].id]: {
          isSet: storageBackup[item].isSet,
          properties: storageBackup[item].properties,
          title: storageBackup[item].title,
          id: storageBackup[item].id,
        },
      };
      let titleObj = {
        key: storageBackup[item].id,
        label: storageBackup[item].title,
        disabled: false,
      };
      options.push(titleObj);

      availableStorage = { ...availableStorage, ...obj };
      //debugger;
      console.log("availableStorage", availableStorage);

      if (storageBackup[item].current) {
        this.isSetDefaultIdStorage = true;

        this.setState({
          selectedStorage: storageBackup[item].title,
          defaultSelectedStorage: storageBackup[item].title,
          selectedId: storageBackup[item].id,
          defaultSelectedId: storageBackup[item].id,
        });
      }

      if (!this.isFirstSet && storageBackup[item].isSet) {
        this.isFirstSet = true;
        this.firstSetId = storageBackup[item].id;
      }
    }

    if (!this.isSetDefaultIdStorage && !this.isFirstSet) {
      this.setState({
        selectedStorage: availableStorage[googleStorageId].title,
        defaultSelectedStorage: availableStorage[googleStorageId].title,
        selectedId: availableStorage[googleStorageId].id,
        defaultSelectedId: availableStorage[googleStorageId].id,
      });
    }

    if (!this.isSetDefaultIdStorage && this.isFirstSet) {
      this.setState({
        selectedStorage: availableStorage[this.firstSetId].title,
        defaultSelectedStorage: availableStorage[this.firstSetId].title,
        selectedId: availableStorage[this.firstSetId].id,
        defaultSelectedId: availableStorage[this.firstSetId].id,
      });
    }

    this.setState({
      availableOptions: options,
      availableStorage: availableStorage,
      isLoading: false,
    });
  };

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage, isSetDefaultStorage } = this.state;
    const { onSetDisableOptions } = this.props;

    this.setState(
      {
        selectedStorage: availableStorage[selectedStorageId].title,
        selectedId: availableStorage[selectedStorageId].id,
        //isChanged: true,
        //isSetDefaultStorage: false,
        // isError: false,
        // input_1: "",
        // input_2: "",
        // input_3: "", //в них нет необходимости
        // input_4: "",
        // input_5: "",
        // input_6: "",
      },
      function () {
        this.checkChanges();
      }
    );
    //if (isSetDefaultStorage) onSetDisableOptions && onSetDisableOptions(false);
  };

  fillInputValueArray = (inputNumber, valuesArray) => {
    const { selectedId, availableStorage } = this.state;
    let obj = {};
    inputValueArray = [];

    const selectedStorage = availableStorage[selectedId];

    for (let i = 1; i <= inputNumber; i++) {
      obj = {
        key: selectedStorage.properties[i - 1].name,
        value: valuesArray[i - 1],
      };
      inputValueArray.push(obj);
    }
    this.onSaveModuleSettings();
  };

  onSelectPeriod = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;
    //debugger;

    saveToSessionStorage("numberPeriod", key);

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

  onSelectWeekDay = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;
    //debugger;
    saveToSessionStorage("day", key);

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
      saveToSessionStorage("time", label);
      this.setState({ selectedTimeOption: label }, function () {
        this.checkChanges();
      });
    } else {
      saveToSessionStorage("day", label);
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

    saveToSessionStorage("maxCopies", label);

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
  checkChanges = () => {
    const { defaultStorageType } = this.props;
    const { isChanged } = this.state;

    let changed;

    if (+defaultStorageType === 5) {
      changed = this.checkOptions();
      isChanged !== changed &&
        this.setState({
          isChanged: changed,
        });
      return;
    } else {
      isChanged !== changed &&
        this.setState({
          isChanged: true,
        });
      return;
    }
  };
  checkOptions = () => {
    const {
      selectedTimeOption,
      dailySchedule,
      monthlySchedule,
      weeklySchedule,
      selectedMonthOption,
      selectedNumberMaxCopies,
      selectedNumberWeekdayOption,
      selectedId,
      defaultSelectedId,
    } = this.state;

    const {
      defaultHour,
      defaultMaxCopies,
      defaultPeriod,
      defaultDay,
      defaultSelectedWeekdayOption,
    } = this.props;
    //debugger;
    if (selectedId !== defaultSelectedId) {
      return true;
    }
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

    if (weeklySchedule) {
      if (+selectedNumberWeekdayOption !== defaultSelectedWeekdayOption) {
        return true;
      }
    }

    return false;
  };
  onSaveModuleSettings = () => {
    const {
      weeklySchedule,
      selectedTimeOption,
      monthlySchedule,
      selectedMonthOption,
      selectedNumberWeekdayOption,
      selectedNumberMaxCopies,
      isError,
      selectedId,
      availableStorage,
    } = this.state;
    const {
      t,
      changedDefaultOptions,
      onSetDefaultOptions,
      onSetLoadingData,
    } = this.props;
    let storageParams = [];

    onSetLoadingData && onSetLoadingData(true);
    settingNames.forEach((settingName) => {
      saveToSessionStorage(settingName, "");
    });
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
      let storageType = "5";

      //console.log("storageFiledValue", this.storageFiledValue);

      storageParams = [
        {
          key: "module",
          value: selectedId,
        },
      ];

      let obj = {};

      for (let i = 0; i < inputValueArray.length; i++) {
        obj = {
          key: inputValueArray[i].key,
          value: inputValueArray[i].value,
        };
        storageParams.push(obj);
      }

      //debugger;
      let defaultSelectedFolder;
      let folderId;
      this.isSetDefaultIdStorage = true;

      createBackupSchedule(
        storageType,
        storageParams,
        selectedNumberMaxCopies,
        period,
        time,
        day
      )
        .then(() => getBackupSchedule())
        .then((selectedSchedule) => {
          if (selectedSchedule) {
            folderId = selectedSchedule.storageParams.folderId;

            defaultSelectedFolder = folderId;
            const defaultStorageType = `${selectedSchedule.storageType}`;
            const defaultHour = `${selectedSchedule.cronParams.hour}:00`;
            const defaultPeriod = `${selectedSchedule.cronParams.period}`;
            const defaultDay = selectedSchedule.cronParams.day;
            const defaultMaxCopies = `${selectedSchedule.backupsStored}`;

            changedDefaultOptions(
              defaultSelectedFolder,
              defaultStorageType,
              defaultHour,
              defaultPeriod,
              defaultDay,
              defaultMaxCopies
            );
          }
        })
        .then(() => getBackupStorage())
        .then((storageBackup) => this.getOptions(storageBackup))
        .then(() => {
          this._isMounted && onSetDefaultOptions();
        })
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => console.log("error", error))
        .finally(() => {
          this._isMounted && onSetLoadingData && onSetLoadingData(false);
          this._isMounted &&
            this.setState({
              isLoadingData: false,
              isChanged: false,
              defaultSelectedId: selectedId,
              defaultSelectedStorage: availableStorage[selectedId].title,
            });
        });
    });
  };
  // onCancelSettings = () => {
  //   // перенести в каждый компонент
  //   const {
  //     defaultSelectedOption,
  //     defaultSelectedId,
  //     isSetDefaultStorage,
  //   } = this.state;
  //   const { onCancelModuleSettings, onSetDisableOptions } = this.props;

  //   if (this.isSetDefaultIdStorage) {
  //     this.setState({
  //       isSetDefaultStorage: true,
  //     });
  //     onSetDisableOptions && onSetDisableOptions(true);
  //   }

  //   this.setState({
  //     selectedStorage: defaultSelectedOption,
  //     selectedId: defaultSelectedId,
  //     isChanged: false,
  //     input_1: "",
  //     input_2: "",
  //     input_3: "",
  //     input_4: "",
  //     input_5: "",
  //     input_6: "",
  //   });

  //   if (this.isSetDefaultIdStorage && !isSetDefaultStorage) {
  //     this.setState({
  //       isSetDefaultStorage: true,
  //     });
  //   }
  //   onCancelModuleSettings();
  // };

  onCancelSettings = () => {
    const {
      onCancelModuleSettings,
      periodOptions,
      weekdaysOptions,
      defaultPeriod,
      defaultHour,
      defaultMaxCopies,
      defaultDay,
      lng,
      defaultWeekly,
      defaultDaily,
      defaultMonthly,
      defaultStorageType,
      onSetLoadingData,
    } = this.props;
    const {
      monthlySchedule,
      weeklySchedule,
      defaultSelectedStorage,
      defaultSelectedId,
      isError,
    } = this.state;

    onSetLoadingData && onSetLoadingData(true);

    settingNames.forEach((settingName) => {
      saveToSessionStorage(settingName, "");
    });

    if (defaultStorageType) {
      this.setState({
        isSetDefaultFolderPath: true,
        selectedTimeOption: defaultHour,
        selectedMaxCopies: defaultMaxCopies,
        selectedNumberMaxCopies: defaultMaxCopies,
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
      let defaultSelectedOption;
      let defaultSelectedWeekdayOption;

      if (+defaultPeriod === 1) {
        //Every Week option
        //debugger;
        const arrayIndex = lng === "en" ? defaultDay : defaultDay - 1; //selected number of week
        defaultSelectedOption = periodOptions[1].label;
        defaultSelectedWeekdayOption = defaultDay;
        // defaultWeekly = true;
        this.setState({
          selectedOption: defaultSelectedOption,
          weeklySchedule: true,
          selectedWeekdayOption: weekdaysOptions[arrayIndex].label,
          selectedNumberWeekdayOption: defaultDay,
        });
      } else {
        if (+defaultPeriod === 2) {
          //Every Month option
          defaultSelectedOption = periodOptions[2].label;
          this.setState({
            selectedOption: defaultSelectedOption,
            monthlySchedule: true,
            selectedMonthOption: `${defaultDay}`, //selected day of month
          });
        } else {
          defaultSelectedOption = periodOptions[0].label;
          this.setState({
            selectedOption: defaultSelectedOption,
          });
        }
      }
    }
    this.setState({
      isChanged: false,
      selectedStorage: defaultSelectedStorage,
      selectedId: defaultSelectedId,
    });
    onCancelModuleSettings();
    onSetLoadingData && onSetLoadingData(false);
  };
  onMakeCopy = () => {
    const { setInterval } = this.props;
    const { selectedId } = this.state;

    let storageParams = [
      {
        key: "module",
        value: selectedId,
      },
    ];

    let obj = {};

    for (let i = 0; i < inputValueArray.length; i++) {
      obj = {
        key: inputValueArray[i].key,
        value: inputValueArray[i].value,
      };
      storageParams.push(obj);
    }

    startBackup("5", storageParams);
    setInterval();
  };

  isInvalidForm = (formSettings) => {
    let errors = {};
    let firstError = false;

    for (let key in formSettings) {
      const elem = formSettings[key];
      errors[key] = !elem.trim();

      if (!elem.trim() && !firstError) {
        firstError = true;
      }
    }

    return [firstError, errors];
  };

  render() {
    const {
      t,
      helpUrlCreatingBackup,
      isLoadingData,
      isCopyingToLocal,
      maxProgress,
      weekOptions,
      periodOptions,
      monthNumberOptionsArray,
      timeOptionsArray,
      maxNumberCopiesArray,
    } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedStorage,
      selectedOption,
      isLoading,
      selectedId,
      isChanged,
      defaultSelectedId,
      selectedWeekdayOption,
      selectedTimeOption,
      selectedMonthOption,
      selectedMaxCopies,
      weeklySchedule,
      monthlySchedule,
    } = this.state;

    return (
      <StyledAutoBackup>
        <Box marginProp="16px 0 16px 0">
          <Link
            color="#316DAA"
            target="_blank"
            isHovered={true}
            href={helpUrlCreatingBackup}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isLoadingData || isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          className="backup_combo"
        />

        {selectedId === ThirdPartyStorages.GoogleId && !isLoading && (
          <GoogleCloudStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            isCopyingToLocal={isCopyingToLocal}
            maxProgress={maxProgress}
            isChanged={isChanged}
            isChanged={isChanged}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
            onCancelSettings={this.onCancelSettings}
            currentStorageId={
              this.isSetDefaultIdStorage ? defaultSelectedId : ""
            }
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.RackspaceId && !isLoading && (
          <RackspaceStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            isCopyingToLocal={isCopyingToLocal}
            maxProgress={maxProgress}
            isChanged={isChanged}
            isChanged={isChanged}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
            onCancelSettings={this.onCancelSettings}
            currentStorageId={
              this.isSetDefaultIdStorage ? defaultSelectedId : ""
            }
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.SelectelId && !isLoading && (
          <SelectelStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            isCopyingToLocal={isCopyingToLocal}
            maxProgress={maxProgress}
            isChanged={isChanged}
            isChanged={isChanged}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
            onCancelSettings={this.onCancelSettings}
            currentStorageId={
              this.isSetDefaultIdStorage ? defaultSelectedId : ""
            }
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.AmazonId && !isLoading && (
          <AmazonStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            isCopyingToLocal={isCopyingToLocal}
            maxProgress={maxProgress}
            isChanged={isChanged}
            isChanged={isChanged}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
            onCancelSettings={this.onCancelSettings}
            currentStorageId={
              this.isSetDefaultIdStorage ? defaultSelectedId : ""
            }
            isInvalidForm={this.isInvalidForm}
          />
        )}

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
          periodOptions={periodOptions}
          monthNumberOptionsArray={monthNumberOptionsArray}
          timeOptionsArray={timeOptionsArray}
          maxNumberCopiesArray={maxNumberCopiesArray}
          onSelectMaxCopies={this.onSelectMaxCopies}
          onSelectMonthNumberAndTimeOptions={
            this.onSelectMonthNumberAndTimeOptions
          }
          onSelectWeekDay={this.onSelectWeekDay}
          onSelectPeriod={this.onSelectPeriod}
        />
      </StyledAutoBackup>
    );
  }
}

export default inject(({ auth }) => {
  const { helpUrlCreatingBackup } = auth.settingsStore;

  return {
    helpUrlCreatingBackup,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyStorageModule)));
