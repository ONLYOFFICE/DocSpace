import React from "react";
import { StyledTableCell } from "./StyledTableContainer";

const TableCell = (props) => {
  return (
    <StyledTableCell
      className="table-container_cell"
      ref={props.forwardedRef}
      {...props}
    />
  );
};

export default TableCell;
