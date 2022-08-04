import React from "react";
import PropTypes from "prop-types";
import Viewer from "react-viewer";
import styled from "styled-components";

import MediaZoomInIcon from "../../../../../public/images/media.zoomin.react.svg";
import MediaZoomOutIcon from "../../../../../public/images/media.zoomout.react.svg";
import MediaRotateLeftIcon from "../../../../../public/images/media.rotateleft.react.svg";
import MediaRotateRightIcon from "../../../../../public/images/media.rotateright.react.svg";
import MediaResetIcon from "../../../../../public/images/media.reset.react.svg";
import MediaDeleteIcon from "../../../../../public/images/media.delete.react.svg";
import MediaDownloadIcon from "../../../../../public/images/media.download.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import MediaScrollButton from "./scroll-button";
import ControlBtn from "./control-btn";
import equal from "fast-deep-equal/react";
import { Base } from "@docspace/components/themes";

const StyledMediaZoomInIcon = styled(MediaZoomInIcon)`
  ${commonIconsStyles}
`;
const StyledMediaZoomOutIcon = styled(MediaZoomOutIcon)`
  ${commonIconsStyles}
`;
const StyledMediaRotateLeftIcon = styled(MediaRotateLeftIcon)`
  ${commonIconsStyles}
`;
const StyledMediaRotateRightIcon = styled(MediaRotateRightIcon)`
  ${commonIconsStyles}
`;
const StyledMediaResetIcon = styled(MediaResetIcon)`
  ${commonIconsStyles}
`;
const StyledMediaDeleteIcon = styled(MediaDeleteIcon)`
  ${commonIconsStyles}
`;
const StyledMediaDownloadIcon = styled(MediaDownloadIcon)`
  ${commonIconsStyles}
`;
const StyledViewer = styled(Viewer)`
  .react-viewer-footer {
    bottom: 5px !important;
    z-index: 301 !important;
    overflow: visible;
  }
  .react-viewer-canvas {
    z-index: 300 !important;
    margin-top: 50px;
  }
  .react-viewer-navbar,
  .react-viewer-mask,
  .react-viewer-attribute,
  .react-viewer-close {
    display: none;
  }
  .react-viewer-toolbar {
    position: relative;
    overflow: visible;
    bottom: 4px;
  }
  .react-viewer-toolbar li {
    width: 40px;
    height: 30px;
    margin-top: 4px;
    border-radius: 2px;
    cursor: pointer;
    line-height: 24px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
  .react-viewer-btn {
    background-color: transparent;
    &:hover {
      background-color: ${(props) =>
        props.theme.mediaViewer.imageViewer.backgroundColor};
    }
  }
  .react-viewer-image-transition {
    transition-duration: 0s;
  }
  li[data-key="prev"] {
    left: 20px;
  }
  li[data-key="next"] {
    right: 20px;
  }
  li[data-key="prev"],
  li[data-key="next"] {
    position: fixed;
    top: calc(50% - 20px);

    height: auto;
    background: none;

    &:hover {
      background: none;
    }
  }
  li[data-key="delete"],
  li[data-key="customDownload"] {
    position: fixed;
    @media (max-width: 600px) {
      position: initial;
    }
    bottom: 9px;
    .controlBtn {
      margin: 0;
    }
  }
  li[data-key="delete"] {
    right: 62px;
  }
  li[data-key="customDownload"] {
    right: 12px;
  }
  .iconContainer {
    width: 16px;
    height: 16px;
    line-height: 20px;
    margin: 3px auto;

    &.reset {
      width: 18px;
    }

    path,
    rect {
      fill: ${(props) => props.theme.mediaViewer.imageViewer.fill};
    }
  }

  .btnContainer {
    display: block;
    width: 16px;
    height: 16px;
    margin: 4px 12px;
    line-height: 19px;

    path,
    rect {
      fill: ${(props) => props.theme.mediaViewer.imageViewer.fill};
    }
  }
  .scrollBtn {
    cursor: ${(props) => (props.inactive ? "default" : "pointer")};
    opacity: ${(props) => (props.inactive ? "0.2" : "1")};
    &:hover {
      background-color: ${(props) =>
        !props.inactive
          ? props.theme.mediaViewer.imageViewer.backgroundColor
          : props.theme.mediaViewer.imageViewer.inactiveBackgroundColor};
    }
  }
`;

