import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import DropDown from "../drop-down";
import { Icons } from "../icons";
import { handleAnyClick } from "../../utils/event";
import Text from "../text";

const backgroundColor = "#ED7309",
  disableBackgroundColor = "#FFCCA6",
  hoverBackgroundColor = "#FF8932",
  clickBackgroundColor = "#C96C27";

const hoveredCss = css`
  background-color: ${hoverBackgroundColor};
  cursor: pointer;
`;
const clickCss = css`
  background-color: ${clickBackgroundColor};
  cursor: pointer;
`;

const arrowDropdown = css`
  border-left: 4px solid transparent;
  border-right: 4px solid transparent;
  border-top: 4px solid white;
  content: "";
  height: 0;
  margin-top: -1px;
  position: absolute;
  right: 10px;
  top: 50%;
  width: 0;
`;

const notDisableStyles = css`
  &:hover {
    ${hoveredCss}
  }

  &:active {
    ${clickCss}
  }
`;

const notDropdown = css`
  &:after {
    display: none;
  }

  border-top-right-radius: 0;
  border-bottom-right-radius: 0;
`;

const GroupMainButton = styled.div`
  position: relative;
  display: grid;
  grid-template-columns: ${props => (props.isDropdown ? "1fr" : "1fr 32px")};
  ${props => !props.isDropdown && "grid-column-gap: 1px"};
`;

const StyledDropDown = styled(DropDown)`
  width: 100%;
`;

const StyledMainButton = styled.div`
  position: relative;
  display: block;
  vertical-align: middle;
  box-sizing: border-box;
  background-color: ${props =>
    props.isDisabled ? disableBackgroundColor : backgroundColor};
  padding: 5px 10px;
  border-radius: 3px;
  -moz-border-radius: 3px;
  -webkit-border-radius: 3px;
  line-height: 22px;

  &:after {
    ${arrowDropdown}
  }

  ${props => !props.isDisabled && notDisableStyles}
  ${props => !props.isDropdown && notDropdown}

    & > svg {
    display: block;
    margin: auto;
    height: 100%;
  }
`;

const StyledSecondaryButton = styled(StyledMainButton)`
  display: inline-block;
  height: 32px;
  padding: 0;
  border-radius: 3px;
  -moz-border-radius: 3px;
  -webkit-border-radius: 3px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
`;

class MainButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.iconNames = Object.keys(Icons);

    this.state = {
      isOpen: props.opened
    };

    if(props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = e => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = e => e.preventDefault();
  toggle = isOpen => this.setState({ isOpen: isOpen });

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    // Store prevId in state so we can compare when props change.
    // Clear out previously-loaded data (so we don't render stale stuff).
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if(this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }
  }

  onMainButtonClick = (e) => {
    if (!this.props.isDisabled) {
      if (!this.props.isDropdown) {
        this.props.clickAction && this.props.clickAction();
      } else {
        this.toggle(!this.state.isOpen);
      }
    } else {
      this.stopAction(e);
    }
  };

  onDropDownClick = () => {
    this.props.onClick && this.props.onClick();
    this.toggle(!this.state.isOpen);
  };

  onSecondaryButtonClick = (e) => {
    if (!this.props.isDisabled) {
      this.props.clickActionSecondary && this.props.clickActionSecondary();
    } else {
      this.stopAction(e);
    }
  };

  render() {
    //console.log("MainButton render");
    return (
      <GroupMainButton {...this.props} ref={this.ref}>
        <StyledMainButton {...this.props} onClick={this.onMainButtonClick}>
          <Text fontSize='16px' fontWeight='bold' color="#fff">
            {this.props.text}
          </Text>
        </StyledMainButton>
        {this.props.isDropdown ? (
          <StyledDropDown
            open={this.state.isOpen}
            withBackdrop
            clickOutsideAction={this.handleClick}
            {...this.props}
            onClick={this.onDropDownClick}
          />
        ) : (
          <StyledSecondaryButton
            {...this.props}
            onClick={this.onSecondaryButtonClick}
          >
            {this.iconNames.includes(this.props.iconName) &&
              React.createElement(Icons[this.props.iconName], {
                size: "medium",
                color: "#ffffff"
              })}
          </StyledSecondaryButton>
        )}
      </GroupMainButton>
    );
  }
}

MainButton.propTypes = {
  text: PropTypes.string,
  isDisabled: PropTypes.bool,
  isDropdown: PropTypes.bool,
  clickAction: PropTypes.func,
  clickActionSecondary: PropTypes.func,
  iconName: PropTypes.string,
  opened: PropTypes.bool,  //TODO: Make us whole
  onClick: PropTypes.func,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

MainButton.defaultProps = {
  text: "Button",
  isDisabled: false,
  isDropdown: true,
  iconName: "PeopleIcon"
};

export default MainButton;
