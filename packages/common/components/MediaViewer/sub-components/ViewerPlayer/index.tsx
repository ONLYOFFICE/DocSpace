import lodash from "lodash";
import { useGesture } from "@use-gesture/react";
import { useSpring, animated } from "@react-spring/web";
import { isMobile, isDesktop, isIOS, isMobileOnly } from "react-device-detect";
import React, {
  useCallback,
  useEffect,
  useLayoutEffect,
  useMemo,
  useRef,
  useState,
} from "react";

import ViewerPlayerProps from "./ViewerPlayer.props";
import {
  ContainerPlayer,
  ControlContainer,
  PlayerControlsWrapper,
  StyledPlayerControls,
  VideoWrapper,
} from "./ViewerPlayer.styled";

import PlayerBigPlayButton from "../PlayerBigPlayButton";
import ViewerLoader from "../ViewerLoader";
import PlayerPlayButton from "../PlayerPlayButton";
import PlayerDuration from "../PlayerDuration/inxed";
import PlayerVolumeControl from "../PlayerVolumeControl";
import PlayerTimeline from "../PlayerTimeline";
import PlayerSpeedControl from "../PlayerSpeedControl";
import PlayerFullScreen from "../PlayerFullScreen";
import PlayerDesktopContextMenu from "../PlayerDesktopContextMenu";
import { compareTo, KeyboardEventKeys } from "../../helpers";
import PlayerMessageError from "../PlayerMessageError";

const VolumeLocalStorageKey = "player-volume";
const defaultVolume = 100;
const audioWidth = 190;
const audioHeight = 190;

