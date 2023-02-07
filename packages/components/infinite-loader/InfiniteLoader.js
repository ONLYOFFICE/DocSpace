import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import ListComponent from "./List";
import GridComponent from "./Grid";
import { isMobile } from "../utils/device";
import Loader from "../loader";
import { showLoader, hideLoader } from "./infiniteLoaderUtils";

const InfiniteLoaderComponent = (props) => {
  const { viewAs, isLoading } = props;

  useEffect(() => (isLoading ? showLoader() : hideLoader()), [isLoading]);

  const scroll = isMobileOnly
    ? document.querySelector("#customScrollBar > .scroll-body")
    : document.querySelector("#sectionScroll > .scroll-body");

  if (scroll) {
    scroll.style.paddingRight = isMobile() ? "8px" : "17px";
  }

  return isLoading ? (
    <Loader
      style={{ display: "none" }}
      id="infinite-page-loader"
      className="pageLoader"
      type="rombs"
      size="40px"
    />
  ) : viewAs === "tile" ? (
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
