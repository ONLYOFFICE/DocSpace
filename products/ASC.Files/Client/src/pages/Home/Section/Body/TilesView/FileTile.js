import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";

import Tile from "./sub-components/Tile";
import FilesTileContent from "./FilesTileContent";
import { withRouter } from "react-router-dom";

import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";

const FilesTile = (props) => {
  const {
    t,
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
    //sharedButton,
    contextOptionsProps,
    checkedProps,
    //element,
    getIcon,
    onFilesClick,
    onMouseClick,
    showShare,
    isActive,
    isEdit,
    quickButtonsComponent,
  } = props;

  const temporaryExtension =
    item.id === -1 ? `.${item.fileExst}` : item.fileExst;

  const temporaryIcon = getIcon(
    96,
    temporaryExtension,
    item.providerKey,
    item.contentLength
  );

  const { thumbnailUrl } = item;
  const element = (
    <ItemIcon id={item.id} icon={item.icon} fileExst={item.fileExst} />
  );

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
          contentElement={quickButtonsComponent}
          onSelect={onContentFileSelect}
          tileContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          dragging={dragging && isDragging}
          onClick={onMouseClick}
          thumbnailClick={onFilesClick}
          onDoubleClick={onFilesClick}
          checked={checkedProps}
          {...contextOptionsProps}
          contextButtonSpacerWidth={displayShareButton}
          isActive={isActive}
          isEdit={isEdit}
          title={
            item.isFolder
              ? t("Translations:TitleShowFolderActions")
              : t("Translations:TitleShowActions")
          }
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
    withFileActions(
      withContextOptions(withRouter(withQuickButtons(observer(FilesTile))))
    )
  )
);
