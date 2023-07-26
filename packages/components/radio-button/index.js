import React from "react";
import PropTypes from "prop-types";

import { RadioButtonReactSvg, RadioButtonCheckedReactSvg } from "./svg";
import Text from "../text";
import { Label, Input } from "./styled-radio-button";

// eslint-disable-next-line react/prop-types
const RadiobuttonIcon = ({ isChecked, theme }) => {
  let newProps = {
    size: "medium",
    className: "radio-button",
    theme: theme,
  };

  return (
    <>
      {isChecked ? (
        <RadioButtonCheckedReactSvg {...newProps} />
      ) : (
        <RadioButtonReactSvg {...newProps} />
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
    const setClassNameInput = this.props.classNameInput
      ? {
          className: this.props.classNameInput,
        }
      : {};

    return (
      <Label
        theme={this.props.theme}
        orientation={this.props.orientation}
        spacing={this.props.spacing}
        isDisabled={this.props.isDisabled}
        id={this.props.id}
        className={this.props.className}
        style={this.props.style}
      >
        <Input
          theme={this.props.theme}
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
          {...setClassNameInput}
        />
        <RadiobuttonIcon {...this.props} />
        <Text
          theme={this.props.theme}
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
  /** Used as HTML `checked` property for each `<input>` tag */
  isChecked: PropTypes.bool,
  /** Used as HTML `disabled` property for each `<input>` tag */
  isDisabled: PropTypes.bool,
  /** Radiobutton name. In case the name is not stated, `value` is used */
  label: PropTypes.oneOfType([PropTypes.any, PropTypes.string]),
  /** Link font size */
  fontSize: PropTypes.string,
  /** Link font weight */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Used as HTML `name` property for `<input>` tag. */
  name: PropTypes.string.isRequired,
  /** Allows handling the changing events of the component  */
  onChange: PropTypes.func,
  /** Allows handling component clicking events */
  onClick: PropTypes.func,
  /** Used as HTML `value` property for `<input>` tag. Facilitates identification of each radiobutton  */
  value: PropTypes.string.isRequired,
  /** Sets margin between radiobuttons. In case the orientation is `horizontal`,
   * `margin-left` is applied for all radiobuttons, except the first one.
   * In case the orientation is `vertical`, `margin-bottom` is applied for all radiobuttons, except the last one */
  spacing: PropTypes.string,
  /** Accepts class  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Position of radiobuttons */
  orientation: PropTypes.oneOf(["horizontal", "vertical"]),
};

RadioButton.defaultProps = {
  isChecked: false,
  isDisabled: false,
  label: "",
};

export default RadioButton;
