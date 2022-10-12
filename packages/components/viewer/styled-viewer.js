import styled from "styled-components";
import { Base } from "@docspace/components/themes";
import { ViewerBase } from "./sub-components/viewer-base";

const StyledViewerContainer = styled.div`
  color: ${(props) => props.theme.mediaViewer.color};
  display: ${(props) => (props.visible ? "block" : "none")};
  overflow: hidden;
  .videoViewerOverlay {
    position: fixed;
    z-index: 300;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: black;
    opacity: 0.5;
  }
  .mediaViewerToolbox {
    z-index: 1006;
    padding-top: 14px;
    padding-bottom: 14px;
    height: 20px;
    width: 100%;
    background-color: ${(props) => props.theme.mediaViewer.backgroundColor};
    position: fixed;
    bottom: 0;
    left: 0;
    text-align: center;
  }
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
    z-index: 1007;
    padding-top: 14px;
    padding-bottom: 14px;
    height: 20px;
    width: 100%;
    background: ${(props) => props.theme.mediaViewer.background};
    position: fixed;
    top: 0;
    left: 0;
    .title {
      text-align: center;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      width: calc(100% - 50px);
      padding-left: 16px;
      box-sizing: border-box;
      color: ${(props) => props.theme.mediaViewer.titleColor};
    }
  }
  .mediaPlayerClose {
    position: fixed;
    top: 4px;
    right: 10px;
    height: 25px;
    width: 25px;
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
    position: fixed;
    right: 0;
    bottom: 0;
    left: 0;
    overflow: hidden;
    text-align: center;
    z-index: @zIndex + 6;
  }
  .react-viewer-mask {
    position: fixed;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    background-color: #373737;
    background-color: rgba(55, 55, 55, 0.6);
    height: 100%;
  }

  .react-viewer-toolbar {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 0;
    margin: 0;
    z-index: 2001;
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

const StyledNextToolbar = styled.div`
  height: 100%;
  z-index: 1006;
  position: fixed;
  display: block;
  width: 73px;
  background: inherit;
  opacity: 1;
  // transition: all 0.3s;
  ${(props) => (props.left ? "left: 0" : "right: 0")};
  &:hover {
    cursor: pointer;
    opacity: 1;
    ${(props) =>
      props.left
        ? "background: linear-gradient(270deg,rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.5) 100%)"
        : "background: linear-gradient(270deg, rgba(0, 0, 0, 0.5) 0%, rgba(0, 0, 0, 0) 100%)"}
  }
`;

const StyledButtonScroll = styled.div`
  z-index: 1007;
  position: fixed;
  top: calc(50% - 20px);

  ${(props) => (props.orientation === "left" ? "left: 20px;" : "right: 20px;")}
  svg {
    path {
      fill: none;
    }
  }
`;

export {
  StyledViewerContainer,
  StyledViewer,
  StyledNextToolbar,
  StyledButtonScroll,
};
