import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ReactSVG } from "react-svg";
import { withTranslation, Trans } from "react-i18next";
import equal from "fast-deep-equal/react";
import copy from "copy-to-clipboard";
import styled from "styled-components";
import queryString from "query-string";
import {
  Row,
  RowContainer,
  Link,
  IconButton,
  DragAndDrop,
  Box,
  Text,
  utils,
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";
import FilesTileContent from "./FilesTileContent";
import TileContainer from "./TileContainer";
import Tile from "./Tile";
import {
  api,
  constants,
  MediaViewer,
  toastr,
  Loaders,
  store,
} from "asc-web-common";
import {
  clearSecondaryProgressData,
  loopFilesOperations,
  markItemAsFavorite,
  removeItemFromFavorite,
  fetchFavoritesFolder,
  deselectFile,
  updateFile,
  fetchFiles,
  selectFile,
  setAction,
  setDragging,
  setDragItem,
  setIsLoading,
  setMediaViewerData,
  setUpdateTree,
  setSecondaryProgressBarData,
  setSelected,
  setSelection,
  setTreeFolders,
  getFileInfo,
  addFileToRecentlyViewed,
  setSharingPanelVisible,
} from "../../../../../store/files/actions";
import { TIMEOUT } from "../../../../../helpers/constants";
import {
  getCurrentFolderCount,
  getDragging,
  getDragItem,
  getFileAction,
  getFileIcon,
  getFiles,
  getFilter,
  getFirstLoad,
  getFolderIcon,
  getSelectedFolderId,
  getFolders,
  getIsLoading,
  getMediaViewerId,
  getMediaViewerVisibility,
  getSelectedFolderParentId,
  getSelected,
  getSelectedFolderTitle,
  getSelection,
  getTreeFolders,
  getViewAs,
  isFileSelected,
  loopTreeFolders,
  getFilesList,
  getMediaViewerImageFormats,
  getMediaViewerMediaFormats,
  getIsShareFolder,
  getIsCommonFolder,
  getIsRecycleBinFolder,
  getIsRecentFolder,
  getIsMyFolder,
  getIsFavoritesFolder,
  getMyFolderId,
  getTooltipLabel,
  getIsPrivacyFolder,
  getPrivacyInstructionsLink,
  getIconOfDraggedFile,
  getSharePanelVisible,
} from "../../../../../store/files/selectors";
import { OperationsPanel } from "../../../../panels";
const {
  isAdmin,
  getSettings,
  getCurrentUser,
  isEncryptionSupport,
  getOrganizationName,
} = store.auth.selectors;
//import { getFilterByLocation } from "../../../../../helpers/converters";
//import config from "../../../../../../package.json";

const { FilesFilter } = api;
const { FileAction } = constants;
const { Consumer } = utils.context;

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  color: "#555f65",
  className: "empty-folder_link",
  display: "flex",
};
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

