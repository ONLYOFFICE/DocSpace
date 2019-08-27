import React, { memo } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import SearchInput from "../search-input";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import Link from "../link";
import Checkbox from "../checkbox";
import Button from "../button";
import ComboBox from "../combobox";
import { isArrayEqual } from "../../utils/array";
import findIndex from "lodash/findIndex";
import filter from "lodash/filter";

const Container = ({
  value,
  placeholder,
  isMultiSelect,
  mode,
  width,
  maxHeight,
  isDisabled,
  onSearchChanged,
  options,
  selectedOptions,
  buttonLabel,
  selectAllLabel,
  groups,
  selectedGroups,
  onChangeGroup,
  ...props
}) => <div {...props} />;

const StyledContainer = styled(Container)`
  ${props => (props.width ? `width: ${props.width}px;` : "")}

  .options_searcher {
    margin-bottom: 12px;
  }

  .options_group_selector {
    margin-bottom: 12px;
  }

  .option_select_all_checkbox {
    margin-bottom: 12px;
    margin-left: 8px;
  }

  .options_list {
    .option {
      line-height: 32px;
      cursor: pointer;

      .option_checkbox {
        margin-left: 8px;
      }

      .option_link {
        padding-left: 8px;
      }

      &:hover {
        background-color: #eceef1;
      }
    }
  }

  .add_members_btn {
    margin: 16px 0;
  }
`;

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    const groups = this.convertGroups(this.props.groups);
    const currentGroup = this.getCurrentGroup(groups);

    this.state = {
      selectedOptions: this.props.selectedOptions || [],
      selectedAll: this.props.selectedAll || false,
      groups: groups,
      currentGroup: currentGroup
    };
  }

  componentDidUpdate(prevProps) {
    if (!isArrayEqual(this.props.selectedOptions, prevProps.selectedOptions)) {
      this.setState({ selectedOptions: this.props.selectedOptions });
    }

    if (this.props.isMultiSelect !== prevProps.isMultiSelect) {
      this.setState({ selectedOptions: [] });
    }

    if (this.props.selectedAll !== prevProps.selectedAll) {
      this.setState({ selectedAll: this.props.selectedAll });
    }

    if (!isArrayEqual(this.props.groups, prevProps.groups)) {
      const groups = this.convertGroups(this.props.groups);
      const currentGroup = this.getCurrentGroup(groups);
      this.setState({ groups, currentGroup });
    }
  }

  convertGroups = groups => {
    if (!groups) return [];

    const wrappedGroups = groups.map(this.convertGroup);

    return wrappedGroups;
  };

  convertGroup = group => {
    return {
      key: group.key,
      label: `${group.label} (${group.total})`,
      total: group.total
    };
  };

  getCurrentGroup = (groups) => {
    const currentGroup = groups.length > 0 ? groups[0] : "No groups";
    return currentGroup;
  }

  onButtonClick = () => {
    this.props.onSelect &&
      this.props.onSelect(
        this.state.selectedAll ? this.props.options : this.state.selectedOptions
      );
  };

  onSelect = option => {
    this.props.onSelect && this.props.onSelect(option);
  };

  onChange = (option, e) => {
    const { selectedOptions } = this.state;
    const newSelectedOptions = e.target.checked
      ? [...selectedOptions, option]
      : filter(selectedOptions, obj => obj.key !== option.key);

    //console.log("onChange", option, e.target.checked, newSelectedOptions);

    this.setState({
      selectedOptions: newSelectedOptions,
      selectedAll: newSelectedOptions.length === this.props.options.length
    });
  };

  onSelectedAllChange = e => {
    this.setState({
      selectedAll: e.target.checked,
      selectedOptions: e.target.checked ? this.props.options : []
    });
  };

  onCurrentGroupChange = group => {
    this.setState({
      currentGroup: group
    });

    this.props.onChangeGroup && this.props.onChangeGroup(group);
  };

  renderRow = ({ data, index, style }) => {
    const option = data[index];
    var isChecked =
      this.state.selectedAll ||
      findIndex(this.state.selectedOptions, { key: option.key }) > -1;

    //console.log("renderRow", option, isChecked, this.state.selectedOptions);
    return (
      <div className="option" style={style} key={option.key}>
        {this.props.isMultiSelect ? (
          <Checkbox
            label={option.label}
            isChecked={isChecked}
            className="option_checkbox"
            onChange={this.onChange.bind(this, option)}
          />
        ) : (
          <Link
            as="span"
            truncate={true}
            className="option_link"
            onClick={this.onSelect.bind(this, option)}
          >
            {option.label}
          </Link>
        )}
      </div>
    );
  };

  render() {
    const {
      value,
      placeholder,
      maxHeight,
      isDisabled,
      onSearchChanged,
      options,
      isMultiSelect,
      buttonLabel,
      selectAllLabel
    } = this.props;

    const { selectedOptions, selectedAll, currentGroup, groups } = this.state;

    console.log("AdvancedSelector render()", currentGroup, options);

    return (
      <StyledContainer {...this.props}>
        <SearchInput
          className="options_searcher"
          isDisabled={isDisabled}
          size="base"
          scale={true}
          isNeedFilter={false}
          placeholder={placeholder}
          value={value}
          onChange={onSearchChanged}
        />
        {groups && groups.length > 0 && (
          <ComboBox
            className="options_group_selector"
            isDisabled={isDisabled}
            options={groups}
            selectedOption={currentGroup}
            dropDownMaxHeight={200}
            scaled={true}
            size="content"
            onSelect={this.onCurrentGroupChange}
          />
        )}
        {isMultiSelect && (
          <Checkbox
            label={selectAllLabel}
            isChecked={selectedAll || selectedOptions.length === options.length}
            isIndeterminate={!selectedAll && selectedOptions.length > 0}
            className="option_select_all_checkbox"
            onChange={this.onSelectedAllChange}
          />
        )}
        <FixedSizeList
          className="options_list"
          height={maxHeight}
          itemSize={32}
          itemCount={options.length}
          itemData={options}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {this.renderRow.bind(this)}
        </FixedSizeList>
        {isMultiSelect && (
          <Button
            className="add_members_btn"
            primary={true}
            size="big"
            label={buttonLabel}
            scale={true}
            isDisabled={
              !this.state.selectedOptions || !this.state.selectedOptions.length
            }
            onClick={this.onButtonClick}
          />
        )}
      </StyledContainer>
    );
  }
}

AdvancedSelector.propTypes = {
  value: PropTypes.string,
  placeholder: PropTypes.string,
  isMultiSelect: PropTypes.bool,
  mode: PropTypes.oneOf(["base", "compact"]),
  width: PropTypes.number,
  maxHeight: PropTypes.number,
  isDisabled: PropTypes.bool,
  onSearchChanged: PropTypes.func,
  options: PropTypes.array.isRequired,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,
  selectedAll: PropTypes.bool,
  buttonLabel: PropTypes.string,
  onSelect: PropTypes.func,
  onChangeGroup: PropTypes.func,
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  width: 325,
  maxHeight: 545,
  mode: "base",
  buttonLabel: "Add members",
  selectAllLabel: "Select all"
};

export default AdvancedSelector;
