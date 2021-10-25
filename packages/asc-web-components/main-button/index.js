import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import { handleAnyClick } from "../utils/event";
import Text from "../text";
import {
  StyledSecondaryButton,
  StyledMainButton,
  StyledDropDown,
  GroupMainButton,
} from "./styled-main-button";

class MainButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
    };

    if (props.opened) handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) => {
    if (this.state.isOpen && this.ref.current.contains(e.target)) return;
    this.toggle(false);
  };

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    // Store prevId in state so we can compare when props change.
    // Clear out previously-loaded data (so we don't render stale stuff).
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if (this.state.isOpen !== prevState.isOpen) {
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
          <Text className="main-button_text">{this.props.text}</Text>
        </StyledMainButton>
        {this.props.isDropdown ? (
          <StyledDropDown
            open={this.state.isOpen}
            clickOutsideAction={this.handleClick}
            {...this.props}
            onClick={this.onDropDownClick}
          />
        ) : (
          <StyledSecondaryButton
            {...this.props}
            onClick={this.onSecondaryButtonClick}
          >
            {this.props.iconName && <ReactSVG src={this.props.iconName} />}
          </StyledSecondaryButton>
        )}
      </GroupMainButton>
    );
  }
}

MainButton.propTypes = {
  /** Button text */
  text: PropTypes.string,
  /** Tells when the button should present a disabled state */
  isDisabled: PropTypes.bool,
  /** Select a state between two separate buttons or one with a drop-down list */
  isDropdown: PropTypes.bool,
  /** What the main button will trigger when clicked  */
  clickAction: PropTypes.func,
  /** What the secondary button will trigger when clicked  */
  clickActionSecondary: PropTypes.func,
  /** Icon inside button */
  iconName: PropTypes.string,
  /** Open DropDown */
  opened: PropTypes.bool, //TODO: Make us whole
  /** DropDown component click action */
  onClick: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

MainButton.defaultProps = {
  text: "Button",
  isDisabled: false,
  isDropdown: true,
  iconName: "/static/images/people.react.svg",
};

export default MainButton;
