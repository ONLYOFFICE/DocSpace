import React from "react";
import PropTypes from "prop-types";

import Viewer from "react-viewer";
import { Icons } from "asc-web-components";
import styled from "styled-components";
import MediaScrollButton from "./scroll-button";
import ControlBtn from "./control-btn";

const StyledViewer = styled(Viewer)`
  .react-viewer-footer {
    bottom: 5px !important;
    z-index: 4001 !important;
    overflow: visible;
  }
  .react-viewer-canvas {
    z-index: 4000 !important;
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
      background-color: rgba(200, 200, 200, 0.2);
    }
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
    bottom: 10px;
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
    width: 20px;
    line-height: 20px;
    margin: 4px auto;

    &.reset {
      width: 18px;
    }
  }

  .btnContainer {
    display: block;
    width: 18px;
    margin: 4px 10px;
    line-height: 19px;
  }
  .scrollBtn {
    cursor: ${(props) => (props.inactive ? "default" : "pointer")};
    opacity: ${(props) => (props.inactive ? "0.2" : "1")};
    &:hover {
      background-color: ${(props) =>
        !props.inactive ? "rgba(200, 200, 200, 0.2)" : "rgba(11,11,11,0.7)"};
    }
  }
`;

var customToolbar = [
  {
    key: "zoomIn",
    actionType: 1,
    render: (
      <div className="iconContainer zoomIn">
        <Icons.MediaZoomInIcon size="scale" />
      </div>
    ),
  },
  {
    key: "zoomOut",
    actionType: 2,
    render: (
      <div className="iconContainer zoomOut">
        <Icons.MediaZoomOutIcon size="scale" />
      </div>
    ),
  },
  {
    key: "reset",
    actionType: 7,
    render: (
      <div className="iconContainer reset">
        <Icons.MediaResetIcon size="scale" />
      </div>
    ),
  },
  {
    key: "rotateLeft",
    actionType: 5,
    render: (
      <div className="iconContainer rotateLeft">
        <Icons.MediaRotateLeftIcon size="scale" />
      </div>
    ),
  },
  {
    key: "rotateRight",
    actionType: 6,
    render: (
      <div className="iconContainer rotateRight">
        <Icons.MediaRotateRightIcon size="scale" />
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
          <Icons.MediaDeleteIcon size="scale" />
        </div>
      </ControlBtn>
    ),
  },
  {
    key: "customDownload",
    render: (
      <ControlBtn className="controlBtn">
        <div className="btnContainer">
          <Icons.MediaDownloadIcon size="scale" />
        </div>
      </ControlBtn>
    ),
  },
];

class ImageViewer extends React.Component {
  componentDidUpdate() {
    document.getElementsByClassName("iconContainer reset").length > 0 &&
      document.getElementsByClassName("iconContainer reset")[0].click();
  }

  render() {
    const { className, visible, images, inactive, onClose } = this.props;

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
    return (
      <div className={className}>
        <StyledViewer
          inactive={inactive}
          visible={visible}
          zoomSpeed={0.1}
          onMaskClick={onClose}
          customToolbar={(toolbars) => {
            return customToolbar;
          }}
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
