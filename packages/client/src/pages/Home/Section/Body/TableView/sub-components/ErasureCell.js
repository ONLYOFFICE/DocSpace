import React from "react";
import { StyledText } from "./CellStyles";

const ErasureCell = ({ t, sideColor, item }) => {
  const { daysRemaining } = item;
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
