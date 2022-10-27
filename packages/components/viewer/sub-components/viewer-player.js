import * as React from "react";
import styled from "styled-components";
import ContextMenu from "@docspace/components/context-menu";
import { isMobile } from "react-device-detect";

import { tablet } from "@docspace/components/utils/device";

import IconPlay from "../../../../public/images/videoplayer.play.react.svg";
import IconStop from "../../../../public/images/videoplayer.stop.react.svg";

import IconSound from "../../../../public/images/videoplayer.sound.react.svg";
import IconMuted from "../../../../public/images/videoplayer.mute.react.svg";

import IconFullScreen from "../../../../public/images/videoplayer.full.react.svg";
import IconExitFullScreen from "../../../../public/images/videoplayer.exit.react.svg";
import IconSpeed from "../../../../public/images/videoplayer.speed.react.svg";
import MediaContextMenu from "../../../../public/images/vertical-dots.react.svg";

import BigIconPlay from "../../../../public/images/videoplayer.bgplay.react.svg";

let iconWidth = 80;
let iconHeight = 60;

const ACTION_TYPES = {
  setActiveIndex: "setActiveIndex",
  update: "update",
};

function createAction(type, payload) {
  return {
    type,
    payload: payload || {},
  };
}

const StyledVideoPlayer = styled.div`
  &:focus-visible,
  #videoPlayer:focus-visible {
    outline: none;
  }
  .video-wrapper {
    position: fixed;
    z-index: 305;
    top: 0;
    bottom: 0;
    right: 0;
    left: 0;
    ${(props) =>
      props.isFullScreen ? "background: #000" : "background: transparent"};
  }

  .dropdown-speed {
    position: relative;
    display: inline-block;
  }
  .dropdown-item {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 3px 3px 0px 0px;
    height: 30px;
    width: 40px;
    &:hover {
      cursor: pointer;
      background: #222;
    }
  }

  .dropdown-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    position: absolute;
    bottom: 52px;
    color: #fff;
    background: #000;
    text-align: center;
    border-radius: 3px 3px 0px 0px;
  }

  .bg-play {
    position: fixed;
    &:hover {
      cursor: pointer;
    }
  }
`;

const StyledVideoActions = styled.div`
  display: flex;
  justify-content: start;
  align-items: center;

  .controller {
    display: flex;
    justify-content: center;
    align-items: center;
    min-width: 48px;
    height: 48px;
    &:hover {
      cursor: pointer;
      background: rgb(77, 77, 77);
    }
  }
`;

const StyledVideoControls = styled.div`
  position: fixed;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 307;
  height: 48px;
  background: rgba(17, 17, 17, 0.867);

  input[type="range"] {
    -webkit-appearance: none;
    margin-right: 15px;
    width: 80%;
    height: 8px;
    background: #4d4d4d;
    border: 1px solid rgba(0, 0, 0, 0.4);
    border-radius: 5px;
    background-image: linear-gradient(#d1d1d1, #d1d1d1);
    background-repeat: no-repeat;

    @media ${tablet} {
      width: 63%;
    }
  }
  input[type="range"]::-webkit-slider-thumb {
    -webkit-appearance: none;
    height: 14px;
    width: 14px;
    border-radius: 50%;
    background: #fff;
    border: 1px solid rgba(0, 0, 0);
    //  transition: background 0.3s ease-in-out;
  }

  .volume-container {
    position: relative;
  }

  .volume-wrapper {
    background: #000;
    position: absolute;
    bottom: 78px;
    padding: 9px;
    transform: rotate(270deg);
    //  left: -4px;
  }

  .volume-toolbar {
    width: 50px !important;
  }
`;

const getDuration = (time) => {
  const timestamp = Math.floor(time);

  const hours = Math.floor(timestamp / 60 / 60);
  const minutes = Math.floor(timestamp / 60) - hours * 60;
  const seconds = timestamp % 60;

  const formatted = hours
    ? [
        hours.toString().padStart(2, "0"),
        minutes.toString().padStart(2, "0"),
        seconds.toString().padStart(2, "0"),
      ].join(":")
    : [
        minutes.toString().padStart(2, "0"),
        seconds.toString().padStart(2, "0"),
      ].join(":");

  return formatted;
};

