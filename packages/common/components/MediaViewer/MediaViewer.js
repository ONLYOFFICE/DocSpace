import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import ImageViewer from "./sub-components/image-viewer";
import equal from "fast-deep-equal/react";
import Hammer from "hammerjs";
import { isMobileOnly } from "react-device-detect";
import { FileStatus } from "@docspace/common/constants";

const mediaTypes = Object.freeze({
  audio: 1,
  video: 2,
});

const ButtonKeys = Object.freeze({
  leftArrow: 37,
  rightArrow: 39,
  upArrow: 38,
  downArrow: 40,
  space: 32,
  esc: 27,
  ctr: 17,
  one: 49,
  del: 46,
  s: 83,
});

let ctrIsPressed = false;
class MediaViewer extends React.Component {
  constructor(props) {
    super(props);

    const item = props.playlist.find(
      (file) => String(file.fileId) === String(props.currentFileId)
    );

    const playlistPos = item ? item.id : 0;

    this.state = {
      visible: props.visible,
      allowConvert: true,
      playlist: props.playlist,
      playlistPos,
      fileUrl: item.src,
      canSwipeImage: true,
    };

    this.detailsContainer = React.createRef();
    this.viewerToolbox = React.createRef();
  }

  updateHammer() {
    const { playlistPos, playlist } = this.state;

    const currentFile = playlist[playlistPos];
    const { title } = currentFile;

    const ext = this.getFileExtension(title);
    const _this = this;

    if (this.hammer) {
      this.hammer.off("doubletap", this.prevMedia);
    }
    this.hammer = null;
    setTimeout(function () {
      try {
        if (_this.canImageView(ext)) {
          const pinch = new Hammer.Pinch();
          _this.hammer = Hammer(
            document.getElementsByClassName("react-viewer-canvas")[0]
          );
          _this.hammer.add([pinch]);

          _this.hammer.on("doubletap", _this.doubleTap);
        }
      } catch (ex) {
        //console.error("MediaViewer updateHammer", ex);
        this.hammer = null;
      }
    }, 500);
  }

  componentDidUpdate(prevProps, prevState) {
    const {
      visible,
      playlist,
      currentFileId,
      onEmptyPlaylistError,
    } = this.props;

    const { playlistPos, fileUrl } = this.state;
    const src = playlist[playlistPos]?.src;
    const title = playlist[playlistPos]?.title;
    const ext = this.getFileExtension(title);

    if (visible !== prevProps.visible) {
      const newPlaylistPos =
        playlist.length > 0
          ? playlist.find((file) => file.fileId === currentFileId).id
          : 0;

      this.setState({
        visible: visible,
        playlistPos: newPlaylistPos,
      });
    }

    if (
      src &&
      src !== fileUrl &&
      playlistPos === prevState.playlistPos &&
      ext !== ".tif" &&
      ext !== ".tiff"
    ) {
      this.setState({ fileUrl: src });
    }

    if (
      visible &&
      visible === prevProps.visible &&
      playlistPos !== prevState.playlistPos
    ) {
      this.updateHammer();
      if (ext === ".tiff" || ext === ".tif") {
        this.getTiffDataURL(src);
      } else {
        this.setState({ fileUrl: src });
      }
    }

    if (
      visible &&
      visible === prevProps.visible &&
      !equal(playlist, prevProps.playlist)
    ) {
      if (playlist.length > 0) {
        this.updateHammer();

        const newPlaylistPos =
          playlistPos < playlist.length
            ? playlist.length !== prevProps.playlist.length
              ? playlistPos + 1
              : playlistPos
            : 0;

        this.setState({
          playlist: playlist,
          playlistPos: newPlaylistPos,
        });
      } else {
        onEmptyPlaylistError();
        this.setState({
          visible: false,
        });
      }
    } else if (!equal(playlist, prevProps.playlist)) {
      this.setState({
        playlist: playlist,
      });
    }
  }

