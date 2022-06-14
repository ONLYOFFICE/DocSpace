import React from "react";
import styled from "styled-components";

import TableCell from "@appserver/components/table-container/TableCell";

import Text from "@appserver/components/text";

const TypeCell = ({ type, sideColor }) => {
  return (
    <TableCell className="table-container_element-wrapper">
      <Text isBold={true} truncate={true} noSelect={true} color={sideColor}>
        {type}
      </Text>
    </TableCell>
  );
};

export default React.memo(TypeCell);
