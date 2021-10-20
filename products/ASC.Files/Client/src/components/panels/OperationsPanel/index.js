import React from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { StyledAsidePanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";

const OperationsPanelComponent = (props) => {
  const {
    t,
    tReady,
    filter,
    isCopy,
    visible,
    provider,
    selection,
    isFolderActions,
    isRecycleBin,
    setDestFolderId,
    setIsFolderActions,
    currentFolderId,
    operationsFolders,
    setCopyPanelVisible,
    setExpandedPanelKeys,
    setMoveToPanelVisible,
    checkOperationConflict,
    setThirdPartyMoveDialogVisible,
    parentFolderId,
  } = props;

  const zIndex = 310;
  const deleteAfter = false; // TODO: get from settings

  const expandedKeys = props.expandedKeys.map((item) => item.toString());

  const onClose = () => {
    if (isCopy) {
      setCopyPanelVisible(false);
      setIsFolderActions(false);
    } else {
      setMoveToPanelVisible(false);
    }
    setExpandedPanelKeys(null);
  };

  const onSelect = (folder, treeNode) => {
    const folderTitle = treeNode.node.props.title;
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];

    if (isFolderActions && destFolderId === parentFolderId) {
      return onClose();
    }

    if (currentFolderId === destFolderId) {
      return onClose();
    }

    if (isCopy) {
      startOperation(isCopy, destFolderId, folderTitle);
    } else {
      if (
        provider &&
        treeNode.node.props.providerKey !== provider.providerKey
      ) {
        setDestFolderId(destFolderId);
        setThirdPartyMoveDialogVisible(true);
      } else {
        startOperation(isCopy, destFolderId, folderTitle);
      }
    }
    onClose();
  };

  const startOperation = (isCopy, destFolderId, folderTitle) => {
    const isProviderFolder = selection.find((x) => !x.providerKey);
    const items =
      isProviderFolder && !isCopy
        ? selection.filter((x) => !x.providerKey)
        : selection;

    let fileIds = [];
    let folderIds = [];


    for (let item of items) {
      if (item.fileExst || item.contentLength) {
        fileIds.push(item.id);
      } else if (item.id === destFolderId) {
        toastr.error(t("MoveToFolderMessage"));
      } else {
        folderIds.push(item.id);
      }
    }

    if (isFolderActions) {
      fileIds = [];
      folderIds = [];

      folderIds.push(currentFolderId);
    }

    if (!folderIds.length && !fileIds.length) return;

    checkOperationConflict({
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      isCopy,
      folderTitle,
      translations: {
        copy: t("Translations:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      },
    });
  };

  //console.log("Operations panel render");
  return (
    <StyledAsidePanel visible={visible}>
      <ModalDialog
        visible={visible}
        displayType="aside"
        zIndex={zIndex}
        onClose={onClose}
        isLoading={!tReady}
      >
        <ModalDialog.Header>
          {isRecycleBin
            ? t("Translations:Restore")
            : isCopy
            ? t("Translations:Copy")
            : t("Translations:Move")}
        </ModalDialog.Header>
        <ModalDialog.Body>
          <TreeFolders
            expandedPanelKeys={expandedKeys}
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

const OperationsPanel = withTranslation(["OperationsPanel", "Translations"])(
  OperationsPanelComponent
);

export default inject(
  ({
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
    filesActionsStore,
  }) => {
    const { filter, selection, bufferSelection } = filesStore;
    const {
      isRecycleBinFolder,
      operationsFolders,
      setExpandedPanelKeys,
      expandedPanelKeys,
    } = treeFoldersStore;
    const { checkOperationConflict } = filesActionsStore;

    const {
      moveToPanelVisible,
      copyPanelVisible,
      isFolderActions,
      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setThirdPartyMoveDialogVisible,
      setIsFolderActions,
    } = dialogsStore;

    const selections = selection.length ? selection : [bufferSelection];

    const provider = selections.find((x) => x.providerKey);

    return {
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      currentFolderId: selectedFolderStore.id,
      parentFolderId: selectedFolderStore.parentId,
      isRecycleBin: isRecycleBinFolder,
      filter,
      operationsFolders,
      visible: copyPanelVisible || moveToPanelVisible,
      provider,
      selection: selections,
      isFolderActions,

      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setIsFolderActions,
      setThirdPartyMoveDialogVisible,
      checkOperationConflict,
      setExpandedPanelKeys,
    };
  }
)(withRouter(observer(OperationsPanel)));
