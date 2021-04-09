import React from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import { withTranslation } from "react-i18next";
import { getFolder } from "@appserver/common/api/files";
import toastr from "studio/toastr";
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

  onClose = () => this.props.setDeleteThirdPartyDialogVisible(false);

  onDeleteThirdParty = () => {
    const {
      setThirdPartyProviders,
      removeItem,
      providers,
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
          getFolder(folderId).then((data) => {
            const path = [folderId];
            this.updateTree(path, data.folders);
          });
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => this.onClose());
  };

  render() {
    const { visible, t, removeItem } = this.props;

    return (
      <ModalDialog visible={visible} zIndex={310} onClose={this.onClose}>
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
)(withRouter(observer(DeleteThirdPartyDialog)));
