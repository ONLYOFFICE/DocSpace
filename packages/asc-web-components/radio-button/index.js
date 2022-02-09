import React from "react";
import PropTypes from "prop-types";

import { RadioButtonIcon, RadioButtonIconChecked } from "./svg";
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
  /** Name of the radiobutton. If missed, `value` will be used  */
  label: PropTypes.oneOfType([PropTypes.any, PropTypes.string]),
  /** Font size of link  */
  fontSize: PropTypes.string,
  /** Font weight of link  */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Used as HTML `name` property for `<input>` tag. */
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  /** Allow you to handle clicking events on component */
  onClick: PropTypes.func,
  /** Used as HTML `value` property for `<input>` tag. Used for identification each radiobutton */
  value: PropTypes.string.isRequired,
  /** Margin between radiobutton. If orientation `horizontal`,
   * it is `margin-left`(apply for all radiobuttons, except first),
   * if orientation `vertical`, it is `margin-bottom`(apply for all radiobuttons, except last) */
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
};

export default RadioButton;
