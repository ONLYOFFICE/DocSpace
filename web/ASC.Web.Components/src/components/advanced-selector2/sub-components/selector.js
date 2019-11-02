import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Checkbox from "../../checkbox";
//import ComboBox from "../../combobox";
import SearchInput from "../../search-input";
import Loader from "../../loader";
import { Text } from "../../text";
import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";
import ADSelectorColumn from "./column";
import ADSelectorFooter from "./footer";
import ADSelectorHeader from "./header";
import ADSelectorBody from "./body";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from 'react-virtualized-auto-sizer';

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    display: grid;

    ${props => props.displayType === "dropdown" ? css`
        grid-auto-rows: max-content;
        grid-template-areas: "column1 column2" "footer footer";

        .column2 { 
            grid-area: column2; 

            display: grid;
            background-color: gold;
            padding: 16px 16px 0 16px;
            grid-row-gap: 16px;

            grid-template-columns: 1fr;
            grid-template-rows: 30px 1fr;
            grid-template-areas: "header2" "body2";

            .header2 {
                grid-area: header2; 
                background-color: cyan;
            }

            .body2 {
                grid-area: body2;
                background-color: white;
            }
        }
    `
        : css`
        height: 100%;
        grid-template-columns: 1fr;
        grid-template-rows: 1fr 69px;
        grid-template-areas: "column1" "footer";
    `}   

    .column1 { 
        grid-area: column1;

        display: grid;
        background-color: red;
        padding: 16px 16px 0 16px;
        grid-row-gap: 16px;

        grid-template-columns: 1fr;
        grid-template-rows: 30px 1fr;
        grid-template-areas: "header1" "body1";

        .header1 {
            grid-area: header1; 
            background-color: white;
        }

        .body1 {
            grid-area: body1;
            background-color: white;
        }
    }

    .row-block {
        line-height: 32px;
        cursor: pointer;

        &:hover {
            background-color: #f8f9f9;
        }
    }

    .row-block.selected {
        background-color: #ECEEF1;
    }

    .footer { 
        grid-area: footer; 
    }
