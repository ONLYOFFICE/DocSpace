import React from "react";
import PropTypes from "prop-types";
import ListComponent from "./List";
import GridComponent from "./Grid";

const InfiniteLoaderComponent = (props) =>
  props.viewAs === "tile" ? (
    <GridComponent {...props} />
  ) : (
    <ListComponent {...props} />
  );

InfiniteLoaderComponent.propTypes = {
  viewAs: PropTypes.string.isRequired,
  hasMoreFiles: PropTypes.bool.isRequired,
  filesLength: PropTypes.number.isRequired,
  itemCount: PropTypes.number.isRequired,
  loadMoreItems: PropTypes.func.isRequired,
  itemSize: PropTypes.number,
  children: PropTypes.any.isRequired,
  /** Called when the list scroll positions changes */
  onScroll: PropTypes.func,
};

export default InfiniteLoaderComponent;
