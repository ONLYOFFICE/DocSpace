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
  return (
    <StyledText
      color={sideColor}
      fontSize="12px"
      fontWeight={400}
      title=""
      truncate
    >
      {fileExst || contentLength ? contentLength : "—"}
    </StyledText>
  );
};

export default SizeCell;
