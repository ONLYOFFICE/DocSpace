import React from "react";
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
import DropDown from "../drop-down";
import { handleAnyClick } from "../../utils/event";
import isEmpty from 'lodash/isEmpty';

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
  onChangeGroup,
  isOpen,
  isDropDown,
  containerWidth,
  containerHeight,
  ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
  display: flex;
  flex-direction: column;

  ${props => (props.containerWidth ? `width: ${props.containerWidth}px;` : "")}
  ${props =>
    props.containerHeight
      ? `height: ${props.containerHeight}px;`
      : ""}

  .data_container {
    margin: 16px 16px 0 16px;

    .options_searcher {
      margin-bottom: 12px;
    }

    .options_group_selector {
      margin-bottom: 12px;
    }

    .option_select_all_checkbox {
      margin-bottom: 12px;
      /*margin-left: 8px;*/
    }

    .options_list {
      .option {
        line-height: 32px;
        cursor: pointer;

        .option_checkbox {
          /*margin-left: 8px;*/
        }

        .option_link {
          padding-left: 8px;
        }

        /*&:hover {
          background-color: #eceef1;
        }*/
      }
    }
  }

  .button_container {
      border-top: 1px solid #eceef1;
      display: flex;

      .add_members_btn {
        margin: 16px;
      }
    }
`;

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    const groups = this.convertGroups(this.props.groups);
    const currentGroup = this.getCurrentGroup(groups);

    this.state = {
      selectedOptions: this.props.selectedOptions || [],
      selectedAll: this.props.selectedAll || false,
      groups: groups,
      currentGroup: currentGroup
    };

    if (props.isOpen) handleAnyClick(true, this.handleClick);
  }

  handleClick = e => {
    if (this.props.isOpen && !this.ref.current.contains(e.target)) {
      this.props.onSelect && this.props.onSelect(this.state.selectedOptions);
    }
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps) {
    let newState = {};

    if (!isArrayEqual(this.props.selectedOptions, prevProps.selectedOptions)) {
      newState = { selectedOptions: this.props.selectedOptions };
    }

    if (this.props.isMultiSelect !== prevProps.isMultiSelect) {
      newState = Object.assign({}, newState, {
        selectedOptions: []
      });
    }

    if (this.props.selectedAll !== prevProps.selectedAll) {
      newState = Object.assign({}, newState, {
        selectedAll: this.props.selectedAll
      });
    }

    if (!isArrayEqual(this.props.groups, prevProps.groups)) {
      const groups = this.convertGroups(this.props.groups);
      const currentGroup = this.getCurrentGroup(groups);
      newState = Object.assign({}, newState, {
        groups, currentGroup
      });
    }

    if(!isEmpty(newState)) {
      this.setState({ ...this.state, ...newState });
    }

    if (this.props.isOpen !== prevProps.isOpen) {
      handleAnyClick(this.props.isOpen, this.handleClick);
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

  getCurrentGroup = groups => {
    const currentGroup = groups.length > 0 ? groups[0] : "No groups";
    return currentGroup;
  };

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

  renderBody = () => {
    const {
      value,
      placeholder,
      isDisabled,
      onSearchChanged,
      options,
      isMultiSelect,
      buttonLabel,
      selectAllLabel,
      size
    } = this.props;

    const { selectedOptions, selectedAll, currentGroup, groups } = this.state;

    const containerHeight = size === "compact" ? (!groups || !groups.length ? 336 : 326) : 545;
    const containerWidth = size === "compact" ? (!groups || !groups.length ? 325 : 326) : 690;
    const listHeight = size === "compact" ? (!groups || !groups.length ? 176 : 120) : 345;
    const itemHeight = 32;

    return (
      <StyledContainer
        containerHeight={containerHeight}
        containerWidth={containerWidth}
        {...this.props}
      >
        <div className="data_container" ref={this.ref}>
          <SearchInput
            className="options_searcher"
            isDisabled={isDisabled}
            size="base"
            scale={true}
            isNeedFilter={false}
            placeholder={placeholder}
            value={value}
            onChange={onSearchChanged}
            onClearSearch={onSearchChanged.bind(this, "")}
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
              isChecked={
                selectedAll || selectedOptions.length === options.length
              }
              isIndeterminate={!selectedAll && selectedOptions.length > 0}
              className="option_select_all_checkbox"
              onChange={this.onSelectedAllChange}
            />
          )}
          <FixedSizeList
            className="options_list"
            height={listHeight}
            itemSize={itemHeight}
            itemCount={this.props.options.length}
            itemData={this.props.options}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {this.renderRow.bind(this)}
          </FixedSizeList>
        </div>
        {isMultiSelect && (
          <div className="button_container">
            <Button
              className="add_members_btn"
              primary={true}
              size="big"
              label={buttonLabel}
              scale={true}
              isDisabled={
                !this.state.selectedOptions ||
                !this.state.selectedOptions.length
              }
              onClick={this.onButtonClick}
            />
          </div>
        )}
      </StyledContainer>
    );
  };

  render() {
    const { isDropDown, isOpen, options } = this.props;
    const { currentGroup } = this.state;
    console.log("AdvancedSelector render()", currentGroup, options);

    return isDropDown ? (
      <DropDown opened={isOpen}>{this.renderBody()}</DropDown>
    ) : (
      this.renderBody()
    );
  }
}

AdvancedSelector.propTypes = {
  value: PropTypes.string,
  placeholder: PropTypes.string,
  isMultiSelect: PropTypes.bool,
  size: PropTypes.oneOf(["compact", "full"]),
  maxHeight: PropTypes.number,
  isDisabled: PropTypes.bool,
  onSearchChanged: PropTypes.func,
  options: PropTypes.array.isRequired,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,
  selectedAll: PropTypes.bool,
  selectAllLabel: PropTypes.string,
  buttonLabel: PropTypes.string,
  onSelect: PropTypes.func,
  onChangeGroup: PropTypes.func,
  isDropDown: PropTypes.bool,
  isOpen: PropTypes.bool
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "compact",
  buttonLabel: "Add members",
  selectAllLabel: "Select all"
};

export default AdvancedSelector;
