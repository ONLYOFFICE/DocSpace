import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import { getSettingsThirdParty } from "@docspace/common/api/files";
import {
  getBackupStorage,
  getStorageRegions,
} from "@docspace/common/api/settings";
import RestoreBackupLoader from "@docspace/common/components/Loaders/RestoreBackupLoader";
import toastr from "@docspace/components/toast/toastr";
import { RadioButtonGroup } from "@docspace/components";
import { StyledRestoreBackup } from "../StyledBackup";
import Text from "@docspace/components/text";
import LocalFileModule from "./sub-components/LocalFileModule";
import ThirdPartyStoragesModule from "./sub-components/ThirdPartyStoragesModule";
import ThirdPartyResourcesModule from "./sub-components/ThirdPartyResourcesModule";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import FloatingButton from "@docspace/common/components/FloatingButton";
import { BackupStorageType, TenantStatus } from "@docspace/common/constants";
import { startRestore } from "@docspace/common/api/portal";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import BackupListModalDialog from "./sub-components/backup-list";
import RoomsModule from "./sub-components/RoomsModule";

const LOCAL_FILE = "localFile",
  BACKUP_ROOM = "backupRoom",
  DISK_SPACE = "thirdPartyDiskSpace",
  STORAGE_SPACE = "thirdPartyStorageSpace";

const NOTIFICATION = "notification",
  CONFIRMATION = "confirmation";

const {
  DocumentModuleType,
  ResourcesModuleType,
  StorageModuleType,
  LocalFileModuleType,
} = BackupStorageType;

