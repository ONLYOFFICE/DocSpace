import React from "react";
import styled from "styled-components";

import RoomType from "./RoomType";

const StyledRoomTypeList = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
`;

const RoomTypeList = ({ rooms, chooseRoomType }) => {
  return (
    <StyledRoomTypeList>
      {rooms.map((room, i) => (
        <RoomType
          key={room.type}
          room={room}
          type={"listItem"}
          onClick={() => chooseRoomType(room.type)}
        />
      ))}
    </StyledRoomTypeList>
  );
};

export default RoomTypeList;