  componentDidMount() {
    const { playlist, files, setBufferSelection } = this.props;
    const { playlistPos } = this.state;

    const currentFile = playlist[playlistPos];

    const currentFileId =
      playlist.length > 0
        ? playlist.find((file) => file.id === playlistPos).fileId
        : 0;

    const targetFile = files.find((item) => item.id === currentFileId);
    if (targetFile) setBufferSelection(targetFile);
    const { src, title } = currentFile;
    const ext = this.getFileExtension(title);

    if (ext === ".tiff" || ext === ".tif") {
      this.getTiffDataURL(src);
    }

    this.updateHammer();

    document.addEventListener("keydown", this.onKeydown, false);
    document.addEventListener("keyup", this.onKeyup, false);
  }

  componentWillUnmount() {
    if (this.hammer) {
      this.hammer.off("doubletap", this.prevMedia);
    }
    document.removeEventListener("keydown", this.onKeydown, false);
    document.removeEventListener("keyup", this.onKeyup, false);
    this.onClose();
  }

  mapSupplied = {
    ".aac": { supply: "m4a", type: mediaTypes.audio },
    ".flac": { supply: "mp3", type: mediaTypes.audio },
    ".m4a": { supply: "m4a", type: mediaTypes.audio },
    ".mp3": { supply: "mp3", type: mediaTypes.audio },
    ".oga": { supply: "oga", type: mediaTypes.audio },
    ".ogg": { supply: "oga", type: mediaTypes.audio },
    ".wav": { supply: "wav", type: mediaTypes.audio },

    ".f4v": { supply: "m4v", type: mediaTypes.video },
    ".m4v": { supply: "m4v", type: mediaTypes.video },
    ".mov": { supply: "m4v", type: mediaTypes.video },
    ".mp4": { supply: "m4v", type: mediaTypes.video },
    ".ogv": { supply: "ogv", type: mediaTypes.video },
    ".webm": { supply: "webmv", type: mediaTypes.video },
    ".wmv": { supply: "m4v", type: mediaTypes.video, convertable: true },
    ".avi": { supply: "m4v", type: mediaTypes.video, convertable: true },
    ".mpeg": { supply: "m4v", type: mediaTypes.video, convertable: true },
    ".mpg": { supply: "m4v", type: mediaTypes.video, convertable: true },
  };

  canImageView = function (ext) {
    const { extsImagePreviewed } = this.props;
    return extsImagePreviewed.indexOf(ext) != -1;
  };

  canPlay = (fileTitle, allowConvert) => {
    const { extsMediaPreviewed } = this.props;

    const ext =
      fileTitle[0] === "." ? fileTitle : this.getFileExtension(fileTitle);

    const supply = this.mapSupplied[ext];

    const canConvert = allowConvert || this.props.allowConvert;

    return (
      !!supply &&
      extsMediaPreviewed.indexOf(ext) != -1 &&
      (!supply.convertable || canConvert)
    );
  };

  getFileExtension = (fileTitle) => {
    if (!fileTitle) {
      return "";
    }
    fileTitle = fileTitle.trim();
    const posExt = fileTitle.lastIndexOf(".");
    return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : "";
  };

  zoom = 1;

  handleZoomEnd = () => {
    this.zoom = 1;
  };

  handleZoomIn = (e) => {
    if (this.zoom - e.scale > 0.1) {
      this.zoom = e.scale;
      document.querySelector('li[data-key="zoomOut"]').click();
    }
  };

  handleZoomOut = (e) => {
    if (e.scale - this.zoom > 0.3) {
      this.zoom = e.scale;
      document.querySelector('li[data-key="zoomIn"]').click();
    }
  };

  doubleTap = () => {
    document.querySelector('li[data-key="zoomIn"]')?.click();
  };

