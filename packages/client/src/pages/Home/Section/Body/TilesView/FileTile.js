import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@docspace/components/drag-and-drop";

import Tile from "./sub-components/Tile";
import FilesTileContent from "./FilesTileContent";
import { withRouter } from "react-router-dom";

import withFileActions from "../../../../../HOCs/withFileActions";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import ItemIcon from "../../../../../components/ItemIcon";
import withBadges from "../../../../../HOCs/withBadges";

const StyledDragAndDrop = styled(DragAndDrop)`
  border-radius: 6px;
`;

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
    checkedProps,
    getIcon,
    onFilesClick,
    onMouseClick,
    isActive,
    isEdit,
    inProgress,
    quickButtonsComponent,
    showHotkeyBorder,
    badgesComponent,
    t,
    getContextModel,
    onHideContextMenu,
    thumbSize,
    setSelection,
    id,
    onSelectTag,
    onSelectOption,
    columnCount,
    isRooms,
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
    <ItemIcon
      id={item.id}
      icon={item.icon}
      fileExst={item.fileExst}
      isRoom={item.isRoom}
    />
  );

  return (
    <div ref={props.selectableRef} id={id}>
      <StyledDragAndDrop
        data-title={item.title}
        value={value}
        className={`files-item ${className} ${item.id}_${item.fileExst}`}
        onDrop={onDrop}
        onMouseDown={onMouseDown}
        dragging={dragging && isDragging}
        contextOptions={item.contextOptions}
      >
        <Tile
          key={item.id}
          item={item}
          temporaryIcon={temporaryIcon}
          thumbnail={
            thumbnailUrl && thumbSize
              ? `${thumbnailUrl}&size=${thumbSize}`
              : thumbnailUrl
          }
          element={element}
          sectionWidth={sectionWidth}
          contentElement={quickButtonsComponent}
          onSelect={onContentFileSelect}
          tileContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          isDragging={dragging}
          dragging={dragging && isDragging}
          onClick={onMouseClick}
          thumbnailClick={onFilesClick}
          onDoubleClick={onFilesClick}
          checked={checkedProps}
          contextOptions={item.contextOptions}
          contextButtonSpacerWidth={displayShareButton}
          isActive={isActive}
          inProgress={inProgress}
          isEdit={isEdit}
          getContextModel={getContextModel}
          hideContextMenu={onHideContextMenu}
          t={t}
          showHotkeyBorder={showHotkeyBorder}
          setSelection={setSelection}
          selectTag={onSelectTag}
          selectOption={onSelectOption}
          columnCount={columnCount}
          isRooms={isRooms}
        >
          <FilesTileContent
            item={item}
            sectionWidth={sectionWidth}
            onFilesClick={onFilesClick}
          />
          {badgesComponent}
        </Tile>
      </StyledDragAndDrop>
    </div>
  );
};

export default inject(({ settingsStore, filesStore, treeFoldersStore }) => {
  const { getIcon } = settingsStore;
  const { setSelection } = filesStore;

  const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

  const isRooms = isRoomsFolder || isArchiveFolder;

  return { getIcon, setSelection, isRooms };
})(
  withTranslation(["Files", "InfoPanel"])(
    withRouter(
      withFileActions(withBadges(withQuickButtons(observer(FileTile))))
    )
  )
);
