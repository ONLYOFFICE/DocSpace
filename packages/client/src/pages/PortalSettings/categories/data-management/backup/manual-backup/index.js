import React from "react";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { startBackup } from "@docspace/common/api/portal";
import RadioButton from "@docspace/components/radio-button";
import toastr from "@docspace/components/toast/toastr";
import { BackupStorageType, FolderType } from "@docspace/common/constants";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import RoomsModule from "./sub-components/RoomsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";
import { StyledModules, StyledManualBackup } from "./../StyledBackup";
import { getFromLocalStorage, saveToLocalStorage } from "../../../../utils";
//import { getThirdPartyCommonFolderTree } from "@docspace/common/api/files";
import DataBackupLoader from "@docspace/common/components/Loaders/DataBackupLoader";
import {
  getBackupStorage,
  getStorageRegions,
} from "@docspace/common/api/settings";
import FloatingButton from "@docspace/components/floating-button";
import { getSettingsThirdParty } from "@docspace/common/api/files";

let selectedStorageType = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);
    selectedStorageType = getFromLocalStorage("LocalCopyStorageType");
    const checkedDocuments = selectedStorageType
      ? selectedStorageType === "Documents"
      : false;
    const checkedTemporary = selectedStorageType
      ? selectedStorageType === "TemporaryStorage"
      : true;
    const checkedThirdPartyResource = selectedStorageType
      ? selectedStorageType === "ThirdPartyResource"
      : false;
    const checkedThirdPartyStorage = selectedStorageType
      ? selectedStorageType === "ThirdPartyStorage"
      : false;

    this.timerId = null;

    this.state = {
      selectedFolder: "",
      isPanelVisible: false,
      isInitialLoading: false,
      isEmptyContentBeforeLoader: true,
      isCheckedTemporaryStorage: checkedTemporary,
      isCheckedDocuments: checkedDocuments,
      isCheckedThirdParty: checkedThirdPartyResource,
      isCheckedThirdPartyStorage: checkedThirdPartyStorage,
    };
    this.switches = [
      "isCheckedTemporaryStorage",
      "isCheckedDocuments",
      "isCheckedThirdParty",
      "isCheckedThirdPartyStorage",
    ];
  }

  setBasicSettings = async () => {
    const {
      getProgress,
      // setCommonThirdPartyList,
      t,
      setThirdPartyStorage,
      setStorageRegions,
      setConnectedThirdPartyAccount,
    } = this.props;
    try {
      getProgress(t);

      //if (isDocSpace) {
      const [account, backupStorage, storageRegions] = await Promise.all([
        getSettingsThirdParty(),
        getBackupStorage(),
        getStorageRegions(),
      ]);

      setConnectedThirdPartyAccount(account);
      setThirdPartyStorage(backupStorage);
      setStorageRegions(storageRegions);
      //   } else {
      //    const commonThirdPartyList = await getThirdPartyCommonFolderTree();
      // commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);
      //   }
    } catch (error) {
      toastr.error(error);
      //this.clearLocalStorage();
    }

    clearTimeout(this.timerId);
    this.timerId = null;

    this.setState({
      isInitialLoading: false,
      isEmptyContentBeforeLoader: false,
    });
  };

  componentDidMount() {
    const { fetchTreeFolders, rootFoldersTitles, isNotPaidPeriod } = this.props;
    const valueFromLocalStorage = getFromLocalStorage("LocalCopyStorageType");

    if (valueFromLocalStorage) {
      let newStateObj = {};
      const name = valueFromLocalStorage;
      newStateObj[name] = true;
      const newState = this.switches.filter((el) => el !== name);
      newState.forEach((name) => (newStateObj[name] = false));
      this.setState({
        ...newStateObj,
      });
    } else {
      saveToLocalStorage("LocalCopyStorageType", "isCheckedTemporaryStorage");
    }

    if (isNotPaidPeriod) {
      this.setState({
        isEmptyContentBeforeLoader: false,
      });

      return;
    }
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

  onMakeTemporaryBackup = async () => {
    const {
      getIntervalProgress,
      setDownloadingProgress,
      t,
      clearLocalStorage,
    } = this.props;
    const { TemporaryModuleType } = BackupStorageType;

    clearLocalStorage();

    try {
      await startBackup(`${TemporaryModuleType}`, null);
      setDownloadingProgress(1);
      getIntervalProgress(t);
    } catch (e) {
      toastr.error(t("BackupCreatedError"));
      console.error(err);
    }
  };
  onClickDownloadBackup = () => {
    const { temporaryLink } = this.props;
    const url = window.location.origin;
    const downloadUrl = `${url}` + `${temporaryLink}`;
    window.open(downloadUrl, "_self");
  };
  onClickShowStorage = (e) => {
    let newStateObj = {};
    const name = e.target.name;
    newStateObj[name] = true;
    const newState = this.switches.filter((el) => el !== name);
    newState.forEach((name) => (newStateObj[name] = false));
    this.setState({
      ...newStateObj,
    });
    saveToLocalStorage("LocalCopyStorageType", name);
  };
  onMakeCopy = async (
    selectedFolder,
    moduleName,
    moduleType,
    selectedStorageId,
    selectedStorageTitle
  ) => {
    const { isCheckedThirdPartyStorage } = this.state;
    const {
      t,
      getIntervalProgress,
      setDownloadingProgress,
      clearLocalStorage,
      setTemporaryLink,
      getStorageParams,
      saveToLocalStorage,
    } = this.props;

    clearLocalStorage();

    const storageParams = getStorageParams(
      isCheckedThirdPartyStorage,
      selectedFolder,
      selectedStorageId
    );

    const folderId = isCheckedThirdPartyStorage
      ? selectedStorageId
      : selectedFolder;

    saveToLocalStorage(
      isCheckedThirdPartyStorage,
      moduleName,
      folderId,
      selectedStorageTitle
    );

    try {
      await startBackup(moduleType, storageParams);
      setDownloadingProgress(1);
      setTemporaryLink("");
      getIntervalProgress(t);
    } catch (err) {
      toastr.error(t("BackupCreatedError"));
      console.error(err);
      //clearLocalStorage();
    }
  };
  render() {
    const {
      t,
      temporaryLink,
      downloadingProgress,
      //commonThirdPartyList,
      buttonSize,
      organizationName,
      renderTooltip,
      //isDocSpace,
      rootFoldersTitles,
      isNotPaidPeriod,
    } = this.props;
    const {
      isInitialLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isEmptyContentBeforeLoader,
    } = this.state;

    const isMaxProgress = downloadingProgress === 100;

    // const isDisabledThirdParty = isDocSpace
    //   ? false
    //   : commonThirdPartyList?.length === 0;

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };
    const commonModulesProps = {
      isMaxProgress,
      onMakeCopy: this.onMakeCopy,
      buttonSize,
    };

    const roomName = rootFoldersTitles[FolderType.USER]?.title;

    return isEmptyContentBeforeLoader && !isInitialLoading ? (
      <></>
    ) : isInitialLoading ? (
      <DataBackupLoader />
    ) : (
      <StyledManualBackup>
        <div className="backup_modules-header_wrapper">
          <Text isBold fontSize="16px">
            {t("DataBackup")}
          </Text>
          {renderTooltip(t("ManualBackupHelp"))}
        </div>
        <Text className="backup_modules-description">
          {t("ManualBackupDescription")}
        </Text>
        <StyledModules>
          <RadioButton
            label={t("TemporaryStorage")}
            name={"isCheckedTemporaryStorage"}
            key={0}
            isChecked={isCheckedTemporaryStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("TemporaryStorageDescription")}
          </Text>
          {isCheckedTemporaryStorage && (
            <div className="manual-backup_buttons">
              <Button
                label={t("Common:Create")}
                onClick={this.onMakeTemporaryBackup}
                primary
                isDisabled={!isMaxProgress}
                size={buttonSize}
              />
              {temporaryLink?.length > 0 && isMaxProgress && (
                <Button
                  label={t("DownloadCopy")}
                  onClick={this.onClickDownloadBackup}
                  isDisabled={false}
                  size={buttonSize}
                  style={{ marginLeft: "8px" }}
                />
              )}
              {!isMaxProgress && (
                <Button
                  label={t("Common:CopyOperation") + "..."}
                  isDisabled={true}
                  size={buttonSize}
                  style={{ marginLeft: "8px" }}
                />
              )}
            </div>
          )}
        </StyledModules>
        <StyledModules isDisabled={isNotPaidPeriod}>
          <RadioButton
            label={t("RoomsModule")}
            name={"isCheckedDocuments"}
            key={1}
            isChecked={isCheckedDocuments}
            isDisabled={!isMaxProgress || isNotPaidPeriod}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description module-documents">
            <Trans t={t} i18nKey="RoomsModuleDescription" ns="Settings">
              {{ roomName }}
            </Trans>
          </Text>
          {isCheckedDocuments && (
            <RoomsModule
              {...commonModulesProps}
              isCheckedDocuments={isCheckedDocuments}
            />
          )}
        </StyledModules>

        <StyledModules isDisabled={isNotPaidPeriod}>
          <RadioButton
            label={t("ThirdPartyResource")}
            name={"isCheckedThirdParty"}
            key={2}
            isChecked={isCheckedThirdParty}
            isDisabled={!isMaxProgress || isNotPaidPeriod}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          {isCheckedThirdParty && <ThirdPartyModule {...commonModulesProps} />}
        </StyledModules>
        <StyledModules isDisabled={isNotPaidPeriod}>
          <RadioButton
            label={t("Common:ThirdPartyStorage")}
            name={"isCheckedThirdPartyStorage"}
            key={3}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={!isMaxProgress || isNotPaidPeriod}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorageModule {...commonModulesProps} />
          )}
        </StyledModules>

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledManualBackup>
    );
  }
}

