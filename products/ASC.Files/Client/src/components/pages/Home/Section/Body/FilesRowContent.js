
import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { RowContent, Link, Text, Icons, IconButton, Badge, toastr } from "asc-web-components";
import { constants, api } from 'asc-web-common';
import { createFile, createFolder, renameFolder, updateFile, fetchFiles, setTreeFolders, setProgressBarData, clearProgressData, setNewTreeFilesBadge, setNewRowItems, setIsLoading } from '../../../../../store/files/actions';
import { canWebEdit, isImage, isSound, isVideo, canConvert, getTitleWithoutExst } from '../../../../../store/files/selectors';
import store from "../../../../../store/store";
import { NewFilesPanel } from "../../../../panels";
import { ConvertDialog } from "../../../../dialogs";
import EditingWrapperComponent from "./EditingWrapperComponent";

const { FileAction } = constants;

const SimpleFilesRowContent = styled(RowContent)`
.badge-ext {
  margin-left: -8px;
  margin-right: 8px;
}

.badge {
  height: 14px;
  width: 14px;
  margin-right: 8px;
}

.badges {
  display: flex;
  align-items: center;
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

const okIcon = <Icons.CheckIcon
  className='edit-ok-icon'
  size='scale'
  isfill={true}
  color='#A3A9AE'
/>;

const cancelIcon = <Icons.CrossIcon
  className='edit-cancel-icon'
  size='scale'
  isfill={true}
  color='#A3A9AE'
/>;

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
      showConvertDialog: false
      //loading: false
    };
  }

  completeAction = (e) => {
    //this.setState({ loading: false }, () =>)
    this.props.onEditComplete(e);
  }

  updateItem = (e) => {
    const { fileAction, updateFile, renameFolder, item, setIsLoading } = this.props;

    const { itemTitle } = this.state;
    const originalTitle = getTitleWithoutExst(item);

    setIsLoading(true);
    if (originalTitle === itemTitle)
      return this.completeAction(e);

    item.fileExst
      ? updateFile(fileAction.id, itemTitle)
        .then(() => this.completeAction(e)).finally(() => setIsLoading(false))
      : renameFolder(fileAction.id, itemTitle)
        .then(() => this.completeAction(e)).finally(() => setIsLoading(false));
  };

  createItem = (e) => {
    const { createFile, createFolder, item, setIsLoading } = this.props;
    const { itemTitle } = this.state;

    setIsLoading(true);

    if (itemTitle.trim() === '')
      return this.completeAction(e);

    !item.fileExst
      ? createFolder(item.parentId, itemTitle)
        .then(() => this.completeAction(e)).finally(() => setIsLoading(false))
      : createFile(item.parentId, `${itemTitle}.${item.fileExst}`)
        .then(() => this.completeAction(e)).finally(() => setIsLoading(false))
  }

  componentDidUpdate(prevProps) {
    const { fileAction, item, newRowItems, setNewRowItems } = this.props;
    const itemId = item.id.toString();

    if (newRowItems.length && newRowItems.includes(itemId)) {
      const rowItems = newRowItems.filter(x => x !== itemId)
      if (this.state.newItems !== 0) {
        this.setState({ newItems: 0 }, () => setNewRowItems(rowItems));
      }
    }

    if (fileAction) {
      if (fileAction.id !== prevProps.fileAction.id) {
        this.setState({ editingId: fileAction.id })
      }
    }
  }

  renameTitle = e => {
    this.setState({ itemTitle: e.target.value });
  }

  cancelUpdateItem = (e) => {
    //this.setState({ loading: false });
    this.completeAction(e);
  }

  onClickUpdateItem = () => {
    (this.props.fileAction.type === FileAction.Create)
      ? this.createItem()
      : this.updateItem();
  }

  onFilesClick = () => {
    const { id, fileExst, viewUrl } = this.props.item;
    const { filter, parentFolder, setIsLoading, onMediaFileClick } = this.props;
    if (!fileExst) {
      setIsLoading(true);
      const newFilter = filter.clone();
      if (!newFilter.treeFolders.includes(parentFolder.toString())) {
        newFilter.treeFolders.push(parentFolder.toString());
      }

      fetchFiles(id, newFilter, store.dispatch)
        .catch(err => {
          toastr.error(err);
          setIsLoading(false);
        })
        .finally(() => setIsLoading(false));
    } else {
      if (canWebEdit(fileExst)) {
        return window.open(`./doceditor?fileId=${id}`, "_blank");
      }

      const isOpenMedia = isImage(fileExst) || isSound(fileExst) || isVideo(fileExst);

      if (isOpenMedia) {
        onMediaFileClick(id);
        return;
      }

      return window.open(viewUrl, "_blank");
    }
  };

  onMobileRowClick = (e) => {
    if (window.innerWidth > 1024)
      return;

    this.onFilesClick();
  }

  getStatusByDate = () => {
    const { culture, t, item } = this.props;
    const { created, updated, version, fileExst } = item;

    const title = version > 1
      ? t("TitleModified")
      : fileExst
        ? t("TitleUploaded")
        : t("TitleCreated");

    const date = fileExst ? updated : created;
    const dateLabel = new Date(date).toLocaleString(culture);

    return `${title}: ${dateLabel}`;
  };

  getDefaultName = (format) => {
    const { t } = this.props;

    switch (format) {
      case 'docx':
        return t("NewDocument");
      case 'xlsx':
        return t("NewSpreadsheet");
      case 'pptx':
        return t("NewPresentation");
      default:
        return t("NewFolder");
    }
  };

  onShowVersionHistory = (e) => {
    const { settings, history } = this.props;
    const fileId = e.currentTarget.dataset.id;

    history.push(`${settings.homepage}/${fileId}/history`);
  }

  onBadgeClick = () => {
    const { showNewFilesPanel } = this.state;
    const { item, treeFolders, setTreeFolders, rootFolderId, newItems, setNewRowItems, setNewTreeFilesBadge } = this.props;
    if (item.fileExst) {
      api.files
        .markAsRead([], [item.id])
        .then(() => {
          const data = treeFolders;
          const dataItem = data.find((x) => x.id === rootFolderId);
          dataItem.newItems = newItems ? dataItem.newItems - 1 : 0;
          setNewTreeFilesBadge(true);
          setTreeFolders(data);
          setNewRowItems([`${item.id}`]);
        })
        .catch((err) => toastr.error(err))
    } else {
      const newFolderId = this.props.selectedFolder.pathParts;
      newFolderId.push(item.id);
      this.setState({
        showNewFilesPanel: !showNewFilesPanel,
        newFolderId,
      });
    }
  }

  onShowNewFilesPanel = () => {
    const { showNewFilesPanel } = this.state;
    this.setState({ showNewFilesPanel: !showNewFilesPanel });
  };

  setConvertDialogVisible = () =>
    this.setState({ showConvertDialog: !this.state.showConvertDialog });

  getConvertProgress = fileId => {
    const { selectedFolder, filter, setIsLoading, setProgressBarData, t } = this.props;
    api.files.getConvertFile(fileId).then(res => {
      if (res && res[0] && res[0].progress !== 100) {
        setProgressBarData({ visible: true, percent: res[0].progress, label: t("Convert") });
        setTimeout(() => this.getConvertProgress(fileId), 1000);
      } else {
        if (res[0].error) {
          toastr.error(res[0].error);
          clearProgressData(store.dispatch);
        } else {
          setProgressBarData({ visible: true, percent: 100, label: t("Convert") });
          setTimeout(() => clearProgressData(), 5000)
          const newFilter = filter.clone();
          fetchFiles(selectedFolder.id, newFilter, store.dispatch)
            .catch(err => toastr.error(err))
            .finally(() => setIsLoading(false));
        }
      }
    });
  }

  onConvert = () => {
    const { item, t, setProgressBarData } = this.props;
    setProgressBarData({ visible: true, percent: 0, label: t("Convert") });
    this.setState({ showConvertDialog: false }, () =>
      api.files.convertFile(item.id).then(convertRes => {
        if (convertRes && convertRes[0] && convertRes[0].progress !== 100) {
          this.getConvertProgress(item.id);
        }
      })
    );
  }

  render() {
    const { t, item, fileAction, isTrashFolder, folders, widthProp } = this.props;
    const { itemTitle, editingId, showNewFilesPanel, newItems, newFolderId, showConvertDialog } = this.state;
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
      locked
    } = item;

    const titleWithoutExt = getTitleWithoutExst(item);
    const fileOwner = createdBy && ((this.props.viewer.id === createdBy.id && t("AuthorMe")) || createdBy.displayName);
    const updatedDate = updated && this.getStatusByDate();
    const canEditFile = fileExst && canWebEdit(fileExst);
    const canConvertFile = fileExst && canConvert(fileExst);

    const isEdit = (id === editingId) && (fileExst === fileAction.extension);
    const linkStyles = isTrashFolder ? { noHover: true } : { onClick: this.onFilesClick };
    const showNew = !!newItems;

    return isEdit
      ? <EditingWrapperComponent
        itemTitle={itemTitle}
        okIcon={okIcon}
        cancelIcon={cancelIcon}
        renameTitle={this.renameTitle}
        onClickUpdateItem={this.onClickUpdateItem}
        cancelUpdateItem={this.cancelUpdateItem}
        itemId={id}
      />
      : (
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
            widthProp={widthProp}
            sideColor="#333"
            isFile={fileExst}
            onClick={this.onMobileRowClick}
          >
            <Link
              containerWidth='100%'
              type='page'
              title={titleWithoutExt}
              fontWeight="600"
              fontSize='15px'
              {...linkStyles}
              color="#333"
              isTextOverflow
            >
              {titleWithoutExt}
            </Link>
            <>
              {fileExst ?
                <div className='badges'>
                  <Text
                    className='badge-ext'
                    as="span"
                    color="#A3A9AE"
                    fontSize='15px'
                    fontWeight={600}
                    title={fileExst}
                    truncate={true}
                  >
                    {fileExst}
                  </Text>
                  {canConvertFile &&
                    <IconButton
                      onClick={this.setConvertDialogVisible}
                      iconName="FileActionsConvertIcon"
                      className='badge'
                      size='small'
                      isfill={true}
                      color='#A3A9AE'
                    />
                  }
                  {canEditFile &&
                    <Icons.AccessEditIcon
                      className='badge'
                      size='small'
                      isfill={true}
                      color='#A3A9AE'
                    />
                  }
                  {fileStatus === 1 &&
                    <Icons.FileActionsConvertEditDocIcon
                      className='badge'
                      size='small'
                      isfill={true}
                      color='#3B72A7'
                    />
                  }
                  {locked &&
                    <Icons.FileActionsLockedIcon
                      className='badge'
                      size='small'
                      isfill={true}
                      color='#3B72A7'
                    />
                  }
                  {versionGroup > 1 &&
                    <Badge
                      className='badge-version'
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
                  }
                  {showNew &&
                    <Badge
                      className='badge-version'
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
                  }
                </div>
                :
                <div className='badges'>
                  {showNew &&
                    <Badge
                      className='badge-version'
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
                  }
                </div>
              }
            </>
            <Text
              containerMinWidth='120px'
              containerWidth='10%'
              as="div"
              color="#333"
              fontSize='12px'
              fontWeight={400}
              title={fileOwner}
              truncate={true}
            >
              {fileOwner}
            </Text>
            <Text
              containerMinWidth='200px'
              containerWidth='15%'
              title={updatedDate}
              fontSize='12px'
              fontWeight={400}
              color="#333"
              className="row_update-text"
            >
              {updatedDate && updatedDate}
            </Text>
            <Text
              containerMinWidth='90px'
              containerWidth='8%'
              as="div"
              color="#333"
              fontSize='12px'
              fontWeight={400}
              title=''
              truncate={true}
            >
              {fileExst
                ? contentLength
                : `${t("TitleDocuments")}: ${filesCount} | ${t("TitleSubfolders")}: ${foldersCount}`}
            </Text>
          </SimpleFilesRowContent>
        </>
      )
  }
};

function mapStateToProps(state) {
  const { filter, fileAction, selectedFolder, treeFolders, folders, newRowItems, dragging } = state.files;
  const { settings } = state.auth;
  const indexOfTrash = 3;
  const rootFolderId = selectedFolder.pathParts && selectedFolder.pathParts[0];

  return {
    filter,
    fileAction,
    parentFolder: selectedFolder.id,
    isTrashFolder: treeFolders.length && treeFolders[indexOfTrash].id === selectedFolder.id,
    settings,
    treeFolders,
    rootFolderId,
    newItems: selectedFolder.new,
    selectedFolder,
    folders,
    newRowItems,
    dragging
  }
}

export default connect(mapStateToProps, { createFile, createFolder, updateFile, renameFolder, setTreeFolders, setProgressBarData, setNewTreeFilesBadge, setNewRowItems, setIsLoading })(
  withRouter(withTranslation()(FilesRowContent))
);