import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import { handleAnyClick } from "../../utils/event";

const QuestionIconStyle = styled.div`
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -khtml-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;

  svg {
    min-width: 3px;
    min-height: 3px;

    background-color: ${props => props.backgroundColor};
    height: ${props => props.size};
    width: ${props => props.size};
    max-width: ${props => props.size};
    :hover {
      cursor: pointer;
    }
  }
`;

const DropDownStyle = styled.div`
  position: relative;
`;

class QuestionIcon extends Component {
  constructor(props) {
    super(props);

    if (props.isOpen) {
      handleAnyClick(true, this.handleClick);
    }

    this.state = {
      isOpen: props.isOpen
    };
  }

  onClick = isOpen => {
    this.setState({ isOpen: !isOpen });
    this.props.onClick("onClick") && this.props.onClick();
  };

  handleClick = () => {
    this.onClick(this.state.isOpen);
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.props.isOpen !== prevProps.isOpen) {
      this.setState({ isOpen: this.props.isOpen });
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }
  }

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  render() {
    const {
      dropDownBody,
      dropDownManualWidth,
      color,
      backgroundColor,
      iconSize,
      dropDownDirectionY,
      dropDownDirectionX,
      dropDownManualY,
      dropDownManualX
    } = this.props;
    const { isOpen } = this.state;

    return (
      <QuestionIconStyle
        className="question-icon"
        color={color}
        backgroundColor={backgroundColor}
        size={`${iconSize}px`}
      >
        <Icons.QuestionIcon onClick={this.onClick.bind(this, isOpen)} />
        <DropDownStyle>
          <DropDown
            directionY={dropDownDirectionY}
            directionX={dropDownDirectionX}
            manualWidth={`${dropDownManualWidth}px`}
            manualY={`${dropDownManualY}px`}
            manualX={`${dropDownManualX}px`}
            className="dropdown"
            opened={isOpen}
          >
            {dropDownBody}
          </DropDown>
        </DropDownStyle>
      </QuestionIconStyle>
    );
  }
}

QuestionIcon.propTypes = {
  dropDownBody: PropTypes.object,
  isOpen: PropTypes.bool,
  onClick: PropTypes.func,
  dropDownManualWidth: PropTypes.number,
  iconSize: PropTypes.number,
  backgroundColor: PropTypes.string,
  color: PropTypes.string,
  dropDownDirectionY: PropTypes.oneOf(["bottom", "top"]),
  dropDownDirectionX: PropTypes.oneOf(["left", "right"]),
  dropDownManualY: PropTypes.number,
  dropDownManualX: PropTypes.number
};

QuestionIcon.defaultProps = {
  backgroundColor: "#fff",
  color: "#A3A9AE",
  iconSize: 12,
  dropDownDirectionX: "left",
  dropDownDirectionY: "top"
};

export default QuestionIcon;
