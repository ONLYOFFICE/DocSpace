import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { isMobile } from "react-device-detect";

import Row from "@appserver/components/row";
import RoomLogo from "@appserver/components/room-logo";

import { Base } from "@appserver/components/themes";

import RoomsRowContent from "./RoomsRowContent";

const marginStyles = css`
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  ${isMobile &&
  css`
    margin-left: -20px;
    margin-right: -20px;
    padding-left: 20px;
    padding-right: 20px;
  `}

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }

  @media (max-width: 375px) {
    margin-left: -16px;
    margin-right: -8px;
    padding-left: 16px;
    padding-right: 8px;
  }
`;

const StyledRoomsRowWrapper = styled.div`
  .row-drag-wrapper {
    position: relative;
    outline: none;
    background: none;
    border: 1px solid transparent;
    border-left: none;
    border-right: none;
    margin-left: 0;
  }
`;

const StyledRoomsRow = styled(Row)`
  margin-top: -2px;

  position: unset;

  .styled-element {
    margin-right: 0;
  }

  .room-row_logo {
    height: 56px;
    max-height: 56px;
  }

  :hover {
    background: ${(props) =>
      props.theme.filesSection.rowView.checkedBackground};

    margin-top: -3px;
    border-top: ${(props) =>
      `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};

    .room-logo_icon-container {
      display: none;
    }

    .room-logo_checkbox {
      display: flex;
    }

    ${marginStyles}
  }

  ${(props) =>
    (props.isChecked || props.isActive) &&
    css`
      background: ${(props) =>
        props.theme.filesSection.rowView.checkedBackground};

      margin-top: -3px;
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};

      .room-logo_icon-container {
        display: none;
      }

      .room-logo_checkbox {
        display: flex;
      }

      ${marginStyles}
    `}
`;

StyledRoomsRow.defaultProps = { theme: Base };

const RoomsRow = ({
  item,
  selectRoom,
  isChecked,
  isActive,
  sectionWidth,
  openContextMenu,
  closeContextMenu,
  getRoomsContextOptions,
}) => {
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

  const element = (
    <RoomLogo
      className="room-row_logo"
      type={item.roomType}
      isPrivacy={false}
      isArchive={false}
      withCheckbox={true}
      isChecked={isChecked}
      isIndeterminate={false}
      onChange={onRoomSelect}
    />
  );

  return (
    <StyledRoomsRowWrapper
      id={item.id}
      className={`row-wrapper ${isChecked || isActive ? "row-selected" : ""}`}
    >
      <div className="row-drag-wrapper">
        <StyledRoomsRow
          key={item.id}
          className={`rooms-row`}
          data={item}
          isEdit={false}
          element={element}
          sectionWidth={sectionWidth}
          isActive={isActive}
          isChecked={isChecked}
          rowContextClick={onContextMenu}
          rowContextClose={onCloseContextMenu}
          contextOptions={getRoomsContextOptionsActions()}
          getContextModel={getRoomsContextOptionsActions}
        >
          <RoomsRowContent {...item} sectionWidth={sectionWidth} />
        </StyledRoomsRow>
      </div>
    </StyledRoomsRowWrapper>
  );
};

export default inject(({ roomsStore, contextOptionsStore }, { item }) => {
  const {
    selectRoom,
    selection,
    bufferSelection,
    openContextMenu,
    closeContextMenu,
  } = roomsStore;

  const { getRoomsContextOptions } = contextOptionsStore;

  const isChecked = !!selection.find((room) => room.id === item.id);
  const isActive = !isChecked && bufferSelection?.id === item.id;

  return {
    isChecked,
    isActive,
    selectRoom,
    getRoomsContextOptions,
    openContextMenu,
    closeContextMenu,
  };
})(observer(RoomsRow));
