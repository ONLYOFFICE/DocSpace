import React from "react";
import { StyledText } from "./CellStyles";

const DateCell = ({ create, updatedDate, createdDate, sideColor }) => {
  const date = create ? createdDate : updatedDate;

  return (
    <StyledText
      title={date}
      fontSize="12px"
      fontWeight={400}
      color={sideColor}
      className="row_update-text"
      truncate
    >
      {date && date}
    </StyledText>
  );
};

export default DateCell;