function ViewerPlayer({
  src,
  isAudio,
  isVideo,
  isError,
  audioIcon,
  errorTitle,
  isLastImage,
  isFistImage,
  isFullScreen,
  panelVisible,
  thumbnailSrc,
  mobileDetails,
  isPreviewFile,
  isOpenContextMenu,
  onMask,
  onNext,
  onPrev,
  setIsError,
  contextModel,
  setPanelVisible,
  setIsFullScreen,
  onDownloadClick,
  generateContextMenu,
  removeToolbarVisibleTimer,
  removePanelVisibleTimeout,
  restartToolbarVisibleTimer,
}: ViewerPlayerProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const playerWrapperRef = useRef<HTMLDivElement>(null);

  const isDurationInfinityRef = useRef<boolean>(false);

  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isPlaying, setIsPlaying] = useState<boolean>(false);
  const [isWaiting, setIsWaiting] = useState<boolean>(false);

  const [isMuted, setIsMuted] = useState<boolean>(() => {
    const valueStorage = localStorage.getItem(VolumeLocalStorageKey);

    if (!valueStorage) return false;

    return valueStorage === "0";
  });

  const [timeline, setTimeline] = useState<number>(0);
  const [duration, setDuration] = useState<number>(0);
  const [currentTime, setCurrentTime] = useState<number>(0);
  const [volume, setVolume] = useState<number>(() => {
    const valueStorage = localStorage.getItem(VolumeLocalStorageKey);

    if (!valueStorage) return defaultVolume;

    return JSON.parse(valueStorage);
  });

  const [style, api] = useSpring(() => ({
    width: 0,
    height: 0,
    x: 0,
    y: 0,
    opacity: 1,
  }));

  useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, [isFullScreen, isLoading]);

  useLayoutEffect(() => {
    setIsLoading(true);
    resetState();
  }, [src]);

  useEffect(() => {
    if (!isOpenContextMenu && isPlaying) {
      restartToolbarVisibleTimer();
    }
  }, [isOpenContextMenu]);
  useEffect(() => {
    window.addEventListener("fullscreenchange", onExitFullScreen, {
      capture: true,
    });
    return () =>
      window.removeEventListener("fullscreenchange", onExitFullScreen, {
        capture: true,
      });
  }, []);

  useEffect(() => {
    document.addEventListener("keydown", onKeyDown);

    return () => {
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [isPlaying]);

  const calculateAdjustImage = (point: { x: number; y: number }) => {
    if (!playerWrapperRef.current || !containerRef.current) return point;

    let playerBounds = playerWrapperRef.current.getBoundingClientRect();
    const containerBounds = containerRef.current.getBoundingClientRect();

    const originalWidth = playerWrapperRef.current.clientWidth;
    const widthOverhang = (playerBounds.width - originalWidth) / 2;

    const originalHeight = playerWrapperRef.current.clientHeight;
    const heightOverhang = (playerBounds.height - originalHeight) / 2;

    const isWidthOutContainer = playerBounds.width >= containerBounds.width;

    const isHeightOutContainer = playerBounds.height >= containerBounds.height;

    if (
      compareTo(playerBounds.left, containerBounds.left) &&
      isWidthOutContainer
    ) {
      point.x = widthOverhang;
    } else if (
      compareTo(containerBounds.right, playerBounds.right) &&
      isWidthOutContainer
    ) {
      point.x = containerBounds.width - playerBounds.width + widthOverhang;
    } else if (!isWidthOutContainer) {
      point.x =
        (containerBounds.width - playerBounds.width) / 2 + widthOverhang;
    }

    if (
      compareTo(playerBounds.top, containerBounds.top) &&
      isHeightOutContainer
    ) {
      point.y = heightOverhang;
    } else if (
      compareTo(containerBounds.bottom, playerBounds.bottom) &&
      isHeightOutContainer
    ) {
      point.y = containerBounds.height - playerBounds.height + heightOverhang;
    } else if (!isHeightOutContainer) {
      point.y =
        (containerBounds.height - playerBounds.height) / 2 + heightOverhang;
    }

    return point;
  };

  useGesture(
    {
      onDrag: ({ offset: [dx, dy], movement: [mdx, mdy], memo, first }) => {
        if (isDesktop) return;

        if (first) {
          memo = style.y.get();
        }

        api.start({
          x:
            (isFistImage && mdx > 0) || (isLastImage && mdx < 0) || isFullScreen
              ? style.x.get()
              : dx,
          y: dy >= memo ? dy : style.y.get(),
          opacity: mdy > 0 ? Math.max(1 - mdy / 120, 0) : style.opacity.get(),
          immediate: true,
        });

        return memo;
      },
      onDragEnd: ({ movement: [mdx, mdy] }) => {
        if (isDesktop) return;

        if (!isFullScreen) {
          if (mdx < -style.width.get() / 4) {
            return onNext();
          } else if (mdx > style.width.get() / 4) {
            return onPrev();
          }
        }
        if (mdy > 120) {
          return onMask();
        }

        const newPoint = calculateAdjustImage({
          x: style.x.get(),
          y: style.y.get(),
        });

        api.start({
          ...newPoint,
          opacity: 1,
        });
      },
      onClick: ({ dragging, event }) => {
        if (isDesktop && event.target === containerRef.current) return onMask();

        if (
          dragging ||
          !isMobile ||
          isAudio ||
          event.target !== containerRef.current
        )
          return;

        if (panelVisible) {
          removeToolbarVisibleTimer();
          setPanelVisible(false);
        } else {
          isPlaying && restartToolbarVisibleTimer();
          setPanelVisible(true);
        }
      },
    },
    {
      drag: {
        from: () => [style.x.get(), style.y.get()],
        axis: "lock",
      },
      target: containerRef,
    }
  );

  const onKeyDown = (event: KeyboardEvent) => {
    if (event.code === KeyboardEventKeys.Space) {
      togglePlay();
    }
  };
  const onExitFullScreen = () => {
    if (!document.fullscreenElement) {
      setIsFullScreen(false);
      handleResize();
    }
  };

  const resetState = () => {
    setTimeline(0);
    setDuration(0);
    setCurrentTime(0);
    setIsPlaying(false);
    setIsError(false);
    removePanelVisibleTimeout();
  };

  const getVideoWidthHeight = (video: HTMLVideoElement): [number, number] => {
    const maxWidth = window.innerWidth;
    const maxHeight = window.innerHeight;

    const elementWidth = isAudio ? audioWidth : video.videoWidth;
    const elementHeight = isAudio ? audioHeight : video.videoHeight;

    let width =
      elementWidth > maxWidth
        ? maxWidth
        : isFullScreen
        ? Math.max(maxWidth, elementWidth)
        : Math.min(maxWidth, elementWidth);

    let height = (width / elementWidth) * elementHeight;

    if (height > maxHeight) {
      height = maxHeight;
      width = (height / elementHeight) * elementWidth;
    }

    return [width, height];
  };

  const getVideoPosition = (
    width: number,
    height: number
  ): [number, number] => {
    let left = (window.innerWidth - width) / 2;
    let top = (window.innerHeight - height) / 2;

    return [left, top];
  };

  const setSizeAndPosition = (target: HTMLVideoElement) => {
    const [width, height] = getVideoWidthHeight(target);
    const [x, y] = getVideoPosition(width, height);

    api.start({
      x,
      y,
      width,
      height,
      immediate: true,
    });
  };

  const handleResize = () => {
    const target = videoRef.current;

    if (!target || isLoading) return;

    setSizeAndPosition(target);
  };

  const handleLoadedMetaDataVideo = (
    event: React.SyntheticEvent<HTMLVideoElement, Event>
  ) => {
    const target = event.target as HTMLVideoElement;

    setSizeAndPosition(target);

    target.volume = volume / 100;
    target.muted = isMuted;
    target.playbackRate = 1;

    if (target.duration === Infinity) {
      isDurationInfinityRef.current = true;
      target.currentTime = Number.MAX_SAFE_INTEGER;
      return;
    }
    setDuration(target.duration);
    setIsLoading(false);
  };

  const handleDurationChange = (
    event: React.SyntheticEvent<HTMLVideoElement, Event>
  ) => {
    const target = event.target as HTMLVideoElement;
    if (!Number.isFinite(target.duration) || !isDurationInfinityRef.current)
      return;

    target.currentTime = 0;
    isDurationInfinityRef.current = false;
    setDuration(target.duration);
    setIsLoading(false);
  };

  const togglePlay = useCallback(() => {
    if (!videoRef.current) return;

    if (isMobile && !isPlaying && isVideo) {
      restartToolbarVisibleTimer();
    }

    if (isPlaying) {
      videoRef.current.pause();
      setIsPlaying(false);
      setPanelVisible(true);
      isMobile && removeToolbarVisibleTimer();
    } else {
      videoRef.current.play();
      setIsPlaying(true);
    }
  }, [isPlaying, isVideo]);

  const handleBigPlayButtonClick = () => {
    togglePlay();
  };

  const handleTimeUpdate = () => {
    if (!videoRef.current || isLoading) return;

    const { currentTime, duration } = videoRef.current;
    const percent = (currentTime / duration) * 100;

    setTimeline(percent);

    setCurrentTime(currentTime);
  };

  const handleChangeTimeLine = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!videoRef.current) return;

    const percent = Number(event.target.value);
    const newCurrentTime = (percent / 100) * videoRef.current.duration;

    setTimeline(percent);
    setCurrentTime(newCurrentTime);
    videoRef.current.currentTime = newCurrentTime;
  };

  const handleClickVideo = () => {
    if (isMobile) {
      if (!isPlaying) {
        return setPanelVisible((prev) => !prev);
      }

      if (panelVisible) {
        videoRef.current?.pause();
        setIsPlaying(false);
        return removeToolbarVisibleTimer();
      }

      return isPlaying && restartToolbarVisibleTimer();
    }
    togglePlay();
  };

  const handleVideoEnded = () => {
    setIsPlaying(false);
    if (isMobile) removePanelVisibleTimeout();
  };

  const handleVolumeChange = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      if (!videoRef.current) return;

      const newVolume = Number(event.target.value);
      localStorage.setItem(VolumeLocalStorageKey, event.target.value);

      if (newVolume === 0) {
        setIsMuted(true);
        videoRef.current.muted = true;
      }

      if (isMuted && newVolume > 0) {
        setIsMuted(false);
        videoRef.current.muted = false;
      }

      videoRef.current.volume = newVolume / 100;
      setVolume(newVolume);
    },
    [isMuted]
  );

  const handleSpeedChange = useCallback((speed: number) => {
    if (!videoRef.current) return;

    videoRef.current.playbackRate = speed;
  }, []);

  const toggleVolumeMute = useCallback(() => {
    if (!videoRef.current) return;

    const volume = videoRef.current.volume * 100 || defaultVolume;

    if (isMuted) {
      setIsMuted(false);
      setVolume(volume);

      videoRef.current.volume = volume / 100;
      videoRef.current.muted = false;

      localStorage.setItem(VolumeLocalStorageKey, volume.toString());
    } else {
      setIsMuted(true);
      setVolume(0);
      videoRef.current.muted = true;
      localStorage.setItem(VolumeLocalStorageKey, "0");
    }
  }, [isMuted]);

  const toggleVideoFullscreen = useCallback(() => {
    if (!videoRef.current) return;

    if (isIOS && isMobileOnly) {
      videoRef.current.pause();
      videoRef.current.playsInline = false;
      videoRef.current.play();
      videoRef.current.playsInline = true;

      return;
    }

    if (isFullScreen) {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document["webkitExitFullscreen"]) {
        document["webkitExitFullscreen"]();
      } else if (document["mozCancelFullScreen"]) {
        document["mozCancelFullScreen"]();
      } else if (document["msExitFullscreen"]) {
        document["msExitFullscreen"]();
      }
    } else {
      if (document.documentElement.requestFullscreen) {
        document.documentElement.requestFullscreen();
      } else if (document.documentElement["mozRequestFullScreen"]) {
        document.documentElement["mozRequestFullScreen"]();
      } else if (document.documentElement["webkitRequestFullScreen"]) {
        document.documentElement["webkitRequestFullScreen"]();
      } else if (document.documentElement["webkitEnterFullScreen"]) {
        document.documentElement["webkitEnterFullScreen"]();
      }
    }

    setIsFullScreen((pre) => !pre);
  }, [isFullScreen]);

  const onMouseEnter = () => {
    if (isMobile) return;

    removeToolbarVisibleTimer();
  };
  const onMouseLeave = () => {
    if (isMobile) return;

    restartToolbarVisibleTimer();
  };

  const onTouchStart = () => {
    if (isPlaying && isVideo) restartToolbarVisibleTimer();
  };
  const onTouchMove = () => {
    if (isPlaying && isVideo) restartToolbarVisibleTimer();
  };

  const model = useMemo(contextModel, [contextModel]);
  const hideContextMenu = useMemo(
    () => model.filter((item) => !item.disabled).length <= 1,
    [model]
  );

  return (
    <>
      {isMobile && panelVisible && mobileDetails}
      <ContainerPlayer ref={containerRef} $isFullScreen={isFullScreen}>
        <VideoWrapper
          $visible={!isLoading}
          style={style}
          ref={playerWrapperRef}
        >
          <animated.video
            style={lodash.omit(style, ["x", "y"])}
            src={thumbnailSrc ? src : `${src}#t=0.001`}
            playsInline
            ref={videoRef}
            hidden={isAudio}
            preload="metadata"
            poster={thumbnailSrc && `${thumbnailSrc}&size=1280x720`}
            onClick={handleClickVideo}
            onEnded={handleVideoEnded}
            onDurationChange={handleDurationChange}
            onTimeUpdate={handleTimeUpdate}
            onPlaying={() => setIsWaiting(false)}
            onWaiting={() => setIsWaiting(true)}
            onError={(error) => {
              console.error("video error", error);
              setIsError(true);
              setIsLoading(false);
            }}
            onLoadedMetadata={handleLoadedMetaDataVideo}
          />
          <PlayerBigPlayButton
            onClick={handleBigPlayButtonClick}
            visible={!isPlaying && isVideo && !isError}
          />
          {isAudio && (
            <div className="audio-container">
              <img src={audioIcon} />
            </div>
          )}
        </VideoWrapper>

        <ViewerLoader
          isLoading={isLoading || (isWaiting && isPlaying)}
          isError={isError}
        />
      </ContainerPlayer>
      {isError ? (
        <PlayerMessageError
          model={model}
          onMaskClick={onMask}
          errorTitle={errorTitle}
        />
      ) : (
        <StyledPlayerControls
          $isShow={panelVisible && !isLoading}
          onTouchStart={onTouchStart}
          onTouchMove={onTouchMove}
        >
          <PlayerControlsWrapper>
            <PlayerTimeline
              value={timeline}
              duration={duration}
              onChange={handleChangeTimeLine}
              onMouseEnter={onMouseEnter}
              onMouseLeave={onMouseLeave}
            />
            <ControlContainer>
              <div
                className="player_left-control"
                onMouseEnter={onMouseEnter}
                onMouseLeave={onMouseLeave}
              >
                <PlayerPlayButton isPlaying={isPlaying} onClick={togglePlay} />
                <PlayerDuration currentTime={currentTime} duration={duration} />
                {!isMobile && (
                  <PlayerVolumeControl
                    volume={volume}
                    isMuted={isMuted}
                    onChange={handleVolumeChange}
                    toggleVolumeMute={toggleVolumeMute}
                  />
                )}
              </div>
              <div
                className="player_right-control"
                onMouseEnter={onMouseEnter}
                onMouseLeave={onMouseLeave}
              >
                <PlayerSpeedControl
                  src={src}
                  onMouseLeave={onMouseLeave}
                  handleSpeedChange={handleSpeedChange}
                />
                <PlayerFullScreen
                  isAudio={isAudio}
                  isFullScreen={isFullScreen}
                  onClick={toggleVideoFullscreen}
                />
                {isDesktop && (
                  <PlayerDesktopContextMenu
                    isPreviewFile={isPreviewFile}
                    hideContextMenu={hideContextMenu}
                    onDownloadClick={onDownloadClick}
                    generateContextMenu={generateContextMenu}
                  />
                )}
              </div>
            </ControlContainer>
          </PlayerControlsWrapper>
        </StyledPlayerControls>
      )}
    </>
  );
}

export default ViewerPlayer;
