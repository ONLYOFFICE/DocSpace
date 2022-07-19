import React from "react";
import styled from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";

const StyledSetRoomParams = styled.div`
  display: flex;
`;

const SetRoomParams = ({ roomParams, rooms, chooseRoomType }) => {
  return (
    <StyledSetRoomParams>
      <RoomTypeDropdown
        rooms={rooms}
        currentRoomType={roomParams.type}
        chooseRoomType={chooseRoomType}
      />
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
