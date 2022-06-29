import React from "react";
import styled from "styled-components";

import TableCell from "@appserver/components/table-container/TableCell";

import Text from "@appserver/components/text";

const DateCell = ({ sideColor }) => {
  return (
    <TableCell className="table-container_element-wrapper">
      <Text
        fontWeight={600}
        fontSize={"11px"}
        truncate={true}
        noSelect={true}
        color={sideColor}
      >
        50kb
      </Text>
    </TableCell>
  );
};

export default React.memo(DateCell);
