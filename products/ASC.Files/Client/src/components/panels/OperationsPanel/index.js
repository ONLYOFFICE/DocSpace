import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import TreeFolders from "../../FolderTreeBody/TreeFolders";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";
import Button from "@appserver/components/button";
import styled from "styled-components";

const StyledModalDialog = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    width: 90%;
  }
`;

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
    setConflictDialogData,
    itemOperationToFolder,
    checkFileConflicts,
    setThirdPartyMoveDialogVisible,
    parentFolderId,
  } = props;

  const zIndex = 310;
  const deleteAfter = false; // TODO: get from settings

  const expandedKeys = props.expandedKeys.map((item) => item.toString());

  const [isLoading, setIsLoading] = useState(false);
  const [selectedFolder, setSelectedFolder] = useState(null);
  const [folderTitle, setFolderTitle] = useState(null);
  const [providerKey, setProviderKey] = useState(null);

  const onClose = () => {
    if (isCopy) {
      setCopyPanelVisible(false);
      setIsFolderActions(false);
    } else {
      setMoveToPanelVisible(false);
    }
    setExpandedPanelKeys(null);
  };

  const onSubmit = () => {
    if (currentFolderId === selectedFolder) {
      return;
    }

    if (isCopy) {
      startOperation(isCopy, selectedFolder, folderTitle);
    } else {
      if (provider && providerKey !== provider.providerKey) {
        setDestFolderId(selectedFolder);
        setThirdPartyMoveDialogVisible(true);
      } else {
        startOperation(isCopy, selectedFolder, folderTitle);
      }
    }
  };

  const onSelect = (folder, treeNode) => {
    setProviderKey(treeNode.node.props.providerKey);
    setFolderTitle(treeNode.node.props.title);
    setSelectedFolder(isNaN(+folder[0]) ? folder[0] : +folder[0]);
  };

  const startOperation = async (isCopy, destFolderId, folderTitle) => {
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

    const operationData = {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      isCopy,
      folderTitle,
      translations: {
        copy: t("Common:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      },
    };

    setIsLoading(true);
    checkFileConflicts(destFolderId, folderIds, fileIds).then(
      async (conflicts) => {
        if (conflicts.length) {
          setConflictDialogData(conflicts, operationData);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClose();
          await itemOperationToFolder(operationData);
        }
      }
    );
  };

  //console.log("Operations panel render");
  return (
    <StyledModalDialog
      visible={visible}
      displayType="aside"
      zIndex={zIndex}
      onClose={onClose}
      isLoading={!tReady}
      className="operations-panel-dialog"
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
          isPanel={true}
          expandedPanelKeys={expandedKeys}
          data={operationsFolders}
          filter={filter}
          onSelect={onSelect}
          needUpdate={false}
          disabled={isLoading || isLoading}
          selectedKeys={[selectedFolder + ""]}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          scale
          key="OkButton"
          label={
            isRecycleBin
              ? t("Translations:Restore")
              : isCopy
              ? t("Translations:Copy")
              : t("Translations:Move")
          }
          size="small"
          primary
          onClick={onSubmit}
          isLoading={isLoading}
          isDisabled={!selectedFolder || isLoading}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

const OperationsPanel = withTranslation([
  "OperationsPanel",
  "Translations",
  "Common",
])(OperationsPanelComponent);

export default inject(
  (
    {
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      dialogsStore,
      filesActionsStore,
      uploadDataStore,
    },
    { isCopy }
  ) => {
    const { filter, selection, bufferSelection } = filesStore;
    const {
      isRecycleBinFolder,
      operationsFolders,
      setExpandedPanelKeys,
      expandedPanelKeys,
    } = treeFoldersStore;
    const { setConflictDialogData, checkFileConflicts } = filesActionsStore;
    const { itemOperationToFolder } = uploadDataStore;

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
    const selectionsWithoutEditing = isCopy
      ? selections
      : selections.filter((f) => !f.isEditing);

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
      selection: selectionsWithoutEditing,
      isFolderActions,

      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setIsFolderActions,
      setThirdPartyMoveDialogVisible,
      setConflictDialogData,
      setExpandedPanelKeys,
      itemOperationToFolder,
      checkFileConflicts,
    };
  }
)(withRouter(observer(OperationsPanel)));
