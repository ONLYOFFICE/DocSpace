import React from "react";
import PropTypes from "prop-types";
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

TableCell.propTypes = {
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  forwardedRef: PropTypes.shape({ current: PropTypes.any }),
};

export default TableCell;
