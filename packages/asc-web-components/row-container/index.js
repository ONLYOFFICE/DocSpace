/* eslint-disable react/display-name */
import React, { memo } from "react";

import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import ContextMenu from "../context-menu";
import StyledRowContainer from "./styled-row-container";

class RowContainer extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      contextOptions: [],
    };
  }

  onRowContextClick = (options) => {
    if (Array.isArray(options)) {
      this.setState({
        contextOptions: options,
      });
    }
  };

  componentDidMount() {
    window.addEventListener("contextmenu", this.onRowContextClick);
  }

  componentWillUnmount() {
    window.removeEventListener("contextmenu", this.onRowContextClick);
  }

  // eslint-disable-next-line react/prop-types
  renderRow = memo(({ data, index, style }) => {
    // eslint-disable-next-line react/prop-types
    const options = data[index].props.contextOptions;

    return (
      <div
        onContextMenu={this.onRowContextClick.bind(this, options)}
        style={style}
      >
        {data[index]}
      </div>
    );
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
    } = this.props;

    const renderList = ({ height, width }) => (
      <List
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
        {useReactWindow ? (
          <AutoSizer>{renderList}</AutoSizer>
        ) : (
          children.map((item, index) => (
            <div
              key={index}
              onContextMenu={this.onRowContextClick.bind(
                this,
                item.props.contextOptions
              )}
            >
              {item}
            </div>
          ))
        )}
        <ContextMenu targetAreaId={id} options={this.state.contextOptions} />
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
};

RowContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true,
  id: "rowContainer",
};

export default RowContainer;
