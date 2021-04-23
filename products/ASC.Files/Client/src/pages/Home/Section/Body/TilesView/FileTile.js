import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";

import Tile from "./sub-components/Tile";
import FilesTileContent from "./FilesTileContent";
import { withRouter } from "react-router-dom";
import { createSelectable } from "react-selectable-fast";

import withFileActions from "../hoc/withFileActions";

const FilesTile = createSelectable((props) => {
  const {
    item,
    sectionWidth,
    dragging,
    onContentRowSelect,
    rowContextClick,
    onDrop,
    onMouseDown,
    className,
    isDragging,
    value,
    displayShareButton,
    isPrivacy,
    sharedButton,
    contextOptionsProps,
    checkedProps,
    element,
  } = props;

  return (
    <div ref={props.selectableRef}>
      <DragAndDrop
        data-title={item.title}
        value={value}
        className={className}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
        {...contextOptionsProps}
      >
        <Tile
          key={item.id}
          data={item}
          element={element}
          sectionWidth={sectionWidth}
          contentElement={sharedButton}
          onSelect={onContentRowSelect}
          rowContextClick={rowContextClick}
          isPrivacy={isPrivacy}
          {...checkedProps}
          {...contextOptionsProps}
          contextButtonSpacerWidth={displayShareButton}
        >
          <FilesTileContent item={item} sectionWidth={sectionWidth} />
        </Tile>
      </DragAndDrop>
    </div>
  );
});

export default withTranslation("Home")(withFileActions(withRouter(FilesTile)));
