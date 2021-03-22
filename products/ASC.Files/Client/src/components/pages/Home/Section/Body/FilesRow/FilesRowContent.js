import React from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import RowContent from "@appserver/components/row-content";
import IconButton from "@appserver/components/icon-button";
import Badge from "@appserver/components/badge";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import {
  convertFile,
  markAsRead,
  getFileConversationProgress,
} from "@appserver/common/api/files";
import {
  AppServerConfig,
  FileAction,
  ShareAccessRights,
} from "@appserver/common/constants";
import toastr from "studio/toastr";
import FavoriteIcon from "../../../../../../../public/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "../../../../../../../public/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "../../../../../../../public/images/file.actions.locked.react.svg";
import CheckIcon from "../../../../../../../public/images/check.react.svg";
import CrossIcon from "../../../../../../../../../../public/images/cross.react.svg";
import { TIMEOUT } from "../../../../../../helpers/constants";
import { getTitleWithoutExst } from "../../../../../../helpers/files-helpers";
import { NewFilesPanel } from "../../../../../panels";
import { ConvertDialog } from "../../../../../dialogs";
import EditingWrapperComponent from "../EditingWrapperComponent";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import config from "../../../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";

const sideColor = "#A3A9AE";
const StyledCheckIcon = styled(CheckIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;

const StyledFavoriteIcon = styled(FavoriteIcon)`
  ${commonIconsStyles}
`;

const StyledFileActionsConvertEditDocIcon = styled(
  FileActionsConvertEditDocIcon
)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;

const StyledFileActionsLockedIcon = styled(FileActionsLockedIcon)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;
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
  .lock-file {
    cursor: pointer;
  }
  .badges {
    display: flex;
    align-items: center;
  }

  .favorite {
    cursor: pointer;
    margin-right: 6px;
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

const okIcon = <StyledCheckIcon className="edit-ok-icon" size="scale" />;

const cancelIcon = (
  <StyledCrossIcon className="edit-cancel-icon" size="scale" />
);

class FilesRowContent extends React.PureComponent {
  constructor(props) {
    super(props);
    let titleWithoutExt = getTitleWithoutExst(props.item);

    if (props.fileActionId === -1) {
      titleWithoutExt = this.getDefaultName(props.fileActionExt);
    }

    this.state = {
      itemTitle: titleWithoutExt,
      showNewFilesPanel: false,
      newFolderId: [],
      newItems: props.item.new || props.item.fileStatus === 2,
      showConvertDialog: false,
      //loading: false
    };
  }

  completeAction = (id) => {
    this.props.editCompleteAction(id, this.props.item);
  };

  updateItem = () => {
    const {
      updateFile,
      renameFolder,
      item,
      setIsLoading,
      fileActionId,
    } = this.props;

    const { itemTitle } = this.state;
    const originalTitle = getTitleWithoutExst(item);

    setIsLoading(true);
    if (originalTitle === itemTitle || itemTitle.trim() === "") {
      this.setState({
        itemTitle: originalTitle,
      });
      return this.completeAction(fileActionId);
    }

    item.fileExst
      ? updateFile(fileActionId, itemTitle)
          .then(() => this.completeAction(fileActionId))
          .finally(() => setIsLoading(false))
      : renameFolder(fileActionId, itemTitle)
          .then(() => this.completeAction(fileActionId))
          .finally(() => setIsLoading(false));
  };

  createItem = (e) => {
    const {
      createFile,
      item,
      setIsLoading,
      openDocEditor,
      isPrivacy,
      isDesktop,
      replaceFileStream,
      t,
      setEncryptionAccess,
      createFolder,
    } = this.props;
    const { itemTitle } = this.state;

    setIsLoading(true);

    const itemId = e.currentTarget.dataset.itemid;

    if (itemTitle.trim() === "") {
      toastr.warning(this.props.t("CreateWithEmptyTitle"));
      return this.completeAction(itemId);
    }

    let tab =
      !isDesktop && item.fileExst
        ? window.open(
            combineUrl(
              AppServerConfig.proxyURL,
              config.homepage,
              "/products/files/doceditor"
            ),
            "_blank"
          )
        : null;

    !item.fileExst
      ? createFolder(item.parentId, itemTitle)
          .then(() => this.completeAction(itemId))
          .then(() =>
            toastr.success(
              <Trans i18nKey="FolderCreated" ns="Home">
                New folder {{ itemTitle }} is created
              </Trans>
            )
          )
          .catch((e) => toastr.error(e))
          .finally(() => {
            return setIsLoading(false);
          })
      : createFile(item.parentId, `${itemTitle}.${item.fileExst}`)
          .then((file) => {
            if (isPrivacy) {
              return setEncryptionAccess(file).then((encryptedFile) => {
                if (!encryptedFile) return Promise.resolve();
                toastr.info(t("EncryptedFileSaving"));
                return replaceFileStream(
                  file.id,
                  encryptedFile,
                  true,
                  false
                ).then(() =>
                  openDocEditor(file.id, file.providerKey, tab, file.webUrl)
                );
              });
            }
            return openDocEditor(file.id, file.providerKey, tab, file.webUrl);
          })
          .then(() => this.completeAction(itemId))
          .then(() => {
            const exst = item.fileExst;
            return toastr.success(
              <Trans i18nKey="FileCreated" ns="Home">
                New file {{ itemTitle }}.{{ exst }} is created
              </Trans>
            );
          })
          .catch((e) => toastr.error(e))
          .finally(() => {
            return setIsLoading(false);
          });
  };

  // componentDidUpdate(prevProps) {
  //   const { item, newRowItems, setNewRowItems } = this.props;
  //   const itemId = item.id.toString();

  //   if (newRowItems.length && newRowItems.includes(itemId)) {
  //     const rowItems = newRowItems.filter((x) => x !== itemId);
  //     if (this.state.newItems !== 0) {
  //       this.setState({ newItems: 0 }, () => setNewRowItems(rowItems));
  //     }
  //   }

  //   if (fileAction) {
  //     if (fileActionId !== prevProps.fileActionId) {
  //       this.setState({ editingId: fileActionId });
  //     }
  //   }
  // }

  renameTitle = (e) => {
    let title = e.target.value;
    //const chars = '*+:"<>?|/'; TODO: think how to solve problem with interpolation escape values in i18n translate
    const regexp = new RegExp('[*+:"<>?|\\\\/]', "gim");
    if (title.match(regexp)) {
      toastr.warning(this.props.t("ContainsSpecCharacter"));
    }
    title = title.replace(regexp, "_");
    return this.setState({ itemTitle: title });
  };

  cancelUpdateItem = (e) => {
    const originalTitle = getTitleWithoutExst(this.props.item);
    this.setState({
      itemTitle: originalTitle,
    });

    return this.completeAction(e);
  };

  onClickUpdateItem = (e) => {
    this.props.fileActionType === FileAction.Create
      ? this.createItem(e)
      : this.updateItem(e);
  };

  onFilesClick = () => {
    const {
      filter,
      parentFolder,
      setIsLoading,
      fetchFiles,
      isImage,
      isSound,
      isVideo,
      canWebEdit,
      item,
      isTrashFolder,
      openDocEditor,
      expandedKeys,
      addExpandedKeys,
      setMediaViewerData,
    } = this.props;
    const { id, fileExst, viewUrl, providerKey } = item;

    if (isTrashFolder) return;

    if (!fileExst) {
      setIsLoading(true);

      if (!expandedKeys.includes(parentFolder + "")) {
        addExpandedKeys(parentFolder + "");
      }

      fetchFiles(id, filter)
        .catch((err) => {
          toastr.error(err);
          setIsLoading(false);
        })
        .finally(() => setIsLoading(false));
    } else {
      if (canWebEdit) {
        return openDocEditor(id, providerKey);
      }

      if (isImage || isSound || isVideo) {
        setMediaViewerData({ visible: true, id });
        return;
      }

      return window.open(viewUrl, "_blank");
    }
  };

  onMobileRowClick = () => {
    if (this.props.isTrashFolder || window.innerWidth > 1024) return;
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

  onShowVersionHistory = () => {
    const {
      homepage,
      isTabletView,
      item,
      setIsVerHistoryPanel,
      fetchFileVersions,
      history,
    } = this.props;

    if (!isTabletView) {
      fetchFileVersions(item.id + "");
      setIsVerHistoryPanel(true);
    } else {
      history.push(`${homepage}/${item.id}/history`);
    }
  };

  onBadgeClick = () => {
    const { showNewFilesPanel } = this.state;
    const {
      item,
      treeFolders,
      setTreeFolders,
      selectedFolderPathParts,
      newItems,
      setNewRowItems,
    } = this.props;
    if (item.fileExst) {
      markAsRead([], [item.id])
        .then(() => {
          const data = treeFolders;
          const dataItem = data.find(
            (x) => x.id === selectedFolderPathParts[0]
          );
          dataItem.newItems = newItems ? dataItem.newItems - 1 : 0;
          setTreeFolders(data);
          setNewRowItems([`${item.id}`]);
        })
        .catch((err) => toastr.error(err));
    } else {
      const newFolderId = this.props.selectedFolderPathParts;
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
      selectedFolderId,
      filter,
      setIsLoading,
      setSecondaryProgressBarData,
      t,
      clearSecondaryProgressData,
      fetchFiles,
    } = this.props;
    getFileConversationProgress(fileId).then((res) => {
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
          fetchFiles(selectedFolderId, newFilter)
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
      convertFile(item.id).then((convertRes) => {
        if (convertRes && convertRes[0] && convertRes[0].progress !== 100) {
          this.getConvertProgress(item.id);
        }
      })
    );
  };

  onClickLock = () => {
    const { item } = this.props;
    const { locked, id } = item;
    this.props.lockFileAction(id, !locked).catch((err) => toastr.error(err));
  };

  onClickFavorite = () => {
    const { t, item } = this.props;
    this.props
      .setFavoriteAction("remove", item.id)
      .then(() => toastr.success(t("RemovedFromFavorites")))
      .catch((err) => toastr.error(err));
  };
  render() {
    const {
      t,
      item,
      isTrashFolder,
      isLoading,
      isMobile,
      canWebEdit,
      /* canConvert,*/
      sectionWidth,
      fileActionId,
      fileActionExt,
    } = this.props;
    const {
      itemTitle,
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
      providerKey,
    } = item;
    const titleWithoutExt = getTitleWithoutExst(item);
    const fileOwner =
      createdBy &&
      ((this.props.viewer.id === createdBy.id && t("AuthorMe")) ||
        createdBy.displayName);
    const updatedDate = updated && this.getStatusByDate();

    const accessToEdit =
      item.access === ShareAccessRights.FullAccess ||
      item.access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)
    const isEdit = id === fileActionId && fileExst === fileActionExt;

    const linkStyles =
      isTrashFolder || window.innerWidth <= 1024
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
                {canWebEdit && !isTrashFolder && accessToEdit && (
                  <IconButton
                    onClick={this.onFilesClick}
                    iconName="images/access.edit.react.svg"
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#A3A9AE"
                    hoverColor="#3B72A7"
                  />
                )}
                {locked && (
                  <StyledFileActionsLockedIcon
                    className="badge lock-file"
                    size="small"
                    data-id={item.id}
                    data-locked={true}
                    onClick={this.onClickLock}
                  />
                )}
                {fileStatus === 32 && !isTrashFolder && (
                  <StyledFavoriteIcon
                    className="favorite"
                    size="small"
                    data-action="remove"
                    data-id={item.id}
                    data-title={item.title}
                    onClick={this.onClickFavorite}
                  />
                )}
                {fileStatus === 1 && (
                  <StyledFileActionsConvertEditDocIcon
                    className="badge"
                    size="small"
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
            {(fileExst || !providerKey) && updatedDate && updatedDate}
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
              : !providerKey
              ? `${t("TitleDocuments")}: ${filesCount} | ${t(
                  "TitleSubfolders"
                )}: ${foldersCount}`
              : ""}
          </Text>
        </SimpleFilesRowContent>
      </>
    );
  }
}

export default inject(
  (
    {
      auth,
      initFilesStore,
      filesStore,
      formatsStore,
      uploadDataStore,
      treeFoldersStore,
      selectedFolderStore,
      filesActionsStore,
      mediaViewerDataStore,
      versionHistoryStore,
    },
    { item }
  ) => {
    const { replaceFileStream, setEncryptionAccess } = auth;
    const { culture, isDesktopClient, isTabletView } = auth.settingsStore;
    const { setIsLoading, isLoading } = initFilesStore;
    const { secondaryProgressDataStore } = uploadDataStore;
    const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
    const {
      iconFormatsStore,
      mediaViewersFormatsStore,
      docserviceStore,
    } = formatsStore;

    const {
      fetchFiles,
      filter,
      setNewRowItems,
      newRowItems,
      createFile,
      updateFile,
      renameFolder,
      createFolder,
      openDocEditor,
    } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      isRecycleBinFolder,
      isPrivacyFolder,
      expandedKeys,
      addExpandedKeys,
    } = treeFoldersStore;

    const {
      type: fileActionType,
      extension: fileActionExt,
      id: fileActionId,
    } = filesStore.fileActionStore;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const canWebEdit = docserviceStore.canWebEdit(item.fileExst);
    const canConvert = docserviceStore.canConvert(item.fileExst);
    const isVideo = mediaViewersFormatsStore.isVideo(item.fileExst);
    const isImage = iconFormatsStore.isImage(item.fileExst);
    const isSound = iconFormatsStore.isSound(item.fileExst);

    const { setMediaViewerData } = mediaViewerDataStore;

    return {
      isDesktop: isDesktopClient,
      isTabletView,
      homepage: config.homepage,
      viewer: auth.userStore.user,
      culture,
      fileActionId,
      fileActionType,
      fileActionExt,
      selectedFolderId: selectedFolderStore.id,
      selectedFolderPathParts: selectedFolderStore.pathParts,
      newItems: selectedFolderStore.new,
      parentFolder: selectedFolderStore.parentId,
      isLoading,
      treeFolders,
      isTrashFolder: isRecycleBinFolder,
      isPrivacy: isPrivacyFolder,
      filter,
      canWebEdit,
      canConvert,
      isVideo,
      isImage,
      isSound,
      newRowItems,
      expandedKeys,

      setIsLoading,
      fetchFiles,
      setTreeFolders,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      setNewRowItems,
      createFile,
      createFolder,
      updateFile,
      renameFolder,
      replaceFileStream,
      setEncryptionAccess,
      addExpandedKeys,
      openDocEditor,
      editCompleteAction: filesActionsStore.editCompleteAction,
      lockFileAction: filesActionsStore.lockFileAction,
      setFavoriteAction: filesActionsStore.setFavoriteAction,
      setMediaViewerData,
      setIsVerHistoryPanel,
      fetchFileVersions,
    };
  }
)(withRouter(withTranslation("Home")(observer(FilesRowContent))));
