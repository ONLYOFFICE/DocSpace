import React, { Component } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { findDOMNode } from "react-dom";
import screenfull from "screenfull";
import ReactPlayer from "react-player";

import Duration from "./duration";
import Progress from "./progress";
import MediaPauseIcon from "../../../../../public/images/media.pause.react.svg";
import MediaPlayIcon from "../../../../../public/images/media.play.react.svg";
import MediaFullScreenIcon from "../../../../../public/images/media.fullscreen.video.react.svg";
import MediaMuteIcon from "../../../../../public/images/media.mute.react.svg";
import MediaMuteOffIcon from "../../../../../public/images/media.muteoff.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";

const iconsStyles = css`
  path,
  stroke,
  rect {
    fill: ${(props) => props.theme.mediaViewer.videoViewer.fill};
  }
`;

const controlsHeight = 40;
const StyledControls = styled.div`
  height: ${(props) => props.height}px;
  display: block;
  position: fixed;
  z-index: 301;
  ${(props) =>
    !props.isVideo &&
    `background-color: ${props.theme.mediaViewer.videoViewer.backgroundColor};`}
  top: calc(50% + ${(props) => props.top}px);
  left: ${(props) => props.left}px;
`;

StyledControls.defaultProps = { theme: Base };
const StyledVideoControlBtn = styled.div`
  display: inline-block;
  height: 26px;
  line-height: 30px;
  margin: 5px 2px;
  width: 38px;
  border-radius: 2px;
  cursor: pointer;
  text-align: center;
  vertical-align: top;
  &:hover {
    background-color: ${(props) =>
      props.theme.mediaViewer.videoViewer.background};
  }

  .playBtnContainer {
    width: 16px;
    height: 16px;
    line-height: 0;
    margin: 5px auto;
  }
  .pauseBtnContainer {
    display: block;
    width: 16px;
    height: 16px;
    margin: 3px 10px;
    line-height: 19px;
  }
  .muteBtnContainer {
    display: block;
    width: 16px;
    height: 16px;
    margin: 3px 11px;
    line-height: 19px;
  }
  .fullscreenBtnContainer {
    display: block;
    width: 16px;
    height: 16px;
    margin: 3px 11px;
    line-height: 19px;
  }
`;

StyledVideoControlBtn.defaultProps = { theme: Base };
const StyledMediaPauseIcon = styled(MediaPauseIcon)`
  ${commonIconsStyles}
  ${iconsStyles}
`;
StyledMediaPauseIcon.defaultProps = { theme: Base };
const StyledMediaPlayIcon = styled(MediaPlayIcon)`
  ${commonIconsStyles}
  ${iconsStyles}
`;
StyledMediaPlayIcon.defaultProps = { theme: Base };
const StyledMediaFullScreenIcon = styled(MediaFullScreenIcon)`
  ${commonIconsStyles}
  ${iconsStyles}
`;
StyledMediaFullScreenIcon.defaultProps = { theme: Base };
const StyledMediaMuteIcon = styled(MediaMuteIcon)`
  ${commonIconsStyles}

  path:first-child {
    stroke: ${(props) => props.theme.mediaViewer.videoViewer.stroke};
  }

  path:last-child {
    fill: ${(props) => props.theme.mediaViewer.videoViewer.fill};
  }
`;
const StyledMediaMuteOffIcon = styled(MediaMuteOffIcon)`
  ${commonIconsStyles}

  path, rect {
    fill: ${(props) => props.theme.mediaViewer.videoViewer.fill};
  }
`;

StyledMediaMuteIcon.defaultProps = { theme: Base };
const VideoControlBtn = (props) => {
  return (
    <StyledVideoControlBtn {...props}>{props.children}</StyledVideoControlBtn>
  );
};
VideoControlBtn.propTypes = {
  children: PropTypes.any,
};
const Controls = (props) => {
  return <StyledControls {...props}>{props.children}</StyledControls>;
};
Controls.propTypes = {
  children: PropTypes.any,
};
const PlayBtn = (props) => {
  return (
    <VideoControlBtn onClick={props.onClick}>
      {props.playing ? (
        <div className="pauseBtnContainer">
          <StyledMediaPauseIcon size="scale" />
        </div>
      ) : (
        <div className="playBtnContainer">
          <StyledMediaPlayIcon size="scale" />
        </div>
      )}
    </VideoControlBtn>
  );
};
PlayBtn.propTypes = {
  playing: PropTypes.bool,
  onClick: PropTypes.func,
};
const FullScreenBtn = (props) => {
  return (
    <VideoControlBtn onClick={props.onClick}>
      <div className="fullscreenBtnContainer">
        <StyledMediaFullScreenIcon size="scale" />
      </div>
    </VideoControlBtn>
  );
};
FullScreenBtn.propTypes = {
  onClick: PropTypes.func,
};

