import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import copy from "copy-to-clipboard";

import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { combineUrl } from "@appserver/common/utils";
import { FileAction, AppServerConfig } from "@appserver/common/constants";
import toastr from "studio/toastr";

import { EncryptedFileIcon } from "../sub-components/icons";
import config from "../../../../../../../package.json";

const svgLoader = () => <div style={{ width: "24px" }}></div>;
export default function withFileActions(WrappedFileItem) {
  class WithFileActions extends React.Component {
    onContentRowSelect = (checked, file) => {
      const { selectRowAction } = this.props;
      if (!file) return;
      selectRowAction(checked, file);
    };

    onClickShare = () => {
      const { onSelectItem, setSharingPanelVisible, item } = this.props;
      onSelectItem(item);
      setSharingPanelVisible(true);
    };

    rowContextClick = () => {
      const { onSelectItem, item } = this.props;
      onSelectItem(item);
    };

    getSharedButton = (shared) => {
      const { t } = this.props;
      const color = shared ? "#657077" : "#a3a9ae";
      return (
        <Text
          className="share-button"
          as="span"
          title={t("Share")}
          fontSize="12px"
          fontWeight={600}
          color={color}
          display="inline-flex"
          onClick={this.onClickShare}
        >
          <IconButton
            className="share-button-icon"
            color={color}
            hoverColor="#657077"
            size={18}
            iconName="images/catalog.shared.react.svg"
          />
          {t("Share")}
        </Text>
      );
    };

    getItemIcon = (isEdit) => {
      const { item, isPrivacy } = this.props;
      const { icon, fileExst } = item;
      return (
        <>
          <ReactSVG
            className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
            src={icon}
            loading={svgLoader}
          />
          {isPrivacy && fileExst && <EncryptedFileIcon isEdit={isEdit} />}
        </>
      );
    };

    onDropZoneUpload = (files, uploadToFolder) => {
      const {
        selectedFolderId,
        dragging,
        setDragging,
        startUpload,
      } = this.props;

      const folderId = uploadToFolder ? uploadToFolder : selectedFolderId;
      dragging && setDragging(false);
      startUpload(files, folderId, t);
    };

    onDrop = (items) => {
      const { item, selectedFolderId } = this.props;
      const { fileExst, id } = item;

      if (!fileExst) {
        this.onDropZoneUpload(items, id);
      } else {
        this.onDropZoneUpload(items, selectedFolderId);
      }
    };

    onMouseDown = (e) => {
      const { draggable, setTooltipPosition, setStartDrag } = this.props;
      if (!draggable) {
        return;
      }

      if (
        window.innerWidth < 1025 ||
        e.target.tagName === "rect" ||
        e.target.tagName === "path"
      ) {
        return;
      }
      const mouseButton = e.which
        ? e.which !== 1
        : e.button
        ? e.button !== 0
        : false;
      const label = e.currentTarget.getAttribute("label");
      if (mouseButton || e.currentTarget.tagName !== "DIV" || label) {
        return;
      }

      setTooltipPosition(e.pageX, e.pageY);
      setStartDrag(true);
    };

    onOpenLocation = () => {
      const { isFolder, item, openLocationAction } = this.props;
      const { id, folderId, fileExst } = item;

      const locationId = !fileExst ? id : folderId;
      console.log(locationId, id, folderId, isFolder);
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
      } = this.props;
      const { id } = item;

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
      const { fileExst, canOpenPlayer, webUrl } = item;

      const isFile = !!fileExst;
      copy(
        isFile
          ? canOpenPlayer
            ? `${window.location.href}&preview=${id}`
            : webUrl
          : `${window.location.origin + homepage}/filter?folder=${id}`
      );

      toastr.success(t("LinkCopySuccess"));
    };

    onClickLinkEdit = () => {
      const { item, openDocEditor } = this.props;
      const { id, providerKey } = item;
      openDocEditor(id, providerKey);
    };

    onClickDownload = () => {
      const { item, downloadAction, t } = this.props;
      const { fileExst, contentLength, viewUrl } = item;
      const isFile = !!fileExst && contentLength;
      isFile
        ? window.open(viewUrl, "_blank")
        : downloadAction(t("ArchivingData")).catch((err) => toastr.error(err));
    };

    onClickDownloadAs = () => {
      const { setDownloadDialogVisible } = this.props;
      setDownloadDialogVisible(true);
    };

    onDuplicate = () => {
      const { duplicateAction, t, item } = this.props;
      duplicateAction(item, t("CopyOperation")).catch((err) =>
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
        confirmDelete,
        setDeleteDialogVisible,
        t,
        deleteFileAction,
        deleteFolderAction,
        isThirdPartyFolder,
      } = this.props;
      const { id, title, fileExst, contentLength, folderId, parentId } = item;

      if (isThirdPartyFolder) {
        const splitItem = id.split("-");
        setRemoveItem({ id: splitItem[splitItem.length - 1], title });
        setDeleteThirdPartyDialogVisible(true);
        return;
      }

      if (confirmDelete) {
        setDeleteDialogVisible(true);
      } else {
        const translations = {
          deleteOperation: t("DeleteOperation"),
        };

        fileExst || contentLength
          ? deleteFileAction(id, folderId, translations)
              .then(() => toastr.success(t("FileRemoved")))
              .catch((err) => toastr.error(err))
          : deleteFolderAction(id, parentId, translations)
              .then(() => toastr.success(t("FolderRemoved")))
              .catch((err) => toastr.error(err));
      }
    };

    getFilesContextOptions = () => {
      const { item, t, isThirdPartyFolder } = this.props;
      const { access, contextOptions } = item;
      const isSharable = access !== 1 && access !== 0;
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
              icon: "/static/images/access.edit.react.svg",
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
              icon: "/static/images/access.edit.react.svg",
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

    render() {
      const {
        item,
        isRecycleBin,
        draggable,
        canShare,
        isPrivacy,
        actionType,
        actionExtension,
        actionId,
        sectionWidth,
        checked,
        dragging,
      } = this.props;
      const {
        fileExst,
        access,
        contentLength,
        id,
        shared,
        contextOptions,
      } = item;

      const isEdit =
        !!actionType && actionId === id && fileExst === actionExtension;

      const isDragging = !fileExst && access < 2 && !isRecycleBin;

      let className = isDragging ? " droppable" : "";
      if (draggable) className += " draggable not-selectable";

      let value = fileExst || contentLength ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : "";

      const isMobile = sectionWidth < 500;
      const displayShareButton = isMobile
        ? "26px"
        : !canShare
        ? "38px"
        : "96px";

      const sharedButton =
        !canShare || (isPrivacy && !fileExst) || isEdit || id <= 0 || isMobile
          ? null
          : this.getSharedButton(shared);

      const contextOptionsProps =
        !isEdit && contextOptions && contextOptions.length > 0
          ? {
              contextOptions: this.getFilesContextOptions(),
            }
          : {};

      const checkedProps = isEdit || id <= 0 ? {} : { checked };
      const element = this.getItemIcon(isEdit || id <= 0);

      return (
        <WrappedFileItem
          onContentRowSelect={this.onContentRowSelect}
          onClickShare={this.onClickShare}
          rowContextClick={this.rowContextClick}
          onDrop={this.onDrop}
          onMouseDown={this.onMouseDown}
          getClassName={this.getClassName}
          className={className}
          isDragging={isDragging}
          value={value}
          displayShareButton={displayShareButton}
          isPrivacy={isPrivacy}
          sharedButton={sharedButton}
          contextOptionsProps={contextOptionsProps}
          checkedProps={checkedProps}
          element={element}
          dragging={dragging}
          {...this.props}
        />
      );
    }
  }

  return inject(
    (
      {
        filesActionsStore,
        dialogsStore,
        treeFoldersStore,
        selectedFolderStore,
        filesStore,
        uploadDataStore,
        auth,
        mediaViewerDataStore,
        settingsStore,
        versionHistoryStore,
      },
      { item, t, history }
    ) => {
      const {
        selectRowAction,
        onSelectItem,
        openLocationAction,
        finalizeVersionAction,
        setFavoriteAction,
        lockFileAction,
        downloadAction,
        duplicateAction,
        setThirdpartyInfo,
        deleteFileAction,
        deleteFolderAction,
      } = filesActionsStore;
      const {
        setSharingPanelVisible,
        setChangeOwnerPanelVisible,
        setMoveToPanelVisible,
        setCopyPanelVisible,
        setDownloadDialogVisible,
        setRemoveItem,
        setDeleteThirdPartyDialogVisible,
        setDeleteDialogVisible,
      } = dialogsStore;
      const { isPrivacyFolder, isRecycleBinFolder } = treeFoldersStore;
      const { id: selectedFolderId, isRootFolder } = selectedFolderStore;
      const {
        dragging,
        setDragging,
        selection,
        setTooltipPosition,
        setStartDrag,
        openDocEditor,
        fileActionStore,
        canShare,
        isFileSelected,
      } = filesStore;
      const { setAction } = fileActionStore;
      const { startUpload } = uploadDataStore;
      const { type, extension, id } = filesStore.fileActionStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const { isTabletView } = auth.settingsStore;
      const { setMediaViewerData } = mediaViewerDataStore;

      const selectedItem = selection.find(
        (x) => x.id === item.id && x.fileExst === item.fileExst
      );

      const draggable =
        !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

      const isFolder = selectedItem
        ? false
        : item.fileExst || item.contentLength
        ? false
        : true;

      const isThirdPartyFolder = item.providerKey && isRootFolder;

      return {
        t,
        item,
        selectRowAction,
        onSelectItem,
        setSharingPanelVisible,
        isPrivacy: isPrivacyFolder,
        selectedFolderId,
        dragging,
        setDragging,
        startUpload,
        draggable,
        setTooltipPosition,
        setStartDrag,
        history,
        isFolder,
        openLocationAction,
        setChangeOwnerPanelVisible,
        setMoveToPanelVisible,
        setCopyPanelVisible,
        isTabletView,
        fetchFileVersions,
        setIsVerHistoryPanel,
        homepage: config.homepage,
        finalizeVersionAction,
        setFavoriteAction,
        lockFileAction,
        openDocEditor,
        setDownloadDialogVisible,
        downloadAction,
        duplicateAction,
        setAction,
        setThirdpartyInfo,
        setMediaViewerData,
        isRootFolder,
        setRemoveItem,
        setDeleteThirdPartyDialogVisible,
        confirmDelete: settingsStore.confirmDelete,
        setDeleteDialogVisible,
        deleteFileAction,
        deleteFolderAction,
        isThirdPartyFolder,
        canShare,
        actionType: type,
        actionExtension: extension,
        actionId: fileActionStore.id,
        checked: isFileSelected(item.id, item.parentId),
      };
    }
  )(observer(WithFileActions));
}
