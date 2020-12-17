import React, { Component } from "react";
import PropTypes from "prop-types";

import { findDOMNode } from "react-dom";
import screenfull from "screenfull";

import ReactPlayer from "react-player";
import Duration from "./duration";
import Progress from "./progress";

import styled from "styled-components";
import { Icons } from "asc-web-components";

const controlsHeight = 40;
const StyledControls = styled.div`
  height: ${(props) => props.height}px;
  display: block;
  position: fixed;
  z-index: 4001;
  ${(props) => !props.isVideo && "background-color: rgba(11,11,11,0.7);"}
  top: calc(50% + ${(props) => props.top}px);
  left: ${(props) => props.left}px;
`;
const StyledVideoControlBtn = styled.div`
  display: inline-block;
  height: 30px;
  line-height: 30px;
  margin: 5px;
  width: 40px;
  border-radius: 2px;
  cursor: pointer;
  text-align: center;
  vertical-align: top;
  &:hover {
    background-color: rgba(200, 200, 200, 0.2);
  }

  .playBtnContainer {
    width: 23px;
    line-height: 0;
    margin: 3px auto;
  }
  .pauseBtnContainer {
    display: block;
    width: 19px;
    margin: 3px 10px;
    line-height: 19px;
  }
  .muteBtnContainer {
    display: block;
    width: 26px;
    margin: 3px 7px;
    line-height: 19px;
  }
  .fullscreenBtnContainer {
    display: block;
    width: 20px;
    margin: 3px 10px;
    line-height: 19px;
  }
`;

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
          <Icons.MediaPauseIcon size="scale" />
        </div>
      ) : (
        <div className="playBtnContainer">
          <Icons.MediaPlayIcon size="scale" />
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
        <Icons.MediaFullScreenIcon size="scale" />
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
  height: 30px;
  line-height: 30px;
  margin: 5px;
  width: 60px;
  text-align: center;
  vertical-align: top;
  border-radius: 2px;
  cursor: pointer;

  &:hover {
    background-color: rgba(200, 200, 200, 0.2);
  }
`;
const StyledVideoViewer = styled.div`
  color: #d1d1d1;

  .playerWrapper {
    display: ${(props) => (props.isVideo ? "block" : "none")};
    width: ${(props) => props.width}px;
    height: ${(props) => props.height}px;
    left: ${(props) => props.left}px;
    top: calc(50% - ${(props) => props.top / 2}px);
    z-index: 4001;
    position: fixed;
    padding-bottom: 40px;
    background-color: rgba(11, 11, 11, 0.7);

    video {
      z-index: 4000;
    }
  }
`;

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
              <Icons.MediaMuteOffIcon size="scale" />
            ) : (
              <Icons.MediaMuteIcon size="scale" />
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
    playing: this.props.playing,
    controls: false,
    light: false,
    volume: 0.3,
    muted: false,
    played: 0,
    loaded: 0,
    duration: 0,
    playbackRate: 1.0,
    loop: false,
  };

  componentDidUpdate(prevProps, prevState) {
    let newUrl = prevState.url;
    let newPlaying = prevState.playing;
    if (
      this.props.url !== prevProps.url ||
      this.props.playing !== prevProps.playing
    ) {
      if (this.props.url !== prevProps.url) {
        newUrl = this.props.url;
      }
      if (this.props.playing !== prevProps.playing) {
        newPlaying = this.props.playing;
      }
      this.setState({
        url: newUrl,
        playing: newPlaying,
      });
    }
  }

  handlePlayPause = () => {
    this.setState({ playing: !this.state.playing });
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
    } = this.state;
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

    return (
      <StyledVideoViewer
        isVideo={this.props.isVideo}
        width={width}
        height={height}
        left={left}
        top={height + controlsHeight}
      >
        <div>
          <div className="playerWrapper">
            <ReactPlayer
              ref={this.ref}
              className="react-player"
              width="100%"
              height="100%"
              url={url}
              pip={pip}
              playing={playing}
              controls={controls}
              light={light}
              loop={loop}
              playbackRate={playbackRate}
              volume={volume}
              muted={muted}
              onPlay={this.handlePlay}
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
  playing: PropTypes.bool,
  getOffset: PropTypes.func,
};

export default VideoViewer;
