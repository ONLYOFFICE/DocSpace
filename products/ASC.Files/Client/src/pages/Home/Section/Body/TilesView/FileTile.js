import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";

import Tile from "./sub-components/Tile";
import FilesTileContent from "./FilesTileContent";
import { withRouter } from "react-router-dom";

import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";

const FilesTile = (props) => {
  const {
    item,
    sectionWidth,
    dragging,
    onContentFileSelect,
    fileContextClick,
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
    getIcon,
    onFilesClick,
    onMouseUp,
  } = props;
  const temporaryIcon = getIcon(
    96,
    item.fileExst,
    item.providerKey,
    item.contentLength
  );

  const { thumbnailUrl } = item;

  return (
    <div ref={props.selectableRef}>
      <DragAndDrop
        data-title={item.title}
        value={value}
        className={`files-item ${className}`}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
        {...contextOptionsProps}
      >
        <Tile
          key={item.id}
          item={item}
          temporaryIcon={temporaryIcon}
          thumbnail={thumbnailUrl}
          element={element}
          sectionWidth={sectionWidth}
          contentElement={sharedButton}
          onSelect={onContentFileSelect}
          tileContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          dragging={dragging && isDragging}
          onMouseUp={onMouseUp}
          thumbnailClick={onFilesClick}
          onDoubleClick={onFilesClick}
          {...checkedProps}
          {...contextOptionsProps}
          contextButtonSpacerWidth={displayShareButton}
        >
          <FilesTileContent
            item={item}
            sectionWidth={sectionWidth}
            onFilesClick={onFilesClick}
          />
        </Tile>
      </DragAndDrop>
    </div>
  );
};

export default inject(({ formatsStore }) => {
  const { getIcon } = formatsStore.iconFormatsStore;
  return { getIcon };
})(
  withTranslation("Home")(
    withFileActions(withContextOptions(withRouter(observer(FilesTile))))
  )
);
