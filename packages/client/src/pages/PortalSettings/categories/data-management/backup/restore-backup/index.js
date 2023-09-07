import React, { useState, useEffect, useCallback } from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { getSettingsThirdParty } from "@docspace/common/api/files";
import {
  getBackupStorage,
  getStorageRegions,
} from "@docspace/common/api/settings";
import RestoreBackupLoader from "@docspace/common/components/Loaders/RestoreBackupLoader";
import toastr from "@docspace/components/toast/toastr";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import { BackupStorageType } from "@docspace/common/constants";
import Checkbox from "@docspace/components/checkbox";
import Text from "@docspace/components/text";

import LocalFileModule from "./sub-components/LocalFileModule";
import ThirdPartyStoragesModule from "./sub-components/ThirdPartyStoragesModule";
import ThirdPartyResourcesModule from "./sub-components/ThirdPartyResourcesModule";
import BackupListModalDialog from "./sub-components/backup-list";
import RoomsModule from "./sub-components/RoomsModule";
import ButtonContainer from "./sub-components/ButtonComponent";
import { StyledRestoreBackup } from "../StyledBackup";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";

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
    buttonSize,
    standalone,
  } = props;

  const [radioButtonState, setRadioButtonState] = useState(LOCAL_FILE);
  const [checkboxState, setCheckboxState] = useState({
    notification: true,
    confirmation: false,
  });
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [isVisibleBackupListDialog, setIsVisibleBackupListDialog] =
    useState(false);
  const [isVisibleSelectFileDialog, setIsVisibleSelectFileDialog] =
    useState(false);

  const startRestoreBackup = useCallback(async () => {
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
  }, []);

  useEffect(() => {
    setDocumentTitle(t("RestoreBackup"));
    startRestoreBackup();
    return () => {
      clearProgressInterval();
      setRestoreResource(null);
    };
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
    setRestoreResource(id);
  };

  const radioButtonContent = (
    <>
      <RadioButtonGroup
        name="restore_backup"
        orientation="vertical"
        fontSize="13px"
        fontWeight="400"
        className="backup_radio-button"
        options={[
          { id: "local-file", value: LOCAL_FILE, label: t("LocalFile") },
          { id: "backup-room", value: BACKUP_ROOM, label: t("RoomsModule") },
          {
            id: "third-party-resource",
            value: DISK_SPACE,
            label: t("ThirdPartyResource"),
          },
          {
            id: "third-party-storage",
            value: STORAGE_SPACE,
            label: t("Common:ThirdPartyStorage"),
          },
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
      {radioButtonState === LOCAL_FILE && <LocalFileModule t={t} />}

      {radioButtonState === BACKUP_ROOM && <RoomsModule />}
      {radioButtonState === DISK_SPACE && (
        <ThirdPartyResourcesModule buttonSize={buttonSize} />
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
      {!standalone && (
        <Text
          className="restore-backup_warning-link settings_unavailable"
          noSelect
        >
          {t("RestoreBackupResetInfoWarningText")}
        </Text>
      )}
    </>
  );

  const onClickVersionListProp = isEnableRestore
    ? { onClick: onClickBackupList }
    : {};

  if (isInitialLoading) return <RestoreBackupLoader />;

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
          isVisibleDialog={isVisibleBackupListDialog}
          onModalClose={onModalClose}
          isNotify={checkboxState.notification}
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
      <ButtonContainer
        isConfirmed={checkboxState.confirmation}
        isNotification={checkboxState.notification}
        getStorageType={getStorageType}
        radioButtonState={radioButtonState}
        isCheckedThirdPartyStorage={radioButtonState === STORAGE_SPACE}
        isCheckedLocalFile={radioButtonState === LOCAL_FILE}
        t={t}
        buttonSize={buttonSize}
      />
    </StyledRestoreBackup>
  );
};

export default inject(({ auth, backup }) => {
  const { settingsStore, currentQuotaStore } = auth;
  const { isTabletView, standalone } = settingsStore;
  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;
  const {
    getProgress,
    clearProgressInterval,
    setStorageRegions,
    setThirdPartyStorage,
    setConnectedThirdPartyAccount,
    setRestoreResource,
  } = backup;

  const buttonSize = isTabletView ? "normal" : "small";

  return {
    standalone,
    isEnableRestore: isRestoreAndAutoBackupAvailable,
    setStorageRegions,
    setThirdPartyStorage,
    buttonSize,
    setConnectedThirdPartyAccount,
    clearProgressInterval,
    getProgress,
    setRestoreResource,
  };
})(withTranslation(["Settings", "Common"])(observer(RestoreBackup)));
