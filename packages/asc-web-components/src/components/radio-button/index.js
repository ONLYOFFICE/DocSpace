import React from "react";
import PropTypes from "prop-types";

import { RadioButtonIcon, RadioButtonIconChecked } from "./svg";
import Text from "../text";
import { Label, Input } from "./styled-radio-button";

// eslint-disable-next-line react/prop-types
const RadiobuttonIcon = ({ isChecked }) => {
  let newProps = {
    size: "medium",
    className: "radio-button",
  };

  return (
    <>
      {isChecked ? (
        <RadioButtonIconChecked {...newProps} />
      ) : (
        <RadioButtonIcon {...newProps} />
      )}
    </>
  );
};

class RadioButton extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: this.props.isChecked,
    };
  }

  componentDidUpdate(prevProps) {
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ isChecked: this.props.isChecked });
    }
  }

  render() {
    return (
      <Label
        orientation={this.props.orientation}
        spacing={this.props.spacing}
        isDisabled={this.props.isDisabled}
        id={this.props.id}
        className={this.props.className}
        style={this.props.style}
      >
        <Input
          type="radio"
          name={this.props.name}
          value={this.props.value}
          checked={this.props.isChecked}
          onChange={
            this.props.onChange
              ? this.props.onChange
              : (e) => {
                  this.setState({ isChecked: true });
                  this.props.onClick && this.props.onClick(e);
                }
          }
          disabled={this.props.isDisabled}
        />
        <RadiobuttonIcon {...this.props} />
        <Text
          as="span"
          className="radio-button_text"
          fontSize={this.props.fontSize}
          fontWeight={this.props.fontWeight}
        >
          {this.props.label || this.props.value}
        </Text>
      </Label>
    );
  }
}

RadioButton.propTypes = {
  isChecked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  label: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  onClick: PropTypes.func,
  value: PropTypes.string.isRequired,
  spacing: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  orientation: PropTypes.oneOf(["horizontal", "vertical"]),
};

RadioButton.defaultProps = {
  isChecked: false,
  isDisabled: false,
};

export default RadioButton;
