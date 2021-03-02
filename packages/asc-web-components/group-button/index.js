import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import Checkbox from "../checkbox";
import { isArrayEqual } from "../utils/array";
import {
  StyledCheckbox,
  Separator,
  Caret,
  StyledDropdownToggle,
  StyledGroupButton,
} from "./styled-group-button";
import ExpanderDownIcon from "../../../public/images/expander-down.react.svg";
import commonIconsStyles from "../utils/common-icons-style";
import Base from "../themes/base";

const textColor = "#333333",
  disabledTextColor = "#A3A9AE";

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    color: ${(props) =>
      props.disabled
        ? props.theme.groupButton.disableColor
        : props.theme.groupButton.color};
    fill: ${(props) => props.color};
  }
`;
StyledExpanderDownIcon.defaultProps = { theme: Base };
class GroupButton extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      selected: props.selected,
    };
  }

  setIsOpen = (isOpen) => this.setState({ isOpen: isOpen });

  setSelected = (selected) => this.setState({ selected: selected });

  componentDidUpdate(prevProps) {
    if (this.props.opened !== prevProps.opened) {
      this.setIsOpen(this.props.opened);
    }

    if (this.props.selected !== prevProps.selected) {
      this.setSelected(this.props.selected);
    }

    if (!isArrayEqual(this.props.children, prevProps.children)) {
      this.setSelected(this.props.selected);
    }
  }

  clickOutsideAction = (e) => {
    this.state.isOpen && !this.ref.current.contains(e.target);
    this.setIsOpen(false);
  };

  checkboxChange = (e) => {
    this.props.onChange && this.props.onChange(e.target && e.target.checked);

    this.setSelected(this.props.selected);
  };

  dropDownItemClick = (e) => {
    const index = e.currentTarget.dataset.index;
    const child = this.props.children[index];

    child && child.props.onClick && child.props.onClick(e);
    this.props.onSelect && this.props.onSelect(child);

    child && this.setSelected(child.props.label);
    this.setIsOpen(!this.state.isOpen);
  };

  dropDownToggleClick = () => {
    this.setIsOpen(!this.state.isOpen);
  };

  render() {
    //console.log("GroupButton render");
    const {
      checked,
      children,
      className,
      disabled,
      dropDownMaxHeight,
      id,
      isDropdown,
      isIndeterminate,
      isSelect,
      isSeparator,
      label,
      style,
    } = this.props;

    const itemLabel = !isSelect ? label : this.state.selected;
    const dropDownMaxHeightProp = dropDownMaxHeight
      ? { maxHeight: dropDownMaxHeight }
      : {};
    const offsetSelectDropDown = isSelect
      ? { manualX: window.innerWidth <= 1024 ? "16px" : "24px" }
      : {};

    return (
      <StyledGroupButton
        ref={this.ref}
        id={id}
        className={className}
        style={style}
      >
        {isDropdown ? (
          <>
            {isSelect && (
              <StyledCheckbox>
                <Checkbox
                  isChecked={checked}
                  isIndeterminate={isIndeterminate}
                  onChange={this.checkboxChange}
                />
              </StyledCheckbox>
            )}
            <StyledDropdownToggle
              {...this.props}
              onClick={this.dropDownToggleClick}
            >
              {itemLabel}
              <Caret isOpen={this.state.isOpen}>
                <StyledExpanderDownIcon size="scale" disabled={disabled} />
              </Caret>
            </StyledDropdownToggle>
            <DropDown
              {...dropDownMaxHeightProp}
              {...offsetSelectDropDown}
              manualY="72px"
              open={this.state.isOpen}
              clickOutsideAction={this.clickOutsideAction}
              showDisabledItems={true}
            >
              {React.Children.map(children, (child) => (
                <DropDownItem
                  {...child.props}
                  onClick={this.dropDownItemClick}
                />
              ))}
            </DropDown>
          </>
        ) : (
          <StyledDropdownToggle {...this.props}>{label}</StyledDropdownToggle>
        )}
        {isSeparator && <Separator />}
      </StyledGroupButton>
    );
  }
}

GroupButton.propTypes = {
  activated: PropTypes.bool,
  checked: PropTypes.bool,
  children: PropTypes.any,
  className: PropTypes.string,
  disabled: PropTypes.bool,
  dropDownMaxHeight: PropTypes.number,
  fontWeight: PropTypes.string,
  hovered: PropTypes.bool,
  id: PropTypes.string,
  isDropdown: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isSelect: PropTypes.bool,
  isSeparator: PropTypes.bool,
  label: PropTypes.string,
  onChange: PropTypes.func,
  onClick: PropTypes.func,
  onSelect: PropTypes.func,
  opened: PropTypes.bool,
  selected: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  tabIndex: PropTypes.number,
};

GroupButton.defaultProps = {
  activated: false,
  disabled: false,
  fontWeight: "600",
  hovered: false,
  isDropdown: false,
  isSelect: false,
  isSeparator: false,
  label: "Group button",
  opened: false,
  tabIndex: -1,
};

export default GroupButton;
