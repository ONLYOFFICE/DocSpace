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
  HotkeyPanel,
  InvitePanel,
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
  CreateRoomDialog,
  InviteUsersWarningDialog,
} from "../dialogs";
import ConvertPasswordDialog from "../dialogs/ConvertPasswordDialog";
import RestoreArchiveDialog from "../dialogs/RestoreArchiveDialog";

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
    hotkeyPanelVisible,
    invitePanelVisible,
    convertPasswordDialogVisible,
    createRoomDialogVisible,
    restoreAllPanelVisible,
    restoreArchiveDialogVisible,
    inviteUsersWarningDialogVisible,
  } = props;

  console.log(restoreArchiveDialogVisible);

  const { t } = useTranslation(["Translations", "SelectFile"]);

  const onClose = () => {
    setSelectFileDialogVisible(false);
  };

  return [
    uploadPanelVisible && <UploadPanel key="upload-panel" />,
    sharingPanelVisible && (
      <SharingPanel
        key="sharing-panel"
        uploadPanelVisible={uploadPanelVisible}
      />
    ),
    ownerPanelVisible && <ChangeOwnerPanel key="change-owner-panel" />,
    (moveToPanelVisible || copyPanelVisible || restoreAllPanelVisible) && (
      <OperationsPanel
        key="operation-panel"
        isCopy={copyPanelVisible}
        isRestore={restoreAllPanelVisible}
      />
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
    createRoomDialogVisible && <CreateRoomDialog key="create-room-dialog" />,
    selectFileDialogVisible && (
      <SelectFileDialog
        key="select-file-dialog"
        //resetTreeFolders
        onSelectFile={createMasterForm}
        isPanelVisible={selectFileDialogVisible}
        onClose={onClose}
        filteredType="exceptPrivacyTrashArchiveFolders"
        ByExtension
        searchParam={".docx"}
        dialogName={t("Translations:CreateMasterFormFromFile")}
        filesListTitle={t("SelectFile:SelectDOCXFormat")}
        creationButtonPrimary
        withSubfolders={false}
      />
    ),
    hotkeyPanelVisible && <HotkeyPanel key="hotkey-panel" />,
    invitePanelVisible && <InvitePanel key="invite-panel" />,
    convertPasswordDialogVisible && (
      <ConvertPasswordDialog key="convert-password-dialog" />
    ),
    restoreArchiveDialogVisible && (
      <RestoreArchiveDialog key="restore-all-archive-dialog" />
    ),
    inviteUsersWarningDialogVisible && (
      <InviteUsersWarningDialog key="invite-users-warning-dialog" />
    ),
  ];
};

export default inject(
  ({ auth, dialogsStore, uploadDataStore, versionHistoryStore }) => {
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
      createRoomDialogVisible,
      convertPasswordDialogVisible,
      connectItem, //TODO:
      restoreAllPanelVisible,
      restoreArchiveDialogVisible,

      createMasterForm,
      selectFileDialogVisible,
      setSelectFileDialogVisible,
      invitePanelOptions,
      inviteUsersWarningDialogVisible,
    } = dialogsStore;

    const { uploadPanelVisible } = uploadDataStore;
    const { isVisible: versionHistoryPanelVisible } = versionHistoryStore;
    const { hotkeyPanelVisible } = auth.settingsStore;

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
      createRoomDialogVisible,
      convertPasswordDialogVisible,
      selectFileDialogVisible,
      createMasterForm,
      setSelectFileDialogVisible,
      hotkeyPanelVisible,
      restoreAllPanelVisible,
      invitePanelVisible: invitePanelOptions.visible,
      restoreArchiveDialogVisible,
      inviteUsersWarningDialogVisible,
    };
  }
)(observer(Panels));
