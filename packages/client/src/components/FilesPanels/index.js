import React from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  SharingPanel,
  UploadPanel,
  VersionHistoryPanel,
  ChangeOwnerPanel,
  NewFilesPanel,
  HotkeyPanel,
  InvitePanel,
  EditLinkPanel,
  EmbeddingPanel,
} from "../panels";
import {
  ConnectDialog,
  DeleteThirdPartyDialog,
  EmptyTrashDialog,
  DeleteDialog,
  DownloadDialog,
  ConflictResolveDialog,
  ConvertDialog,
  CreateRoomDialog,
  InviteUsersWarningDialog,
  CreateRoomConfirmDialog,
  ChangeUserTypeDialog,
  SubmitToFormGallery,
  UnsavedChangesDialog,
  DeleteLinkDialog,
  RoomSharingDialog,
  MoveToPublicRoom,
  SettingsPluginDialog,
  PluginDialog,
  DeletePluginDialog,
} from "../dialogs";
import ConvertPasswordDialog from "../dialogs/ConvertPasswordDialog";
import ArchiveDialog from "../dialogs/ArchiveDialog";
import RestoreRoomDialog from "../dialogs/RestoreRoomDialog";
import PreparationPortalDialog from "../dialogs/PreparationPortalDialog";
import FilesSelector from "../FilesSelector";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";
import LeaveRoomDialog from "../dialogs/LeaveRoomDialog";
import ChangeRoomOwnerPanel from "../panels/ChangeRoomOwnerPanel";

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
    createRoomConfirmDialogVisible,
    confirmDialogIsLoading,
    restoreAllPanelVisible,
    archiveDialogVisible,
    inviteUsersWarningDialogVisible,
    preparationPortalDialogVisible,
    changeUserTypeDialogVisible,
    restoreRoomDialogVisible,
    submitToGalleryDialogVisible,
    editLinkPanelIsVisible,
    unsavedChangesDialogVisible,
    deleteLinkDialogVisible,
    embeddingPanelIsVisible,
    roomSharingPanelVisible,
    moveToPublicRoomVisible,
    settingsPluginDialogVisible,
    pluginDialogVisible,
    leaveRoomDialogVisible,
    changeRoomOwnerIsVisible,
    deletePluginDialogVisible,
  } = props;

  const { t } = useTranslation(["Translations", "Common"]);

  const onClose = () => {
    setSelectFileDialogVisible(false);
  };

  return [
    settingsPluginDialogVisible && (
      <SettingsPluginDialog
        isVisible={settingsPluginDialogVisible}
        key={"settings-plugin-dialog"}
      />
    ),
    deletePluginDialogVisible && (
      <DeletePluginDialog
        isVisible={deletePluginDialogVisible}
        key={"delete-plugin-dialog"}
      />
    ),
    pluginDialogVisible && (
      <PluginDialog isVisible={pluginDialogVisible} key={"plugin-dialog"} />
    ),
    uploadPanelVisible && <UploadPanel key="upload-panel" />,
    sharingPanelVisible && (
      <SharingPanel
        key="sharing-panel"
        uploadPanelVisible={uploadPanelVisible}
      />
    ),
    ownerPanelVisible && <ChangeOwnerPanel key="change-owner-panel" />,
    (moveToPanelVisible || copyPanelVisible || restoreAllPanelVisible) && (
      <FilesSelector
        key="files-selector"
        isMove={moveToPanelVisible}
        isCopy={copyPanelVisible}
        isRestoreAll={restoreAllPanelVisible}
      />
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

    newFilesPanelVisible && <NewFilesPanel key="new-files-panel" />,
    conflictResolveDialogVisible && (
      <ConflictResolveDialog key="conflict-resolve-dialog" />
    ),
    convertDialogVisible && <ConvertDialog key="convert-dialog" />,
    changeUserTypeDialogVisible && (
      <ChangeUserTypeDialog key="change-user-type-dialog" />
    ),
    createRoomDialogVisible && <CreateRoomDialog key="create-room-dialog" />,
    (createRoomConfirmDialogVisible || confirmDialogIsLoading) && (
      <CreateRoomConfirmDialog key="create-room-confirm-dialog" />
    ),
    selectFileDialogVisible && (
      <FilesSelector
        key="select-file-dialog"
        filterParam={FilesSelectorFilterTypes.DOCX}
        isPanelVisible={selectFileDialogVisible}
        onSelectFile={createMasterForm}
        onClose={onClose}
      />
    ),

    hotkeyPanelVisible && <HotkeyPanel key="hotkey-panel" />,
    invitePanelVisible && <InvitePanel key="invite-panel" />,
    convertPasswordDialogVisible && (
      <ConvertPasswordDialog key="convert-password-dialog" />
    ),
    archiveDialogVisible && <ArchiveDialog key="archive-dialog" />,
    restoreRoomDialogVisible && <RestoreRoomDialog key="archive-dialog" />,
    inviteUsersWarningDialogVisible && (
      <InviteUsersWarningDialog key="invite-users-warning-dialog" />
    ),
    preparationPortalDialogVisible && (
      <PreparationPortalDialog key="preparation-portal-dialog" />
    ),
    submitToGalleryDialogVisible && (
      <SubmitToFormGallery key="submit-to-form-gallery-dialog" />
    ),
    editLinkPanelIsVisible && <EditLinkPanel key="edit-link-panel" />,
    unsavedChangesDialogVisible && (
      <UnsavedChangesDialog key="unsaved-dialog" />
    ),
    deleteLinkDialogVisible && <DeleteLinkDialog key="delete-link-dialog" />,
    embeddingPanelIsVisible && <EmbeddingPanel key="embedding-panel" />,
    roomSharingPanelVisible && <RoomSharingDialog key="room-sharing-dialog" />,
    moveToPublicRoomVisible && (
      <MoveToPublicRoom key="move-to-public-room-panel" />
    ),
    leaveRoomDialogVisible && <LeaveRoomDialog key="leave-room-dialog" />,
    changeRoomOwnerIsVisible && (
      <ChangeRoomOwnerPanel key="change-room-owner" />
    ),
  ];
};

