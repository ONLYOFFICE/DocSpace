import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import history from "@docspace/common/history";
import MediaViewer from "@docspace/common/components/MediaViewer";
import { createTreeFolders } from "../../../helpers/files-helpers";

const FilesMediaViewer = (props) => {
  const {
    t,
    files,
    playlist,
    visible,
    currentMediaFileId,
    deleteItemAction,
    setMediaViewerData,
    mediaViewerMediaFormats,
    mediaViewerImageFormats,
    location,
    setRemoveMediaItem,
    userAccess,
    deleteDialogVisible,
    previewFile,
    fetchFiles,
    setIsLoading,
    setFirstLoad,
    setExpandedKeys,
    setToPreviewFile,
    expandedKeys,
    setScrollToItem,
    setCurrentId,
    setBufferSelection,
    mediaViewerAudioFormats,
    isFavoritesFolder,
  } = props;

  useEffect(() => {
    const previewId = queryString.parse(location.search).preview;

    if (previewId) {
      removeQuery("preview");
      onMediaFileClick(+previewId);
    }
  }, [removeQuery, onMediaFileClick]);

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

  const canDelete = (fileId) => true; //TODO:
  const canDownload = (fileId) => true; //TODO:

  const onDeleteMediaFile = (id) => {
    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFolder: t("FolderRemoved"),
      successRemoveFile: t("FileRemoved"),
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
    if (previewFile) {
      setIsLoading(true);
      setFirstLoad(true);

      fetchFiles(previewFile.folderId)
        .then((data) => {
          const pathParts = data.selectedFolder.pathParts;
          const newExpandedKeys = createTreeFolders(pathParts, expandedKeys);
          setExpandedKeys(newExpandedKeys);
        })
        .finally(() => {
          setIsLoading(false);
          setFirstLoad(false);
          setScrollToItem({ id: previewFile.id, type: "file" });
          setBufferSelection(previewFile);
          setToPreviewFile(null);
        });
    }

    setMediaViewerData({ visible: false, id: null });

    if (e) {
      const url = localStorage.getItem("isFirstUrl");

      if (!url) {
        return;
      }

      setScrollToItem({ id: currentMediaFileId, type: "file" });
      const targetFile = files.find((item) => item.id === currentMediaFileId);
      if (targetFile) setBufferSelection(targetFile);

      window.history.replaceState(null, null, url);
    }
  };

  const mediaFormats = [...mediaViewerMediaFormats, ...mediaViewerAudioFormats];

  return (
    visible && (
      <MediaViewer
        userAccess={userAccess}
        currentFileId={currentMediaFileId}
        allowConvert={true} //TODO:
        canDelete={canDelete} //TODO:
        canDownload={canDownload} //TODO:
        visible={visible}
        playlist={playlist}
        onDelete={onDeleteMediaFile}
        onDownload={onDownloadMediaFile}
        onClose={onMediaViewerClose}
        onEmptyPlaylistError={onMediaViewerClose}
        deleteDialogVisible={deleteDialogVisible}
        extsMediaPreviewed={mediaFormats} //TODO:
        extsImagePreviewed={mediaViewerImageFormats} //TODO:
        errorLabel={t("Translations:MediaLoadError")}
        isPreviewFile={!!previewFile}
        onChangeUrl={onChangeUrl}
        isFavoritesFolder={isFavoritesFolder}
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
  }) => {
    const {
      files,
      userAccess,
      fetchFiles,
      setIsLoading,
      setFirstLoad,
      setScrollToItem,
      setBufferSelection,
    } = filesStore;
    const {
      visible,
      id: currentMediaFileId,
      setMediaViewerData,
      playlist,
      previewFile,
      setToPreviewFile,
      setCurrentId,
    } = mediaViewerDataStore;
    const { deleteItemAction } = filesActionsStore;
    const { extsVideo, extsImage, extsAudio } = settingsStore;
    const {
      expandedKeys,
      setExpandedKeys,
      isFavoritesFolder,
    } = treeFoldersStore;

    return {
      files,
      playlist,
      userAccess,
      visible: playlist.length > 0 && visible,
      currentMediaFileId,
      deleteItemAction,
      setMediaViewerData,
      mediaViewerImageFormats: extsImage,
      mediaViewerMediaFormats: extsVideo,
      mediaViewerAudioFormats: extsAudio,
      setRemoveMediaItem: dialogsStore.setRemoveMediaItem,
      deleteDialogVisible: dialogsStore.deleteDialogVisible,
      fetchFiles,
      previewFile,
      setIsLoading,
      setFirstLoad,
      setExpandedKeys,
      setToPreviewFile,
      expandedKeys,
      setScrollToItem,
      setCurrentId,
      setBufferSelection,
      isFavoritesFolder,
    };
  }
)(
  withRouter(
    withTranslation(["Files", "Translations"])(observer(FilesMediaViewer))
  )
);
