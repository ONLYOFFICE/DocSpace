import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { useNavigate } from "react-router-dom";
import Button from "@docspace/components/button";
import FloatingButton from "@docspace/components/floating-button";
import { TenantStatus } from "@docspace/common/constants";
import { startRestore } from "@docspace/common/api/portal";
import { combineUrl } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";

const ButtonContainer = (props) => {
  const {
    downloadingProgress,
    isMaxProgress,
    isConfirmed,
    getStorageType,
    isNotification,
    restoreResource,
    isCheckedThirdPartyStorage,
    isCheckedLocalFile,

    isEnableRestore,
    t,
    buttonSize,
    socketHelper,
    setTenantStatus,
    isFormReady,
    getStorageParams,
    uploadLocalFile,
  } = props;

  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);

  const onRestoreClick = async () => {
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
      storageParams = getStorageParams(true, null, restoreResource);
    } else {
      tempObj.key = "filePath";
      tempObj.value = isCheckedLocalFile ? "" : restoreResource;

      storageParams.push(tempObj);
    }

    if (isCheckedLocalFile) {
      const uploadedFile = await uploadLocalFile();

      if (!uploadedFile) {
        toastr.error(t("BackupCreatedError"));
        setIsLoading(false);
        return;
      }

      if (!uploadedFile.data.EndUpload) {
        toastr.error(uploadedFile.data.Message ?? t("BackupCreatedError"));
        setIsLoading(false);
        return;
      }
    }

    try {
      await startRestore(backupId, storageType, storageParams, isNotification);
      setTenantStatus(TenantStatus.PortalRestore);

      socketHelper.emit({
        command: "restore-backup",
      });

      navigate(
        combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          "/preparation-portal"
        )
      );
    } catch (e) {
      toastr.error(e);

      setIsLoading(false);
    }
  };

  const isButtonDisabled =
    isLoading ||
    !isMaxProgress ||
    !isConfirmed ||
    !isEnableRestore ||
    !restoreResource;
  const isLoadingButton = isLoading;

  return (
    <>
      <Button
        className="restore-backup_button"
        label={t("Common:Restore")}
        onClick={onRestoreClick}
        primary
        isDisabled={isButtonDisabled}
        isLoading={isLoadingButton}
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
};

export default inject(({ auth, backup }) => {
  const { settingsStore, currentQuotaStore } = auth;
  const { socketHelper, setTenantStatus } = settingsStore;
  const {
    downloadingProgress,
    isFormReady,
    getStorageParams,
    restoreResource,
    uploadLocalFile,
  } = backup;

  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;
  const isMaxProgress = downloadingProgress === 100;
  return {
    uploadLocalFile,
    isMaxProgress,
    setTenantStatus,
    isEnableRestore: isRestoreAndAutoBackupAvailable,
    downloadingProgress,
    socketHelper,
    isFormReady,
    getStorageParams,
    restoreResource,
  };
})(observer(ButtonContainer));
