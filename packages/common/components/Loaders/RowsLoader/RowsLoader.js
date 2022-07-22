import React from "react";
import RowLoader from "../RowLoader";
import PropTypes from "prop-types";
const RowsLoader = ({ count, ...props }) => {
  const items = [];

  for (var i = 0; i < count; i++) {
    items.push(<RowLoader key={`row_loader_${i}`} {...props} />);
  }
  return <div>{items}</div>;
};

RowsLoader.propTypes = {
  count: PropTypes.number,
};

RowsLoader.defaultProps = {
  count: 25,
};
export default RowsLoader;
