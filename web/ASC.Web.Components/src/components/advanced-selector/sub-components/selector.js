import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import Checkbox from "../../checkbox";
import ComboBox from "../../combobox";
import Loader from "../../loader";
import { Text } from "../../text";
import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";
import ADSelectorOptionsHeader from "./options/header";
import ADSelectorGroupsHeader from "./groups/header";
import ADSelectorGroupsBody from "./groups/body";
import ADSelectorFooter from "./footer";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  value,
  placeholder,
  isMultiSelect,
  size,
  width,
  maxHeight,
  isDisabled,
  onSelect,
  onSearchChanged,
  options,
  selectedOptions,
  buttonLabel,
  selectAllLabel,
  groups,
  selectedGroups,
  onGroupSelect,
  onGroupChange,
  isOpen,
  displayType,
  containerWidth,
  containerHeight,
  allowCreation,
  onAddNewClick,
  allowAnyClickClose,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  isSelected,
  ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    display: flex;
    flex-direction: column;
  
    ${props => (props.containerWidth ? `width: ${props.containerWidth};` : "")}
    ${props =>
      props.containerHeight ? `height: ${props.containerHeight};` : ""}
  
    .data_container {
      margin: 16px 16px -5px 16px;
  
      .head_container {
        display: flex;
        margin-bottom: ${props =>
          props.displayType === "dropdown" ? 8 : 16}px;
  
        .options_searcher {
          display: inline-block;
          width: 100%;
  
          ${props =>
            props.displayType === "dropdown" &&
            props.size === "full" &&
            css`
              margin-right: ${props => (props.allowCreation ? 8 : 16)}px;
            `}
          /*${props =>
            props.allowCreation
              ? css`
                  width: 272px;
                  margin-right: 8px;
                `
              : css`
                  width: ${props => (props.isDropDown ? "313px" : "100%")};
                `}*/
        }
  
        .add_new_btn {
          ${props =>
            props.allowCreation &&
            css`
              display: inline-block;
              vertical-align: top;
              height: 32px;
              width: 36px;
              margin-right: 16px;
              line-height: 18px;
            `}
        }
  
      }
  
      .options_group_selector {
        margin-bottom: 12px;
      }
  
      .data_column_one {
        ${props =>
          props.size === "full" &&
          props.displayType === "dropdown" &&
          props.groups &&
          props.groups.length > 0
            ? css`
                width: 50%;
                display: inline-block;
              `
            : ""}
  
        .options_list {
          margin-top: 4px;
          margin-left: -8px;
          .option {
            line-height: 32px;
            padding-left: ${props => (props.isMultiSelect ? 8 : 0)}px;
            cursor: pointer;
  
            .option_checkbox {
              /*margin-left: 8px;*/
            }
  
            .option_link {
              padding-left: 8px;
            }
  
            &:hover {
              background-color: #f8f9f9;
            }
          }
        }
      }
  
      .data_column_two {
        ${props =>
          props.displayType === "dropdown" &&
          props.groups &&
          props.groups.length > 0
            ? css`
                width: 50%;
                display: inline-block;
                border-left: 1px solid #eceef1;
              `
            : ""}
  
        .group_header {
          font-weight: 600;
          padding-left: 16px;
          padding-bottom: 14px;
        }
  
        .group_list {
          margin-left: 8px;
  
          .option {
            line-height: 32px;
            padding-left: 8px;
            cursor: pointer;
  
            .option_checkbox {
              /*margin-left: 8px;*/
            }
  
            .option_link {
              padding-left: 8px;
            }
  
            &:hover {
              background-color: #f8f9f9;
            }
          }
  
          .option.selected {
            background-color: #ECEEF1;
          }
        }
      }
    }
  `;

const ADSelector = props => {
  const {
    options,
    groups,
    hasNextPage,
    isNextPageLoading,
    loadNextPage,
    value,
    placeholder,
    isDisabled,
    onSearchChanged,
    isMultiSelect,
    buttonLabel,
    selectAllLabel,
    size,
    displayType,
    onAddNewClick,
    allowCreation,
    selectedOptions,
    selectedGroups,
    onSelect
  } = props;

  // We create a reference for the InfiniteLoader
  const listRef = useRef(null);
  //const hasMountedRef = useRef(false);
  const [selected, setSelected] = useState(selectedOptions || []);
  const [selectedGrps, setSelectedGroups] = useState(selectedGroups || []);
  const [selectedAll, setSelectedAll] = useState(false);

  const convertGroups = items => {
    if (!items) return [];

    const wrappedGroups = items.map(convertGroup);

    return wrappedGroups;
  };

  const convertGroup = group => {
    return {
      key: group.key,
      label: `${group.label} (${group.total})`,
      total: group.total
    };
  };

  const getCurrentGroup = items => {
    const currentGroup = items.length > 0 ? items[0] : "No groups";
    return currentGroup;
  };

  const convertedGroups = convertGroups(groups);
  const curGroup = getCurrentGroup(convertedGroups);

  const [currentGroup, setCurrentGroup] = useState(curGroup);

  // Each time the sort prop changed we called the method resetloadMoreItemsCache to clear the cache
  /*useEffect(() => {
    if (listRef.current && hasMountedRef.current) {
      listRef.current.resetloadMoreItemsCache();
    }
    hasMountedRef.current = true;
  }, [sortOrder]);*/

  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? options.length + 1 : options.length;

  // Only load 1 page of items at a time.
  // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
  const loadMoreItems = isNextPageLoading ? () => {} : loadNextPage;

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = index => !hasNextPage || index < options.length;

  const onChange = (e, item) => {
    const newSelected = e.target.checked
      ? [item, ...selected]
      : selected.filter(el => el.name !== item.name);
    //console.log("OnChange", newSelected);
    setSelected(newSelected);
  };

  const onGroupChange = (e, item) => {
    const newSelectedGroups = e.target.checked
      ? [item, ...selectedGrps]
      : selectedGrps.filter(el => el.name !== item.name);
    //console.log("onGroupChange", item);
    setSelectedGroups(newSelectedGroups);
  };
  const onCurrentGroupChange = (e, item) => {
    //console.log("onCurrentGroupChange", item);
    setCurrentGroup(item);
  };
  const onSelectedAllChange = (e, item) => {
    //console.log("onSelectedAllChange", item);
    setSelectedAll(item);
  };

  const onSearchReset = () => {
    onSearchChanged && onSearchChanged("");
  }

  const onButtonClick = () => {
    onSelect && onSelect(selectedAll ? options : selectedOptions);
  };

  // Render an item or a loading indicator.
  const Item = ({ index, style }) => {
    let content;
    if (!isItemLoaded(index)) {
      content = <div className="option" style={style} key="loader">
      <Loader
        type="oval"
        size={16}
        style={{
          display: "inline",
          marginRight: "10px"
        }}
      />
      <Text.Body as="span">Loading... Please wait...</Text.Body>
    </div>;
    } else {
      const option = options[index];
      const checked = selected.findIndex(el => el.key === option.key) > -1;
      //console.log("Item render", item, checked, selected);
      content = (
        <Checkbox
          key={option.key}
          label={option.label}
          isChecked={checked}
          className="option_checkbox"
          onChange={e => onChange(e, option)}
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

  let containerHeight;
  let containerWidth;
  let listHeight;
  let listWidth;
  const itemHeight = 32;
  const hasGroups = convertedGroups && convertedGroups.length > 0;

  switch (size) {
    case "compact":
      containerHeight = hasGroups ? "326px" : "100%";
      containerWidth = "379px";
      listWidth = displayType === "dropdown" ? 356 : 356;
      listHeight = hasGroups ? 488 : isMultiSelect ? 176 : 226;
      break;
    case "full":
    default:
      containerHeight = "100%";
      containerWidth = displayType === "dropdown" ? "690px" : "326px";
      listWidth = displayType === "dropdown" ? 320 : 300;
      listHeight = 488;
      break;
  }

  // We passed down the ref to the InfiniteLoader component
  return (
    <StyledContainer
        containerHeight={containerHeight}
        containerWidth={containerWidth}
        {...props}
    >
      <div className="data_container">
        <div className="data_column_one">
          <ADSelectorOptionsHeader
            value={value}
            searchPlaceHolder={placeholder}
            isDisabled={isDisabled}
            allowCreation={allowCreation}
            onAddNewClick={onAddNewClick}
            onChange={onSearchChanged}
            onClearSearch={onSearchReset}
          />
          {displayType === "aside" && convertedGroups && convertedGroups.length > 0 && (
            <ComboBox
              className="options_group_selector"
              isDisabled={isDisabled}
              options={convertedGroups}
              selectedOption={currentGroup}
              dropDownMaxHeight={200}
              scaled={true}
              scaledOptions={true}
              size="content"
              onSelect={onCurrentGroupChange}
            />
          )}
          {isMultiSelect && !convertedGroups && !convertedGroups.length && (
            <Checkbox
              label={selectAllLabel}
              isChecked={
                selectedAll || selectedOptions.length === options.length
              }
              isIndeterminate={!selectedAll && selectedOptions.length > 0}
              className="option_select_all_checkbox"
              onChange={onSelectedAllChange}
            />
          )}
          <InfiniteLoader
            ref={listRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                className="options_list"
                //style={{ border: "1px solid #ddd", borderRadius: "0.25rem" }}
                height={listHeight}
                itemCount={itemCount}
                itemSize={itemHeight}
                onItemsRendered={onItemsRendered}
                ref={ref}
                width={listWidth}
                outerElementType={CustomScrollbarsVirtualList}
              >
                {Item}
              </List>
            )}
          </InfiniteLoader>
        </div>
        {displayType === "dropdown" &&
          size === "full" &&
          convertedGroups &&
          convertedGroups.length > 0 && (
            <div className="data_column_two">
              <ADSelectorGroupsHeader headerLabel="Groups" />
              <ADSelectorGroupsBody
                options={convertedGroups}
                selectedOptions={selectedGrps}
                isMultiSelect={isMultiSelect}
                currentGroup={currentGroup}
                listHeight={listHeight}
                itemHeight={itemHeight}
                onRowChecked={onGroupChange}
              />
            </div>
          )}
      </div>
      <ADSelectorFooter
        buttonLabel={buttonLabel}
        isDisabled={!selectedOptions || !selectedOptions.length}
        isMultiSelect={isMultiSelect}
        onClick={onButtonClick}
        selectedOptions={selectedOptions}
      />
    </StyledContainer>
  );
};

ADSelector.propTypes = {
  isOpen: PropTypes.bool,
  options: PropTypes.array,
  groups: PropTypes.array,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,
  value: PropTypes.string,
  placeholder: PropTypes.string,
  isDisabled: PropTypes.bool,
  onSearchChanged: PropTypes.func,
  isMultiSelect: PropTypes.bool,
  buttonLabel: PropTypes.string,
  selectAllLabel: PropTypes.string,
  size: PropTypes.string,
  displayType: PropTypes.oneOf(["dropdown", "aside"]),
  onAddNewClick: PropTypes.func,
  allowCreation: PropTypes.bool,
  onSelect: PropTypes.func,
  onChange: PropTypes.func,
  onGroupSelect: PropTypes.func,
  onGroupChange: PropTypes.func,
  selectedOptions: PropTypes.array,
  selectedGroups: PropTypes.array,
  selectedAll: PropTypes.bool,
  onCancel: PropTypes.func,
  allowAnyClickClose: PropTypes.bool
};

export default ADSelector;
