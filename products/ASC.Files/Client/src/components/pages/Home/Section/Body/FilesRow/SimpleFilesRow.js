import React, { useCallback } from "react";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import DragAndDrop from "@appserver/components/drag-and-drop";
import Row from "@appserver/components/row";
import FilesRowContent from "./FilesRowContent";
import history from "@appserver/common/history";
import toastr from "studio/toastr";
import { FileAction } from "@appserver/common/constants";

const StyledSimpleFilesRow = styled(Row)`
  margin-top: -2px;
  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon {
    margin-right: 7px;
    margin-top: -1px;
  }

  .share-button:hover,
  .share-button-icon:hover {
    cursor: pointer;
    color: #657077;
    path {
      fill: #657077;
    }
  }
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media (max-width: 1312px) {
    .share-button {
      padding-top: 3px;
    }
  }

  .styled-element {
    margin-right: 1px;
    margin-bottom: 2px;
  }
`;

const EncryptedFileIcon = styled.div`
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: ${(props) => (props.isEdit ? "40px" : "12px")};
`;

const svgLoader = () => <div style={{ width: "24px" }}></div>;

const SimpleFilesRow = (props) => {
  const {
    t,
    item,
    sectionWidth,
    actionType,
    actionExtension,
    isPrivacy,
    isRecycleBin,
    dragging,
    //selected,
    //setSelected,
    checked,
    canShare,
    isFolder,
    draggable,
    isRootFolder,
    //setSelection,
    homepage,
    isTabletView,
    actionId,
    selectedFolderId,

    setSharingPanelVisible,
    setChangeOwnerPanelVisible,
    showDeleteThirdPartyDialog,
    setRemoveItem,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    openDocEditor,
    setIsVerHistoryPanel,
    setVerHistoryFileId,
    setAction,
    deleteFileAction,
    deleteFolderAction,
    lockFileAction,
    duplicateAction,
    finalizeVersionAction,
    setFavoriteAction,
    openLocationAction,
    selectRowAction,
    setThirdpartyInfo,
    setMediaViewerData,
    setDragging,
    startUpload,
    setShareItem,
  } = props;

  const {
    id,
    title,
    fileExst,
    shared,
    access,
    contextOptions,
    icon,
    providerKey,
    folderId,
    viewUrl,
    webUrl,
    canOpenPlayer,
    locked,
  } = item;

  let value = fileExst ? `file_${id}` : `folder_${id}`;
  value += draggable ? "_draggable" : "";

  const isThirdPartyFolder = providerKey && isRootFolder;

  const onContentRowSelect = (checked, file) => {
    if (!file) return;

    selectRowAction(checked, file);
  };

  const onClickShare = () => {
    setSharingPanelVisible(true);
    setShareItem(item);
  };
  const onOwnerChange = () => setChangeOwnerPanelVisible(true);
  const onMoveAction = () => setMoveToPanelVisible(true);
  const onCopyAction = () => setCopyPanelVisible(true);

  const getSharedButton = (shared) => {
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
        onClick={onClickShare}
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

  const getItemIcon = (isEdit) => {
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

  const onOpenLocation = () => {
    const locationId = isFolder ? id : folderId;
    openLocationAction(locationId, isFolder);
  };

  const showVersionHistory = () => {
    if (!isTabletView) {
      setVerHistoryFileId(id + "");
      setIsVerHistoryPanel(true);
    } else {
      history.push(`${homepage}/${id}/history`);
    }
  };

  const finalizeVersion = () =>
    finalizeVersionAction(id).catch((err) => toastr.error(err));

  const onClickFavorite = (e) => {
    const { action } = e.currentTarget.dataset;
    setFavoriteAction(action, id)
      .then(() =>
        action === "mark"
          ? toastr.success(t("MarkedAsFavorite"))
          : toastr.success(t("RemovedFromFavorites"))
      )
      .catch((err) => toastr.error(err));
  };

  const lockFile = () =>
    lockFileAction(id, !locked).catch((err) => toastr.error(err));

  const onClickLinkForPortal = () => {
    const isFile = !!fileExst;
    const { t } = this.props;
    copy(
      isFile
        ? canOpenPlayer
          ? `${window.location.href}&preview=${id}`
          : webUrl
        : `${window.location.origin + homepage}/filter?folder=${id}`
    );

    toastr.success(t("LinkCopySuccess"));
  };

  const onClickLinkEdit = () => openDocEditor(id, providerKey);

  const onClickDownload = () => window.open(viewUrl, "_blank");

  const onDuplicate = () =>
    duplicateAction(item, t("CopyOperation")).catch((err) => toastr.error(err));

  const onClickRename = () => {
    setAction({
      type: FileAction.Rename,
      extension: fileExst,
      id,
    });
  };

  const onChangeThirdPartyInfo = () => setThirdpartyInfo();

  const onMediaFileClick = (fileId) => {
    const itemId = typeof fileId !== "object" ? fileId : id;
    setMediaViewerData({ visible: true, id: itemId });
  };

  const onClickDelete = () => {
    if (isThirdPartyFolder) {
      const splitItem = id.split("-");
      setRemoveItem({ id: splitItem[splitItem.length - 1], title });
      showDeleteThirdPartyDialog(true);
      return;
    }

    const translations = {
      deleteOperation: t("DeleteOperation"),
    };

    item.fileExst
      ? deleteFileAction(item.id, item.folderId, translations)
          .then(() => toastr.success(t("FileRemoved")))
          .catch((err) => toastr.error(err))
      : deleteFolderAction(item.id, item.parentId, translations)
          .then(() => toastr.success(t("FolderRemoved")))
          .catch((err) => toastr.error(err));
  };

  const getFilesContextOptions = useCallback(() => {
    const isSharable = item.access !== 1 && item.access !== 0;

    return contextOptions.map((option) => {
      switch (option) {
        case "open":
          return {
            key: option,
            label: t("Open"),
            icon: "images/catalog.folder.react.svg",
            onClick: onOpenLocation,
            disabled: false,
          };
        case "show-version-history":
          return {
            key: option,
            label: t("ShowVersionHistory"),
            icon: "images/history.react.svg",
            onClick: showVersionHistory,
            disabled: false,
          };
        case "finalize-version":
          return {
            key: option,
            label: t("FinalizeVersion"),
            icon: "images/history-finalized.react.svg",
            onClick: finalizeVersion,
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
            onClick: onOpenLocation,
            disabled: false,
          };
        case "mark-as-favorite":
          return {
            key: option,
            label: t("MarkAsFavorite"),
            icon: "images/favorites.react.svg",
            onClick: onClickFavorite,
            disabled: false,
            "data-action": "mark",
          };
        case "block-unblock-version":
          return {
            key: option,
            label: t("UnblockVersion"),
            icon: "images/lock.react.svg",
            onClick: lockFile,
            disabled: false,
          };
        case "sharing-settings":
          return {
            key: option,
            label: t("SharingSettings"),
            icon: "images/catalog.shared.react.svg",
            onClick: onClickShare,
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
            onClick: onOwnerChange,
            disabled: false,
          };
        case "link-for-portal-users":
          return {
            key: option,
            label: t("LinkForPortalUsers"),
            icon: "/static/images/invitation.link.react.svg",
            onClick: onClickLinkForPortal,
            disabled: false,
          };
        case "edit":
          return {
            key: option,
            label: t("Edit"),
            icon: "images/access.edit.react.svg",
            onClick: onClickLinkEdit,
            disabled: false,
          };
        case "preview":
          return {
            key: option,
            label: t("Preview"),
            icon: "EyeIcon",
            onClick: onClickLinkEdit,
            disabled: true,
          };
        case "view":
          return {
            key: option,
            label: t("View"),
            icon: "/static/images/eye.react.svg",
            onClick: onMediaFileClick,
            disabled: false,
          };
        case "download":
          return {
            key: option,
            label: t("Download"),
            icon: "images/download.react.svg",
            onClick: onClickDownload,
            disabled: false,
          };
        case "move":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "images/move.react.svg",
            onClick: onMoveAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Copy"),
            icon: "/static/images/copy.react.svg",
            onClick: onCopyAction,
            disabled: false,
          };
        case "duplicate":
          return {
            key: option,
            label: t("Duplicate"),
            icon: "/static/images/copy.react.svg",
            onClick: onDuplicate,
            disabled: false,
          };
        case "rename":
          return {
            key: option,
            label: t("Rename"),
            icon: "images/rename.react.svg",
            onClick: onClickRename,
            disabled: false,
          };
        case "change-thirdparty-info":
          return {
            key: option,
            label: t("ThirdPartyInfo"),
            icon: "images/access.edit.react.svg",
            onClick: onChangeThirdPartyInfo,
            disabled: false,
          };
        case "delete":
          return {
            key: option,
            label: isThirdPartyFolder ? t("DeleteThirdParty") : t("Delete"),
            icon: "/static/images/catalog.trash.react.svg",
            onClick: onClickDelete,
            disabled: false,
          };
        case "remove-from-favorites":
          return {
            key: option,
            label: t("RemoveFromFavorites"),
            icon: "images/favorites.react.svg",
            onClick: onClickFavorite,
            disabled: false,
            "data-action": "remove",
          };
        default:
          break;
      }

      return undefined;
    });
  }, [contextOptions, item]);

  const onDropZoneUpload = (files, uploadToFolder) => {
    const folderId = uploadToFolder ? uploadToFolder : selectedFolderId;

    dragging && setDragging(false);
    startUpload(files, folderId, t);
  };

  const onDrop = (items) => {
    if (!fileExst) {
      onDropZoneUpload(items, item.id);
    } else {
      onDropZoneUpload(items, selectedFolderId);
    }
  };

  const isMobile = sectionWidth < 500;

  const isEdit =
    !!actionType && actionId === id && fileExst === actionExtension;

  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: getFilesContextOptions(),
        }
      : {};

  const checkedProps = isEdit || id <= 0 ? {} : { checked };
  const element = getItemIcon(isEdit || id <= 0);
  const displayShareButton = isMobile ? "26px" : !canShare ? "38px" : "96px";
  let className = isFolder && access < 2 && !isRecycleBin ? " dropable" : "";
  if (draggable) className += " draggable";

  const sharedButton =
    !canShare || (isPrivacy && !fileExst) || isEdit || id <= 0 || isMobile
      ? null
      : getSharedButton(shared);

  return (
    <DragAndDrop
      className={className}
      onDrop={onDrop}
      //onMouseDown={this.onMouseDown}
      dragging={dragging && isFolder && access < 2}
      {...contextOptionsProps}
      value={value}
    >
      <StyledSimpleFilesRow
        sectionWidth={sectionWidth}
        key={id}
        data={item}
        element={element}
        contentElement={sharedButton}
        onSelect={onContentRowSelect}
        isPrivacy={isPrivacy}
        {...checkedProps}
        {...contextOptionsProps}
        contextButtonSpacerWidth={displayShareButton}
      >
        <FilesRowContent item={item} sectionWidth={sectionWidth} />
      </StyledSimpleFilesRow>
    </DragAndDrop>
  );
};

