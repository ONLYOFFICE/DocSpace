import React, { useState, useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import SelectFolderDialog from "../SelectFolderDialog";

let timerId;
const OperationsPanelComponent = (props) => {
  const {
    t,
    isCopy,
    isRestore,
    visible,
    isProviderFolder,
    selection,
    isFolderActions,
    isRecycleBin,
    setIsFolderActions,
    currentFolderId,
    setCopyPanelVisible,
    setExpandedPanelKeys,
    setMoveToPanelVisible,
    setConflictDialogData,
    itemOperationToFolder,
    checkFileConflicts,
    conflictResolveDialogVisible,
    clearActiveOperations,
    thirdPartyMoveDialogVisible,
    setRestoreAllPanelVisible,
    setMovingInProgress,
  } = props;

  const deleteAfter = false; // TODO: get from settings

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    return () => {
      clearTimeout(timerId);
      timerId = null;
    };
  });
  const onClose = () => {
    if (isCopy) {
      setCopyPanelVisible(false);
      setIsFolderActions(false);
    } else if (isRestore) {
      setRestoreAllPanelVisible(false);
    } else {
      setMoveToPanelVisible(false);
    }
    setExpandedPanelKeys(null);
  };

  const onSubmit = (selectedFolder, folderTitle, providerKey) => {
    if (!isCopy && currentFolderId === selectedFolder) {
      return;
    }

    if (isCopy) {
      startOperation(isCopy, selectedFolder, folderTitle);
    } else {
      startOperation(isCopy, selectedFolder, folderTitle);
    }
  };

  const startOperation = async (isCopy, destFolderId, folderTitle) => {
    const items =
      isProviderFolder && !isCopy
        ? selection.filter((x) => x && !x.providerKey)
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

    if (!folderIds.length && !fileIds.length) {
      toastr.error(t("Common:ErrorEmptyList"));
      return;
    }

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

    if (!timerId)
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

    checkFileConflicts(destFolderId, folderIds, fileIds)
      .then(async (conflicts) => {
        if (conflicts.length) {
          setConflictDialogData(conflicts, operationData);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClose();
          const move = !isCopy;
          if (move) setMovingInProgress(move);
          await itemOperationToFolder(operationData);
        }
      })
      .catch((e) => {
        toastr.error(e);
        setIsLoading(false);
        clearActiveOperations(fileIds, folderIds);
      })
      .finally(() => {
        clearTimeout(timerId);
        timerId = null;
      });
  };

  // console.log("Operations panel render", expandedKeys);
  const isVisible =
    conflictResolveDialogVisible || thirdPartyMoveDialogVisible
      ? false
      : visible;

  return (
    <SelectFolderDialog
      selectionFiles={selection}
      isDisableTree={isLoading}
      filteredType="exceptSortedByTags"
      isPanelVisible={isVisible}
      onSubmit={onSubmit}
      onClose={onClose}
      id={isRecycleBin ? null : currentFolderId}
      withoutImmediatelyClose
      dialogName={
        isRecycleBin
          ? t("Common:Restore")
          : isCopy
          ? t("Common:Copy")
          : t("Common:MoveTo")
      }
      buttonName={
        isRecycleBin
          ? t("Common:RestoreHere")
          : isCopy
          ? t("Translations:CopyHere")
          : t("Translations:MoveHere")
      }
      operationsType={!isCopy || isRecycleBin ? "move" : "copy"}
      isRecycleBin={isRecycleBin}
      currentFolderId={currentFolderId}
    ></SelectFolderDialog>
  );
};

const OperationsPanel = withTranslation(["Translations", "Common", "Files"])(
  OperationsPanelComponent
);

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
    { isCopy, isRestore }
  ) => {
    const {
      selection,
      filesList,
      bufferSelection,
      setMovingInProgress,
    } = filesStore;
    const { isRecycleBinFolder, setExpandedPanelKeys } = treeFoldersStore;
    const { setConflictDialogData, checkFileConflicts } = filesActionsStore;
    const { itemOperationToFolder, clearActiveOperations } = uploadDataStore;

    const {
      moveToPanelVisible,
      copyPanelVisible,
      isFolderActions,
      setCopyPanelVisible,
      setMoveToPanelVisible,
      setIsFolderActions,
      conflictResolveDialogVisible,
      thirdPartyMoveDialogVisible,
      restoreAllPanelVisible,
      setRestoreAllPanelVisible,
    } = dialogsStore;

    const selections = isRestore
      ? filesList
      : selection.length > 0 && selection[0] != null
      ? selection
      : bufferSelection != null
      ? [bufferSelection]
      : [];

    const selectionsWithoutEditing = isRestore
      ? filesList
      : isCopy
      ? selections
      : selections.filter((f) => f && !f?.isEditing);

    const isProviderFolder = selections?.find((x) => x && !x?.providerKey);

    return {
      currentFolderId: selectedFolderStore.id,
      parentFolderId: selectedFolderStore.parentId,
      isRecycleBin: isRecycleBinFolder,
      visible: copyPanelVisible || moveToPanelVisible || restoreAllPanelVisible,
      isProviderFolder,
      selection: selectionsWithoutEditing,
      isFolderActions,

      setCopyPanelVisible,
      setMoveToPanelVisible,
      setRestoreAllPanelVisible,
      setIsFolderActions,
      setConflictDialogData,
      setExpandedPanelKeys,
      itemOperationToFolder,
      checkFileConflicts,
      conflictResolveDialogVisible,
      clearActiveOperations,
      thirdPartyMoveDialogVisible,
      setMovingInProgress,
    };
  }
)(withRouter(observer(OperationsPanel)));
