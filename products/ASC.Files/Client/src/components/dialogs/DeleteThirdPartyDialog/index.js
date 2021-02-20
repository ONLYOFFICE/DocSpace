import React from "react";
import { withRouter } from "react-router";
import { ModalDialog, Button } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { toastr, api } from "asc-web-common";
import { loopTreeFolders } from "../../../helpers/files-helpers";
import { inject, observer } from "mobx-react";

class DeleteThirdPartyDialogComponent extends React.Component {
  updateTree = (path, folders) => {
    const { t, treeFolders, removeItem, setTreeFolders } = this.props;

    const newTreeFolders = treeFolders;
    loopTreeFolders(path, newTreeFolders, folders, null);
    setTreeFolders(newTreeFolders);
    toastr.success(t("SuccessDeleteThirdParty", { service: removeItem.title }));
  };

  onDeleteThirdParty = () => {
    const {
      setThirdPartyProviders,
      removeItem,
      providers,
      onClose,
      fetchFiles,
      currentFolderId,
      commonId,
      myId,
      deleteThirdParty,
    } = this.props;

    const providerItem = providers.find((x) => x.provider_id === removeItem.id);
    const newProviders = providers.filter(
      (x) => x.provider_id !== removeItem.id
    );

    deleteThirdParty(+removeItem.id)
      .then(() => {
        setThirdPartyProviders(newProviders);
        if (currentFolderId) {
          fetchFiles(currentFolderId).then((data) => {
            const path = data.selectedFolder.pathParts;
            const folders = data.selectedFolder.folders;
            this.updateTree(path, folders);
          });
        } else {
          const folderId = providerItem.corporate ? commonId : myId;
          api.files.getFolder(folderId).then((data) => {
            const path = [folderId];
            this.updateTree(path, data.folders);
          });
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => onClose());
  };

  render() {
    const { onClose, visible, t, removeItem } = this.props;

    return (
      <ModalDialog visible={visible} zIndex={310} onClose={onClose}>
        <ModalDialog.Header>{t("DeleteThirdParty")}</ModalDialog.Header>
        <ModalDialog.Body>
          {t("DeleteThirdPartyAlert", { service: removeItem.title })}
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            label={t("OKButton")}
            size="big"
            primary
            onClick={this.onDeleteThirdParty}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

const DeleteThirdPartyDialog = withTranslation("DeleteThirdPartyDialog")(
  DeleteThirdPartyDialogComponent
);

export default inject(
  ({ filesStore, thirdParty, treeFoldersStore, selectedFolderStore }) => {
    const { providers, setThirdPartyProviders, deleteThirdParty } = thirdParty;
    const { fetchFiles } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      myFolderId,
      commonFolderId,
    } = treeFoldersStore;

    return {
      currentFolderId: selectedFolderStore.id,
      treeFolders,
      myId: myFolderId,
      commonId: commonFolderId,
      providers,

      fetchFiles,
      setTreeFolders,
      setThirdPartyProviders,
      deleteThirdParty,
    };
  }
)(withRouter(observer(DeleteThirdPartyDialog)));
