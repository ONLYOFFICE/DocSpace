import React, {
  ForwardedRef,
  forwardRef,
  useImperativeHandle,
  useState,
} from "react";

import ImageViewerToolbarProps, {
  ImperativeHandle,
  ToolbarItemType,
} from "./ImageViewerToolbar.props";

import {
  ImageViewerToolbarWrapper,
  ListTools,
  ToolbarItem,
} from "./imageViewerToolbar.styled";

import MediaContextMenu from "PUBLIC_DIR/images/vertical-dots.react.svg";

function ImageViewerToolbar(
  {
    toolbar,
    className,
    percentValue,
    toolbarEvent,
    generateContextMenu,
    setIsOpenContextMenu,
  }: ImageViewerToolbarProps,
  ref: ForwardedRef<ImperativeHandle>
) {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [percent, setPercent] = useState<number>(() =>
    Math.round(percentValue * 100)
  );

  useImperativeHandle(
    ref,
    () => {
      return {
        setPercentValue(percent) {
          setPercent(Math.round(percent * 100));
        },
      };
    },
    []
  );

  function getContextMenu(item: ToolbarItemType) {
    const contextMenu = generateContextMenu(isOpen);
    return (
      <ToolbarItem
        style={{ position: "relative" }}
        key={item.key}
        onClick={() => {
          setIsOpenContextMenu((open) => !open);
          setIsOpen((open) => !open);
        }}
        data-key={item.key}
      >
        <div className="context" style={{ height: "16px" }}>
          <MediaContextMenu size="scale" />
        </div>
        {contextMenu}
      </ToolbarItem>
    );
  }

  function getPercentCompoent() {
    return (
      <div
        className="iconContainer zoomPercent"
        style={{ width: "auto", color: "#fff", userSelect: "none" }}
      >
        {`${percent}%`}
      </div>
    );
  }

  function renderToolbarItem(item: ToolbarItemType) {
    if (item.key === "context-menu") {
      return getContextMenu(item);
    }

    let content: JSX.Element | undefined = item?.render;

    if (item.key === "percent") {
      content = getPercentCompoent();
    }

    return (
      <ToolbarItem
        $percent={item.percent ? percent : 100}
        $isSeparator={item.actionType === -1}
        key={item.key}
        onClick={() => {
          toolbarEvent(item);
        }}
        data-key={item.key}
      >
        {content}
      </ToolbarItem>
    );
  }

  return (
    <ImageViewerToolbarWrapper className={className}>
      <ListTools>{toolbar.map((item) => renderToolbarItem(item))}</ListTools>
    </ImageViewerToolbarWrapper>
  );
}

export default forwardRef<ImperativeHandle, ImageViewerToolbarProps>(
  ImageViewerToolbar
);
