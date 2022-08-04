import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";
import { getFolder } from "@docspace/common/api/files";
import toastr from "client/toastr";
import { loopTreeFolders } from "../../../helpers/files-helpers";
import { inject, observer } from "mobx-react";

const DeleteThirdPartyDialog = (props) => {
  const {
    t,
    myId,
    tReady,
    visible,
    commonId,
    providers,
    removeItem,
    fetchFiles,
    treeFolders,
    setTreeFolders,
    currentFolderId,
    deleteThirdParty,
    setThirdPartyProviders,
    setDeleteThirdPartyDialogVisible,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const updateTree = (path, folders) => {
    const newTreeFolders = treeFolders;
    loopTreeFolders(path, newTreeFolders, folders, null);
    setTreeFolders(newTreeFolders);
    toastr.success(t("SuccessDeleteThirdParty", { service: removeItem.title }));
  };

  const onClose = () => setDeleteThirdPartyDialogVisible(false);

  const onDeleteThirdParty = () => {
    const providerItem = providers.find((x) => x.provider_id === removeItem.id);
    const newProviders = providers.filter(
      (x) => x.provider_id !== removeItem.id
    );

    setIsLoading(true);
    deleteThirdParty(+removeItem.id)
      .then(() => {
        setThirdPartyProviders(newProviders);
        if (currentFolderId) fetchFiles(currentFolderId, null, true, true);
        else {
          const folderId = providerItem.corporate ? commonId : myId;
          getFolder(folderId).then((data) => {
            const path = [folderId];
            updateTree(path, data.folders);
          });
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
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const {
      providers,
      setThirdPartyProviders,
      deleteThirdParty,
    } = settingsStore.thirdPartyStore;
    const { fetchFiles } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      myFolderId,
      commonFolderId,
    } = treeFoldersStore;

    const {
      deleteThirdPartyDialogVisible: visible,
      setDeleteThirdPartyDialogVisible,
      removeItem,
    } = dialogsStore;

    return {
      currentFolderId: selectedFolderStore.id,
      treeFolders,
      myId: myFolderId,
      commonId: commonFolderId,
      providers,
      visible,
      removeItem,

      fetchFiles,
      setTreeFolders,
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
