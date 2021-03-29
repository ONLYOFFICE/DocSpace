import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./FilesRow/FilesRowContainer";
import FilesTileContainer from "./FilesTile/FilesTileContainer";
import EmptyContainer from "./EmptyContainer";

import { AppServerConfig, FileAction } from "@appserver/common/constants";
//import copy from "copy-to-clipboard";
import config from "../../../../../../package.json";
import toastr from "studio/toastr";
import copy from "copy-to-clipboard";
import { combineUrl } from "@appserver/common/utils";
import { ReactSVG } from "react-svg";

const backgroundDragColor = "#EFEFB2";

const backgroundDragEnterColor = "#F8F7BF";

const CustomTooltip = styled.div`
  position: fixed;
  display: none;
  padding: 8px;
  z-index: 150;
  background: #fff;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  -moz-border-radius: 6px;
  -webkit-border-radius: 6px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);

  .tooltip-moved-obj-wrapper {
    display: flex;
    align-items: center;
  }
  .tooltip-moved-obj-icon {
    margin-right: 6px;
  }
  .tooltip-moved-obj-extension {
    color: #a3a9ae;
  }
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    // this.state = {
    //   isDrag: false,
    //   canDrag: true,
    // };

    // this.tooltipRef = React.createRef();
    // this.currentDroppable = null;
  }

  componentDidMount() {
    this.customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    // window.addEventListener("mouseup", this.onMouseUp);
    // document.addEventListener("dragstart", this.onDragStart);
    // document.addEventListener("dragover", this.onDragOver);
    // document.addEventListener("dragleave", this.onDragLeaveDoc);
    // document.addEventListener("drop", this.onDropEvent);
  }

  // componentWillUnmount() {
  //   window.removeEventListener("mouseup", this.onMouseUp);
  //   document.addEventListener("dragstart", this.onDragStart);
  //   document.removeEventListener("dragover", this.onDragOver);
  //   document.removeEventListener("dragleave", this.onDragLeaveDoc);
  //   document.removeEventListener("drop", this.onDropEvent);
  // }

  componentDidUpdate(prevProps, prevState) {
    Object.entries(this.props).forEach(
      ([key, val]) =>
        prevProps[key] !== val && console.log(`Prop '${key}' changed`)
    );
    if (this.state) {
      Object.entries(this.state).forEach(
        ([key, val]) =>
          prevState[key] !== val && console.log(`State '${key}' changed`)
      );
    }
  }

  componentDidUpdate(prevProps) {
    const { folderId } = this.props;

    if (isMobile) {
      if (folderId !== prevProps.folderId) {
        this.customScrollElm && this.customScrollElm.scrollTo(0, 0);
      }
    }
  }

  // onDragStart = (e) => {
  //   if (e.dataTransfer.dropEffect === "none") {
  //     this.state.canDrag && this.setState({ canDrag: false });
  //   }
  // };

  // onDropEvent = () => {
  //   this.props.dragging && this.props.setDragging(false);
  // };

  // onDragOver = (e) => {
  //   e.preventDefault();
  //   const { dragging, setDragging } = this.props;
  //   if (e.dataTransfer.items.length > 0 && !dragging && this.state.canDrag) {
  //     setDragging(true);
  //   }
  // };

  // onDragLeaveDoc = (e) => {
  //   e.preventDefault();
  //   const { dragging, setDragging } = this.props;
  //   if (dragging && !e.relatedTarget) {
  //     setDragging(false);
  //   }
  // };

  // onMouseDown = (e) => {
  //   if (
  //     window.innerWidth < 1025 ||
  //     e.target.tagName === "rect" ||
  //     e.target.tagName === "path"
  //   ) {
  //     return;
  //   }
  //   const mouseButton = e.which
  //     ? e.which !== 1
  //     : e.button
  //     ? e.button !== 0
  //     : false;
  //   const label = e.currentTarget.getAttribute("label");
  //   if (mouseButton || e.currentTarget.tagName !== "DIV" || label) {
  //     return;
  //   }
  //   document.addEventListener("mousemove", this.onMouseMove);
  //   this.setTooltipPosition(e);
  //   const { selection } = this.props;

  //   const elem = e.currentTarget.closest(".draggable");
  //   if (!elem) {
  //     return;
  //   }
  //   const value = elem.getAttribute("value");
  //   if (!value) {
  //     return;
  //   }
  //   let splitValue = value.split("_");
  //   let item = null;
  //   if (splitValue[0] === "folder") {
  //     splitValue.splice(0, 1);
  //     if (splitValue[splitValue.length - 1] === "draggable") {
  //       splitValue.splice(-1, 1);
  //     }
  //     splitValue = splitValue.join("_");

  //     item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
  //   } else {
  //     splitValue.splice(0, 1);
  //     if (splitValue[splitValue.length - 1] === "draggable") {
  //       splitValue.splice(-1, 1);
  //     }
  //     splitValue = splitValue.join("_");

  //     item = selection.find((x) => x.id + "" === splitValue && x.fileExst);
  //   }
  //   if (item) {
  //     this.setState({ isDrag: true });
  //   }
  // };

  // onMouseUp = (e) => {
  //   const { selection, dragging, setDragging, dragItem } = this.props;

  //   document.body.classList.remove("drag-cursor");

  //   if (this.state.isDrag || !this.state.canDrag) {
  //     this.setState({ isDrag: false, canDrag: true });
  //   }
  //   const mouseButton = e.which
  //     ? e.which !== 1
  //     : e.button
  //     ? e.button !== 0
  //     : false;
  //   if (mouseButton || !this.tooltipRef.current || !dragging) {
  //     return;
  //   }
  //   document.removeEventListener("mousemove", this.onMouseMove);
  //   this.tooltipRef.current.style.display = "none";

  //   const elem = e.target.closest(".dropable");
  //   if (elem && selection.length && dragging) {
  //     const value = elem.getAttribute("value");
  //     if (!value) {
  //       setDragging(false);
  //       return;
  //     }
  //     let splitValue = value.split("_");
  //     let item = null;
  //     if (splitValue[0] === "folder") {
  //       splitValue.splice(0, 1);
  //       if (splitValue[splitValue.length - 1] === "draggable") {
  //         splitValue.splice(-1, 1);
  //       }
  //       splitValue = splitValue.join("_");

  //       item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
  //     } else {
  //       return;
  //     }
  //     if (item) {
  //       setDragging(false);
  //       return;
  //     } else {
  //       setDragging(false);
  //       this.onMoveTo(splitValue);
  //       return;
  //     }
  //   } else {
  //     setDragging(false);
  //     if (dragItem) {
  //       this.onMoveTo(dragItem);
  //       return;
  //     }
  //     return;
  //   }
  // };

  // onMouseMove = (e) => {
  //   if (this.state.isDrag) {
  //     document.body.classList.add("drag-cursor");
  //     !this.props.dragging && this.props.setDragging(true);
  //     const tooltip = this.tooltipRef.current;
  //     tooltip.style.display = "block";
  //     this.setTooltipPosition(e);

  //     const wrapperElement = document.elementFromPoint(e.clientX, e.clientY);
  //     if (!wrapperElement) {
  //       return;
  //     }
  //     const droppable = wrapperElement.closest(".dropable");

  //     if (this.currentDroppable !== droppable) {
  //       if (this.currentDroppable) {
  //         this.currentDroppable.style.background = backgroundDragEnterColor;
  //       }
  //       this.currentDroppable = droppable;

  //       if (this.currentDroppable) {
  //         droppable.style.background = backgroundDragColor;
  //         this.currentDroppable = droppable;
  //       }
  //     }
  //   }
  // };

  // setTooltipPosition = (e) => {
  //   const tooltip = this.tooltipRef.current;
  //   if (tooltip) {
  //     const margin = 8;
  //     tooltip.style.left = e.pageX + margin + "px";
  //     tooltip.style.top = e.pageY + margin + "px";
  //   }
  // };

  // onMoveTo = (destFolderId) => {
  //   const {
  //     selection,
  //     t,
  //     isShare,
  //     isCommon,
  //     isAdmin,
  //     setSecondaryProgressBarData,
  //     copyToAction,
  //     moveToAction,
  //   } = this.props;

  //   const folderIds = [];
  //   const fileIds = [];
  //   const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
  //   const deleteAfter = true;

  //   setSecondaryProgressBarData({
  //     icon: "move",
  //     visible: true,
  //     percent: 0,
  //     label: t("MoveToOperation"),
  //     alert: false,
  //   });

  //   for (let item of selection) {
  //     if (item.fileExst) {
  //       fileIds.push(item.id);
  //     } else {
  //       folderIds.push(item.id);
  //     }
  //   }

  //   if (isAdmin) {
  //     if (isShare) {
  //       copyToAction(
  //         destFolderId,
  //         folderIds,
  //         fileIds,
  //         conflictResolveType,
  //         deleteAfter
  //       );
  //     } else {
  //       moveToAction(
  //         destFolderId,
  //         folderIds,
  //         fileIds,
  //         conflictResolveType,
  //         deleteAfter
  //       );
  //     }
  //   } else {
  //     if (isShare || isCommon) {
  //       copyToAction(
  //         destFolderId,
  //         folderIds,
  //         fileIds,
  //         conflictResolveType,
  //         deleteAfter
  //       );
  //     } else {
  //       moveToAction(
  //         destFolderId,
  //         folderIds,
  //         fileIds,
  //         conflictResolveType,
  //         deleteAfter
  //       );
  //     }
  //   }
  // };

  renderFileMoveTooltip = () => {
    const { selection, iconOfDraggedFile } = this.props;
    const { title } = selection[0];

    const reg = /^([^\\]*)\.(\w+)/;
    const matches = title.match(reg);

    let nameOfMovedObj, fileExtension;
    if (matches) {
      nameOfMovedObj = matches[1];
      fileExtension = matches.pop();
    } else {
      nameOfMovedObj = title;
    }

    return (
      <div className="tooltip-moved-obj-wrapper">
        {iconOfDraggedFile ? (
          <img
            className="tooltip-moved-obj-icon"
            src={`${iconOfDraggedFile}`}
            alt=""
          />
        ) : null}
        {nameOfMovedObj}
        {fileExtension ? (
          <span className="tooltip-moved-obj-extension">.{fileExtension}</span>
        ) : null}
      </div>
    );
  };

  // startMoveOperation = () => {
  //   this.props.moveToAction(this.props.dragItem);
  //   this.onCloseThirdPartyMoveDialog();
  // };

  // startCopyOperation = () => {
  //   this.props.copyToAction(this.props.dragItem);
  //   this.onCloseThirdPartyMoveDialog();
  // };

  getItemIcon = (isEdit, icon, fileExst) => {
    const { isPrivacyFolder } = this.props;
    const svgLoader = () => <div style={{ width: "24px" }}></div>;
    return (
      <>
        <ReactSVG
          className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
          src={icon}
          loading={svgLoader}
        />
        {isPrivacyFolder && fileExst && <EncryptedFileIcon isEdit={isEdit} />}
      </>
    );
  };

  getFilesContextOptions = (item, isFolder = null) => {
    const {
      t,
      isTabletView,
      setIsVerHistoryPanel,
      fetchFileVersions,
      history,
      homepage,
      setFavoriteAction,
      finalizeVersionAction,
      lockFileAction,
      onSelectItem,
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      openDocEditor,
      setMediaViewerData,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      duplicateAction,
      setAction,
      setThirdpartyInfo,
      isRootFolder,
      setRemoveItem,
      setDeleteThirdPartyDialogVisible,
      deleteFileAction,
      openLocationAction,
      deleteFolderAction,
    } = this.props;

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
    } = item;

    const onOpenLocation = () => {
      const { selection } = this.props;
      const { id } = selection[0];
      const locationId = isFolder ? id : folderId;
      openLocationAction(locationId, isFolder);
    };

    const isSharable = item.access !== 1 && item.access !== 0;
    const isThirdPartyFolder = providerKey && isRootFolder;

    const showVersionHistory = () => {
      if (!isTabletView) {
        fetchFileVersions(id + "");
        setIsVerHistoryPanel(true);
      } else {
        history.push(
          combineUrl(AppServerConfig.proxyURL, homepage, `/${id}/history`)
        );
      }
    };

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

    const finalizeVersion = () =>
      finalizeVersionAction(id).catch((err) => toastr.error(err));

    const lockFile = () =>
      lockFileAction(id, !locked).catch((err) => toastr.error(err));

    const onClickShare = () => {
      onSelectItem(item);
      setSharingPanelVisible(true);
    };

    const onOwnerChange = () => setChangeOwnerPanelVisible(true);

    const onClickLinkForPortal = () => {
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

    const onClickLinkEdit = () => openDocEditor(id, providerKey);

    const onMediaFileClick = (fileId) => {
      const itemId = typeof fileId !== "object" ? fileId : id;
      setMediaViewerData({ visible: true, id: itemId });
    };

    const onClickDownload = () => window.open(viewUrl, "_blank");
    const onMoveAction = () => setMoveToPanelVisible(true);
    const onCopyAction = () => setCopyPanelVisible(true);
    const onDuplicate = () =>
      duplicateAction(item, t("CopyOperation")).catch((err) =>
        toastr.error(err)
      );

    const onClickRename = () => {
      setAction({
        type: FileAction.Rename,
        extension: fileExst,
        id,
      });
    };

    const onChangeThirdPartyInfo = () => setThirdpartyInfo();

    const onClickDelete = () => {
      if (isThirdPartyFolder) {
        const splitItem = id.split("-");
        setRemoveItem({ id: splitItem[splitItem.length - 1], title });
        setDeleteThirdPartyDialogVisible(true);
        return;
      }

      const translations = {
        deleteOperation: t("DeleteOperation"),
      };

      fileExst
        ? deleteFileAction(id, folderId, translations)
            .then(() => toastr.success(t("FileRemoved")))
            .catch((err) => toastr.error(err))
        : deleteFolderAction(id, parentId, translations)
            .then(() => toastr.success(t("FolderRemoved")))
            .catch((err) => toastr.error(err));
    };

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
  };

  render() {
    const {
      selection,
      fileActionId,
      dragging,
      viewAs,
      t,
      isMobile,
      firstLoad,
      tooltipValue,
      isLoading,
      isEmptyFilesList,

      folderId,
    } = this.props;

    console.log("Files Home SectionBodyContent render", this.props);

    let fileMoveTooltip;
    if (dragging) {
      fileMoveTooltip = tooltipValue
        ? selection.length === 1 &&
          tooltipValue.label === "TooltipElementMoveMessage"
          ? this.renderFileMoveTooltip()
          : t(tooltipValue.label, { element: tooltipValue.filesCount })
        : "";
    }

    return (!fileActionId && isEmptyFilesList) || null ? (
      firstLoad || (isMobile && isLoading) ? (
        <Loaders.Rows />
      ) : (
        <EmptyContainer />
      )
    ) : (
      <>
        <CustomTooltip ref={this.tooltipRef}>{fileMoveTooltip}</CustomTooltip>
        {viewAs === "tile" ? (
          <FilesTileContainer
            t={t}
            folderId={folderId}
            getItemIcon={this.getItemIcon}
            getFilesContextOptions={this.getFilesContextOptions}
          />
        ) : (
          <FilesRowContainer
            t={t}
            //getFilesContextOptions={this.getFilesContextOptions}
            folderId={folderId}
          />
        )}
      </>
    );
  }
}

export default inject(
  ({
    auth,
    initFilesStore,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    filesActionsStore,
    selectedFolderStore,
    versionHistoryStore,
    dialogsStore,
    mediaViewerDataStore,
  }) => {
    const {
      dragging,
      setDragging,
      isLoading,
      viewAs,
      dragItem,
      tooltipValue,
    } = initFilesStore;
    const {
      firstLoad,
      selection,
      fileActionStore,
      iconOfDraggedFile,
      filesList,
      openDocEditor,
    } = filesStore;
    const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
    const { setMediaViewerData } = mediaViewerDataStore;

    const {
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setRemoveItem,
      setDeleteThirdPartyDialogVisible,
    } = dialogsStore;

    const { isShareFolder, isCommonFolder, isPrivacyFolder } = treeFoldersStore;
    const { id: fileActionId, setAction } = fileActionStore;
    const {
      setSecondaryProgressBarData,
    } = uploadDataStore.secondaryProgressDataStore;
    const {
      copyToAction,
      moveToAction,
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

    return {
      isAdmin: auth.isAdmin,

      dragging,
      fileActionId,
      firstLoad,
      selection,
      isShare: isShareFolder,
      isCommon: isCommonFolder,
      viewAs,
      dragItem,
      iconOfDraggedFile,
      tooltipValue,
      isLoading,
      isEmptyFilesList: filesList.length <= 0,

      setDragging,
      setSecondaryProgressBarData,
      copyToAction,
      moveToAction,
      folderId: selectedFolderStore.id,

      isPrivacyFolder,

      openLocationAction,
      isTabletView: auth.settingsStore,
      setIsVerHistoryPanel,
      fetchFileVersions,
      homepage: config.homepage,
      setFavoriteAction,
      finalizeVersionAction,
      lockFileAction,
      onSelectItem,
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      openDocEditor,
      setMediaViewerData,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      duplicateAction,
      setAction,
      setThirdpartyInfo,
      deleteFileAction,
      isRootFolder: selectedFolderStore.isRootFolder,
      setRemoveItem,
      setDeleteThirdPartyDialogVisible,
      deleteFolderAction,
    };
  }
)(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
