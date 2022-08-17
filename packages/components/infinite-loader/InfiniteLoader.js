import React from "react";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import ListComponent from "./List";
import GridComponent from "./Grid";
import { isMobile } from "../utils/device";

const InfiniteLoaderComponent = (props) => {
  const { viewAs } = props;

  const scroll = isMobileOnly
    ? document.querySelector("#customScrollBar > .scroll-body")
    : document.querySelector("#sectionScroll > .scroll-body");

  if (viewAs === "row") scroll.style.paddingRight = 0;
  else scroll.style.paddingRight = isMobile() ? "8px" : "17px";

  return viewAs === "tile" ? (
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
