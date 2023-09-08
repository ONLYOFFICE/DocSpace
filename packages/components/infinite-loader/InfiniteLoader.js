import React from "react";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import ListComponent from "./List";
import GridComponent from "./Grid";

const InfiniteLoaderComponent = (props) => {
  const { viewAs, isLoading } = props;

  const scroll = isMobileOnly
    ? document.querySelector("#customScrollBar .scroll-wrapper > .scroller")
    : document.querySelector("#sectionScroll .scroll-wrapper > .scroller");

  if (isLoading) return <></>;

  return viewAs === "tile" ? (
    <GridComponent scroll={scroll ?? window} {...props} />
  ) : (
    <ListComponent scroll={scroll ?? window} {...props} />
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

InfiniteLoaderComponent.defaultProps = {
  isLoading: false,
};

export default InfiniteLoaderComponent;
