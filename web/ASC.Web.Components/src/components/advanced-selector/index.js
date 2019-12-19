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
import Text from "../text";
import findIndex from "lodash/findIndex";
import filter from "lodash/filter";
import isEqual from "lodash/isEqual";
import DropDown from "../drop-down";
import { handleAnyClick } from "../../utils/event";
import isEmpty from "lodash/isEmpty";
import Aside from "../layout/sub-components/aside";

const displayTypes = ["dropdown", "aside"];

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
  displayType,
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
  .dropdown-container {

  }

  .aside-container {

  }
`;

const StyledBodyContainer = styled(Container)`
  display: flex;
  flex-direction: column;

  ${props => (props.containerWidth ? `width: ${props.containerWidth};` : "")}
  ${props => (props.containerHeight ? `height: ${props.containerHeight};` : "")}

  .data_container {
    margin: 16px 16px -5px 16px;

    .head_container {
      display: flex;
      margin-bottom: ${props => (props.displayType === "dropdown" ? 8 : 16)}px;

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
      box-sizing: border-box;
      ${props =>
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
          box-sizing: border-box;
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
      box-sizing: border-box;
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
          box-sizing: border-box;
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
      e &&
      e.target &&
      e.target.className &&
      typeof e.target.className.indexOf === "function" &&
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

    if (this.props.allowAnyClickClose !== prevProps.allowAnyClickClose) {
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
      displayType,
      onAddNewClick,
      allowCreation
    } = this.props;

    const { selectedOptions, selectedAll, currentGroup, groups } = this.state;

    /*const containerHeight =
      size === "compact" ? (!groups || !groups.length ? 336 : 326) : 614;
    const containerWidth =
      size === "compact" ? (!groups || !groups.length ? 325 : 326) : isDropDown ? 690 : 326;
    const listHeight =
      size === "compact"
        ? !groups || !groups.length
          ? isMultiSelect
            ? 176
            : 226
          : 120
        : 488;
    const listWidth = isDropDown ? 320 : "100%";*/

    let containerHeight;
    let containerWidth;
    let listHeight;
    let listWidth;
    const itemHeight = 32;
    const hasGroups = groups && groups.length > 0;

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
        listWidth = displayType === "dropdown" ? 320 : 302;
        listHeight = 488;
        break;
    }

    return (
      <StyledBodyContainer
        containerHeight={containerHeight}
        containerWidth={containerWidth}
        {...this.props}
      >
        <div ref={this.ref}>
          <div className="data_container">
            <div className="data_column_one">
              <div className="head_container">
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
              </div>
              {displayType === "aside" && groups && groups.length > 0 && (
                <ComboBox
                  className="options_group_selector"
                  isDisabled={isDisabled}
                  options={groups}
                  selectedOption={currentGroup}
                  dropDownMaxHeight={200}
                  scaled={true}
                  scaledOptions={true}
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
            {displayType === "dropdown" &&
              size === "full" &&
              groups &&
              groups.length > 0 && (
                <div className="data_column_two">
                  <Text
                    as="p"
                    className="group_header"
                    fontSize='15px'
                    isBold={true}
                  >
                    Groups
                  </Text>
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
      </StyledBodyContainer>
    );
  };

  render() {
    const { displayType, isOpen } = this.props;
    //console.log("AdvancedSelector render()");

    return (
      <StyledContainer>
        {displayType === "dropdown" ? (
          <DropDown open={isOpen} className="dropdown-container">
            {this.renderBody()}
          </DropDown>
        ) : (
          <Aside visible={isOpen} scale={false} className="aside-container">
            {this.renderBody()}
          </Aside>
        )}
      </StyledContainer>
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
  isOpen: PropTypes.bool,
  allowCreation: PropTypes.bool,
  onAddNewClick: PropTypes.func,
  allowAnyClickClose: PropTypes.bool,
  displayType: PropTypes.oneOf(displayTypes)
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "compact",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowAnyClickClose: true,
  displayType: "dropdown"
};

export default AdvancedSelector;
