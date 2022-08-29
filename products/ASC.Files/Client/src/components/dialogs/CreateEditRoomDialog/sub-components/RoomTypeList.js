import React from "react";
import styled from "styled-components";
import { roomTypes } from "../data";

import RoomType from "./RoomType";

const StyledRoomTypeList = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
`;

const RoomTypeList = ({ t, setRoomType }) => {
  return (
    <StyledRoomTypeList>
      {roomTypes.map((room) => (
        <RoomType
          t={t}
          key={room.type}
          room={room}
          type={"listItem"}
          onClick={() => setRoomType(room.type)}
        />
      ))}
    </StyledRoomTypeList>
  );
};

export default RoomTypeList;
