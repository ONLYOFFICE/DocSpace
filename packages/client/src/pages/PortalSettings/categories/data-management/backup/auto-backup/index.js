import React from "react";
import moment from "moment";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButton from "@docspace/components/radio-button";
import Text from "@docspace/components/text";
import {
  deleteBackupSchedule,
  getBackupSchedule,
  createBackupSchedule,
  enableAutoBackup,
} from "@docspace/common/api/portal";
import toastr from "@docspace/components/toast/toastr";
import {
  BackupStorageType,
  AutoBackupPeriod,
  FolderType,
} from "@docspace/common/constants";
import ToggleButton from "@docspace/components/toggle-button";
import {
  getBackupStorage,
  getStorageRegions,
} from "@docspace/common/api/settings";
import { StyledModules, StyledAutoBackup } from "../StyledBackup";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import DocumentsModule from "./sub-components/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";
//import { getThirdPartyCommonFolderTree } from "@docspace/common/api/files";
import ButtonContainer from "./sub-components/ButtonContainer";
import AutoBackupLoader from "@docspace/common/components/Loaders/AutoBackupLoader";
import FloatingButton from "@docspace/common/components/FloatingButton";

const {
  DocumentModuleType,
  ResourcesModuleType,
  StorageModuleType,
} = BackupStorageType;
const { EveryDayType, EveryWeekType, EveryMonthType } = AutoBackupPeriod;
class AutomaticBackup extends React.PureComponent {
  constructor(props) {
    super(props);
    const { t, language } = props;
    moment.locale(language);

    this.state = {
      isLoadingData: false,
      isInitialLoading: false,
      isEmptyContentBeforeLoader: true,
      isError: false,
      isEnableAuto: false,
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
    this.timerId = null;
    this.hoursArray = [];
    this.monthNumbersArray = [];
    this.maxNumberCopiesArray = [];
    this.weekdaysLabelArray = [];

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
      //setCommonThirdPartyList,
      getProgress,
      setStorageRegions,
    } = this.props;

    try {
      getProgress(t);

      const [
        ///thirdPartyList,
        backupSchedule,
        backupStorage,
        enableAuto,
        storageRegions,
      ] = await Promise.all([
        //getThirdPartyCommonFolderTree(),
        getBackupSchedule(),
        getBackupStorage(),
        enableAutoBackup(),
        getStorageRegions(),
      ]);

      setThirdPartyStorage(backupStorage);
      setBackupSchedule(backupSchedule);
      setStorageRegions(storageRegions);
      //thirdPartyList && setCommonThirdPartyList(thirdPartyList);

      setDefaultOptions(t, this.periodsObject, this.weekdaysLabelArray);

      clearTimeout(this.timerId);
      this.timerId = null;

      this.setState({
        isEmptyContentBeforeLoader: false,
        isInitialLoading: false,
        isEnableAuto: enableAuto,
      });
    } catch (error) {
      toastr.error(error);
      clearTimeout(this.timerId);
      this.timerId = null;
      this.setState({
        isEmptyContentBeforeLoader: false,
        isInitialLoading: false,
      });
    }
  };

  componentDidMount() {
    const { fetchTreeFolders, rootFoldersTitles } = this.props;

    this.getWeekdays();

    this.timerId = setTimeout(() => {
      this.setState({ isInitialLoading: true });
    }, 200);

    if (Object.keys(rootFoldersTitles).length === 0) fetchTreeFolders();

    this.setBasicSettings();
  }

  componentWillUnmount() {
    const { clearProgressInterval } = this.props;
    clearTimeout(this.timerId);
    this.timerId = null;
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
    const { language } = this.props;
    const gettingWeekdays = moment.weekdays();
    for (let item = 0; item < gettingWeekdays.length; item++) {
      let obj = {
        key: `${item + 1}`,
        label: `${gettingWeekdays[item]}`,
      };
      this.weekdaysLabelArray.push(obj);
    }
    const isEnglishLanguage = language === "en";
    if (!isEnglishLanguage) {
      this.weekdaysLabelArray.push(this.weekdaysLabelArray.shift());
    }
  };

  onClickPermissions = () => {
    const { seStorageType, setSelectedEnableSchedule } = this.props;

    seStorageType(DocumentModuleType.toString());

    setSelectedEnableSchedule();
  };

  onClickShowStorage = (e) => {
    const { seStorageType } = this.props;
    const key = e.target.name;
    seStorageType(key);
  };

  onCancelModuleSettings = () => {
    const { isError } = this.state;
    const {
      toDefault,
      isCheckedThirdParty,
      isCheckedDocuments,
      setResetProcess,
    } = this.props;

    toDefault();

    (isCheckedThirdParty || isCheckedDocuments) && setResetProcess(true);
    this.setState({
      ...(isError && { isError: false }),
    });
  };

