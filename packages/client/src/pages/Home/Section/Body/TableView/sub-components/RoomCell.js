import React from "react";
import { StyledText } from "./CellStyles";

const RoomCell = ({ t, sideColor, item }) => {
  const { originTitle } = item;
  return (
    <StyledText
      title={originTitle}
      fontSize="12px"
      fontWeight={600}
      color={sideColor}
      className="row_update-text"
      truncate
    >
      {originTitle}
    </StyledText>
  );
};

export default RoomCell;
