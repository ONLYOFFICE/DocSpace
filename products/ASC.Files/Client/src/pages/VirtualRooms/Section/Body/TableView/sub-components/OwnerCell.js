import React from "react";
import styled from "styled-components";

import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import TableCell from "@appserver/components/table-container/TableCell";

const StyledOwnerCell = styled.div`
  display: flex;
  width: 100%;

  align-items: center;

  .author-avatar-cell {
    width: 16px;
    height: 16px;
    margin-right: 8px;
  }
`;

const OwnerCell = ({ sideColor }) => {
  return (
    <TableCell>
      <StyledOwnerCell>
        <Avatar className="author-avatar-cell" role="user" />
        <Text color={sideColor} fontSize="12px" fontWeight={400} truncate>
          Me
        </Text>
      </StyledOwnerCell>
    </TableCell>
  );
};

export default OwnerCell;
