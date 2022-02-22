/* eslint-disable react/display-name */
import React, { memo } from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import StyledRowContainer from "./styled-row-container";

class RowContainer extends React.PureComponent {
  renderRow = memo(({ data, index, style }) => {
    return <div style={style}>{data[index]}</div>;
  }, areEqual);

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
    } = this.props;

    const renderList = ({ height, width }) => (
      <List
        onScroll={onScroll}
        className="List"
        height={height}
        width={width}
        itemSize={itemHeight}
        itemCount={children.length}
        itemData={children}
        outerElementType={CustomScrollbarsVirtualList}
      >
        {this.renderRow}
      </List>
    );

    return (
      <StyledRowContainer
        id={id}
        className={className}
        style={style}
        manualHeight={manualHeight}
        useReactWindow={useReactWindow}
      >
        {useReactWindow ? <AutoSizer>{renderList}</AutoSizer> : children}
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
};

RowContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true,
  id: "rowContainer",
};

export default RowContainer;
