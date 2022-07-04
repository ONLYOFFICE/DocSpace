import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";

import FileNameCell from "./cells/RoomNameCell";
import TypeCell from "./cells/TypeCell";
import OwnerCell from "./cells/OwnerCell";
import DateCell from "./cells/DateCell";
import SizeCell from "./cells/SizeCell";
import TagsCell from "./cells/TagsCell";

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    height: 48px;
    max-height: 48px;
    min-height: 48px;

    background: ${(props) =>
      (props.isChecked || props.isHover) &&
      `${props.theme.filesSection.tableView.row.backgroundActive} !important`};
    cursor: ${(props) =>
      (props.isChecked || props.isHover) &&
      "url(/static/images/cursor.palm.react.svg), auto"};
  }

  &:hover {
    .room-name_cell {
      margin-left: -24px;
      padding-left: 24px;

      .room-logo_icon-container {
        display: none;
      }

      .room-logo_checkbox {
        display: flex;
      }
    }

    .table-container_row-context-menu-wrapper {
      margin-right: -20px;
      padding-right: 18px;
    }

    .table-container_cell {
      cursor: pointer;
      background: ${(props) =>
        `${props.theme.filesSection.tableView.row.backgroundActive} !important`};

      margin-top: -1px;
      border-top: ${(props) =>
        `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
    }
  }

  ${(props) =>
    (props.isHover || props.isChecked) &&
    css`
      .room-name_cell {
        .room-logo_icon-container {
          display: none;
        }

        .room-logo_checkbox {
          display: flex;
        }
      }
    `}
`;

const Row = React.forwardRef(
  (
    {
      item,
      tagCount,
      theme,

      isChecked,
      isHover,
      isMe,

      getRoomsContextOptions,
      selectRoom,
      openContextMenu,
      closeContextMenu,
      unpinRoom,
    },
    ref
  ) => {
    const onContextMenu = React.useCallback(
      (e) => {
        openContextMenu && openContextMenu(item);
      },
      [openContextMenu, item]
    );

    const onCloseContextMenu = React.useCallback(() => {
      closeContextMenu && closeContextMenu(item);
    }, [item, closeContextMenu]);

    const onRoomSelect = React.useCallback(
      (e) => {
        selectRoom && selectRoom(e.target.checked, item);
      },
      [selectRoom, item]
    );

    const getRoomsContextOptionsActions = React.useCallback(() => {
      return getRoomsContextOptions && getRoomsContextOptions(item);
    }, [getRoomsContextOptions, item]);

    const onClickUnpinRoomAction = React.useCallback(() => {
      unpinRoom && unpinRoom(item);
    }, [item, unpinRoom]);

    const onBadgeClick = React.useCallback(() => {
      console.log("on badge click");
    }, []);

    return (
      <StyledTableRow
        className={`table-row${
          isHover || isChecked ? " table-row-selected" : ""
        }`}
        key={item.id}
        contextOptions={getRoomsContextOptionsActions()}
        getContextModel={getRoomsContextOptionsActions}
        fileContextClick={onContextMenu}
        onHideContextMenu={onCloseContextMenu}
        isChecked={isChecked}
        isHover={isHover}
      >
        <FileNameCell
          theme={theme}
          label={item.title}
          type={item.roomType}
          isPrivacy={item.isPrivacy}
          isChecked={isChecked}
          pinned={item.pinned}
          badgeLabel={item.new}
          onRoomSelect={onRoomSelect}
          onClickUnpinRoom={onClickUnpinRoomAction}
          onBadgeClick={onBadgeClick}
        />
        <TypeCell
          type={item.roomType}
          sideColor={theme.filesSection.tableView.row.sideColor}
        />
        <TagsCell ref={ref} tags={item.tags} tagCount={tagCount} />
        <OwnerCell
          owner={item.createdBy}
          isMe={isMe}
          sideColor={theme.filesSection.tableView.row.sideColor}
        />
        <DateCell sideColor={theme.filesSection.tableView.row.sideColor} />
        <SizeCell sideColor={theme.filesSection.tableView.row.sideColor} />
      </StyledTableRow>
    );
  }
);

export default inject(({ auth, contextOptionsStore, roomsStore }, { item }) => {
  const { getRoomsContextOptions } = contextOptionsStore;

  const {
    selection,
    bufferSelection,
    selectRoom,
    openContextMenu,
    closeContextMenu,
    unpinRoom,
  } = roomsStore;

  const isChecked = !!selection.find((room) => room.id === item.id);
  const isHover = !isChecked && bufferSelection?.id === item.id;
  const isMe = item.createdBy.id === auth.userStore.user.id;

  return {
    isChecked,
    isHover,
    isMe,

    getRoomsContextOptions,
    selectRoom,
    openContextMenu,
    closeContextMenu,
    unpinRoom,
  };
})(observer(Row));