const StyledValumeContainer = styled.div`
  display: inline-block;
  vertical-align: top;
  line-height: 39px;
  position: relative;

  .muteConteiner {
    display: none;
    position: absolute;
    width: 40px;
    height: 80px;
    border-radius: 5px;
    top: -76px;
    left: 5px;
    background: black;
  }
  &:hover {
    .muteConteiner {
      display: inline-block;
    }
  }
  .mute {
    display: inline-block;
    transform: rotate(-90deg);
    margin: 22px -14px;
  }
`;
const StyledDuration = styled.div`
  display: inline-block;
  height: 26px;
  line-height: 26px;
  margin: 5px;
  width: 60px;
  text-align: center;
  vertical-align: top;
  border-radius: 2px;
  cursor: pointer;

  &:hover {
    background-color: ${(props) =>
      props.theme.mediaViewer.videoViewer.background};
  }
`;

StyledValumeContainer.defaultProps = { theme: Base };
const StyledVideoViewer = styled.div`
  color: ${(props) => props.theme.mediaViewer.videoViewer.color};

  .playerWrapper {
    display: ${(props) => (props.isVideo ? "block" : "none")};
    width: ${(props) => props.width}px;
    height: ${(props) => props.height}px;
    left: ${(props) => props.left}px;
    top: calc(50% - ${(props) => props.top / 2}px);
    z-index: 301;
    position: fixed;
    padding-bottom: 40px;
    background-color: ${(props) =>
      props.theme.mediaViewer.videoViewer.backgroundColor};

    video {
      z-index: 300;
    }
  }
`;

StyledVideoViewer.defaultProps = { theme: Base };

const ErrorContainer = styled.div`
  z-index: 301;
  display: block;
  position: fixed;
  left: calc(50% - 110px);
  top: calc(50% - 40px);
  background-color: ${(props) =>
    props.theme.mediaViewer.videoViewer.backgroundColorError};
  color: ${(props) => props.theme.mediaViewer.videoViewer.colorError};
  border-radius: 10px;
  padding: 20px;
  text-align: center;
`;

ErrorContainer.defaultProps = { theme: Base };

class ValumeBtn extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <StyledValumeContainer>
        <div className="muteConteiner">
          <Progress
            className="mute"
            width={this.props.width}
            value={this.props.volume}
            onMouseDown={this.props.onMouseDown}
            handleSeekChange={this.props.onChange}
            onMouseUp={this.props.handleSeekMouseUp}
          />
        </div>

        <VideoControlBtn onClick={this.props.onChangeMute}>
          <div className="muteBtnContainer">
            {this.props.muted ? (
              <StyledMediaMuteOffIcon size="scale" />
            ) : (
              <StyledMediaMuteIcon size="scale" />
            )}
          </div>
        </VideoControlBtn>
      </StyledValumeContainer>
    );
  }
}
ValumeBtn.propTypes = {
  width: PropTypes.number,
  volume: PropTypes.number,
  muted: PropTypes.bool,
  onMouseDown: PropTypes.func,
  onChange: PropTypes.func,
  handleSeekMouseUp: PropTypes.func,
  onChangeMute: PropTypes.func,
};

class VideoViewer extends Component {
  state = {
    url: this.props.url,
    pip: false,
    playing: false,
    controls: false,
    light: false,
    volume: 0.3,
    muted: false,
    played: 0,
    loaded: 0,
    duration: 0,
    playbackRate: 1.0,
    loop: false,
    isNew: false,
    error: false,
    isLoaded: false,
  };

  componentDidMount() {
    document.addEventListener("keydown", this.onKeydown, false);
  }

  componentWillUnmount() {
    document.removeEventListener("keydown", this.onKeydown, false);
  }

  componentDidUpdate(prevProps) {
    if (this.props.url !== prevProps.url) {
      this.setState({
        url: this.props.url,
        isNew: true,
        error: false,
      });
    }
  }

  onKeydown = (e) => {
    if (e.keyCode === 32) this.handlePlayPause();
  };

  handlePlayPause = () => {
    this.setState({ playing: !this.state.playing, isNew: false });
  };

  handleStop = () => {
    this.setState({ url: null, playing: false });
  };

  handleVolumeChange = (e) => {
    this.setState({
      volume: parseFloat(e.target.value),
      muted: false,
    });
  };

  handleToggleMuted = () => {
    this.setState({ muted: !this.state.muted });
  };

  handlePlay = () => {
    this.setState({ playing: true });
  };

  handleEnablePIP = () => {
    this.setState({ pip: true });
  };

  handleDisablePIP = () => {
    this.setState({ pip: false });
  };

  handlePause = () => {
    this.setState({ playing: false });
  };

  handleSeekMouseDown = (e) => {
    this.setState({ seeking: true });
  };

  handleSeekChange = (e) => {
    this.setState({ played: parseFloat(e.target.value) });
  };

  handleSeekMouseUp = (e) => {
    console.log(!isNaN(parseFloat(e.target.value)), parseFloat(e.target.value));
    if (!isNaN(parseFloat(e.target.value))) {
      this.setState({ seeking: false });
      this.player.seekTo(parseFloat(e.target.value));
    }
  };

