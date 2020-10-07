import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ReactSVG } from "react-svg";
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import copy from "copy-to-clipboard";
import styled from "styled-components";
import queryString from "query-string";
import {
  IconButton,
  Row,
  RowContainer,
  Link,
  DragAndDrop,
  Box,
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
  clearProgressData,
  deselectFile,
  fetchFiles,
  selectFile,
  setAction,
  setDragging,
  setDragItem,
  setIsLoading,
  setMediaViewerData,
  setUpdateTree,
  setProgressBarData,
  setSelected,
  setSelection,
  setTreeFolders,
} from "../../../../../store/files/actions";
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
  getIsAdmin,
  getIsLoading,
  getMediaViewerId,
  getMediaViewerVisibility,
  getSelectedFolderParentId,
  getPathParts,
  getSelected,
  getSelectedFolderTitle,
  getSelectedFolderType,
  getSelection,
  getSettings,
  getTreeFolders,
  getViewAs,
  getViewer,
  isFileSelected,
  isImage,
  isSound,
  isVideo,
  loopTreeFolders,
  getFilesList,
  isMediaOrImage,
  getMediaViewerFormats,
} from "../../../../../store/files/selectors";
import { SharingPanel, OperationsPanel } from "../../../../panels";
const { isAdmin } = store.auth.selectors;
//import { getFilterByLocation } from "../../../../../helpers/converters";
//import config from "../../../../../../package.json";

const { FilesFilter } = api;
const { FileAction } = constants;

