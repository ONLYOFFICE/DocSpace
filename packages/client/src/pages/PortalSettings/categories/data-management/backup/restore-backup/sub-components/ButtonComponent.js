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
import { request } from "@docspace/common/api/client";

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
  } = props;

  const navigate = useNavigate();

  const [isUploadingLocalFile, setIsUploadingLocalFile] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

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
      const isUploadedFile = await localFileUploading();

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

      setIsUploadingLocalFile(false);
      setIsLoading(false);
    }
  };

  const isButtonDisabled =
    isLoading ||
    isUploadingLocalFile ||
    !isMaxProgress ||
    !isConfirmed ||
    !isEnableRestore ||
    !restoreResource;
  const isLoadingButton = isUploadingLocalFile || isLoading;

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
  } = backup;

  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;
  const isMaxProgress = downloadingProgress === 100;
  return {
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
