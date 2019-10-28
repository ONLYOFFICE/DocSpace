import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import Checkbox from "../../checkbox";

import ComboBox from "../../combobox";
import { Text } from "../../text";

import filter from "lodash/filter";
import isEqual from "lodash/isEqual";

import { handleAnyClick } from "../../../utils/event";
import isEmpty from "lodash/isEmpty";

import ADSelectorFooter from "./footer";

import ADSelectorMainHeader from "./sections/main/header";
import ADSelectorMainBody from "./sections/main/body";
import ADSelectorAdditionalBody from "./sections/additional/body";

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
    hasNextPage,
    isNextPageLoading,
    loadNextPage,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

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

class ADSelectorBody extends React.Component {
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

    render() {

        const {
            options,
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
            allowCreation
        } = this.props;

        const { selectedOptions, selectedAll, currentGroup, groups } = this.state;

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

        // If there are more items to be loaded then add an extra row to hold a loading indicator.
        //const itemCount = hasNextPage ? options.length + 1 : options.length;

        return (
            <StyledBodyContainer
                containerHeight={containerHeight}
                containerWidth={containerWidth}
                {...this.props}
            >
                <div ref={this.ref}>
                    <div className="data_container">
                        <div className="data_column_one">
                            <ADSelectorMainHeader
                                value={value}
                                searchPlaceHolder={placeholder}
                                isDisabled={isDisabled}
                                allowCreation={allowCreation}
                                onAddNewClick={onAddNewClick}
                                onChange={onSearchChanged}
                                onClearSearch={onSearchChanged.bind(this, "")}
                            />
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
                            <ADSelectorMainBody
                                options={options}
                                hasNextPage={hasNextPage}
                                isNextPageLoading={isNextPageLoading}
                                loadNextPage={loadNextPage}

                                isMultiSelect={isMultiSelect}
                                listHeight={listHeight}
                                listWidth={listWidth}
                                itemHeight={itemHeight}
                                onRowChecked={this.onChange}
                                onRowSelect={this.onSelect}
                            />
                        </div>
                        {/* {displayType === "dropdown" &&
                            size === "full" &&
                            groups &&
                            groups.length > 0 && (
                                <ADSelectorAdditionalBody
                                    height={listHeight}
                                    itemSize={itemHeight}
                                    groups={groups}
                                />
                            )} */}
                    </div>
                    {isMultiSelect && (
                        <ADSelectorFooter
                            buttonLabel={buttonLabel}
                            isDisabled={
                                !this.state.selectedOptions ||
                                !this.state.selectedOptions.length
                            }
                            onClick={this.onButtonClick}
                        />
                    )}
                </div>
            </StyledBodyContainer>
        );
    }
}

export default ADSelectorBody;