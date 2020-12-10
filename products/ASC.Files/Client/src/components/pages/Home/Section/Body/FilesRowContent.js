import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  RowContent,
  Link,
  Text,
  Icons,
  IconButton,
  Badge,
} from "asc-web-components";
import { constants, api, toastr, store as initStore } from "asc-web-common";
import {
  clearSecondaryProgressData,
  createFile,
  createFolder,
  fetchFiles,
  renameFolder,
  setIsLoading,
  setNewRowItems,
  setSecondaryProgressBarData,
  setTreeFolders,
  setUpdateTree,
  updateFile,
} from "../../../../../store/files/actions";
import { TIMEOUT } from "../../../../../helpers/constants";
import {
  canConvert,
  canWebEdit,
  getDragging,
  getFileAction,
  getFilter,
  getFolders,
  getIsLoading,
  getIsRecycleBinFolder,
  getNewRowItems,
  getRootFolderId,
  getSelectedFolder,
  getSelectedFolderNew,
  getSelectedFolderParentId,
  getTitleWithoutExst,
  getTreeFolders,
  isImage,
  isSound,
  isVideo,
} from "../../../../../store/files/selectors";
import { NewFilesPanel } from "../../../../panels";
import { ConvertDialog } from "../../../../dialogs";
import EditingWrapperComponent from "./EditingWrapperComponent";
import { isMobile } from "react-device-detect";

const { FileAction } = constants;
const sideColor = "#A3A9AE";
const { getSettings } = initStore.auth.selectors;

