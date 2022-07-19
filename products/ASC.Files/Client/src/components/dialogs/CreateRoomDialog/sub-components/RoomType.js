import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import RoomLogo from "@appserver/components/room-logo";
import HelpButton from "@appserver/components/help-button";

const StyledRoomType = styled.div`
  cursor: pointer;
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
      .choose_room-title-info_button {
        border-radius: 50%;
        background-color: #a3a9ae;
        circle,
        rect {
          fill: #ffffff;
        }
      }
    }
    .choose_room-description {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
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
  border: 1px solid #f3f4f4;
  border-radius: 6px;
  &:hover {
    background-color: #eceef1;
  }
`;

const StyledDropdownButton = styled(StyledRoomType)`
  background-color: #f8f8f8;
  border-radius: 6px;

  .choose_room-forward_btn {
    &.dropdown-button {
      transform: ${(props) =>
        props.isOpen ? "rotate(-90deg)" : "rotate(90deg)"};
    }
  }
`;

const StyledDropdownItem = styled(StyledRoomType)`
  border-radius: 0px;
  &:hover {
    background-color: #f3f4f4;
  }
`;

const RoomType = ({ room, onClick, type = "listItem", isOpen }) => {
  const arrowClassName =
    type === "dropdownButton"
      ? "choose_room-forward_btn dropdown-button"
      : type === "dropdownItem"
      ? "choose_room-forward_btn dropdown-item"
      : "choose_room-forward_btn";

  const content = (
    <>
      <div className="choose_room-logo_wrapper">
        <RoomLogo type={room.type} />
      </div>

      <div className="choose_room-info_wrapper">
        <div className="choose_room-title">
          <Text noSelect className="choose_room-title-text">
            {room.title}
          </Text>
          {room.withSecondaryInfo && (
            <HelpButton
              displayType="auto"
              color="none"
              className="choose_room-title-info_button"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={room.description}
              size={12}
            />
          )}
        </div>
        <Text noSelect className="choose_room-description">
          {room.description}
        </Text>
      </div>

      {type !== "dropdownItem" && (
        <IconButton
          className={arrowClassName}
          iconName="images/arrow.react.svg"
          size={16}
          onClick={() => {}}
        />
      )}
    </>
  );

  return type === "listItem" ? (
    <StyledListItem onClick={onClick}>{content}</StyledListItem>
  ) : type === "dropdownButton" ? (
    <StyledDropdownButton isOpen={isOpen} onClick={onClick}>
      {content}
    </StyledDropdownButton>
  ) : type === "dropdownItem" ? (
    <StyledDropdownItem onClick={onClick}>{content}</StyledDropdownItem>
  ) : (
    <StyledRoomType>{content}</StyledRoomType>
  );
};

RoomType.propTypes = {
  room: PropTypes.object,
  onClick: PropTypes.func,
  type: PropTypes.oneOf(["listItem", "dropdownButton", "dropdownItem"]),
  isOpen: PropTypes.bool,
};

export default RoomType;
