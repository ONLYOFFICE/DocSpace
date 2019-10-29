import React from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../../../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import { Text } from "../../../../text";
import ADSelectorRow from "../../row";
import findIndex from "lodash/findIndex";

class ADSelectorAdditionalBody extends React.Component {


    renderRow = ({ data, index, style }) => {
        const option = data[index];
        var isChecked = this.props.isMultiSelect ? (
            this.state.selectedAll ||
            findIndex(this.state.selectedOptions, { key: option.key }) > -1) : undefined;

        //console.log("renderRow", option, isChecked, this.state.selectedOptions);
        return (
            <ADSelectorRow
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
        const { groups, listHeight, itemHeight } = this.props;
        return (
            <div className="data_column_two">
                      <Text.Body
                        as="p"
                        className="group_header"
                        fontSize={15}
                        isBold={true}
                      >
                        Groups
                      </Text.Body>
                      <FixedSizeList
                        className="group_list"
                        height={listHeight}
                        itemSize={itemHeight}
                        itemCount={groups.length}
                        itemData={groups}
                        outerElementType={CustomScrollbarsVirtualList}
                      >
                        {this.renderRow.bind(this)}
                      </FixedSizeList>
                    </div>
        );
    }
}

ADSelectorAdditionalBody.propTypes = {
    groups: PropTypes.array,
    listHeight: PropTypes.number,
    itemHeight: PropTypes.number
}

export default ADSelectorAdditionalBody;