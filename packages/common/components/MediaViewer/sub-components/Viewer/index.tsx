import ReactDOM from "react-dom";
import { isMobileOnly, isMobile } from "react-device-detect";
import React, { useRef, useState, useEffect, useCallback } from "react";

import ContextMenu from "@docspace/components/context-menu";
import ViewerPlayer from "@docspace/components/viewer/sub-components/viewer-player";

import { StyledViewerContainer } from "../../StyledComponents";

import NextButton from "../NextButton";
import PrevButton from "../PrevButton";
import ImageViewer from "../ImageViewer";
import MobileDetails from "../MobileDetails";
import DesktopDetails from "../DesktopDetails";

import type ViewerProps from "./Viewer.props";

function Viewer(props: ViewerProps) {
  const timerIDRef = useRef<NodeJS.Timeout>();

  const containerRef = React.useRef(document.createElement("div"));

  const [panelVisible, setPanelVisible] = useState<boolean>(true);
  const [isOpenContextMenu, setIsOpenContextMenu] = useState<boolean>(false);
  const [isError, setIsError] = useState<boolean>(false);
  const [isPlay, setIsPlay] = useState<boolean | null>(null);

  const [imageTimer, setImageTimer] = useState<NodeJS.Timeout>();

  const contextMenuRef = useRef<ContextMenu>(null);
  const videoElementRef = useRef<HTMLVideoElement>(null);

  const [isFullscreen, setIsFullScreen] = useState<boolean>(false);
  useEffect(() => {
    document.body.appendChild(containerRef.current);

    return () => {
      document.body.removeChild(containerRef.current);
      timerIDRef.current && clearTimeout(timerIDRef.current);
    };
  }, []);

  useEffect(() => {
    if ((!isPlay || isOpenContextMenu) && (!props.isImage || isOpenContextMenu))
      return clearTimeout(timerIDRef.current);
  }, [isPlay, isOpenContextMenu, props.isImage]);

  const resetToolbarVisibleTimer = () => {
    setPanelVisible(true);
    clearTimeout(timerIDRef.current);
    timerIDRef.current = setTimeout(() => setPanelVisible(false), 2500);
    setImageTimer(timerIDRef.current);
  };

  useEffect(() => {
    if (isMobile) return;

    document.addEventListener("mousemove", resetToolbarVisibleTimer, {
      passive: true,
    });

    return () => {
      document.removeEventListener("mousemove", resetToolbarVisibleTimer);
      clearTimeout(timerIDRef.current);
      setPanelVisible(true);
    };
  }, [setImageTimer, setPanelVisible]);

  useEffect(() => {
    document.addEventListener("touchstart", onTouch);

    return () => document.removeEventListener("touchstart", onTouch);
  }, [setPanelVisible]);

  const onTouch = useCallback(
    (e: TouchEvent, canTouch?: boolean) => {
      if (e.target === videoElementRef.current || canTouch) {
        setPanelVisible((visible) => !visible);
      }
    },
    [setPanelVisible]
  );

  const nextClick = () => {
    clearTimeout(imageTimer);
    props.onNextClick();
  };

  const prevClick = () => {
    clearTimeout(imageTimer);
    props.onPrevClick();
  };

  const onMobileContextMenu = useCallback(
    (e: TouchEvent) => {
      setIsOpenContextMenu((open) => !open);
      props.onSetSelectionFile();
      contextMenuRef.current?.show(e);
    },
    [props.onSetSelectionFile, setIsOpenContextMenu]
  );

  const onHide = useCallback(() => {
    setIsOpenContextMenu(false);
  }, [setIsOpenContextMenu]);

  const mobileDetails = (
    <MobileDetails
      onHide={onHide}
      isError={isError}
      title={props.title}
      ref={contextMenuRef}
      icon={props.headerIcon}
      onMaskClick={props.onMaskClick}
      contextModel={props.contextModel}
      onContextMenu={onMobileContextMenu}
      isPreviewFile={props.isPreviewFile}
    />
  );

  const displayUI = (isMobileOnly && props.isAudio) || panelVisible;

  const isNotFirstElement = props.playlistPos !== 0;
  const isNotLastElement = props.playlistPos < props.playlist.length - 1;

  return (
    <StyledViewerContainer visible={props.visible}>
      {!isFullscreen && !isMobile && panelVisible && (
        <DesktopDetails title={props.title} onMaskClick={props.onMaskClick} />
      )}

      {props.playlist.length > 1 && !isFullscreen && !isMobile && (
        <>
          {isNotFirstElement && <PrevButton prevClick={prevClick} />}
          {isNotLastElement && <NextButton nextClick={nextClick} />}
        </>
      )}

      {props.isImage
        ? ReactDOM.createPortal(
            <ImageViewer
              panelVisible={panelVisible}
              toolbar={props.toolbar}
              src={props.images[0].src}
              mobileDetails={mobileDetails}
              onMask={props.onMaskClick}
              onPrev={props.onPrevClick}
              onNext={props.onNextClick}
              isLastImage={!isNotLastElement}
              isFistImage={!isNotFirstElement}
              setPanelVisible={setPanelVisible}
              generateContextMenu={props.generateContextMenu}
              setIsOpenContextMenu={setIsOpenContextMenu}
              resetToolbarVisibleTimer={resetToolbarVisibleTimer}
            />,
            containerRef.current
          )
        : (props.isVideo || props.isAudio) &&
          ReactDOM.createPortal(
            <ViewerPlayer
              {...props}
              onNextClick={nextClick}
              onPrevClick={prevClick}
              isAudio={props.isAudio}
              audioIcon={props.audioIcon}
              contextModel={props.contextModel}
              mobileDetails={mobileDetails}
              displayUI={displayUI}
              isOpenContextMenu={isOpenContextMenu}
              onTouch={onTouch}
              title={props.title}
              setIsPlay={setIsPlay}
              setIsOpenContextMenu={setIsOpenContextMenu}
              isPlay={isPlay}
              onMaskClick={props.onMaskClick}
              setPanelVisible={setPanelVisible}
              generateContextMenu={props.generateContextMenu}
              setIsFullScreen={setIsFullScreen}
              setIsError={setIsError}
              videoRef={videoElementRef}
              video={props.playlist[props.playlistPos]}
              activeIndex={props.playlistPos}
            />,
            containerRef.current
          )}
    </StyledViewerContainer>
  );
}

export default Viewer;