StyledViewer.defaultProps = { theme: Base };

var customToolbar = [
  {
    key: "zoomIn",
    actionType: 1,
    render: (
      <div className="iconContainer zoomIn">
        <StyledMediaZoomInIcon size="scale" />
      </div>
    ),
  },
  {
    key: "zoomOut",
    actionType: 2,
    render: (
      <div className="iconContainer zoomOut">
        <StyledMediaZoomOutIcon size="scale" />
      </div>
    ),
  },
  {
    key: "reset",
    actionType: 7,
    render: (
      <div className="iconContainer reset">
        <StyledMediaResetIcon size="scale" />
      </div>
    ),
  },
  {
    key: "rotateLeft",
    actionType: 5,
    render: (
      <div className="iconContainer rotateLeft">
        <StyledMediaRotateLeftIcon size="scale" />
      </div>
    ),
  },
  {
    key: "rotateRight",
    actionType: 6,
    render: (
      <div className="iconContainer rotateRight">
        <StyledMediaRotateRightIcon size="scale" />
      </div>
    ),
  },
  {
    key: "prev",
    actionType: 3,
    render: <MediaScrollButton orientation="right" className="scrollBtn" />,
  },
  {
    key: "next",
    actionType: 4,
    render: <MediaScrollButton orientation="left" className="scrollBtn" />,
  },
  {
    key: "delete",
    render: (
      <ControlBtn className="controlBtn">
        <div className="btnContainer">
          <StyledMediaDeleteIcon size="scale" />
        </div>
      </ControlBtn>
    ),
  },
  {
    key: "customDownload",
    render: (
      <ControlBtn className="controlBtn">
        <div className="btnContainer">
          <StyledMediaDownloadIcon size="scale" />
        </div>
      </ControlBtn>
    ),
  },
];

class ImageViewer extends React.Component {
  // componentDidUpdate() {
  //   document.getElementsByClassName("iconContainer reset").length > 0 &&
  //     document.getElementsByClassName("iconContainer reset")[0].click();
  // }

  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }
  render() {
    const {
      className,
      visible,
      images,
      inactive,
      onClose,
      userAccess,

      isFavoritesFolder,
    } = this.props;

    customToolbar.forEach((button) => {
      switch (button.key) {
        case "prev":
          button.onClick = this.props.onPrevClick;
          break;
        case "next":
          button.onClick = this.props.onNextClick;
          break;
        case "delete":
          button.onClick = this.props.onDeleteClick;
          break;
        case "customDownload":
          button.onClick = this.props.onDownloadClick;
          break;
        default:
          break;
      }
    });

    const toolbars =
      userAccess && !isFavoritesFolder
        ? customToolbar
        : customToolbar.filter((x) => x.key !== "delete");

    return (
      <div className={className}>
        <StyledViewer
          inactive={inactive}
          visible={visible}
          zoomSpeed={0.1}
          onMaskClick={onClose}
          customToolbar={() => toolbars}
          images={images}
          disableKeyboardSupport={true}
        />
      </div>
    );
  }
}

ImageViewer.propTypes = {
  className: PropTypes.string,
  visible: PropTypes.bool,
  inactive: PropTypes.bool,
  images: PropTypes.arrayOf(PropTypes.object),
  onNextClick: PropTypes.func,
  onPrevClick: PropTypes.func,
  onDeleteClick: PropTypes.func,
  onDownloadClick: PropTypes.func,
  onClose: PropTypes.func,
};

export default ImageViewer;
