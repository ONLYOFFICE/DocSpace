import React from "react";
import styled from "styled-components";

import TableCell from "@appserver/components/table-container/TableCell";

import Text from "@appserver/components/text";

import { RoomsType } from "@appserver/common/constants";

const TypeCell = ({ type, sideColor }) => {
  const [typeName, setTypeName] = React.useState("");

  React.useEffect(() => {
    let title = "";
    switch (type) {
      case RoomsType.FillingFormsRoom:
        title = "Filling form";
        break;
      case RoomsType.EditingRoom:
        title = "Collaboration";
        break;
      case RoomsType.ReviewRoom:
        title = "Review";
        break;
      case RoomsType.ReadOnlyRoom:
        title = "View-only";
        break;
      case RoomsType.CustomRoom:
        title = "Custom";
        break;
    }

    setTypeName(title);
  }, [type]);

  return (
    <TableCell className="table-container_element-wrapper">
      <Text
        fontWeight={600}
        fontSize={"11px"}
        truncate={true}
        noSelect={true}
        color={sideColor}
      >
        {typeName}
      </Text>
    </TableCell>
  );
};

export default React.memo(TypeCell);
