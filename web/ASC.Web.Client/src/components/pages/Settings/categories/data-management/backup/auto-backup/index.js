import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButton from "@appserver/components/radio-button";
import moment from "moment";
import Button from "@appserver/components/button";
import {
  deleteBackupSchedule,
  getBackupSchedule,
  createBackupSchedule,
} from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import SelectFolderDialog from "files/SelectFolderDialog";
import Loader from "@appserver/components/loader";
import { AppServerConfig, BackupTypes } from "@appserver/common/constants";
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
const {
  DocumentModuleType,
  ResourcesModuleType,
  StorageModuleType,
  EveryDayType,
  EveryWeekType,
  EveryMonthType,
} = BackupTypes;

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
      isAdditionalChanged: false,
      isReset: false,
      isSuccessSave: false,
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
    const {
      setDefaultOptions,
      t,
      setThirdPartyStorage,
      setBackupSchedule,
      setCommonThirdPartyList,
      getProgress,
    } = this.props;
    try {
      getProgress(t);

      const [
        commonThirdPartyList,
        backupSchedule,
        backupStorage,
      ] = await Promise.all([
        SelectFolderDialog.getCommonThirdPartyList(),
        getBackupSchedule(),
        getBackupStorage(),
      ]);

      setThirdPartyStorage(backupStorage);
      setBackupSchedule(backupSchedule);
      commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);

      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);

      this.setState({
        isEnable: !!backupSchedule,
        isInitialLoading: false,
      });
    } catch (error) {
      toastr.error(error);
    }
  };

  componentDidMount() {
    const { setDefaultOptions, t, backupSchedule } = this.props;

    this._isMounted = true;
    this.getWeekdays();

    if (!isMobileOnly) {
      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);
      this.setState({
        isEnable: !!backupSchedule,
      });
    } else {
      this.setBasicSettings();
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { isAdditionalChanged, isSuccessSave, isReset } = this.state;

    const { isChanged } = this.props;
    if (
      (isAdditionalChanged !== prevState.isAdditionalChanged ||
        isChanged !== prevProps.isChanged) &&
      isSuccessSave
    ) {
      this.setState({ isSuccessSave: false });
    }
    if (
      (isAdditionalChanged !== prevState.isAdditionalChanged ||
        isChanged !== prevProps.isChanged) &&
      isReset
    ) {
      this.setState({ isReset: false });
    }
  }

  componentWillUnmount() {
    const { clearProgressInterval } = this.props;
    this._isMounted = false;
    clearProgressInterval();
  }

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
        label: `${t("MaxCopies", { copiesCount: item })}`,
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

    seStorageType(DocumentModuleType.toString());

    if (backupSchedule) {
      this.setState({
        isAdditionalChanged: !isEnable ? false : true,
      });
    } else {
      this.setState({
        isAdditionalChanged: isEnable ? true : false,
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
      isAdditionalChanged: false,
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
        period = EveryWeekType;
        day = selectedWeekday;
      } else {
        if (selectedPeriodNumber === "2") {
          period = EveryMonthType;
          day = selectedMonthDay;
        } else {
          period = EveryDayType;
          day = null;
        }
      }

      let time = selectedHour.substring(0, selectedHour.indexOf(":"));

      const storageType = isCheckedDocuments
        ? DocumentModuleType
        : isCheckedThirdParty
        ? ResourcesModuleType
        : StorageModuleType;

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
        isAdditionalChanged: false,
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

            isAdditionalChanged: false,
          });
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({
            isLoadingData: false,
            isAdditionalChanged: false,
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

  onSetIsChanged = (changed) => {
    this.setState({
      isAdditionalChanged: changed,
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
      downloadingProgress,
      commonThirdPartyList,
    } = this.props;

    const {
      isInitialLoading,
      isEnable,
      isReset,
      isLoadingData,

      isError,
      isSuccessSave,
      isErrorsFields,
      isAdditionalChanged,
    } = this.state;

    const isDisabledThirdPartyList = !!commonThirdPartyList;

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
    console.log("render auto ", this.state);

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
                name={`${DocumentModuleType}`}
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
                name={`${ResourcesModuleType}`}
                isChecked={isCheckedThirdParty}
                isDisabled={isLoadingData || isDisabledThirdPartyList}
              />
              <Text className="backup-description">
                {t("ThirdPartyResourceDescription")}
              </Text>

              {isCheckedThirdParty && (
                <ThirdPartyModule
                  {...commonProps}
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
                name={`${StorageModuleType}`}
                isChecked={isCheckedThirdPartyStorage}
                isDisabled={isLoadingData}
              />
              <Text className="backup-description">
                {t("ThirdPartyStorageDescription")}
              </Text>

              {isCheckedThirdPartyStorage && (
                <ThirdPartyStorageModule
                  {...commonProps}
                  onSetIsChanged={this.onSetIsChanged}
                  onSetFormSettings={this.onSetFormSettings}
                  onSetRequiredFormNames={this.onSetRequiredFormNames}
                  isErrorsFields={isErrorsFields}
                />
              )}
            </StyledModules>
          </div>
        )}

        {(isAdditionalChanged || isChanged) && (
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
        {isMobileOnly &&
          downloadingProgress > 0 &&
          downloadingProgress !== 100 && (
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
    setCommonThirdPartyList,

    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedWeekday,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,

    selectedPeriodNumber,
    backupSchedule,
    deleteSchedule,

    selectedFolderId,
    selectedStorageId,
    downloadingProgress,
    clearProgressInterval,
    getProgress,
    commonThirdPartyList,
  } = backup;
  const isCheckedDocuments = selectedStorageType === `${DocumentModuleType}`;
  const isCheckedThirdParty = selectedStorageType === `${ResourcesModuleType}`;
  const isCheckedThirdPartyStorage =
    selectedStorageType === `${StorageModuleType}`;

  return {
    language,
    setThirdPartyStorage,
    setDefaultOptions,
    setBackupSchedule,
    isChanged,
    toDefault,
    selectedStorageType,
    seStorageType,
    setCommonThirdPartyList,

    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,

    selectedPeriodNumber,
    backupSchedule,
    deleteSchedule,
    selectedFolderId,
    selectedStorageId,
    selectedWeekday,
    downloadingProgress,
    clearProgressInterval,
    getProgress,
    commonThirdPartyList,

    isCheckedThirdPartyStorage,
    isCheckedThirdParty,
    isCheckedDocuments,
  };
})(withTranslation(["Settings", "Common"])(observer(AutomaticBackup)));
