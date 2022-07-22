import { Base } from "@docspace/components/themes";
import styled from "styled-components";

const StyledMediaViewer = styled.div`
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
    z-index: 301;
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
    z-index: 302;
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
`;

StyledMediaViewer.defaultProps = { theme: Base };

export default StyledMediaViewer;
