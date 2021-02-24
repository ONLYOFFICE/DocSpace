import React from "react";
import PropTypes from "prop-types";
import RadioButton from "../radio-button";
import StyledDiv from "./styled-radio-button-group";

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
    return (
      <StyledDiv
        id={this.props.id}
        className={this.props.className}
        style={this.props.style}
        orientation={this.props.orientation}
        width={this.props.width}
      >
        {options.map((option) => {
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
  isDisabled: PropTypes.bool,
  name: PropTypes.string.isRequired,
  onClick: PropTypes.func,
  options: PropTypes.arrayOf(
    PropTypes.shape({
      value: PropTypes.string.isRequired,
      label: PropTypes.string,
      disabled: PropTypes.bool,
    })
  ).isRequired,
  selected: PropTypes.string.isRequired,
  spacing: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  orientation: PropTypes.oneOf(["horizontal", "vertical"]),
  width: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
};

RadioButtonGroup.defaultProps = {
  isDisabled: false,
  selected: undefined,
  spacing: "15px",
  orientation: "horizontal",
  width: "100%",
};

export default RadioButtonGroup;
