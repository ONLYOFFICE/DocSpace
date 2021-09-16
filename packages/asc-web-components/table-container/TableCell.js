import React from "react";
import PropTypes from "prop-types";
import { StyledTableCell } from "./StyledTableContainer";

const TableCell = ({ className, forwardedRef, checked, ...rest }) => {
  return (
    <StyledTableCell
      className={`${className} table-container_cell`}
      ref={forwardedRef}
      checked={checked}
      {...rest}
    />
  );
};

TableCell.propTypes = {
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  forwardedRef: PropTypes.shape({ current: PropTypes.any }),
  checked: PropTypes.bool,
};

export default TableCell;
