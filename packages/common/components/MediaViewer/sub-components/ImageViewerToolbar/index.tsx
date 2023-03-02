import React, { useState } from "react";

import ImageViewerToolbarProps, {
  ToolbarItemType,
} from "./ImageViewerToolbar.props";

import {
  ImageViewerToolbarWrapper,
  ListTools,
  ToolbarItem,
} from "./imagleViewerToolbar.styled";

import MediaContextMenu from "PUBLIC_DIR/images/vertical-dots.react.svg";

function ImageViewerToolbar({
  toolbar,
  percent,
  ToolbarEvent,
  generateContextMenu,
  setIsOpenContextMenu,
}: ImageViewerToolbarProps) {
  const [isOpen, setIsOpen] = useState<boolean>(false);

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
          ToolbarEvent(item);
        }}
        data-key={item.key}
      >
        {content}
      </ToolbarItem>
    );
  }

  return (
    <ImageViewerToolbarWrapper>
      <ListTools>{toolbar.map((item) => renderToolbarItem(item))}</ListTools>
    </ImageViewerToolbarWrapper>
  );
}

export default ImageViewerToolbar;
