import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import RoomLogo from "@docspace/components/room-logo";
import { Base } from "@docspace/components/themes";

const StyledRoomType = styled.div`
  cursor: pointer;
  user-select: none;
  outline: 0;

  padding: 16px;
  width: 100%;
  box-sizing: border-box;

  display: flex;
  gap: 12px;
  align-items: center;

  .choose_room-logo_wrapper {
    width: 32px;
    margin-bottom: auto;
  }

  .choose_room-info_wrapper {
    display: flex;
    flex-direction: column;
    gap: 4px;
    .choose_room-title {
      display: flex;
      flex-direction: row;
      gap: 6px;
      align-items: center;
      .choose_room-title-text {
        font-weight: 600;
        font-size: 14px;
        line-height: 16px;
      }
    }
    .choose_room-description {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
    }
  }

  .choose_room-forward_btn {
    margin-left: auto;
    max-width: 17px;
    max-height: 17px;
    min-width: 17px;
    min-height: 17px;
  }
`;

const StyledListItem = styled(StyledRoomType)`
  background-color: ${(props) =>
    props.theme.createEditRoomDialog.roomType.listItem.background};
  border: 1px solid
    ${(props) => props.theme.createEditRoomDialog.roomType.listItem.borderColor};
  border-radius: 6px;

  .choose_room-description {
    color: ${(props) =>
      props.theme.createEditRoomDialog.roomType.listItem.descriptionText};
  }
`;

const StyledDropdownButton = styled(StyledRoomType)`
  border-radius: 6px;
  background-color: ${(props) =>
    props.theme.createEditRoomDialog.roomType.dropdownButton.background};
  border: 1px solid
    ${(props) =>
      props.isOpen
        ? props.theme.createEditRoomDialog.roomType.dropdownButton
            .isOpenBorderColor
        : props.theme.createEditRoomDialog.roomType.dropdownButton.borderColor};

  .choose_room-description {
    color: ${(props) =>
      props.theme.createEditRoomDialog.roomType.dropdownButton.descriptionText};
  }

  .choose_room-forward_btn {
    &.dropdown-button {
      transform: ${(props) =>
        props.isOpen ? "rotate(-90deg)" : "rotate(90deg)"};
    }
  }
`;

const StyledDropdownItem = styled(StyledRoomType)`
  background-color: ${(props) =>
    props.theme.createEditRoomDialog.roomType.dropdownItem.background};

  &:hover {
    background-color: ${(props) =>
      props.theme.createEditRoomDialog.roomType.dropdownItem.hoverBackground};
  }

  .choose_room-description {
    color: ${(props) =>
      props.theme.createEditRoomDialog.roomType.dropdownItem.descriptionText};
  }

  .choose_room-forward_btn {
    display: none;
  }
`;

const StyledDisplayItem = styled(StyledRoomType)`
  cursor: default;
  background-color: ${(props) =>
    props.theme.createEditRoomDialog.roomType.displayItem.background};
  border: 1px solid
    ${(props) =>
      props.theme.createEditRoomDialog.roomType.displayItem.borderColor};
  border-radius: 6px;

  .choose_room-description {
    color: ${(props) =>
      props.theme.createEditRoomDialog.roomType.displayItem.descriptionText};
  }

  .choose_room-forward_btn {
    display: none;
  }
`;

const RoomType = ({ t, room, onClick, type = "listItem", isOpen }) => {
  const arrowClassName =
    type === "dropdownButton"
      ? "choose_room-forward_btn dropdown-button"
      : type === "dropdownItem"
      ? "choose_room-forward_btn dropdown-item"
      : "choose_room-forward_btn";

  const onSecondaryInfoClick = (e) => {
    e.stopPropagation();
  };

  const content = (
    <>
      <div className="choose_room-logo_wrapper">
        <RoomLogo type={room.type} />
      </div>

      <div className="choose_room-info_wrapper">
        <div className="choose_room-title">
          <Text noSelect className="choose_room-title-text">
            {t(room.title)}
          </Text>
        </div>
        <Text noSelect className="choose_room-description">
          {t(room.description)}
        </Text>
      </div>

      <IconButton
        className={arrowClassName}
        iconName="images/arrow.react.svg"
        size={16}
        onClick={() => {}}
      />
    </>
  );

  return type === "listItem" ? (
    <StyledListItem title={t(room.title)} onClick={onClick}>
      {content}
    </StyledListItem>
  ) : type === "dropdownButton" ? (
    <StyledDropdownButton
      title={t(room.title)}
      onClick={onClick}
      isOpen={isOpen}
    >
      {content}
    </StyledDropdownButton>
  ) : type === "dropdownItem" ? (
    <StyledDropdownItem title={t(room.title)} onClick={onClick} isOpen={isOpen}>
      {content}
    </StyledDropdownItem>
  ) : (
    <StyledDisplayItem title={t(room.title)}>{content}</StyledDisplayItem>
  );
};

StyledListItem.defaultProps = { theme: Base };
StyledDropdownButton.defaultProps = { theme: Base };
StyledDropdownItem.defaultProps = { theme: Base };
StyledDisplayItem.defaultProps = { theme: Base };

RoomType.propTypes = {
  room: PropTypes.object,
  onClick: PropTypes.func,
  type: PropTypes.oneOf([
    "displayItem",
    "listItem",
    "dropdownButton",
    "dropdownItem",
  ]),
  isOpen: PropTypes.bool,
};

export default RoomType;
