import React from "react";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import DragAndDrop from "@appserver/components/drag-and-drop";
import Row from "@appserver/components/row";
import FilesRowContent from "./FilesRowContent";

import { lockFile } from "@appserver/common/api/files"; //TODO: move to actions

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
    selected,
    setSelected,
    selectFile,
    deselectFile,
    checked,
    canShare,
    isFolder,
    draggable,
    isRootFolder,
    setSelection,
    providers,
    capabilities,

    setSharingPanelVisible,
    setChangeOwnerPanelVisible,
    setConnectDialogVisible,
    showDeleteThirdPartyDialog,
    setConnectItem,
    setRemoveItem,
    setMoveToPanelVisible,
    setCopyPanelVisible,
  } = props;

  const {
    id,
    title,
    fileExst,
    shared,
    access,
    value,
    contextOptions,
    icon,
    providerKey,
  } = item;

  const isThirdPartyFolder = providerKey && isRootFolder;

  const onContentRowSelect = (checked, file) => {
    if (!file) return;

    selected === "close" && setSelected("none");
    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  const onClickShare = () => setSharingPanelVisible(true);
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
          iconName="CatalogSharedIcon"
        />
        {t("Share")}
      </Text>
    );
  };

  const getItemIcon = (isEdit) => {
    return (
      <>
        <ReactSVG
          beforeInjection={(svg) => {
            svg.setAttribute("style", "margin-top: 4px");
            isEdit && svg.setAttribute("style", "margin: 4px 0 0 28px");
          }}
          src={icon}
          loading={svgLoader}
        />
        {isPrivacy && fileExst && <EncryptedFileIcon isEdit={isEdit} />}
      </>
    );
  };

  const onOpenLocation = () => {
    /*const { filter, selection } = this.props;
    const { folderId, checked, id, isFolder } = selection[0];
    const item = selection[0];
    const locationId = isFolder ? id : folderId;
    const locationFilter = isFolder ? filter : null;

    return this.props
      .fetchFiles(locationId, locationFilter)
      .then(() => (isFolder ? null : this.onContentRowSelect(!checked, item)));*/
  };

  const showVersionHistory = (e) => {
    /*const {
      homepage,
      history,
      setIsLoading,
      setIsVerHistoryPanel,
      setVerHistoryFileId,
      isTabletView,
    } = this.props;

    const fileId = e.currentTarget.dataset.id;

    if (!isTabletView) {
      setIsLoading(true);
      setVerHistoryFileId(fileId);
      setIsVerHistoryPanel(true);
    } else {
      history.push(`${homepage}/${fileId}/history`);
    }*/
  };

  const finalizeVersion = (e) => {
    /*const { selectedFolderId, filter, setIsLoading, fetchFiles } = this.props;

    const fileId = e.currentTarget.dataset.id;
    //const version = (e.currentTarget.dataset.version)++;

    setIsLoading(true);

    finalizeVersion(fileId, 0, false)
      .then((data) => {
        return fetchFiles(selectedFolderId, filter).catch((err) =>
          toastr.error(err)
        );
      })
      .finally(() => setIsLoading(false));*/
  };

  const onClickFavorite = (e) => {
    /*const {
      markItemAsFavorite,
      removeItemFromFavorite,
      getFileInfo,
      fetchFavoritesFolder,
      isFavorites,
      selectedFolderId,
      setSelected,
      //selection,
      t,
    } = this.props;
    const { action, id } = e.currentTarget.dataset;
    //let data = selection.map(item => item.id)
    switch (action) {
      case "mark":
        return markItemAsFavorite([id])
          .then(() => getFileInfo(id))
          .then(() => toastr.success(t("MarkedAsFavorite")))
          .catch((e) => toastr.error(e));
      case "remove":
        return removeItemFromFavorite([id])
          .then(() => {
            return isFavorites
              ? fetchFavoritesFolder(selectedFolderId)
              : getFileInfo(id);
          })
          .then(() => toastr.success(t("RemovedFromFavorites")))
          .then(() => setSelected("close"))
          .catch((e) => toastr.error(e));
      default:
        return;
    }*/
  };

  const lockFile = (e) => {
    // const {
    //   selection,
    //   /*files,*/ selectedFolderId,
    //   filter,
    //   setIsLoading,
    //   fetchFiles,
    // } = this.props;
    // let fileId, isLockedFile;
    // const file = selection[0];
    // if (file) {
    //   fileId = file.id;
    //   isLockedFile = !file.locked;
    // } else {
    //   const { id, locked } = e.currentTarget.dataset;
    //   fileId = Number(id);
    //   isLockedFile = !Boolean(locked);
    // }
    // //TODO: move lockFile to actions
    // lockFile(fileId, isLockedFile).then((res) => {
    //   /*const newFiles = files;
    //     const indexOfFile = newFiles.findIndex(x => x.id === res.id);
    //     newFiles[indexOfFile] = res;*/
    //   setIsLoading(true);
    //   fetchFiles(selectedFolderId, filter)
    //     .catch((err) => toastr.error(err))
    //     .finally(() => setIsLoading(false));
    // });
  };

  const onClickLinkForPortal = () => {
    /*const { homepage, selection } = this.props;
    const item = selection[0];
    const isFile = !!item.fileExst;
    const { t } = this.props;
    copy(
      isFile
        ? item.canOpenPlayer
          ? `${window.location.href}&preview=${item.id}`
          : item.webUrl
        : `${window.location.origin + homepage}/filter?folder=${item.id}`
    );

    toastr.success(t("LinkCopySuccess"));*/
  };

  const onClickLinkEdit = (e) => {
    // const { id, providerKey } = e.currentTarget.dataset;
    // return this.openDocEditor(id, providerKey);
  };

  const onClickDownload = () => {
    // return window.open(this.props.selection[0].viewUrl, "_blank");
  };

  const onDuplicate = () => {
    /*const {
      selection,
      selectedFolderId,
      setSecondaryProgressBarData,
      t,
    } = this.props;
    const folderIds = [];
    const fileIds = [];
    selection[0].fileExst
      ? fileIds.push(selection[0].id)
      : folderIds.push(selection[0].id);
    const conflictResolveType = 2; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = false;

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label: t("CopyOperation"),
      alert: false,
    });
    this.copyTo(
      selectedFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );*/
  };

  const onClickRename = () => {
    /*const { id, fileExst } = this.props.selection[0];

    this.setState({ editingId: id }, () => {
      this.props.setAction({
        type: FileAction.Rename,
        extension: fileExst,
        id,
      });
    });*/
  };

  const onChangeThirdPartyInfo = (e) => {
    const provider = providers.find((x) => x.provider_key === providerKey);
    const capabilityItem = capabilities.find((x) => x[0] === providerKey);
    const capability = {
      title: capabilityItem ? capabilityItem[0] : provider.customer_title,
      link: capabilityItem ? capabilityItem[1] : " ",
    };

    setConnectDialogVisible(true);
    setConnectItem({ ...provider, ...capability });
  };

  const onClickDelete = (e) => {
    const splitItem = id.split("-");

    if (isThirdPartyFolder) {
      setRemoveItem({ id: splitItem[splitItem.length - 1], title });
      showDeleteThirdPartyDialog(true);
      return;
    }

    const item = this.props.selection[0];
    item.fileExst
      ? this.onDeleteFile(item.id, item.folderId)
      : this.onDeleteFolder(item.id, item.parentId);
  };

  const getFilesContextOptions = (options, item) => {
    const isSharable = item.access !== 1 && item.access !== 0;

    return options.map((option) => {
      switch (option) {
        case "open":
          return {
            key: option,
            label: t("Open"),
            icon: "CatalogFolderIcon",
            onClick: onOpenLocation,
            disabled: false,
          };
        case "show-version-history":
          return {
            key: option,
            label: t("ShowVersionHistory"),
            icon: "HistoryIcon",
            onClick: showVersionHistory,
            disabled: false,
            "data-id": item.id,
          };
        case "finalize-version":
          return {
            key: option,
            label: t("FinalizeVersion"),
            icon: "HistoryFinalizedIcon",
            onClick: finalizeVersion,
            disabled: false,
            "data-id": item.id,
            "data-version": item.version,
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
            icon: "DownloadAsIcon",
            onClick: onOpenLocation,
            disabled: false,
          };
        case "mark-as-favorite":
          return {
            key: option,
            label: t("MarkAsFavorite"),
            icon: "FavoritesIcon",
            onClick: onClickFavorite,
            disabled: false,
            "data-action": "mark",
            "data-id": item.id,
            "data-title": item.title,
          };
        case "block-unblock-version":
          return {
            key: option,
            label: t("UnblockVersion"),
            icon: "LockIcon",
            onClick: lockFile,
            disabled: false,
          };
        case "sharing-settings":
          return {
            key: option,
            label: t("SharingSettings"),
            icon: "CatalogSharedIcon",
            onClick: onClickShare,
            disabled: isSharable,
          };
        case "send-by-email":
          return {
            key: option,
            label: t("SendByEmail"),
            icon: "MailIcon",
            disabled: true,
          };
        case "owner-change":
          return {
            key: option,
            label: t("ChangeOwner"),
            icon: "CatalogUserIcon",
            onClick: onOwnerChange,
            disabled: false,
          };
        case "link-for-portal-users":
          return {
            key: option,
            label: t("LinkForPortalUsers"),
            icon: "InvitationLinkIcon",
            onClick: onClickLinkForPortal,
            disabled: false,
          };
        case "edit":
          return {
            key: option,
            label: t("Edit"),
            icon: "AccessEditIcon",
            onClick: onClickLinkEdit,
            disabled: false,
            "data-id": item.id,
            "data-provider-key": item.providerKey,
          };
        case "preview":
          return {
            key: option,
            label: t("Preview"),
            icon: "EyeIcon",
            onClick: onClickLinkEdit,
            disabled: true,
            "data-id": item.id,
            "data-provider-key": item.providerKey,
          };
        case "view":
          return {
            key: option,
            label: t("View"),
            icon: "EyeIcon",
            //onClick: this.onMediaFileClick,
            disabled: false,
          };
        case "download":
          return {
            key: option,
            label: t("Download"),
            icon: "DownloadIcon",
            onClick: onClickDownload,
            disabled: false,
          };
        case "move":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "MoveToIcon",
            onClick: onMoveAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Copy"),
            icon: "CopyIcon",
            onClick: onCopyAction,
            disabled: false,
          };
        case "duplicate":
          return {
            key: option,
            label: t("Duplicate"),
            icon: "CopyIcon",
            onClick: onDuplicate,
            disabled: false,
          };
        case "rename":
          return {
            key: option,
            label: t("Rename"),
            icon: "RenameIcon",
            onClick: onClickRename,
            disabled: false,
          };
        case "change-thirdparty-info":
          return {
            key: option,
            label: t("ThirdPartyInfo"),
            icon: "AccessEditIcon",
            onClick: onChangeThirdPartyInfo,
            disabled: false,
          };
        case "delete":
          return {
            key: option,
            label: isThirdPartyFolder ? t("DeleteThirdParty") : t("Delete"),
            icon: "CatalogTrashIcon",
            onClick: onClickDelete,
            disabled: false,
          };
        case "remove-from-favorites":
          return {
            key: option,
            label: t("RemoveFromFavorites"),
            icon: "FavoritesIcon",
            onClick: onClickFavorite,
            disabled: false,
            "data-action": "remove",
            "data-id": item.id,
            "data-title": item.title,
          };
        default:
          break;
      }

      return undefined;
    });
  };

  const onSelectItem = () => {
    selected === "close" && setSelected("none");
    setSelection([item]);
  };

  const editingId = null; // TODO: move editingId to store

  const isMobile = sectionWidth < 500;

  const isEdit =
    !!actionType && editingId === id && fileExst === actionExtension;

  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: getFilesContextOptions(contextOptions, item),
        }
      : {};

  const checkedProps = isEdit || id <= 0 ? {} : { checked };

  const element = getItemIcon(isEdit || id <= 0);

  const sharedButton =
    !canShare || (isPrivacy && !fileExst) || isEdit || id <= 0 || isMobile
      ? null
      : getSharedButton(shared);

  const displayShareButton = isMobile ? "26px" : !canShare ? "38px" : "96px";

  let className = isFolder && access < 2 && !isRecycleBin ? " dropable" : "";
  if (draggable) className += " draggable";

  return (
    <DragAndDrop
      className={className}
      //onDrop={this.onDrop.bind(this, item)}
      //onMouseDown={this.onMouseDown}
      dragging={dragging && isFolder && access < 2}
      {...contextOptionsProps}
      value={value}
      //{...props}
    >
      <StyledSimpleFilesRow
        sectionWidth={sectionWidth}
        key={id}
        data={item}
        element={element}
        contentElement={sharedButton}
        onSelect={onContentRowSelect}
        editing={editingId}
        isPrivacy={isPrivacy}
        {...checkedProps}
        {...contextOptionsProps}
        //needForUpdate={this.needForUpdate}
        selectItem={onSelectItem}
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
      initFilesStore,
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      dialogsStore,
      settingsStore,
    },
    { item }
  ) => {
    const { dragging } = initFilesStore;
    const { type, extension, id } = filesStore.fileActionStore;
    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
    const {
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setConnectDialogVisible,
      setConnectItem,
      setRemoveItem,
      showDeleteThirdPartyDialog,
      setMoveToPanelVisible,
      setCopyPanelVisible,
    } = dialogsStore;

    const {
      selected,
      setSelected,
      selectFile,
      deselectFile,
      selection,
      canShare,
      setSelection,
    } = filesStore;

    const { isRootFolder } = selectedFolderStore;

    const { providers, capabilities } = settingsStore.thirdPartyStore;

    const selectedItem = selection.find(
      (x) => x.id === item.id && x.fileExst === item.fileExst
    );

    const isFolder = selectedItem ? false : item.fileExst ? false : true;
    const draggable =
      selectedItem && isRecycleBinFolder && selectedItem.id !== id;

    return {
      dragging,
      actionType: type,
      actionExtension: extension,
      isPrivacy: isPrivacyFolder,
      isRecycleBin: isRecycleBinFolder,
      isRootFolder,
      canShare,
      selected,
      setSelected,
      selectFile,
      deselectFile,
      setSelection,
      checked: selection.some((el) => el.id === item.id),
      isFolder,
      draggable,
      providers,
      capabilities,

      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setConnectDialogVisible,
      setConnectItem,
      setRemoveItem,
      showDeleteThirdPartyDialog,
      setMoveToPanelVisible,
      setCopyPanelVisible,
    };
  }
)(withTranslation()(observer(SimpleFilesRow)));
