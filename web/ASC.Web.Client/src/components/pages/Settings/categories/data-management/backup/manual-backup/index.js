import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { startBackup } from "@appserver/common/api/portal";
import RadioButton from "@appserver/components/radio-button";
import toastr from "@appserver/components/toast/toastr";
import { BackupStorageType } from "@appserver/common/constants";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import DocumentsModule from "./sub-components/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";
import { StyledModules, StyledManualBackup } from "./../StyledBackup";
import { saveToSessionStorage, getFromSessionStorage } from "../../../../utils";
import { getThirdPartyCommonFolderTree } from "@appserver/common/api/files";
import DataBackupLoader from "@appserver/common/components/Loaders/DataBackupLoader";

let selectedStorageType = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);

    selectedStorageType = getFromSessionStorage("LocalCopyStorageType");

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

    this.state = {
      selectedFolder: "",
      isPanelVisible: false,
      isInitialLoading: true,
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
    const { getProgress, setCommonThirdPartyList, t, isDocSpace } = this.props;
    try {
      getProgress(t);

      if (isDocSpace) {
        //TODO: another parameter

        const commonThirdPartyList = await getThirdPartyCommonFolderTree();
        commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);
      } else {
        const commonThirdPartyList = await getThirdPartyCommonFolderTree();
        commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);
      }
    } catch (error) {
      console.error(error);
      this.clearSessionStorage();
    }

    this.setState({
      isInitialLoading: false,
    });
  };

  componentDidMount() {
    this.setBasicSettings();
  }

  componentWillUnmount() {
    const { clearProgressInterval } = this.props;
    clearProgressInterval();
  }

  onMakeTemporaryBackup = async () => {
    const { getIntervalProgress, setDownloadingProgress, t } = this.props;
    const { TemporaryModuleType } = BackupStorageType;

    saveToSessionStorage("LocalCopyStorageType", "TemporaryStorage");

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
  };

  onMakeCopy = async (
    selectedFolder,
    moduleName,
    moduleType,
    selectedStorageId,
    selectedStorage
  ) => {
    const {
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    const {
      t,
      getIntervalProgress,
      setDownloadingProgress,
      clearSessionStorage,
      setTemporaryLink,
      getStorageParams,
    } = this.props;

    const storageParams = getStorageParams(
      isCheckedThirdPartyStorage,
      selectedFolder,
      selectedStorageId
    );

    saveToSessionStorage("LocalCopyStorageType", moduleName);

    if (isCheckedDocuments || isCheckedThirdParty) {
      saveToSessionStorage("LocalCopyFolder", `${selectedFolder}`);
    } else {
      saveToSessionStorage("LocalCopyStorage", `${selectedStorageId}`);
      saveToSessionStorage(
        "LocalCopyThirdPartyStorageType",
        `${selectedStorage}`
      );
    }
    console.log("storageParams", storageParams);
    return;
    try {
      await startBackup(moduleType, storageParams);
      setDownloadingProgress(1);
      setTemporaryLink("");
      getIntervalProgress(t);
    } catch (err) {
      toastr.error(t("BackupCreatedError"));
      console.error(err);

      clearSessionStorage();
    }
  };
  render() {
    const {
      t,
      temporaryLink,
      downloadingProgress,
      commonThirdPartyList,
      buttonSize,
      organizationName,
      renderTooltip,
      isDocSpace,
    } = this.props;
    const {
      isInitialLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;

    const isMaxProgress = downloadingProgress === 100;

    const isDisabledThirdParty = isDocSpace
      ? false
      : commonThirdPartyList?.length === 0;

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

    return isInitialLoading ? (
      <DataBackupLoader />
    ) : (
      <StyledManualBackup>
        <div className="backup_modules-header_wrapper">
          <Text isBold fontSize="16px">
            {t("DataBackup")}
          </Text>
          {renderTooltip(
            t("ManualBackupHelp") +
              " " +
              t("ManualBackupHelpNote", { organizationName })
          )}
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
                label={t("Common:Duplicate")}
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

        <StyledModules>
          <RadioButton
            label={t("DocumentsModule")}
            name={"isCheckedDocuments"}
            key={1}
            isChecked={isCheckedDocuments}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />

          <Text className="backup-description module-documents">
            {t("DocumentsModuleDescription")}
          </Text>

          {isCheckedDocuments && (
            <DocumentsModule
              {...commonModulesProps}
              isCheckedDocuments={isCheckedDocuments}
            />
          )}
        </StyledModules>

        <StyledModules isDisabled={isDisabledThirdParty}>
          <RadioButton
            label={t("ThirdPartyResource")}
            name={"isCheckedThirdParty"}
            key={2}
            isChecked={isCheckedThirdParty}
            isDisabled={isDisabledThirdParty || !isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          {isCheckedThirdParty && <ThirdPartyModule {...commonModulesProps} />}
        </StyledModules>

        <StyledModules>
          <RadioButton
            label={t("ThirdPartyStorage")}
            name={"isCheckedThirdPartyStorage"}
            key={3}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorageModule {...commonModulesProps} />
          )}
        </StyledModules>
      </StyledManualBackup>
    );
  }
}

export default inject(({ auth, backup }) => {
  const {
    clearProgressInterval,
    clearSessionStorage,
    commonThirdPartyList,
    downloadingProgress,
    getProgress,
    getIntervalProgress,
    setDownloadingProgress,
    setTemporaryLink,
    setCommonThirdPartyList,
    temporaryLink,
    getStorageParams,
  } = backup;
  const { organizationName } = auth.settingsStore;

  const isDocSpace = true;

  return {
    organizationName,

    clearProgressInterval,
    clearSessionStorage,
    commonThirdPartyList,
    downloadingProgress,
    getProgress,
    getIntervalProgress,
    setDownloadingProgress,
    setTemporaryLink,
    setCommonThirdPartyList,
    temporaryLink,
    getStorageParams,
    isDocSpace,
  };
})(withTranslation(["Settings", "Common"])(observer(ManualBackup)));
