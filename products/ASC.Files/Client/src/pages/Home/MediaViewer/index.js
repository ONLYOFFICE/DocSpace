import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import history from "@appserver/common/history";
import MediaViewer from "@appserver/common/components/MediaViewer";
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
    expandedKeys,
  } = props;

  const [firstUrl, setFirstUrl] = useState();

  useEffect(() => {
    const previewId = queryString.parse(location.search).preview;

    if (previewId) {
      removeQuery("preview");
      onMediaFileClick(+previewId);
    }
  }, [removeQuery, onMediaFileClick]);

  const checkURLchange = () => {
    if (!oldURL.includes("#preview")) {
      setFirstUrl(oldURL);
    }
  };

  let oldURL = window.location.href;
  setInterval(checkURLchange, 1000);

  window.onpopstate = function (event) {
    const hash = window.location.hash;
    const id = +hash.slice(9);
    if (!id) {
      setMediaViewerData({ visible: false, id: null });
      return;
    }
    setMediaViewerData({ visible: true, id });
  };

  const onChangeUrl = (id) => {
    const url = "/products/files/#preview/" + id;
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
      folderRemoved: t("FolderRemoved"),
      fileRemoved: t("FileRemoved"),
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

  const onMediaViewerClose = (event) => {
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
        });
    }
    setMediaViewerData({ visible: false, id: null });

    if (event.currentTarget.closed === false) {
      return;
    } else {
      window.history.replaceState(null, null, firstUrl);
    }
  };

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
        extsMediaPreviewed={mediaViewerMediaFormats} //TODO:
        extsImagePreviewed={mediaViewerImageFormats} //TODO:
        errorLabel={t("Translations:MediaLoadError")}
        previewFile={previewFile}
        onChangeUrl={onChangeUrl}
      />
    )
  );
};

export default inject(
  ({
    filesStore,
    mediaViewerDataStore,
    filesActionsStore,
    formatsStore,
    dialogsStore,
    treeFoldersStore,
  }) => {
    const {
      files,
      userAccess,
      fetchFiles,
      setIsLoading,
      setFirstLoad,
    } = filesStore;
    const {
      visible,
      id: currentMediaFileId,
      setMediaViewerData,
      playlist,
      previewFile,
    } = mediaViewerDataStore;
    const { deleteItemAction } = filesActionsStore;
    const { media, images } = formatsStore.mediaViewersFormatsStore;
    const { expandedKeys, setExpandedKeys } = treeFoldersStore;

    return {
      files,
      playlist,
      userAccess,
      visible: playlist.length > 0 && visible,
      currentMediaFileId,
      deleteItemAction,
      setMediaViewerData,
      mediaViewerImageFormats: images,
      mediaViewerMediaFormats: media,
      setRemoveMediaItem: dialogsStore.setRemoveMediaItem,
      deleteDialogVisible: dialogsStore.deleteDialogVisible,
      fetchFiles,
      previewFile,
      setIsLoading,
      setFirstLoad,
      setExpandedKeys,
      expandedKeys,
    };
  }
)(
  withRouter(
    withTranslation(["Home", "Translations"])(observer(FilesMediaViewer))
  )
);
