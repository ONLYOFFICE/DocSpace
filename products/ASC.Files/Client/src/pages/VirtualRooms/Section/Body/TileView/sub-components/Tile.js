import React from "react";

import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import Tags from "@appserver/common/components/Tags";
import Heading from "@appserver/components/heading";
import Badge from "@appserver/components/badge";
import ContextMenuButton from "@appserver/components/context-menu-button";
import RoomLogo from "@appserver/components/room-logo";
import ContextMenu from "@appserver/components/context-menu";
import { Base } from "@appserver/components/themes";

import RoomsBadges from "../../../../components/Badges";
import { FolderType } from "@appserver/common/constants";

const StyledTile = styled.div`
  border: 1px solid #eceef1;

  border-radius: 12px;

  box-sizing: border-box;

  cursor: pointer;

  &:hover {
    .tile-header {
      background: ${(props) =>
        props.theme.filesSection.tilesView.tile.checkedColor} !important;

      .room-logo_icon-container {
        display: none;
      }

      .room-logo_checkbox {
        display: flex;
      }
    }
  }

  ${(props) =>
    (props.isActive || props.isChecked) &&
    css`
      .tile-header {
        background: ${(props) =>
          props.theme.filesSection.tilesView.tile.checkedColor} !important;

        .room-logo_icon-container {
          display: none !important;
        }

        .room-logo_checkbox {
          display: flex !important;
        }
      }
    `}
`;

const StyledHeader = styled.div`
  width: 100%;
  height: 64px;

  box-sizing: border-box;

  padding: 16px;

  border-bottom: 1px solid #eceef1;

  border-radius: 12px 12px 0 0;

  display: grid;
  align-items: center;

  grid-template-columns: ${(props) =>
    props.withBadge || props.isPinned
      ? "auto 1fr auto auto"
      : "auto 1fr  auto"};

  .tile-header_heading {
    font-size: 16px;
    line-height: 22px;

    margin-right: 12px;
  }

  .tile-header_context-menu-button {
    margin: 0 6px;
  }
`;

StyledHeader.defaultProps = { theme: Base };

const StyledContent = styled.div`
  width: 100%;

  box-sizing: border-box;

  padding: 16px;

  display: flex;

  flex-direction: column;

  overflow: hidden;
`;

const Tile = React.forwardRef(
  (
    {
      item,
      roomType,
      isPrivacy,
      title,
      pinned,
      badge,
      tags,
      isArchive,

      columnCount,

      isChecked,
      isActive,

      onClickPinRoom,
      getRoomsContextOptions,
      selectRoom,
      openContextMenu,
      closeContextMenu,

      onOpenRoom,
      onSelectRoom,
      onSelectTag,

      history,
    },
    forwardRef
  ) => {
    const tileRef = React.useRef(null);
    const cmRef = React.useRef(null);

    const onBadgeClick = React.useCallback(() => {
      console.log("on badge click");
    }, []);

    const onContextMenu = React.useCallback(
      (e) => {
        if (!cmRef.current.menuRef.current) {
          tileRef.current.click(e);
        }
        cmRef.current.show(e);
        openContextMenu && openContextMenu(item);
      },
      [cmRef.current, tileRef.current, openContextMenu, item]
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

    const onOpenRoomAction = React.useCallback(
      (e) => {
        if (isArchive) return;
        onOpenRoom && onOpenRoom(e, item.id, history);
      },
      [onOpenRoom, item.id, history, isArchive]
    );

    return (
      <StyledTile
        ref={tileRef}
        isChecked={isChecked}
        isActive={isActive}
        onContextMenu={onContextMenu}
        onClick={onOpenRoomAction}
      >
        <StyledHeader
          className="tile-header"
          withBadge={!!badge}
          isPinned={pinned}
        >
          <RoomLogo
            className={"tile-header_logo"}
            type={roomType}
            withCheckbox={true}
            isChecked={isChecked}
            isIndeterminate={false}
            isArchive={isArchive}
            onChange={onRoomSelect}
          />

          <Heading className="tile-header_heading" truncate>
            {title}
          </Heading>

          {(!!badge || pinned) && (
            <RoomsBadges
              badge={badge}
              pinned={pinned}
              onClickPinRoom={onClickPinRoom}
              onBadgeClick={onBadgeClick}
            />
          )}

          <ContextMenuButton
            className="tile-header_context-menu-button"
            getData={getRoomsContextOptionsActions}
            directionX="right"
            isNew={true}
            onClick={onContextMenu}
            onClose={onCloseContextMenu}
          />

          <ContextMenu
            getContextModel={getRoomsContextOptionsActions}
            ref={cmRef}
            withBackdrop={true}
            onHide={onCloseContextMenu}
          />
        </StyledHeader>
        <StyledContent className="virtual-rooms_tile-content" ref={forwardRef}>
          {columnCount && (
            <Tags
              className="virtual-rooms_tile-content-tags"
              tags={tags}
              columnCount={columnCount}
              onSelectTag={onSelectTag}
            />
          )}
        </StyledContent>
      </StyledTile>
    );
  }
);

export default inject(
  ({ contextOptionsStore, roomsStore, roomsActionsStore }, { item }) => {
    const { onClickPinRoom, getRoomsContextOptions } = contextOptionsStore;

    const {
      selection,
      bufferSelection,
      selectRoom,
      openContextMenu,
      closeContextMenu,
    } = roomsStore;

    const { onOpenRoom, onSelectRoom, onSelectTag } = roomsActionsStore;

    const isChecked = !!selection.find((room) => room.id === item.id);
    const isActive = !isChecked && bufferSelection?.id === item.id;

    return {
      isChecked,
      isActive,

      onOpenRoom,
      onSelectRoom,
      onSelectTag,

      onClickPinRoom,
      getRoomsContextOptions,
      selectRoom,
      openContextMenu,
      closeContextMenu,
    };
  }
)(observer(Tile));
