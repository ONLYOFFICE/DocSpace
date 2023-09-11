import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { useNavigate, useLocation } from "react-router-dom";
import queryString from "query-string";
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

    setRemoveMediaItem,
    userAccess,
    deleteDialogVisible,
    previewFile,
    fetchFiles,
    setIsLoading,

    setToPreviewFile,
    setScrollToItem,
    setCurrentId,

    setBufferSelection,

    archiveRoomsId,

    onShowInfoPanel,
    onClickDownload,

    onClickLinkEdit,
    onPreviewClick,
    onCopyLink,
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
    getFirstUrl,
    firstLoad,
    setSelection,
    activeFiles,
    activeFolders,
    onClickDownloadAs,
    someDialogIsOpen,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

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
    navigate(url);
  };

  const resetSelection = () => {
    setSelection([]);
  };

  const removeQuery = (queryName) => {
    const queryParams = new URLSearchParams(location.search);

    if (queryParams.has(queryName)) {
      queryParams.delete(queryName);
      navigate(_, {
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
        // try to fix with one check later (see deleteAction)
        const isActiveFile = activeFiles.find((elem) => elem.id === file.id);
        const isActiveFolder = activeFolders.find(
          (elem) => elem.id === file.id
        );

        if (isActiveFile || isActiveFolder) return;

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

    // const url = localStorage.getItem("isFirstUrl");
    const url = getFirstUrl();

    if (!url) {
      return;
    }

    const targetFile = files.find((item) => item.id === currentMediaFileId);
    if (targetFile) setBufferSelection(targetFile);

    navigate(url, { replace: true });
  };

  return (
    visible && (
      <MediaViewer
        t={t}
        userAccess={userAccess}
        someDialogIsOpen={someDialogIsOpen}
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
        onClickLinkEdit={onClickLinkEdit}
        onPreviewClick={onPreviewClick}
        onCopyLink={onCopyLink}
        onClickDownloadAs={onClickDownloadAs}
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
    clientLoadingStore,
  }) => {
    const {
      firstLoad,

      setIsSectionFilterLoading,
    } = clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

    const {
      files,
      userAccess,
      fetchFiles,

      setScrollToItem,
      setBufferSelection,
      setIsPreview,
      isPreview,
      resetUrl,
      setSelection,
      setAlreadyFetchingRooms,
      activeFiles,
      activeFolders,
    } = filesStore;
    const {
      visible,
      id: currentMediaFileId,
      currentPostionIndex,
      setMediaViewerData,
      getFirstUrl,
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
      onClickLinkEdit,
      onPreviewClick,
      onCopyLink,
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
      someDialogIsOpen: dialogsStore.someDialogIsOpen,
      fetchFiles,
      previewFile,
      setIsLoading,
      firstLoad,

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
      onClickLinkEdit,
      onPreviewClick,
      onCopyLink,
      onClickRename,
      onMoveAction,
      getIcon,
      onCopyAction,
      onDuplicate,
      archiveRoomsId,
      setSelection,
      getFirstUrl,
      activeFiles,
      activeFolders,
    };
  }
)(withTranslation(["Files", "Translations"])(observer(FilesMediaViewer)));
