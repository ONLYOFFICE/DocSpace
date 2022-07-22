import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import Checkbox from "../checkbox";
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

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    color: ${(props) =>
      props.disabled
        ? props.theme.groupButton.disableColor
        : props.theme.groupButton.color};
    fill: ${(props) =>
      props.disabled
        ? props.theme.groupButton.disableColor
        : props.theme.groupButton.color};
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
      alt,
    } = this.props;

    const itemLabel = !isSelect ? label : this.state.selected;
    const dropDownMaxHeightProp = dropDownMaxHeight
      ? { maxHeight: dropDownMaxHeight }
      : {};
    const offsetSelectDropDown = isSelect
      ? { manualX: window.innerWidth <= 1024 ? "44px" : "50px" }
      : {};

    const manualY = window.innerWidth <= 1024 ? "60px" : "53px";

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
                  title={itemLabel}
                />
              </StyledCheckbox>
            )}
            <StyledDropdownToggle
              {...this.props}
              onClick={this.dropDownToggleClick}
              title={itemLabel}
            >
              {itemLabel}
              <Caret isOpen={this.state.isOpen}>
                <StyledExpanderDownIcon size="scale" disabled={disabled} />
              </Caret>
            </StyledDropdownToggle>
            <DropDown
              forwardedRef={this.ref}
              {...dropDownMaxHeightProp}
              {...offsetSelectDropDown}
              manualY={manualY}
              open={this.state.isOpen}
              clickOutsideAction={this.clickOutsideAction}
              showDisabledItems={true}
              title={this.state.isOpen ? "" : itemLabel}
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
          <StyledDropdownToggle {...this.props} title={alt || itemLabel}>
            {label}
          </StyledDropdownToggle>
        )}
        {isSeparator && <Separator title="" />}
      </StyledGroupButton>
    );
  }
}

GroupButton.propTypes = {
  activated: PropTypes.bool,
  /** Initial value of checkbox */
  checked: PropTypes.bool,
  /** Children elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Tells when the button should present a disabled state */
  disabled: PropTypes.bool,
  /** Selected height value of DropDown */
  dropDownMaxHeight: PropTypes.number,
  /** Value of font weight */
  fontWeight: PropTypes.string,
  hovered: PropTypes.bool,
  /** Accepts id */
  id: PropTypes.string,
  /** Tells when the button should present a dropdown state */
  isDropdown: PropTypes.bool,
  /** Initial value of Indeterminate checkbox */
  isIndeterminate: PropTypes.bool,
  isSelect: PropTypes.bool,
  /** Tells when the button should contain separator */
  isSeparator: PropTypes.bool,
  /** Value of the group button */
  label: PropTypes.string,
  /** Called when checkbox is checked */
  onChange: PropTypes.func,
  /** Property for onClick action */
  onClick: PropTypes.func,
  /** Called when value is selected in selector */
  onSelect: PropTypes.func,
  /** Tells when the button should be opened by default */
  opened: PropTypes.bool,
  /** Selected value label */
  selected: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Value of tab index */
  tabIndex: PropTypes.number,
  /** Alternative value of the group button */
  alt: PropTypes.string,
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
