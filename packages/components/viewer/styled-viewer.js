import styled from "styled-components";
import { Base } from "@docspace/components/themes";
import { ViewerBase } from "./sub-components/viewer-base";

const StyledViewerContainer = styled.div`
  color: ${(props) => props.theme.mediaViewer.color};
  display: ${(props) => (props.visible ? "block" : "none")};
  overflow: hidden;
  span {
    position: fixed;
    right: 0;
    bottom: 5px;
    margin-right: 10px;
    z-index: 305;
  }
  .deleteBtnContainer,
  .downloadBtnContainer {
    display: block;
    width: 16px;
    height: 16px;
    margin: 4px 12px;
    line-height: 19px;
    svg {
      path {
        fill: ${(props) => props.theme.mediaViewer.fill};
      }
    }
  }
  .details {
    z-index: 307;
    padding-top: 21px;
    height: 64px;
    width: 100%;
    background: linear-gradient(
      0deg,
      rgba(0, 0, 0, 0) 0%,
      rgba(0, 0, 0, 0.8) 100%
    );
    position: fixed;
    top: 0;
    left: 0;
    .title {
      text-align: center;
      white-space: nowrap;
      overflow: hidden;
      font-size: 20px;
      font-weight: 600;
      text-overflow: ellipsis;
      width: calc(100% - 50px);
      padding-left: 16px;
      box-sizing: border-box;
      color: ${(props) => props.theme.mediaViewer.titleColor};
    }
  }
  .mediaPlayerClose {
    position: fixed;
    top: 13px;
    right: 12px;
    height: 17px;
    &:hover {
      background-color: transparent;
    }
    svg {
      path {
        fill: ${(props) => props.theme.mediaViewer.iconColor};
      }
    }
  }

  .containerVideo {
    position: fixed;
    top: 0;
    bottom: 0;
    right: 0;
    left: 0;
  }
`;

StyledViewerContainer.defaultProps = { theme: Base };

const StyledViewer = styled(ViewerBase)`
  .react-viewer-inline {
    position: relative;
    width: 100%;
    height: 100%;
    min-height: 400px;
  }
  .react-viewer-inline > .react-viewer-mask,
  .react-viewer-inline > .react-viewer-close,
  .react-viewer-inline > .react-viewer-canvas,
  .react-viewer-inline > .react-viewer-footer {
    position: absolute;
  }

  .react-viewer ul {
    margin: 0;
    padding: 0;
  }

  .react-viewer li {
    list-style: none;
  }

  .react-viewer-footer {
    padding: 10px 24px;
    position: fixed;
    border-radius: 18px;
    bottom: 24px;
    z-index: 307;
    height: 48px;
    transition: all 0.26s ease-out;
    background: rgba(0, 0, 0, 0.4);
    text-align: center;
    &:hover {
      background: rgba(0, 0, 0, 0.8);
    }
  }

  .react-viewer-container {
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .react-viewer-mask {
    position: fixed;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    background-color: rgba(55, 55, 55, 0.6);
    height: 100%;
  }

  .react-viewer-toolbar {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 0;
    margin: 0;
    z-index: 308;
  }

  .react-viewer-canvas {
    position: fixed;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    overflow: hidden;

    img {
      display: block;
      width: auto;
      height: auto;
      user-select: none;
    }
    img.drag {
      cursor: move;
    }
  }

  .react-viewer-list {
    height: 50px;
    padding: 1px;
    text-align: left;
  }

  .react-viewer-list > li {
    display: inline-block;
    width: 30px;
    height: 50px;
    cursor: pointer;
    overflow: hidden;
    margin-right: 1px;
  }

  .react-viewer-list > li > img {
    width: 60px;
    height: 50px;
    margin-left: -15px;
    opacity: 0.5;
  }

  .react-viewer-list > li.active > img {
    opacity: 1;
  }
`;

const StyledSwitchToolbar = styled.div`
  height: 100%;
  z-index: 306;
  position: fixed;
  width: 73px;
  background: inherit;
  display: block;
  opacity: 0;
  transition: all 0.3s;
  ${(props) => (props.left ? "left: 0" : "right: 0")};
  &:hover {
    cursor: pointer;
    opacity: 1;
  }
`;

const StyledMobileDetails = styled.div`
  z-index: 307;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 53px;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(
    0deg,
    rgba(0, 0, 0, 0) 0%,
    rgba(0, 0, 0, 0.8) 100%
  );

  svg {
    path {
      fill: #fff;
    }
  }

  .mobile-close {
    position: fixed;
    left: 21px;
    top: 22px;
  }

  .mobile-context {
    position: fixed;
    right: 22px;
    top: 22px;
  }

  .title {
    font-weight: 600;
    margin-top: 6px;
    width: calc(100% - 100px);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const StyledButtonScroll = styled.div`
  z-index: 307;
  position: fixed;
  top: calc(50% - 20px);

  ${(props) => (props.orientation === "left" ? "left: 20px;" : "right: 20px;")}
`;

export {
  StyledViewerContainer,
  StyledViewer,
  StyledSwitchToolbar,
  StyledButtonScroll,
  StyledMobileDetails,
};
