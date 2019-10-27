import React from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../../../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import ADSelectorBodyRow from "./row";
import findIndex from "lodash/findIndex";

class ADSelectorMainBody extends React.Component {


    renderRow = ({ data, index, style }) => {
        const option = data[index];
        var isChecked = this.props.isMultiSelect ? (
            this.state.selectedAll ||
            findIndex(this.state.selectedOptions, { key: option.key }) > -1) : undefined;

        //console.log("renderRow", option, isChecked, this.state.selectedOptions);
        return (
            <ADSelectorBodyRow
                key={option.key}
                label={option.label}
                isChecked={isChecked}
                style={style}
                onChange={this.props.onRowChecked.bind(this, option)}
                onSelect={this.props.onRowSelect.bind(this, option)}
            />
        );
    };

    render() {
        const { total, isNextPageLoading, listHeight, listWidth, itemHeight, isItemLoaded, loadNextPage } = this.props;
        return (
            <div>
                <InfiniteLoader
                    isItemLoaded={isItemLoaded}
                    itemCount={total}
                    loadMoreItems={isNextPageLoading ? () => { console.log("loadMoreItems"); } : loadNextPage}
                >
                    {({ onItemsRendered, ref }) => (
                        <FixedSizeList
                            className="options_list"
                            height={listHeight}
                            width={listWidth}
                            itemSize={itemHeight}
                            //itemCount={this.props.options.length}
                            //itemData={this.props.options}
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
    isMultiSelect: PropTypes.bool,
    total: PropTypes.number,
    isItemLoaded: PropTypes.func,
    isNextPageLoading: PropTypes.bool,
    listHeight: PropTypes.number,
    listWidth: PropTypes.number,
    itemHeight: PropTypes.number,
    loadNextPage: PropTypes.func,
    onRowChecked: PropTypes.func,
    onRowSelect: PropTypes.func,
}

export default ADSelectorMainBody;