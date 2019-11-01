/* eslint-disable react/prop-types */
import React, { useRef, useEffect, useState } from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import faker, { name } from "faker";
import uniqueId from "lodash/uniqueId";
import Section from "../../../.storybook/decorators/section";
//import ADSelectorRow from "./sub-components/row";
import Checkbox from "../checkbox";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import DropDown from "../drop-down";

const ExampleWrapper = ({
  // Are there more items to load?
  // (This information comes from the most recent API request.)
  hasNextPage,

  // Are we currently loading a page of items?
  // (This may be an in-flight flag in your Redux store for example.)
  isNextPageLoading,

  // Array of items loaded so far.
  items,

  // Callback function responsible for loading the next page of items.
  loadNextPage,

  sortOrder
}) => {
  // We create a reference for the InfiniteLoader
  const listRef = useRef(null);
  const hasMountedRef = useRef(false);
  const [selected, setSelected] = useState([]);

  // Each time the sort prop changed we called the method resetloadMoreItemsCache to clear the cache
  useEffect(() => {
    if (listRef.current && hasMountedRef.current) {
      listRef.current.resetloadMoreItemsCache();
    }
    hasMountedRef.current = true;
  }, [sortOrder]);

  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? items.length + 1 : items.length;

  // Only load 1 page of items at a time.
  // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
  const loadMoreItems = isNextPageLoading ? () => {} : loadNextPage;

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = index => !hasNextPage || index < items.length;

  const onChange = (e, item) => {
    const newSelected = e.target.checked
      ? [item, ...selected]
      : selected.filter(el => el.name !== item.name);
    //console.log("OnChange", newSelected);
    setSelected(newSelected);
  };

  const onSelect = (e, item) => {
    console.log("onSelect", item);
  };

  // Render an item or a loading indicator.
  const Item = ({ index, style }) => {
    let content;
    if (!isItemLoaded(index)) {
      content = "Loading...";
    } else {
      const item = items[index];
      const checked = selected.findIndex(el => el.id === item.id) > -1;
      const newStyle = Object.assign({}, style, {
        padding: "0 0.5rem",
        lineHeight: "30px"
      });
      //console.log("Item render", item, checked, selected);
      content = (
        <Checkbox
            key={item.id}
            label={item.name}
            isChecked={checked}
            className="option_checkbox"
            onChange={e => onChange(e, item)}
          />
        /*<>
          <input
            id={item.id}
            type="checkbox"
            onChange={e => onChange(e, item)}
            checked={checked}
          />
          <label htmlFor={item.id}>{item.name}</label>
        </>*/
        /*<ADSelectorRow 
          key={item.id}
          label={item.name}
          isChecked={checked}
          isMultiSelect={true}
          isSelected={false}
          className="ListItem"
          style={{ padding: "0 0.5rem", lineHeight: "30px" }}
          onChange={e => onChange(e, item)}
          onSelect={e => onSelect(e, item)}
        />*/
      );
    }

    return <div style={style}>{content}</div>;
  };

  // We passed down the ref to the InfiniteLoader component
  return (
    <InfiniteLoader
      ref={listRef}
      isItemLoaded={isItemLoaded}
      itemCount={itemCount}
      loadMoreItems={loadMoreItems}
    >
      {({ onItemsRendered, ref }) => (
        <List
          className="List"
          style={{border: "1px solid #ddd", borderRadius: "0.25rem"}}
          height={300}
          itemCount={itemCount}
          itemSize={30}
          onItemsRendered={onItemsRendered}
          ref={ref}
          width={500}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Item}
        </List>
      )}
    </InfiniteLoader>
  );
}

class ADSelectorExample extends React.PureComponent {
  state = {
    hasNextPage: true,
    isNextPageLoading: false,
    sortOrder: "asc",
    items: []
  };

  constructor(props) {
    super(props);
    faker.seed(123);
    this.persons = new Array(1000)
      .fill(true)
      .map(() => ({ name: name.findName(), id: uniqueId() }));
    this.persons.sort((a, b) => a.name.localeCompare(b.name));
  }

  _loadNextPage = (...args) => {
    this.setState({ isNextPageLoading: true }, () => {
      setTimeout(() => {
        this.setState(state => ({
          hasNextPage: state.items.length < 100,
          isNextPageLoading: false,
          items: [...state.items].concat(
            this.persons.slice(args[0], args[0] + 10)
          )
        }));
      }, 2500);
    });
  };

  _handleSortOrderChange = e => {
    this.persons.sort((a, b) => {
      if (e.target.value === "asc") {
        return a.name.localeCompare(b.name);
      }
      return b.name.localeCompare(a.name);
    });
    this.setState({
      sortOrder: e.target.value,
      items: []
    });
  };

  render() {
    const { hasNextPage, isNextPageLoading, items, sortOrder } = this.state;
    return (
      <>
        <div style={{position: "relative"}}>
          <select onChange={this._handleSortOrderChange}>
            <option value="asc">ASC</option>
            <option value="desc">DESC</option>
          </select>
        </div>
          <ExampleWrapper
            hasNextPage={hasNextPage}
            isNextPageLoading={isNextPageLoading}
            items={items}
            sortOrder={sortOrder}
            loadNextPage={this._loadNextPage}
          />
      </>
    );
  }
}

storiesOf("Components|AdvancedSelector", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("only list", () => {
    return (
      <Section>
        <ADSelectorExample />
      </Section>
    );
  });
