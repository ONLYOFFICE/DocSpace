import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import Checkbox from "../checkbox";
import { handleAnyClick } from '../../utils/event';
import { tablet } from '../../utils/device'
 
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
  font-weight: ${props => props.fontWeight};
  font-size: 14px;
  line-height: 19px;

  cursor: default;
  outline: 0;

  color: ${props => (props.disabled ? disabledTextColor : textColor)};

  float: left;
  height: 19px;
  margin: 18px 12px 19px ${props => props.isSelect ? '0px' : '12px'};
  overflow: hidden;
  padding: 0px;

  text-align: center;
  text-decoration: none;
  white-space: nowrap;

  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;

  ${props =>
    !props.disabled &&
    (props.activated
      ? `${activatedCss}`
      : css`
          &:active {
            ${activatedCss}
          }
        `)}

  ${props =>
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

  ${props => props.isOpen && `
    padding-bottom: 2px;
    transform: scale(1, -1);
  `}
`;

const Separator = styled.div`
  vertical-align: middle;
  border: 0.5px solid #ECEEF1;
  width: 0px;
  height: 24px;
  margin: 16px 11px 0 11px;
`;

const StyledCheckbox = styled.div`
    display: inline-block;
    margin: auto 0 auto 24px;

    @media ${ tablet } {
      margin: auto 0 auto 16px;
    }

    & > * {
        margin: 0px;
    }
`;

class GroupButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      selected: props.selected
    };

    if (props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) =>
    this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  checkboxChange = (e) => {
    this.props.onChange && this.props.onChange(e.target.checked);
    this.setState({ selected: this.props.selected });
  };

  dropDownItemClick = (child) => {
    child.props.onClick && child.props.onClick();
    this.props.onSelect && this.props.onSelect(child);
    this.setState({ selected: child.props.label });
    this.toggle(!this.state.isOpen);
  };

  dropDownToggleClick = (e) => {
    this.props.disabled ? this.stopAction(e) : this.toggle(!this.state.isOpen);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }
  }

  render() {
    //console.log("GroupButton render");
    const { label, isDropdown, disabled, isSeparator, isSelect, isIndeterminate, children, checked, dropDownMaxHeight, id, className, style } = this.props;

    const color = disabled ? disabledTextColor : textColor;
    const itemLabel = !isSelect ? label : this.state.selected;
    const dropDownMaxHeightProp = dropDownMaxHeight ? { maxHeight: dropDownMaxHeight } : {};
    const offsetSelectDropDown = isSelect ? { manualX : (window.innerWidth <= 1024) ? '16px' : '24px'} : {};

    return (
      <StyledGroupButton ref={this.ref} id={id} className={className} style={style}>
        {isDropdown
          ? <>
            {isSelect &&
              <StyledCheckbox>
                <Checkbox
                  isChecked={checked}
                  isIndeterminate={isIndeterminate}
                  onChange={this.checkboxChange} />
              </StyledCheckbox>
            }
            <StyledDropdownToggle
              {...this.props}
              onClick={this.dropDownToggleClick}
            >
              {itemLabel}
              <Caret
                isOpen={this.state.isOpen}
              >
                <Icons.ExpanderDownIcon
                  size="scale"
                  color={color}
                />
              </Caret>
            </StyledDropdownToggle>
            <DropDown
              {...dropDownMaxHeightProp}
              {...offsetSelectDropDown}
              manualY='72px'
              open={this.state.isOpen}
              clickOutsideAction={this.dropDownToggleClick}
              withBackdrop={true}
            >
              {React.Children.map(children, (child) =>
                <DropDownItem
                  {...child.props}
                  onClick={this.dropDownItemClick.bind(this, child)}
                />
              )}
            </DropDown>
          </>
          : <StyledDropdownToggle
            {...this.props}>
            {label}
          </StyledDropdownToggle>
        }
        {isSeparator && <Separator />}
      </StyledGroupButton>
    );
  }
}

GroupButton.propTypes = {
  label: PropTypes.string,
  disabled: PropTypes.bool,
  activated: PropTypes.bool,
  opened: PropTypes.bool,
  hovered: PropTypes.bool,
  isDropdown: PropTypes.bool,
  isSeparator: PropTypes.bool,
  tabIndex: PropTypes.number,
  onClick: PropTypes.func,
  fontWeight: PropTypes.string,
  onSelect: PropTypes.func,
  isSelect: PropTypes.bool,
  selected: PropTypes.string,
  onChange: PropTypes.func,
  isIndeterminate: PropTypes.bool,
  children: PropTypes.any,
  checked: PropTypes.bool,
  dropDownMaxHeight: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

GroupButton.defaultProps = {
  label: "Group button",
  disabled: false,
  activated: false,
  opened: false,
  hovered: false,
  isDropdown: false,
  isSeparator: false,
  tabIndex: -1,
  fontWeight: "600",
  isSelect: false
};

export default GroupButton;
