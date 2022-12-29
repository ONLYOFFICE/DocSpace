import React from "react";
import DefaultUserPhotoSize32PngUrl from "ASSETS_DIR/images/default_user_photo_size_32-32.png?url";
import { StyledText, StyledAuthorCell } from "./CellStyles";
import Avatar from "@docspace/components/avatar";

const AuthorCell = ({ fileOwner, sideColor, item }) => {
  const { avatarSmall, hasAvatar } = item.createdBy;

  const avatarSource = hasAvatar ? avatarSmall : DefaultUserPhotoSize32PngUrl;

  return (
    <StyledAuthorCell className="author-cell">
      <Avatar
        source={avatarSource}
        className="author-avatar-cell"
        role="user"
      />
      <StyledText
        color={sideColor}
        fontSize="12px"
        fontWeight={600}
        title={fileOwner}
        truncate
      >
        {fileOwner}
      </StyledText>
    </StyledAuthorCell>
  );
};

export default AuthorCell;
