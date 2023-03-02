import React from "react";

import MediaZoomInIcon from "PUBLIC_DIR/images/media.zoomin.react.svg";
import MediaZoomOutIcon from "PUBLIC_DIR/images/media.zoomout.react.svg";
import MediaRotateLeftIcon from "PUBLIC_DIR/images/media.rotateleft.react.svg";
import MediaRotateRightIcon from "PUBLIC_DIR/images/media.rotateright.react.svg";
import MediaDeleteIcon from "PUBLIC_DIR/images/media.delete.react.svg";
import MediaDownloadIcon from "PUBLIC_DIR/images/download.react.svg";
// import MediaFavoriteIcon from "PUBLIC_DIR/images/favorite.react.svg";
import ViewerSeparator from "PUBLIC_DIR/images/viewer.separator.react.svg";
import { ActionType } from ".";

export const getCustomToolbar = (
  onDeleteClick: VoidFunction,
  onDownloadClick: VoidFunction
) => {
  return [
    {
      key: "zoomOut",
      percent: true,
      actionType: ActionType.ZoomOut,
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
      actionType: ActionType.ZoomIn,
      render: (
        <div className="iconContainer zoomIn">
          <MediaZoomInIcon size="scale" />
        </div>
      ),
    },
    {
      key: "rotateLeft",
      actionType: ActionType.RotateLeft,
      render: (
        <div className="iconContainer rotateLeft">
          <MediaRotateLeftIcon size="scale" />
        </div>
      ),
    },
    {
      key: "rotateRight",
      actionType: ActionType.RotateRight,
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
    {
      key: "download",
      actionType: ActionType.Download,
      render: (
        <div className="iconContainer download" style={{ height: "16px" }}>
          <MediaDownloadIcon size="scale" />
        </div>
      ),
      onClick: onDownloadClick,
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
      onClick: onDeleteClick,
    },
  ];
};
