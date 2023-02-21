import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import history from "@docspace/common/history";
import MediaViewer from "@docspace/common/components/MediaViewer";

const FilesMediaViewer = (props) => {
  const {
    t,
    files,
    playlist,
    currentPostionIndex,
    visible,
    currentMediaFileId,
    deleteItemAction,
    setMediaViewerData,
    location,
    setRemoveMediaItem,
    userAccess,
    deleteDialogVisible,
    previewFile,
    fetchFiles,
    setIsLoading,
    setFirstLoad,
    setToPreviewFile,
    setScrollToItem,
    setCurrentId,
    setAlreadyFetchingRooms,
    setBufferSelection,
    isFavoritesFolder,
    archiveRoomsId,
    onClickFavorite,
    onShowInfoPanel,
    onClickDownload,
    onClickDownloadAs,
    onClickRename,
    onClickDelete,
    onMoveAction,
    onCopyAction,
    getIcon,
    onDuplicate,
    extsImagePreviewed,
    extsMediaPreviewed,
    setIsPreview,
    isPreview,
    nextMedia,
    prevMedia,
    resetUrl,
    firstLoad,
    setSelection,
  } = props;

  useEffect(() => {
    if (visible) {
      resetSelection();
    }
  }, [visible]);

  useEffect(() => {
    const previewId = queryString.parse(location.search).preview;

    if (previewId) {
      removeQuery("preview");
      onMediaFileClick(+previewId);
    }
  }, [removeQuery, onMediaFileClick]);

  useEffect(() => {
    if (previewFile) {
      // fetch file after preview with
      fetchFiles(previewFile.folderId).finally(() => {
        setIsLoading(false);
        setFirstLoad(false);
        setAlreadyFetchingRooms(false);
      });
    }
  }, [previewFile]);

  useEffect(() => {
    window.addEventListener("popstate", onButtonBackHandler);

    return () => window.removeEventListener("popstate", onButtonBackHandler);
  }, [onButtonBackHandler]);

  const onButtonBackHandler = () => {
    const hash = window.location.hash;
    const id = hash.slice(9);
    if (!id) {
      setMediaViewerData({ visible: false, id: null });
      return;
    }
    setMediaViewerData({ visible: true, id });
  };

  const onChangeUrl = (id) => {
    const url = "/products/files/#preview/" + id;
    setCurrentId(id);
    window.history.pushState(null, null, url);
  };

  const resetSelection = () => {
    setSelection([]);
  };

  const removeQuery = (queryName) => {
    const queryParams = new URLSearchParams(location.search);

    if (queryParams.has(queryName)) {
      queryParams.delete(queryName);
      history.replace({
        search: queryParams.toString(),
      });
    }
  };

  const onMediaFileClick = (id) => {
    //const itemId = typeof id !== "object" ? id : this.props.selection[0].id; TODO:

    if (typeof id !== "object") {
      const item = { visible: true, id };
      setMediaViewerData(item);
    }
  };

  const onDeleteMediaFile = (id) => {
    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFolder: t("Files:FolderRemoved"),
      successRemoveFile: t("Files:FileRemoved"),
    };

    if (files.length > 0) {
      let file = files.find((file) => file.id === id);
      if (file) {
        setRemoveMediaItem(file);
        deleteItemAction(file.id, translations, true, file.providerKey);
      }
    }
  };

  const onDownloadMediaFile = (id) => {
    if (playlist.length > 0) {
      let viewUrlFile = playlist.find((file) => file.fileId === id).src;
      return window.open(viewUrlFile, "_self");
    }
  };

  const onMediaViewerClose = (e) => {
    if (isPreview) {
      setIsPreview(false);
      resetUrl();
      if (previewFile) {
        setScrollToItem({ id: previewFile.id, type: "file" });
        setBufferSelection(previewFile);
      }
      setToPreviewFile(null);
    }

    setMediaViewerData({ visible: false, id: null });

    const url = localStorage.getItem("isFirstUrl");

    if (!url) {
      return;
    }

    setScrollToItem({ id: currentMediaFileId, type: "file" });
    const targetFile = files.find((item) => item.id === currentMediaFileId);
    if (targetFile) setBufferSelection(targetFile);

    window.history.replaceState(null, null, url);
  };

  return (
    visible && (
      <MediaViewer
        t={t}
        userAccess={userAccess}
        currentFileId={currentMediaFileId}
        visible={visible}
        playlist={playlist}
        playlistPos={currentPostionIndex}
        onDelete={onDeleteMediaFile}
        onDownload={onDownloadMediaFile}
        setBufferSelection={setBufferSelection}
        archiveRoomsId={archiveRoomsId}
        files={files}
        onClickDownload={onClickDownload}
        onShowInfoPanel={onShowInfoPanel}
        onClickDelete={onClickDelete}
        onClickRename={onClickRename}
        onMoveAction={onMoveAction}
        onCopyAction={onCopyAction}
        onDuplicate={onDuplicate}
        onClose={onMediaViewerClose}
        getIcon={getIcon}
        onEmptyPlaylistError={onMediaViewerClose}
        deleteDialogVisible={deleteDialogVisible}
        extsMediaPreviewed={extsMediaPreviewed}
        extsImagePreviewed={extsImagePreviewed}
        isPreviewFile={firstLoad}
        onChangeUrl={onChangeUrl}
        nextMedia={nextMedia}
        prevMedia={prevMedia}
      />
    )
  );
};