const linkStyles = {
  isHovered: true,
  type: "action",
  fontSize: "13px",
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
  -moz-border-radius: 6px;
  -webkit-border-radius: 6px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const SimpleFilesRow = styled(Row)`
  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      editingId: null,
      showSharingPanel: false,
      showMoveToPanel: false,
      showCopyPanel: false,
      isDrag: false,
    };

    this.tooltipRef = React.createRef();
    this.currentDroppable = null;
  }

  componentDidMount() {
    //const { fetchFiles } = this.props;

    //TODO: use right algorithm, fix fetching in src/index.html

    // var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    // const match = window.location.pathname.match(re);

    // if (match && match.length > 0) {
    //   const newFilter = getFilterByLocation(window.location);
    //   if (newFilter) {
    //     return fetchFiles(newFilter)
    //       .catch(error => toastr.error(error));
    //   } else {
    //     const filter = FilesFilter.getDefault();

    //     fetchFiles(filter)
    //       .catch(error => toastr.error(error));
    //   }
    // }
    let previewId = queryString.parse(this.props.location.search).preview;

    if (previewId) {
      this.onMediaFileClick(+previewId);
    }

    window.addEventListener("mouseup", this.onMouseUp);

    document.addEventListener("dragover", this.onDragOver);
    document.addEventListener("dragleave", this.onDragLeaveDoc);
  }

  componentWillUnmount() {
    window.removeEventListener("mouseup", this.onMouseUp);

    document.removeEventListener("dragover", this.onDragOver);
    document.removeEventListener("dragleave", this.onDragLeaveDoc);
  }

  /* componentDidUpdate(prevProps, prevState) {
    Object.entries(this.props).forEach(([key, val]) =>
      prevProps[key] !== val && console.log(`Prop '${key}' changed`)
    );
    if (this.state) {
      Object.entries(this.state).forEach(([key, val]) =>
        prevState[key] !== val && console.log(`State '${key}' changed`)
      );
    }
  } */

  shouldComponentUpdate(nextProps, nextState) {
    if (this.props && this.props.firstLoad) return true;

    const { showMoveToPanel, showCopyPanel, isDrag } = this.state;
    if (this.state.showSharingPanel !== nextState.showSharingPanel) {
      return true;
    }

    if (this.props.dragItem !== nextProps.dragItem) {
      return false;
    }

    if (
      !isEqual(this.props, nextProps) ||
      !isEqual(this.state.mediaViewerVisible, nextState.mediaViewerVisible)
    ) {
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

  onEditComplete = (id) => {
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
          if (item && !item.fileExst) {
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
    const { t, setProgressBarData, clearProgressData } = this.props;
    setProgressBarData({
      visible: true,
      percent: 0,
      label: t("DeleteOperation"),
    });
    api.files
      .deleteFile(fileId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false);
      })
      .catch((err) => {
        toastr.error(err);
        clearProgressData();
      });
  };

  loopDeleteProgress = (id, folderId, isFolder) => {
    const {
      filter,
      treeFolders,
      setTreeFolders,
      currentFolderType,
      t,
      setProgressBarData,
      fetchFiles,
      setUpdateTree,
    } = this.props;
    api.files.getProgress().then((res) => {
      const deleteProgress = res.find((x) => x.id === id);
      if (deleteProgress && deleteProgress.progress !== 100) {
        setProgressBarData({
          visible: true,
          percent: deleteProgress.progress,
          label: t("DeleteOperation"),
        });
        setTimeout(() => this.loopDeleteProgress(id, folderId, isFolder), 1000);
      } else {
        setProgressBarData({
          visible: true,
          percent: 100,
          label: t("DeleteOperation"),
        });
        fetchFiles(folderId, filter)
          .then((data) => {
            if (currentFolderType !== "Trash" && isFolder) {
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
            toastr.error(err);
            this.props.clearProgressData();
          })
          .finally(() =>
            setTimeout(() => this.props.clearProgressData(), 5000)
          );
      }
    });
  };

  onDeleteFolder = (folderId, currentFolderId) => {
    const { t, setProgressBarData, clearProgressData } = this.props;
    const progressLabel = t("DeleteOperation");
    setProgressBarData({ visible: true, percent: 0, label: progressLabel });
    api.files
      .deleteFolder(folderId, currentFolderId)
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true);
      })
      .catch((err) => {
        toastr.error(err);
        clearProgressData();
      });
  };

  onClickShare = () =>
    this.setState({ showSharingPanel: !this.state.showSharingPanel });

  onClickLinkForPortal = () => {
    const { settings, selection } = this.props;
    const item = selection[0];
    const isFile = !!item.fileExst;
    const { t } = this.props;

    copy(
      isFile
        ? isMediaOrImage(item.fileExst)
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

  onClickLinkEdit = (e) => {
    const id = e.currentTarget.dataset.id;
    return window.open(`./doceditor?fileId=${id}`, "_blank");
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
    const { selection, selectedFolderId, setProgressBarData, t } = this.props;
    const folderIds = [];
    const fileIds = [];
    selection[0].fileExst
      ? fileIds.push(selection[0].id)
      : folderIds.push(selection[0].id);
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = false;

    setProgressBarData({
      visible: true,
      percent: 0,
      label: t("CopyOperation"),
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
        case "selector0":
        case "selector1":
          return { key: option, isSeparator: true };
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
            icon: "DownloadAsIcon",
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
    if (!isEqual(currentProps.data, nextProps.data)) {
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
    const { fileAction } = this.props;

    const actionExtension = fileAction.extension && `.${fileAction.extension}`;
    const extension = isEdit ? actionExtension : item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px");
          isEdit && svg.setAttribute("style", "margin: 4px 0 0 24px");
        }}
        src={icon}
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
    const { currentFolderType, title, t } = this.props;
    const subheadingText = t("SubheadingEmptyText");
    const myDescription = t("MyEmptyContainerDescription");
    const shareDescription = t("SharedEmptyContainerDescription");
    const commonDescription = t("CommonEmptyContainerDescription");
    const trashDescription = t("TrashEmptyContainerDescription");

    const commonButtons = (
      <>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            noHover
            data-format="docx"
            onClick={this.onCreate}
          >
            +
          </Link>

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
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate}>
            {t("Folder")}
          </Link>
        </div>
      </>
    );

    const trashButtons = (
      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_up-image"
          src="images/empty_screen_people.svg"
          alt=""
          onClick={this.onGoToMyDocuments}
        />
        <Link onClick={this.onGoToMyDocuments} {...linkStyles}>
          {t("GoToMyButton")}
        </Link>
      </div>
    );

    switch (currentFolderType) {
      case "My":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={myDescription}
            imageSrc="images/empty_screen.png"
            buttons={commonButtons}
          />
        );
      case "Share":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={shareDescription}
            imageSrc="images/empty_screen_forme.png"
          />
        );
      case "Common":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={commonDescription}
            imageSrc="images/empty_screen_corporate.png"
            buttons={commonButtons}
          />
        );
      case "Trash":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={trashDescription}
            imageSrc="images/empty_screen_trash.png"
            buttons={trashButtons}
          />
        );
      default:
        return;
    }
  };

  renderEmptyFolderContainer = () => {
    const { t } = this.props;
    const buttons = (
      <>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            noHover
            data-format="docx"
            onClick={this.onCreate}
          >
            +
          </Link>

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
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate}>
            {t("Folder")}
          </Link>
        </div>

        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_up-image"
            src="images/up.svg"
            onClick={this.onBackToParentFolder}
            alt=""
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
          color="A3A9AE"
        />
        <Link onClick={this.onResetFilter} {...linkStyles}>
          {this.props.t("ClearButton")}
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

  onDrop = (item, items, e) => {
    if (!item.fileExst) {
      const { setDragging, onDropZoneUpload } = this.props;
      setDragging(false);
      onDropZoneUpload(items, e, item.id);
    }
  };

  onDragOver = (e) => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (e.dataTransfer.items.length > 0 && !dragging) {
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
    this.state.isDrag && this.setState({ isDrag: false });
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
      setProgressBarData,
    } = this.props;
    const folderIds = [];
    const fileIds = [];
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;

    setProgressBarData({
      visible: true,
      percent: 0,
      label: t("MoveToOperation"),
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
    const { loopFilesOperations, clearProgressData } = this.props;

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
        toastr.error(err);
        clearProgressData();
      });
  };

  moveTo = (
    destFolderId,
    folderIds,
    fileIds,
    conflictResolveType,
    deleteAfter
  ) => {
    const { loopFilesOperations, clearProgressData } = this.props;

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
        toastr.error(err);
        clearProgressData();
      });
  };

  getTooltipLabel = () => {
    const { t, selection, isAdmin, isShare, isCommon } = this.props;
    const elementTitle = selection.length && selection[0].title;
    const elementCount = selection.length;
    if (selection.length) {
      if (selection.length > 1) {
        if (isAdmin) {
          if (isShare) {
            return t("TooltipElementsCopyMessage", { element: elementCount });
          } else {
            return t("TooltipElementsMoveMessage", { element: elementCount });
          }
        } else {
          if (isShare || isCommon) {
            return t("TooltipElementsCopyMessage", { element: elementCount });
          } else {
            return t("TooltipElementsMoveMessage", { element: elementCount });
          }
        }
      } else {
        if (isAdmin) {
          if (isShare) {
            return t("TooltipElementCopyMessage", { element: elementTitle });
          } else {
            return t("TooltipElementMoveMessage", { element: elementTitle });
          }
        } else {
          if (isShare || isCommon) {
            return t("TooltipElementCopyMessage", { element: elementTitle });
          } else {
            return t("TooltipElementMoveMessage", { element: elementTitle });
          }
        }
      }
    }
  };

  onSelectItem = (item) => {
    const { selected, setSelected, setSelection } = this.props;
    selected === "close" && setSelected("none");
    setSelection([item]);
  };

  onCreateAddTempItem = (items, folderId, fileAction) => {
    if (items.length && items[0].id === -1) return; //TODO: if change media collection from state remove this;
    items.unshift({
      id: -1,
      title: "",
      parentId: folderId,
      fileExst: fileAction.extension,
    });
  };

  render() {
    const {
      files,
      viewer,
      parentId,
      folderId,
      settings,
      selection,
      fileAction,
      setIsLoading,
      isLoading,
      currentFolderCount,
      currentFolderType,
      dragging,
      mediaViewerVisible,
      currentMediaFileId,
      viewAs,
      t,
      loopFilesOperations,
      widthProp,
      isMobile,
      firstLoad,
      filesList,
      mediaFormats,
    } = this.props;

    const {
      editingId,
      showSharingPanel,
      showMoveToPanel,
      showCopyPanel,
    } = this.state;

    const operationsPanelProps = {
      setIsLoading,
      isLoading,
      loopFilesOperations,
    };

    const items = filesList;

    const tooltipLabel = this.getTooltipLabel();

    if (fileAction && fileAction.type === FileAction.Create) {
      this.onCreateAddTempItem(items, folderId, fileAction);
    }

    var playlist = [];
    let id = 0;

    if (files) {
      files.forEach(function (file, i, files) {
        if (
          isImage(file.fileExst) ||
          isSound(file.fileExst) ||
          isVideo(file.fileExst)
        ) {
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

    return !fileAction.id && currentFolderCount === 0 ? (
      parentId === 0 ? (
        this.renderEmptyRootFolderContainer()
      ) : (
        this.renderEmptyFolderContainer()
      )
    ) : !fileAction.id && items.length === 0 ? (
      firstLoad ? (
        <Loaders.Rows />
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
        <CustomTooltip ref={this.tooltipRef}>{tooltipLabel}</CustomTooltip>

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
              const draggable = selectedItem && currentFolderType !== "Trash";
              let value = item.fileExst
                ? `file_${item.id}`
                : `folder_${item.id}`;
              value += draggable ? "_draggable" : "";
              const classNameProp =
                isFolder && item.access < 2 ? { className: " dropable" } : {};

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
                    />
                  </Tile>
                </DragAndDrop>
              );
            })}
          </TileContainer>
        ) : (
          <RowContainer draggable useReactWindow={false}>
            {items.map((item) => {
              const { checked, isFolder, value, contextOptions } = item;
              const isEdit =
                !!fileAction.type &&
                editingId === item.id &&
                item.fileExst === fileAction.extension;
              const contextOptionsProps =
                contextOptions && contextOptions.length > 0
                  ? {
                      contextOptions: this.getFilesContextOptions(
                        contextOptions,
                        item
                      ),
                    }
                  : {};
              const checkedProps = isEdit || item.id <= 0 ? {} : { checked };
              const element = this.getItemIcon(item, isEdit || item.id <= 0);
              const classNameProp =
                isFolder && item.access < 2 ? { className: " dropable" } : {};
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
                    widthProp={widthProp}
                    key={item.id}
                    data={item}
                    element={element}
                    onSelect={this.onContentRowSelect}
                    editing={editingId}
                    {...checkedProps}
                    {...contextOptionsProps}
                    needForUpdate={this.needForUpdate}
                    selectItem={this.onSelectItem.bind(this, item)}
                  >
                    <FilesRowContent
                      widthProp={widthProp}
                      isMobile={isMobile}
                      item={item}
                      viewer={viewer}
                      culture={settings.culture}
                      onEditComplete={this.onEditComplete}
                      onMediaFileClick={this.onMediaFileClick}
                    />
                  </SimpleFilesRow>
                </DragAndDrop>
              );
            })}
          </RowContainer>
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
            extsMediaPreviewed={mediaFormats.extsMediaPreviewed} //TODO
            extsImagePreviewed={mediaFormats.extsImagePreviewed} //TODO
          />
        )}
        {showSharingPanel && (
          <SharingPanel
            onClose={this.onClickShare}
            visible={showSharingPanel}
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
  const pathParts = getPathParts(state);
  const treeFolders = getTreeFolders(state);

  const myFolderIndex = 0;
  const shareFolderIndex = 1;
  const commonFolderIndex = 2;

  const myDocumentsId =
    treeFolders.length &&
    treeFolders[myFolderIndex] &&
    treeFolders[myFolderIndex].id;
  const shareFolderId =
    treeFolders.length &&
    treeFolders[shareFolderIndex] &&
    treeFolders[shareFolderIndex].id;
  const commonFolderId =
    treeFolders.length &&
    treeFolders[commonFolderIndex] &&
    treeFolders[commonFolderIndex].id;
  const isShare = pathParts && pathParts[0] === shareFolderId;
  const isCommon = pathParts && pathParts[0] === commonFolderId;

  return {
    currentFolderCount: getCurrentFolderCount(state),
    currentFolderType: getSelectedFolderType(state),
    currentMediaFileId: getMediaViewerId(state),
    dragging: getDragging(state),
    dragItem: getDragItem(state),
    fileAction: getFileAction(state),
    files: getFiles(state),
    filter: getFilter(state),
    firstLoad: getFirstLoad(state),
    folderId: getSelectedFolderId(state),
    folders: getFolders(state),
    isAdmin: isAdmin(state),
    isCommon,
    isLoading: getIsLoading(state),
    isShare,
    mediaViewerVisible: getMediaViewerVisibility(state),
    myDocumentsId,
    parentId: getSelectedFolderParentId(state),
    selected: getSelected(state),
    selectedFolderId: getSelectedFolderId(state),
    selection: getSelection(state),
    settings: getSettings(state),
    title: getSelectedFolderTitle(state),
    treeFolders,
    viewAs: getViewAs(state),
    viewer: getViewer(state),
    filesList: getFilesList(state),
    mediaFormats: getMediaViewerFormats(state),
  };
};

export default connect(mapStateToProps, {
  deselectFile,
  fetchFiles,
  selectFile,
  setAction,
  setTreeFolders,
  setDragging,
  setDragItem,
  setMediaViewerData,
  setProgressBarData,
  setSelection,
  setSelected,
  setUpdateTree,
  setIsLoading,
  clearProgressData,
})(withRouter(withTranslation()(SectionBodyContent)));