export default inject(
  ({
    auth,
    dialogsStore,
    uploadDataStore,
    versionHistoryStore,
    backup,
    createEditRoomStore,
    pluginStore,
  }) => {
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
      newFilesPanelVisible,
      conflictResolveDialogVisible,
      convertDialogVisible,
      createRoomDialogVisible,
      createRoomConfirmDialogVisible,
      convertPasswordDialogVisible,
      connectItem, //TODO:
      restoreAllPanelVisible,
      archiveDialogVisible,
      restoreRoomDialogVisible,

      unsavedChangesDialogVisible,
      createMasterForm,
      selectFileDialogVisible,
      setSelectFileDialogVisible,
      invitePanelOptions,
      inviteUsersWarningDialogVisible,
      changeUserTypeDialogVisible,

      submitToGalleryDialogVisible,
      editLinkPanelIsVisible,
      deleteLinkDialogVisible,
      embeddingPanelIsVisible,
      roomSharingPanelVisible,
      moveToPublicRoomVisible,
      leaveRoomDialogVisible,
      changeRoomOwnerIsVisible,
    } = dialogsStore;

    const { preparationPortalDialogVisible } = backup;

    const { uploadPanelVisible } = uploadDataStore;
    const { isVisible: versionHistoryPanelVisible } = versionHistoryStore;
    const { hotkeyPanelVisible } = auth.settingsStore;
    const { confirmDialogIsLoading } = createEditRoomStore;

    const {
      settingsPluginDialogVisible,
      deletePluginDialogVisible,
      pluginDialogVisible,
    } = pluginStore;

    return {
      preparationPortalDialogVisible,
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
      newFilesPanelVisible,
      conflictResolveDialogVisible,
      convertDialogVisible,
      createRoomDialogVisible,
      createRoomConfirmDialogVisible,
      convertPasswordDialogVisible,
      selectFileDialogVisible,
      createMasterForm,
      setSelectFileDialogVisible,
      hotkeyPanelVisible,
      restoreAllPanelVisible,
      invitePanelVisible: invitePanelOptions.visible,
      archiveDialogVisible,
      inviteUsersWarningDialogVisible,
      confirmDialogIsLoading,
      changeUserTypeDialogVisible,
      restoreRoomDialogVisible,
      submitToGalleryDialogVisible,
      editLinkPanelIsVisible,
      unsavedChangesDialogVisible,
      deleteLinkDialogVisible,
      embeddingPanelIsVisible,
      roomSharingPanelVisible,
      moveToPublicRoomVisible,
      settingsPluginDialogVisible,
      pluginDialogVisible,
      leaveRoomDialogVisible,
      changeRoomOwnerIsVisible,
      deletePluginDialogVisible,
    };
  }
)(observer(Panels));
