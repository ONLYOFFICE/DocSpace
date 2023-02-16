import * as React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly, isIOS } from "react-device-detect";
import { tablet } from "@docspace/components/utils/device";

import IconPlay from "PUBLIC_DIR/images/videoplayer.play.react.svg";
import IconStop from "PUBLIC_DIR/images/videoplayer.stop.react.svg";

import IconVolumeMax from "PUBLIC_DIR/images/media.volumemax.react.svg";
import IconVolumeMuted from "PUBLIC_DIR/images/media.volumeoff.react.svg";
import IconVolumeMin from "PUBLIC_DIR/images/media.volumemin.react.svg";

import IconFullScreen from "PUBLIC_DIR/images/videoplayer.full.react.svg";
import IconExitFullScreen from "PUBLIC_DIR/images/videoplayer.exit.react.svg";
import MediaContextMenu from "PUBLIC_DIR/images/vertical-dots.react.svg";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg";

import Icon1x from "PUBLIC_DIR/images/media.viewer1x.react.svg";
import Icon05x from "PUBLIC_DIR/images/media.viewer05x.react.svg";
import Icon15x from "PUBLIC_DIR/images/media.viewer15x.react.svg";
import Icon2x from "PUBLIC_DIR/images/media.viewer2x.react.svg";

