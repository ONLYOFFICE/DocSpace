import * as React from "react";
import styled from "styled-components";

import IconPlay from "../../../../public/images/videoplayer.play.react.svg";
import IconStop from "../../../../public/images/videoplayer.stop.react.svg";

import IconSound from "../../../../public/images/videoplayer.sound.react.svg";
import IconMuted from "../../../../public/images/videoplayer.mute.react.svg";

import IconFullScreen from "../../../../public/images/videoplayer.full.react.svg";
import IconExitFullScreen from "../../../../public/images/videoplayer.exit.react.svg";

import BigIconPlay from "../../../../public/images/videoplayer.bgplay.react.svg";

import Slider from "@docspace/components/slider";

let iconWidth = 80;
let iconHeight = 60;

const StyledVideoPlayer = styled.div`
  ${(props) =>
    props.isFullScreen ? "background: #000" : "background: transparent"};

  .video-wrapper {
    position: fixed;
    z-index: 1005;
    top: 0;
    bottom: 0;
    right: 0;
    left: 0;
  }

  .bg-play {
    position: fixed;
    left: ${(window.innerWidth - iconWidth) / 2 + "px"};
    top: ${(window.innerHeight - iconHeight - (48 - 48)) / 2 + "px"};
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
    width: 48px;
    height: 48px;
    &:hover {
      cursor: pointer;
    }
  }
`;

const StyledVideoControls = styled.div`
  position: fixed;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 1500;
  height: 48px;
  background: rgba(17, 17, 17, 0.867);

  input[type="range"] {
    //-webkit-appearance: none !important;
    background: #4d4d4d;
    border: 1px solid rgba(0, 0, 0, 0.4);
    border-radius: 2px;
    transition: all 0.26s ease-out;
    height: 8px;
    width: 70%;
  }

  input[type="range"]::-webkit-slider-thumb {
    -webkit-appearance: none !important;
    cursor: pointer;
    // height: 6px;
    border: 1px solid rgba(0, 0, 0, 0.4);
  }

  /* input[type="range"]::-moz-range-progress {
    background: white;
  } */
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
  const { setOverlay } = props;

  const footerHeight = 48;
  const titleHeight = 48;
  const [style, setStyle] = React.useState(null);

  const [isPlaying, setIsPlaying] = React.useState(false);
  const [progress, setProgress] = React.useState(0);

  const [duration, setDuration] = React.useState(0);
  const [isMuted, setIsMuted] = React.useState(false);
  const [isFullScreen, setIsFullScreen] = React.useState(false);

  const [width, setWidth] = React.useState(0);
  const [height, setHeight] = React.useState(0);
  const [left, setLeft] = React.useState(0);
  const [top, setTop] = React.useState(0);

  const togglePlay = () => setIsPlaying((playing) => !playing);
  const toggleMute = () => setIsMuted((muted) => !muted);
  const toggleScreen = () => setIsFullScreen((screen) => !screen);

  const elem = document.documentElement;

  function openFullscreen() {
    if (elem.requestFullscreen) {
      elem.requestFullscreen();
    }
  }

  function closeFullscreen() {
    if (document.exitFullscreen) {
      document.exitFullscreen();
    }
  }

  const handleVideoProgress = (e) => {
    const manualChange = Number(e.target.value);
    props.forwardedRef.current.currentTime =
      (props.forwardedRef.current.duration / 100) * manualChange;
    setProgress(manualChange);
  };

  const getCurrentStyle = (video) => {
    const [width, height] = getVideoWidthHeight(video);
    let left = (window.innerWidth - width) / 2;
    let top = (window.innerHeight - height - (footerHeight - 48)) / 2;

    let imgStyle = {
      width: `${width}px`,
      height: `${height}px`,
      transition: "all .26s ease-out",
      transform: `translateX(${
        left !== null ? left + "px" : "auto"
      }) translateY(${top}px)`,
    };

    return imgStyle;
  };

  const getVideoWidthHeight = (video) => {
    let width = 0;
    let height = 0;

    let maxWidth = window.innerWidth;
    let maxHeight = window.innerHeight - (footerHeight + titleHeight);

    width = Math.min(maxWidth, video.videoWidth);
    height = (width / video.videoWidth) * video.videoHeight;

    if (height > maxHeight) {
      height = maxHeight;
      width = (height / video.videoHeight) * video.videoWidth;
    }

    const videoplayer = document.getElementById("video-playerId");

    if (isFullScreen) {
      height = videoplayer.offsetHeight;
      width = window.innerWidth;
    }

    return [width, height];
  };

  const handleOnTimeUpdate = () => {
    const progress =
      (props.forwardedRef.current.currentTime /
        props.forwardedRef.current.duration) *
      100;
    setProgress(progress);

    const currentTime = getDuration(props.forwardedRef.current.currentTime);
    const duration = getDuration(props.forwardedRef.current.duration);

    const lasting = `${currentTime} / ${duration}`;

    setDuration(lasting);
  };

  React.useEffect(() => {
    // let video = props.forwardedRef.current;
    setOverlay((overlay) => !overlay);

    if (isFullScreen) {
      openFullscreen();
    } else {
      closeFullscreen();
    }
  }, [isFullScreen]);

  const handleResize = () => {
    let video = props.forwardedRef.current;

    const imgStyle = getCurrentStyle(video);
    setStyle(imgStyle);
  };

  React.useEffect(() => {
    isMuted
      ? (props.forwardedRef.current.muted = true)
      : (props.forwardedRef.current.muted = false);
  }, [isMuted, props.forwardedRef.current]);

  React.useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, [props.forwardedRef.current]);

  React.useEffect(() => {
    isPlaying
      ? props.forwardedRef.current.play()
      : props.forwardedRef.current.pause();
  }, [isPlaying, props.forwardedRef.current]);

  function loadVideo(video) {
    const currentTime = getDuration(props.forwardedRef.current.currentTime);
    const duration = getDuration(props.forwardedRef.current.duration);

    const lasting = `${currentTime} / ${duration}`;
    setDuration(lasting);

    const imgStyle = getCurrentStyle(video);
    setStyle(imgStyle);
  }

  React.useEffect(() => {
    props.forwardedRef.current.addEventListener("loadedmetadata", function (e) {
      loadVideo(props.forwardedRef.current);
    });
  }, []);

  return (
    <StyledVideoPlayer id="video-playerId" isFullScreen={isFullScreen}>
      <div className="video-wrapper">
        <video
          onClick={togglePlay}
          id="videoPlayer"
          ref={props.forwardedRef}
          src={props.video.src}
          style={style}
          onTimeUpdate={handleOnTimeUpdate}
        ></video>
        {/* {!isPlaying && (
          <div className="bg-play">
            <BigIconPlay onClick={togglePlay} />
          </div>
        )} */}
      </div>
      <StyledVideoControls>
        <StyledVideoActions>
          <div className="controller" onClick={togglePlay}>
            {!isPlaying ? <IconPlay /> : <IconStop />}
          </div>
          <input
            type="range"
            min="0"
            max="100"
            value={progress}
            onChange={(e) => handleVideoProgress(e)}
          />
          <div
            style={{
              paddingLeft: "10px",
              paddingRight: "14px",
              color: "#DDDDDD",
            }}
          >
            {duration}
          </div>
          <div className="controller" onClick={toggleMute}>
            {!isMuted ? <IconSound /> : <IconMuted />}
          </div>
          <div className="controller" onClick={toggleScreen}>
            {!isFullScreen ? <IconFullScreen /> : <IconExitFullScreen />}
          </div>
        </StyledVideoActions>
      </StyledVideoControls>
    </StyledVideoPlayer>
  );
}
