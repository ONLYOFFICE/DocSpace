import React, { useState } from "react";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withQuickButtons from "../../../../../HOCs/withQuickButtons";
import withFileActions from "../../../../../HOCs/withFileActions";
import ItemIcon from "../../../../../components/ItemIcon";
import { withTranslation } from "react-i18next";
import { classNames } from "@docspace/components/utils/classNames";
import RoomsRowDataComponent from "./sub-components/RoomsRowData";
import TrashRowDataComponent from "./sub-components/TrashRowData";
import RowDataComponent from "./sub-components/RowData";
import { StyledTableRow, StyledDragAndDrop } from "./StyledTable";

const FilesTableRow = (props) => {
  const {
    t,
    fileContextClick,
    item,
    checkedProps,
    className,
    value,
    onMouseClick,
    dragging,
    isDragging,
    onDrop,
    onMouseDown,
    isActive,
    onHideContextMenu,
    onFilesClick,
    onDoubleClick,
    inProgress,
    index,
    setFirsElemChecked,
    setHeaderBorder,
    theme,
    getContextModel,
    showHotkeyBorder,
    id,
    isRooms,
    isTrashFolder,
    isHighlight,
    hideColumns,
  } = props;
  const { acceptBackground, background } = theme.dragAndDrop;

  const element = (
    <ItemIcon
      id={item.id}
      icon={item.icon}
      fileExst={item.fileExst}
      isRoom={item.isRoom}
      defaultRoomIcon={item.defaultRoomIcon}
    />
  );

  const selectionProp = {
    className: `files-item ${className} ${value}`,
    value,
  };

  const [isDragActive, setIsDragActive] = useState(false);

  const dragStyles = {
    style: {
      background:
        dragging && isDragging
          ? isDragActive
            ? acceptBackground
            : background
          : "none",
    },
  };

  const onDragOver = (dragActive) => {
    if (dragActive !== isDragActive) {
      setIsDragActive(dragActive);
    }
  };

  const onDragLeave = () => {
    setIsDragActive(false);
  };

  React.useEffect(() => {
    if (index === 0) {
      if (checkedProps || isActive) {
        setFirsElemChecked(true);
      } else {
        setFirsElemChecked(false);
      }
      if (showHotkeyBorder) {
        setHeaderBorder(true);
      } else {
        setHeaderBorder(false);
      }
    }
  }, [checkedProps, isActive, showHotkeyBorder]);

  const idWithFileExst = item.fileExst
    ? `${item.id}_${item.fileExst}`
    : item.id ?? "";

  return (
    <StyledDragAndDrop
      id={id}
      data-title={item.title}
      value={value}
      className={classNames("files-item", className, idWithFileExst, {
        ["table-hotkey-border"]: showHotkeyBorder,
        ["table-row-selected"]: !showHotkeyBorder && (checkedProps || isActive),
      })}
      onDrop={onDrop}
      onMouseDown={onMouseDown}
      dragging={dragging && isDragging}
      onDragOver={onDragOver}
      onDragLeave={onDragLeave}
    >
      <StyledTableRow
        className="table-row"
        {...dragStyles}
        isDragging={dragging}
        dragging={dragging && isDragging}
        selectionProp={selectionProp}
        key={item.id}
        fileContextClick={fileContextClick}
        onClick={onMouseClick}
        isActive={isActive}
        inProgress={inProgress}
        isFolder={item.isFolder}
        onHideContextMenu={onHideContextMenu}
        isThirdPartyFolder={item.isThirdPartyFolder}
        onDoubleClick={onDoubleClick}
        checked={checkedProps}
        contextOptions={item.contextOptions}
        getContextModel={getContextModel}
        showHotkeyBorder={showHotkeyBorder}
        title={
          item.isFolder
            ? t("Translations:TitleShowFolderActions")
            : t("Translations:TitleShowActions")
        }
        isRoom={item.isRoom}
        isHighlight={isHighlight}
        hideColumns={hideColumns}
      >
        {isRooms ? (
          <RoomsRowDataComponent
            element={element}
            dragStyles={dragStyles}
            {...props}
          />
        ) : isTrashFolder ? (
          <TrashRowDataComponent
            element={element}
            dragStyles={dragStyles}
            {...props}
          />
        ) : (
          <RowDataComponent
            element={element}
            dragStyles={dragStyles}
            selectionProp={selectionProp}
            {...props}
          />
        )}
      </StyledTableRow>
    </StyledDragAndDrop>
  );
};

export default withTranslation([
  "Files",
  "Common",
  "InfoPanel",
  "Notifications",
])(withFileActions(withContent(withQuickButtons(withBadges(FilesTableRow)))));