const SimpleFilesRow = styled(Row)`
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

  .share-button,
  .share-button-icon:hover {
    cursor: pointer;
    div {
      color: "#657077";
    }
  }

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

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      editingId: null,
      showMoveToPanel: false,
      showCopyPanel: false,
      isDrag: false,
      canDrag: true,
    };

    this.tooltipRef = React.createRef();
    this.currentDroppable = null;
  }

  componentDidMount() {
    let previewId = queryString.parse(this.props.location.search).preview;

    if (previewId) {
      this.onMediaFileClick(+previewId);
    }

    window.addEventListener("mouseup", this.onMouseUp);

    document.addEventListener("dragstart", this.onDragStart);
    document.addEventListener("dragover", this.onDragOver);
    document.addEventListener("dragleave", this.onDragLeaveDoc);
    document.addEventListener("drop", this.onDropEvent);
  }

  componentWillUnmount() {
    window.removeEventListener("mouseup", this.onMouseUp);

    document.addEventListener("dragstart", this.onDragStart);
    document.removeEventListener("dragover", this.onDragOver);
    document.removeEventListener("dragleave", this.onDragLeaveDoc);
    document.removeEventListener("drop", this.onDropEvent);
  }

  // componentDidUpdate(prevProps, prevState) {
  //   Object.entries(this.props).forEach(([key, val]) =>
  //     prevProps[key] !== val && console.log(`Prop '${key}' changed`)
  //   );
  //   if (this.state) {
  //     Object.entries(this.state).forEach(([key, val]) =>
  //       prevState[key] !== val && console.log(`State '${key}' changed`)
  //     );
  //   }
  // }

  shouldComponentUpdate(nextProps, nextState) {
    if (this.props && this.props.firstLoad) return true;

    const { showMoveToPanel, showCopyPanel, isDrag } = this.state;
    if (this.props.sharingPanelVisible !== nextProps.sharingPanelVisible) {
      return true;
    }

    if (this.props.dragItem !== nextProps.dragItem) {
      return false;
    }

    if (!equal(this.props, nextProps)) {
      return true;
    }

    if (
      showMoveToPanel !== nextState.showMoveToPanel ||
      showCopyPanel !== nextState.showCopyPanel
    ) {
      return true;
    }

    if (isDrag !== nextState.isDrag) {
      return true;
    }

    return false;
  }

  onOpenLocation = () => {
    const item = this.props.selection[0];
    const { folderId, checked } = this.props.selection[0];
    return this.props
      .fetchFiles(folderId)
      .then(() => this.onContentRowSelect(!checked, item));
  };

  onClickFavorite = (e) => {
    const {
      markItemAsFavorite,
      removeItemFromFavorite,
      getFileInfo,
      fetchFavoritesFolder,
      isFavorites,
      selectedFolderId,
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
          .catch((e) => toastr.error(e));
      default:
        return;
    }
  };

  onClickRename = () => {
    const { id, fileExst } = this.props.selection[0];

    this.setState({ editingId: id }, () => {
      this.props.setAction({
        type: FileAction.Rename,
        extension: fileExst,
        id,
      });
    });
  };

  onEditComplete = (id, isFolder) => {
    const {
      folderId,
      fileAction,
      filter,
      folders,
      files,
      treeFolders,
      setTreeFolders,
      setIsLoading,
      fetchFiles,
      setUpdateTree,
    } = this.props;
    const items = [...folders, ...files];
    const item = items.find((o) => o.id === id && !o.fileExst); //TODO maybe need files find and folders find, not at one function?
    if (
      fileAction.type === FileAction.Create ||
      fileAction.type === FileAction.Rename
    ) {
      setIsLoading(true);
      fetchFiles(folderId, filter)
        .then((data) => {
          const newItem = (item && item.id) === -1 ? null : item; //TODO not add new folders?
          if (isFolder) {
            const path = data.selectedFolder.pathParts;
            const newTreeFolders = treeFolders;
            const folders = data.selectedFolder.folders;
            loopTreeFolders(path, newTreeFolders, folders, null, newItem);
            setUpdateTree(true);
            setTreeFolders(newTreeFolders);
          }
        })
        .finally(() => setIsLoading(false));
    }

    this.setState({ editingId: null }, () => {
      this.props.setAction({
        type: null,
      });
    });
  };

  onClickDelete = () => {
    const item = this.props.selection[0];
    item.fileExst
      ? this.onDeleteFile(item.id, item.folderId)
      : this.onDeleteFolder(item.id, item.parentId);
  };

  onDeleteFile = (fileId, currentFolderId) => {
    const {
      t,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.props;
    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: t("DeleteOperation"),
      alert: false,
    });
    api.files
      .deleteFile(fileId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  loopDeleteProgress = (id, folderId, isFolder) => {
    const {
      filter,
      treeFolders,
      setTreeFolders,
      isRecycleBin,
      t,
      setSecondaryProgressBarData,
      fetchFiles,
      setUpdateTree,
    } = this.props;
    api.files.getProgress().then((res) => {
      const deleteProgress = res.find((x) => x.id === id);
      if (deleteProgress && deleteProgress.progress !== 100) {
        setSecondaryProgressBarData({
          icon: "trash",
          visible: true,
          percent: deleteProgress.progress,
          label: t("DeleteOperation"),
          alert: false,
        });
        setTimeout(() => this.loopDeleteProgress(id, folderId, isFolder), 1000);
      } else {
        setSecondaryProgressBarData({
          icon: "trash",
          visible: true,
          percent: 100,
          label: t("DeleteOperation"),
          alert: false,
        });
        fetchFiles(folderId, filter)
          .then((data) => {
            if (!isRecycleBin && isFolder) {
              const path = data.selectedFolder.pathParts.slice(0);
              const newTreeFolders = treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              setUpdateTree(true);
              setTreeFolders(newTreeFolders);
            }
            isFolder
              ? toastr.success(`Folder moved to recycle bin`)
              : toastr.success(`File moved to recycle bin`);
          })
          .catch((err) => {
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            });
            //toastr.error(err);
            setTimeout(() => this.props.clearSecondaryProgressData(), TIMEOUT);
          })
          .finally(() =>
            setTimeout(() => this.props.clearSecondaryProgressData(), TIMEOUT)
          );
      }
    });
  };

  onDeleteFolder = (folderId, currentFolderId) => {
    const {
      t,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.props;
    const progressLabel = t("DeleteOperation");
    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: progressLabel,
      alert: false,
    });
    api.files
      .deleteFolder(folderId, currentFolderId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  onClickShare = () =>
    this.props.setSharingPanelVisible(!this.props.sharingPanelVisible);

  onClickLinkForPortal = () => {
    const { settings, selection } = this.props;
    const item = selection[0];
    const isFile = !!item.fileExst;
    const { t } = this.props;

    copy(
      isFile
        ? item.canOpenPlayer
          ? `${window.location.origin + settings.homepage}/filter?folder=${
              item.folderId
            }&preview=${item.id}`
          : item.webUrl
        : `${window.location.origin + settings.homepage}/filter?folder=${
            item.id
          }`
    );

    toastr.success(t("LinkCopySuccess"));
  };

  onClickDownload = () => {
    return window.open(this.props.selection[0].viewUrl, "_blank");
  };

  openDocEditor = (id, tab = null, url = null) => {
    return this.props
      .addFileToRecentlyViewed(id)
      .then(() => console.log("Pushed to recently viewed"))
      .catch((e) => console.error(e))
      .finally(
        tab
          ? (tab.location = url)
          : window.open(`./doceditor?fileId=${id}`, "_blank")
      );
  };

  onClickLinkEdit = (e) => {
    const id = e.currentTarget.dataset.id;
    return this.openDocEditor(id);
  };

  showVersionHistory = (e) => {
    const { settings, history } = this.props;
    const fileId = e.currentTarget.dataset.id;

    history.push(`${settings.homepage}/${fileId}/history`);
  };

  lockFile = () => {
    const {
      selection,
      /*files,*/ selectedFolderId,
      filter,
      setIsLoading,
      fetchFiles,
    } = this.props;
    const file = selection[0];

    api.files.lockFile(file.id, !file.locked).then((res) => {
      /*const newFiles = files;
        const indexOfFile = newFiles.findIndex(x => x.id === res.id);
        newFiles[indexOfFile] = res;*/
      setIsLoading(true);
      fetchFiles(selectedFolderId, filter)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    });
  };

  finalizeVersion = (e) => {
    const { selectedFolderId, filter, setIsLoading, fetchFiles } = this.props;

    const fileId = e.currentTarget.dataset.id;
    //const version = (e.currentTarget.dataset.version)++;

    setIsLoading(true);

    api.files
      .finalizeVersion(fileId, 0, false)
      .then((data) => {
        //console.log("api.files.finalizeVersion", data);
        return fetchFiles(selectedFolderId, filter).catch((err) =>
          toastr.error(err)
        );
      })
      .finally(() => setIsLoading(false));
  };

  onMoveAction = () =>
    this.setState({ showMoveToPanel: !this.state.showMoveToPanel });
  onCopyAction = () =>
    this.setState({ showCopyPanel: !this.state.showCopyPanel });
  onDuplicate = () => {
    const {
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
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
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
    );
  };

  getFilesContextOptions = (options, item) => {
    const { t } = this.props;

    const isSharable = item.access !== 1 && item.access !== 0;

    return options.map((option) => {
      switch (option) {
        case "show-version-history":
          return {
            key: option,
            label: t("ShowVersionHistory"),
            icon: "HistoryIcon",
            onClick: this.showVersionHistory,
            disabled: false,
            "data-id": item.id,
          };
        case "finalize-version":
          return {
            key: option,
            label: t("FinalizeVersion"),
            icon: "HistoryFinalizedIcon",
            onClick: this.finalizeVersion,
            disabled: false,
            "data-id": item.id,
            "data-version": item.version,
          };
        case "separator0":
        case "separator1":
        case "separator2":
          return { key: option, isSeparator: true };
        case "open-location":
          return {
            key: option,
            label: t("OpenLocation"),
            icon: "DownloadAsIcon",
            onClick: this.onOpenLocation,
            disabled: false,
          };
        case "mark-as-favorite":
          return {
            key: option,
            label: t("MarkAsFavorite"),
            icon: "FavoritesIcon",
            onClick: this.onClickFavorite,
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
            onClick: this.lockFile,
            disabled: false,
          };
        case "sharing-settings":
          return {
            key: option,
            label: t("SharingSettings"),
            icon: "CatalogSharedIcon",
            onClick: this.onClickShare,
            disabled: isSharable,
          };
        case "send-by-email":
          return {
            key: option,
            label: t("SendByEmail"),
            icon: "MailIcon",
            disabled: true,
          };
        case "link-for-portal-users":
          return {
            key: option,
            label: t("LinkForPortalUsers"),
            icon: "InvitationLinkIcon",
            onClick: this.onClickLinkForPortal,
            disabled: false,
          };
        case "edit":
          return {
            key: option,
            label: t("Edit"),
            icon: "AccessEditIcon",
            onClick: this.onClickLinkEdit,
            disabled: false,
            "data-id": item.id,
          };
        case "preview":
          return {
            key: option,
            label: t("Preview"),
            icon: "EyeIcon",
            onClick: this.onClickLinkEdit,
            disabled: true,
            "data-id": item.id,
          };
        case "view":
          return {
            key: option,
            label: t("View"),
            icon: "EyeIcon",
            onClick: this.onMediaFileClick,
            disabled: false,
          };
        case "download":
          return {
            key: option,
            label: t("Download"),
            icon: "DownloadIcon",
            onClick: this.onClickDownload,
            disabled: false,
          };
        case "move":
          return {
            key: option,
            label: t("MoveTo"),
            icon: "MoveToIcon",
            onClick: this.onMoveAction,
            disabled: false,
          };
        case "copy":
          return {
            key: option,
            label: t("Copy"),
            icon: "CopyIcon",
            onClick: this.onCopyAction,
            disabled: false,
          };
        case "duplicate":
          return {
            key: option,
            label: t("Duplicate"),
            icon: "CopyIcon",
            onClick: this.onDuplicate,
            disabled: false,
          };
        case "rename":
          return {
            key: option,
            label: t("Rename"),
            icon: "RenameIcon",
            onClick: this.onClickRename,
            disabled: false,
          };
        case "delete":
          return {
            key: option,
            label: t("Delete"),
            icon: "CatalogTrashIcon",
            onClick: this.onClickDelete,
            disabled: false,
          };
        case "remove-from-favorites":
          return {
            key: option,
            label: t("RemoveFromFavorites"),
            icon: "FavoritesIcon",
            onClick: this.onClickFavorite,
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

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.widthProp !== nextProps.widthProp) {
      return true;
    }
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.editing !== nextProps.editing) {
      return true;
    }
    if (currentProps.sectionWidth !== nextProps.sectionWidth) {
      return true;
    }
    if (!equal(currentProps.data, nextProps.data)) {
      return true;
    }
    if (currentProps.viewAs !== nextProps.viewAs) {
      return true;
    }
    return false;
  };

  onContentRowSelect = (checked, file) => {
    if (!file) return;
    const { selected, setSelected, selectFile, deselectFile } = this.props;

    selected === "close" && setSelected("none");
    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  svgLoader = () => <div style={{ width: "24px" }}></div>;

  getItemIcon = (item, isEdit) => {
    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px");
          isEdit && svg.setAttribute("style", "margin: 4px 0 0 28px");
        }}
        src={item.icon}
        loading={this.svgLoader}
      />
    );
  };

  onCreate = (e) => {
    const format = e.currentTarget.dataset.format || null;
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onResetFilter = () => {
    const { selectedFolderId, setIsLoading, fetchFiles } = this.props;
    setIsLoading(true);
    const newFilter = FilesFilter.getDefault();
    fetchFiles(selectedFolderId, newFilter)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  onGoToMyDocuments = () => {
    const { filter, myDocumentsId, setIsLoading, fetchFiles } = this.props;
    const newFilter = filter.clone();
    setIsLoading(true);
    fetchFiles(myDocumentsId, newFilter).finally(() => setIsLoading(false));
  };

  onBackToParentFolder = () => {
    const { filter, parentId, setIsLoading, fetchFiles } = this.props;
    const newFilter = filter.clone();
    setIsLoading(true);
    fetchFiles(parentId, newFilter).finally(() => setIsLoading(false));
  };

  renderEmptyRootFolderContainer = () => {
    const {
      isMy,
      isShare,
      isCommon,
      isRecycleBin,
      isFavorites,
      isRecent,
      isPrivacy,
      organizationName,
      privacyInstructions,
      title,
      t,
      i18n,
    } = this.props;
    const subheadingText = t("SubheadingEmptyText");
    const myDescription = t("MyEmptyContainerDescription");
    const shareDescription = t("SharedEmptyContainerDescription");
    const commonDescription = t("CommonEmptyContainerDescription");
    const trashDescription = t("TrashEmptyContainerDescription");
    const favoritesDescription = t("FavoritesEmptyContainerDescription");
    const recentDescription = t("RecentEmptyContainerDescription");

    const privateRoomHeader = t("PrivateRoomHeader");
    const privacyIcon = <img alt="" src="images/privacy.svg" />;
    const privateRoomDescTranslations = [
      t("PrivateRoomDescriptionSafest"),
      t("PrivateRoomDescriptionSecure"),
      t("PrivateRoomDescriptionEncrypted"),
      t("PrivateRoomDescriptionUnbreakable"),
    ];
    const privateRoomDescription = (
      <>
        <Text fontSize="15px" as="div">
          {privateRoomDescTranslations.map((el) => (
            <Box
              displayProp="flex"
              alignItems="center"
              paddingProp="0 0 13px 0"
              key={el}
            >
              <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
              <Box>{el}</Box>
            </Box>
          ))}
        </Text>
        <Text fontSize="12px">
          <Trans i18nKey="PrivateRoomSupport" i18n={i18n}>
            Work in Private Room is available via {{ organizationName }} desktop
            app.
            <Link isBold isHovered color="#116d9d" href={privacyInstructions}>
              Instructions
            </Link>
          </Trans>
        </Text>
      </>
    );

    const commonButtons = (
      <span>
        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_plus-image"
            src="images/plus.svg"
            data-format="docx"
            onClick={this.onCreate}
            alt="plus_icon"
          />
          <Box className="flex-wrapper_container">
            <Link data-format="docx" onClick={this.onCreate} {...linkStyles}>
              {t("Document")},
            </Link>
            <Link data-format="xlsx" onClick={this.onCreate} {...linkStyles}>
              {t("Spreadsheet")},
            </Link>
            <Link data-format="pptx" onClick={this.onCreate} {...linkStyles}>
              {t("Presentation")}
            </Link>
          </Box>
        </div>

        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_plus-image"
            src="images/plus.svg"
            onClick={this.onCreate}
            alt="plus_icon"
          />
          <Link {...linkStyles} onClick={this.onCreate}>
            {t("Folder")}
          </Link>
        </div>
      </span>
    );

    const trashButtons = (
      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_up-image"
          src="images/empty_screen_people.svg"
          width="12px"
          alt=""
          onClick={this.onGoToMyDocuments}
        />
        <Link onClick={this.onGoToMyDocuments} {...linkStyles}>
          {t("GoToMyButton")}
        </Link>
      </div>
    );

    if (isMy) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={myDescription}
          imageSrc="images/empty_screen.png"
          buttons={commonButtons}
        />
      );
    } else if (isShare) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={shareDescription}
          imageSrc="images/empty_screen_forme.png"
        />
      );
    } else if (isCommon) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={commonDescription}
          imageSrc="images/empty_screen_corporate.png"
          buttons={commonButtons}
        />
      );
    } else if (isRecycleBin) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={trashDescription}
          imageSrc="images/empty_screen_trash.png"
          buttons={trashButtons}
        />
      );
    } else if (isFavorites) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={favoritesDescription}
          imageSrc="images/empty_screen_favorites.png"
        />
      );
    } else if (isRecent) {
      return (
        <EmptyFolderContainer
          headerText={title}
          subheadingText={subheadingText}
          descriptionText={recentDescription}
          imageSrc="images/empty_screen_recent.png"
        />
      );
    } else if (isPrivacy) {
      return (
        <EmptyFolderContainer
          headerText={privateRoomHeader}
          descriptionText={privateRoomDescription}
          imageSrc="images/empty_screen_privacy.png"
        />
      );
    } else {
      return null;
    }
  };

  renderEmptyFolderContainer = () => {
    const { t } = this.props;
    const buttons = (
      <>
        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_plus-image"
            src="images/plus.svg"
            data-format="docx"
            onClick={this.onCreate}
            alt="plus_icon"
          />
          <Box className="flex-wrapper_container">
            <Link data-format="docx" onClick={this.onCreate} {...linkStyles}>
              {t("Document")},
            </Link>
            <Link data-format="xlsx" onClick={this.onCreate} {...linkStyles}>
              {t("Spreadsheet")},
            </Link>
            <Link data-format="pptx" onClick={this.onCreate} {...linkStyles}>
              {t("Presentation")}
            </Link>
          </Box>
        </div>

        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_plus-image"
            src="images/plus.svg"
            onClick={this.onCreate}
            alt="plus_icon"
          />
          <Link {...linkStyles} onClick={this.onCreate}>
            {t("Folder")}
          </Link>
        </div>

        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_up-image"
            src="images/up.svg"
            onClick={this.onBackToParentFolder}
            alt="up_icon"
          />

          <Link onClick={this.onBackToParentFolder} {...linkStyles}>
            {t("BackToParentFolderButton")}
          </Link>
        </div>
      </>
    );

    return (
      <EmptyFolderContainer
        headerText={t("EmptyFolderHeader")}
        imageSrc="images/empty_screen.png"
        buttons={buttons}
      />
    );
  };

  renderEmptyFilterContainer = () => {
    const { t } = this.props;
    const subheadingText = t("EmptyFilterSubheadingText");
    const descriptionText = t("EmptyFilterDescriptionText");

    const buttons = (
      <div className="empty-folder_container-links">
        <IconButton
          className="empty-folder_container-icon"
          size="12"
          onClick={this.onResetFilter}
          iconName="CrossIcon"
          isFill
          color="#657077"
        />
        <Link onClick={this.onResetFilter} {...linkStyles}>
          {t("ClearButton")}
        </Link>
      </div>
    );

    return (
      <EmptyFolderContainer
        headerText={t("Filter")}
        subheadingText={subheadingText}
        descriptionText={descriptionText}
        imageSrc="images/empty_screen_filter.png"
        buttons={buttons}
      />
    );
  };

  onMediaViewerClose = () => {
    const item = { visible: false, id: null };
    this.props.setMediaViewerData(item);
  };
  onMediaFileClick = (id) => {
    const itemId = typeof id !== "object" ? id : this.props.selection[0].id;
    const item = { visible: true, id: itemId };
    this.props.setMediaViewerData(item);
  };

  onDownloadMediaFile = (id) => {
    if (this.props.files.length > 0) {
      let viewUrlFile = this.props.files.find((file) => file.id === id).viewUrl;
      return window.open(viewUrlFile, "_blank");
    }
  };

  onDeleteMediaFile = (id) => {
    if (this.props.files.length > 0) {
      let file = this.props.files.find((file) => file.id === id);
      if (file) this.onDeleteFile(file.id, file.folderId);
    }
  };

  onDragStart = (e) => {
    if (e.dataTransfer.dropEffect === "none") {
      this.state.canDrag && this.setState({ canDrag: false });
    }
  };

  onDrop = (item, items, e) => {
    const { onDropZoneUpload, selectedFolderId } = this.props;

    if (!item.fileExst) {
      onDropZoneUpload(items, item.id);
    } else {
      onDropZoneUpload(items, selectedFolderId);
    }
  };

  onDropEvent = () => {
    this.props.dragging && this.props.setDragging(false);
  };

  onDragOver = (e) => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (e.dataTransfer.items.length > 0 && !dragging && this.state.canDrag) {
      setDragging(true);
    }
  };

  onDragLeaveDoc = (e) => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (dragging && !e.relatedTarget) {
      setDragging(false);
    }
  };

  onMouseDown = (e) => {
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
    document.addEventListener("mousemove", this.onMouseMove);
    this.setTooltipPosition(e);
    const { selection } = this.props;

    const elem = e.currentTarget.closest(".draggable");
    if (!elem) {
      return;
    }
    const value = elem.getAttribute("value");
    if (!value) {
      return;
    }
    const splitValue = value.split("_");
    let item = null;
    if (splitValue[0] === "folder") {
      item = selection.find(
        (x) => x.id === Number(splitValue[1]) && !x.fileExst
      );
    } else {
      item = selection.find(
        (x) => x.id === Number(splitValue[1]) && x.fileExst
      );
    }
    if (item) {
      this.setState({ isDrag: true });
    }
  };

  onMouseUp = (e) => {
    const {
      selection,
      dragging,
      setDragging,
      dragItem,
      setDragItem,
    } = this.props;

    document.body.classList.remove("drag-cursor");

    if (this.state.isDrag || !this.state.canDrag) {
      this.setState({ isDrag: false, canDrag: true });
    }
    const mouseButton = e.which
      ? e.which !== 1
      : e.button
      ? e.button !== 0
      : false;
    if (mouseButton || !this.tooltipRef.current || !dragging) {
      return;
    }
    document.removeEventListener("mousemove", this.onMouseMove);
    this.tooltipRef.current.style.display = "none";

    const elem = e.target.closest(".dropable");
    if (elem && selection.length && dragging) {
      const value = elem.getAttribute("value");
      if (!value) {
        setDragging(false);
        return;
      }
      const splitValue = value.split("_");
      let item = null;
      if (splitValue[0] === "folder") {
        item = selection.find(
          (x) => x.id === Number(splitValue[1]) && !x.fileExst
        );
      } else {
        return;
      }
      if (item) {
        setDragging(false);
        return;
      } else {
        setDragging(false);
        this.onMoveTo(Number(splitValue[1]));
        return;
      }
    } else {
      setDragging(false);
      if (dragItem) {
        this.onMoveTo(dragItem);
        setDragItem(null);
        return;
      }
      return;
    }
  };

  onMouseMove = (e) => {
    if (this.state.isDrag) {
      document.body.classList.add("drag-cursor");
      !this.props.dragging && this.props.setDragging(true);
      const tooltip = this.tooltipRef.current;
      tooltip.style.display = "block";
      this.setTooltipPosition(e);

      const wrapperElement = document.elementFromPoint(e.clientX, e.clientY);
      if (!wrapperElement) {
        return;
      }
      const droppable = wrapperElement.closest(".dropable");

      if (this.currentDroppable !== droppable) {
        if (this.currentDroppable) {
          this.currentDroppable.style.background = backgroundDragEnterColor;
        }
        this.currentDroppable = droppable;

        if (this.currentDroppable) {
          droppable.style.background = backgroundDragColor;
          this.currentDroppable = droppable;
        }
      }
    }
  };

  setTooltipPosition = (e) => {
    const tooltip = this.tooltipRef.current;
    if (tooltip) {
      const margin = 8;
      tooltip.style.left = e.pageX + margin + "px";
      tooltip.style.top = e.pageY + margin + "px";
    }
  };

  onMoveTo = (destFolderId) => {
    const {
      selection,
      t,
      isShare,
      isCommon,
      isAdmin,
      setSecondaryProgressBarData,
    } = this.props;
    const folderIds = [];
    const fileIds = [];
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;

    setSecondaryProgressBarData({
      icon: "move",
      visible: true,
      percent: 0,
      label: t("MoveToOperation"),
      alert: false,
    });
    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    if (isAdmin) {
      if (isShare) {
        this.copyTo(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        this.moveTo(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    } else {
      if (isShare || isCommon) {
        this.copyTo(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        this.moveTo(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    }
  };

  copyTo = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    const { loopFilesOperations, clearSecondaryProgressData } = this.props;

    api.files
      .copyToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter
      )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, true);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  moveTo = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    const { loopFilesOperations, clearSecondaryProgressData } = this.props;

    api.files
      .moveToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter
      )
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, false);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  onSelectItem = (item) => {
    const { selected, setSelected, setSelection } = this.props;
    selected === "close" && setSelected("none");
    setSelection([item]);
  };

  onCreateAddTempItem = (items, folderId, fileAction) => {
    if (items.length && items[0].id === -1) return; //TODO: if change media collection from state remove this;
    const icon = fileAction.extension
      ? getFileIcon(`.${fileAction.extension}`, 24)
      : getFolderIcon(null, 24);

    items.unshift({
      id: -1,
      title: "",
      parentId: folderId,
      fileExst: fileAction.extension,
      icon,
    });
  };

  getSharedButton = () => {
    return (
      <Text
        className="share-button"
        as="span"
        title={this.props.t("Share")}
        fontSize="12px"
        fontWeight={600}
        color="#A3A9AE"
        display="inline-flex"
        onClick={this.onClickShare}
      >
        <IconButton
          className="share-button-icon"
          color="#a3a9ae"
          hoverColor="#657077"
          size={18}
          iconName="CatalogSharedIcon"
        />
        {this.props.t("Share")}
      </Text>
    );
  };

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

  render() {
    //console.log("Files Home SectionBodyContent render", this.props);

    const {
      viewer,
      parentId,
      folderId,
      settings,
      selection,
      fileAction,
      setIsLoading,
      isLoading,
      currentFolderCount,
      isRecycleBin,
      isPrivacy,
      isEncryptionSupport,
      dragging,
      mediaViewerVisible,
      currentMediaFileId,
      viewAs,
      t,
      isMobile,
      firstLoad,
      filesList,
      mediaViewerImageFormats,
      mediaViewerMediaFormats,
      tooltipValue,
    } = this.props;

    const { editingId, showMoveToPanel, showCopyPanel } = this.state;

    const operationsPanelProps = {
      setIsLoading,
      isLoading,
    };

    let fileMoveTooltip;
    if (dragging) {
      fileMoveTooltip = tooltipValue
        ? selection.length === 1 &&
          tooltipValue.label === "TooltipElementMoveMessage"
          ? this.renderFileMoveTooltip()
          : t(tooltipValue.label, { element: tooltipValue.filesCount })
        : "";
    }

    const items = filesList;

    if (fileAction && fileAction.type === FileAction.Create) {
      this.onCreateAddTempItem(items, folderId, fileAction);
    }

    var playlist = [];
    let id = 0;

    if (items) {
      items.forEach(function (file, i, files) {
        if (file.canOpenPlayer) {
          playlist.push({
            id: id,
            fileId: file.id,
            src: file.viewUrl,
            title: file.title,
          });
          id++;
        }
      });
    }

    return (!fileAction.id && currentFolderCount === 0) || null ? (
      parentId === 0 ? (
        this.renderEmptyRootFolderContainer()
      ) : (
        this.renderEmptyFolderContainer()
      )
    ) : !fileAction.id && items.length === 0 ? (
      firstLoad ? (
        <Loaders.Rows />
      ) : isPrivacy && !isEncryptionSupport ? (
        this.renderEmptyRootFolderContainer()
      ) : (
        this.renderEmptyFilterContainer()
      )
    ) : (
      <>
        {showMoveToPanel && (
          <OperationsPanel
            {...operationsPanelProps}
            isCopy={false}
            visible={showMoveToPanel}
            onClose={this.onMoveAction}
          />
        )}

        {showCopyPanel && (
          <OperationsPanel
            {...operationsPanelProps}
            isCopy={true}
            visible={showCopyPanel}
            onClose={this.onCopyAction}
          />
        )}
        <CustomTooltip ref={this.tooltipRef}>{fileMoveTooltip}</CustomTooltip>

        {viewAs === "tile" ? (
          <TileContainer
            className="tileContainer"
            draggable
            useReactWindow={false}
            headingFolders={t("Folders")}
            headingFiles={t("Files")}
          >
            {items.map((item) => {
              const isEdit =
                !!fileAction.type &&
                editingId === item.id &&
                item.fileExst === fileAction.extension;
              const contextOptions = this.getFilesContextOptions(
                item,
                viewer
              ).filter((o) => o);
              const contextOptionsProps =
                !contextOptions.length || isEdit ? {} : { contextOptions };
              const checked = isFileSelected(selection, item.id, item.parentId);
              const checkedProps = isEdit || item.id <= 0 ? {} : { checked };
              const element = this.getItemIcon(item, isEdit || item.id <= 0);

              const selectedItem = selection.find(
                (x) => x.id === item.id && x.fileExst === item.fileExst
              );
              const isFolder = selectedItem
                ? false
                : item.fileExst
                ? false
                : true;
              const draggable = selectedItem && !isRecycleBin;
              let value = item.fileExst
                ? `file_${item.id}`
                : `folder_${item.id}`;
              value += draggable ? "_draggable" : "";
              let classNameProp =
                isFolder && item.access < 2 && !isRecycleBin
                  ? { className: " dropable" }
                  : {};

              if (item.draggable) classNameProp.className += " draggable";

              return (
                <DragAndDrop
                  {...classNameProp}
                  onDrop={this.onDrop.bind(this, item)}
                  onMouseDown={this.onMouseDown}
                  dragging={dragging && isFolder && item.access < 2}
                  key={`dnd-key_${item.id}`}
                  {...contextOptionsProps}
                  value={value}
                  isFolder={!item.fileExst}
                >
                  <Tile
                    key={item.id}
                    item={item}
                    isFolder={!item.fileExst}
                    element={element}
                    onSelect={this.onContentRowSelect}
                    editing={editingId}
                    viewAs={viewAs}
                    {...checkedProps}
                    {...contextOptionsProps}
                    needForUpdate={this.needForUpdate}
                  >
                    <FilesTileContent
                      item={item}
                      viewer={viewer}
                      culture={settings.culture}
                      onEditComplete={this.onEditComplete}
                      onMediaFileClick={this.onMediaFileClick}
                      openDocEditor={this.openDocEditor}
                    />
                  </Tile>
                </DragAndDrop>
              );
            })}
          </TileContainer>
        ) : (
          <Consumer>
            {(context) => (
              <RowContainer
                className="files-row-container"
                draggable
                useReactWindow={false}
              >
                {items.map((item) => {
                  const { checked, isFolder, value, contextOptions } = item;
                  const sectionWidth = context.sectionWidth;
                  const isEdit =
                    !!fileAction.type &&
                    editingId === item.id &&
                    item.fileExst === fileAction.extension;
                  const contextOptionsProps =
                    !isEdit && contextOptions && contextOptions.length > 0
                      ? {
                          contextOptions: this.getFilesContextOptions(
                            contextOptions,
                            item
                          ),
                        }
                      : {};
                  const checkedProps =
                    isEdit || item.id <= 0 ? {} : { checked };
                  const element = this.getItemIcon(
                    item,
                    isEdit || item.id <= 0
                  );
                  const sharedButton =
                    isRecycleBin || isEdit || item.id <= 0 || sectionWidth < 500
                      ? null
                      : this.getSharedButton();
                  const displayShareButton =
                    sectionWidth < 500
                      ? "26px"
                      : isRecycleBin
                      ? "38px"
                      : "96px";
                  let classNameProp =
                    isFolder && item.access < 2 && !isRecycleBin
                      ? { className: " dropable" }
                      : { className: "" };

                  if (item.draggable) classNameProp.className += " draggable";

                  return (
                    <DragAndDrop
                      {...classNameProp}
                      onDrop={this.onDrop.bind(this, item)}
                      onMouseDown={this.onMouseDown}
                      dragging={dragging && isFolder && item.access < 2}
                      key={`dnd-key_${item.id}`}
                      {...contextOptionsProps}
                      value={value}
                    >
                      <SimpleFilesRow
                        sectionWidth={sectionWidth}
                        key={item.id}
                        data={item}
                        element={element}
                        contentElement={sharedButton}
                        onSelect={this.onContentRowSelect}
                        editing={editingId}
                        {...checkedProps}
                        {...contextOptionsProps}
                        needForUpdate={this.needForUpdate}
                        selectItem={this.onSelectItem.bind(this, item)}
                        contextButtonSpacerWidth={displayShareButton}
                      >
                        <FilesRowContent
                          sectionWidth={sectionWidth}
                          isMobile={isMobile}
                          item={item}
                          viewer={viewer}
                          culture={settings.culture}
                          onEditComplete={this.onEditComplete}
                          onMediaFileClick={this.onMediaFileClick}
                          onClickFavorite={this.onClickFavorite}
                          openDocEditor={this.openDocEditor}
                        />
                      </SimpleFilesRow>
                    </DragAndDrop>
                  );
                })}
              </RowContainer>
            )}
          </Consumer>
        )}
        {playlist.length > 0 && mediaViewerVisible && (
          <MediaViewer
            currentFileId={currentMediaFileId}
            allowConvert={true} //TODO
            canDelete={(fileId) => {
              return true;
            }} //TODO
            canDownload={(fileId) => {
              return true;
            }} //TODO
            visible={mediaViewerVisible}
            playlist={playlist}
            onDelete={this.onDeleteMediaFile}
            onDownload={this.onDownloadMediaFile}
            onClose={this.onMediaViewerClose}
            onEmptyPlaylistError={this.onMediaViewerClose}
            extsMediaPreviewed={mediaViewerMediaFormats} //TODO
            extsImagePreviewed={mediaViewerImageFormats} //TODO
          />
        )}
      </>
    );
  }
}

SectionBodyContent.defaultProps = {
  files: null,
};

const mapStateToProps = (state) => {
  return {
    currentFolderCount: getCurrentFolderCount(state),
    currentMediaFileId: getMediaViewerId(state),
    dragging: getDragging(state),
    dragItem: getDragItem(state),
    fileAction: getFileAction(state),
    files: getFiles(state),
    filesList: getFilesList(state)(state),
    filter: getFilter(state),
    firstLoad: getFirstLoad(state),
    folderId: getSelectedFolderId(state),
    folders: getFolders(state),
    isAdmin: isAdmin(state),
    isCommon: getIsCommonFolder(state),
    isEncryptionSupport: isEncryptionSupport(state),
    isFavorites: getIsFavoritesFolder(state),
    isLoading: getIsLoading(state),
    isMy: getIsMyFolder(state),
    isRecycleBin: getIsRecycleBinFolder(state),
    isRecent: getIsRecentFolder(state),
    isShare: getIsShareFolder(state),
    isPrivacy: getIsPrivacyFolder(state),
    mediaViewerImageFormats: getMediaViewerImageFormats(state),
    mediaViewerMediaFormats: getMediaViewerMediaFormats(state),
    mediaViewerVisible: getMediaViewerVisibility(state),
    myDocumentsId: getMyFolderId(state),
    organizationName: getOrganizationName(state),
    parentId: getSelectedFolderParentId(state),
    privacyInstructions: getPrivacyInstructionsLink(state),
    selected: getSelected(state),
    selectedFolderId: getSelectedFolderId(state),
    selection: getSelection(state),
    settings: getSettings(state),
    title: getSelectedFolderTitle(state),
    treeFolders: getTreeFolders(state),
    viewAs: getViewAs(state),
    viewer: getCurrentUser(state),
    tooltipValue: getTooltipLabel(state),
    iconOfDraggedFile: getIconOfDraggedFile(state)(state),
    sharingPanelVisible: getSharePanelVisible(state),
  };
};

export default connect(mapStateToProps, {
  deselectFile,
  updateFile,
  fetchFiles,
  selectFile,
  setAction,
  setTreeFolders,
  setDragging,
  setDragItem,
  setMediaViewerData,
  setSecondaryProgressBarData,
  setSelection,
  setSelected,
  setUpdateTree,
  setIsLoading,
  clearSecondaryProgressData,
  markItemAsFavorite,
  removeItemFromFavorite,
  fetchFavoritesFolder,
  getFileInfo,
  addFileToRecentlyViewed,
  loopFilesOperations,
  setSharingPanelVisible,
})(withRouter(withTranslation()(SectionBodyContent)));
