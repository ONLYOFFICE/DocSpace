/* eslint-disable react/display-name */
import React from "react";
import PropTypes from "prop-types";
import StyledRowContainer from "./styled-row-container";
import InfiniteLoaderComponent from "../infinite-loader";

class RowContainer extends React.PureComponent {
  render() {
    const {
      manualHeight,
      itemHeight,
      children,
      useReactWindow,
      id,
      className,
      style,
      onScroll,
      filesLength,
      itemCount,
      fetchMoreFiles,
      hasMoreFiles,
    } = this.props;

    return (
      <StyledRowContainer
        id={id}
        className={className}
        style={style}
        manualHeight={manualHeight}
        useReactWindow={useReactWindow}
      >
        {useReactWindow ? (
          <InfiniteLoaderComponent
            className="List"
            viewAs="row"
            hasMoreFiles={hasMoreFiles}
            filesLength={filesLength}
            itemCount={itemCount}
            loadMoreItems={fetchMoreFiles}
            itemSize={itemHeight}
            onScroll={onScroll}
          >
            {children}
          </InfiniteLoaderComponent>
        ) : (
          children
        )}
      </StyledRowContainer>
    );
  }
}

RowContainer.propTypes = {
  /** Height of one Row element. Required for the proper functioning of the scroll */
  itemHeight: PropTypes.number,
  /** Allows setting fixed block height for Row */
  manualHeight: PropTypes.string,
  /** Child elements */
  children: PropTypes.any.isRequired,
  /** Enables react-window for efficient rendering of large lists */
  useReactWindow: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets a callback function that is called when the list scroll positions change */
  onScroll: PropTypes.func,
  /** The property required for the infinite loader */
  filesLength: PropTypes.number,
  /** The property required for the infinite loader */
  itemCount: PropTypes.number,
  /** The property required for the infinite loader */
  loadMoreItems: PropTypes.func,
  /** The property required for the infinite loader */
  hasMoreFiles: PropTypes.bool,
};

RowContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true,
  id: "rowContainer",
};

export default RowContainer;
