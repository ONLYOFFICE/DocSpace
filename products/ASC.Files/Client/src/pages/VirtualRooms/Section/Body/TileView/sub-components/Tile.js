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

const StyledTile = styled.div`
  border: 1px solid #eceef1;

  border-radius: 12px;

  box-sizing: border-box;

  cursor: pointer;

  &:hover {
    .tile-header {
      background: #f3f4f4;
    }
  }
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
    props.withBadge
      ? props.isPinned
        ? "auto 1fr auto auto auto"
        : "auto 1fr auto auto"
      : props.isPinned
      ? "auto 1fr  auto auto"
      : "auto 1fr auto"};

  .tile-header_logo {
    margin-right: 12px;
  }

  .tile-header_heading {
    font-size: 16px;
    line-height: 22px;

    margin-right: 12px;
  }

  .tile-header_badge {
    p {
      line-height: 16px;
    }
  }

  .tile-header_pin-icon {
    height: 16px;

    margin: 0 0 0 12px;
  }

  .tile-header_context-menu-button {
    margin: 0 6px;
  }
`;

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
      type,
      isPrivacy,
      label,
      isPinned,
      badge,
      tags,
      columnCount,
      onClickPinRoom,
      getRoomsContextOptions,
    },
    forwardRef
  ) => {
    const tileRef = React.useRef(null);
    const cmRef = React.useRef(null);

    const onBadgeClick = React.useCallback(() => {
      console.log("on badge click");
    }, []);

    const onTileClick = React.useCallback(() => {
      console.log("tile click");
    }, []);

    const onContextMenu = React.useCallback(
      (e) => {
        if (!cmRef.current.menuRef.current) {
          tileRef.current.click(e); //TODO: need fix context menu to global
        }
        cmRef.current.show(e);
      },
      [cmRef.current, tileRef.current]
    );

    return (
      <StyledTile
        ref={tileRef}
        onContextMenu={onContextMenu}
        onClick={onTileClick}
      >
        <StyledHeader
          className="tile-header"
          withBadge={!!badge}
          isPinned={isPinned}
        >
          <RoomLogo
            className={"tile-header_logo"}
            type={type}
            isPrivacy={isPrivacy}
          />

          <Heading className="tile-header_heading" truncate>
            {label}
          </Heading>

          {!!badge && (
            <Badge
              className="tile-header_badge"
              label={badge}
              onClick={onBadgeClick}
            />
          )}

          {isPinned && (
            <ReactSVG
              className="tile-header_pin-icon"
              onClick={onClickPinRoom}
              src="images/unpin.react.svg"
            />
          )}

          <ContextMenuButton
            className="tile-header_context-menu-button"
            getData={getRoomsContextOptions}
            directionX="right"
            isNew={true}
            onClick={onContextMenu}
          />

          <ContextMenu
            getContextModel={getRoomsContextOptions}
            ref={cmRef}
            withBackdrop={true}
          />
        </StyledHeader>
        <StyledContent className="virtual-rooms_tile-content" ref={forwardRef}>
          {columnCount && (
            <Tags
              className="virtual-rooms_tile-content-tags"
              tags={tags}
              columnCount={columnCount}
            />
          )}
        </StyledContent>
      </StyledTile>
    );
  }
);

export default inject(({ contextOptionsStore }) => {
  const { onClickPinRoom, getRoomsContextOptions } = contextOptionsStore;

  return { onClickPinRoom, getRoomsContextOptions };
})(observer(Tile));
