import React from "react";
import styled from "styled-components";

import RoomType from "../RoomType";

import { Base } from "@docspace/components/themes";
import { RoomsType } from "@docspace/common/constants";

const StyledDropdownDesktop = styled.div`
  max-width: 100%;
  position: relative;

  ${(props) => !props.isOpen && "display: none"};

  .dropdown-content {
    background: ${(props) =>
      props.theme.createEditRoomDialog.roomTypeDropdown.desktop.background};
    border: 1px solid
      ${(props) =>
        props.theme.createEditRoomDialog.roomTypeDropdown.desktop.borderColor};
    margin-top: 4px;
    overflow: visible;
    z-index: 400;
    top: 0;
    left: 0;
    box-sizing: border-box;
    width: 100%;
    position: absolute;
    display: flex;
    flex-direction: column;
    padding: 6px 0;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
    border-radius: 6px;
  }
`;

StyledDropdownDesktop.defaultProps = { theme: Base };

const DropdownDesktop = ({ t, open, chooseRoomType }) => {
  return (
    <StyledDropdownDesktop className="dropdown-content-wrapper" isOpen={open}>
      <div className="dropdown-content">
        {Object.values(RoomsType).map((roomType) => (
          <RoomType
            id={roomType}
            t={t}
            key={roomType}
            roomType={roomType}
            type="dropdownItem"
            onClick={() => chooseRoomType(roomType)}
          />
        ))}
      </div>
    </StyledDropdownDesktop>
  );
};

export default DropdownDesktop;
