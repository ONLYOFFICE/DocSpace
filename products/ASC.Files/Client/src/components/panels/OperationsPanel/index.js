import React from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import toastr from "studio/toastr";
import { StyledAsidePanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import { ThirdPartyMoveDialog } from "../../dialogs";

import { inject, observer } from "mobx-react";

class OperationsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showProviderDialog: false,
      providerKey: "",
      destFolderId: null,
    };
  }

  onClose = () => {
    const { isCopy, setMoveToPanelVisible, setCopyPanelVisible } = this.props;
    isCopy ? setCopyPanelVisible(false) : setMoveToPanelVisible(false);
  };

  onSelect = (folder, treeNode) => {
    const { currentFolderId, selection, isCopy } = this.props;
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];

    const provider = selection.find((x) => x.providerKey);
    const isProviderFolder = selection.find((x) => !x.providerKey);

    if (currentFolderId === destFolderId) {
      return this.onClose();
    } else {
      provider &&
      !isProviderFolder &&
      !isCopy &&
      treeNode.node.props.providerKey !== provider.providerKey
        ? this.setState(
            {
              providerKey: provider.providerKey,
              showProviderDialog: true,
              destFolderId,
            },
            () => this.onClose()
          )
        : this.startOperation(isCopy, destFolderId);
    }
  };

  startMoveOperation = () => {
    this.startOperation(false);
  };

  startCopyOperation = () => {
    this.startOperation(true);
  };

  startOperation = (isCopy, folderId) => {
    const {
      itemOperationToFolder,
      t,
      selection,
      setSecondaryProgressBarData,
      currentFolderId,
    } = this.props;

    const destFolderId = folderId ? folderId : this.state.destFolderId;
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    if (currentFolderId === destFolderId) {
      return this.onClose();
    } else {
      const isProviderFolder = selection.find((x) => !x.providerKey);
      const items =
        isProviderFolder && !isCopy
          ? selection.filter((x) => !x.providerKey)
          : selection;

      for (let item of items) {
        if (item.fileExst) {
          fileIds.push(item.id);
        } else if (item.id === destFolderId) {
          toastr.error(t("MoveToFolderMessage"));
        } else {
          folderIds.push(item.id);
        }
      }
      this.onClose();
      setSecondaryProgressBarData({
        icon: isCopy ? "duplicate" : "move",
        visible: true,
        percent: 0,
        label: isCopy ? t("CopyOperation") : t("MoveToOperation"),
        alert: false,
      });
      itemOperationToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter,
        isCopy
      );
    }

    this.onClose();
  };

  render() {
    //console.log("Operations panel render");
    const {
      t,
      filter,
      isCopy,
      isRecycleBin,
      operationsFolders,
      visible,
    } = this.props;
    const { showProviderDialog, providerKey } = this.state;

    const zIndex = 310;
    const expandedKeys = this.props.expandedKeys.map((item) => item.toString());

    return (
      <>
        {showProviderDialog && (
          <ThirdPartyMoveDialog
            visible={showProviderDialog}
            onClose={this.onClose}
            startMoveOperation={this.startMoveOperation}
            startCopyOperation={this.startCopyOperation}
            provider={providerKey}
          />
        )}

        <StyledAsidePanel visible={visible}>
          <ModalDialog
            visible={visible}
            displayType="aside"
            zIndex={zIndex}
            onClose={this.onClose}
          >
            <ModalDialog.Header>
              {isRecycleBin ? t("Restore") : isCopy ? t("Copy") : t("Move")}
            </ModalDialog.Header>
            <ModalDialog.Body>
              <TreeFolders
                expandedKeys={expandedKeys}
                data={operationsFolders}
                filter={filter}
                onSelect={this.onSelect}
                needUpdate={false}
              />
            </ModalDialog.Body>
          </ModalDialog>
        </StyledAsidePanel>
      </>
    );
  }
}

const OperationsPanel = withTranslation("OperationsPanel")(
  OperationsPanelComponent
);

export default inject(
  ({
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
  }) => {
    const {
      secondaryProgressDataStore,
      itemOperationToFolder,
    } = uploadDataStore;
    const { selection, filter } = filesStore;
    const { isRecycleBinFolder, operationsFolders } = treeFoldersStore;
    const { setSecondaryProgressBarData } = secondaryProgressDataStore;

    const {
      moveToPanelVisible,
      copyPanelVisible,
      setCopyPanelVisible,
      setMoveToPanelVisible,
    } = dialogsStore;

    return {
      expandedKeys: selectedFolderStore.pathParts,
      currentFolderId: selectedFolderStore.id,
      selection,
      isRecycleBin: isRecycleBinFolder,
      filter,
      operationsFolders,
      visible: copyPanelVisible || moveToPanelVisible,

      setSecondaryProgressBarData,
      itemOperationToFolder,
      setCopyPanelVisible,
      setMoveToPanelVisible,
    };
  }
)(withRouter(observer(OperationsPanel)));
