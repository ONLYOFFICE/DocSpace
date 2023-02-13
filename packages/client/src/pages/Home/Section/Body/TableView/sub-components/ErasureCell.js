import React from "react";
import { StyledText } from "./CellStyles";
import moment from "moment";

const ErasureCell = ({ t, item, sideColor }) => {
  const daysRemaining = Math.abs(moment().diff(item.autoDelete, "days"));
  const title = t("Files:DaysRemaining", { daysRemaining });

  return (
    <StyledText
      title={title}
      fontSize="12px"
      fontWeight={600}
      color={sideColor}
      className="row_update-text"
      truncate
    >
      {title}
    </StyledText>
  );
};

export default ErasureCell;
