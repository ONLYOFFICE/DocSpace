import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";

const DeleteThirdPartyDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    providers,
    removeItem,
    fetchFiles,
    currentFolderId,
    deleteThirdParty,
    setThirdPartyProviders,
    setDeleteThirdPartyDialogVisible,
    isConnectionViaBackupModule,
    updateInfo,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const onClose = () => setDeleteThirdPartyDialogVisible(false);

  const onDeleteThirdParty = () => {
    if (isConnectionViaBackupModule) {
      deleteThirdParty(+removeItem.provider_id)
        .catch((err) => toastr.error(err))
        .finally(() => {
          updateInfo && updateInfo();
          setIsLoading(false);
          onClose();
        });

      return;
    }

    const newProviders = providers.filter(
      (x) => x.provider_id !== removeItem.id
    );

    setIsLoading(true);
    deleteThirdParty(+removeItem.id)
      .then(() => {
        setThirdPartyProviders(newProviders);
        if (currentFolderId) fetchFiles(currentFolderId, null, true, true);
        else {
          toastr.success(
            t("SuccessDeleteThirdParty", { service: removeItem.title })
          );
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        onClose();
        setIsLoading(false);
      });
  };

  return (
    <ModalDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={310}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("DisconnectCloudTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        {t("DisconnectCloudMessage", {
          service: removeItem.title,
          account: removeItem.providerKey,
        })}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          isLoading={isLoading}
          label={t("Common:OKButton")}
          size="normal"
          primary
          scale
          onClick={onDeleteThirdParty}
        />
        <Button
          isLoading={isLoading}
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(
  ({
    filesStore,
    settingsStore,
    dialogsStore,
    selectedFolderStore,
    backup,
  }) => {
    const {
      providers,
      setThirdPartyProviders,
      deleteThirdParty,
    } = settingsStore.thirdPartyStore;
    const { fetchFiles } = filesStore;
    const { selectedThirdPartyAccount: backupConnectionItem } = backup;
    const {
      deleteThirdPartyDialogVisible: visible,
      setDeleteThirdPartyDialogVisible,
      removeItem: storeItem,
    } = dialogsStore;

    const removeItem = backupConnectionItem ?? storeItem;

    return {
      currentFolderId: selectedFolderStore.id,
      providers,
      visible,
      removeItem,

      fetchFiles,
      setThirdPartyProviders,
      deleteThirdParty,
      setDeleteThirdPartyDialogVisible,
    };
  }
)(
  withRouter(
    withTranslation(["DeleteThirdPartyDialog", "Common", "Translations"])(
      observer(DeleteThirdPartyDialog)
    )
  )
);
