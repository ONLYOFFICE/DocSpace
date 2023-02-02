import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import MediaZoomInIcon from "PUBLIC_DIR/images/media.zoomin.react.svg";
import MediaZoomOutIcon from "PUBLIC_DIR/images/media.zoomout.react.svg";
import MediaRotateLeftIcon from "PUBLIC_DIR/images/media.rotateleft.react.svg";
import MediaRotateRightIcon from "PUBLIC_DIR/images/media.rotateright.react.svg";
import MediaDeleteIcon from "PUBLIC_DIR/images/media.delete.react.svg";
import MediaDownloadIcon from "PUBLIC_DIR/images/download.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import MediaFavoriteIcon from "PUBLIC_DIR/images/favorite.react.svg";

import ViewerSeparator from "PUBLIC_DIR/images/viewer.separator.react.svg";
import MediaShare from "PUBLIC_DIR/images/share.react.svg";

import DropDownItem from "@docspace/components/drop-down-item";
import DropDown from "@docspace/components/drop-down";
import equal from "fast-deep-equal/react";
import { Base } from "@docspace/components/themes";
import { Viewer } from "@docspace/components/viewer";

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

const StyledDropDown = styled(DropDown)`
  background: #333;
`;

const StyledDropDownItem = styled(DropDownItem)`
  color: #fff;

  .drop-down-item_icon svg {
    path {
      fill: #fff !important;
    }
  }

  /* .is-separator {
    height: 1px;
    background: #474747;
  } */

  &:hover {
    background: #444;
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
      isAudio,
      isVideo,
      isPreviewFile,
      archiveRoom,
      contextModel,
      audioIcon,
      headerIcon,
      onSetSelectionFile,
      onDownloadClick,
    } = this.props;

    const generateContextMenu = (isOpen, right, bottom) => {
      const model = contextModel();

      const onItemClick = (e, item) => {
        const { action, onClick } = item;

        return onClick({ originalEvent: e, action: action, item });
      };

      return (
        <StyledDropDown
          open={isOpen}
          isDefaultMode={false}
          directionY="top"
          directionX="right"
          fixedDirection={true}
          withBackdrop={false}
          manualY={(bottom || "63") + "px"}
          manualX={(right || "-31") + "px"}
        >
          {model.map((item) => {
            if (item.disabled) return;

            const onClick = (e) => {
              onClose();
              onItemClick(e, item);
            };
            return (
              <StyledDropDownItem
                className={`${item.isSeparator ? "is-separator" : ""}`}
                key={item.key}
                label={item.label}
                icon={item.icon ? item.icon : ""}
                action={item.action}
                onClick={onClick}
              />
            );
          })}
        </StyledDropDown>
      );
    };

    var customToolbar = [
      {
        key: "zoomOut",
        percent: true,
        actionType: 2,
        render: (
          <div className="iconContainer zoomOut">
            <MediaZoomOutIcon size="scale" />
          </div>
        ),
      },
      {
        key: "percent",
        actionType: 999,
      },
      {
        key: "zoomIn",
        actionType: 1,
        render: (
          <div className="iconContainer zoomIn">
            <MediaZoomInIcon size="scale" />
          </div>
        ),
      },
      {
        key: "rotateLeft",
        actionType: 5,
        render: (
          <div className="iconContainer rotateLeft">
            <MediaRotateLeftIcon size="scale" />
          </div>
        ),
      },
      {
        key: "rotateRight",
        actionType: 6,
        render: (
          <div className="iconContainer rotateRight">
            <MediaRotateRightIcon size="scale" />
          </div>
        ),
      },
      {
        key: "separator download-separator",
        actionType: -1,
        noHover: true,
        render: (
          <div className="separator" style={{ height: "16px" }}>
            <ViewerSeparator size="scale" />
          </div>
        ),
      },
      // {
      //   key: "share",
      //   actionType: 101,
      //   render: (
      //     <div className="iconContainer share" style={{ height: "16px" }}>
      //       <MediaShare size="scale" />
      //     </div>
      //   ),
      // },
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
        key: "context-separator",
        actionType: -1,
        noHover: true,
        render: (
          <div className="separator" style={{ height: "16px" }}>
            <ViewerSeparator size="scale" />
          </div>
        ),
      },
      {
        key: "context-menu",
        actionType: -1,
      },
      {
        key: "delete",
        actionType: 103,
        render: (
          <div className="iconContainer viewer-delete">
            <MediaDeleteIcon size="scale" />
          </div>
        ),
      },
      {
        key: "favorite",
        actionType: 104,
        render: (
          <div className="iconContainer viewer-favorite">
            <MediaFavoriteIcon size="scale" />
          </div>
        ),
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
        case "download":
          button.onClick = onDownloadClick;
          break;
        default:
          break;
      }
    });

    const canShare = playlist[playlistPos].canShare;
    const toolbars =
      !canShare && userAccess
        ? customToolbar.filter(
            (x) => x.key !== "share" && x.key !== "share-separator"
          )
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
          headerIcon={headerIcon}
          isAudio={isAudio}
          isVideo={isVideo}
          isPreviewFile={isPreviewFile}
          archiveRoom={archiveRoom}
          audioIcon={audioIcon}
          onSetSelectionFile={onSetSelectionFile}
          onMaskClick={onClose}
          onNextClick={onNextClick}
          onPrevClick={onPrevClick}
          onDownloadClick={onDownloadClick}
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
