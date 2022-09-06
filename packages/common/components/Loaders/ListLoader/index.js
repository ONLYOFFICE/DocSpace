import ListItemLoader from "./ListItemLoader";
import React from "react";
import PropTypes from "prop-types";
import { StyledList } from "./StyledListLoader";
const ListLoader = ({ count, ...props }) => {
  const items = [];

  for (var i = 0; i < count; i++) {
    items.push(<ListItemLoader key={`list_loader_${i}`} {...props} />);
  }
  return <StyledList className="list-loader-wrapper">{items}</StyledList>;
};

ListLoader.propTypes = {
  count: PropTypes.number,
};

ListLoader.defaultProps = {
  count: 25,
};
export default ListLoader;
