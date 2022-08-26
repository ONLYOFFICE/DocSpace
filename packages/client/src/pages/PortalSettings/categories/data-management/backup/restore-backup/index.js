import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import Checkbox from "@docspace/components/checkbox";
import Text from "@docspace/components/text";
import RadioButton from "@docspace/components/radio-button";
import toastr from "@docspace/components/toast/toastr";
import { startRestore } from "@docspace/common/api/portal";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig, BackupStorageType } from "@docspace/common/constants";
import { request } from "@docspace/common/api/client";
import { StyledRestoreBackup } from "./../StyledBackup";
import BackupListModalDialog from "./sub-components/backup-list";
import Documents from "./sub-components/DocumentsModule";
import ThirdPartyResources from "./sub-components/ThirdPartyResourcesModule";
import ThirdPartyStorages from "./sub-components/ThirdPartyStoragesModule";
import LocalFile from "./sub-components/LocalFileModule";
import config from "PACKAGE_FILE";
import {
  getBackupStorage,
  getStorageRegions,
} from "@docspace/common/api/settings";
import { enableRestore } from "@docspace/common/api/portal";
import RestoreBackupLoader from "@docspace/common/components/Loaders/RestoreBackupLoader";
import FloatingButton from "@docspace/common/components/FloatingButton";

