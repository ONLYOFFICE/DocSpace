import React from "react";
import styled from "styled-components";

import RectangleLoader from "../RectangleLoader";

const StyledSetRoomParamsLoader = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;

  .tag_input {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
`;

const SetRoomParamsLoader = ({}) => {
  return (
    <StyledSetRoomParamsLoader>
      <RectangleLoader width={"100%"} height={"86"} borderRadius={"6"} />
      <RectangleLoader width={"100%"} height={"53.6"} borderRadius={"6"} />
      <div className="tag_input">
        <RectangleLoader width={"100%"} height={"53.6"} borderRadius={"6"} />
        <RectangleLoader width={"84"} height={"32"} borderRadius={"3"} />
      </div>
      <RectangleLoader width={"100%"} height={"146"} borderRadius={"4"} />
    </StyledSetRoomParamsLoader>
  );
};
export default SetRoomParamsLoader;
