import ReactDOM from "react-dom";
import React, { useRef, useState, useEffect, useCallback } from "react";
import { isMobileOnly } from "react-device-detect";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import ContextMenu from "@docspace/components/context-menu";

import { StyledViewer } from "@docspace/components/viewer/styled-viewer";
import ViewerPlayer from "@docspace/components/viewer/sub-components/viewer-player";

import { ControlBtn, StyledViewerContainer } from "../../StyledComponents";

import MobileDetails from "../MobileDetails";
import PrevButton from "../PrevButton";
import NextButton from "../NextButton";

import type ViewerProps from "./Viewer.props";

import ViewerMediaCloseSvgUrl from "PUBLIC_DIR/images/viewer.media.close.svg?url";

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

  useEffect(() => {
    if (isMobileOnly) return;

    const resetTimer = () => {
      setPanelVisible(true);
      clearTimeout(timerIDRef.current);
      timerIDRef.current = setTimeout(() => setPanelVisible(false), 2500);
      setImageTimer(timerIDRef.current);
    };

    document.addEventListener("mousemove", resetTimer, { passive: true });

    return () => {
      document.removeEventListener("mousemove", resetTimer);
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
      isError={isError}
      title={props.title}
      icon={props.headerIcon}
      contextModel={props.contextModel}
      isPreviewFile={props.isPreviewFile}
      onHide={onHide}
      onContextMenu={onMobileContextMenu}
      onMaskClick={props.onMaskClick}
      ref={contextMenuRef}
    />
  );

  const displayUI = (isMobileOnly && props.isAudio) || panelVisible;

  const isNotFirstElement = props.playlistPos !== 0;
  const isNotLastElement = props.playlistPos < props.playlist.length - 1;

  return (
    <StyledViewerContainer visible={props.visible}>
      {!isFullscreen && !isMobileOnly && displayUI && (
        <div>
          <div className="details">
            <Text
              isBold
              fontSize="14px"
              className="title"
              title={undefined}
              tag={undefined}
              as={undefined}
              fontWeight={undefined}
              color={undefined}
              textAlign={undefined}
            >
              {props.title}
            </Text>
            <ControlBtn
              onClick={props.onMaskClick}
              className="mediaPlayerClose"
            >
              <IconButton
                color={"#fff"}
                iconName={ViewerMediaCloseSvgUrl}
                size={28}
                isClickable
              />
            </ControlBtn>
          </div>
        </div>
      )}

      {props.playlist.length > 1 && !isFullscreen && displayUI && (
        <>
          {isNotFirstElement && <PrevButton prevClick={prevClick} />}
          {isNotLastElement && <NextButton nextClick={nextClick} />}
        </>
      )}

      {props.isImage
        ? ReactDOM.createPortal(
            <StyledViewer
              {...props}
              displayUI={displayUI}
              mobileDetails={mobileDetails}
              setIsOpenContextMenu={setIsOpenContextMenu}
              container={containerRef.current}
              imageTimer={imageTimer}
              onMaskClick={props.onMaskClick}
              setPanelVisible={setPanelVisible}
              generateContextMenu={props.generateContextMenu}
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
