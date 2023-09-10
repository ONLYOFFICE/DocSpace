import React, { useMemo, memo, useCallback } from "react";
import equal from "fast-deep-equal/react";

import Viewer from "../Viewer";
import { isSeparator } from "../../helpers";
import {
  getCustomToolbar,
  getPDFToolbar,
} from "../../helpers/getCustomToolbar";
import { ContextMenuModel } from "../../types";

import { StyledDropDown } from "../StyledDropDown";
import { StyledDropDownItem } from "../StyledDropDownItem";
import ViewerWrapperProps from "./ViewerWrapper.props";

function ViewerWrapper(props: ViewerWrapperProps) {
  const onClickContextItem = useCallback((item: ContextMenuModel) => {
    if (isSeparator(item)) return;
    item.onClick();
  }, []);

  const generateContextMenu = (
    isOpen: boolean,
    right?: string,
    bottom?: string
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

  const toolbar = useMemo(() => {
    const {
      onDeleteClick,
      onDownloadClick,
      playlist,
      playlistPos,
      userAccess,
    } = props;

    const file = props.targetFile;
    const isEmptyContextMenu =
      props.contextModel().filter((item) => !item.disabled).length === 0;

    const customToolbar = props.isPdf
      ? getPDFToolbar()
      : file
      ? getCustomToolbar(
          file,
          isEmptyContextMenu,
          onDeleteClick,
          onDownloadClick
        )
      : [];

    const canShare = playlist[playlistPos].canShare;
    const toolbars =
      !canShare && userAccess
        ? customToolbar.filter(
            (x) => x.key !== "share" && x.key !== "share-separator"
          )
        : customToolbar.filter((x) => x.key !== "delete");

    return toolbars;
  }, [
    props.isPdf,
    props.onDeleteClick,
    props.onDownloadClick,
    props.playlist,
    props.playlistPos,
    props.userAccess,
    props.targetFile,
  ]);

  return (
    <Viewer
      title={props.title}
      fileUrl={props.fileUrl}
      isAudio={props.isAudio}
      isVideo={props.isVideo}
      isPdf={props.isPdf}
      visible={props.visible}
      isImage={props.isImage}
      playlist={props.playlist}
      inactive={props.inactive}
      audioIcon={props.audioIcon}
      errorTitle={props.errorTitle}
      headerIcon={props.headerIcon}
      targetFile={props.targetFile}
      toolbar={toolbar}
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
      someDialogIsOpen={props.someDialogIsOpen}
    />
  );
}

export default memo(ViewerWrapper, (prevProps, nextProps) =>
  equal(prevProps, nextProps)
);