export default inject(
  (
    {
      auth,
      initFilesStore,
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      dialogsStore,
      versionHistoryStore,
      filesActionsStore,
      mediaViewerDataStore,
      uploadDataStore,
    },
    { item }
  ) => {
    const { homepage, isTabletView } = auth.settingsStore;
    const { dragging, setDragging } = initFilesStore;
    const { type, extension, id } = filesStore.fileActionStore;
    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    const {
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setRemoveItem,
      showDeleteThirdPartyDialog,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setShareItem,
    } = dialogsStore;

    const {
      //selected,
      //setSelected,
      selection,
      canShare,
      //setSelection,
      openDocEditor,
      fileActionStore,
    } = filesStore;

    const { isRootFolder, id: selectedFolderId } = selectedFolderStore;
    const { setIsVerHistoryPanel, setVerHistoryFileId } = versionHistoryStore;
    const { setAction } = fileActionStore;

    const selectedItem = selection.find(
      (x) => x.id === item.id && x.fileExst === item.fileExst
    );

    const isFolder = selectedItem ? false : item.fileExst ? false : true;
    const draggable =
      !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

    const {
      deleteFileAction,
      deleteFolderAction,
      lockFileAction,
      finalizeVersionAction,
      duplicateAction,
      setFavoriteAction,
      openLocationAction,
      selectRowAction,
      setThirdpartyInfo,
    } = filesActionsStore;

    const { setMediaViewerData } = mediaViewerDataStore;
    const { startUpload } = uploadDataStore;

    return {
      dragging,
      actionType: type,
      actionExtension: extension,
      isPrivacy: isPrivacyFolder,
      isRecycleBin: isRecycleBinFolder,
      isRootFolder,
      canShare,
      //selected,
      //setSelected,
      //setSelection,
      checked: selection.some((el) => el.id === item.id),
      isFolder,
      draggable,
      isItemsSelected: !!selection.length,
      homepage,
      isTabletView,
      actionId: fileActionStore.id,
      setSharingPanelVisible,
      setShareItem,
      setChangeOwnerPanelVisible,
      setRemoveItem,
      showDeleteThirdPartyDialog,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      openDocEditor,
      setIsVerHistoryPanel,
      setVerHistoryFileId,
      setAction,
      deleteFileAction,
      deleteFolderAction,
      lockFileAction,
      finalizeVersionAction,
      duplicateAction,
      setFavoriteAction,
      openLocationAction,
      selectRowAction,
      setThirdpartyInfo,
      setMediaViewerData,
      selectedFolderId,
      setDragging,
      startUpload,
    };
  }
)(withTranslation("Home")(observer(SimpleFilesRow)));