const SimpleFilesRowContent = styled(RowContent)`
  .badge-ext {
    margin-left: -8px;
    margin-right: 8px;
  }

  .badge {
    height: 14px;
    width: 14px;
    margin-right: 6px;
  }

  .badges {
    display: flex;
    align-items: center;
  }

  .favorite {
    cursor: pointer;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const okIcon = (
  <Icons.CheckIcon
    className="edit-ok-icon"
    size="scale"
    isfill={true}
    color="#A3A9AE"
    hoveredcolor="#657077"
  />
);

const cancelIcon = (
  <Icons.CrossIcon
    className="edit-cancel-icon"
    size="scale"
    isfill={true}
    color="#A3A9AE"
    hoveredcolor="#657077"
  />
);

class FilesRowContent extends React.PureComponent {
  constructor(props) {
    super(props);
    let titleWithoutExt = getTitleWithoutExst(props.item);

    if (props.fileAction.id === -1) {
      titleWithoutExt = this.getDefaultName(props.fileAction.extension);
    }

    this.state = {
      itemTitle: titleWithoutExt,
      editingId: props.fileAction.id,
      showNewFilesPanel: false,
      newFolderId: [],
      newItems: props.item.new || props.item.fileStatus === 2,
      showConvertDialog: false,
      //loading: false
    };
  }

  completeAction = (id) => {
    this.props.onEditComplete(id, !this.props.item.fileExst);
  };

  updateItem = (e) => {
    const {
      fileAction,
      updateFile,
      renameFolder,
      item,
      setIsLoading,
    } = this.props;

    const { itemTitle } = this.state;
    const originalTitle = getTitleWithoutExst(item);

    setIsLoading(true);
    if (originalTitle === itemTitle) return this.completeAction(fileAction.id);

    item.fileExst
      ? updateFile(fileAction.id, itemTitle)
          .then(() => this.completeAction(fileAction.id))
          .finally(() => setIsLoading(false))
      : renameFolder(fileAction.id, itemTitle)
          .then(() => this.completeAction(fileAction.id))
          .finally(() => setIsLoading(false));
  };

  createItem = (e) => {
    const {
      createFile,
      createFolder,
      item,
      setIsLoading,
      openDocEditor,
    } = this.props;
    const { itemTitle } = this.state;

    setIsLoading(true);

    const itemId = e.currentTarget.dataset.itemid;

    if (itemTitle.trim() === "") return this.completeAction(itemId);

    let tab = item.fileExst ? window.open("about:blank", "_blank") : null;

    !item.fileExst
      ? createFolder(item.parentId, itemTitle)
          .then(() => this.completeAction(itemId))
          .finally(() => setIsLoading(false))
      : createFile(item.parentId, `${itemTitle}.${item.fileExst}`)
          .then((file) => {
            openDocEditor(file.id, tab, file.webUrl);
            this.completeAction(itemId);
          })
          .finally(() => setIsLoading(false));
  };

  componentDidUpdate(prevProps) {
    const { fileAction, item, newRowItems, setNewRowItems } = this.props;
    const itemId = item.id.toString();

    if (newRowItems.length && newRowItems.includes(itemId)) {
      const rowItems = newRowItems.filter((x) => x !== itemId);
      if (this.state.newItems !== 0) {
        this.setState({ newItems: 0 }, () => setNewRowItems(rowItems));
      }
    }

    if (fileAction) {
      if (fileAction.id !== prevProps.fileAction.id) {
        this.setState({ editingId: fileAction.id });
      }
    }
  }

  renameTitle = (e) => {
    this.setState({ itemTitle: e.target.value });
  };

  cancelUpdateItem = (e) => {
    this.completeAction(e);
  };

  onClickUpdateItem = (e) => {
    this.props.fileAction.type === FileAction.Create
      ? this.createItem(e)
      : this.updateItem(e);
  };

  onFilesClick = () => {
    const {
      filter,
      parentFolder,
      setIsLoading,
      onMediaFileClick,
      fetchFiles,
      isImage,
      isSound,
      isVideo,
      canWebEdit,
      item,
      isTrashFolder,
      openDocEditor,
    } = this.props;
    const { id, fileExst, viewUrl } = item;

    if (isTrashFolder) return;

    if (!fileExst) {
      setIsLoading(true);
      const newFilter = filter.clone();

      if (!newFilter.treeFolders.includes(parentFolder.toString())) {
        newFilter.treeFolders.push(parentFolder.toString());
      }

      fetchFiles(id, newFilter)
        .catch((err) => {
          toastr.error(err);
          setIsLoading(false);
        })
        .finally(() => setIsLoading(false));
    } else {
      if (canWebEdit) {
        return openDocEditor(id);
      }

      if (isImage || isSound || isVideo) {
        onMediaFileClick(id);
        return;
      }

      return window.open(viewUrl, "_blank");
    }
  };

  onMobileRowClick = (e) => {
    const { isTrashFolder } = this.props;

    if (isTrashFolder || window.innerWidth > 1024) return;

    this.onFilesClick();
  };

  getStatusByDate = () => {
    const { culture, t, item, sectionWidth } = this.props;
    const { created, updated, version, fileExst } = item;

    const title =
      version > 1
        ? t("TitleModified")
        : fileExst
        ? t("TitleUploaded")
        : t("TitleCreated");

    const date = fileExst ? updated : created;
    const dateLabel = new Date(date).toLocaleString(culture);
    const mobile = (sectionWidth && sectionWidth <= 375) || isMobile;

    return mobile ? dateLabel : `${title}: ${dateLabel}`;
  };

  getDefaultName = (format) => {
    const { t } = this.props;

    switch (format) {
      case "docx":
        return t("NewDocument");
      case "xlsx":
        return t("NewSpreadsheet");
      case "pptx":
        return t("NewPresentation");
      default:
        return t("NewFolder");
    }
  };

  onShowVersionHistory = (e) => {
    const { settings, history } = this.props;
    const fileId = e.currentTarget.dataset.id;

    history.push(`${settings.homepage}/${fileId}/history`);
  };

  onBadgeClick = () => {
    const { showNewFilesPanel } = this.state;
    const {
      item,
      treeFolders,
      setTreeFolders,
      rootFolderId,
      newItems,
      setNewRowItems,
      setUpdateTree,
    } = this.props;
    if (item.fileExst) {
      api.files
        .markAsRead([], [item.id])
        .then(() => {
          const data = treeFolders;
          const dataItem = data.find((x) => x.id === rootFolderId);
          dataItem.newItems = newItems ? dataItem.newItems - 1 : 0;
          setUpdateTree(true);
          setTreeFolders(data);
          setNewRowItems([`${item.id}`]);
        })
        .catch((err) => toastr.error(err));
    } else {
      const newFolderId = this.props.selectedFolder.pathParts;
      newFolderId.push(item.id);
      this.setState({
        showNewFilesPanel: !showNewFilesPanel,
        newFolderId,
      });
    }
  };

  onShowNewFilesPanel = () => {
    const { showNewFilesPanel } = this.state;
    this.setState({ showNewFilesPanel: !showNewFilesPanel });
  };

  setConvertDialogVisible = () =>
    this.setState({ showConvertDialog: !this.state.showConvertDialog });

  getConvertProgress = (fileId) => {
    const {
      selectedFolder,
      filter,
      setIsLoading,
      setSecondaryProgressBarData,
      t,
      clearSecondaryProgressData,
      fetchFiles,
    } = this.props;
    api.files.getFileConversationProgress(fileId).then((res) => {
      if (res && res[0] && res[0].progress !== 100) {
        setSecondaryProgressBarData({
          icon: "file",
          visible: true,
          percent: res[0].progress,
          label: t("Convert"),
          alert: false,
        });
        setTimeout(() => this.getConvertProgress(fileId), 1000);
      } else {
        if (res[0].error) {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          toastr.error(res[0].error);
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        } else {
          setSecondaryProgressBarData({
            icon: "file",
            visible: true,
            percent: 100,
            label: t("Convert"),
            alert: false,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          const newFilter = filter.clone();
          fetchFiles(selectedFolder.id, newFilter)
            .catch((err) => {
              setSecondaryProgressBarData({
                visible: true,
                alert: true,
              });
              //toastr.error(err);
              setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
            })
            .finally(() => setIsLoading(false));
        }
      }
    });
  };

  onConvert = () => {
    const { item, t, setSecondaryProgressBarData } = this.props;
    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label: t("Convert"),
      alert: false,
    });
    this.setState({ showConvertDialog: false }, () =>
      api.files.convertFile(item.id).then((convertRes) => {
        if (convertRes && convertRes[0] && convertRes[0].progress !== 100) {
          this.getConvertProgress(item.id);
        }
      })
    );
  };

  render() {
    const {
      t,
      item,
      fileAction,
      isTrashFolder,
      folders,
      isLoading,
      isMobile,
      canWebEdit,
      canConvert,
      sectionWidth,
    } = this.props;
    const {
      itemTitle,
      editingId,
      showNewFilesPanel,
      newItems,
      newFolderId,
      showConvertDialog,
    } = this.state;
    const {
      contentLength,
      updated,
      createdBy,
      fileExst,
      filesCount,
      foldersCount,
      fileStatus,
      id,
      versionGroup,
      locked,
    } = item;
    const titleWithoutExt = getTitleWithoutExst(item);
    const fileOwner =
      createdBy &&
      ((this.props.viewer.id === createdBy.id && t("AuthorMe")) ||
        createdBy.displayName);
    const updatedDate = updated && this.getStatusByDate();

    const isEdit = id === editingId && fileExst === fileAction.extension;
    const linkStyles = isTrashFolder
      ? { noHover: true }
      : { onClick: this.onFilesClick };
    const showNew = !!newItems;

    return isEdit ? (
      <EditingWrapperComponent
        itemTitle={itemTitle}
        okIcon={okIcon}
        cancelIcon={cancelIcon}
        renameTitle={this.renameTitle}
        onClickUpdateItem={this.onClickUpdateItem}
        cancelUpdateItem={this.cancelUpdateItem}
        itemId={id}
        isLoading={isLoading}
      />
    ) : (
      <>
        {showConvertDialog && (
          <ConvertDialog
            visible={showConvertDialog}
            onClose={this.setConvertDialogVisible}
            onConvert={this.onConvert}
          />
        )}
        {showNewFilesPanel && (
          <NewFilesPanel
            visible={showNewFilesPanel}
            onClose={this.onShowNewFilesPanel}
            folderId={newFolderId}
            folders={folders}
          />
        )}
        <SimpleFilesRowContent
          sectionWidth={sectionWidth}
          isMobile={isMobile}
          sideColor={sideColor}
          isFile={fileExst}
          onClick={this.onMobileRowClick}
        >
          <Link
            containerWidth="55%"
            type="page"
            title={titleWithoutExt}
            fontWeight="600"
            fontSize="15px"
            {...linkStyles}
            color="#333"
            isTextOverflow
          >
            {titleWithoutExt}
          </Link>
          <>
            {fileExst ? (
              <div className="badges">
                <Text
                  className="badge-ext"
                  as="span"
                  color="#A3A9AE"
                  fontSize="15px"
                  fontWeight={600}
                  title={fileExst}
                  truncate={true}
                >
                  {fileExst}
                </Text>
                {/* TODO: Uncomment after fix conversation {canConvert && !isTrashFolder && (
                  <IconButton
                    onClick={this.setConvertDialogVisible}
                    iconName="FileActionsConvertIcon"
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#A3A9AE"
                    hoverColor="#3B72A7"
                  />
                )} */}
                {canWebEdit && !isTrashFolder && (
                  <IconButton
                    onClick={this.onFilesClick}
                    iconName="AccessEditIcon"
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#A3A9AE"
                    hoverColor="#3B72A7"
                  />
                )}
                {fileStatus === 32 && !isTrashFolder && (
                  <Icons.FavoriteIcon
                    className="favorite"
                    size="small"
                    data-action="remove"
                    data-id={item.id}
                    data-title={item.title}
                    onClick={this.props.onClickFavorite}
                  />
                )}
                {fileStatus === 1 && (
                  <Icons.FileActionsConvertEditDocIcon
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#3B72A7"
                  />
                )}
                {locked && (
                  <Icons.FileActionsLockedIcon
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#3B72A7"
                  />
                )}
                {versionGroup > 1 && (
                  <Badge
                    className="badge-version"
                    backgroundColor="#A3A9AE"
                    borderRadius="11px"
                    color="#FFFFFF"
                    fontSize="10px"
                    fontWeight={800}
                    label={`Ver.${versionGroup}`}
                    maxWidth="50px"
                    onClick={this.onShowVersionHistory}
                    padding="0 5px"
                    data-id={id}
                  />
                )}
                {showNew && (
                  <Badge
                    className="badge-version"
                    backgroundColor="#ED7309"
                    borderRadius="11px"
                    color="#FFFFFF"
                    fontSize="10px"
                    fontWeight={800}
                    label={`New`}
                    maxWidth="50px"
                    onClick={this.onBadgeClick}
                    padding="0 5px"
                    data-id={id}
                  />
                )}
              </div>
            ) : (
              <div className="badges">
                {showNew && (
                  <Badge
                    className="badge-version"
                    backgroundColor="#ED7309"
                    borderRadius="11px"
                    color="#FFFFFF"
                    fontSize="10px"
                    fontWeight={800}
                    label={newItems}
                    maxWidth="50px"
                    onClick={this.onBadgeClick}
                    padding="0 5px"
                    data-id={id}
                  />
                )}
              </div>
            )}
          </>
          <Text
            containerMinWidth="120px"
            containerWidth="15%"
            as="div"
            color={sideColor}
            fontSize="12px"
            fontWeight={400}
            title={fileOwner}
            truncate={true}
          >
            {fileOwner}
          </Text>
          <Text
            containerMinWidth="200px"
            containerWidth="15%"
            title={updatedDate}
            fontSize="12px"
            fontWeight={400}
            color={sideColor}
            className="row_update-text"
          >
            {updatedDate && updatedDate}
          </Text>
          <Text
            containerMinWidth="90px"
            containerWidth="10%"
            as="div"
            color={sideColor}
            fontSize="12px"
            fontWeight={400}
            title=""
            truncate={true}
          >
            {fileExst
              ? contentLength
              : `${t("TitleDocuments")}: ${filesCount} | ${t(
                  "TitleSubfolders"
                )}: ${foldersCount}`}
          </Text>
        </SimpleFilesRowContent>
      </>
    );
  }
}

function mapStateToProps(state, props) {
  return {
    filter: getFilter(state),
    fileAction: getFileAction(state),
    parentFolder: getSelectedFolderParentId(state),
    isTrashFolder: getIsRecycleBinFolder(state),
    settings: getSettings(state),
    treeFolders: getTreeFolders(state),
    rootFolderId: getRootFolderId(state),
    newItems: getSelectedFolderNew(state),
    selectedFolder: getSelectedFolder(state),
    folders: getFolders(state),
    newRowItems: getNewRowItems(state),
    dragging: getDragging(state),
    isLoading: getIsLoading(state),

    canWebEdit: canWebEdit(props.item.fileExst)(state),
    canConvert: canConvert(props.item.fileExst)(state),
    isImage: isImage(props.item.fileExst)(state),
    isSound: isSound(props.item.fileExst)(state),
    isVideo: isVideo(props.item.fileExst)(state),
  };
}

export default connect(mapStateToProps, {
  createFile,
  createFolder,
  updateFile,
  renameFolder,
  setTreeFolders,
  setSecondaryProgressBarData,
  setUpdateTree,
  setNewRowItems,
  setIsLoading,
  clearSecondaryProgressData,
  fetchFiles,
})(withRouter(withTranslation()(FilesRowContent)));
