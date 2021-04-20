import { makeAutoObservable } from "mobx";
import copy from "copy-to-clipboard";

import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig, FileAction } from "@appserver/common/constants";

import store from "studio/store";
import toastr from "studio/toastr";
import config from "../../package.json";

const { homepage } = config;

class ContextOptionsStore {
  t = null;
  history = null;
  filesStore;
  fileActionStore;
  selectedFolderStore;
  filesActionsStore;
  dialogsStore;
  versionHistoryStore;
  mediaViewerDataStore;

  constructor(
    filesStore,
    fileActionStore,
    selectedFolderStore,
    filesActionsStore,
    dialogsStore,
    versionHistoryStore,
    mediaViewerDataStore
  ) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
    this.fileActionStore = fileActionStore;
    this.selectedFolderStore = selectedFolderStore;
    this.filesActionsStore = filesActionsStore;
    this.dialogsStore = dialogsStore;
    this.versionHistoryStore = versionHistoryStore;
    this.mediaViewerDataStore = mediaViewerDataStore;
    //getFilesContextOptions: action,
  }

  onOpenLocation = () => {
    const { id, folderId, isFolder } = this.filesStore.selection[0];
    const locationId = isFolder ? id : folderId;
    this.filesActionsStore.openLocationAction(locationId, isFolder);
  };

  showVersionHistory = () => {
    const { isTabletView } = store.auth.settingsStore;
    const { id } = this.filesStore.selection[0];

    if (!isTabletView) {
      this.versionHistoryStore.fetchFileVersions(id + "");
      this.versionHistoryStore.setIsVerHistoryPanel(true);
    } else {
      this.history.push(
        combineUrl(AppServerConfig.proxyURL, homepage, `/${id}/history`) //TODO: something better
      );
    }
  };

  onClickFavorite = (e) => {
    const { id } = this.filesStore.selection[0];
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;
    this.filesActionsStore
      .setFavoriteAction(action, id)
      .then(() =>
        action === "mark"
          ? toastr.success(this.t("MarkedAsFavorite")) // TODO: t
          : toastr.success(this.t("RemovedFromFavorites"))
      )
      .catch((err) => {
        toastr.error(err);
      });
  };

  finalizeVersion = () => {
    const { id } = this.filesStore.selection[0];
    this.filesActionsStore
      .finalizeVersionAction(id)
      .catch((err) => toastr.error(err));
  };

  lockFile = () => {
    const { id, locked } = this.filesStore.selection[0];
    this.filesActionsStore
      .lockFileAction(id, !locked)
      .catch((err) => toastr.error(err));
  };

  onClickShare = () => {
    this.filesActionsStore.onSelectItem(this.filesStore.selection[0]);
    this.dialogsStore.setSharingPanelVisible(true);
  };

  onOwnerChange = () => this.dialogsStore.setChangeOwnerPanelVisible(true);

  onClickLinkForPortal = () => {
    //const isFile = !!fileExst;
    const {
      id,
      isFolder,
      canOpenPlayer,
      webUrl,
    } = this.filesStore.selection[0];
    copy(
      !isFolder
        ? canOpenPlayer
          ? `${window.location.href}&preview=${id}`
          : webUrl
        : `${window.location.origin + homepage}/filter?folder=${id}`
    );

    toastr.success(this.t("LinkCopySuccess"));
  };

  onClickLinkEdit = () => {
    const { id, providerKey } = this.filesStore.selection[0];
    this.filesStore.openDocEditor(id, providerKey);
  };

  onMediaFileClick = (fileId) => {
    const { id } = this.filesStore.selection[0];
    const itemId = typeof fileId !== "object" ? fileId : id;
    this.mediaViewerDataStore.setMediaViewerData({ visible: true, id: itemId });
  };

  onClickDownload = () => {
    const { viewUrl, fileExst, contentLength } = this.filesStore.selection[0];
    const isFile = !!fileExst && contentLength;

    isFile
      ? window.open(viewUrl, "_blank")
      : this.filesActionsStore
          .downloadAction(this.t("ArchivingData"))
          .catch((err) => toastr.error(err));
    window.open(viewUrl, "_blank");
  };

  onClickDownloadAs = () => this.dialogsStore.setDownloadDialogVisible(true);

  onMoveAction = () => this.dialogsStore.setMoveToPanelVisible(true);
  onCopyAction = () => this.dialogsStore.setCopyPanelVisible(true);

  onDuplicate = () => {
    this.filesActionsStore
      .duplicateAction(this.filesStore.selection[0], this.t("CopyOperation"))
      .catch((err) => toastr.error(err));
  };

  onClickRename = () => {
    const { fileExst, id } = this.filesStore.selection[0];
    this.fileActionStore.setAction({
      type: FileAction.Rename,
      extension: fileExst,
      id,
    });
  };

  onChangeThirdPartyInfo = () => {
    const { providerKey } = this.filesStore.selection[0];
    this.filesActionsStore.setThirdpartyInfo(providerKey);
  };

  onClickDelete = () => {
    const {
      id,
      fileExst,
      folderId,
      parentId,
      providerKey,
      contentLength,
    } = this.filesStore.selection[0];

    const isThirdPartyFolder =
      providerKey && this.selectedFolderStore.isRootFolder;

    if (isThirdPartyFolder) {
      const splitItem = id.split("-");
      this.dialogsStore.setRemoveItem({
        id: splitItem[splitItem.length - 1],
        title,
      });
      this.dialogsStore.setDeleteThirdPartyDialogVisible(true);
      return;
    }

    const translations = {
      deleteOperation: this.t("DeleteOperation"),
    };

    fileExst || contentLength
      ? this.filesActionsStore
          .deleteFileAction(id, folderId, translations)
          .then(() => toastr.success(this.t("FileRemoved")))
          .catch((err) => toastr.error(err))
      : this.filesActionsStore
          .deleteFolderAction(id, parentId, translations)
          .then(() => toastr.success(this.t("FolderRemoved")))
          .catch((err) => toastr.error(err));
  };

  getContextOptions = (item, t, history) => {
    const { contextOptions, providerKey, access } = item;

    this.t = t;
    this.history = history;

    const isSharable = access !== 1 && access !== 0;
    const isThirdPartyFolder =
      providerKey && this.selectedFolderStore.isRootFolder;

    return contextOptions.map((option) => {
      switch (option) {
        case "open":
          return {
            key: option,
            label: t("Open"),
            icon: "images/catalog.folder.react.svg",
            onClick: this.onOpenLocation,
            disabled: false,
          };
        case "show-version-history":
          return {
            key: option,
            label: t("ShowVersionHistory"),
            icon: "images/history.react.svg",
            onClick: this.showVersionHistory,
            disabled: false,
          };
        case "finalize-version":
          return {
            key: option,
            label: t("FinalizeVersion"),
            icon: "images/history-finalized.react.svg",
            onClick: this.finalizeVersion,
            disabled: false,
          };
        case "separator0":
        case "separator1":
        case "separator2":
        case "separator3":
          return { key: option, isSeparator: true };
        case "open-location":
          return {
            key: option,
            label: t("OpenLocation"),
            icon: "images/download-as.react.svg",
            onClick: this.onOpenLocation,
            disabled: false,
          };
        case "mark-as-favorite":
          return {
            key: option,
            label: t("MarkAsFavorite"),
            icon: "images/favorites.react.svg",
            onClick: this.onClickFavorite,
            disabled: false,
            "data-action": "mark",
            action: "mark",
          };
        case "block-unblock-version":
          return {
            key: option,
            label: t("UnblockVersion"),
            icon: "images/lock.react.svg",
            onClick: this.lockFile,
            disabled: false,
          };
        case "sharing-settings":
          return {
            key: option,
            label: t("SharingSettings"),
            icon: "images/catalog.shared.react.svg",
            onClick: this.onClickShare,
            disabled: isSharable,
          };
        case "send-by-email":
          return {
            key: option,
            label: t("SendByEmail"),
            icon: "/static/images/mail.react.svg",
            disabled: true,
          };
        case "owner-change":
          return {
            key: option,
            label: t("ChangeOwner"),
            icon: "images/catalog.user.react.svg",
            onClick: this.onOwnerChange,
            disabled: false,
          };
        case "link-for-portal-users":
          return {
            key: option,
            label: t("LinkForPortalUsers"),
            icon: "/static/images/invitation.link.react.svg",
            onClick: this.onClickLinkForPortal,
            disabled: false,
          };
        case "edit":
          return {
            key: option,
            label: t("Edit"),
            icon: "images/access.edit.react.svg",
            onClick: this.onClickLinkEdit,
            disabled: false,
          };
        case "preview":
          return {
            key: option,
            label: t("Preview"),
            icon: "EyeIcon",
            onClick: this.onClickLinkEdit,
            disabled: true,
          };
        case "view":
          return {
            key: option,
            label: t("View"),
            icon: "/static/images/eye.react.svg",
            onClick: this.onMediaFileClick,
            disabled: false,
          };
        case "download":
          return {
            key: option,
            label: t("Download"),
            icon: "images/download.react.svg",
            onClick: this.onClickDownload,
            disabled: false,
          };
        case "download-as":
          return {
            key: option,
            label: t("DownloadAs"),
            icon: "images/download-as.react.svg",
            onClick: this.onClickDownloadAs,
            disabled: false,
          };
        case "move-to":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "images/move.react.svg",
            onClick: this.onMoveAction,
            disabled: false,
          };
        case "restore":
          return {
            key: option,
            label: t("Restore"),
            icon: "images/move.react.svg",
            onClick: this.onMoveAction,
            disabled: false,
          };
        case "copy-to":
          return {
            key: option,
            label: t("Copy"),
            icon: "/static/images/copy.react.svg",
            onClick: this.onCopyAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Duplicate"),
            icon: "/static/images/copy.react.svg",
            onClick: this.onDuplicate,
            disabled: false,
          };
        case "rename":
          return {
            key: option,
            label: t("Rename"),
            icon: "images/rename.react.svg",
            onClick: this.onClickRename,
            disabled: false,
          };
        case "change-thirdparty-info":
          return {
            key: option,
            label: t("ThirdPartyInfo"),
            icon: "images/access.edit.react.svg",
            onClick: this.onChangeThirdPartyInfo,
            disabled: false,
          };
        case "delete":
          return {
            key: option,
            label: isThirdPartyFolder ? t("DeleteThirdParty") : t("Delete"),
            icon: "/static/images/catalog.trash.react.svg",
            onClick: this.onClickDelete,
            disabled: false,
          };
        case "remove-from-favorites":
          return {
            key: option,
            label: t("RemoveFromFavorites"),
            icon: "images/favorites.react.svg",
            onClick: this.onClickFavorite,
            disabled: false,
            "data-action": "remove",
            action: "remove",
          };
        default:
          break;
      }

      return undefined;
    });
  };
}

export default ContextOptionsStore;
