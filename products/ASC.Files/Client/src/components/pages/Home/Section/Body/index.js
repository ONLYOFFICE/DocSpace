import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ReactSVG } from 'react-svg'
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import copy from "copy-to-clipboard";
import styled from "styled-components";
import queryString from 'query-string';
import {
  IconButton,
  Row,
  RowContainer,
  toastr,
  Link
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";
import { api, constants, MediaViewer, DragAndDrop } from 'asc-web-common';
import {
  deleteFile,
  deleteFolder,
  deselectFile,
  fetchFiles,
  selectFile,
  setAction,
  setTreeFolders,
  moveToFolder,
  copyToFolder,
  getProgress,
  setDragging,
  setDragItem,
  setMediaViewerData
} from '../../../../../store/files/actions';
import { isFileSelected, getFileIcon, getFolderIcon, getFolderType, loopTreeFolders, isImage, isSound, isVideo } from '../../../../../store/files/selectors';
import store from "../../../../../store/store";
import { SharingPanel } from "../../../../panels";
//import { getFilterByLocation } from "../../../../../helpers/converters";
//import config from "../../../../../../package.json";

const { FilesFilter } = api;
const { FileAction } = constants;

const linkStyles = { isHovered: true, type: "action", fontSize: "14px", className: "empty-folder_link", display: "flex" };
const backgroundDragColor = "#EFEFB2";
const backgroundDragEnterColor = "#F8F7BF";

const extsMediaPreviewed = [".aac", ".flac", ".m4a", ".mp3", ".oga", ".ogg", ".wav", ".f4v", ".m4v", ".mov", ".mp4", ".ogv", ".webm", ".avi", ".mpg", ".mpeg", ".wmv"];
const extsImagePreviewed = [".bmp", ".gif", ".jpeg", ".jpg", ".png", ".ico", ".tif", ".tiff", ".webp"];

const CustomTooltip = styled.div`
  position: fixed;
  display: none;
  padding: 8px;
  z-index: 150;
  background: #FFF;
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
      currentItem: null
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
    if (this.state.showSharingPanel !== nextState.showSharingPanel) {
      return true;
    }

    if (this.props.dragItem !== nextProps.dragItem) {
      return false;
    }

    if (!isEqual(this.props, nextProps) || !isEqual(this.state.mediaViewerVisible, nextState.mediaViewerVisible)) {
      return true;
    }

    return false;
  }

  onClickRename = (item) => {
    const { id, fileExst } = item;

    this.setState({ editingId: id }, () => {
      this.props.setAction(
        {
          type: FileAction.Rename,
          extension: fileExst,
          id
        }
      );
    });
  };

  onEditComplete = item => {
    const { folderId, fileAction, filter, treeFolders, setTreeFolders, onLoading } = this.props;

    if (fileAction.type === FileAction.Create || fileAction.type === FileAction.Rename) {
      onLoading(true);
      fetchFiles(folderId, filter, store.dispatch).then(data => {
        const newItem = item.id === -1 ? null : item;
        if (!item.fileExst) {
          const path = data.selectedFolder.pathParts;
          const newTreeFolders = treeFolders;
          const folders = data.selectedFolder.folders;
          loopTreeFolders(path, newTreeFolders, folders, null, newItem);
          setTreeFolders(newTreeFolders);
        }
      }).finally(() => onLoading(false))
    }

    this.setState({ editingId: null }, () => {
      this.props.setAction({
        type: null
      });
    })
  }

  onClickDelete = (item) => {
    item.fileExst
      ? this.onDeleteFile(item.id, item.folderId)
      : this.onDeleteFolder(item.id, item.parentId);
  }

  onDeleteFile = (fileId, currentFolderId) => {
    const { deleteFile, t, startFilesOperations, finishFilesOperations } = this.props;

    startFilesOperations(t("DeleteOperation"));
    deleteFile(fileId)
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, false);
      })
      .catch(err => finishFilesOperations(err))
  }

  loopDeleteProgress = (id, folderId, isFolder) => {
    const { filter, treeFolders, setTreeFolders, currentFolderType, getProgress, setProgressValue, finishFilesOperations } = this.props;
    getProgress().then(res => {
      const deleteProgress = res.find(x => x.id === id);
      if (deleteProgress && deleteProgress.progress !== 100) {
        setProgressValue(deleteProgress.progress);
        setTimeout(() => this.loopDeleteProgress(id, folderId, isFolder), 1000);
      } else {
        fetchFiles(folderId, filter, store.dispatch).then(data => {
          if (currentFolderType !== "Trash" && isFolder) {
            const path = data.selectedFolder.pathParts.slice(0);
            const newTreeFolders = treeFolders;
            const folders = data.selectedFolder.folders;
            const foldersCount = data.selectedFolder.foldersCount;
            loopTreeFolders(path, newTreeFolders, folders, foldersCount);
            setTreeFolders(newTreeFolders);
          }
          isFolder
            ? toastr.success(`Folder moved to recycle bin`)
            : toastr.success(`File moved to recycle bin`);
          setProgressValue(100);
          finishFilesOperations();
        }).catch(err => finishFilesOperations(err))
      }
    })
  }

  onDeleteFolder = (folderId, currentFolderId) => {
    const { deleteFolder, startFilesOperations, finishFilesOperations, t } = this.props;
    startFilesOperations(t("DeleteOperation"));
    deleteFolder(folderId, currentFolderId)
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        this.loopDeleteProgress(id, currentFolderId, true);
      })
      .catch(err => finishFilesOperations(err))
  }

  onClickShare = item => {
    let currentItem = item;
    if (this.state.showSharingPanel) {
      currentItem = null;
    }
    this.setState({
      currentItem,
      showSharingPanel: !this.state.showSharingPanel,
    });
  }

  onClickLinkForPortal = item => {
    const { settings } = this.props;
    const isFile = !!item.fileExst;
    const { t } = this.props;

    copy(isFile
      ?
      this.isMediaOrImage(item.fileExst)
        ? `${window.location.origin + settings.homepage}/filter?folder=${item.folderId}&preview=${item.id}`
        : item.webUrl
      :
      `${window.location.origin + settings.homepage}/filter?folder=${item.id}`);

    toastr.success(t("LinkCopySuccess"));
  }

  onClickDownload = item => {
    return window.open(item.viewUrl, "_blank");
  }

  onClickLinkEdit = item => {
    return window.open(`./doceditor?fileId=${item.id}`, "_blank");
  }

  showVersionHistory = (e) => {
    const {settings, history} = this.props;
    const fileId = e.currentTarget.dataset.id;

    history.push(`${settings.homepage}/${fileId}/history`);
  }
  
  finalizeVersion = (e) => {
    console.log("Finalize version clicked", e);
  }

  getFilesContextOptions = (item, viewer) => {
    const { t } = this.props;
    const isFile = !!item.fileExst;

    if (item.id <= 0) return [];

    const versionHistoryMenu = isFile
      ? [
          {
            key: "show-version-history",
            label: t("ShowVersionHistory"),
            onClick: this.showVersionHistory,
            disabled: false,
            "data-id": item.id
          },
          {
            key: "finalize-version",
            label: t("FinalizeVersion"),
            onClick: this.finalizeVersion,
            disabled: false,
            "data-id": item.id
          },
          {
            key: "sep2",
            isSeparator: true
          }
        ]
      : [];

    const menu = [
      {
        key: "sharing-settings",
        label: t("SharingSettings"),
        onClick: this.onClickShare.bind(this, item),
        disabled: item.access !== 1 && item.access !== 0
      },
      isFile
        ? {
          key: "send-by-email",
          label: t("SendByEmail"),
          onClick: () => { },
          disabled: true
        }
        : null,
      {
        key: "link-for-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.onClickLinkForPortal.bind(this, item),
        disabled: false
      },
      {
        key: "sep",
        isSeparator: true
      },
      ...versionHistoryMenu,
      (isFile && !this.isMediaOrImage(item.fileExst))
        ? {
          key: "edit",
          label: t("Edit"),
          onClick: this.onClickLinkEdit.bind(this, item),
          disabled: false
        }
        : null,
      (isFile && !this.isMediaOrImage(item.fileExst))
        ? {
          key: "preview",
          label: t("Preview"),
          onClick: this.onClickLinkEdit.bind(this, item),
          disabled: true
        }
        : null,
      (isFile && this.isMediaOrImage(item.fileExst))
        ? {
          key: "view",
          label: t("View"),
          onClick: this.onMediaFileClick.bind(this, item.id),
          disabled: false
        }
        : null,
      isFile
        ? {
          key: "download",
          label: t("Download"),
          onClick: this.onClickDownload.bind(this, item),
          disabled: false
        }
        : null,
      {
        key: "rename",
        label: t("Rename"),
        onClick: this.onClickRename.bind(this, item),
        disabled: false
      },
      {
        key: "delete",
        label: t("Delete"),
        onClick: this.onClickDelete.bind(this, item),
        disabled: false
      },
    ];

    return menu;
  };

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.editing !== nextProps.editing) {
      return true;
    }
    if (!isEqual(currentProps.data, nextProps.data)) {
      return true;
    }
    return false;
  };

  onContentRowSelect = (checked, file) => {
    if (!file) return;

    if (checked) {
      this.props.selectFile(file);
    } else {
      this.props.deselectFile(file);
    }
  };

  svgLoader = () => <div style={{ width: '24px' }}></div>;

  getItemIcon = (item, isEdit) => {
    const extension = item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

    return <ReactSVG
      beforeInjection={svg => {
        svg.setAttribute('style', 'margin-top: 4px');
        isEdit && svg.setAttribute('style', 'margin: 4px 0 0 24px');
      }}
      src={icon}
      loading={this.svgLoader}
    />;
  };

  onCreate = (format) => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onResetFilter = () => {
    const { selectedFolderId, onLoading } = this.props;
    onLoading(true);
    const newFilter = FilesFilter.getDefault();
    fetchFiles(selectedFolderId, newFilter, store.dispatch).catch(err =>
      toastr.error(err)
    ).finally(() => onLoading(false));
  }

  onGoToMyDocuments = () => {
    const { filter, myDocumentsId, onLoading } = this.props;
    const newFilter = filter.clone();
    onLoading(true);
    fetchFiles(myDocumentsId, newFilter, store.dispatch).finally(() =>
      onLoading(false)
    );
  };

  onBackToParentFolder = () => {
    const { filter, parentId, onLoading } = this.props;
    const newFilter = filter.clone();
    onLoading(true);
    fetchFiles(parentId, newFilter, store.dispatch).finally(() =>
      onLoading(false)
    );
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
            onClick={this.onCreate.bind(this, "docx")}
          >
            +
          </Link>
          <Link onClick={this.onCreate.bind(this, "docx")} {...linkStyles}>
            {t("Document")},
          </Link>
          <Link onClick={this.onCreate.bind(this, "xlsx")} {...linkStyles}>
            {t("Spreadsheet")},
          </Link>
          <Link onClick={this.onCreate.bind(this, "pptx")} {...linkStyles}>
            {t("Presentation")}
          </Link>
        </div>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate.bind(this, null)}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate.bind(this, null)}>
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
            onClick={() => console.log("Create document click")}
          >
            +
          </Link>
          <Link onClick={this.onCreate.bind(this, "docx")} {...linkStyles}>
            {t("Document")},
          </Link>
          <Link onClick={this.onCreate.bind(this, "xlsx")} {...linkStyles}>
            {t("Spreadsheet")},
          </Link>
          <Link onClick={this.onCreate.bind(this, "pptx")} {...linkStyles}>
            {t("Presentation")}
          </Link>
        </div>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate.bind(this, null)}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate.bind(this, null)}>
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
    )
  }
  onMediaViewerClose = () => {
    const item = { visible: false, id: null };
    this.props.setMediaViewerData(item);
  }
  onMediaFileClick = (id) => {
    const item = { visible: true, id };
    this.props.setMediaViewerData(item);
  }
  onDownloadMediaFile = (id) => {
    if (this.props.files.length > 0) {
      let viewUrlFile = this.props.files.find(file => file.id === id).viewUrl;
      return window.open(viewUrlFile, "_blank");
    }
  }
  onDeleteMediaFile = (id) => {
    if (this.props.files.length > 0) {
      let file = this.props.files.find(file => file.id === id);
      if (file)
        this.onDeleteFile(file.id, file.folderId)
    }
  }

  onDragEnter = (item, e) => {
    const isCurrentItem = this.props.selection.find(x => x.id === item.id && x.fileExst === item.fileExst);
    if (!item.fileExst && (!isCurrentItem || e.dataTransfer.items.length)) {
      e.currentTarget.style.background = backgroundDragColor;
    }
  }

  onDragLeave = (item, e) => {
    const { selection, dragging, setDragging } = this.props;
    const isCurrentItem = selection.find(x => x.id === item.id && x.fileExst === item.fileExst);
    if (!e.dataTransfer.items.length) {
      e.currentTarget.style.background = "none";
    } else if (!item.fileExst && !isCurrentItem) {
      e.currentTarget.style.background = backgroundDragEnterColor;
    }
    if (dragging && !e.relatedTarget) { setDragging(false); }
  }

  onDrop = (item, e) => {
    if (e.dataTransfer.items.length > 0 && !item.fileExst) {
      const { setDragging, onDropZoneUpload } = this.props;
      e.currentTarget.style.background = backgroundDragEnterColor;
      setDragging(false);
      onDropZoneUpload(e, item.id);
    }
  }

  onDragOver = e => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (e.dataTransfer.items.length > 0 && !dragging) {
      setDragging(true);
    }
  }

  onDragLeaveDoc = e => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (dragging && !e.relatedTarget) {
      setDragging(false);
    }
  }

  onMouseDown = e => {
    const mouseButton = e.which ? e.which !== 1 : e.button ? e.button !== 0 : false;
    const label = e.target.getAttribute('label');
    if (mouseButton || e.target.tagName !== "DIV" || label) { return; }
    document.addEventListener("mousemove", this.onMouseMove);
    this.setTooltipPosition(e);
    const { selection, setDragging } = this.props;

    const elem = e.target.closest('.draggable');
    if (!elem) {
      return;
    }
    const value = elem.getAttribute('value');
    if (!value) {
      return;
    }
    const splitValue = value.split("_");
    let item = null;
    if (splitValue[0] === "folder") {
      item = selection.find(x => x.id === Number(splitValue[1]) && !x.fileExst);
    } else {
      item = selection.find(x => x.id === Number(splitValue[1]) && x.fileExst);
    }
    if (item) {
      setDragging(true);
    }
  }

  onMouseUp = e => {
    const { selection, dragging, setDragging, dragItem, setDragItem } = this.props;
    const mouseButton = e.which ? e.which !== 1 : e.button ? e.button !== 0 : false;
    if (mouseButton || !this.tooltipRef.current || !dragging) { return; }
    document.removeEventListener("mousemove", this.onMouseMove);
    this.tooltipRef.current.style.display = "none";

    const elem = e.target.closest('.dropable');
    if (elem && selection.length && dragging) {
      const value = elem.getAttribute('value');
      if (!value) {
        setDragging(false);
        return;
      }
      const splitValue = value.split("_");
      let item = null;
      if (splitValue[0] === "folder") {
        item = selection.find(x => x.id === Number(splitValue[1]) && !x.fileExst);
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
  }

  onMouseMove = e => {
    if (this.props.dragging) {
      const tooltip = this.tooltipRef.current;
      tooltip.style.display = "block";
      this.setTooltipPosition(e);

      const wrapperElement = document.elementFromPoint(e.clientX, e.clientY);
      if (!wrapperElement) { return; }
      const droppable = wrapperElement.closest('.dropable');

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
  }

  setTooltipPosition = e => {
    const tooltip = this.tooltipRef.current;
    if (tooltip) {
      const margin = 8;
      tooltip.style.left = e.pageX + margin + "px";
      tooltip.style.top = e.pageY + margin + "px";
    }
  };

  isMediaOrImage = (fileExst) => {
    if (extsMediaPreviewed.includes(fileExst) || extsImagePreviewed.includes(fileExst)) {
      return true
    }

    return false
  }

  onMoveTo = (destFolderId) => {
    const { selection, startFilesOperations, t, isShare, isCommon, isAdmin } = this.props;
    const folderIds = [];
    const fileIds = [];
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;

    startFilesOperations(t("MoveToOperation"));
    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id)
      } else {
        folderIds.push(item.id)
      }
    }

    if (isAdmin) {
      if (isShare) {
        this.copyTo(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
      } else {
        this.moveTo(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
      }
    } else {
      if (isShare || isCommon) {
        this.copyTo(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
      } else {
        this.moveTo(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter);
      }
    }
  }

  copyTo = (destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) => {
    const { copyToFolder, loopFilesOperations, finishFilesOperations } = this.props;

    copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, true);
      })
      .catch(err => finishFilesOperations(err))
  }

  moveTo = (destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter) => {
    const { moveToFolder, loopFilesOperations, finishFilesOperations } = this.props;

    moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopFilesOperations(id, destFolderId, false);
      })
      .catch(err => finishFilesOperations(err))
  }

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
  }

  render() {
    const {
      files,
      folders,
      viewer,
      parentId,
      folderId,
      settings,
      selection,
      fileAction,
      onLoading,
      isLoading,
      currentFolderCount,
      currentFolderType,
      dragging,
      mediaViewerVisible,
      currentMediaFileId,
      startFilesOperations,
      finishFilesOperations,
      setProgressValue
    } = this.props;

    const { editingId, showSharingPanel, currentItem } = this.state;

    let items = [...folders, ...files];

    const tooltipLabel = this.getTooltipLabel();

    if (fileAction && fileAction.type === FileAction.Create) {
      items.unshift({
        id: -1,
        title: "",
        parentId: folderId,
        fileExst: fileAction.extension,
      });
    }


    var playlist = [];
    let id = 0;
    files.forEach(function (file, i, files) {
      if (isImage(file.fileExst) || isSound(file.fileExst) || isVideo(file.fileExst)) {
        playlist.push(
          {
            id: id,
            fileId: file.id,
            src: file.viewUrl,
            title: file.title
          }
        );
        id++;
      }
    });

    return !fileAction.id && currentFolderCount === 0 ? (
      parentId === 0 ? (
        this.renderEmptyRootFolderContainer()
      ) : (
          this.renderEmptyFolderContainer()
        )
    ) : !fileAction.id && items.length === 0 ? (
      this.renderEmptyFilterContainer()
    ) : (
          <>
            <CustomTooltip ref={this.tooltipRef}>{tooltipLabel}</CustomTooltip>
            <RowContainer draggable useReactWindow={false}>
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
                const checkedProps = (isEdit || item.id <= 0) ? {} : { checked };
                const element = this.getItemIcon(item, (isEdit || item.id <= 0));

                const selectedItem = selection.find(x => x.id === item.id && x.fileExst === item.fileExst);
                const isFolder = selectedItem ? false : item.fileExst ? false : true;
                const draggable = selectedItem && currentFolderType !== "Trash";
                let value = item.fileExst ? `file_${item.id}` : `folder_${item.id}`;
                value += draggable ? "_draggable" : "";
                const classNameProp = isFolder && item.access < 2 ? { className: " dropable" } : {};

                return (
                  <DragAndDrop
                    {...classNameProp}
                    onDrop={this.onDrop.bind(this, item)}
                    onDragEnter={this.onDragEnter.bind(this, item)}
                    onDragLeave={this.onDragLeave.bind(this, item)}
                    onMouseDown={this.onMouseDown}
                    dragging={dragging && isFolder && item.access < 2}
                    key={`dnd-key_${item.id}`}
                    {...contextOptionsProps}
                    value={value}
                  >
                    <SimpleFilesRow
                      key={item.id}
                      data={item}
                      element={element}
                      onSelect={this.onContentRowSelect}
                      editing={editingId}
                      {...checkedProps}
                      {...contextOptionsProps}
                      needForUpdate={this.needForUpdate}
                    >
                      <FilesRowContent
                        item={item}
                        viewer={viewer}
                        culture={settings.culture}
                        onEditComplete={this.onEditComplete.bind(this, item)}
                        onLoading={onLoading}
                        onMediaFileClick={this.onMediaFileClick}
                        isLoading={isLoading}
                        setProgressValue={setProgressValue}
                        startFilesOperations={startFilesOperations}
                        finishFilesOperations={finishFilesOperations}
                      />
                    </SimpleFilesRow>
                  </DragAndDrop>
                );
              })}
            </RowContainer>
            {playlist.length > 0 && mediaViewerVisible &&
              <MediaViewer
                currentFileId={currentMediaFileId}
                allowConvert={true} //TODO
                canDelete={(fileId) => { return true }} //TODO 
                canDownload={(fileId) => { return true }} //TODO 
                visible={mediaViewerVisible}
                playlist={playlist}
                onDelete={this.onDeleteMediaFile}
                onDownload={this.onDownloadMediaFile}
                onClose={this.onMediaViewerClose}
                onEmptyPlaylistError={this.onMediaViewerClose}
                extsMediaPreviewed={extsMediaPreviewed}  //TODO
                extsImagePreviewed={extsImagePreviewed} //TODO
              />
            }
            {showSharingPanel && (
              <SharingPanel
                onLoading={onLoading}
                selectedItems={currentItem}
                onClose={this.onClickShare}
                visible={showSharingPanel}
              />
            )}
          </>
        );
  }
}

SectionBodyContent.defaultProps = {
  files: null
};

const mapStateToProps = state => {
  const { selectedFolder, treeFolders, selection, dragItem, mediaViewerData, dragging } = state.files;
  const { id, title, foldersCount, filesCount, pathParts } = selectedFolder;
  const currentFolderType = getFolderType(id, treeFolders);

  const myFolderIndex = 0;
  const shareFolderIndex = 1;
  const commonFolderIndex = 2;
  const currentFolderCount = filesCount + foldersCount;
  const myDocumentsId = treeFolders[myFolderIndex].id;
  const shareFolderId = treeFolders[shareFolderIndex].id;
  const commonFolderId = treeFolders[commonFolderIndex].id;

  return {
    fileAction: state.files.fileAction,
    files: state.files.files,
    filter: state.files.filter,
    folderId: state.files.selectedFolder.id,
    folders: state.files.folders,
    parentId: state.files.selectedFolder.parentId,
    selected: state.files.selected,
    selection,
    settings: state.auth.settings,
    viewer: state.auth.user,
    treeFolders,
    currentFolderType,
    title,
    myDocumentsId,
    currentFolderCount,
    selectedFolderId: id,
    dragItem,
    isShare: pathParts[0] === shareFolderId,
    isCommon: pathParts[0] === commonFolderId,
    isAdmin: state.auth.user.isAdmin,
    mediaViewerVisible: mediaViewerData.visible,
    currentMediaFileId: mediaViewerData.id,
    dragging
  };
};

export default connect(
  mapStateToProps,
  {
    deleteFile,
    deleteFolder,
    deselectFile,
    fetchFiles,
    //fetchRootFolders,
    selectFile,
    setAction,
    setTreeFolders,
    moveToFolder,
    copyToFolder,
    getProgress,
    setDragging,
    setDragItem,
    setMediaViewerData
  }
)(withRouter(withTranslation()(SectionBodyContent)));
