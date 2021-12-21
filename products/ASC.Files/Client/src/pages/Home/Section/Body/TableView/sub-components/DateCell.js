import React from "react";
import { StyledText } from "./CellStyles";
import moment from "moment";

const DateCell = ({ create, updatedDate, createdDate, sideColor }) => {
  const date = moment(new Date(create ? createdDate : updatedDate)).format(
    "MM/D/YYYY	hh:mm A"
  );

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
