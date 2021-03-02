import React, { Component } from "react";
import PropTypes from "prop-types";
import { ToggleButtonContainer, HiddenInput } from "./styled-toggle-button";
import { ToggleButtonIcon, ToggleButtonCheckedIcon } from "./svg";
import Text from "../text";
import globalColors from "../utils/globalColors";

const ToggleIcon = ({ isChecked }) => {
  return (
    <>
      {isChecked ? (
        <ToggleButtonCheckedIcon className="toggle-button" />
      ) : (
        <ToggleButtonIcon className="toggle-button" />
      )}
    </>
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
    const { gray } = globalColors;
    const colorProps = isDisabled ? { color: gray } : {};

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
          <Text className="toggle-button-text" as="span" {...colorProps}>
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
