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

import MediaNextIcon from "../../../public/images/media.view.react.svg";
import MediaPrevIcon from "../../../public/images/media.viewer.prev.react.svg";
import ViewerPlayer from "./sub-components/viewer-player";

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
  } = props;

  const defaultContainer = React.useRef(
    typeof document !== "undefined" ? document.createElement("div") : null
  );
  const [container, setContainer] = React.useState(props.container);
  const [init, setInit] = React.useState(false);

  const detailsContainerRef = React.useRef(null);
  const viewerToolboxRef = React.useRef(null);
  const videoElement = React.useRef(null);

  const [overlay, setOverlay] = React.useState(true);

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

  const viewerPortal = ReactDOM.createPortal(
    <StyledViewer {...props} />,
    container
  );

  const videoPortal = ReactDOM.createPortal(
    <ViewerPlayer
      setOverlay={setOverlay}
      forwardedRef={videoElement}
      video={playlist[playlistPos]}
    />,
    container
  );

  return (
    <StyledViewerContainer visible={visible}>
      {overlay && <div className="videoViewerOverlay"></div>}
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
              iconName="/static/images/cross.react.svg"
              size={25}
              isClickable
            />
          </ControlBtn>
        </div>
      </div>
      {playlist.length > 1 && (
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
