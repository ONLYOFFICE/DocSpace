import { makeAutoObservable, action } from "mobx";
import store from "studio/store";
import versionHistoryStore from "./VersionHistoryStore";
import filesActionsStore from "./FilesActionsStore";
import dialogsStore from "./DialogsStore";
import filesStore from "./FilesStore";
import mediaViewerDataStore from "./MediaViewerDataStore";
import selectedFolderStore from "./SelectedFolderStore";

import config from "../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig, FileAction } from "@appserver/common/constants";

import toastr from "studio/toastr";
import copy from "copy-to-clipboard";

const { homepage } = config;

const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
const { openDocEditor, fileActionStore } = filesStore;
const { setAction } = fileActionStore;
const { setMediaViewerData } = mediaViewerDataStore;
const { isRootFolder } = selectedFolderStore;
const {
  setFavoriteAction,
  finalizeVersionAction,
  lockFileAction,
  onSelectItem,
  duplicateAction,
  setThirdpartyInfo,
  deleteFileAction,
  openLocationAction,
  deleteFolderAction,
} = filesActionsStore;

const {
  setSharingPanelVisible,
  setChangeOwnerPanelVisible,
  setMoveToPanelVisible,
  setCopyPanelVisible,
  setRemoveItem,
  setDeleteThirdPartyDialogVisible,
} = dialogsStore;

class ContextOptionsStore {
  t = null;
  history = null;

  constructor() {
    makeAutoObservable(this, {
      getFilesContextOptions: action,
    });
  }

  onOpenLocation = () => {
    const { id, folderId, isFolder } = filesStore.selection[0];
    const locationId = isFolder ? id : folderId;
    openLocationAction(locationId, isFolder);
  };

  showVersionHistory = () => {
    const { isTabletView } = store.auth.settingsStore;
    const { id } = filesStore.selection[0];

    if (!isTabletView) {
      fetchFileVersions(id + "");
      setIsVerHistoryPanel(true);
    } else {
      this.history.push(
        combineUrl(AppServerConfig.proxyURL, homepage, `/${id}/history`) //TODO: something better
      );
    }
  };

  onClickFavorite = (e) => {
    const { id } = filesStore.selection[0];
    const { action } = e.currentTarget.dataset;
    setFavoriteAction(action, id)
      .then(() =>
        action === "mark"
          ? toastr.success(this.t("MarkedAsFavorite")) // TODO: t
          : toastr.success(this.t("RemovedFromFavorites"))
      )
      .catch((err) => {
        console.log(err);
        toastr.error(err);
      });
  };

  finalizeVersion = () => {
    const { id } = filesStore.selection[0];
    finalizeVersionAction(id).catch((err) => toastr.error(err));
  };

  lockFile = () => {
    const { id, locked } = filesStore.selection[0];
    lockFileAction(id, !locked).catch((err) => toastr.error(err));
  };

  onClickShare = () => {
    onSelectItem(filesStore.selection[0]);
    setSharingPanelVisible(true);
  };

  onOwnerChange = () => setChangeOwnerPanelVisible(true);

  onClickLinkForPortal = () => {
    //const isFile = !!fileExst;
    const { id, isFolder, canOpenPlayer, webUrl } = filesStore.selection[0];
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
    const { id, providerKey } = filesStore.selection[0];
    openDocEditor(id, providerKey);
  };

  onMediaFileClick = (fileId) => {
    const { id } = filesStore.selection[0];
    const itemId = typeof fileId !== "object" ? fileId : id;
    setMediaViewerData({ visible: true, id: itemId });
  };

  onClickDownload = () => {
    const { viewUrl } = filesStore.selection[0];
    window.open(viewUrl, "_blank");
  };

  onMoveAction = () => setMoveToPanelVisible(true);
  onCopyAction = () => setCopyPanelVisible(true);

  onDuplicate = () => {
    duplicateAction(
      filesStore.selection[0],
      this.t("CopyOperation")
    ).catch((err) => toastr.error(err));
  };

  onClickRename = () => {
    const { fileExst, id } = filesStore.selection[0];
    setAction({
      type: FileAction.Rename,
      extension: fileExst,
      id,
    });
  };

  onChangeThirdPartyInfo = () => setThirdpartyInfo();

  onClickDelete = () => {
    const {
      id,
      fileExst,
      folderId,
      parentId,
      providerKey,
    } = filesStore.selection[0];
    const isThirdPartyFolder = providerKey && isRootFolder;
    if (isThirdPartyFolder) {
      const splitItem = id.split("-");
      setRemoveItem({ id: splitItem[splitItem.length - 1], title });
      setDeleteThirdPartyDialogVisible(true);
      return;
    }

    const translations = {
      deleteOperation: this.t("DeleteOperation"),
    };

    fileExst
      ? deleteFileAction(id, folderId, translations)
          .then(() => toastr.success(this.t("FileRemoved")))
          .catch((err) => toastr.error(err))
      : deleteFolderAction(id, parentId, translations)
          .then(() => toastr.success(this.t("FolderRemoved")))
          .catch((err) => toastr.error(err));
  };

  getContextOptions = (item, t, history) => {
    const { contextOptions, providerKey, access } = item;

    this.t = t;
    this.history = history;

    const isSharable = access !== 1 && access !== 0;
    const isThirdPartyFolder = providerKey && isRootFolder;

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
        case "move":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "images/move.react.svg",
            onClick: this.onMoveAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Copy"),
            icon: "/static/images/copy.react.svg",
            onClick: this.onCopyAction,
            disabled: false,
          };
        case "duplicate":
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
          };
        default:
          break;
      }

      return undefined;
    });
  };
}

export default new ContextOptionsStore();