const {
  DocumentModuleType,
  ResourcesModuleType,
  StorageModuleType,
  LocalFileModuleType,
} = BackupStorageType;
class RestoreBackup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isChecked: false,
      isNotify: true,
      isVisibleDialog: false,
      isPanelVisible: false,
      isCheckedDocuments: true,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,
      isCheckedLocalFile: false,
      selectedFileId: "",
      selectedFile: "",

      isFileSelectedError: false,
      isInitialLoading: true,
      checkingRecoveryData: false,
      isEnableRestore: false,
    };

    this.switches = [
      "isCheckedLocalFile",
      "isCheckedDocuments",
      "isCheckedThirdParty",
      "isCheckedThirdPartyStorage",
    ];

    this.storageId = "";
  }

  setBasicSettings = async () => {
    const {
      getProgress,
      t,
      setThirdPartyStorage,
      setStorageRegions,
    } = this.props;

    try {
      getProgress(t);
      const [
        //commonThirdPartyList,
        backupStorage,
        isEnableRestore,
        storageRegions,
      ] = await Promise.all([
        //   getThirdPartyCommonFolderTree(),
        getBackupStorage(),
        enableRestore(),
        getStorageRegions(),
      ]);

      setThirdPartyStorage(backupStorage);
      setStorageRegions(storageRegions);
      // commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);

      this.setState({
        isInitialLoading: false,
        isEnableRestore,
      });
    } catch (error) {
      toastr.error(error);

      this.setState({
        isInitialLoading: false,
      });
    }
  };

  componentDidMount() {
    this.setBasicSettings();
  }

  componentWillUnmount() {
    const { clearProgressInterval } = this.props;
    clearProgressInterval();
  }
  onChangeCheckbox = () => {
    this.setState({
      isChecked: !this.state.isChecked,
    });
  };
  onChangeCheckboxNotify = () => {
    this.setState({
      isNotify: !this.state.isNotify,
    });
  };
  onClickBackupList = () => {
    this.setState({
      isVisibleDialog: !this.state.isVisibleDialog,
    });
  };
  onModalClose = () => {
    this.setState({
      isVisibleDialog: false,
    });
  };
  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };
  onPanelClose = () => {
    this.setState({
      isPanelVisible: false,
    });
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
  onSelectFile = (file) => {
    this.setState({
      selectedFileId: file.id,
    });
  };
  onSelectLocalFile = (data) => {
    this.setState({
      selectedFile: data,
    });
  };
  canRestore = () => {
    const {
      isCheckedDocuments,
      isCheckedLocalFile,
      selectedFileId,
      selectedFile,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
    } = this.state;

    const { isFormReady } = this.props;

    if (isCheckedDocuments || isCheckedThirdParty) {
      if (!selectedFileId) return false;
      return true;
    }
    if (isCheckedLocalFile) {
      if (!selectedFile) return false;
      return true;
    }

    if (isCheckedThirdPartyStorage) {
      return isFormReady();
    }
  };
  onRestoreClick = async () => {
    const {
      isNotify,
      isCheckedDocuments,
      isCheckedLocalFile,
      selectedFileId,
      selectedFile,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
    } = this.state;
    const { history, socketHelper, getStorageParams } = this.props;

    if (!this.canRestore()) {
      this.setState({
        isFileSelectedError: true,
      });
      return;
    }
    this.setState({
      checkingRecoveryData: true,
      isFileSelectedError: false,
    });
    let storageParams = [];
    let obj = {};
    const backupId = "";
    const storageType = isCheckedDocuments
      ? `${DocumentModuleType}`
      : isCheckedThirdParty
      ? `${ResourcesModuleType}`
      : isCheckedLocalFile
      ? `${LocalFileModuleType}`
      : `${StorageModuleType}`;

    if (isCheckedThirdPartyStorage) {
      storageParams = getStorageParams(true, null, this.storageId);
    } else {
      obj.key = "filePath";
      if (isCheckedDocuments || isCheckedThirdParty) {
        obj.value = selectedFileId;
      } else {
        obj.value = "";
      }
      storageParams.push(obj);
    }
    let checkedFile;
    try {
      if (isCheckedLocalFile) {
        checkedFile = await request({
          baseURL: combineUrl(AppServerConfig.proxyURL, config.homepage),
          method: "post",
          url: `/backupFileUpload.ashx`,
          responseType: "text",
          data: selectedFile,
        });
      }
    } catch (e) {
      toastr.error(e);
      this.setState({
        checkingRecoveryData: false,
      });
      return;
    }
    if (isCheckedLocalFile && checkedFile?.Message) {
      toastr.error(checkedFile.Message);
      this.setState({
        checkingRecoveryData: false,
      });
      return;
    }

    startRestore(backupId, storageType, storageParams, isNotify)
      .then(() => {
        socketHelper.emit({
          command: "restore-backup",
        });
      })
      .then(() => this.setState({ checkingRecoveryData: false }))
      .then(() =>
        history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            "/preparation-portal"
          )
        )
      )
      .catch((error) => {
        toastr.error(error);
        this.setState({
          checkingRecoveryData: false,
        });
      });
  };

  onSetStorageId = (storageId) => {
    this.storageId = storageId;
  };

  render() {
    const {
      t,
      history,
      downloadingProgress,
      organizationName,
      buttonSize,
      theme,
    } = this.props;
    const {
      isChecked,
      isInitialLoading,
      isNotify,
      isVisibleDialog,
      isPanelVisible,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isCheckedLocalFile,
      checkingRecoveryData,
      isFileSelectedError,
      isEnableRestore,
    } = this.state;

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };

    const onClickVersionListProp = isEnableRestore
      ? { onClick: this.onClickBackupList }
      : {};

    // const isDisabledThirdParty = commonThirdPartyList?.length === 0;

    const isMaxProgress = downloadingProgress === 100;

    return isInitialLoading ? (
      <RestoreBackupLoader />
    ) : (
      <StyledRestoreBackup theme={theme} isEnableRestore={isEnableRestore}>
        <div className="restore-description">
          <Text className="restore-description">
            {t("RestoreBackupDescription")}
          </Text>
        </div>
        <RadioButton
          label={organizationName}
          name={"isCheckedDocuments"}
          key={1}
          isChecked={isCheckedDocuments}
          isDisabled={!isEnableRestore}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("ThirdPartyResource")}
          name={"isCheckedThirdParty"}
          key={2}
          isChecked={isCheckedThirdParty}
          isDisabled={!isEnableRestore}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("ThirdPartyStorage")}
          name={"isCheckedThirdPartyStorage"}
          key={3}
          isChecked={isCheckedThirdPartyStorage}
          isDisabled={!isEnableRestore}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("LocalFile")}
          name={"isCheckedLocalFile"}
          key={4}
          isChecked={isCheckedLocalFile}
          isDisabled={!isEnableRestore}
          {...commonRadioButtonProps}
        />

        <div className="restore-backup_modules">
          {isCheckedDocuments && (
            <Documents
              isDisabled={!isEnableRestore}
              t={t}
              isPanelVisible={isPanelVisible}
              onClose={this.onPanelClose}
              onClickInput={this.onClickInput}
              onSelectFile={this.onSelectFile}
              isError={isFileSelectedError}
            />
          )}
          {isCheckedThirdParty && (
            <ThirdPartyResources
              t={t}
              isPanelVisible={isPanelVisible}
              onClose={this.onPanelClose}
              onClickInput={this.onClickInput}
              onSelectFile={this.onSelectFile}
              isError={isFileSelectedError}
            />
          )}
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorages
              onResetFormSettings={this.onResetFormSettings}
              onSetStorageId={this.onSetStorageId}
            />
          )}
          {isCheckedLocalFile && (
            <LocalFile
              hasError={isFileSelectedError}
              onSelectLocalFile={this.onSelectLocalFile}
            />
          )}
        </div>

        <Text
          className="restore-backup_list"
          {...onClickVersionListProp}
          noSelect
        >
          {t("BackupList")}
        </Text>

        {isVisibleDialog && (
          <BackupListModalDialog
            isVisibleDialog={isVisibleDialog}
            onModalClose={this.onModalClose}
            isNotify={isNotify}
            isCopyingToLocal={!isMaxProgress}
            history={history}
          />
        )}
        <Checkbox
          truncate
          className="restore-backup-checkbox_notification"
          onChange={this.onChangeCheckboxNotify}
          isChecked={isNotify}
          label={t("SendNotificationAboutRestoring")}
          isDisabled={!isEnableRestore}
        />

        <Text className="restore-backup_warning" noSelect>
          {t("Common:Warning")}
          {"!"}
        </Text>
        <Text className="restore-backup_warning-description" noSelect>
          {t("RestoreBackupWarningText")}
        </Text>
        <Text className="restore-backup_warning-link" noSelect>
          {t("RestoreBackupResetInfoWarningText")}
        </Text>

        <Checkbox
          truncate
          className="restore-backup-checkbox"
          onChange={this.onChangeCheckbox}
          isChecked={isChecked}
          label={t("UserAgreement")}
          isDisabled={!isEnableRestore}
        />

        <Button
          className="restore-backup_button"
          label={t("Common:Restore")}
          onClick={this.onRestoreClick}
          primary
          isDisabled={
            checkingRecoveryData ||
            !isMaxProgress ||
            !isChecked ||
            !isEnableRestore
          }
          isLoading={checkingRecoveryData}
          size={buttonSize}
          tabIndex={10}
        />

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledRestoreBackup>
    );
  }
}

export default inject(({ auth, backup }) => {
  const { settingsStore } = auth;
  const { socketHelper, theme, isTabletView, organizationName } = settingsStore;
  const {
    downloadingProgress,
    getProgress,
    clearProgressInterval,
    setStorageRegions,
    setThirdPartyStorage,
    isFormReady,
    getStorageParams,
  } = backup;

  const buttonSize = isTabletView ? "normal" : "small";

  return {
    setStorageRegions,
    setThirdPartyStorage,
    buttonSize,

    theme,
    clearProgressInterval,
    downloadingProgress,
    socketHelper,
    isFormReady,

    getProgress,
    getStorageParams,
    organizationName,
  };
})(withTranslation(["Settings", "Common"])(observer(RestoreBackup)));