export default function ViewerPlayer(props) {
  const { setIsFullScreen, videoRef, generateContextMenu } = props;

  const initialState = {
    width: 0,
    height: 0,
    left: 0,
    top: 0,
    activeIndex: props.activeIndex,
    isPlaying: false,
    isMuted: false,
    isFullScreen: false,
    speedSelection: false,
    progress: 0,
    duration: 0,
    volumeSelection: false,
    volume: 100,
    size: "0%",
  };
  function reducer(state, action) {
    switch (action.type) {
      case ACTION_TYPES.setActiveIndex:
        return {
          ...state,
          activeIndex: action.payload.index,
          startLoading: true,
        };
      case ACTION_TYPES.update:
        return {
          ...state,
          ...action.payload,
        };
      default:
        break;
    }
    return state;
  }

  const inputRef = React.useRef(null);
  const volumeRef = React.useRef(null);
  const contextRef = React.useRef(null);
  const cm = React.useRef(null);

  const [state, dispatch] = React.useReducer(reducer, initialState);
  const [isOpen, setIsOpen] = React.useState(false);

  const footerHeight = 48;
  const titleHeight = 48;

  const togglePlay = () =>
    dispatch(
      createAction(ACTION_TYPES.update, {
        isPlaying: !state.isPlaying,
      })
    );
  const handleVolumeUpdate = (e) => {
    const volume = e.target.value / 100;

    videoRef.current.volume = volume;

    dispatch(
      createAction(ACTION_TYPES.update, {
        isMuted: volume ? false : true,
        volume: e.target.value,
      })
    );
  };

  const toggleVolumeSelection = () =>
    dispatch(
      createAction(ACTION_TYPES.update, {
        volumeSelection: !state.volumeSelection,
      })
    );

  const toggleScreen = () => {
    handleFullScreen(!state.isFullScreen);
    setIsFullScreen(!state.isFullScreen);

    dispatch(
      createAction(ACTION_TYPES.update, {
        isFullScreen: !state.isFullScreen,
      })
    );
  };

  const toggleSpeedSelectionMenu = () =>
    dispatch(
      createAction(ACTION_TYPES.update, {
        speedSelection: !state.speedSelection,
      })
    );

  const elem = document.documentElement;

  const handleFullScreen = (isFull) => {
    if (elem.requestFullscreen && isFull) return elem.requestFullscreen();
    return document.exitFullscreen();
  };

  const handleVideoProgress = (e) => {
    const manualChange = Number(e.target.value);
    videoRef.current.currentTime =
      (videoRef.current.duration / 100) * manualChange;
    dispatch(
      createAction(ACTION_TYPES.update, {
        progress: manualChange,
      })
    );
  };

  const handleVideoSpeed = (speed) => {
    const currentSpeeed = Number(speed);
    videoRef.current.playbackRate = currentSpeeed;
  };

  const SpeedButtonComponent = () => {
    const speed = ["0.5", "1", "1.5", "2"];
    const items = speed.map((speed) => (
      <div
        className="dropdown-item"
        onClick={() => {
          dispatch(
            createAction(ACTION_TYPES.update, {
              speedSelection: false,
            })
          );
          return handleVideoSpeed(speed);
        }}
      >
        {speed}
      </div>
    ));
    return items;
  };

  const getVideoPosition = (video) => {
    const [width, height] = getVideoWidthHeight(video);

    let left = (window.innerWidth - width) / 2;
    let top = !state.isFullScreen
      ? (window.innerHeight - height - (footerHeight - 48)) / 2
      : 0;

    return [width, height, left, top];
  };

  const getVideoWidthHeight = (video) => {
    let width = 0;
    let height = 0;

    let maxWidth = window.innerWidth;
    let maxHeight = !state.isFullScreen
      ? window.innerHeight - (footerHeight + titleHeight)
      : window.innerHeight - footerHeight;

    width =
      video.videoWidth > maxWidth
        ? maxWidth
        : state.isFullScreen
        ? Math.max(maxWidth, video.videoWidth)
        : Math.min(maxWidth, video.videoWidth);

    height = (width / video.videoWidth) * video.videoHeight;

    if (height > maxHeight) {
      height = maxHeight;
      width = (height / video.videoHeight) * video.videoWidth;
    }

    return [width, height];
  };

  const handleOnTimeUpdate = () => {
    const progress =
      (videoRef.current.currentTime / videoRef.current.duration) * 100;

    const currentTime = getDuration(videoRef.current.currentTime);
    const duration = getDuration(videoRef.current.duration);

    const lasting = `${currentTime} / ${duration}`;

    dispatch(
      createAction(ACTION_TYPES.update, {
        duration: lasting,
        progress: progress,
      })
    );
  };

  const handleResize = () => {
    let video = videoRef.current;
    const [width, height, left, top] = getVideoPosition(video);

    dispatch(
      createAction(ACTION_TYPES.update, {
        width: width,
        height: height,
        left: left,
        top: top,
      })
    );
  };

  React.useEffect(() => {
    state.isMuted
      ? (videoRef.current.muted = true)
      : (videoRef.current.muted = false);
  }, [state.isMuted, videoRef.current]);

  React.useEffect(() => {
    inputRef.current.style.backgroundSize =
      ((state.progress - inputRef.current.min) * 100) /
        (inputRef.current.max - inputRef.current.min) +
      "% 100%";
  }, [inputRef.current, state.progress]);

  React.useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, [videoRef.current, state.isFullScreen]);

  React.useEffect(() => {
    state.isPlaying ? videoRef.current.play() : videoRef.current.pause();
  }, [state.isPlaying, videoRef.current]);

  const onContextMenu = (e) => {
    cm.current.show(e);
  };

  function loadVideo(video) {
    const currentTime = getDuration(video.currentTime);
    const duration = getDuration(video.duration);

    const lasting = `${currentTime} / ${duration}`;

    const [width, height, left, top] = getVideoPosition(video);
    dispatch(
      createAction(ACTION_TYPES.update, {
        width: width,
        height: height,
        left: left,
        top: top,
        duration: lasting,
        progress: 0,
        isPlaying: false,
        isMuted: false,
        isFullScreen: state.isFullScreen,
        speedSelection: false,
        volume: state.volume,
      })
    );
  }

  React.useEffect(() => {
    videoRef.current.addEventListener("loadedmetadata", function (e) {
      loadVideo(videoRef.current);
    });
  }, [props.activeIndex]);

  const contextMenu = generateContextMenu(isOpen);

  let iconLeft = (window.innerWidth - iconWidth) / 2 + "px";
  let iconTop = (window.innerHeight - iconHeight - (48 - 48)) / 2 + "px";

  let imgStyle = {
    width: `${state.width}px`,
    height: `${state.height}px`,
    transition: "all .26s ease-out",
    transform: `
translateX(${state.left !== null ? state.left + "px" : "auto"}) translateY(${
      state.top
    }px)`,
  };
  return (
    <StyledVideoPlayer id="video-playerId" isFullScreen={state.isFullScreen}>
      <div className="video-wrapper">
        <video
          onClick={togglePlay}
          id="videoPlayer"
          ref={videoRef}
          src={props.video.src}
          style={imgStyle}
          onTimeUpdate={handleOnTimeUpdate}
        ></video>
        {!state.isPlaying && (
          <div
            className="bg-play"
            style={{ left: `${iconLeft}`, top: `${iconTop}` }}
          >
            <BigIconPlay onClick={togglePlay} />
          </div>
        )}
      </div>
      <StyledVideoControls>
        <StyledVideoActions>
          <div className="controller volume-container" onClick={togglePlay}>
            {!state.isPlaying ? <IconPlay /> : <IconStop />}
          </div>
          <input
            ref={inputRef}
            type="range"
            min="0"
            max="100"
            value={state.progress}
            onChange={(e) => handleVideoProgress(e)}
          />

          <div
            style={{
              paddingLeft: "10px",
              paddingRight: "14px",
              width: "102px",
              color: "#DDDDDD",
            }}
          >
            {state.duration}
          </div>
          <div
            className="controller volume-container"
            onClick={toggleVolumeSelection}
          >
            {!state.isMuted ? <IconSound /> : <IconMuted />}
            {state.volumeSelection && (
              <div className="volume-wrapper">
                <input
                  ref={volumeRef}
                  className="volume-toolbar"
                  type="range"
                  min="0"
                  max="100"
                  value={state.volume}
                  onChange={(e) => handleVolumeUpdate(e)}
                />
              </div>
            )}
          </div>
          <div className="controller" onClick={toggleScreen}>
            {!state.isFullScreen ? <IconFullScreen /> : <IconExitFullScreen />}
          </div>
          <div
            className="controller dropdown-speed"
            onClick={toggleSpeedSelectionMenu}
          >
            <IconSpeed />
            {state.speedSelection && (
              <div className="dropdown-content">{SpeedButtonComponent()}</div>
            )}
          </div>
          <div
            className="controller"
            onClick={() => setIsOpen((open) => !open)}
            style={{ position: "relative" }}
          >
            <MediaContextMenu />
            {contextMenu}
          </div>
        </StyledVideoActions>
      </StyledVideoControls>
    </StyledVideoPlayer>
  );
}
