import React from "react";
import { inject, observer } from "mobx-react";
import TableCell from "@docspace/components/table-container/TableCell";
import FileNameCell from "./FileNameCell";
import TypeCell from "./TypeCell";
import AuthorCell from "./AuthorCell";
import DateCell from "./DateCell";
import SizeCell from "./SizeCell";
import { classNames } from "@docspace/components/utils/classNames";
import {
  StyledBadgesContainer,
  StyledQuickButtonsContainer,
} from "../StyledTable";
import ErasureCell from "./ErasureCell";
import RoomCell from "./RoomCell";

const TrashRowDataComponent = (props) => {
  const {
    authorTrashColumnIsEnabled,
    createdTrashColumnIsEnabled,
    roomColumnIsEnabled,
    erasureColumnIsEnabled,
    sizeTrashColumnIsEnabled,
    typeTrashColumnIsEnabled,
    quickButtonsColumnIsEnabled,

    dragStyles,
    selectionProp,
    value,
    theme,
    onContentFileSelect,
    checkedProps,
    element,
    inProgress,
    showHotkeyBorder,
    badgesComponent,
    quickButtonsComponent,
  } = props;

  return (
    <>
      <TableCell
        {...dragStyles}
        className={classNames(
          selectionProp?.className,
          "table-container_file-name-cell"
        )}
        value={value}
      >
        <FileNameCell
          theme={theme}
          onContentSelect={onContentFileSelect}
          checked={checkedProps}
          element={element}
          inProgress={inProgress}
          {...props}
        />
        <StyledBadgesContainer showHotkeyBorder={showHotkeyBorder}>
          {badgesComponent}
        </StyledBadgesContainer>
      </TableCell>

      {roomColumnIsEnabled ? (
        <TableCell
          style={
            !roomColumnIsEnabled ? { background: "none" } : dragStyles.style
          }
          {...selectionProp}
        >
          <RoomCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}

      {authorTrashColumnIsEnabled ? (
        <TableCell
          style={
            !authorTrashColumnIsEnabled
              ? { background: "none" }
              : dragStyles.style
          }
          {...selectionProp}
        >
          <AuthorCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}

      {createdTrashColumnIsEnabled ? (
        <TableCell
          style={
            !createdTrashColumnIsEnabled
              ? { background: "none !important" }
              : dragStyles.style
          }
          {...selectionProp}
        >
          <DateCell
            create
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}

      {erasureColumnIsEnabled ? (
        <TableCell
          style={
            !erasureColumnIsEnabled ? { background: "none" } : dragStyles.style
          }
          {...selectionProp}
        >
          <ErasureCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}

      {sizeTrashColumnIsEnabled ? (
        <TableCell
          style={
            !sizeTrashColumnIsEnabled
              ? { background: "none" }
              : dragStyles.style
          }
          {...selectionProp}
        >
          <SizeCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}

      {typeTrashColumnIsEnabled ? (
        <TableCell
          style={
            !typeTrashColumnIsEnabled
              ? { background: "none !important" }
              : dragStyles.style
          }
          {...selectionProp}
        >
          <TypeCell
            sideColor={theme.filesSection.tableView.row.sideColor}
            {...props}
          />
        </TableCell>
      ) : (
        <div />
      )}
      {quickButtonsColumnIsEnabled ? (
        <TableCell
          style={
            !quickButtonsColumnIsEnabled
              ? { background: "none" }
              : dragStyles.style
          }
          {...selectionProp}
          className={classNames(
            selectionProp?.className,
            "table-container_quick-buttons-wrapper"
          )}
        >
          <StyledQuickButtonsContainer>
            {quickButtonsComponent}
          </StyledQuickButtonsContainer>
        </TableCell>
      ) : (
        <div />
      )}
    </>
  );
};

export default inject(({ tableStore }) => {
  const {
    authorTrashColumnIsEnabled,
    createdTrashColumnIsEnabled,
    roomColumnIsEnabled,
    erasureColumnIsEnabled,
    sizeTrashColumnIsEnabled,
    typeTrashColumnIsEnabled,
    quickButtonsColumnIsEnabled,
  } = tableStore;

  return {
    authorTrashColumnIsEnabled,
    createdTrashColumnIsEnabled,
    roomColumnIsEnabled,
    erasureColumnIsEnabled,
    sizeTrashColumnIsEnabled,
    typeTrashColumnIsEnabled,
    quickButtonsColumnIsEnabled,
  };
})(observer(TrashRowDataComponent));
