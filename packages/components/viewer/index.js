import * as React from "react";
import * as ReactDOM from "react-dom";
import {
  StyledViewer,
  StyledViewerContainer,
  StyledNextToolbar,
  StyledButtonScroll,
} from "./styled-viewer";
import ControlBtn from "./sub-components/control-btn";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { isMobileOnly } from "react-device-detect";

import MediaNextIcon from "../../../public/images/media.view.react.svg";
import MediaPrevIcon from "../../../public/images/media.viewer.prev.react.svg";
import ViewerPlayer from "./sub-components/viewer-player";

import MediaContextMenu from "../../../public/images/vertical-dots.react.svg";
import ContextMenu from "@docspace/components/context-menu";
import BackArrow from "../../../public/images/arrow.path.react.svg";

export const Viewer = (props) => {
  const {
    visible,
    onMaskClick,
    title,
    onNextClick,
    onPrevClick,
    playlistPos,
    playlist,
    isImage,
    contextModel,
    generateContextMenu,
  } = props;

  const defaultContainer = React.useRef(
    typeof document !== "undefined" ? document.createElement("div") : null
  );
  const [container, setContainer] = React.useState(props.container);
  const [init, setInit] = React.useState(false);

  const detailsContainerRef = React.useRef(null);
  const viewerToolboxRef = React.useRef(null);
  const videoElement = React.useRef(null);
  const cm = React.useRef(null);

  const [isFullscreen, setIsFullScreen] = React.useState(false);

  React.useEffect(() => {
    document.body.appendChild(defaultContainer.current);
  }, []);

  React.useEffect(() => {
    if (props.visible && !init) {
      setInit(true);
    }
  }, [props.visible, init]);

  React.useEffect(() => {
    if (props.container) {
      setContainer(props.container);
    } else {
      setContainer(defaultContainer.current);
    }
  }, [props.container]);

  if (!init) {
    return null;
  }
  const onContextMenu = (e) => {
    cm.current.show(e);
  };

  let detailsStyle = {
    "z-index": "307",
    position: "fixed",
    top: "0",
    left: "0",
    right: "0",
    height: "53px",
    background: "#333",
    display: "flex",
    "justify-content": "space-around",
    "align-items": "center",
  };

  const mobileDetails = (
    <div className="mobile-details" style={detailsStyle}>
      <BackArrow onClick={onMaskClick} />
      <Text isBold fontSize="14px" color={"#D1D1D1"} className="title">
        {title}
      </Text>
      <div className="details-context">
        <MediaContextMenu onClick={onContextMenu} />
        <ContextMenu
          getContextModel={contextModel}
          ref={cm}
          withBackdrop={true}
        />
      </div>
    </div>
  );

  const viewerPortal = ReactDOM.createPortal(
    <StyledViewer
      {...props}
      mobileDetails={mobileDetails}
      generateContextMenu={generateContextMenu}
    />,
    container
  );

  const videoPortal = ReactDOM.createPortal(
    <ViewerPlayer
      contextModel={contextModel}
      mobileDetails={mobileDetails}
      title={title}
      onMaskClick={onMaskClick}
      generateContextMenu={generateContextMenu}
      setIsFullScreen={setIsFullScreen}
      videoRef={videoElement}
      video={playlist[playlistPos]}
      activeIndex={playlistPos}
    />,
    container
  );

  return (
    <StyledViewerContainer visible={visible}>
      {!isFullscreen && !isMobileOnly && (
        <div>
          <div className="details" ref={detailsContainerRef}>
            <Text isBold fontSize="14px" className="title">
              {title}
            </Text>
            <ControlBtn
              onClick={onMaskClick && onMaskClick}
              className="mediaPlayerClose"
            >
              <IconButton
                color={"#fff"}
                iconName="/static/images/cross.react.svg"
                size={17}
                isClickable
              />
            </ControlBtn>
          </div>
        </div>
      )}

      {playlist.length > 1 && !isFullscreen && (
        <>
          <StyledNextToolbar left onClick={onPrevClick}>
            <StyledButtonScroll orientation="left">
              <MediaPrevIcon />
            </StyledButtonScroll>
          </StyledNextToolbar>
          {playlistPos < playlist.length - 1 && (
            <>
              <StyledNextToolbar onClick={onNextClick}>
                <StyledButtonScroll orientation="right">
                  <MediaNextIcon />
                </StyledButtonScroll>
              </StyledNextToolbar>
            </>
          )}
        </>
      )}

      {isImage ? (
        <>
          <div className="mediaViewerToolbox" ref={viewerToolboxRef}></div>
          {viewerPortal}
        </>
      ) : (
        <>{videoPortal}</>
      )}
    </StyledViewerContainer>
  );
};
