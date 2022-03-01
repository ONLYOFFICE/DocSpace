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
import withBadges from "../../../../../HOCs/withBadges";

const FileTile = (props) => {
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
    contextOptionsProps,
    checkedProps,
    getIcon,
    onFilesClick,
    onMouseClick,
    isActive,
    isEdit,
    inProgress,
    quickButtonsComponent,
    badgesComponent,
    t,
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
          inProgress={inProgress}
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
          {badgesComponent}
        </Tile>
      </DragAndDrop>
    </div>
  );
};

export default inject(({ settingsStore }) => {
  const { getIcon } = settingsStore;
  return { getIcon };
})(
  withTranslation(["Home", "VersionBadge"])(
    withFileActions(
      withContextOptions(
        withRouter(withBadges(withQuickButtons(observer(FileTile))))
      )
    )
  )
);
