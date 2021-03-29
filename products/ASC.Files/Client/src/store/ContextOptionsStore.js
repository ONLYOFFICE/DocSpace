import { makeAutoObservable, action } from "mobx";
import store from "studio/store";
import versionHistoryStore from "./VersionHistoryStore";
import filesActionsStore from "./FilesActionsStore";
import dialogsStore from "./DialogsStore";
import filesStore from "./FilesStore";
import mediaViewerDataStore from "./MediaViewerDataStore";
import selectedFolderStore from "./SelectedFolderStore";

import config from "../../package.json";
import history from "@appserver/common/history";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const { homepage } = config;

const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
const { openDocEditor, fileActionStore } = filesStore;
const { setAction } = fileActionStore;
const { setMediaViewerData } = mediaViewerDataStore;
const { isRootFolder } = selectedFolderStore; //      isRootFolder: selectedFolderStore.isRootFolder,
const {
  //copyToAction,
  //moveToAction,
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

    console.log(isTabletView);

    if (!isTabletView) {
      fetchFileVersions(id + "");
      setIsVerHistoryPanel(true);
    } else {
      window.history.replaceState(
        "",
        "",
        combineUrl(AppServerConfig.proxyURL, homepage, `/${id}/history`)
      );
    }
  };

  getContextOptions = (item, t) => {
    const {
      contextOptions,
      id,
      folderId,
      locked,
      fileExst,
      canOpenPlayer,
      providerKey,
      viewUrl,
      title,
      parentId,
      webUrl,
      access,
    } = item;

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
            //onClick: finalizeVersion,
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
            //onClick: onOpenLocation,
            disabled: false,
          };
        case "mark-as-favorite":
          return {
            key: option,
            label: t("MarkAsFavorite"),
            icon: "images/favorites.react.svg",
            //onClick: onClickFavorite,
            disabled: false,
            "data-action": "mark",
          };
        case "block-unblock-version":
          return {
            key: option,
            label: t("UnblockVersion"),
            icon: "images/lock.react.svg",
            //onClick: lockFile,
            disabled: false,
          };
        case "sharing-settings":
          return {
            key: option,
            label: t("SharingSettings"),
            icon: "images/catalog.shared.react.svg",
            //onClick: onClickShare,
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
            //onClick: onOwnerChange,
            disabled: false,
          };
        case "link-for-portal-users":
          return {
            key: option,
            label: t("LinkForPortalUsers"),
            icon: "/static/images/invitation.link.react.svg",
            //onClick: onClickLinkForPortal,
            disabled: false,
          };
        case "edit":
          return {
            key: option,
            label: t("Edit"),
            icon: "images/access.edit.react.svg",
            //onClick: onClickLinkEdit,
            disabled: false,
          };
        case "preview":
          return {
            key: option,
            label: t("Preview"),
            icon: "EyeIcon",
            //onClick: onClickLinkEdit,
            disabled: true,
          };
        case "view":
          return {
            key: option,
            label: t("View"),
            icon: "/static/images/eye.react.svg",
            //onClick: onMediaFileClick,
            disabled: false,
          };
        case "download":
          return {
            key: option,
            label: t("Download"),
            icon: "images/download.react.svg",
            //onClick: onClickDownload,
            disabled: false,
          };
        case "move":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "images/move.react.svg",
            //onClick: onMoveAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Copy"),
            icon: "/static/images/copy.react.svg",
            //onClick: onCopyAction,
            disabled: false,
          };
        case "duplicate":
          return {
            key: option,
            label: t("Duplicate"),
            icon: "/static/images/copy.react.svg",
            //onClick: onDuplicate,
            disabled: false,
          };
        case "rename":
          return {
            key: option,
            label: t("Rename"),
            icon: "images/rename.react.svg",
            //onClick: onClickRename,
            disabled: false,
          };
        case "change-thirdparty-info":
          return {
            key: option,
            label: t("ThirdPartyInfo"),
            icon: "images/access.edit.react.svg",
            //onClick: onChangeThirdPartyInfo,
            disabled: false,
          };
        case "delete":
          return {
            key: option,
            label: isThirdPartyFolder ? t("DeleteThirdParty") : t("Delete"),
            icon: "/static/images/catalog.trash.react.svg",
            //onClick: onClickDelete,
            disabled: false,
          };
        case "remove-from-favorites":
          return {
            key: option,
            label: t("RemoveFromFavorites"),
            icon: "images/favorites.react.svg",
            //onClick: onClickFavorite,
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
