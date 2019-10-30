import React from "react";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import ADSelectorRow from "../row";
import findIndex from "lodash/findIndex";

class ADSelectorGroupsBody extends React.Component {
  renderRow = ({ data, index, style }) => {
    const {isMultiSelect, selectedAll, selectedOptions, currentGroup} = this.props;

    const option = data[index];
    const isChecked = isMultiSelect
      ? selectedAll ||
        findIndex(selectedOptions, { key: option.key }) > -1
      : undefined;
    const isSelected = currentGroup.key === option.key;

    //console.log("renderRow", option, isChecked, this.state.selectedOptions);
    return (
      <ADSelectorRow
        key={option.key}
        label={option.label}
        isChecked={isChecked}
        isMultiSelect={isMultiSelect}
        style={style}
        isSelected={isSelected}
        onChange={this.props.onRowChecked.bind(this, option)}
      />
    );
  };

  render() {
    const { options, listHeight, itemHeight } = this.props;
    return (
        <FixedSizeList
          className="group_list"
          height={listHeight}
          itemSize={itemHeight}
          itemCount={options.length}
          itemData={options}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {this.renderRow.bind(this)}
        </FixedSizeList>

    );
  }
}

ADSelectorGroupsBody.propTypes = {
  options: PropTypes.array,
  selectedOptions: PropTypes.array,
  selectedAll: PropTypes.bool,
  currentGroup: PropTypes.object,
  isMultiSelect: PropTypes.bool,
  listHeight: PropTypes.number,
  itemHeight: PropTypes.number,
  onRowChecked: PropTypes.func,
  onRowSelect: PropTypes.func
};

export default ADSelectorGroupsBody;
