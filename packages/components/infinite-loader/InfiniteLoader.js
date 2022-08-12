import React from "react";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import ListComponent from "./List";
import GridComponent from "./Grid";

const InfiniteLoaderComponent = (props) => {
  const scroll = isMobileOnly
    ? document.querySelector("#customScrollBar > .scroll-body")
    : document.querySelector("#sectionScroll > .scroll-body");

  return props.viewAs === "tile" ? (
    <GridComponent scroll={scroll} {...props} />
  ) : (
    <ListComponent scroll={scroll} {...props} />
  );
};
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
