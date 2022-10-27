import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import MediaZoomInIcon from "../../../../../public/images/media.zoomin.react.svg";
import MediaZoomOutIcon from "../../../../../public/images/media.zoomout.react.svg";
import MediaRotateLeftIcon from "../../../../../public/images/media.rotateleft.react.svg";
import MediaRotateRightIcon from "../../../../../public/images/media.rotateright.react.svg";
import MediaResetIcon from "../../../../../public/images/media.reset.react.svg";
import MediaDeleteIcon from "../../../../../public/images/media.delete.react.svg";
import MediaDownloadIcon from "../../../../../public/images/download.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import ViewerSeparator from "../../../../../public/images/viewer.separator.react.svg";
import MediaShare from "../../../../../public/images/share.react.svg";
import MediaContextMenu from "../../../../../public/images/vertical-dots.react.svg";

import DropDownItem from "@docspace/components/drop-down-item";
import DropDown from "@docspace/components/drop-down";
import equal from "fast-deep-equal/react";
import { Base } from "@docspace/components/themes";
import { Viewer } from "@docspace/components/viewer";

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
// const StyledMediaResetIcon = styled(MediaResetIcon)`
//   ${commonIconsStyles}
// `;
// const StyledMediaDeleteIcon = styled(MediaDeleteIcon)`
//   ${commonIconsStyles}
// `;
// const StyledMediaDownloadIcon = styled(MediaDownloadIcon)`
//   ${commonIconsStyles}
// `;

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
    width: 24px;
    height: 24px;
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
      title,
      onPrevClick,
      onNextClick,
      playlist,
      playlistPos,
      isImage,
      contextModel,
    } = this.props;

    const generateContextMenu = (isOpen) => {
      const model = contextModel();

      const onItemClick = (e, item) => {
        const { action, onClick } = item;

        return onClick({ originalEvent: e, action: action, item });
      };

      return (
        <DropDown
          open={isOpen}
          isDefaultMode={false}
          directionY="top"
          directionX="right"
          fixedDirection={true}
          withBackdrop={false}
          manualY="55px"
          manualX="30px"
        >
          {model.map((item) => {
            const onClick = (e) => onItemClick(e, item);
            return (
              <DropDownItem
                key={item.key}
                label={item.label}
                icon={item.icon ? item.icon : ""}
                action={item.action}
                onClick={onClick}
              />
            );
          })}
        </DropDown>
      );
    };

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
        key: "percent",
        noHover: true,
        actionType: 999,
        render: (
          <div className="iconContainer zoomPercent" style={{ width: "auto" }}>
            100%
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
        key: "separator",
        actionType: -1,
        noHover: true,
        render: (
          <div className="separatortest" style={{ height: "16px" }}>
            <ViewerSeparator size="scale" />
          </div>
        ),
      },
      {
        key: "share",
        actionType: 101,
        render: (
          <div className="iconContainer share" style={{ height: "16px" }}>
            <MediaShare size="scale" />
          </div>
        ),
      },
      {
        key: "download",
        actionType: 102,
        render: (
          <div className="iconContainer download" style={{ height: "16px" }}>
            <MediaDownloadIcon size="scale" />
          </div>
        ),
      },
      {
        key: "separator",
        actionType: -1,
        noHover: true,
        render: (
          <div className="separatortest" style={{ height: "16px" }}>
            <ViewerSeparator size="scale" />
          </div>
        ),
      },
      {
        key: "context-menu",
        actionType: -1,
        onClick: this.onMenuHandlerClick,
      },
    ];

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

    const toolbars = userAccess
      ? customToolbar
      : customToolbar.filter((x) => x.key !== "delete");

    return (
      <div className={className}>
        <Viewer
          inactive={inactive}
          visible={visible}
          zoomSpeed={0.25}
          title={title}
          contextModel={contextModel}
          generateContextMenu={generateContextMenu}
          isImage={isImage}
          onMaskClick={onClose}
          onNextClick={onNextClick}
          onPrevClick={onPrevClick}
          playlist={playlist}
          playlistPos={playlistPos}
          customToolbar={() => toolbars}
          images={images}
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
