import React from "react";
import styled from "styled-components";

import RectangleLoader from "../RectangleLoader";

const StyledRoomTypeListLoader = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
`;

const RoomTypeListLoader = ({}) => {
  return (
    <StyledRoomTypeListLoader>
      {[...Array(5).keys()].map((key) => (
        <RectangleLoader
          key={key}
          width={"100%"}
          height={"86"}
          borderRadius={"6"}
        />
      ))}
    </StyledRoomTypeListLoader>
  );
};
export default RoomTypeListLoader;
