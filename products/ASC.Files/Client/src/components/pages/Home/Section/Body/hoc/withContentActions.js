import React from "react";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";
import { isMobile } from "react-device-detect";

import toastr from "studio/toastr";
import {
  AppServerConfig,
  FileAction,
  ShareAccessRights,
} from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import {
  convertFile,
  getFileConversationProgress,
} from "@appserver/common/api/files";

import config from "../../../../../../../package.json";
import EditingWrapperComponent from "../sub-components/EditingWrapperComponent";
import { getTitleWithoutExst } from "../../../../../../helpers/files-helpers";
import { ConvertDialog } from "../../../../../dialogs";

export default function withContentActions(WrappedContent) {
  class WithContentActions extends React.Component {
    constructor(props) {
      super(props);
      let titleWithoutExt = getTitleWithoutExst(props.item);

      if (props.fileActionId === -1) {
        titleWithoutExt = this.getDefaultName(props.fileActionExt);
      }

      this.state = {
        itemTitle: titleWithoutExt,
        showConvertDialog: false,
        //loading: false
      };
    }

    componentDidUpdate(prevProps) {
      const { fileActionId, fileActionExt } = this.props;
      if (fileActionId === -1 && fileActionExt !== prevProps.fileActionExt) {
        const itemTitle = this.getDefaultName(fileActionExt);
        this.setState({ itemTitle });
      }
      // if (fileAction) {
      //   if (fileActionId !== prevProps.fileActionId) {
      //     this.setState({ editingId: fileActionId });
      //   }
      // }
    }

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

    completeAction = (id) => {
      const { editCompleteAction, item } = this.props;

      const isCancel =
        (id.currentTarget && id.currentTarget.dataset.action === "cancel") ||
        id.keyCode === 27;
      editCompleteAction(id, item, isCancel);
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
      const { id, fileExst, viewUrl, providerKey, contentLength } = item;

      if (isTrashFolder) return;

      if (!fileExst && !contentLength) {
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

    updateItem = () => {
      const {
        updateFile,
        renameFolder,
        item,
        setIsLoading,
        fileActionId,
        editCompleteAction,
      } = this.props;

      const { itemTitle } = this.state;
      const originalTitle = getTitleWithoutExst(item);

      setIsLoading(true);
      const isSameTitle =
        originalTitle.trim() === itemTitle.trim() || itemTitle.trim() === "";
      if (isSameTitle) {
        this.setState({
          itemTitle: originalTitle,
        });
        return editCompleteAction(fileActionId, item, isSameTitle);
      }

      item.fileExst || item.contentLength
        ? updateFile(fileActionId, itemTitle)
            .then(() => this.completeAction(fileActionId))
            .finally(() => setIsLoading(false))
        : renameFolder(fileActionId, itemTitle)
            .then(() => this.completeAction(fileActionId))
            .finally(() => setIsLoading(false));
    };

    cancelUpdateItem = (e) => {
      const { item } = this.props;

      const originalTitle = getTitleWithoutExst(item);
      this.setState({
        itemTitle: originalTitle,
      });

      return this.completeAction(e);
    };

    onClickUpdateItem = (e) => {
      const { fileActionType } = this.props;

      fileActionType === FileAction.Create
        ? this.createItem(e)
        : this.updateItem(e);
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
        toastr.warning(t("CreateWithEmptyTitle"));
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

      !item.fileExst && !item.contentLength
        ? createFolder(item.parentId, itemTitle)
            .then(() => this.completeAction(itemId))
            .then(() =>
              toastr.success(
                <Trans t={t} i18nKey="FolderCreated" ns="Home">
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

    renameTitle = (e) => {
      const { t } = this.props;

      let title = e.target.value;
      //const chars = '*+:"<>?|/'; TODO: think how to solve problem with interpolation escape values in i18n translate
      const regexp = new RegExp('[*+:"<>?|\\\\/]', "gim");
      if (title.match(regexp)) {
        toastr.warning(t("ContainsSpecCharacter"));
      }
      title = title.replace(regexp, "_");
      return this.setState({ itemTitle: title });
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

    onShowVersionHistory = () => {
      const {
        homepage,
        isTabletView,
        item,
        setIsVerHistoryPanel,
        fetchFileVersions,
        history,
        isTrashFolder,
      } = this.props;
      if (isTrashFolder) return;

      if (!isTabletView) {
        fetchFileVersions(item.id + "");
        setIsVerHistoryPanel(true);
      } else {
        history.push(
          combineUrl(AppServerConfig.proxyURL, homepage, `/${item.id}/history`)
        );
      }
    };

    onBadgeClick = () => {
      const {
        item,
        selectedFolderPathParts,
        markAsRead,
        setNewFilesPanelVisible,
        setNewFilesIds,
        updateRootBadge,
        updateFileBadge,
      } = this.props;
      if (item.fileExst) {
        markAsRead([], [item.id])
          .then(() => {
            updateRootBadge(selectedFolderPathParts[0], 1);
            updateFileBadge(item.id);
          })
          .catch((err) => toastr.error(err));
      } else {
        setNewFilesPanelVisible(true);
        const newFolderIds = selectedFolderPathParts;
        newFolderIds.push(item.id);
        setNewFilesIds(newFolderIds);
      }
    };

    setConvertDialogVisible = () =>
      this.setState({ showConvertDialog: !this.state.showConvertDialog });

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

    onClickLock = () => {
      const { item, lockFileAction } = this.props;
      const { locked, id } = item;

      lockFileAction(id, !locked).catch((err) => toastr.error(err));
    };

    onClickFavorite = () => {
      const { t, item, setFavoriteAction } = this.props;

      setFavoriteAction("remove", item.id)
        .then(() => toastr.success(t("RemovedFromFavorites")))
        .catch((err) => toastr.error(err));
    };

    render() {
      const { itemTitle, showConvertDialog } = this.state;
      const {
        item,
        fileActionId,
        fileActionExt,
        isLoading,
        viewer,
        t,
        isTrashFolder,
        canWebEdit,
        canConvert,
      } = this.props;
      const { id, fileExst, updated, createdBy, access, fileStatus } = item;

      const titleWithoutExt = getTitleWithoutExst(item);

      const isEdit = id === fileActionId && fileExst === fileActionExt;

      const updatedDate = updated && this.getStatusByDate();

      const fileOwner =
        createdBy &&
        ((viewer.id === createdBy.id && t("AuthorMe")) ||
          createdBy.displayName);

      const accessToEdit =
        access === ShareAccessRights.FullAccess ||
        access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

      const linkStyles = isTrashFolder //|| window.innerWidth <= 1024
        ? { noHover: true }
        : { onClick: this.onFilesClick };

      const newItems = item.new || fileStatus === 2;
      const showNew = !!newItems;

      return isEdit ? (
        <EditingWrapperComponent
          itemTitle={itemTitle}
          itemId={id}
          isLoading={isLoading}
          renameTitle={this.renameTitle}
          onClickUpdateItem={this.onClickUpdateItem}
          cancelUpdateItem={this.cancelUpdateItem}
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
          <WrappedContent
            titleWithoutExt={titleWithoutExt}
            updatedDate={updatedDate}
            fileOwner={fileOwner}
            accessToEdit={accessToEdit}
            linkStyles={linkStyles}
            newItems={newItems}
            showNew={showNew}
            canWebEdit={canWebEdit}
            canConvert={canConvert}
            isTrashFolder={isTrashFolder}
            onFilesClick={this.onFilesClick}
            onShowVersionHistory={this.onShowVersionHistory}
            onBadgeClick={this.onBadgeClick}
            onClickLock={this.onClickLock}
            onClickFavorite={this.onClickFavorite}
            setConvertDialogVisible={this.setConvertDialogVisible}
            {...this.props}
          />
        </>
      );
    }
  }

  return inject(
    (
      {
        filesActionsStore,
        filesStore,
        selectedFolderStore,
        formatsStore,
        treeFoldersStore,
        mediaViewerDataStore,
        auth,
        versionHistoryStore,
        dialogsStore,
        uploadDataStore,
      },
      { item, t, history }
    ) => {
      const {
        editCompleteAction,
        markAsRead,
        lockFileAction,
        setFavoriteAction,
      } = filesActionsStore;
      const {
        filter,
        setIsLoading,
        fetchFiles,
        openDocEditor,
        updateFile,
        renameFolder,
        createFile,
        createFolder,
        isLoading,
        updateFileBadge,
      } = filesStore;
      const {
        iconFormatsStore,
        mediaViewersFormatsStore,
        docserviceStore,
      } = formatsStore;
      const {
        isRecycleBinFolder,
        expandedKeys,
        addExpandedKeys,
        isPrivacyFolder,
        updateRootBadge,
      } = treeFoldersStore;
      const { setMediaViewerData } = mediaViewerDataStore;
      const {
        type: fileActionType,
        extension: fileActionExt,
        id: fileActionId,
      } = filesStore.fileActionStore;
      const { replaceFileStream, setEncryptionAccess } = auth;
      const { culture, isDesktopClient, isTabletView } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const { setNewFilesPanelVisible, setNewFilesIds } = dialogsStore;
      const { secondaryProgressDataStore } = uploadDataStore;

      const {
        setSecondaryProgressBarData,
        clearSecondaryProgressData,
      } = secondaryProgressDataStore;
      const isImage = iconFormatsStore.isImage(item.fileExst);
      const isSound = iconFormatsStore.isSound(item.fileExst);
      const isVideo = mediaViewersFormatsStore.isVideo(item.fileExst);
      const canWebEdit = docserviceStore.canWebEdit(item.fileExst);
      return {
        t,
        editCompleteAction,
        filter,
        parentFolder: selectedFolderStore.parentId,
        setIsLoading,
        fetchFiles,
        isImage,
        isSound,
        isVideo,
        canWebEdit,
        isTrashFolder: isRecycleBinFolder,
        openDocEditor,
        expandedKeys,
        addExpandedKeys,
        setMediaViewerData,
        updateFile,
        renameFolder,
        fileActionId,
        editCompleteAction,
        fileActionType,
        createFile,
        isPrivacy: isPrivacyFolder,
        isDesktop: isDesktopClient,
        replaceFileStream,
        setEncryptionAccess,
        createFolder,
        fileActionExt,
        isLoading,
        culture,
        homepage: config.homepage,
        isTabletView,
        setIsVerHistoryPanel,
        fetchFileVersions,
        history,
        selectedFolderPathParts: selectedFolderStore.pathParts,
        markAsRead,
        setNewFilesPanelVisible,
        setNewFilesIds,
        updateRootBadge,
        updateFileBadge,
        setSecondaryProgressBarData,
        clearSecondaryProgressData,
        selectedFolderId: selectedFolderStore.id,
        lockFileAction,
        setFavoriteAction,
        viewer: auth.userStore.user,
      };
    }
  )(observer(WithContentActions));
}