let storageId = null;
const RestoreBackup = (props) => {
  const {
    getProgress,
    t,
    setThirdPartyStorage,
    setStorageRegions,
    setConnectedThirdPartyAccount,
    clearProgressInterval,
    isEnableRestore,
    setRestoreResource,
    downloadingProgress,
    isMaxProgress,
    buttonSize,
    restoreResource,
    history,
    socketHelper,
    getStorageParams,
    setTenantStatus,
    standalone,
  } = props;

  const [radioButtonState, setRadioButtonState] = useState(LOCAL_FILE);
  const [checkboxState, setCheckboxState] = useState({
    notification: true,
    confirmation: false,
  });
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [isUploadingLocalFile, setIsUploadingLocalFile] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isVisibleBackupListDialog, setIsVisibleBackupListDialog] = useState(
    false
  );
  const [isVisibleSelectFileDialog, setIsVisibleSelectFileDialog] = useState(
    false
  );

  useEffect(async () => {
    try {
      getProgress(t);

      const [account, backupStorage, storageRegions] = await Promise.all([
        getSettingsThirdParty(),
        getBackupStorage(),
        getStorageRegions(),
      ]);

      setConnectedThirdPartyAccount(account);
      setThirdPartyStorage(backupStorage);
      setStorageRegions(storageRegions);

      setIsInitialLoading(false);
    } catch (error) {
      toastr.error(error);
    }

    return () => clearProgressInterval();
  }, []);

  const onChangeRadioButton = (e) => {
    const value = e.target.value;
    if (value === radioButtonState) return;

    setRestoreResource(null);
    setRadioButtonState(value);
  };

  const onChangeCheckbox = (e) => {
    const name = e.target.name;
    const checked = e.target.checked;

    setCheckboxState({ ...checkboxState, [name]: checked });
  };

  const localFileUploading = async () => {
    try {
      const checkedFile = await request({
        baseURL: combineUrl(window.DocSpaceConfig?.proxy?.url, config.homepage),
        method: "post",
        url: `/backupFileUpload.ashx`,
        responseType: "text",
        data: restoreResource,
      });

      return checkedFile;
    } catch (e) {
      toastr.error(e);
      setIsUploadingLocalFile(false);
      return null;
    }
  };

  const getStorageType = () => {
    switch (radioButtonState) {
      case LOCAL_FILE:
        return LocalFileModuleType;
      case BACKUP_ROOM:
        return DocumentModuleType;
      case DISK_SPACE:
        return ResourcesModuleType;
      case STORAGE_SPACE:
        return StorageModuleType;
    }
  };
  const onRestoreClick = async () => {
    const isCheckedThirdPartyStorage = radioButtonState === STORAGE_SPACE;

    if (isCheckedThirdPartyStorage) {
      const requiredFieldsFilled = isFormReady();
      if (!requiredFieldsFilled) return;
    }
    setIsLoading(true);

    let storageParams = [],
      tempObj = {};

    const backupId = "";
    const storageType = getStorageType().toString();

    if (isCheckedThirdPartyStorage) {
      storageParams = getStorageParams(true, null, storageId);
    } else {
      tempObj.key = "filePath";
      tempObj.value =
        radioButtonState === LOCAL_FILE && !standalone ? "" : restoreResource;

      storageParams.push(tempObj);
    }

    if (!standalone && radioButtonState === LOCAL_FILE) {
      const isUploadedFile = localFileUploading();

      if (!isUploadedFile) {
        setIsLoading(false);
        return;
      }

      if (isUploadedFile?.Message) {
        toastr.error(isUploadedFile.Message);
        setIsUploadingLocalFile(false);
        setIsLoading(false);
        return;
      }
    }

    try {
      await startRestore(
        backupId,
        storageType,
        storageParams,
        checkboxState.notification
      );
      setTenantStatus(TenantStatus.PortalRestore);

      socketHelper.emit({
        command: "restore-backup",
      });

      setIsUploadingLocalFile(false);

      history.push(
        combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          "/preparation-portal"
        )
      );
    } catch (e) {
      toastr.error(error);

      setIsUploadingLocalFile(false);
    }

    setIsLoading(false);
  };
  const onClickBackupList = () => {
    setIsVisibleBackupListDialog(true);
  };

  const onClickInput = () => {
    setIsVisibleSelectFileDialog(true);
  };
  const onModalClose = () => {
    setIsVisibleBackupListDialog(false);
    setIsVisibleSelectFileDialog(false);
  };

  const onSetStorageId = (id) => {
    storageId = id;
  };

  const radioButtonContent = (
    <>
      <RadioButtonGroup
        orientation="vertical"
        fontSize="13px"
        fontWeight="400"
        className="backup_radio-button"
        options={[
          { value: LOCAL_FILE, label: t("LocalFile") },
          { value: BACKUP_ROOM, label: t("RoomsModule") },
          { value: DISK_SPACE, label: t("ThirdPartyResource") },
          { value: STORAGE_SPACE, label: t("ThirdPartyStorage") },
        ]}
        onClick={onChangeRadioButton}
        selected={radioButtonState}
        spacing="16px"
        isDisabled={!isEnableRestore}
      />
    </>
  );

  const backupModules = (
    <div className="restore-backup_modules">
      {radioButtonState === LOCAL_FILE && <LocalFileModule />}

      {radioButtonState === BACKUP_ROOM && (
        <RoomsModule
          isDisabled={!isEnableRestore}
          t={t}
          isPanelVisible={isVisibleSelectFileDialog}
          onClose={onModalClose}
          onClickInput={onClickInput}
          onSelectFile={(file) => setRestoreResource(file.id)}
        />
      )}
      {radioButtonState === DISK_SPACE && (
        <ThirdPartyResourcesModule
          t={t}
          isPanelVisible={isVisibleSelectFileDialog}
          onClose={onModalClose}
          onClickInput={onClickInput}
          onSelectFile={(file) => setRestoreResource(file.id)}
          buttonSize={buttonSize}
        />
      )}
      {radioButtonState === STORAGE_SPACE && (
        <ThirdPartyStoragesModule onSetStorageId={onSetStorageId} />
      )}
    </div>
  );

  const warningContent = (
    <>
      <Text className="restore-backup_warning settings_unavailable" noSelect>
        {t("Common:Warning")}
        {"!"}
      </Text>
      <Text
        className="restore-backup_warning-description settings_unavailable"
        noSelect
      >
        {t("RestoreBackupWarningText")}
      </Text>
      <Text
        className="restore-backup_warning-link settings_unavailable"
        noSelect
      >
        {t("RestoreBackupResetInfoWarningText")}
      </Text>
    </>
  );

  const buttonContent = (
    <>
      <Button
        className="restore-backup_button"
        label={t("Common:Restore")}
        onClick={onRestoreClick}
        primary
        isDisabled={
          isLoading ||
          isUploadingLocalFile ||
          !isMaxProgress ||
          !checkboxState.confirmation ||
          !isEnableRestore ||
          !restoreResource
        }
        isLoading={isUploadingLocalFile || isLoading}
        size={buttonSize}
        tabIndex={10}
      />

      {downloadingProgress > 0 && !isMaxProgress && (
        <FloatingButton
          className="layout-progress-bar"
          icon="file"
          alert={false}
          percent={downloadingProgress}
        />
      )}
    </>
  );

  const onClickVersionListProp = isEnableRestore
    ? { onClick: onClickBackupList }
    : {};

  if (isInitialLoading) return <RestoreBackupLoader />;
  console.log("index render");
  return (
    <StyledRestoreBackup isEnableRestore={isEnableRestore}>
      <div className="restore-description">
        <Text className="restore-description settings_unavailable">
          {t("RestoreBackupDescription")}
        </Text>
      </div>
      {radioButtonContent}
      {backupModules}

      <Text
        className="restore-backup_list settings_unavailable"
        {...onClickVersionListProp}
        noSelect
      >
        {t("BackupList")}
      </Text>

      {isVisibleBackupListDialog && (
        <BackupListModalDialog
          isVisibleBackupListDialog={isVisibleBackupListDialog}
          onModalClose={onModalClose}
          isNotify={checkboxState.notification}
          isCopyingToLocal={!isMaxProgress}
          history={history}
        />
      )}
      <Checkbox
        truncate
        name={NOTIFICATION}
        className="restore-backup-checkbox_notification"
        onChange={onChangeCheckbox}
        isChecked={checkboxState.notification}
        label={t("SendNotificationAboutRestoring")}
        isDisabled={!isEnableRestore}
      />
      {warningContent}
      <Checkbox
        truncate
        name={CONFIRMATION}
        className="restore-backup-checkbox"
        onChange={onChangeCheckbox}
        isChecked={checkboxState.confirmation}
        label={t("UserAgreement")}
        isDisabled={!isEnableRestore}
      />
      {buttonContent}
    </StyledRestoreBackup>
  );
};

export default inject(({ auth, backup }) => {
  const { settingsStore, currentQuotaStore } = auth;
  const {
    socketHelper,

    isTabletView,
    setTenantStatus,
    standalone,
  } = settingsStore;
  const {
    downloadingProgress,
    getProgress,
    clearProgressInterval,
    setStorageRegions,
    setThirdPartyStorage,
    isFormReady,
    getStorageParams,
    setConnectedThirdPartyAccount,
    setRestoreResource,
    restoreResource,
  } = backup;

  const buttonSize = isTabletView ? "normal" : "small";
  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;
  const isMaxProgress = downloadingProgress === 100;
  return {
    isMaxProgress,
    standalone,
    setTenantStatus,
    isEnableRestore: isRestoreAndAutoBackupAvailable,
    setStorageRegions,
    setThirdPartyStorage,
    buttonSize,
    setConnectedThirdPartyAccount,

    clearProgressInterval,
    downloadingProgress,
    socketHelper,
    isFormReady,

    getProgress,
    getStorageParams,
    setRestoreResource,
    restoreResource,
  };
})(withTranslation(["Settings", "Common"])(observer(RestoreBackup)));