  handleProgress = (state) => {
    if (!this.state.seeking) {
      this.setState(state);
    }
  };

  handleEnded = () => {
    this.setState({ playing: this.state.loop });
  };

  handleDuration = (duration) => {
    this.setState({ duration });
  };

  handleClickFullscreen = () => {
    screenfull.request(findDOMNode(this.player));
  };

  ref = (player) => {
    this.player = player;
  };

  resizePlayer = (videoSize, screenSize) => {
    var ratio = videoSize.h / videoSize.w;

    if (videoSize.h > screenSize.h) {
      videoSize.h = screenSize.h;
      videoSize.w = videoSize.h / ratio;
    }
    if (videoSize.w > screenSize.w) {
      videoSize.w = screenSize.w;
      videoSize.h = videoSize.w * ratio;
    }

    return {
      width: videoSize.w,
      height: videoSize.h,
    };
  };

  onError = (e) => {
    console.log("onError", e);
    this.setState({ error: true });
  };

  onPlay = () => {
    this.setState({ playing: !this.state.isNew, isNew: false, isLoaded: true });
  };

  render() {
    const {
      url,
      playing,
      controls,
      light,
      volume,
      muted,
      loop,
      played,
      loaded,
      duration,
      playbackRate,
      pip,
      error,
      isLoaded,
    } = this.state;
    const { errorLabel } = this.props;

    const parentOffset = this.props.getOffset() || 0;
    var screenSize = {
      w: window.innerWidth,
      h: window.innerHeight,
    };
    screenSize.h -= parentOffset + controlsHeight;

    let width = screenSize.w;
    let height = screenSize.h;

    let centerAreaOx = screenSize.w / 2 + document.documentElement.scrollLeft;
    let centerAreaOy = screenSize.h / 2 + document.documentElement.scrollTop;

    let videoElement = document.getElementsByTagName("video")[0];
    if (videoElement) {
      width = this.props.isVideo
        ? videoElement.videoWidth || 480
        : screenSize.w - 150;
      height = this.props.isVideo ? videoElement.videoHeight || 270 : 0;

      let resize = this.resizePlayer(
        {
          w: width,
          h: height,
        },
        screenSize
      );
      width = resize.width;
      height = resize.height;
    }

    let left = this.props.isVideo
      ? centerAreaOx - width / 2
      : centerAreaOx - width / 2;

    const videoControlBtnWidth = 220;
    const audioControlBtnWidth = 170;
    let progressWidth = this.props.isVideo
      ? width - videoControlBtnWidth
      : width - audioControlBtnWidth;

    if (error) {
      return (
        <ErrorContainer>
          <p>{errorLabel}</p>
        </ErrorContainer>
      );
    }

    return (
      <StyledVideoViewer
        isVideo={this.props.isVideo}
        width={width}
        height={height}
        left={left}
        top={height + controlsHeight}
      >
        <div>
          <div className="playerWrapper" onClick={this.handlePlayPause}>
            <ReactPlayer
              ref={this.ref}
              className="react-player"
              style={{ opacity: isLoaded ? 1 : 0 }}
              width="100%"
              height="100%"
              url={url}
              pip={pip}
              playing={playing}
              playsinline={true}
              controls={controls}
              light={light}
              loop={loop}
              playbackRate={playbackRate}
              volume={volume}
              muted={muted}
              onPlay={this.onPlay}
              onEnablePIP={this.handleEnablePIP}
              onDisablePIP={this.handleDisablePIP}
              onPause={this.handlePause}
              onEnded={this.handleEnded}
              onError={this.onError}
              onProgress={this.handleProgress}
              onDuration={this.handleDuration}
            />
          </div>
          <Controls
            height={controlsHeight}
            left={left}
            top={height / 2 - controlsHeight / 2}
            isVideo={this.props.isVideo}
          >
            <PlayBtn onClick={this.handlePlayPause} playing={playing} />
            <Progress
              value={played}
              width={progressWidth}
              onMouseDown={this.handleSeekMouseDown}
              handleSeekChange={this.handleSeekChange}
              onMouseUp={this.handleSeekMouseUp}
              onTouchEnd={this.handleSeekMouseUp}
            />
            <StyledDuration>
              -<Duration seconds={duration * (1 - played)} />
            </StyledDuration>
            <ValumeBtn
              width={64}
              muted={muted}
              volume={muted ? 0 : volume}
              onChangeMute={this.handleToggleMuted}
              onChange={this.handleVolumeChange}
            />
            {this.props.isVideo && (
              <FullScreenBtn onClick={this.handleClickFullscreen} />
            )}
          </Controls>
        </div>
      </StyledVideoViewer>
    );
  }
}

VideoViewer.propTypes = {
  isVideo: PropTypes.bool,
  url: PropTypes.string,
  getOffset: PropTypes.func,
  errorLabel: PropTypes.string,
};

export default VideoViewer;