  canSave = () => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      selectedFolderId,
      isFormReady,
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
      return isFormReady();
    }
    return true;
  };
  onSaveModuleSettings = async () => {
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
      selectedEnableSchedule,
      getStorageParams,
    } = this.props;

    if (!selectedEnableSchedule) {
      this.deleteSchedule();
      return;
    }

    if (!this.canSave()) return;
    //return;
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

      const storageParams = getStorageParams(
        isCheckedThirdPartyStorage,
        selectedFolderId,
        selectedStorageId
      );

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

      isCheckedThirdParty,
      isCheckedDocuments,
      setSavingProcess,
    } = this.props;

    try {
      (isCheckedThirdParty || isCheckedDocuments) && setSavingProcess(true);

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
      });
    } catch (e) {
      toastr.error(e);

      (isCheckedThirdParty || isCheckedDocuments) && setSavingProcess(true);

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
          });
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({
            isLoadingData: false,
          });
        });
    });
  };

  render() {
    const {
      t,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
      isCheckedDocuments,
      //commonThirdPartyList,
      buttonSize,
      downloadingProgress,
      theme,
      renderTooltip,
      selectedEnableSchedule,
      organizationName,
      rootFoldersTitles,
    } = this.props;

    const {
      isInitialLoading,
      isLoadingData,
      isError,
      isEnableAuto,
      isEmptyContentBeforeLoader,
    } = this.state;

    // const isDisabledThirdPartyList =
    //   isCheckedThirdParty || isDocSpace
    //     ? false
    //     : commonThirdPartyList?.length === 0;

    const commonProps = {
      isLoadingData,
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

    const roomName = rootFoldersTitles[FolderType.Rooms];

    return isEmptyContentBeforeLoader && !isInitialLoading ? (
      <></>
    ) : isInitialLoading ? (
      <AutoBackupLoader />
    ) : (
      <StyledAutoBackup theme={theme} isEnableAuto={isEnableAuto}>
        <div className="backup_modules-header_wrapper">
          <Text isBold fontSize="16px">
            {t("AutoBackup")}
          </Text>
          {renderTooltip(
            t("AutoBackupHelp") +
              " " +
              t("AutoBackupHelpNote", { organizationName })
          )}
        </div>
        <Text className="backup_modules-description">
          {t("AutoBackupDescription")}
        </Text>
        <div className="backup_toggle-wrapper">
          <ToggleButton
            className="backup_toggle-btn"
            label={t("EnableAutomaticBackup")}
            onChange={this.onClickPermissions}
            isChecked={selectedEnableSchedule}
            isDisabled={isLoadingData || !isEnableAuto}
          />
          <Text className="backup_toggle-btn-description">
            {t("EnableAutomaticBackupDescription")}
          </Text>
        </div>
        {selectedEnableSchedule && isEnableAuto && (
          <div className="backup_modules">
            <StyledModules>
              <RadioButton
                {...commonRadioButtonProps}
                label={organizationName}
                name={`${DocumentModuleType}`}
                key={0}
                isChecked={isCheckedDocuments}
                isDisabled={isLoadingData}
              />
              <Text className="backup-description">
                <Trans t={t} i18nKey="DocumentsModuleDescription" ns="Settings">
                  {{ roomName }}
                </Trans>
              </Text>
              {isCheckedDocuments && (
                <DocumentsModule {...commonProps} isError={isError} />
              )}
            </StyledModules>

            <StyledModules
            // isDisabled={isDisabledThirdPartyList}
            >
              <RadioButton
                {...commonRadioButtonProps}
                label={t("ThirdPartyResource")}
                name={`${ResourcesModuleType}`}
                isChecked={isCheckedThirdParty}
                isDisabled={
                  isLoadingData
                  // || isDisabledThirdPartyList
                }
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
                <ThirdPartyStorageModule {...commonProps} />
              )}
            </StyledModules>
          </div>
        )}

        <ButtonContainer
          t={t}
          isLoadingData={isLoadingData}
          buttonSize={buttonSize}
          onSaveModuleSettings={this.onSaveModuleSettings}
          onCancelModuleSettings={this.onCancelModuleSettings}
        />

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledAutoBackup>
    );
  }
}
export default inject(({ auth, backup, treeFoldersStore }) => {
  const { language, settingsStore } = auth;
  const { organizationName, theme } = settingsStore;
  const {
    downloadingProgress,
    backupSchedule,
    //commonThirdPartyList,
    clearProgressInterval,
    deleteSchedule,
    getProgress,
    setThirdPartyStorage,
    setDefaultOptions,
    setBackupSchedule,
    selectedStorageType,
    seStorageType,
    //setCommonThirdPartyList,
    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedWeekday,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    selectedPeriodNumber,
    selectedFolderId,
    selectedStorageId,
    toDefault,
    isFormReady,
    getStorageParams,
    setSelectedEnableSchedule,
    selectedEnableSchedule,
    updatePathSettings,
    setSavingProcess,
    setResetProcess,
    setStorageRegions,
  } = backup;

  const isCheckedDocuments = selectedStorageType === `${DocumentModuleType}`;
  const isCheckedThirdParty = selectedStorageType === `${ResourcesModuleType}`;
  const isCheckedThirdPartyStorage =
    selectedStorageType === `${StorageModuleType}`;

  const { rootFoldersTitles, fetchTreeFolders } = treeFoldersStore;
  return {
    fetchTreeFolders,
    rootFoldersTitles,
    downloadingProgress,
    theme,
    language,
    isFormReady,
    organizationName,
    backupSchedule,
    //commonThirdPartyList,
    clearProgressInterval,
    deleteSchedule,
    getProgress,
    setThirdPartyStorage,
    setDefaultOptions,
    setBackupSchedule,
    selectedStorageType,
    seStorageType,
    //setCommonThirdPartyList,
    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedWeekday,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    selectedPeriodNumber,
    selectedFolderId,
    selectedStorageId,

    toDefault,

    isCheckedThirdPartyStorage,
    isCheckedThirdParty,
    isCheckedDocuments,

    getStorageParams,

    setSelectedEnableSchedule,
    selectedEnableSchedule,

    updatePathSettings,
    setSavingProcess,
    setResetProcess,
    setStorageRegions,
  };
})(withTranslation(["Settings", "Common"])(observer(AutomaticBackup)));
