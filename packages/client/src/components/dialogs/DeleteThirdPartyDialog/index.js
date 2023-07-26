import React, { useState } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import { useNavigate, useLocation } from "react-router-dom";
import FilesFilter from "@docspace/common/api/files/filter";

const DeleteThirdPartyDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    providers,
    removeItem,

    currentFolderId,

    deleteThirdParty,
    setThirdPartyProviders,
    setDeleteThirdPartyDialogVisible,
    isConnectionViaBackupModule,
    updateInfo,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

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
        if (currentFolderId) {
          const filter = FilesFilter.getDefault();

          filter.folder = currentFolderId;

          navigate(`${location.pathname}?${filter.toUrlParams()}`);
        } else {
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
    const { providers, setThirdPartyProviders, deleteThirdParty } =
      settingsStore.thirdPartyStore;
    const { setIsLoading } = filesStore;
    const { selectedThirdPartyAccount: backupConnectionItem } = backup;
    const {
      deleteThirdPartyDialogVisible: visible,
      setDeleteThirdPartyDialogVisible,
      removeItem: storeItem,
    } = dialogsStore;

    const removeItem = backupConnectionItem ?? storeItem;

    const { id } = selectedFolderStore;

    return {
      currentFolderId: id,

      setIsLoadingStore: setIsLoading,

      providers,
      visible,
      removeItem,

      setThirdPartyProviders,
      deleteThirdParty,
      setDeleteThirdPartyDialogVisible,
    };
  }
)(
  withTranslation(["DeleteThirdPartyDialog", "Common", "Translations"])(
    observer(DeleteThirdPartyDialog)
  )
);
