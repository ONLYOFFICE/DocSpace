import React from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  SharingPanel,
  UploadPanel,
  OperationsPanel,
  VersionHistoryPanel,
  ChangeOwnerPanel,
  NewFilesPanel,
  SelectFileDialog,
} from "../panels";
import {
  ThirdPartyMoveDialog,
  ConnectDialog,
  DeleteThirdPartyDialog,
  EmptyTrashDialog,
  DeleteDialog,
  DownloadDialog,
  ThirdPartyDialog,
  ConflictResolveDialog,
  ConvertDialog,
  CreateFolderDialog,
} from "../dialogs";
import ConvertPasswordDialog from "../dialogs/ConvertPasswordDialog";
import { FileAction } from "@appserver/common/constants";

const Panels = (props) => {
  const {
    uploadPanelVisible,
    sharingPanelVisible,
    ownerPanelVisible,
    copyPanelVisible,
    moveToPanelVisible,
    thirdPartyMoveDialogVisible,
    connectDialogVisible,
    deleteThirdPartyDialogVisible,
    versionHistoryPanelVisible,
    deleteDialogVisible,
    downloadDialogVisible,
    emptyTrashDialogVisible,
    thirdPartyDialogVisible,
    newFilesPanelVisible,
    conflictResolveDialogVisible,
    convertDialogVisible,
    createMasterForm,
    selectFileDialogVisible,
    setSelectFileDialogVisible,
    createFolderDialogVisible,
    convertPasswordDialogVisible,
    actionType,
    setCreateFolderDialogVisible,
  } = props;

  const { t } = useTranslation(["Translations", "SelectFile"]);

  const onClose = () => {
    setSelectFileDialogVisible(false);
  };

  React.useEffect(() => {
    if (actionType === FileAction.Create) {
      //this.onCreateAddTempItem(newItem);
      setCreateFolderDialogVisible(true);
    }
  }, [actionType]);

  return [
    uploadPanelVisible && <UploadPanel key="upload-panel" />,
    sharingPanelVisible && (
      <SharingPanel
        key="sharing-panel"
        uploadPanelVisible={uploadPanelVisible}
      />
    ),
    ownerPanelVisible && <ChangeOwnerPanel key="change-owner-panel" />,
    (moveToPanelVisible || copyPanelVisible) && (
      <OperationsPanel key="operation-panel" isCopy={copyPanelVisible} />
    ),
    thirdPartyMoveDialogVisible && (
      <ThirdPartyMoveDialog key="thirdparty-move-dialog" />
    ),
    connectDialogVisible && <ConnectDialog key="connect-dialog" />,
    deleteThirdPartyDialogVisible && (
      <DeleteThirdPartyDialog key="thirdparty-delete-dialog" />
    ),
    versionHistoryPanelVisible && (
      <VersionHistoryPanel key="version-history-panel" />
    ),
    deleteDialogVisible && <DeleteDialog key="delete-dialog" />,
    emptyTrashDialogVisible && <EmptyTrashDialog key="empty-trash-dialog" />,
    downloadDialogVisible && <DownloadDialog key="download-dialog" />,
    thirdPartyDialogVisible && <ThirdPartyDialog key="thirdparty-dialog" />,
    newFilesPanelVisible && <NewFilesPanel key="new-files-panel" />,
    conflictResolveDialogVisible && (
      <ConflictResolveDialog key="conflict-resolve-dialog" />
    ),
    convertDialogVisible && <ConvertDialog key="convert-dialog" />,
    selectFileDialogVisible && (
      <SelectFileDialog
        key="select-file-dialog"
        resetTreeFolders
        onSelectFile={createMasterForm}
        isPanelVisible={selectFileDialogVisible}
        onClose={onClose}
        foldersType="exceptPrivacyTrashFolders"
        ByExtension
        searchParam={".docx"}
        headerName={t("Translations:CreateMasterFormFromFile")}
        titleFilesList={t("SelectFile:SelectDOCXFormat")}
        creationButtonPrimary
        withSubfolders={false}
      />
    ),
    createFolderDialogVisible && (
      <CreateFolderDialog key="create-folder-dialog" />
    ),
    convertPasswordDialogVisible && (
      <ConvertPasswordDialog key="convert-password-dialog" />
    ),
  ];
};

export default inject(
  ({ dialogsStore, uploadDataStore, versionHistoryStore, filesStore }) => {
    const {
      sharingPanelVisible,
      ownerPanelVisible,
      copyPanelVisible,
      moveToPanelVisible,
      thirdPartyMoveDialogVisible,
      connectDialogVisible,
      deleteThirdPartyDialogVisible,
      deleteDialogVisible,
      downloadDialogVisible,
      emptyTrashDialogVisible,
      thirdPartyDialogVisible,
      newFilesPanelVisible,
      conflictResolveDialogVisible,
      convertDialogVisible,
      convertPasswordDialogVisible,
      connectItem, //TODO:

      createMasterForm,
      selectFileDialogVisible,
      setSelectFileDialogVisible,
      createFolderDialogVisible,
      setCreateFolderDialogVisible,
    } = dialogsStore;

    const { uploadPanelVisible } = uploadDataStore;
    const { isVisible: versionHistoryPanelVisible } = versionHistoryStore;
    const { fileActionStore } = filesStore;
    return {
      sharingPanelVisible,
      uploadPanelVisible,
      ownerPanelVisible,
      copyPanelVisible,
      moveToPanelVisible,
      thirdPartyMoveDialogVisible,
      connectDialogVisible: connectDialogVisible || !!connectItem, //TODO:
      deleteThirdPartyDialogVisible,
      versionHistoryPanelVisible,
      deleteDialogVisible,
      downloadDialogVisible,
      emptyTrashDialogVisible,
      thirdPartyDialogVisible,
      newFilesPanelVisible,
      conflictResolveDialogVisible,
      convertDialogVisible,
      convertPasswordDialogVisible,
      selectFileDialogVisible,
      createMasterForm,
      setSelectFileDialogVisible,
      createFolderDialogVisible,
      actionType: fileActionStore.type,
      setCreateFolderDialogVisible,
    };
  }
)(observer(Panels));
