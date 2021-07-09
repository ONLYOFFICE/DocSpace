import React from "react";
import { StyledTableContainer } from "./StyledTableContainer";
import PropTypes from "prop-types";

const TableContainer = (props) => {
  return (
    <StyledTableContainer
      style={{ marginTop: -18 }}
      ref={props.forwardedRef}
      {...props}
    />
  );
};

TableContainer.propTypes = {
  forwardedRef: PropTypes.shape({ current: PropTypes.any }),
};

export default TableContainer;