  prevMedia = () => {
    const { playlistPos, playlist } = this.state;
    const { setBufferSelection } = this.props;

    let currentPlaylistPos = playlistPos;
    currentPlaylistPos--;
    if (currentPlaylistPos === -1) return;
    if (currentPlaylistPos < 0) currentPlaylistPos = playlist.length - 1;

    const currentFileId = playlist[currentPlaylistPos].fileId;

    const targetFile = this.props.files.find(
      (item) => item.id === currentFileId
    );
    setBufferSelection(targetFile);

    this.setState({
      playlistPos: currentPlaylistPos,
    });

    const id = playlist[currentPlaylistPos].fileId;
    this.props.onChangeUrl(id);
  };

  nextMedia = () => {
    const { playlistPos, playlist } = this.state;
    const { setBufferSelection } = this.props;

    let currentPlaylistPos = playlistPos;
    currentPlaylistPos = (currentPlaylistPos + 1) % playlist.length;
    if (currentPlaylistPos === 0) return;

    const currentFileId = playlist[currentPlaylistPos].fileId;

    const targetFile = this.props.files.find(
      (item) => item.id === currentFileId
    );
    setBufferSelection(targetFile);

    this.setState({
      playlistPos: currentPlaylistPos,
    });

    const id = playlist[currentPlaylistPos].fileId;
    this.props.onChangeUrl(id);
  };

  getOffset = () => {
    if (this.detailsContainer.current && this.viewerToolbox.current) {
      return (
        this.detailsContainer.current.offsetHeight +
        this.viewerToolbox.current.offsetHeight
      );
    } else {
      return 0;
    }
  };

  onDelete = () => {
    const { playlist, playlistPos } = this.state;

    let currentFileId =
      playlist.length > 0
        ? playlist.find((file) => file.id === playlistPos).fileId
        : 0;
    this.props.onDelete && this.props.onDelete(currentFileId);
    this.setState({
      canSwipeImage: false,
    });
  };

  onDownload = () => {
    const { playlist, playlistPos } = this.state;

    let currentFileId =
      playlist.length > 0
        ? playlist.find((file) => file.id === playlistPos).fileId
        : 0;
    this.props.onDownload && this.props.onDownload(currentFileId);
  };

  onKeyup = (e) => {
    if (ButtonKeys.ctr === e.keyCode) {
      ctrIsPressed = false;
    }
  };

  onKeydown = (e) => {
    let isActionKey = false;
    for (let key in ButtonKeys) {
      if (ButtonKeys[key] === e.keyCode) {
        e.preventDefault();
        isActionKey = true;
      }
    }

    if (isActionKey) {
      switch (e.keyCode) {
        case ButtonKeys.leftArrow:
          if (document.fullscreenElement) return;
          this.state.canSwipeImage
            ? ctrIsPressed
              ? document.getElementsByClassName("iconContainer rotateLeft")
                  .length > 0 &&
                document
                  .getElementsByClassName("iconContainer rotateLeft")[0]
                  .click()
              : this.prevMedia()
            : null;
          break;
        case ButtonKeys.rightArrow:
          if (document.fullscreenElement) return;
          this.state.canSwipeImage
            ? ctrIsPressed
              ? document.getElementsByClassName("iconContainer rotateRight")
                  .length > 0 &&
                document
                  .getElementsByClassName("iconContainer rotateRight")[0]
                  .click()
              : this.nextMedia()
            : null;
          break;
        case ButtonKeys.space:
          document.getElementsByClassName("video-play").length > 0 &&
            document.getElementsByClassName("video-play")[0].click();
          break;
        case ButtonKeys.esc:
          if (!this.props.deleteDialogVisible) this.props.onClose();
          break;
        case ButtonKeys.upArrow:
          document.getElementsByClassName("iconContainer zoomIn").length > 0 &&
            document.getElementsByClassName("iconContainer zoomIn")[0].click();
          break;
        case ButtonKeys.downArrow:
          document.getElementsByClassName("iconContainer zoomOut").length > 0 &&
            document.getElementsByClassName("iconContainer zoomOut")[0].click();
          break;
        case ButtonKeys.ctr:
          ctrIsPressed = true;
          break;
        case ButtonKeys.s:
          if (ctrIsPressed) this.onDownload();
          break;
        case ButtonKeys.one:
          ctrIsPressed &&
            document.getElementsByClassName("iconContainer reset").length > 0 &&
            document.getElementsByClassName("iconContainer reset")[0].click();
          break;
        case ButtonKeys.del:
          this.onDelete();
          break;

        default:
          break;
      }
    }
  };

