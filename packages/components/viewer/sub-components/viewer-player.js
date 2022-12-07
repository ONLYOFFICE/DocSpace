import * as React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { tablet } from "@docspace/components/utils/device";

import IconPlay from "../../../../public/images/videoplayer.play.react.svg";
import IconStop from "../../../../public/images/videoplayer.stop.react.svg";

import IconVolumeMax from "../../../../public/images/media.volumemax.react.svg";
import IconVolumeMuted from "../../../../public/images/media.volumeoff.react.svg";
import IconVolumeMin from "../../../../public/images/media.volumemin.react.svg";

import IconFullScreen from "../../../../public/images/videoplayer.full.react.svg";
import IconExitFullScreen from "../../../../public/images/videoplayer.exit.react.svg";
import MediaContextMenu from "../../../../public/images/vertical-dots.react.svg";

import Icon1x from "../../../../public/images/media.viewer1x.react.svg";
import Icon05x from "../../../../public/images/media.viewer05x.react.svg";
import Icon15x from "../../../../public/images/media.viewer15x.react.svg";
import Icon2x from "../../../../public/images/media.viewer2x.react.svg";

import BigIconPlay from "../../../../public/images/media.bgplay.react.svg";
import { useSwipeable } from "react-swipeable";
import { isMatchWith } from "lodash";

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

  .mobile-details {
    background: linear-gradient(
      0deg,
      rgba(0, 0, 0, 0) 0%,
      rgba(0, 0, 0, 0.8) 100%
    );
  }
  .video-wrapper {
    position: fixed;
    z-index: 305;
    top: 0;
    ${isMobileOnly &&
    css`
      top: 0;
    `}
    bottom: 0;
    right: 0;
    left: 0;
    ${(props) =>
      props.isFullScreen ? "background: #000" : "background: transparent"};
  }

  .audio-container {
    width: 190px;
    height: 190px;
    display: flex;
    justify-content: center;
    align-items: center;
    background: rgba(0, 0, 0, 0.7);
    border-radius: 20px;
  }

  svg {
    path {
      fill: #fff;
    }
  }

  rect {
    stroke: #fff;
  }

  .video-backdrop {
    position: fixed;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    background-color: rgba(55, 55, 55, 0.6);
    height: 100%;
  }

  .dropdown-speed {
    position: relative;
    display: inline-block;
  }
  .dropdown-item {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 30px;
    width: 48px;
    &:hover {
      cursor: pointer;
      background: #222;
    }
  }

  .dropdown-content {
    display: flex;
    height: 120px;
    width: 48px;
    padding: 4px 0px;
    flex-direction: column;
    align-items: center;
    position: absolute;
    bottom: 48px;
    color: #fff;
    background: #333;
    text-align: center;
    border-radius: 7px 7px 0px 0px;
  }

  .bg-play {
    position: fixed;
    &:hover {
      cursor: pointer;
    }
  }

  input[type="range"] {
    -webkit-appearance: none;
    margin-right: 15px;
    width: 80%;
    height: 4px;
    background: rgba(255, 255, 255, 0.3);
    border-radius: 5px;
    background-image: linear-gradient(#fff, #fff);
    background-repeat: no-repeat;
    &:hover {
      cursor: pointer;
    }

    @media ${tablet} {
      width: 63%;
    }
  }
  input[type="range"]::-webkit-slider-thumb {
    -webkit-appearance: none;
    height: 4px;
    width: 4px;
    border-radius: 50%;
    background: #fff;
    opacity: 0;
  }

  .mobile-video-progress {
    display: flex;
    justify-content: center;
    align-items: center;
    position: fixed;
    right: 0;
    bottom: 77px;
    left: 0;
    padding: 0 28px;
    height: 30px;

    ${isMobileOnly &&
    css`
      bottom: 44px;
      padding: 0 16px;
    `}

    input[type="range"] {
      width: 100%;
      margin-right: 0px;
    }
  }
`;

const StyledVideoActions = styled.div`
  .actions-container {
    position: fixed;
    bottom: 13px;
    left: 0;
    right: 0;
    justify-content: space-between;
    padding-left: 29px;
    padding-right: 19px;
    display: flex;
    align-items: center;

    ${isMobileOnly &&
    css`
      bottom: 2px;
      padding: 0 11px;
    `}

    .controll-box {
      display: flex;
      align-items: center;
    }
  }

  .controller {
    display: flex;
    justify-content: center;
    align-items: center;
    min-width: 48px;
    height: 48px;
    &:hover {
      cursor: pointer;
    }
  }
`;

const StyledDuration = styled.div`
  padding-left: 10px;
  padding-right: 14px;
  width: 102px;
  color: #fff;
  user-select: none;
  font-size: 12px;
  font-weight: 700;
`;

const StyledVideoControls = styled.div`
  position: fixed;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 307;
  display: flex;
  height: 188px;
  background: linear-gradient(
    180deg,
    rgba(0, 0, 0, 0) 0%,
    rgba(0, 0, 0, 0.64) 48.44%,
    rgba(0, 0, 0, 0.89) 100%
  );
  .volume-container {
    display: flex;

    svg {
      &:hover {
        cursor: pointer;
      }
    }
  }

  ${isMobileOnly &&
  css`
    height: 80px;
    background: rgba(0, 0, 0, 0.8);
  `}

  .volume-wrapper {
    width: 100px;
    height: 28px;
    display: flex;
    align-items: center;
    padding-left: 15px;

    &:hover {
      input[type="range"]::-webkit-slider-thumb {
        height: 10px;
        width: 10px;
        opacity: 1 !important;
        border-radius: 50%;
      }
    }
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
  const {
    setIsFullScreen,
    videoRef,
    generateContextMenu,
    mobileDetails,
    onPrevClick,
    onNextClick,
    onMaskClick,
    displayUI,
    isAudio,
    audioIcon,
  } = props;

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
    speedState: 1,
    isOpenContext: false,
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

  const [state, dispatch] = React.useReducer(reducer, initialState);
  const [currentVolume, setCurrentVolume] = React.useState(100);
  const speedIcons = [<Icon05x />, <Icon1x />, <Icon15x />, <Icon2x />];
  const handlers = useSwipeable({
    onSwipedLeft: (e) => {
      if (e.event.path[0] === inputRef.current) return;
      onNextClick();
    },
    onSwipedRight: (e) => {
      if (e.event.path[0] === inputRef.current) return;
      onPrevClick();
    },
  });

  const footerHeight = 0;
  const titleHeight = 0;

  const togglePlay = (e) => {
    e.stopPropagation();
    dispatch(
      createAction(ACTION_TYPES.update, {
        isPlaying: !state.isPlaying,
      })
    );
  };

  const handleVolumeUpdate = (e) => {
    const volume = e.target.value / 100;

    videoRef.current.volume = volume;

    setCurrentVolume(e.target.value);

    dispatch(
      createAction(ACTION_TYPES.update, {
        isMuted: volume ? false : true,
        volume: e.target.value,
      })
    );
  };

  const toggleVolumeMute = () => {
    dispatch(
      createAction(ACTION_TYPES.update, {
        isMuted: !state.isMuted,
        volume: state.volume ? 0 : currentVolume,
      })
    );
  };

  const toggleContext = () => {
    dispatch(
      createAction(ACTION_TYPES.update, {
        isOpenContext: !state.isOpenContext,
        volumeSelection: false,
        speedSelection: false,
      })
    );
  };

  const toggleScreen = () => {
    handleFullScreen(!state.isFullScreen);
    setIsFullScreen(!state.isFullScreen);

    dispatch(
      createAction(ACTION_TYPES.update, {
        isFullScreen: !state.isFullScreen,
      })
    );
  };

  const toggleSpeedSelectionMenu = (e) => {
    dispatch(
      createAction(ACTION_TYPES.update, {
        speedSelection: !state.speedSelection,
        volumeSelection: false,
        isOpenContext: false,
      })
    );
  };

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
    const currentSpeeed = Number(speed.substring(1));
    videoRef.current.playbackRate = currentSpeeed;
  };

  const SpeedButtonComponent = () => {
    const speed = ["X0.5", "X1", "X1.5", "X2"];
    const items = speed.map((speed, index) => (
      <div
        className="dropdown-item"
        onClick={() => {
          dispatch(
            createAction(ACTION_TYPES.update, {
              speedSelection: false,
              speedState: index,
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
      ? (window.innerHeight - height - footerHeight) / 2
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

    if (isMobileOnly) {
      maxHeight = window.innerHeight;
    }

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

  const onExitFullScreen = () => {
    if (!document.fullscreenElement) {
      setIsFullScreen(false);
      dispatch(
        createAction(ACTION_TYPES.update, {
          isFullScreen: state.isFullScreen,
        })
      );
    }
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

  const handleOutsideClick = (e) => {
    dispatch(
      createAction(ACTION_TYPES.update, {
        volumeSelection: false,
        speedSelection: false,
        isOpenContext: false,
      })
    );
  };

  React.useEffect(() => {
    state.isMuted
      ? (videoRef.current.muted = true)
      : (videoRef.current.muted = false);
  }, [state.isMuted, videoRef.current]);

  React.useEffect(() => {
    document.addEventListener("fullscreenchange", onExitFullScreen, false);
    return () =>
      document.removeEventListener("fullscreenchange", onExitFullScreen, false);
  }, []);

  React.useEffect(() => {
    if (!inputRef.current) return;

    inputRef.current.style.backgroundSize =
      ((state.progress - inputRef.current.min) * 100) /
        (inputRef.current.max - inputRef.current.min) +
      "% 100%";
  }, [inputRef.current, state.progress]);

  React.useEffect(() => {
    if (!volumeRef.current) return;

    volumeRef.current.style.backgroundSize =
      ((state.volume - volumeRef.current.min) * 100) /
        (volumeRef.current.max - volumeRef.current.min) +
      "% 100%";
  }, [volumeRef.current, state.volume]);

  React.useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, [videoRef.current, state.isFullScreen]);

  React.useEffect(() => {
    state.isPlaying ? videoRef.current.play() : videoRef.current.pause();
  }, [state.isPlaying, videoRef.current]);

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

  const onClose = () => {
    onMaskClick();
  };

  React.useEffect(() => {
    videoRef.current.addEventListener("loadedmetadata", function (e) {
      loadVideo(videoRef.current);
    });
  }, [props.activeIndex]);

  let contextRight = 9;
  let contextBottom = 48;
  const contextMenu = generateContextMenu(
    state.isOpenContext,
    contextRight,
    contextBottom
  );

  let iconLeft = (window.innerWidth - iconWidth) / 2 + "px";
  let iconTop = (window.innerHeight - iconHeight) / 2 + "px";

  let imgStyle = {
    width: `${state.width}px`,
    height: `${state.height}px`,
    transform: `
translateX(${state.left !== null ? state.left + "px" : "auto"}) translateY(${
      state.top
    }px)`,
  };
  return (
    <StyledVideoPlayer
      id="video-playerId"
      isFullScreen={state.isFullScreen}
      {...handlers}
    >
      <div className="video-backdrop" style={{ zIndex: 300 }} />
      {isMobileOnly && mobileDetails}
      <div className="video-wrapper" onClick={onClose}>
        <video
          onClick={togglePlay}
          id="videoPlayer"
          ref={videoRef}
          src={props.video.src}
          style={imgStyle}
          onTimeUpdate={handleOnTimeUpdate}
        ></video>
        {!state.isPlaying && !isAudio && (
          <div
            className="bg-play"
            style={{ left: `${iconLeft}`, top: `${iconTop}` }}
          >
            <BigIconPlay onClick={togglePlay} />
          </div>
        )}
        {isAudio && (
          <div
            className="audio-container"
            style={{
              left: `${(window.innerWidth - 190) / 2}px`,
              top: `${(window.innerHeight - 190) / 2}px`,
              position: "fixed",
            }}
          >
            <img src={audioIcon} />
          </div>
        )}
      </div>

      {displayUI && (
        <StyledVideoControls>
          <div className="mobile-video-progress">
            <input
              ref={inputRef}
              type="range"
              min="0"
              max="100"
              value={state.progress}
              onChange={(e) => handleVideoProgress(e)}
            />
          </div>
          <StyledVideoActions>
            <div className="actions-container">
              <div className="controll-box">
                <div
                  className="controller volume-container video-play"
                  onClick={togglePlay}
                >
                  {!state.isPlaying ? <IconPlay /> : <IconStop />}
                </div>

                <StyledDuration>{state.duration}</StyledDuration>

                {!isMobileOnly && (
                  <div className="volume-container">
                    {state.isMuted ? (
                      <IconVolumeMuted onClick={toggleVolumeMute} />
                    ) : state.volume >= 50 ? (
                      <IconVolumeMax onClick={toggleVolumeMute} />
                    ) : (
                      <IconVolumeMin onClick={toggleVolumeMute} />
                    )}

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
                  </div>
                )}
              </div>
              <div className="controll-box">
                <div
                  className="controller dropdown-speed"
                  onClick={toggleSpeedSelectionMenu}
                >
                  {speedIcons[state.speedState]}
                  {state.speedSelection && (
                    <div className="dropdown-content">
                      {SpeedButtonComponent()}
                    </div>
                  )}
                </div>
                {!isAudio && (
                  <div
                    className="controller fullscreen-button"
                    onClick={toggleScreen}
                  >
                    {!state.isFullScreen ? (
                      <IconFullScreen />
                    ) : (
                      <IconExitFullScreen />
                    )}
                  </div>
                )}

                {!isMobileOnly && (
                  <div
                    className="controller"
                    onClick={toggleContext}
                    style={{ position: "relative" }}
                  >
                    <MediaContextMenu />
                    {contextMenu}
                  </div>
                )}
              </div>
            </div>
          </StyledVideoActions>
        </StyledVideoControls>
      )}
    </StyledVideoPlayer>
  );
}
