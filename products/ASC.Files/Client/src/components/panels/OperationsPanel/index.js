import React from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { StyledAsidePanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import { inject, observer } from "mobx-react";

const OperationsPanelComponent = (props) => {
  const {
    t,
    filter,
    isCopy,
    visible,
    provider,
    selection,
    isRecycleBin,
    setDestFolderId,
    currentFolderId,
    operationsFolders,
    setCopyPanelVisible,
    itemOperationToFolder,
    setMoveToPanelVisible,
    setThirdPartyMoveDialogVisible,
  } = props;

  const zIndex = 310;
  const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2 TODO: get from settings
  const deleteAfter = true; // TODO: get from settings

  const expandedKeys = props.expandedKeys.map((item) => item.toString());

  const onClose = () => {
    isCopy ? setCopyPanelVisible(false) : setMoveToPanelVisible(false);
  };

  const onSelect = (folder, treeNode) => {
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];

    if (currentFolderId === destFolderId) {
      return onClose();
    }

    if (isCopy) {
      startOperation(isCopy, destFolderId);
    } else {
      if (
        provider &&
        treeNode.node.props.providerKey !== provider.providerKey
      ) {
        setDestFolderId(destFolderId);
        setThirdPartyMoveDialogVisible(true);
      } else {
        startOperation(isCopy, destFolderId);
      }
    }
    onClose();
  };

  const startOperation = (isCopy, destFolderId) => {
    const isProviderFolder = selection.find((x) => !x.providerKey);
    const items =
      isProviderFolder && !isCopy
        ? selection.filter((x) => !x.providerKey)
        : selection;

    const fileIds = [];
    const folderIds = [];

    for (let item of items) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else if (item.id === destFolderId) {
        toastr.error(t("MoveToFolderMessage"));
      } else {
        folderIds.push(item.id);
      }
    }

    itemOperationToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter,
      isCopy,
      { copy: t("CopyOperation"), move: t("MoveToOperation") }
    );
  };

  //console.log("Operations panel render");
  return (
    <StyledAsidePanel visible={visible}>
      <ModalDialog
        visible={visible}
        displayType="aside"
        zIndex={zIndex}
        onClose={onClose}
      >
        <ModalDialog.Header>
          {isRecycleBin ? t("Restore") : isCopy ? t("Copy") : t("Move")}
        </ModalDialog.Header>
        <ModalDialog.Body>
          <TreeFolders
            expandedKeys={expandedKeys}
            data={operationsFolders}
            filter={filter}
            onSelect={onSelect}
            needUpdate={false}
          />
        </ModalDialog.Body>
      </ModalDialog>
    </StyledAsidePanel>
  );
};

const OperationsPanel = withTranslation("OperationsPanel")(
  OperationsPanelComponent
);

export default inject(
  ({
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
    uploadDataStore,
  }) => {
    const { filter, selection } = filesStore;
    const { isRecycleBinFolder, operationsFolders } = treeFoldersStore;
    const { itemOperationToFolder } = uploadDataStore;

    const {
      moveToPanelVisible,
      copyPanelVisible,
      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setThirdPartyMoveDialogVisible,
    } = dialogsStore;

    const provider = selection.find((x) => x.providerKey);

    return {
      expandedKeys: selectedFolderStore.pathParts,
      currentFolderId: selectedFolderStore.id,
      isRecycleBin: isRecycleBinFolder,
      filter,
      operationsFolders,
      visible: copyPanelVisible || moveToPanelVisible,
      provider,
      selection,

      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setThirdPartyMoveDialogVisible,
      itemOperationToFolder,
    };
  }
)(withRouter(observer(OperationsPanel)));