export default inject(
  ({
    filesStore,
    mediaViewerDataStore,
    filesActionsStore,
    settingsStore,
    dialogsStore,
    treeFoldersStore,
    contextOptionsStore,
  }) => {
    const {
      files,
      userAccess,
      fetchFiles,
      setIsLoading,
      firstLoad,
      setFirstLoad,
      setScrollToItem,
      setBufferSelection,
      setIsPreview,
      isPreview,
      resetUrl,
      setSelection,
      setAlreadyFetchingRooms,
    } = filesStore;
    const {
      visible,
      id: currentMediaFileId,
      currentPostionIndex,
      setMediaViewerData,
      playlist,
      previewFile,
      setToPreviewFile,
      setCurrentId,
      nextMedia,
      prevMedia,
    } = mediaViewerDataStore;
    const { deleteItemAction } = filesActionsStore;
    const { getIcon, extsImagePreviewed, extsMediaPreviewed } = settingsStore;
    const { isFavoritesFolder, archiveRoomsId } = treeFoldersStore;

    const {
      onClickFavorite,
      onShowInfoPanel,
      onClickDownloadAs,
      onClickDownload,
      onClickRename,
      onClickDelete,
      onMoveAction,
      onCopyAction,
      onDuplicate,
    } = contextOptionsStore;

    return {
      files,
      playlist,
      currentPostionIndex,
      nextMedia,
      prevMedia,
      userAccess,
      visible: playlist.length > 0 && visible,
      currentMediaFileId,
      deleteItemAction,
      setMediaViewerData,
      extsImagePreviewed,
      extsMediaPreviewed,
      setRemoveMediaItem: dialogsStore.setRemoveMediaItem,
      deleteDialogVisible: dialogsStore.deleteDialogVisible,
      fetchFiles,
      previewFile,
      setIsLoading,
      firstLoad,
      setFirstLoad,
      setToPreviewFile,
      setIsPreview,
      resetUrl,
      isPreview,
      setScrollToItem,
      setCurrentId,
      setBufferSelection,
      setAlreadyFetchingRooms,
      isFavoritesFolder,
      onClickFavorite,
      onClickDownloadAs,
      onClickDelete,
      onClickDownload,
      onShowInfoPanel,
      onClickRename,
      onMoveAction,
      getIcon,
      onCopyAction,
      onDuplicate,
      archiveRoomsId,
      setSelection,
    };
  }
)(
  withRouter(
    withTranslation(["Files", "Translations"])(observer(FilesMediaViewer))
  )
);