`;



const ADSelector = props => {
    const { displayType, groups, selectButtonLabel, 
        isDisabled, isMultiSelect, hasNextPage, options, 
        isNextPageLoading, loadNextPage, 
        selectedOptions, selectedGroups,
        groupsHeaderLabel, searchPlaceHolderLabel} = props;

    const listRef = useRef(null);
    const [selectedOptionList, setSelectedOptionList] = useState(selectedOptions || []);
    const [selectedGroupList, setSelectedGroupList] = useState(selectedGroups || []);
    const [searchValue, setSearchValue] = useState("");

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

    // Every row is loaded except for our loading indicator row.
    const isItemLoaded = (index) => {
        return !hasNextPage || index < options.length
    };

    const onChange = (e) => {
        const option = options[+e.target.value];
        const newSelected = e.target.checked
            ? [option, ...selectedOptionList]
            : selectedOptionList.filter(el => el.key !== option.key);
        setSelectedOptionList(newSelected);
    };

    const onGroupChange = (e) => {
        const group = groups[+e.target.value];
        const newSelectedGroups = e.target.checked
            ? [group, ...selectedGroupList]
            : selectedGroupList.filter(el => el.key !== group.key);
        //console.log("onGroupChange", item);
        setSelectedGroupList(newSelectedGroups);
        setCurrentGroup(group);
    };

    const onSearchChange = (e) => {
        setSearchValue(e.target.value);
    };

    const onSearchReset = () => {
        setSearchValue("");
    };

    // Render an item or a loading indicator.
    // eslint-disable-next-line react/prop-types
    const renderOption = ({ index, style }) => {
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
            const checked = selectedOptionList.findIndex(el => el.key === option.key) > -1;
            //console.log("Item render", item, checked, selected);
            content = (
                <Checkbox
                    id={option.key}
                    value={`${index}`}
                    label={option.label}
                    isChecked={checked}
                    className="option_checkbox"
                    onChange={onChange}
                />
            );
        }

        return <div style={style} className="row-block">{content}</div>;
    };

    // eslint-disable-next-line react/prop-types
    const renderGroup = ({ index, style }) => {
        const group = groups[index];
        const checked = selectedGroupList.findIndex(el => el.key === group.key) > -1;
        const isSelected = currentGroup.key === group.key;
        return <div style={style} className={`row-block${isSelected ? " selected" : ""}`}>
            <Checkbox
                id={group.key}
                value={`${index}`}
                label={group.label}
                isChecked={checked}
                className="group_checkbox"
                onChange={onGroupChange}
            />
        </div>;

    }

    // If there are more items to be loaded then add an extra row to hold a loading indicator.
    const itemCount = hasNextPage ? options.length + 1 : options.length;

    // Only load 1 page of items at a time.
    // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
    const loadMoreItems = isNextPageLoading ? () => { } : loadNextPage;

    return (
        <StyledContainer displayType={displayType}>
            <ADSelectorColumn className="column1" displayType={displayType}>
                <ADSelectorHeader className="header1">
                    <SearchInput
                        className="options_searcher"
                        isDisabled={isDisabled}
                        size="base"
                        scale={true}
                        isNeedFilter={false}
                        placeholder={searchPlaceHolderLabel}
                        value={searchValue}
                        onChange={onSearchChange}
                        onClearSearch={onSearchReset}
                    />
                </ADSelectorHeader>
                <ADSelectorBody className="body1">
                    <InfiniteLoader
                        ref={listRef}
                        isItemLoaded={isItemLoaded}
                        itemCount={itemCount}
                        loadMoreItems={loadMoreItems}
                    >
                        {({ onItemsRendered, ref }) => (
                            <AutoSizer>
                                {({ height, width }) => (
                                    <List
                                        className="options_list"
                                        height={height}
                                        itemCount={itemCount}
                                        itemSize={32}
                                        onItemsRendered={onItemsRendered}
                                        ref={ref}
                                        width={width}
                                        outerElementType={CustomScrollbarsVirtualList}
                                    >
                                        {renderOption}
                                    </List>
                                )}
                            </AutoSizer>
                        )}
                    </InfiniteLoader>
                </ADSelectorBody>
            </ADSelectorColumn>
            {displayType === "dropdown" && groups && groups.length > 0 &&
                <ADSelectorColumn className="column2" displayType={displayType}>
                    <ADSelectorHeader className="header2">
                        <Text.Body as="p" className="group_header" fontSize={15} isBold={true}>
                            {groupsHeaderLabel}
                        </Text.Body>
                    </ADSelectorHeader>
                    <ADSelectorBody className="body2">
                        <AutoSizer>
                            {({ height, width }) => (
                                <List
                                    className="group_list"
                                    height={height}
                                    width={width}
                                    itemSize={32}
                                    itemCount={groups.length}
                                    itemData={groups}
                                    outerElementType={CustomScrollbarsVirtualList}
                                >
                                    {renderGroup}
                                </List>
                            )}
                        </AutoSizer>
                    </ADSelectorBody>
                </ADSelectorColumn>
            }
            <ADSelectorFooter
                className="footer"
                selectButtonLabel={selectButtonLabel}
                isDisabled={isDisabled}
                isVisible={isMultiSelect && selectedOptionList.length > 0}
            />
        </StyledContainer>
    );
}

ADSelector.propTypes = {
    options: PropTypes.array,
    groups: PropTypes.array,

    hasNextPage: PropTypes.bool,
    isNextPageLoading: PropTypes.bool,
    loadNextPage: PropTypes.func,

    isDisabled: PropTypes.bool,
    isMultiSelect: PropTypes.bool,

    selectButtonLabel: PropTypes.string,
    selectAllLabel: PropTypes.string,
    searchPlaceHolderLabel: PropTypes.string,
    groupsHeaderLabel: PropTypes.string,

    //size: PropTypes.oneOf(["compact", "full"]),
    displayType: PropTypes.oneOf(["dropdown", "aside"]),

    selectedOptions: PropTypes.array,
    selectedGroups: PropTypes.array,

    onSelect: PropTypes.func,
    onSearchChanged: PropTypes.func
};

export default ADSelector;