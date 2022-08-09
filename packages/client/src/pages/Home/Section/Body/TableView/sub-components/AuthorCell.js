import React from "react";
import { StyledText, StyledAuthorCell } from "./CellStyles";
import Avatar from "@docspace/components/avatar";

const AuthorCell = ({ fileOwner, sideColor, item }) => {
  return (
    <StyledAuthorCell>
      <Avatar
        source={item.createdBy.avatarSmall}
        className="author-avatar-cell"
        role="user"
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
    </StyledAuthorCell>
  );
};

export default AuthorCell;
