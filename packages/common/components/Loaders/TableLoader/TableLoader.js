import React from "react";
import TableRow from "../TableRow";
import PropTypes from "prop-types";

const TableLoader = ({ count, ...props }) => {
  const items = [];

  for (var i = 0; i < count; i++) {
    items.push(<TableRow key={`row_loader_${i}`} {...props} />);
  }
  return <div>{items}</div>;
};

TableLoader.propTypes = {
  count: PropTypes.number,
};

TableLoader.defaultProps = {
  count: 25,
};
export default TableLoader;
