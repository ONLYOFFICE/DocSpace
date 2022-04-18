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
  /** Disabling all radiobutton in group */
  isDisabled: PropTypes.bool,
  /** Used as HTML `name` property for `<input>` tag. Used for identification RadioButtonGroup */
  name: PropTypes.string.isRequired,
  /** Allow you to handle clicking events on `<RadioButton />` component */
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
  /** Margin between radiobutton. If orientation `horizontal`, it is `margin-left`(apply for all radiobuttons,
   * except first), if orientation `vertical`, it is `margin-bottom`(apply for all radiobuttons, except last) */
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
  /** Font size of link */
  fontSize: PropTypes.string,
  /** Font weight of link  */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
};

RadioButtonGroup.defaultProps = {
  isDisabled: false,
  selected: undefined,
  spacing: "15px",
  orientation: "horizontal",
};

export default RadioButtonGroup;
