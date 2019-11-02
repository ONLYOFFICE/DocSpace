import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Checkbox from "../../checkbox";
//import ComboBox from "../../combobox";
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

            grid-template-columns: 1fr;
            grid-template-rows: 64px 1fr;
            grid-template-areas: "header2" "body2";

            .header2 {
                grid-area: header2; 
                background-color: white;
            }

            .body2 {
                grid-area: body2;
                background-color: cyan;
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

        grid-template-columns: 1fr;
        grid-template-rows: 64px 1fr;
        grid-template-areas: "header1" "body1";

        .header1 {
            grid-area: header1; 
            background-color: lightblue;
        }

        .body1 {
            grid-area: body1;
            background-color: white;
        }
    }

    .footer { 
        grid-area: footer; 
    }
`;



const ADSelector = props => {
    const { displayType, groups, selectButtonLabel, isDisabled, isMultiSelect, hasNextPage, options, isNextPageLoading, loadNextPage, selectedOptions } = props;

    const listRef = useRef(null);
    //const hasMountedRef = useRef(false);
    const [selected, setSelected] = useState(selectedOptions || []);

    // Every row is loaded except for our loading indicator row.
    const isItemLoaded = (index) => {
        return !hasNextPage || index < options.length
    };

    const onChange = (e) => {
        const option = options[+e.target.value];
        const newSelected = e.target.checked
            ? [option, ...selected]
            : selected.filter(el => el.key !== option.key);
        setSelected(newSelected);
    };

    // Render an item or a loading indicator.
    // eslint-disable-next-line react/prop-types
    const renderRow = ({ index, style }) => {
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
                    id={option.key}
                    value={`${index}`}
                    label={option.label}
                    isChecked={checked}
                    className="option_checkbox"
                    onChange={onChange}
                />
            );
        }

        return <div style={style}>{content}</div>;
    };

    // If there are more items to be loaded then add an extra row to hold a loading indicator.
    const itemCount = hasNextPage ? options.length + 1 : options.length;

    // Only load 1 page of items at a time.
    // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
    const loadMoreItems = isNextPageLoading ? () => { } : loadNextPage;

    return (
        <StyledContainer displayType={displayType}>
            <ADSelectorColumn className="column1" displayType={displayType}>
                <ADSelectorHeader className="header1">
                    <span>Header 1</span>
                </ADSelectorHeader>
                <ADSelectorBody className="body1">
                    <AutoSizer>
                        {({ height, width }) => (
                            <InfiniteLoader
                                ref={listRef}
                                isItemLoaded={isItemLoaded}
                                itemCount={itemCount}
                                loadMoreItems={loadMoreItems}
                            >
                                {({ onItemsRendered, ref }) => (
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
                                        {renderRow}
                                    </List>
                                )}
                            </InfiniteLoader>
                        )}
                    </AutoSizer>
                </ADSelectorBody>
            </ADSelectorColumn>
            {displayType === "dropdown" && groups && groups.length > 0 &&
                <ADSelectorColumn className="column2" displayType={displayType}>
                    <ADSelectorHeader className="header2">
                        <span>Header 2</span>
                    </ADSelectorHeader>
                    <ADSelectorBody className="body2">
                        <span>Body 2</span>
                    </ADSelectorBody>
                </ADSelectorColumn>
            }
            <ADSelectorFooter
                className="footer"
                selectButtonLabel={selectButtonLabel}
                isDisabled={isDisabled}
                isVisible={isMultiSelect && selected.length > 0}
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

    //size: PropTypes.oneOf(["compact", "full"]),
    displayType: PropTypes.oneOf(["dropdown", "aside"]),

    selectedOptions: PropTypes.array,
    selectedGroups: PropTypes.array,

    onSelect: PropTypes.func,
    onSearchChanged: PropTypes.func
};

export default ADSelector;