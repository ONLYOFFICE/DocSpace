import React from "react";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import styled from "styled-components";
import queryString from "query-string";
import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesFilter from "@appserver/common/api/files/filter";
import { getProgress, moveToFolder } from "@appserver/common/api/files";
import { FileAction } from "@appserver/common/constants";
import MediaViewer from "@appserver/common/components/MediaViewer";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import { TIMEOUT } from "../../../../../helpers/constants";
import { loopTreeFolders } from "../../../../../helpers/files-helpers";
import { isMobile } from "react-device-detect";

import { observer, inject } from "mobx-react";
import history from "@appserver/common/history";
import FilesRowContainer from "./FilesRow/FilesRowContainer";
import FilesTileContainer from "./FilesTile/FilesTileContainer";

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

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isDrag: false,
      canDrag: true,
    };

    this.tooltipRef = React.createRef();
    this.currentDroppable = null;
  }

  componentDidMount() {
    this.customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    let previewId = queryString.parse(this.props.location.search).preview;

    if (previewId) {
      this.removeQuery("preview");
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
      isDesktop,
      isEncryptionSupport,
      organizationName,
      privacyInstructions,
      title,
      t,
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
        {!isDesktop && (
          <Text fontSize="12px">
            <Trans i18nKey="PrivateRoomSupport" ns="Home">
              Work in Private Room is available via {{ organizationName }}
              desktop app.
              <Link isBold isHovered color="#116d9d" href={privacyInstructions}>
                Instructions
              </Link>
            </Trans>
          </Text>
        )}
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
          buttons={isDesktop && isEncryptionSupport && commonButtons}
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
          iconName="static/images/cross.react.svg"
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
    const { files, deleteFileAction, t } = this.props;

    const translations = {
      deleteOperation: t("DeleteOperation"),
      folderRemoved: t("FolderRemoved"),
      fileRemoved: t("FileRemoved"),
    };

    if (files.length > 0) {
      let file = files.find((file) => file.id === id);
      if (file) deleteFileAction(file.id, file.folderId, translations);
    }
  };

  onDragStart = (e) => {
    if (e.dataTransfer.dropEffect === "none") {
      this.state.canDrag && this.setState({ canDrag: false });
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
    let splitValue = value.split("_");
    let item = null;
    if (splitValue[0] === "folder") {
      splitValue.splice(0, 1);
      if (splitValue[splitValue.length - 1] === "draggable") {
        splitValue.splice(-1, 1);
      }
      splitValue = splitValue.join("_");

      item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
    } else {
      splitValue.splice(0, 1);
      if (splitValue[splitValue.length - 1] === "draggable") {
        splitValue.splice(-1, 1);
      }
      splitValue = splitValue.join("_");

      item = selection.find((x) => x.id + "" === splitValue && x.fileExst);
    }
    if (item) {
      this.setState({ isDrag: true });
    }
  };

  onMouseUp = (e) => {
    const { selection, dragging, setDragging, dragItem } = this.props;

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
      let splitValue = value.split("_");
      let item = null;
      if (splitValue[0] === "folder") {
        splitValue.splice(0, 1);
        if (splitValue[splitValue.length - 1] === "draggable") {
          splitValue.splice(-1, 1);
        }
        splitValue = splitValue.join("_");

        item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
      } else {
        return;
      }
      if (item) {
        setDragging(false);
        return;
      } else {
        setDragging(false);
        this.onMoveTo(splitValue);
        return;
      }
    } else {
      setDragging(false);
      if (dragItem) {
        this.onMoveTo(dragItem);
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
      copyToAction,
      moveToAction,
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
        copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    } else {
      if (isShare || isCommon) {
        copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    }
  };

  removeQuery = (queryName) => {
    const { location } = this.props;
    const queryParams = new URLSearchParams(location.search);

    if (queryParams.has(queryName)) {
      queryParams.delete(queryName);
      history.replace({
        search: queryParams.toString(),
      });
    }
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

  startMoveOperation = () => {
    this.props.moveToAction(this.props.dragItem);
    this.onCloseThirdPartyMoveDialog();
  };

  startCopyOperation = () => {
    this.props.copyToAction(this.props.dragItem);
    this.onCloseThirdPartyMoveDialog();
  };

  render() {
    //console.log("Files Home SectionBodyContent render", this.props);

    const {
      parentId,
      selection,
      fileActionId,
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
      filter,
      isLoading,
    } = this.props;

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

    const { authorType, search, withSubfolders, filterType } = filter;
    const isFiltered = authorType || search || !withSubfolders || filterType;

    return (!fileActionId && items.length === 0) || null ? (
      firstLoad ? (
        <Loaders.Rows />
      ) : isFiltered ? (
        this.renderEmptyFilterContainer()
      ) : parentId === 0 || (isPrivacy && !isEncryptionSupport) ? (
        this.renderEmptyRootFolderContainer()
      ) : (
        this.renderEmptyFolderContainer()
      )
    ) : isMobile && isLoading ? (
      <Loaders.Rows />
    ) : (
      <>
        <CustomTooltip ref={this.tooltipRef}>{fileMoveTooltip}</CustomTooltip>

        {viewAs === "tile" ? <FilesTileContainer /> : <FilesRowContainer />}
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

export default inject(
  ({
    auth,
    initFilesStore,
    filesStore,
    mediaViewerDataStore,
    formatsStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
    filesActionsStore,
  }) => {
    const { mediaViewersFormatsStore } = formatsStore;
    const { secondaryProgressDataStore } = uploadDataStore;
    const {
      isEncryptionSupport,
      organizationName,
      isDesktopClient,
    } = auth.settingsStore;
    const {
      dragging,
      setDragging,
      setIsLoading,
      viewAs,
      dragItem,
      privacyInstructions,
      tooltipValue,
    } = initFilesStore;
    const {
      files,
      firstLoad,
      filesList,
      fetchFiles,
      selection,
      filter,
      fileActionStore,
      iconOfDraggedFile,
    } = filesStore;

    const {
      myFolderId,
      isMyFolder,
      isRecycleBinFolder,
      isShareFolder,
      isFavoritesFolder,
      isCommonFolder,
      isRecentFolder,
      isPrivacyFolder,
    } = treeFoldersStore;

    const { id: fileActionId, setAction } = fileActionStore;
    const { setSecondaryProgressBarData } = secondaryProgressDataStore;

    const { images, media } = mediaViewersFormatsStore;
    const {
      id: currentMediaFileId,
      visible: mediaViewerVisible,
      setMediaViewerData,
    } = mediaViewerDataStore;

    return {
      isAdmin: auth.isAdmin,
      isEncryptionSupport,
      organizationName,
      isDesktop: isDesktopClient,
      dragging,
      fileActionId,
      files,
      firstLoad,
      filesList,
      title: selectedFolderStore.title,
      parentId: selectedFolderStore.parentId,
      selectedFolderId: selectedFolderStore.id,
      selection,
      isRecycleBin: isRecycleBinFolder,
      myDocumentsId: myFolderId,
      isShare: isShareFolder,
      isFavorites: isFavoritesFolder,
      isCommon: isCommonFolder,
      isRecent: isRecentFolder,
      isMy: isMyFolder,
      isPrivacy: isPrivacyFolder,
      filter,
      viewAs,
      dragItem,
      currentMediaFileId,
      mediaViewerVisible,
      privacyInstructions,
      mediaViewerImageFormats: images,
      mediaViewerMediaFormats: media,
      iconOfDraggedFile,
      tooltipValue,

      setDragging,
      setAction,
      setIsLoading,
      fetchFiles,
      setMediaViewerData,
      setSecondaryProgressBarData,
      copyToAction: filesActionsStore.copyToAction,
      moveToAction: filesActionsStore.moveToAction,
      deleteFileAction: filesActionsStore.deleteFileAction,
    };
  }
)(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
