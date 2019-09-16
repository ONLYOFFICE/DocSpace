import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import SearchInput from "../search-input";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import Link from "../link";
import Checkbox from "../checkbox";
import Button from "../button";
import { Icons } from "../icons";
import ComboBox from "../combobox";
import { Text } from "../text";
import findIndex from "lodash/findIndex";
import filter from "lodash/filter";
import isEqual from "lodash/isEqual";
import DropDown from "../drop-down";
import { handleAnyClick } from "../../utils/event";
import isEmpty from "lodash/isEmpty";

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
  allowCreation,
  onAddNewClick,
  allowAnyClickClose,
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
    margin: 16px 16px -5px 16px;

    .options_searcher {
      display: inline-block;
      ${props =>
        props.allowCreation ?
        css`
          width: 272px;
          margin-right: 8px;
        `
        : css`width: 313px;`
        }
    }

    .add_new_btn {
      ${props =>
        props.allowCreation &&
        css`
          display: inline-block;
          vertical-align: top;
          height: 32px;
          width: 32px;
        `}
    }

    .options_group_selector {
      margin-bottom: 12px;
    }

    .data_column_one {
      ${props =>
        props.isDropDown && props.groups && props.groups.length > 0
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
      }
    }

    .data_column_two {
      ${props =>
        props.isDropDown && props.groups && props.groups.length > 0
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
            background-color: #eceef1;
          }
        }
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
    if (
      this.props.isOpen &&
      this.props.allowAnyClickClose &&
      this.ref &&
      this.ref.current &&
      !this.ref.current.contains(e.target) &&
      e.target.className.indexOf("option_checkbox") === -1
    ) {
      this.props.onCancel && this.props.onCancel();
    }
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen) {
      handleAnyClick(this.props.isOpen, this.handleClick);
    }

    if(this.props.allowAnyClickClose !== prevProps.allowAnyClickClose) {
      handleAnyClick(this.props.allowAnyClickClose, this.handleClick);
    }

    let newState = {};

    if (!isEqual(this.props.selectedOptions, prevProps.selectedOptions)) {
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

    if (!isEqual(this.props.groups, prevProps.groups)) {
      const groups = this.convertGroups(this.props.groups);
      const currentGroup = this.getCurrentGroup(groups);
      newState = Object.assign({}, newState, {
        groups,
        currentGroup
      });
    }

    if (!isEmpty(newState)) {
      this.setState({ ...this.state, ...newState });
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
      size,
      isDropDown,
      onAddNewClick,
      allowCreation
    } = this.props;

    const {
      selectedOptions,
      selectedAll,
      currentGroup,
      groups,
    } = this.state;

    const containerHeight =
      size === "compact" ? (!groups || !groups.length ? 336 : 326) : 614;
    const containerWidth =
      size === "compact" ? (!groups || !groups.length ? 325 : 326) : 690;
    const listHeight =
      size === "compact"
        ? !groups || !groups.length
          ? isMultiSelect
            ? 176
            : 226
          : 120
        : 488;
    const listWidth = 320;

    const itemHeight = 32;

    return (
      <StyledContainer
        containerHeight={containerHeight}
        containerWidth={containerWidth}
        {...this.props}
      >
        <div ref={this.ref}>
          <div className="data_container">
            <div className="data_column_one">
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
              {allowCreation && (
                <Button
                  className="add_new_btn"
                  primary={false}
                  size="base"
                  label=""
                  icon={
                    <Icons.PlusIcon
                      size="medium"
                      isfill={true}
                      color="#D8D8D8"
                    />
                  }
                  onClick={onAddNewClick}
                />
              )}
              {isDropDown &&
                size === "compact" &&
                groups &&
                groups.length > 0 && (
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
              {isMultiSelect && !groups && !groups.length && (
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
                width={listWidth}
                itemSize={itemHeight}
                itemCount={this.props.options.length}
                itemData={this.props.options}
                outerElementType={CustomScrollbarsVirtualList}
              >
                {this.renderRow.bind(this)}
              </FixedSizeList>
            </div>
            {isDropDown && size === "full" && groups && groups.length > 0 && (
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
                  itemCount={this.props.groups.length}
                  itemData={this.props.groups}
                  outerElementType={CustomScrollbarsVirtualList}
                >
                  {this.renderRow.bind(this)}
                </FixedSizeList>
              </div>
            )}
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
        </div>
      </StyledContainer>
    );
  };

  render() {
    const { isDropDown, isOpen } = this.props;
    //console.log("AdvancedSelector render()");

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
  onCancel: PropTypes.func,
  isDropDown: PropTypes.bool,
  isOpen: PropTypes.bool,
  allowCreation: PropTypes.bool,
  onAddNewClick: PropTypes.func,
  allowAnyClickClose: PropTypes.bool
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "compact",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowAnyClickClose: true
};

export default AdvancedSelector;