export default inject(({ auth, backup, treeFoldersStore }) => {
  const {
    clearProgressInterval,
    clearLocalStorage,
    // commonThirdPartyList,
    downloadingProgress,
    getProgress,
    getIntervalProgress,
    setDownloadingProgress,
    setTemporaryLink,
    // setCommonThirdPartyList,
    temporaryLink,
    getStorageParams,
    setThirdPartyStorage,
    setStorageRegions,
    saveToLocalStorage,
    setConnectedThirdPartyAccount,
  } = backup;
  const { currentTariffStatusStore } = auth;
  const { organizationName } = auth.settingsStore;
  const { rootFoldersTitles, fetchTreeFolders } = treeFoldersStore;
  const { isNotPaidPeriod } = currentTariffStatusStore;

  return {
    isNotPaidPeriod,
    organizationName,
    setThirdPartyStorage,
    clearProgressInterval,
    clearLocalStorage,
    // commonThirdPartyList,
    downloadingProgress,
    getProgress,
    getIntervalProgress,
    setDownloadingProgress,
    setTemporaryLink,
    setStorageRegions,
    // setCommonThirdPartyList,
    temporaryLink,
    getStorageParams,
    rootFoldersTitles,
    fetchTreeFolders,
    saveToLocalStorage,
    setConnectedThirdPartyAccount,
  };
})(withTranslation(["Settings", "Common"])(observer(ManualBackup)));
