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
import history from "@appserver/common/history";
import toastr from "@appserver/components/toast";
import { FileAction } from "@appserver/common/constants";

import { lockFile, finalizeVersion } from "@appserver/common/api/files"; //TODO: move to actions

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
    isFavorites,
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
    homepage,
    isTabletView,
    filter,
    selectedFolderId,

    fetchFiles,
    setSharingPanelVisible,
    setChangeOwnerPanelVisible,
    setConnectDialogVisible,
    showDeleteThirdPartyDialog,
    setConnectItem,
    setRemoveItem,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    openDocEditor,
    setIsLoading,
    setIsVerHistoryPanel,
    setVerHistoryFileId,
    setAction,
    setSecondaryProgressBarData,
    markItemAsFavorite,
    removeItemFromFavorite,
    getFileInfo,
    fetchFavoritesFolder,
    actionId,
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
    folderId,
    viewUrl,
    webUrl,
    canOpenPlayer,
    locked,
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
    const locationId = isFolder ? id : folderId;
    const locationFilter = isFolder ? filter : null;

    return fetchFiles(locationId, locationFilter).then(() =>
      isFolder ? null : this.onContentRowSelect(!checked, item)
    );
  };

  const showVersionHistory = () => {
    if (!isTabletView) {
      setIsLoading(true);
      setVerHistoryFileId(id);
      setIsVerHistoryPanel(true);
    } else {
      history.push(`${homepage}/${id}/history`);
    }
  };

  const finalizeVersion = () => {
    setIsLoading(true);

    finalizeVersion(id, 0, false)
      .then(() => {
        return fetchFiles(selectedFolderId, filter).catch((err) =>
          toastr.error(err)
        );
      })
      .finally(() => setIsLoading(false));
  };

  const onClickFavorite = (e) => {
    const { action } = e.currentTarget.dataset;

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
    }
  };

  const lockFile = () => {
    //TODO: move lockFile to actions
    lockFile(id, locked).then((res) => {
      /*const newFiles = files;
        const indexOfFile = newFiles.findIndex(x => x.id === res.id);
        newFiles[indexOfFile] = res;*/
      setIsLoading(true);
      fetchFiles(selectedFolderId, filter)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    });
  };

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

  const onDuplicate = () => {
    const folderIds = [];
    const fileIds = [];
    fileExst ? fileIds.push(id) : folderIds.push(id);
    const conflictResolveType = 2; //Skip = 0, Overwrite = 1, Duplicate = 2 //TODO: get from settings
    const deleteAfter = false;

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label: t("CopyOperation"),
      alert: false,
    });

    //TODO: need add to action
    // this.copyTo(
    //   selectedFolderId,
    //   folderIds,
    //   fileIds,
    //   conflictResolveType,
    //   deleteAfter
    // );
  };

  const onClickRename = () => {
    setAction({
      type: FileAction.Rename,
      extension: fileExst,
      id,
    });
  };

  const onChangeThirdPartyInfo = () => {
    const provider = providers.find((x) => x.provider_key === providerKey);
    const capabilityItem = capabilities.find((x) => x[0] === providerKey);
    const capability = {
      title: capabilityItem ? capabilityItem[0] : provider.customer_title,
      link: capabilityItem ? capabilityItem[1] : " ",
    };

    setConnectDialogVisible(true);
    setConnectItem({ ...provider, ...capability });
  };

  const onClickDelete = () => {
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
          };
        case "finalize-version":
          return {
            key: option,
            label: t("FinalizeVersion"),
            icon: "HistoryFinalizedIcon",
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

  const isMobile = sectionWidth < 500;

  const isEdit =
    !!actionType && actionId === id && fileExst === actionExtension;

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
      auth,
      initFilesStore,
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      dialogsStore,
      settingsStore,
      versionHistoryStore,
      uploadDataStore,
    },
    { item }
  ) => {
    const { homepage, isTabletView } = auth.settingsStore;
    const { dragging, setIsLoading } = initFilesStore;
    const { type, extension, id } = filesStore.fileActionStore;
    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isFavoritesFolder,
    } = treeFoldersStore;

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
      openDocEditor,
      fetchFiles,
      filter,
      fileActionStore,
      markItemAsFavorite,
      removeItemFromFavorite,
      getFileInfo,
      fetchFavoritesFolder,
    } = filesStore;

    const { isRootFolder } = selectedFolderStore;
    const { providers, capabilities } = settingsStore.thirdPartyStore;
    const { setIsVerHistoryPanel, setVerHistoryFileId } = versionHistoryStore;
    const { setAction } = fileActionStore;
    const {
      setSecondaryProgressBarData,
    } = uploadDataStore.secondaryProgressDataStore;

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
      isFavorites: isFavoritesFolder,
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
      homepage,
      isTabletView,
      filter,
      selectedFolderId: selectedFolderStore.id,
      actionId: fileActionStore.id,

      fetchFiles,
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setConnectDialogVisible,
      setConnectItem,
      setRemoveItem,
      showDeleteThirdPartyDialog,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      openDocEditor,
      setIsLoading,
      setIsVerHistoryPanel,
      setVerHistoryFileId,
      setAction,
      setSecondaryProgressBarData,
      markItemAsFavorite,
      removeItemFromFavorite,
      getFileInfo,
      fetchFavoritesFolder,
    };
  }
)(withTranslation()(observer(SimpleFilesRow)));
