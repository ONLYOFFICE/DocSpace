import React from "react";
import { inject, observer } from "mobx-react";
import copy from "copy-to-clipboard";
import { combineUrl } from "@appserver/common/utils";
import { FileAction, AppServerConfig } from "@appserver/common/constants";
import toastr from "studio/toastr";
import config from "../../package.json";

export default function withContextOptions(WrappedComponent) {
  class WithContextOptions extends React.Component {
    onOpenLocation = () => {
      const { item, openLocationAction } = this.props;
      const { parentId, folderId, fileExst } = item;
      const locationId = !fileExst ? parentId : folderId;
      openLocationAction(locationId, !fileExst);
    };

    onOwnerChange = () => {
      const { setChangeOwnerPanelVisible } = this.props;
      setChangeOwnerPanelVisible(true);
    };
    onMoveAction = () => {
      const { setMoveToPanelVisible } = this.props;
      setMoveToPanelVisible(true);
    };
    onCopyAction = () => {
      const { setCopyPanelVisible } = this.props;
      setCopyPanelVisible(true);
    };

    showVersionHistory = () => {
      const {
        item,
        isTabletView,
        fetchFileVersions,
        setIsVerHistoryPanel,
        history,
        homepage,
        isTrashFolder,
      } = this.props;
      const { id } = item;
      if (isTrashFolder) return;

      if (!isTabletView) {
        fetchFileVersions(id + "");
        setIsVerHistoryPanel(true);
      } else {
        history.push(
          combineUrl(AppServerConfig.proxyURL, homepage, `/${id}/history`)
        );
      }
    };

    finalizeVersion = () => {
      const { item, finalizeVersionAction } = this.props;
      const { id } = item;
      finalizeVersionAction(id).catch((err) => toastr.error(err));
    };

    onClickFavorite = (e) => {
      const { item, setFavoriteAction, t } = this.props;
      const { id } = item;
      const data = (e.currentTarget && e.currentTarget.dataset) || e;
      const { action } = data;

      setFavoriteAction(action, id)
        .then(() =>
          action === "mark"
            ? toastr.success(t("MarkedAsFavorite"))
            : toastr.success(t("RemovedFromFavorites"))
        )
        .catch((err) => toastr.error(err));
    };

    lockFile = () => {
      const { item, lockFileAction } = this.props;
      const { id, locked } = item;
      lockFileAction(id, !locked).catch((err) => toastr.error(err));
    };

    onClickLinkForPortal = () => {
      const { item, homepage, t } = this.props;
      const { fileExst, canOpenPlayer, webUrl, id } = item;

      const isFile = !!fileExst;
      copy(
        isFile
          ? canOpenPlayer
            ? `${window.location.href}&preview=${id}`
            : webUrl
          : `${window.location.origin + homepage}/filter?folder=${id}`
      );

      toastr.success(t("Translations:LinkCopySuccess"));
    };

    onClickLinkEdit = () => {
      const {
        item,
        canConvert,
        setConvertItem,
        setConvertDialogVisible,
      } = this.props;

      if (canConvert) {
        setConvertItem(item);
        setConvertDialogVisible(true);
      } else {
        this.openDocEditor(false);
      }
    };

    onPreviewClick = () => {
      this.openDocEditor(true);
    };

    openDocEditor = (preview = false) => {
      const { item, openDocEditor, isDesktop } = this.props;
      const { id, providerKey, fileExst } = item;

      const urlFormation = preview
        ? combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/doceditor?fileId=${id}&action=view`
          )
        : null;

      let tab =
        !isDesktop && fileExst
          ? window.open(
              combineUrl(
                AppServerConfig.proxyURL,
                config.homepage,
                "/doceditor"
              ),
              "_blank"
            )
          : null;

      openDocEditor(id, providerKey, tab, urlFormation);
    };
    onClickDownload = () => {
      const { item, downloadAction, t } = this.props;
      const { fileExst, contentLength, viewUrl } = item;
      const isFile = !!fileExst && contentLength;
      isFile
        ? window.open(viewUrl, "_self")
        : downloadAction(t("Translations:ArchivingData")).catch((err) =>
            toastr.error(err)
          );
    };

    onClickDownloadAs = () => {
      const { setDownloadDialogVisible } = this.props;
      setDownloadDialogVisible(true);
    };

    onDuplicate = () => {
      const { duplicateAction, t, item } = this.props;
      duplicateAction(item, t("Translations:CopyOperation")).catch((err) =>
        toastr.error(err)
      );
    };

    onClickRename = () => {
      const { item, setAction } = this.props;
      const { id, fileExst } = item;
      setAction({
        type: FileAction.Rename,
        extension: fileExst,
        id,
      });
    };

    onChangeThirdPartyInfo = () => {
      const { item, setThirdpartyInfo } = this.props;
      const { providerKey } = item;
      setThirdpartyInfo(providerKey);
    };

    onMediaFileClick = (fileId) => {
      const { item, setMediaViewerData } = this.props;
      const itemId = typeof fileId !== "object" ? fileId : item.id;
      setMediaViewerData({ visible: true, id: itemId });
    };

    onClickDelete = () => {
      const {
        item,
        setRemoveItem,
        setDeleteThirdPartyDialogVisible,
        t,
        deleteItemAction,
      } = this.props;
      const {
        id,
        title,
        fileExst,
        contentLength,
        folderId,
        providerKey,
        rootFolderId,
      } = item;
      const isRootThirdPartyFolder = providerKey && id === rootFolderId;

      if (isRootThirdPartyFolder) {
        const splitItem = id.split("-");
        setRemoveItem({ id: splitItem[splitItem.length - 1], title });
        setDeleteThirdPartyDialogVisible(true);
        return;
      }

      const translations = {
        deleteOperation: t("Translations:DeleteOperation"),
        successRemoveFile: t("FileRemoved"),
        successRemoveFolder: t("FolderRemoved"),
      };

      deleteItemAction(
        id,
        folderId,
        translations,
        fileExst || contentLength,
        providerKey
      );
    };

    onClickShare = () => {
      const { onSelectItem, setSharingPanelVisible, id, isFolder } = this.props;
      onSelectItem({ id, isFolder });
      setSharingPanelVisible(true);
    };

    onClickMarkRead = () => {
      const { markAsRead, item } = this.props;
      item.fileExst
        ? markAsRead([], [item.id], item)
        : markAsRead([item.id], [], item);
    };

    onClickUnsubscribe = () => {
      const { setDeleteDialogVisible, setUnsubscribe } = this.props;

      setUnsubscribe(true);
      setDeleteDialogVisible(true);
    };

    getFilesContextOptions = () => {
      const { item, t } = this.props;
      const { contextOptions } = item;
      const isRootThirdPartyFolder =
        item.providerKey && item.id === item.rootFolderId;

      const isShareable = item.canShare;

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
              icon: "/static/images/catalog.shared.react.svg",
              onClick: this.onClickShare,
              disabled: !isShareable,
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
              label: t("Translations:OwnerChange"),
              icon: "/static/images/catalog.user.react.svg",
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
              label: t("Common:EditButton"),
              icon: "/static/images/access.edit.react.svg",
              onClick: this.onClickLinkEdit,
              disabled: false,
            };
          case "preview":
            return {
              key: option,
              label: t("Preview"),
              icon: "/static/images/eye.react.svg",
              onClick: this.onPreviewClick,
              disabled: false,
            };
          case "view":
            return {
              key: option,
              label: t("Common:View"),
              icon: "/static/images/eye.react.svg",
              onClick: this.onMediaFileClick,
              disabled: false,
            };
          case "download":
            return {
              key: option,
              label: t("Common:Download"),
              icon: "images/download.react.svg",
              onClick: this.onClickDownload,
              disabled: false,
            };
          case "download-as":
            return {
              key: option,
              label: t("Translations:DownloadAs"),
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
              label: t("Translations:Restore"),
              icon: "images/move.react.svg",
              onClick: this.onMoveAction,
              disabled: false,
            };
          case "copy-to":
            return {
              key: option,
              label: t("Translations:Copy"),
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
              label: t("Translations:ThirdPartyInfo"),
              icon: "/static/images/access.edit.react.svg",
              onClick: this.onChangeThirdPartyInfo,
              disabled: false,
            };
          case "delete":
            return {
              key: option,
              label: isRootThirdPartyFolder
                ? t("Translations:DeleteThirdParty")
                : t("Common:Delete"),
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
          case "unsubscribe":
            return {
              key: option,
              label: t("RemoveFromList"),
              icon: "images/remove.svg",
              onClick: this.onClickUnsubscribe,
              disabled: false,
            };
          case "mark-read":
            return {
              key: option,
              label: t("MarkRead"),
              icon: "images/tick.rounded.svg",
              onClick: this.onClickMarkRead,
              disabled: false,
            };
          default:
            break;
        }

        return undefined;
      });
    };
    render() {
      const { actionType, actionId, actionExtension, item } = this.props;
      const { id, fileExst, contextOptions } = item;

      const isEdit =
        !!actionType && actionId === id && fileExst === actionExtension;

      const contextOptionsProps =
        !isEdit && contextOptions && contextOptions.length > 0
          ? {
              contextOptions: this.getFilesContextOptions(),
            }
          : {};

      return (
        <WrappedComponent
          contextOptionsProps={contextOptionsProps}
          {...this.props}
        />
      );
    }
  }

  return inject(
    (
      {
        filesStore,
        filesActionsStore,
        auth,
        versionHistoryStore,
        mediaViewerDataStore,
        dialogsStore,
        treeFoldersStore,
      },
      { item }
    ) => {
      const { openDocEditor, fileActionStore } = filesStore;
      const {
        openLocationAction,
        finalizeVersionAction,
        setFavoriteAction,
        lockFileAction,
        downloadAction,
        duplicateAction,
        setThirdpartyInfo,
        onSelectItem,
        deleteItemAction,
        markAsRead,
        unsubscribeAction,
      } = filesActionsStore;
      const {
        setChangeOwnerPanelVisible,
        setMoveToPanelVisible,
        setCopyPanelVisible,
        setDownloadDialogVisible,
        setRemoveItem,
        setDeleteThirdPartyDialogVisible,
        setSharingPanelVisible,
        setDeleteDialogVisible,
        setUnsubscribe,
      } = dialogsStore;
      const { isTabletView, isDesktopClient } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const { setAction, type, extension, id } = fileActionStore;
      const { setMediaViewerData } = mediaViewerDataStore;

      const { isRecycleBinFolder, isShare } = treeFoldersStore;
      const isShareFolder = isShare(item.rootFolderType);

      return {
        openLocationAction,
        setChangeOwnerPanelVisible,
        setMoveToPanelVisible,
        setCopyPanelVisible,
        isTabletView,
        setIsVerHistoryPanel,
        fetchFileVersions,
        homepage: config.homepage,
        finalizeVersionAction,
        setFavoriteAction,
        lockFileAction,
        openDocEditor,
        downloadAction,
        setDownloadDialogVisible,
        duplicateAction,
        setAction,
        setThirdpartyInfo,
        setMediaViewerData,
        setRemoveItem,
        setDeleteThirdPartyDialogVisible,
        deleteItemAction,
        onSelectItem,
        setSharingPanelVisible,
        actionType: type,
        actionId: id,
        actionExtension: extension,
        isTrashFolder: isRecycleBinFolder,
        isShareFolder,
        markAsRead,
        unsubscribeAction,
        setDeleteDialogVisible,
        setUnsubscribe,
        isDesktop: isDesktopClient,
      };
    }
  )(observer(WithContextOptions));
}
