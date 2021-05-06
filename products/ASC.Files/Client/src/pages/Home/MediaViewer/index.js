import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import history from "@appserver/common/history";
import MediaViewer from "@appserver/common/components/MediaViewer";

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
  } = props;

  useEffect(() => {
    const previewId = queryString.parse(location.search).preview;

    if (previewId) {
      removeQuery("preview");
      onMediaFileClick(+previewId);
    }
  }, [removeQuery, onMediaFileClick]);

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
      deleteOperation: t("DeleteOperation"),
      folderRemoved: t("FolderRemoved"),
      fileRemoved: t("FileRemoved"),
    };

    if (files.length > 0) {
      let file = files.find((file) => file.id === id);
      if (file) deleteItemAction(file.id, file.folderId, translations, true);
    }
  };

  const onDownloadMediaFile = (id) => {
    if (files.length > 0) {
      let viewUrlFile = files.find((file) => file.id === id).viewUrl;
      return window.open(viewUrlFile, "_blank");
    }
  };

  const onMediaViewerClose = () =>
    setMediaViewerData({ visible: false, id: null });

  return (
    visible && (
      <MediaViewer
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
        extsMediaPreviewed={mediaViewerMediaFormats} //TODO:
        extsImagePreviewed={mediaViewerImageFormats} //TODO:
      />
    )
  );
};

export default inject(
  ({ filesStore, mediaViewerDataStore, filesActionsStore, formatsStore }) => {
    const { files } = filesStore;
    const {
      visible,
      id: currentMediaFileId,
      setMediaViewerData,
      playlist,
    } = mediaViewerDataStore;
    const { deleteItemAction } = filesActionsStore;
    const { media, images } = formatsStore.mediaViewersFormatsStore;

    return {
      files,
      playlist,
      visible: playlist.length > 0 && visible,
      currentMediaFileId,
      deleteItemAction,
      setMediaViewerData,
      mediaViewerImageFormats: images,
      mediaViewerMediaFormats: media,
    };
  }
)(withRouter(withTranslation("Home")(observer(FilesMediaViewer))));
