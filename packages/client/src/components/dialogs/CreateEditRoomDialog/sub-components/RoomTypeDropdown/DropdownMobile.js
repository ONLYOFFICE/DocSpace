import React from "react";
import styled from "styled-components";

import RoomType from "../RoomType";
import { RoomsType } from "./../../../../../../../common/constants/index";
import { Backdrop } from "@docspace/components";

import { Base } from "@docspace/components/themes";

const StyledDropdownMobile = styled.div`
  visibility: ${(props) => (props.isOpen ? "visible" : "hidden")};
  position: fixed;
  bottom: 0;
  z-index: 500;
  padding-top: 6px;

  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `margin-right: -16px;`
      : `margin-left: -16px;`}
  box-shadow: 0px -4px 60px rgba(4, 15, 27, 0.12);
  border-radius: 6px 6px 0px 0px;
  background: ${(props) =>
    props.theme.createEditRoomDialog.roomTypeDropdown.mobile.background};
`;

StyledDropdownMobile.defaultProps = { theme: Base };

const DropdownMobile = ({ t, open, onClose, chooseRoomType }) => {
  return (
    <>
      <Backdrop visible={open} onClick={onClose} zIndex={450} />
      <StyledDropdownMobile className="dropdown-mobile" isOpen={open}>
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
      </StyledDropdownMobile>
    </>
  );
};

export default DropdownMobile;
