import React from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
//import ADSelectorRow from "../row";
import Checkbox from "../../../checkbox";
import Loader from "../../../loader";
import Text from "../../../text";
import findIndex from "lodash/findIndex";
//import isEqual from "lodash/isEqual";

class ADSelectorOptionsBody extends React.Component {
  constructor(props) {
    super(props);

    this.listRef = React.createRef();
    //this.hasMountedRef = React.createRef(false);
  }

  /*componentDidMount() {
    this.hasMountedRef.current = true;
  }

  componentDidUpdate(prevProps) {
    if(!isEqual(this.props.selectedOptions, prevProps.selectedOptions)) {
      if(this.listRef.current && this.hasMountedRef.current) {
        //this.listRef.current.resetloadMoreItemsCache(true);
        //console.log("this.listRef.current", this.listRef.current)
      }
    }

  }*/

  renderRow = ({ index, style }) => {
    //console.log("renderRow", option, isChecked, this.state.selectedOptions);

    let content;
    const isLoaded = this.isItemLoaded(index);
    let key = "loader";
    if (!isLoaded) {
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
          <Text as="span">Loading... Please wait...</Text>
        </div>
      );
    } else {
      const {options, isMultiSelect, selectedAll, selectedOptions} = this.props;
      const option = options[index];
      var isChecked = isMultiSelect
        ? selectedAll ||
          findIndex(selectedOptions, { key: option.key }) > -1
        : undefined;
        key = option.key;

      content = (
        <Checkbox
            key={option.key}
            label={option.label}
            isChecked={isChecked}
            className="option_checkbox"
            onChange={this.props.onRowChecked.bind(this, option)}
          />
        /*<ADSelectorRow
          key={option.key}
          label={option.label}
          isChecked={isChecked}
          isMultiSelect={isMultiSelect}
          style={style}
          onChange={this.props.onRowChecked.bind(this, option)}
          onSelect={this.props.onRowSelect.bind(this, option)}
        />*/
        /*<div className="option" style={style}>
          <input
            id={option.key}
            type="checkbox"
            onChange={(e) => { 
              console.log("checkbox click", e);
              this.props.onRowChecked(option, e);
            }}
            checked={isChecked}
          />
          <label htmlFor={option.key}>{option.label}</label>
        </div>*/
      );
    }

    return <div className="option" style={style} key={key}>{content}</div>;
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
          ref={this.listRef}
          isItemLoaded={this.isItemLoaded}
          itemCount={itemCount}
          loadMoreItems={this.loadMoreItems}
        >
          {({ onItemsRendered, ref }) => (
            <List
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
            </List>
          )}
        </InfiniteLoader>
      </div>
    );
  }
}

ADSelectorOptionsBody.propTypes = {
  options: PropTypes.array,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,

  selectedOptions: PropTypes.array,
  selectedAll: PropTypes.bool,
  isMultiSelect: PropTypes.bool,

  listHeight: PropTypes.number,
  listWidth: PropTypes.number,
  itemHeight: PropTypes.number,
  onRowChecked: PropTypes.func,
  onRowSelect: PropTypes.func
};

export default ADSelectorOptionsBody;
