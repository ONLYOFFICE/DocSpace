import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import Checkbox from "../checkbox";
import { tablet } from "../../utils/device";
import { isArrayEqual } from "../../utils/array";

const textColor = "#333333",
  disabledTextColor = "#A3A9AE";

const activatedCss = css`
  cursor: pointer;
`;

const hoveredCss = css`
  cursor: pointer;
`;

const StyledGroupButton = styled.div`
  position: relative;
  display: inline-flex;
  vertical-align: middle;
`;

const StyledDropdownToggle = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: ${(props) => props.fontWeight};
  font-size: 14px;
  line-height: 19px;

  cursor: default;
  outline: 0;

  color: ${(props) => (props.disabled ? disabledTextColor : textColor)};

  float: left;
  height: 19px;
  margin: 18px 12px 19px ${(props) => (props.isSelect ? "0px" : "13px")};
  overflow: hidden;
  padding: 0px;

  text-align: center;
  text-decoration: none;
  white-space: nowrap;

  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;

  ${(props) =>
    !props.disabled &&
    (props.activated
      ? `${activatedCss}`
      : css`
          &:active {
            ${activatedCss}
          }
        `)}

  ${(props) =>
    !props.disabled &&
    (props.hovered
      ? `${hoveredCss}`
      : css`
          &:hover {
            ${hoveredCss}
          }
        `)}
`;

const Caret = styled.div`
  display: inline-block;
  width: 8px;
  margin-left: 6px;

  ${(props) =>
    props.isOpen &&
    `
    padding-bottom: 2px;
    transform: scale(1, -1);
  `}
`;

const Separator = styled.div`
  vertical-align: middle;
  border: 1px solid #eceef1;
  width: 0px;
  height: 24px;
  margin: 16px 12px 0 12px;
`;

const StyledCheckbox = styled.div`
  display: inline-block;
  margin: auto 0 auto 24px;

  @media ${tablet} {
    margin: auto 0 auto 16px;
  }

  & > * {
    margin: 0px;
  }
`;

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
    this.state.isOpen &&
      !this.ref.current.contains(e.target) &&
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

    const color = disabled ? disabledTextColor : textColor;
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
                <Icons.ExpanderDownIcon size="scale" color={color} />
              </Caret>
            </StyledDropdownToggle>
            <DropDown
              {...dropDownMaxHeightProp}
              {...offsetSelectDropDown}
              manualY="72px"
              open={this.state.isOpen}
              clickOutsideAction={this.clickOutsideAction}
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