import BigIconPlay from "PUBLIC_DIR/images/media.bgplay.react.svg";
import { useSwipeable } from "../../react-swipeable";
import { MediaError } from "./media-error";

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
  display: flex;
  justify-content: center;
  align-items: center;

  &:focus-visible,
  #videoPlayer:focus-visible {
    outline: none;
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
    ${(props) =>
      props.audio === "true" ? "margin-right: -10px" : "margin-right: 0px"};
    padding-right: 10px;
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

  .video-speed-toast {
    position: fixed;
    width: 72px;
    height: 56px;
    border-radius: 9px;

    display: flex;
    justify-content: center;
    align-items: center;

    svg {
      width: 46px;
      height: 46px;
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
    bottom: 79px;
    left: 0;
    padding: 0 28px;
    height: 30px;

    ${isMobileOnly &&
    css`
      bottom: 46px;
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
    padding-left: 20px;
    padding-right: 30px;
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

  .mobile-actions {
    padding: 2px 6px;
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

  .context-menu-icon {
    padding-left: 19px;
    padding-bottom: 3px;
    width: 18px;
    height: 20px;
  }
`;

const StyledDuration = styled.div`
  padding-left: 10px;
  padding-right: 0px;
  ${isMobileOnly &&
  css`
    padding-left: 0px;
  `}
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
    margin-left: 10px;

    .icon-play {
      margin-right: 12px;
    }
    .is-audio {
      margin-right: 1px;
    }
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
    width: 123px;
    height: 28px;
    display: flex;
    align-items: center;
    padding-left: 9px;

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
  if (isNaN(timestamp)) return "00:00";

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
    isVideo,
    audioIcon,
    playlist,
    playlistPos,
    setPanelVisible,
    onTouch,
    isOpenContextMenu,
    setGlobalTimer,
    globalTimer,
    videoControls,
    setIsOpenContextMenu,
    contextModel,
    onDownloadClick,
    errorTitle,
    setIsError,
  } = props;

  const localStorageVolume = localStorage.getItem("player-volume");
  const stateVolume =
    localStorageVolume !== null ? Number(localStorageVolume) : 100;

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
    duration: false,
    speedState: 1,
    isOpenContext: false,
    volume: stateVolume,
    size: "0%",
    opacity: 1,
    deltaY: 0,
    deltaX: 0,
    speedToastVisible: false,
    isControlTouch: false,
    isFirstTap: false,
    isSecondTap: false,
    isFirstStart: true,
    loadingError: false,
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
  const actionRef = React.useRef(null);
  const mobileProgressRef = React.useRef(null);

  const [state, dispatch] = React.useReducer(reducer, initialState);
  const [currentVolume, setCurrentVolume] = React.useState(stateVolume);
  const speedIcons = [<Icon05x />, <Icon1x />, <Icon15x />, <Icon2x />];
  const handlers = useSwipeable({
    onSwiping: (e) => {
      const [width, height, left, top] = getVideoPosition(videoRef.current);
      const opacity =
        state.deltaY < e.deltaY
          ? state.opacity - Math.abs(e.deltaX) / 500
          : state.opacity + Math.abs(e.deltaX) / 500;

      const direction =
        Math.abs(e.deltaX) > Math.abs(e.deltaY) ? "horizontal" : "vertical";

      const xSwipe =
        direction === "horizontal"
          ? (playlistPos === 0 && e.deltaX > 0) ||
            (playlistPos === playlist.length - 1 && e.deltaX < 0)
            ? 0
            : e.deltaX
          : 0;

      return dispatch(
        createAction(ACTION_TYPES.update, {
          left: xSwipe,
          top:
            direction === "vertical"
              ? e.deltaY >= 0
                ? top + e.deltaY
                : top
              : top,
          opacity: direction === "vertical" && e.deltaY > 0 ? opacity : 1,
          deltaY: direction === "vertical" ? (e.deltaY > 0 ? e.deltaY : 0) : 0,
          deltaX: xSwipe,
        })
      );
    },
    onSwipedLeft: (e) => {
      if (e.deltaX <= -100) onNextClick();
    },
    onTap: (e) => {
      const wrapper = document.querySelector("#wrapper");
      if (e.event.target === wrapper) {
        onTouch(e.event, true);
        return dispatch(
          createAction(ACTION_TYPES.update, {
            isFirstTap: true,
            isSecondTap: false,
          })
        );
      }
      if (displayUI && props.isPlay && state.isFirstTap) {
        props.setIsPlay(false);
        return dispatch(
          createAction(ACTION_TYPES.update, {
            isFirstTap: false,
            isPlaying: false,
            isSecondTap: true,
          })
        );
      }
      onTouch(e.event);
      dispatch(
        createAction(ACTION_TYPES.update, {
          isFirstTap: true,
          isSecondTap: false,
        })
      );
    },
    onSwipedRight: (e) => {
      if (e.deltaX >= 100) onPrevClick();
    },
    onSwipedDown: (e) => {
      if (e.deltaY > 70) onMaskClick();
    },
    onSwiped: (e) => {
      const [width, height, left, top] = getVideoPosition(videoRef.current);
      if (Math.abs(e.deltaX) < 100) {
        return dispatch(
          createAction(ACTION_TYPES.update, {
            left: 0,
            top: top,
            opacity: 1,
            deltaY: 0,
            deltaX: 0,
          })
        );
      }
    },
  });

  const footerHeight = 0;
  const titleHeight = 0;

  const togglePlay = (e) => {
    e.stopPropagation();
    if ((e.target === videoRef.current && isMobileOnly) || state.isSecondTap) {
      return dispatch(
        createAction(ACTION_TYPES.update, {
          isSecondTap: false,
        })
      );
    }

    if (isMobileOnly) {
      props.setIsPlay(!state.isPlaying);
    }
    if (state.isFirstStart) {
      props.setIsPlay(true);
    }
    dispatch(
      createAction(ACTION_TYPES.update, {
        isPlaying: !state.isPlaying,
        isFirstStart: false,
      })
    );
  };

  const handleVolumeUpdate = (e) => {
    const volume = e.target.value / 100;

    videoRef.current.volume = volume;

    setCurrentVolume(e.target.value);

    localStorage.setItem("player-volume", e.target.value);

    dispatch(
      createAction(ACTION_TYPES.update, {
        isMuted: volume ? false : true,
        volume: e.target.value,
      })
    );
  };

  const toggleVolumeMute = () => {
    let volume = null;

    if (!state.isMuted) {
      localStorage.setItem("player-volume", 0);
    }

    if (state.isMuted) {
      volume = currentVolume === 0 ? 100 : currentVolume;
      videoRef.current.volume = volume / 100;
      localStorage.setItem("player-volume", volume);
    }

    dispatch(
      createAction(ACTION_TYPES.update, {
        isMuted: !state.isMuted,
        volume: state.volume ? 0 : volume,
      })
    );
  };

  const toggleContext = (e) => {
    setIsOpenContextMenu(!state.isOpenContext);
    dispatch(
      createAction(ACTION_TYPES.update, {
        isOpenContext: !state.isOpenContext,
        speedSelection: false,
      })
    );
  };

  const toggleScreen = () => {
    !isIOS && handleFullScreen(!state.isFullScreen);
    setIsFullScreen(!state.isFullScreen);

    dispatch(
      createAction(ACTION_TYPES.update, {
        isFullScreen: !state.isFullScreen,
      })
    );
  };

  const toggleSpeedSelectionMenu = (e) => {
    if (isMobileOnly) {
      let timer;
      const speed = ["X0.5", "X1", "X1.5", "X2"];

      const currentSpeed =
        state.speedState === 1
          ? 0
          : state.speedState === 0
          ? state.speedState + 2
          : state.speedState === 3
          ? 1
          : state.speedState + 1;

      dispatch(
        createAction(ACTION_TYPES.update, {
          speedState: currentSpeed,
          speedToastVisible: true,
        })
      );
      clearTimeout(timer);
      setTimeout(
        () =>
          dispatch(
            createAction(ACTION_TYPES.update, {
              speedToastVisible: false,
            })
          ),
        2000
      );
      const videoSpeed = Number(speed[currentSpeed].substring(1));
      return (videoRef.current.playbackRate = videoSpeed);
    }
    setIsOpenContextMenu(!state.speedSelection);
    dispatch(
      createAction(ACTION_TYPES.update, {
        speedSelection: !state.speedSelection,
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
        key={index}
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
    let top =
      !state.isFullScreen || isMobileOnly
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
        isPlaying: progress === 100 ? false : state.isPlaying,
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
    if (state.isOpenContext) {
      return dispatch(
        createAction(ACTION_TYPES.update, {
          isOpenContext: false,
        })
      );
    }
    if (state.speedSelection) {
      return dispatch(
        createAction(ACTION_TYPES.update, {
          speedSelection: false,
        })
      );
    }
  };

  React.useEffect(() => {
    if (!state.speedSelection && !state.isOpenContext) {
      setIsOpenContextMenu(false);
    }
  }, [state.isOpenContext, state.speedSelection]);

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

  React.useEffect(() => {
    if (videoRef && videoRef.current) {
      videoRef.current.addEventListener("error", (event) => {
        setIsError(true);
        return dispatch(
          createAction(ACTION_TYPES.update, {
            loadingError: true,
          })
        );
      });
    }
  }, [videoRef.current]);

  function loadVideo(video) {
    const currentTime = getDuration(video.currentTime);
    const duration = getDuration(video.duration);

    const lasting = `${currentTime} / ${duration}`;

    setPanelVisible(true);

    localStorage.setItem("displayVisible", false);
    props.setIsPlay(false);

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
        isFullScreen: state.isFullScreen,
        speedSelection: false,
        isOpenContext: false,
        opacity: 1,
        deltaY: 0,
        deltaX: 0,
        speedState: 1,
        speedToastVisible: false,
        isControlTouch: false,
        isFirstTap: false,
        isSecondTap: false,
        isFirstStart: true,
        loadingError: false,
      })
    );
  }

  const onClose = () => {
    if (isMobileOnly) return;
    onMaskClick();
  };

  React.useEffect(() => {
    videoRef.current.addEventListener("loadedmetadata", function (e) {
      loadVideo(videoRef.current);
    });
  }, [props.activeIndex]);

  React.useEffect(() => {
    if (!isMobileOnly) return;

    if (isOpenContextMenu || state.isControlTouch || !props.isPlay) {
      dispatch(
        createAction(ACTION_TYPES.update, {
          isControlTouch: false,
        })
      );
      return clearTimeout(globalTimer);
    }
    if (isMobileOnly && videoRef.current && displayUI) {
      clearTimeout(globalTimer);
      setGlobalTimer(setTimeout(() => setPanelVisible(false), 2500));
    }
  }, [displayUI, isOpenContextMenu, state.isControlTouch, props.isPlay]);

  const onControlTouch = () => {
    dispatch(
      createAction(ACTION_TYPES.update, {
        isControlTouch: true,
      })
    );
  };

  React.useEffect(() => {
    if (videoControls && videoControls.current) {
      videoControls.current.addEventListener("touchstart", onControlTouch);
    }

    return () =>
      videoControls &&
      videoControls.current &&
      videoControls.current.removeEventListener("touchstart", onControlTouch);
  }, [videoControls.current]);

  let contextRight = 9;
  let contextBottom = 48;
  const contextMenu = generateContextMenu(
    state.isOpenContext,
    contextRight,
    contextBottom
  );

  const model = contextModel();
  const hideContextMenu = model.filter((item) => !item.disabled).length <= 1;

  let iconLeft =
    state.deltaX && isMobileOnly
      ? (window.innerWidth - iconWidth) / 2 + state.left + "px"
      : (window.innerWidth - iconWidth) / 2 + "px";
  let iconTop =
    (window.innerHeight - iconHeight) / 2 - 13 + state.deltaY + "px";

  let imgStyle = {
    opacity: `${state.opacity}`,
    width: `${state.width}px`,
    height: `${state.height}px`,
    top: 0,
    position: "fixed",
    transform: `
translateX(${state.left !== null ? state.left + "px" : "auto"}) translateY(${
      state.top
    }px)`,
  };

  return (
    <StyledVideoPlayer
      id="video-playerId"
      isFullScreen={state.isFullScreen}
      audio={isAudio.toString()}
      onClick={handleOutsideClick}
    >
      <div className="video-backdrop" style={{ zIndex: 300 }} />
      {isMobileOnly && displayUI && mobileDetails}
      <div className="video-wrapper" onClick={onClose}>
        <div
          id="wrapper"
          style={{
            position: "fixed",
            left: 0,
            right: 0,
            bottom: "80px",
            top: "53px",
          }}
          {...handlers}
        >
          <video
            onClick={togglePlay}
            id="videoPlayer"
            ref={videoRef}
            onLoadStart={() => {
              const volumeNow =
                localStorageVolume !== null
                  ? Number(localStorage.getItem("player-volume") / 100)
                  : 1;
              videoRef.current.volume = volumeNow;
              dispatch(
                createAction(ACTION_TYPES.update, {
                  isMuted: volumeNow === 0,
                })
              );
            }}
            src={`${props.video.src}#t=0.001`}
            style={imgStyle}
            onTimeUpdate={handleOnTimeUpdate}
            playsInline
          ></video>
        </div>
        {!state.isPlaying && displayUI && !isAudio && !state.loadingError && (
          <div
            className="bg-play"
            style={{
              left: `${iconLeft}`,
              top: `${iconTop}`,
              opacity: `${state.opacity}`,
            }}
          >
            <BigIconPlay onClick={togglePlay} />
          </div>
        )}
        {state.speedToastVisible && (
          <div
            className="video-speed-toast"
            style={{
              left: `${iconLeft}`,
              top: `${iconTop}`,
              background: `rgba(51, 51, 51, 0.65)`,
            }}
          >
            {speedIcons[state.speedState]}
          </div>
        )}

        {isAudio && (
          <div
            className="audio-container"
            style={{
              left: `${(window.innerWidth - 190) / 2 + state.deltaX}px`,
              top: `${(window.innerHeight - 190) / 2 + state.deltaY}px`,
              position: "fixed",
              opacity: `${state.opacity}`,
              zIndex: "-1",
            }}
          >
            <img src={audioIcon} />
          </div>
        )}
      </div>

      {!state.loadingError ? (
        <StyledVideoControls
          ref={videoControls}
          style={{ opacity: `${displayUI ? "1" : "0"}` }}
        >
          <div className="mobile-video-progress" ref={mobileProgressRef}>
            <input
              ref={inputRef}
              type="range"
              min="0"
              max="100"
              value={state.progress ? state.progress : "0"}
              onChange={(e) => handleVideoProgress(e)}
            />
          </div>
          <StyledVideoActions>
            <div
              className={
                isMobileOnly
                  ? "actions-container mobile-actions"
                  : "actions-container"
              }
              ref={actionRef}
            >
              <div className="controll-box">
                <div className="controller video-play" onClick={togglePlay}>
                  {!state.isPlaying ? (
                    <IconPlay
                      className={isAudio ? "icon-play is-audio" : "icon-play"}
                    />
                  ) : (
                    <IconStop className="icon-stop" />
                  )}
                </div>

                <StyledDuration>
                  {state.duration && state.duration}
                </StyledDuration>

                {!isMobileOnly && (
                  <div
                    className={
                      isAudio
                        ? "volume-container volume-container-audio"
                        : "volume-container"
                    }
                  >
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

                {!isMobileOnly && !props.isPreviewFile && !hideContextMenu && (
                  <div
                    className="controller context-menu-wrapper"
                    onClick={toggleContext}
                    style={{ position: "relative" }}
                  >
                    <MediaContextMenu className="context-menu-icon" />
                    {contextMenu}
                  </div>
                )}
                {hideContextMenu && (
                  <div className="controller" onClick={onDownloadClick}>
                    <DownloadReactSvgUrl />
                  </div>
                )}
              </div>
            </div>
          </StyledVideoActions>
        </StyledVideoControls>
      ) : (
        <MediaError
          width={267}
          height={56}
          onMaskClick={onMaskClick}
          model={model}
          errorTitle={errorTitle}
        />
      )}
    </StyledVideoPlayer>
  );
}
