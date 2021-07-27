import React from "react";
import Text from "@appserver/components/text";
import styled from "styled-components";

const StyledAuthorCell = styled.div`
  display: flex;

  .author-avatar-cell {
    width: 16px;
    height: 16px;
    margin-right: 8px;
    border-radius: 20px;
  }
`;

const AuthorCell = ({ fileOwner, sideColor, item }) => {
  return (
    <StyledAuthorCell>
      <img src={item.createdBy.avatarSmall} className="author-avatar-cell" />
      <Text
        as="div"
        color={sideColor}
        fontSize="12px"
        fontWeight={400}
        title={fileOwner}
        truncate={true}
      >
        {fileOwner}
      </Text>
    </StyledAuthorCell>
  );
};

export default AuthorCell;
