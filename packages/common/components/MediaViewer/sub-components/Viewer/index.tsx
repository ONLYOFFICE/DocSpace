import ReactDOM from "react-dom";
import { isMobile } from "react-device-detect";
import React, { useRef, useState, useEffect, useCallback } from "react";

import ContextMenu from "@docspace/components/context-menu";

import { StyledViewerContainer } from "../../StyledComponents";

import NextButton from "../NextButton";
import PrevButton from "../PrevButton";
import ImageViewer from "../ImageViewer";
import MobileDetails from "../MobileDetails";
import DesktopDetails from "../DesktopDetails";
import ViewerPlayer from "../ViewerPlayer";

import type ViewerProps from "./Viewer.props";
import PDFViewer from "../PDFViewer";

function Viewer(props: ViewerProps) {
  const timerIDRef = useRef<NodeJS.Timeout>();

  const containerRef = React.useRef(document.createElement("div"));

  const [isPDFSidebarOpen, setIsPDFSidebarOpen] = useState<boolean>(false);
  const [panelVisible, setPanelVisible] = useState<boolean>(true);
  const [isOpenContextMenu, setIsOpenContextMenu] = useState<boolean>(false);

  const [isError, setIsError] = useState<boolean>(false);

  const [imageTimer, setImageTimer] = useState<NodeJS.Timeout>();

  const panelVisibleRef = useRef<boolean>(false);
  const panelToolbarRef = useRef<boolean>(false);

  const contextMenuRef = useRef<ContextMenu>(null);

  const [isFullscreen, setIsFullScreen] = useState<boolean>(false);
  useEffect(() => {
    document.body.appendChild(containerRef.current);

    return () => {
      document.body.removeChild(containerRef.current);
      timerIDRef.current && clearTimeout(timerIDRef.current);
    };
  }, []);

  useEffect(() => {
    if (isOpenContextMenu) {
      clearTimeout(timerIDRef.current);
    }
  }, [isOpenContextMenu]);

  useEffect(() => {
    if (isMobile) return;
    resetToolbarVisibleTimer();
    document.addEventListener("mousemove", resetToolbarVisibleTimer, {
      passive: true,
    });

    return () => {
      document.removeEventListener("mousemove", resetToolbarVisibleTimer);
      clearTimeout(timerIDRef.current);
      setPanelVisible(true);
    };
  }, [setImageTimer, setPanelVisible]);

  const resetToolbarVisibleTimer = () => {
    if (panelToolbarRef.current) return;

    if (panelVisibleRef.current && panelVisible) {
      clearTimeout(timerIDRef.current);
      timerIDRef.current = setTimeout(() => {
        panelVisibleRef.current = false;
        setPanelVisible(false);
      }, 2500);
    } else {
      setPanelVisible(true);
      clearTimeout(timerIDRef.current);
      panelVisibleRef.current = true;

      timerIDRef.current = setTimeout(() => {
        panelVisibleRef.current = false;
        setPanelVisible(false);
      }, 2500);
    }
  };

  const removeToolbarVisibleTimer = () => {
    clearTimeout(timerIDRef.current);
    panelVisibleRef.current = false;
    panelToolbarRef.current = true;
  };

  const removePanelVisibleTimeout = () => {
    clearTimeout(timerIDRef.current);
    panelVisibleRef.current = true;
    panelToolbarRef.current = false;
    setPanelVisible(true);
  };

  const restartToolbarVisibleTimer = () => {
    panelToolbarRef.current = false;
    resetToolbarVisibleTimer();
  };

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

  const handleMaskClick = () => {
    if (isFullscreen) {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document["webkitExitFullscreen"]) {
        document["webkitExitFullscreen"]();
      } else if (document["mozCancelFullScreen"]) {
        document["mozCancelFullScreen"]();
      } else if (document["msExitFullscreen"]) {
        document["msExitFullscreen"]();
      }
    }

    props.onMaskClick();
  };

  const mobileDetails = (
    <MobileDetails
      onHide={onHide}
      isError={isError}
      title={props.title}
      ref={contextMenuRef}
      icon={props.headerIcon}
      onMaskClick={handleMaskClick}
      contextModel={props.contextModel}
      onContextMenu={onMobileContextMenu}
      isPreviewFile={props.isPreviewFile}
    />
  );

  const isNotFirstElement = props.playlistPos !== 0;
  const isNotLastElement = props.playlistPos < props.playlist.length - 1;

  const targetFile = props.playlist[props.playlistPos];

  const isTiff =
    targetFile?.fileExst === ".tiff" || targetFile.fileExst === ".tif";

  return (
    <StyledViewerContainer visible={props.visible}>
      {!isFullscreen && !isMobile && panelVisible && !props.isPdf && (
        <DesktopDetails title={props.title} onMaskClick={handleMaskClick} />
      )}

      {props.playlist.length > 1 && !isFullscreen && !isMobile && (
        <>
          {isNotFirstElement && !isPDFSidebarOpen && (
            <PrevButton prevClick={prevClick} />
          )}
          {isNotLastElement && (
            <NextButton isPdfFIle={props.isPdf} nextClick={nextClick} />
          )}
        </>
      )}

      {props.isImage
        ? ReactDOM.createPortal(
            <ImageViewer
              panelVisible={panelVisible}
              toolbar={props.toolbar}
              src={isTiff ? props.fileUrl : targetFile.src}
              isTiff={isTiff}
              thumbnailSrc={targetFile.thumbnailUrl}
              imageId={targetFile.fileId}
              version={targetFile.version}
              mobileDetails={mobileDetails}
              onMask={props.onMaskClick}
              onPrev={props.onPrevClick}
              onNext={props.onNextClick}
              isLastImage={!isNotLastElement}
              isFistImage={!isNotFirstElement}
              generateContextMenu={props.generateContextMenu}
              setIsOpenContextMenu={setIsOpenContextMenu}
              resetToolbarVisibleTimer={resetToolbarVisibleTimer}
              contextModel={props.contextModel}
              errorTitle={props.errorTitle}
            />,
            containerRef.current
          )
        : props.isVideo || props.isAudio
        ? ReactDOM.createPortal(
            <ViewerPlayer
              isError={isError}
              canDownload={!!props.targetFile?.security.Download}
              src={props.fileUrl}
              thumbnailSrc={targetFile.thumbnailUrl}
              isAudio={props.isAudio}
              isVideo={props.isVideo}
              panelVisible={panelVisible}
              audioIcon={props.audioIcon}
              isFullScreen={isFullscreen}
              errorTitle={props.errorTitle}
              mobileDetails={mobileDetails}
              isLastImage={!isNotLastElement}
              isFistImage={!isNotFirstElement}
              isPreviewFile={props.isPreviewFile}
              isOpenContextMenu={isOpenContextMenu}
              setIsError={setIsError}
              onMask={handleMaskClick}
              onPrev={props.onPrevClick}
              onNext={props.onNextClick}
              setPanelVisible={setPanelVisible}
              setIsFullScreen={setIsFullScreen}
              contextModel={props.contextModel}
              onDownloadClick={props.onDownloadClick}
              generateContextMenu={props.generateContextMenu}
              removeToolbarVisibleTimer={removeToolbarVisibleTimer}
              removePanelVisibleTimeout={removePanelVisibleTimeout}
              restartToolbarVisibleTimer={restartToolbarVisibleTimer}
            />,
            containerRef.current
          )
        : props.isPdf &&
          ReactDOM.createPortal(
            <PDFViewer
              src={props.fileUrl ?? ""}
              title={props.title}
              toolbar={props.toolbar}
              onMask={handleMaskClick}
              isPDFSidebarOpen={isPDFSidebarOpen}
              mobileDetails={mobileDetails}
              generateContextMenu={props.generateContextMenu}
              setIsOpenContextMenu={setIsOpenContextMenu}
              setIsPDFSidebarOpen={setIsPDFSidebarOpen}
              isLastImage={!isNotLastElement}
              isFistImage={!isNotFirstElement}
              onPrev={props.onPrevClick}
              onNext={props.onNextClick}
            />,
            containerRef.current
          )}
    </StyledViewerContainer>
  );
}

export default Viewer;
