import React from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../../../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import ADSelectorBodyRow from "./row";
import Loader from "../../../../loader";
import { Text } from "../../../../text";
import findIndex from "lodash/findIndex";

class ADSelectorMainBody extends React.Component {
  state = {
    selectedAll: false,
    selectedOptions: []
  };

  renderRow = ({ index, style }) => {
    //console.log("renderRow", option, isChecked, this.state.selectedOptions);

    let content;
    if (!this.isItemLoaded(index)) {
      content = (
        <div className="option" style={style} key="loader">
          <Loader
            type="oval"
            size={16}
            style={{
              display: "inline",
              marginRight: "10px"
            }}
          />
          <Text.Body as="span">Loading... Please wait...</Text.Body>
        </div>
      );
    } else {
      const option = this.props.options[index];
      var isChecked = this.props.isMultiSelect
        ? this.state.selectedAll ||
          findIndex(this.state.selectedOptions, { key: option.key }) > -1
        : undefined;

      content = (
        <ADSelectorBodyRow
          key={option.key}
          label={option.label}
          isChecked={isChecked}
          style={style}
          onChange={this.props.onRowChecked.bind(this, option)}
          onSelect={this.props.onRowSelect.bind(this, option)}
        />
      );
    }

    return <>{content}</>;
  };

  isItemLoaded = index =>
    !this.props.hasNextPage || index < this.props.options.length;

  loadMoreItems = this.props.isNextPageLoading
    ? () => {}
    : this.props.loadNextPage;

  render() {
    const {
      options,
      hasNextPage,
      listHeight,
      listWidth,
      itemHeight
    } = this.props;
    const itemCount = hasNextPage ? options.length + 1 : options.length;

    return (
      <div>
        <InfiniteLoader
          isItemLoaded={this.isItemLoaded}
          itemCount={itemCount}
          loadMoreItems={this.loadMoreItems}
        >
          {({ onItemsRendered, ref }) => (
            <FixedSizeList
              className="options_list"
              height={listHeight}
              width={listWidth}
              itemCount={itemCount}
              itemSize={itemHeight}
              onItemsRendered={onItemsRendered}
              ref={ref}
              outerElementType={CustomScrollbarsVirtualList}
            >
              {this.renderRow}
            </FixedSizeList>
          )}
        </InfiniteLoader>
      </div>
    );
  }
}

ADSelectorMainBody.propTypes = {
  options: PropTypes.array,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,

  isMultiSelect: PropTypes.bool,
  listHeight: PropTypes.number,
  listWidth: PropTypes.number,
  itemHeight: PropTypes.number,
  onRowChecked: PropTypes.func,
  onRowSelect: PropTypes.func
};

export default ADSelectorMainBody;
