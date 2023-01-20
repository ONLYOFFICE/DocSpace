import { isHugeMobile } from "@docspace/components/utils/device";
import React, { useState } from "react";
import styled from "styled-components";
import RoomType from "../RoomType";
import DropdownDesktop from "./DropdownDesktop";
import DropdownMobile from "./DropdownMobile";

const StyledRoomTypeDropdown = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;

  .backdrop-active {
    top: -64px;
    backdrop-filter: unset;
    background: rgba(6, 22, 38, 0.2);
  }
`;

const RoomTypeDropdown = ({
  t,
  currentRoomType,
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
        roomType={currentRoomType}
        id="shared_select-room"
        selectedId={currentRoomType}
        type="dropdownButton"
        isOpen={isOpen}
        onClick={toggleDropdown}
      />
      {isHugeMobile() ? (
        <DropdownMobile
          t={t}
          open={isOpen}
          onClose={toggleDropdown}
          chooseRoomType={chooseRoomType}
        />
      ) : (
        <DropdownDesktop t={t} open={isOpen} chooseRoomType={chooseRoomType} />
      )}
    </StyledRoomTypeDropdown>
  );
};

export default RoomTypeDropdown;
