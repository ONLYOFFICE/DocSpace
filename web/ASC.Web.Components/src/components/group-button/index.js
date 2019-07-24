import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";

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

  color: ${props => (props.disabled ? disabledTextColor : textColor)};

  float: left;
  height: 19px;
  margin: 18px 12px 19px 12px;
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

const Caret = styled(Icons.ExpanderDownIcon)`
  width: 10px;
  margin-left: 4px;
`;

const Separator = styled.div`
  vertical-align: middle;
  border: 0.5px solid #eceef1;
  width: 1px;
  height: 24px;
  margin-top: 16px;
`;

class GroupButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened
    };

    this.handleClick = this.handleClick.bind(this);
    this.stopAction = this.stopAction.bind(this);
    this.toggle = this.toggle.bind(this);
  }

  handleClick = e =>
    !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = e => e.preventDefault();
  toggle = isOpen => this.setState({ isOpen: isOpen });

  componentDidMount() {
    if (this.ref.current) {
      document.addEventListener("click", this.handleClick);
    }
  }

  componentWillUnmount() {
    document.removeEventListener("click", this.handleClick);
  }

  componentDidUpdate(prevProps) {
    // Store prevId in state so we can compare when props change.
    // Clear out previously-loaded data (so we don't render stale stuff).
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }
  }

  render() {
    //console.log("GroupButton render");
    const { label, isDropdown, disabled, isSeparator } = this.props;

    return (
      <StyledGroupButton ref={this.ref}>
        {isDropdown ? (
          <>
            <StyledDropdownToggle
              {...this.props}
              onClick={() => {
                disabled ? false : this.toggle(!this.state.isOpen);
              }}
            >
              {label}
              <Caret
                size="small"
                color={disabled ? disabledTextColor : textColor}
              />
            </StyledDropdownToggle>
            <DropDown isOpen={this.state.isOpen}>
            {
                React.Children.map(this.props.children, (child) => 
                <DropDownItem 
                    {...child.props}
                    onClick={() => { 
                        child.props.onClick && child.props.onClick();
                    this.toggle(!this.state.isOpen);
                    }}
                />
                )
            }
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
  label: PropTypes.string,
  disabled: PropTypes.bool,
  activated: PropTypes.bool,
  opened: PropTypes.bool,
  hovered: PropTypes.bool,
  isDropdown: PropTypes.bool,
  isSeparator: PropTypes.bool,
  tabIndex: PropTypes.number,
  onClick: PropTypes.func,
  fontWeight: PropTypes.string
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
  fontWeight: "600"
};

export default GroupButton;
