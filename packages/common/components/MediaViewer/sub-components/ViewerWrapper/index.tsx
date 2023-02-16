import React, { useMemo, memo, useCallback } from "react";
import equal from "fast-deep-equal/react";

import { Viewer } from "@docspace/components/viewer";
import { isSeparator } from "../../helpers";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";
import { ContextMenuModel } from "../../types";

import { StyledDropDown } from "../StyledDropDown";
import { StyledDropDownItem } from "../StyledDropDownItem";
import ViewerWrapperProps from "./ViewerWrapper.props";

const DefaultSpeedZoom = 0.25;

function ViewerWrapper(props: ViewerWrapperProps) {
  const onClickContextItem = useCallback(
    (item: ContextMenuModel) => {
      if (isSeparator(item)) return;
      item.onClick();
      props.onClose();
    },
    [props.onClose]
  );

  const generateContextMenu = (
    isOpen: boolean,
    right: string,
    bottom: string
  ) => {
    const model = props.contextModel();

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
          const isItemSeparator = isSeparator(item);

          return (
            <StyledDropDownItem
              className={`${item.isSeparator ? "is-separator" : ""}`}
              key={item.key}
              label={isItemSeparator ? undefined : item.label}
              icon={!isItemSeparator && item.icon ? item.icon : ""}
              onClick={() => onClickContextItem(item)}
            />
          );
        })}
      </StyledDropDown>
    );
  };

  const toolbars = useMemo(() => {
    const {
      onDeleteClick,
      onDownloadClick,
      playlist,
      playlistPos,
      userAccess,
    } = props;

    const customToolbar = getCustomToolbar(onDeleteClick, onDownloadClick);

    const canShare = playlist[playlistPos].canShare;
    const toolbars =
      !canShare && userAccess
        ? customToolbar.filter(
            (x) => x.key !== "share" && x.key !== "share-separator"
          )
        : customToolbar.filter((x) => x.key !== "delete");

    return toolbars;
  }, [
    props.onDeleteClick,
    props.onDownloadClick,
    props.playlist,
    props.playlistPos,
    props.userAccess,
  ]);

  return (
    <Viewer
      title={props.title}
      images={props.images}
      isAudio={props.isAudio}
      isVideo={props.isVideo}
      visible={props.visible}
      isImage={props.isImage}
      playlist={props.playlist}
      inactive={props.inactive}
      audioIcon={props.audioIcon}
      zoomSpeed={DefaultSpeedZoom}
      errorTitle={props.errorTitle}
      headerIcon={props.headerIcon}
      customToolbar={() => toolbars}
      playlistPos={props.playlistPos}
      archiveRoom={props.archiveRoom}
      isPreviewFile={props.isPreviewFile}
      onMaskClick={props.onClose}
      onNextClick={props.onNextClick}
      onPrevClick={props.onPrevClick}
      contextModel={props.contextModel}
      onDownloadClick={props.onDownloadClick}
      generateContextMenu={generateContextMenu}
      onSetSelectionFile={props.onSetSelectionFile}
    />
  );
}

export default memo(ViewerWrapper, (prevProps, nextProps) =>
  equal(prevProps, nextProps)
);
