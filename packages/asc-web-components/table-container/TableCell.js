import React from "react";
import { StyledTableCell } from "./StyledTableContainer";

const TableCell = ({ className, forwardedRef, ...rest }) => {
  return (
    <StyledTableCell
      className={`${className} table-container_cell`}
      ref={forwardedRef}
      {...rest}
    />
  );
};

export default TableCell;
