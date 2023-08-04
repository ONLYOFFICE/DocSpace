import React from "react";
import PropTypes from "prop-types";
import RadioButton from "../radio-button";
import StyledDiv from "./styled-radio-button-group";
import Text from "../text";

class RadioButtonGroup extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      selectedOption: this.props.selected,
    };
  }

  handleOptionChange = (changeEvent) => {
    this.setState({
      selectedOption: changeEvent.target.value,
    });
  };

  componentDidUpdate(prevProps) {
    if (this.props.selected !== prevProps.selected) {
      this.setState({ selectedOption: this.props.selected });
    }
  }

  render() {
    const options = this.props.options;
    const theme = this.props.theme;
    return (
      <StyledDiv
        id={this.props.id}
        className={this.props.className}
        style={this.props.style}
        orientation={this.props.orientation}
        width={this.props.width}
      >
        {options.map((option) => {
          if (option.type === "text")
            return (
              <Text key="radio-text" className="subtext">
                {option.label}
              </Text>
            );
          return (
            <RadioButton
              id={option.id}
              key={option.value}
              name={this.props.name}
              value={option.value}
              isChecked={this.state.selectedOption === option.value}
              onChange={(e) => {
                this.handleOptionChange(e);
                this.props.onClick && this.props.onClick(e);
              }}
              isDisabled={this.props.isDisabled || option.disabled}
              label={option.label}
              fontSize={this.props.fontSize}
              fontWeight={this.props.fontWeight}
              spacing={this.props.spacing}
              orientation={this.props.orientation}
            />
          );
        })}
      </StyledDiv>
    );
  }
}

RadioButtonGroup.propTypes = {
  /** Disables all radiobuttons in the group */
  isDisabled: PropTypes.bool,
  /** Used as HTML `value` property for `<input>` tag. Facilitates identification of each RadioButtonGroup */
  name: PropTypes.string.isRequired,
  /** Allows handling clicking events on `<RadioButton />` component */
  onClick: PropTypes.func,
  /** Array of objects, contains props for each `<RadioButton />` component */
  options: PropTypes.arrayOf(
    PropTypes.shape({
      value: PropTypes.string.isRequired,
      label: PropTypes.oneOfType([PropTypes.any, PropTypes.string]),
      disabled: PropTypes.bool,
    })
  ).isRequired,
  /** Value of the selected radiobutton */
  selected: PropTypes.string.isRequired,
  /** Sets margin between radiobuttons. In case the orientation is `horizontal`, `margin-left` is applied for all radiobuttons,
   * except the first one. If the orientation is `vertical`, `margin-bottom` is applied for all radiobuttons, except the last one */
  spacing: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Position of radiobuttons  */
  orientation: PropTypes.oneOf(["horizontal", "vertical"]),
  /** Width of RadioButtonGroup container */
  width: PropTypes.string,
  /** Link font size */
  fontSize: PropTypes.string,
  /** Link font weight  */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
};

RadioButtonGroup.defaultProps = {
  isDisabled: false,
  selected: undefined,
  spacing: "15px",
  orientation: "horizontal",
};

export default RadioButtonGroup;
