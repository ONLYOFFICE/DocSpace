import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Text from "@appserver/components/text";
import MediaDeleteIcon from "../../../../public/images/media.delete.react.svg";
import MediaDownloadIcon from "../../../../public/images/media.download.react.svg";
import ImageViewer from "./sub-components/image-viewer";
import VideoViewer from "./sub-components/video-viewer";
import MediaScrollButton from "./sub-components/scroll-button";
import ControlBtn from "./sub-components/control-btn";
import StyledMediaViewer from "./StyledMediaViewer";
import equal from "fast-deep-equal/react";
import Hammer from "hammerjs";
import IconButton from "@appserver/components/icon-button";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { isDesktop } from "react-device-detect";

const StyledVideoViewer = styled(VideoViewer)`
  z-index: 301;
`;
const mediaTypes = Object.freeze({
  audio: 1,
  video: 2,
});

const ButtonKeys = Object.freeze({
  leftArrow: 37,
  rightArrow: 39,
  upArrow: 38,
  downArrow: 40,
  esc: 27,
  ctr: 17,
  one: 49,
  del: 46,
  s: 83,
});

const StyledMediaDeleteIcon = styled(MediaDeleteIcon)`
  ${commonIconsStyles}
`;

const StyledMediaDownloadIcon = styled(MediaDownloadIcon)`
  ${commonIconsStyles}
`;

let ctrIsPressed = false;
class MediaViewer extends React.Component {
  constructor(props) {
    super(props);

    const item = props.playlist.find(
      (file) => file.fileId === props.currentFileId
    );

    const playlistPos = item ? item.id : 0;

    this.state = {
      visible: props.visible,
      allowConvert: true,
      playlist: props.playlist,
      playlistPos,
      fileUrl: item.src,
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
      this.hammer.off("swipeleft", this.nextMedia);
      this.hammer.off("swiperight", this.prevMedia);
      this.hammer.off("pinchout", this.prevMedia);
      this.hammer.off("pinchin", this.prevMedia);
      this.hammer.off("pinchend", this.prevMedia);
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
          _this.hammer.on("pinchout", _this.handleZoomOut);
          _this.hammer.on("pinchin", _this.handleZoomIn);
          _this.hammer.on("pinchend", _this.handleZoomEnd);
          _this.hammer.on("doubletap", _this.doubleTap);
        } else if (
          _this.mapSupplied[ext] &&
          (_this.mapSupplied[ext].type == mediaTypes.video ||
            _this.mapSupplied[ext].type == mediaTypes.audio)
        ) {
          _this.hammer = Hammer(
            document.getElementsByClassName("videoViewerOverlay")[0]
          );
        }
        if (_this.hammer && !isDesktop) {
          _this.hammer.on("swipeleft", _this.nextMedia);
          _this.hammer.on("swiperight", _this.prevMedia);
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

        const newPlaylistPos = playlistPos < playlist.length ? playlistPos : 0;

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
    const { playlist } = this.props;
    const { playlistPos } = this.state;

    const currentFile = playlist[playlistPos];
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
      this.hammer.off("swipeleft", this.nextMedia);
      this.hammer.off("swiperight", this.prevMedia);
      this.hammer.off("pinchout", this.prevMedia);
      this.hammer.off("pinchin", this.prevMedia);
      this.hammer.off("pinchend", this.prevMedia);
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
    document.querySelector('li[data-key="zoomIn"]').click();
  };

  prevMedia = () => {
    const { playlistPos, playlist } = this.state;

    let currentPlaylistPos = playlistPos;
    currentPlaylistPos--;
    if (currentPlaylistPos < 0) currentPlaylistPos = playlist.length - 1;

    this.setState({
      playlistPos: currentPlaylistPos,
    });
  };

  nextMedia = () => {
    const { playlistPos, playlist } = this.state;

    let currentPlaylistPos = playlistPos;
    currentPlaylistPos = (currentPlaylistPos + 1) % playlist.length;

    this.setState({
      playlistPos: currentPlaylistPos,
    });
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
          ctrIsPressed
            ? document.getElementsByClassName("iconContainer rotateLeft")
                .length > 0 &&
              document
                .getElementsByClassName("iconContainer rotateLeft")[0]
                .click()
            : this.prevMedia();
          break;
        case ButtonKeys.rightArrow:
          ctrIsPressed
            ? document.getElementsByClassName("iconContainer rotateRight")
                .length > 0 &&
              document
                .getElementsByClassName("iconContainer rotateRight")[0]
                .click()
            : this.nextMedia();
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

  onClose = () => {
    this.props.onClose();
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
      onClose,
      userAccess,
      canDelete,
      canDownload,
      errorLabel,
      previewFile,
    } = this.props;

    const currentFileId =
      playlist.length > 0
        ? playlist.find((file) => file.id === playlistPos).fileId
        : 0;

    const currentFile = playlist[playlistPos];
    const { title } = currentFile;

    let isImage = false;
    let isVideo = false;
    let canOpen = true;

    const ext = this.getFileExtension(title);

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
    }

    // TODO: rewrite with fileURL
    /*if (this.mapSupplied[ext])
      if (!isImage && this.mapSupplied[ext].convertable && !src.includes("#")) {
        src += (src.includes("?") ? "&" : "?") + "convpreview=true";
      }*/

    return (
      <StyledMediaViewer visible={visible}>
        <div className="videoViewerOverlay"></div>
        {!isImage && (
          <>
            <MediaScrollButton
              orientation="right"
              onClick={this.prevMedia}
              inactive={playlist.length <= 1}
            />
            <MediaScrollButton
              orientation="left"
              onClick={this.nextMedia}
              inactive={playlist.length <= 1}
            />
          </>
        )}

        <div>
          <div className="details" ref={this.detailsContainer}>
            <Text isBold fontSize="14px" color="#fff" className="title">
              {title}
            </Text>
            <ControlBtn
              onClick={onClose && onClose}
              className="mediaPlayerClose"
            >
              <IconButton
                color="#fff"
                iconName="/static/images/cross.react.svg"
                size={25}
                isClickable
              />
            </ControlBtn>
          </div>
        </div>
        {canOpen &&
          (isImage ? (
            <ImageViewer
              userAccess={userAccess}
              visible={visible}
              onClose={this.onClose}
              images={[{ src: fileUrl, alt: "" }]}
              inactive={playlist.length <= 1}
              onNextClick={this.nextMedia}
              onPrevClick={this.prevMedia}
              onDeleteClick={this.onDelete}
              onDownloadClick={this.onDownload}
            />
          ) : (
            <StyledVideoViewer
              url={fileUrl}
              isVideo={isVideo}
              getOffset={this.getOffset}
              errorLabel={errorLabel}
            />
          ))}
        <div className="mediaViewerToolbox" ref={this.viewerToolbox}>
          {!isImage && (
            <span>
              {canDelete(currentFileId) && !previewFile && (
                <ControlBtn onClick={this.onDelete}>
                  <div className="deleteBtnContainer">
                    <StyledMediaDeleteIcon size="scale" />
                  </div>
                </ControlBtn>
              )}
              {canDownload(currentFileId) && (
                <ControlBtn onClick={this.onDownload}>
                  <div className="downloadBtnContainer">
                    <StyledMediaDownloadIcon size="scale" />
                  </div>
                </ControlBtn>
              )}
            </span>
          )}
        </div>
      </StyledMediaViewer>
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
  previewFile: PropTypes.bool,
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
  previewFile: false,
};

export default MediaViewer;
