import React from "react";
import { StyledText } from "./CellStyles";

const SizeCell = ({ t, item, sideColor }) => {
  const {
    fileExst,
    contentLength,
    providerKey,
    filesCount,
    foldersCount,
  } = item;
  const date = fileExst || contentLength ? contentLength : "—";

  return (
    <StyledText
      color={sideColor}
      fontSize="12px"
      fontWeight={600}
      title={date}
      truncate
    >
      {date}
    </StyledText>
  );
};

export default SizeCell;
