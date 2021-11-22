import React from "react";
import { StyledText, StyledAuthorAvatar } from "./CellStyles";

const AuthorCell = ({ fileOwner, sideColor, item }) => {
  return (
    <>
      <StyledAuthorAvatar
        src={item.createdBy.avatarSmall}
        className="author-avatar-cell"
      />
      <StyledText
        color={sideColor}
        fontSize="12px"
        fontWeight={400}
        title={fileOwner}
        truncate
      >
        {fileOwner}
      </StyledText>
    </>
  );
};

export default AuthorCell;