  onClose = (e) => {
    this.props.onClose(e);
    this.setState({ visible: false });
  };

  getTiffDataURL = (src) => {
    if (!window.Tiff) return;
    const _this = this;
    const xhr = new XMLHttpRequest();
    xhr.responseType = "arraybuffer";
    xhr.open("GET", src);
    xhr.onload = function () {
      try {
        const tiff = new window.Tiff({ buffer: xhr.response });
        const dataUrl = tiff.toDataURL();
        _this.setState({ fileUrl: dataUrl });
      } catch (e) {
        console.log(e);
      }
    };
    xhr.send();
  };

  render() {
    const { playlistPos, playlist, visible, fileUrl } = this.state;
    const {
      t,
      onClose,
      userAccess,
      canDelete,
      canDownload,
      errorLabel,
      isPreviewFile,
      onClickFavorite,
      onShowInfoPanel,
      onClickDownload,
      onMoveAction,
      onCopyAction,
      onDuplicate,
      onClickDownloadAs,
      getIcon,
      onClickRename,
      onClickDelete,
      setBufferSelection,
      files,
    } = this.props;

    const currentFileId =
      playlist.length > 0
        ? playlist.find((file) => file.id === playlistPos).fileId
        : 0;

    const currentFile = playlist[playlistPos];

    const targetFile =
      files.find((item) => item.id === currentFileId) || playlist[0];
    const { title } = currentFile;

    let isImage = false;
    let isVideo = false;
    let isAudio = false;
    let canOpen = true;

    const isFavorite =
      (playlist[playlistPos].fileStatus & FileStatus.IsFavorite) ===
      FileStatus.IsFavorite;

    const ext = this.getFileExtension(title);

    const onSetSelectionFile = () => {
      setBufferSelection(targetFile);
    };

    const getContextModel = () => {
      const desktopModel = [
        {
          key: "download",
          label: t("Common:Download"),
          icon: "images/download.react.svg",
          onClick: () => onClickDownload(targetFile, t),
          disabled: false,
        },
        {
          key: "rename",
          label: t("Rename"),
          icon: "images/rename.react.svg",
          onClick: () => onClickRename(targetFile),
          disabled: false,
        },
        {
          key: "delete",
          label: t("Common:Delete"),
          icon: "images/trash.react.svg",
          onClick: () => onClickDelete(targetFile, t),
          disabled: false,
        },
      ];

      const model = [
        {
          id: "option_room-info",
          key: "room-info",
          label: t("Common:Info"),
          icon: "/static/images/info.outline.react.svg",
          onClick: () => {
            return onShowInfoPanel(targetFile);
          },
          disabled: false,
        },
        {
          key: "download",
          label: t("Common:Download"),
          icon: "images/download.react.svg",
          onClick: () => onClickDownload(targetFile, t),
          disabled: false,
        },
        {
          key: "move-to",
          label: t("MoveTo"),
          icon: "images/move.react.svg",
          onClick: onMoveAction,
          disabled: isPreviewFile,
        },
        // {
        //   key: "download-as",
        //   label: t("Translations:DownloadAs"),
        //   icon: "images/download-as.react.svg", // TODO: uncomment when we can download media by changing the format
        //   onClick: onClickDownloadAs,
        //   disabled: false,
        // },
        {
          id: "option_copy-to",
          key: "copy-to",
          label: t("Translations:Copy"),
          icon: "/static/images/copy.react.svg",
          onClick: onCopyAction,
          disabled: isPreviewFile,
        },
        {
          id: "option_create-copy",
          key: "copy",
          label: t("Common:Duplicate"),
          icon: "/static/images/duplicate.react.svg",
          onClick: () => onDuplicate(targetFile, t),
          disabled: isPreviewFile,
        },
        {
          key: "rename",
          label: t("Rename"),
          icon: "images/rename.react.svg",
          onClick: () => onClickRename(targetFile),
          disabled: isPreviewFile,
        },

        {
          key: "separator0",
          isSeparator: true,
          disabled: isPreviewFile,
        },
        {
          key: "delete",
          label: t("Common:Delete"),
          icon: "images/trash.react.svg",
          onClick: () => onClickDelete(targetFile, t),
          disabled: isPreviewFile,
        },
      ];

      return isMobileOnly
        ? model
        : isImage && !isMobileOnly
        ? desktopModel.filter((el) => el.key !== "download")
        : desktopModel;
    };

    if (!this.canPlay(ext) && !this.canImageView(ext)) {
      canOpen = false;
      this.props.onError && this.props.onError();
    }

    if (this.canImageView(ext)) {
      isImage = true;
    } else {
      isImage = false;
      isVideo = this.mapSupplied[ext]
        ? this.mapSupplied[ext].type == mediaTypes.video
        : false;
      isAudio = this.mapSupplied[ext]
        ? this.mapSupplied[ext].type == mediaTypes.audio
        : false;
    }

    let audioIcon = getIcon(96, ext);
    let headerIcon = getIcon(24, ext);

    // TODO: rewrite with fileURL
    /*if (this.mapSupplied[ext])
      if (!isImage && this.mapSupplied[ext].convertable && !src.includes("#")) {
        src += (src.includes("?") ? "&" : "?") + "convpreview=true";
      }*/
    return (
      <>
        {canOpen && (
          <ImageViewer
            userAccess={userAccess}
            visible={visible}
            title={title}
            onClose={this.onClose}
            images={[{ src: fileUrl, alt: "" }]}
            inactive={playlist.length <= 1}
            playlist={playlist}
            playlistPos={playlistPos}
            onNextClick={this.nextMedia}
            onSetSelectionFile={onSetSelectionFile}
            contextModel={getContextModel}
            onPrevClick={this.prevMedia}
            onDeleteClick={this.onDelete}
            isFavorite={isFavorite}
            headerIcon={headerIcon}
            isImage={isImage}
            isAudio={isAudio}
            isVideo={isVideo}
            isPreviewFile={isPreviewFile}
            audioIcon={audioIcon}
            onDownloadClick={this.onDownload}
            //    isFavoritesFolder={isFavoritesFolder}
          />
        )}
      </>
    );
  }
}

MediaViewer.propTypes = {
  allowConvert: PropTypes.bool,
  visible: PropTypes.bool,
  currentFileId: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  playlist: PropTypes.arrayOf(PropTypes.object),
  extsImagePreviewed: PropTypes.arrayOf(PropTypes.string),
  extsMediaPreviewed: PropTypes.arrayOf(PropTypes.string),
  onError: PropTypes.func,
  canDelete: PropTypes.func,
  canDownload: PropTypes.func,
  onDelete: PropTypes.func,
  onDownload: PropTypes.func,
  onClose: PropTypes.func,
  onEmptyPlaylistError: PropTypes.func,
  deleteDialogVisible: PropTypes.bool,
  errorLabel: PropTypes.string,
  isPreviewFile: PropTypes.bool,
  onChangeUrl: PropTypes.func,
};

MediaViewer.defaultProps = {
  currentFileId: 0,
  visible: false,
  allowConvert: true,
  canDelete: () => {
    return true;
  },
  canDownload: () => {
    return true;
  },
  isPreviewFile: false,
};

export default MediaViewer;
