import React from "react";
import { StyledTableContainer } from "./StyledTableContainer";
import PropTypes from "prop-types";

const TableContainer = (props) => {
  const { forwardedRef, useReactWindow, ...rest } = props;

  return (
    <StyledTableContainer
      id="table-container"
      className="table-container"
      ref={forwardedRef}
      useReactWindow={useReactWindow}
      {...rest}
    />
  );
};

TableContainer.propTypes = {
  forwardedRef: PropTypes.shape({ current: PropTypes.any }),
  useReactWindow: PropTypes.bool,
};

export default TableContainer;
