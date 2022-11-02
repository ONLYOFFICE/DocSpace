import { isHugeMobile } from "@docspace/components/utils/device";
import React, { useState } from "react";
import styled from "styled-components";
import { roomTypes } from "../../data";
import RoomType from "../RoomType";
import DropdownDesktop from "./DropdownDesktop";
import DropdownMobile from "./DropdownMobile";

const StyledRoomTypeDropdown = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
`;

const RoomTypeDropdown = ({
  t,
  currentRoom,
  setRoomType,
  setIsScrollLocked,
  isDisabled,
}) => {
  const [isOpen, setIsOpen] = useState(false);

  const toggleDropdown = () => {
    if (isDisabled) return;
    if (isOpen) {
      setIsScrollLocked(false);
      setIsOpen(false);
    } else {
      setIsScrollLocked(true);
      setIsOpen(true);
    }
  };

  const chooseRoomType = (roomType) => {
    if (isDisabled) return;
    setRoomType(roomType);
    toggleDropdown();
  };

  return (
    <StyledRoomTypeDropdown isOpen={isOpen}>
      <RoomType
        t={t}
        room={currentRoom}
        type="dropdownButton"
        isOpen={isOpen}
        onClick={toggleDropdown}
      />
      {isHugeMobile() ? (
        <DropdownMobile
          t={t}
          open={isOpen}
          onClose={toggleDropdown}
          roomTypes={roomTypes}
          chooseRoomType={chooseRoomType}
        />
      ) : (
        <DropdownDesktop
          t={t}
          open={isOpen}
          roomTypes={roomTypes}
          chooseRoomType={chooseRoomType}
        />
      )}
    </StyledRoomTypeDropdown>
  );
};

export default RoomTypeDropdown;
