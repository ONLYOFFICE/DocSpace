import React, { Component } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Icons } from "../icons";
import Text from "../text";

const ToggleButtonContainer = styled.label`
  position: absolute;
  -webkit-appearance: none;
  align-items: start;
  outline: none;

  user-select: none;
  -moz-user-select: none;
  -o-user-select: none;
  -webkit-user-select: none;

  display: grid;
  grid-template-columns: min-content auto;
  grid-gap: 8px;

  ${(props) =>
    props.isDisabled
      ? css`
          cursor: default;
        `
      : css`
          cursor: pointer;
        `}
  svg {
    ${(props) =>
      props.isDisabled
        ? css`
            rect {
              fill: #eceff1;
            }
          `
        : ""}
  }
  .toggle-button {
    min-width: 28px;
  }
  .toggleText {
    margin-top: 2px;
  }
`;

const HiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
  z-index: -1;
`;

const ToggleIcon = ({ isChecked }) => {
  const iconName = isChecked ? "ToggleButtonCheckedIcon" : "ToggleButtonIcon";
  return (
    <>{React.createElement(Icons[iconName], { className: "toggle-button" })}</>
  );
};

class ToggleButton extends Component {
  constructor(props) {
    super(props);
    this.state = {
      checked: props.isChecked,
    };
  }

  componentDidUpdate(prevProps) {
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ checked: this.props.isChecked });
    }
  }

  render() {
    const { isDisabled, label, onChange, id, className, style } = this.props;
    const colorProps = isDisabled ? { color: "#A3A9AE" } : {};

    //console.log("ToggleButton render");

    return (
      <ToggleButtonContainer
        id={id}
        className={className}
        style={style}
        isDisabled={isDisabled}
      >
        <HiddenInput
          type="checkbox"
          checked={this.state.checked}
          disabled={isDisabled}
          onChange={onChange}
        />
        <ToggleIcon isChecked={this.state.checked} />
        {label && (
          <Text className="toggleText" as="span" {...colorProps}>
            {label}
          </Text>
        )}
      </ToggleButtonContainer>
    );
  }
}

ToggleButton.propTypes = {
  isChecked: PropTypes.bool.isRequired,
  isDisabled: PropTypes.bool,
  onChange: PropTypes.func,
  label: PropTypes.string,
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

ToggleIcon.propTypes = {
  isChecked: PropTypes.bool,
};

export default ToggleButton;
