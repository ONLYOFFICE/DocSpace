import React, { useState, useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";
import SelectFolderDialog from "../SelectFolderDialog";

let operationData, fileWithConflicts, timerId;
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
    conflictResolveDialogVisible,
    clearActiveOperations,
  } = props;

  const deleteAfter = false; // TODO: get from settings

  const [isLoading, setIsLoading] = useState(false);
  const [intermediateHidden, setIntermediateHidden] = useState(false);

  useEffect(() => {
    if (conflictResolveDialogVisible === false) {
      intermediateHidden && setIntermediateHidden(false);
    } else {
      isLoading && setIsLoading(false);
    }
  }, [conflictResolveDialogVisible]);

  useEffect(() => {
    intermediateHidden &&
      setConflictDialogData(fileWithConflicts, operationData);
  }, [intermediateHidden]);

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
    } else {
      setMoveToPanelVisible(false);
    }
    setExpandedPanelKeys(null);
  };

  const onSubmit = (selectedFolder, folderTitle, providerKey) => {
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

    operationData = {
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
          fileWithConflicts = conflicts;
          setIntermediateHidden(true);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClose();
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
  const isVisible = intermediateHidden ? false : visible;
  return (
    <SelectFolderDialog
      selectionFiles={selection}
      isDisableTree={isLoading}
      foldersType="exceptSortedByTags"
      isPanelVisible={isVisible}
      onSubmit={onSubmit}
      onClose={onClose}
      id={isRecycleBin ? null : currentFolderId}
      withoutImmediatelyClose
      dialogName={
        isRecycleBin
          ? t("Common:Restore")
          : isCopy
          ? t("Translations:Copy")
          : t("Home:MoveTo")
      }
      buttonName={
        isRecycleBin
          ? t("Common:RestoreHere")
          : isCopy
          ? t("Translations:CopyHere")
          : t("Translations:MoveHere")
      }
      isRecycleBin={isRecycleBin}
      currentFolderId={currentFolderId}
    ></SelectFolderDialog>
  );
};

const OperationsPanel = withTranslation(["Translations", "Common", "Home"])(
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
    { isCopy }
  ) => {
    const { filter, selection, bufferSelection } = filesStore;
    const {
      isRecycleBinFolder,
      operationsFolders,
      setExpandedPanelKeys,
    } = treeFoldersStore;
    const { setConflictDialogData, checkFileConflicts } = filesActionsStore;
    const { itemOperationToFolder, clearActiveOperations } = uploadDataStore;

    const {
      moveToPanelVisible,
      copyPanelVisible,
      isFolderActions,
      setCopyPanelVisible,
      setMoveToPanelVisible,
      setDestFolderId,
      setThirdPartyMoveDialogVisible,
      setIsFolderActions,
      conflictResolveDialogVisible,
    } = dialogsStore;

    const selections = selection.length ? selection : [bufferSelection];
    const selectionsWithoutEditing = isCopy
      ? selections
      : selections.filter((f) => !f.isEditing);

    const provider = selections.find((x) => x.providerKey);

    return {
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
      conflictResolveDialogVisible,
      clearActiveOperations,
    };
  }
)(withRouter(observer(OperationsPanel)));
