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
  /** Height of one Row element. Required for scroll to work properly */
  itemHeight: PropTypes.number,
  /** Allows you to set fixed block height for Row */
  manualHeight: PropTypes.string,
  /** Child elements */
  children: PropTypes.any.isRequired,
  /** Use react-window for efficiently rendering large lists */
  useReactWindow: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Called when the list scroll positions changes */
  onScroll: PropTypes.func,
  filesLength: PropTypes.number,
  itemCount: PropTypes.number,
  loadMoreItems: PropTypes.func,
  hasMoreFiles: PropTypes.bool,
};

RowContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true,
  id: "rowContainer",
};

export default RowContainer;
