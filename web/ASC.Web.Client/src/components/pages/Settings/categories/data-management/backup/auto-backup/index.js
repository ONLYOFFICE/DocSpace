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
import { isMobileOnly } from "react-device-detect";

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
      isEnable: false,
      isLoadingData: false,
      isInitialLoading: !isMobileOnly ? false : true,
      isChangedInStorage: false,
      isReset: false,
      isSuccessSave: false,
      downloadingProgress: 100,
    };

    this.periodsObject = [
      {
        key: 0,
        label: t("EveryDay"),
      },
      {
        key: 1,
        label: t("EveryWeek"),
      },
      {
        key: 2,
        label: t("EveryMonth"),
      },
    ];

    this._isMounted = false;
    this.hoursArray = [];
    this.monthNumbersArray = [];
    this.maxNumberCopiesArray = [];
    this.weekdaysLabelArray = [];
    this.timerId = null;
    this.formSettings = "";

    this.getTime();
    this.getMonthNumbers();
    this.getMaxNumberCopies();
  }

  setBasicSettings = async () => {
    const { downloadingProgress } = this.state;
    const {
      setDefaultOptions,
      t,
      setThirdPartyStorage,
      setBackupSchedule,
    } = this.props;
    try {
      const [
        commonThirdPartyList,
        backupProgress,
        backupSchedule,
        backupStorage,
      ] = await Promise.all([
        SelectFolderDialog.getCommonThirdPartyList(),
        getBackupProgress(),
        getBackupSchedule(),
        getBackupStorage(),
      ]);

      setThirdPartyStorage(backupStorage);
      setBackupSchedule(backupSchedule);
      this.commonThirdPartyList = commonThirdPartyList;

      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);

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

      this.setState({
        isEnable: !!backupSchedule,
        isInitialLoading: false,
      });
    } catch (error) {
      toastr.error(error);
    }
  };

  componentDidMount() {
    const {
      setDefaultOptions,
      t,
      backupSchedule,
      commonThirdPartyList,
    } = this.props;

    this._isMounted = true;
    this.getWeekdays();

    if (!isMobileOnly) {
      this.commonThirdPartyList = commonThirdPartyList;
      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);
      this.setState({
        isEnable: !!backupSchedule,
      });
    } else {
      this.setBasicSettings();
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { isChangedInStorage, isSuccessSave, isReset } = this.state;

    const { isChanged } = this.props;
    if (
      (isChangedInStorage !== prevState.isChangedInStorage ||
        isChanged !== prevProps.isChanged) &&
      isSuccessSave
    ) {
      this.setState({ isSuccessSave: false });
    }
    if (
      (isChangedInStorage !== prevState.isChangedInStorage ||
        isChanged !== prevProps.isChanged) &&
      isReset
    ) {
      this.setState({ isReset: false });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

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
            this.timerId && toastr.success(`${t("BackupCreatedSuccess")}`);
            this.timerId = null;
          }
        } else {
          clearInterval(this.timerId);
          this.timerId && toastr.success(`${t("BackupCreatedSuccess")}`);
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
    const { seStorageType, backupSchedule } = this.props;

    seStorageType(DOCUMENT_MODULE_TYPE.toString());

    if (backupSchedule) {
      this.setState({
        isChangedInStorage: !isEnable ? false : true,
      });
    } else {
      this.setState({
        isChangedInStorage: isEnable ? true : false,
      });
    }
    this.setState({
      isEnable: !isEnable,
    });
  };

  onClickShowStorage = (e) => {
    const { seStorageType } = this.props;
    const key = e.target.name;
    seStorageType(key);
  };

  onCancelModuleSettings = () => {
    const { isError, isEnable, isErrorsFields } = this.state;
    const { toDefault, backupSchedule } = this.props;

    toDefault();

    if (!!backupSchedule) {
      !isEnable && this.setState({ isEnable: true });
    } else {
      isEnable && this.setState({ isEnable: false });
    }
    this.setState({
      ...(isError && { isError: false }),
      ...(isErrorsFields && { isErrorsFields: false }),
      isChangedInStorage: false,
      isReset: true,
    });
  };

  canSave = () => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      selectedFolderId,
    } = this.props;

    if (
      (isCheckedDocuments && !selectedFolderId) ||
      (isCheckedThirdParty && !selectedFolderId)
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
    const { isEnable } = this.state;

    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,

      selectedMaxCopiesNumber,
      selectedPeriodNumber,
      selectedWeekday,
      selectedHour,
      selectedMonthDay,
      selectedStorageId,
      selectedFolderId,
    } = this.props;

    if (!isEnable) {
      this.deleteSchedule();
      return;
    }

    if (!this.canSave()) return;

    this.setState({ isLoadingData: true }, function () {
      let day, period;

      if (selectedPeriodNumber === "1") {
        period = EVERY_WEEK_TYPE;
        day = selectedWeekday;
      } else {
        if (selectedPeriodNumber === "2") {
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
    const {
      t,
      setThirdPartyStorage,
      setDefaultOptions,
      setBackupSchedule,
    } = this.props;

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

      setBackupSchedule(selectedSchedule);
      setThirdPartyStorage(storageInfo);
      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);

      toastr.success(t("SuccessfullySaveSettingsMessage"));
      this.setState({
        isLoadingData: false,
        isSuccessSave: true,
        isChangedInStorage: false,
      });
    } catch (e) {
      toastr.error(e);

      this._isMounted &&
        this.setState({
          isLoadingData: false,
        });
    }
  };

  deleteSchedule = () => {
    const { t, deleteSchedule } = this.props;
    this.setState({ isLoadingData: true }, () => {
      deleteBackupSchedule()
        .then(() => {
          deleteSchedule(this.weekdaysLabelArray);
          toastr.success(t("SuccessfullySaveSettingsMessage"));
          this.setState({
            isLoadingData: false,

            isChangedInStorage: false,
          });
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({
            isLoadingData: false,
            isChangedInStorage: false,
          });
        });
    });
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(proxyURL, "/settings/datamanagement/backup/manual-backup")
    );
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

  onSetRequiredFormNames = (namesArray) => {
    this.formNames = namesArray;
  };

  render() {
    const {
      t,
      isChanged,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
      isCheckedDocuments,
      backupSchedule,
    } = this.props;

    const {
      isInitialLoading,
      downloadingProgress,
      isEnable,
      isReset,
      isLoadingData,

      isError,
      isSuccessSave,
      isErrorsFields,
      isChangedInStorage,
    } = this.state;

    console.log("backupSchedule", backupSchedule);

    const isDisabledThirdPartyList = !!this.commonThirdPartyList;

    const commonProps = {
      isLoadingData,
      isReset,
      isSuccessSave,
      monthNumbersArray: this.monthNumbersArray,
      hoursArray: this.hoursArray,
      maxNumberCopiesArray: this.maxNumberCopiesArray,
      periodsObject: this.periodsObject,
      weekdaysLabelArray: this.weekdaysLabelArray,
    };

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };
    console.log("render auto ");

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
            {t("RestoreBackupDescription")}
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
                <DocumentsModule {...commonProps} isError={isError} />
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
                  onSetRequiredFormNames={this.onSetRequiredFormNames}
                  isErrorsFields={isErrorsFields}
                />
              )}
            </StyledModules>
          </div>
        )}

        {(isChangedInStorage || isChanged) && (
          <div className="auto-backup_buttons">
            <Button
              label={t("Common:SaveButton")}
              onClick={this.onSaveModuleSettings}
              primary
              isDisabled={isLoadingData}
              size="medium"
              className="save-button"
            />

            <Button
              label={t("Common:CancelButton")}
              isDisabled={isLoadingData}
              onClick={this.onCancelModuleSettings}
              size="medium"
            />
          </div>
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
export default inject(({ auth, backup }) => {
  const { language } = auth;
  const {
    setThirdPartyStorage,
    setDefaultOptions,
    setBackupSchedule,
    isChanged,
    toDefault,

    selectedStorageType,
    seStorageType,

    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedWeekday,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    setPeriod,
    setMonthNumber,
    setTime,
    setMaxCopies,
    setWeekday,
    selectedPeriodNumber,
    backupSchedule,
    deleteSchedule,

    selectedFolderId,
    selectedStorageId,
  } = backup;
  const isCheckedDocuments = selectedStorageType === `${DOCUMENT_MODULE_TYPE}`;
  const isCheckedThirdParty =
    selectedStorageType === `${RESOURCES_MODULE_TYPE}`;
  const isCheckedThirdPartyStorage =
    selectedStorageType === `${STORAGES_MODULE_TYPE}`;

  return {
    language,
    setThirdPartyStorage,
    setDefaultOptions,
    setBackupSchedule,
    isChanged,
    toDefault,
    selectedStorageType,
    seStorageType,

    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    setPeriod,
    setMonthNumber,
    setTime,
    setMaxCopies,
    setWeekday,
    selectedPeriodNumber,
    backupSchedule,
    deleteSchedule,
    selectedFolderId,
    selectedStorageId,
    selectedWeekday,

    isCheckedThirdPartyStorage,
    isCheckedThirdParty,
    isCheckedDocuments,
  };
})(withTranslation(["Settings", "Common"])(observer(AutomaticBackup)));
