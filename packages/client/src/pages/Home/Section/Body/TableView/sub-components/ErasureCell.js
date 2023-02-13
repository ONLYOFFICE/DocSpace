import React from "react";
import { StyledText } from "./CellStyles";
import { getErasure } from "SRC_DIR/helpers/filesUtils";

const ErasureCell = ({ t, autoDelete, sideColor }) => {
  const daysRemaining = getErasure(autoDelete);
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
