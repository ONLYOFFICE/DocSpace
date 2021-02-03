import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons, Text } from "asc-web-components";

import ImageViewer from "./sub-components/image-viewer";
import VideoViewer from "./sub-components/video-viewer";
import MediaScrollButton from "./sub-components/scroll-button";
import ControlBtn from "./sub-components/control-btn";
import StyledMediaViewer from "./StyledMediaViewer";
import equal from "fast-deep-equal/react";
import Hammer from "hammerjs";

const StyledVideoViewer = styled(VideoViewer)`
  z-index: 4001;
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
});

let ctrIsPressed = false;
class MediaViewer extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: props.visible,
      allowConvert: true,
      playlist: props.playlist,
      playlistPos:
        props.playlist.length > 0
          ? props.playlist.find((file) => file.fileId === props.currentFileId)
              .id
          : 0,
    };

    this.detailsContainer = React.createRef();
    this.viewerToolbox = React.createRef();
  }

  updateHammer() {
    let currentPlaylistPos = this.state.playlistPos;

    let currentFile = this.state.playlist[currentPlaylistPos];
    let fileTitle = currentFile.title;
    let url = currentFile.src;
    var ext = this.getFileExtension(fileTitle)
      ? this.getFileExtension(fileTitle)
      : this.getFileExtension(url);
    var _this = this;

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
        if (_this.hammer) {
          _this.hammer.on("swipeleft", _this.nextMedia);
          _this.hammer.on("swiperight", _this.prevMedia);
        }
      } catch (ex) {
        console.error("MediaViewer updateHammer", ex);
      }
    }, 500);
  }

  componentDidUpdate(prevProps) {
    this.updateHammer();
    if (this.props.visible !== prevProps.visible) {
      this.setState({
        visible: this.props.visible,
        playlistPos:
          this.props.playlist.length > 0
            ? this.props.playlist.find(
                (file) => file.fileId === this.props.currentFileId
              ).id
            : 0,
      });
    }
    if (
      this.props.visible &&
      this.props.visible === prevProps.visible &&
      !equal(this.props.playlist, prevProps.playlist)
    ) {
      let playlistPos = 0;
      if (this.props.playlist.length > 0) {
        if (this.props.playlist.length - 1 < this.state.playlistPos) {
          playlistPos = this.props.playlist.length - 1;
        }
        this.setState({
          playlist: this.props.playlist,
          playlistPos: playlistPos,
        });
      } else {
        this.props.onEmptyPlaylistError();
        this.setState({
          visible: false,
        });
      }
    } else if (!equal(this.props.playlist, prevProps.playlist)) {
      this.setState({
        playlist: this.props.playlist,
      });
    }
  }

  componentDidMount() {
    var _this = this;
    setTimeout(function () {
      if (document.getElementsByClassName("react-viewer-canvas").length > 0) {
        _this.hammer = Hammer(
          document.getElementsByClassName("react-viewer-canvas")[0]
        );
        var pinch = new Hammer.Pinch();
        _this.hammer.add([pinch]);
        _this.hammer.on("pinchout", _this.handleZoomOut);
        _this.hammer.on("pinchin", _this.handleZoomIn);
        _this.hammer.on("pinchend", _this.handleZoomEnd);
        _this.hammer.on("doubletap", _this.doubleTap);
      } else {
        _this.hammer = Hammer(
          document.getElementsByClassName("videoViewerOverlay")[0]
        );
      }
      if (_this.hammer) {
        _this.hammer.on("swipeleft", _this.nextMedia);
        _this.hammer.on("swiperight", _this.prevMedia);
      }
    }, 500);
    document.addEventListener("keydown", this.onKeydown, false);
    document.addEventListener("keyup", this.onKeyup, false);
  }

  componentWillUnmount() {
    this.hammer.off("swipeleft", this.nextMedia);
    this.hammer.off("swiperight", this.prevMedia);
    this.hammer.off("pinchout", this.prevMedia);
    this.hammer.off("pinchin", this.prevMedia);
    this.hammer.off("pinchend", this.prevMedia);
    this.hammer.off("doubletap", this.prevMedia);
    document.removeEventListener("keydown", this.onKeydown, false);
    document.removeEventListener("keyup", this.onKeyup, false);
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
    return this.props.extsImagePreviewed.indexOf(ext) != -1;
  };
  canPlay = (fileTitle, allowConvert) => {
    var ext =
      fileTitle[0] === "." ? fileTitle : this.getFileExtension(fileTitle);

    var supply = this.mapSupplied[ext];

    var canConv = allowConvert || this.props.allowConvert;

    return (
      !!supply &&
      this.props.extsMediaPreviewed.indexOf(ext) != -1 &&
      (!supply.convertable || canConv)
    );
  };

  getFileExtension = (fileTitle) => {
    if (!fileTitle) {
      return "";
    }
    fileTitle = fileTitle.trim();
    var posExt = fileTitle.lastIndexOf(".");
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
    let currentPlaylistPos = this.state.playlistPos;
    currentPlaylistPos--;
    if (currentPlaylistPos < 0)
      currentPlaylistPos = this.state.playlist.length - 1;

    this.setState({
      playlistPos: currentPlaylistPos,
    });
  };

  nextMedia = () => {
    let currentPlaylistPos = this.state.playlistPos;
    currentPlaylistPos = (currentPlaylistPos + 1) % this.state.playlist.length;

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
    let currentFileId =
      this.state.playlist.length > 0
        ? this.state.playlist.find((file) => file.id === this.state.playlistPos)
            .fileId
        : 0;
    this.props.onDelete && this.props.onDelete(currentFileId);
  };
  onDownload = () => {
    let currentFileId =
      this.state.playlist.length > 0
        ? this.state.playlist.find((file) => file.id === this.state.playlistPos)
            .fileId
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
          this.props.onClose();
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
        case ButtonKeys.one:
          ctrIsPressed &&
            document.getElementsByClassName("iconContainer reset").length > 0 &&
            document.getElementsByClassName("iconContainer reset")[0].click();
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

  render() {
    let currentPlaylistPos = this.state.playlistPos;
    let currentFileId =
      this.state.playlist.length > 0
        ? this.state.playlist.find((file) => file.id === currentPlaylistPos)
            .fileId
        : 0;

    let currentFile = this.state.playlist[currentPlaylistPos];
    let fileTitle = currentFile.title;
    let url = currentFile.src;

    let isImage = false;
    let isVideo = false;
    let canOpen = true;

    var ext = this.getFileExtension(fileTitle)
      ? this.getFileExtension(fileTitle)
      : this.getFileExtension(url);

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

    if (this.mapSupplied[ext])
      if (!isImage && this.mapSupplied[ext].convertable && !url.includes("#")) {
        url += (url.includes("?") ? "&" : "?") + "convpreview=true";
      }

    return (
      <StyledMediaViewer visible={this.state.visible}>
        <div className="videoViewerOverlay"></div>
        {!isImage && (
          <>
            <MediaScrollButton
              orientation="right"
              onClick={this.prevMedia}
              inactive={this.state.playlist.length <= 1}
            />
            <MediaScrollButton
              orientation="left"
              onClick={this.nextMedia}
              inactive={this.state.playlist.length <= 1}
            />
          </>
        )}

        <div>
          <div className="details" ref={this.detailsContainer}>
            <Text isBold fontSize="14px" color="#fff" className="title">
              {fileTitle}
            </Text>
            <ControlBtn
              onClick={this.props.onClose && this.props.onClose}
              className="mediaPlayerClose"
            >
              <Icons.CrossIcon size="medium" isfill={true} color="#fff" />
            </ControlBtn>
          </div>
        </div>
        {canOpen &&
          (isImage ? (
            <ImageViewer
              visible={this.state.visible}
              onClose={this.onClose}
              images={[{ src: url, alt: "" }]}
              inactive={this.state.playlist.length <= 1}
              onNextClick={this.nextMedia}
              onPrevClick={this.prevMedia}
              onDeleteClick={this.onDelete}
              onDownloadClick={this.onDownload}
            />
          ) : (
            <StyledVideoViewer
              url={url}
              playing={this.state.visible}
              isVideo={isVideo}
              getOffset={this.getOffset}
            />
          ))}
        <div className="mediaViewerToolbox" ref={this.viewerToolbox}>
          {!isImage && (
            <span>
              {this.props.canDelete(currentFileId) && (
                <ControlBtn onClick={this.onDelete}>
                  <div className="deleteBtnContainer">
                    <Icons.MediaDeleteIcon size="scale" />
                  </div>
                </ControlBtn>
              )}
              {this.props.canDownload(currentFileId) && (
                <ControlBtn onClick={this.onDownload}>
                  <div className="downloadBtnContainer">
                    <Icons.MediaDownloadIcon size="scale" />
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
  currentFileId: PropTypes.number,
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
};

export default MediaViewer;
