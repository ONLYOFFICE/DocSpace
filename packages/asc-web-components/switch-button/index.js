import React from "react";
import PropTypes from "prop-types";

import { StyledSwitchButton, StyledHiddenInput } from "./styled-switch-button";

import { SwitchButtonActiveUnchecked, SwitchButtonActiveChecked } from "./svg";

const SwitchButton = ({ checked, disabled, onChange, ...rest }) => {
  const renderSwitch = () => {
    return (
      <>
        {checked ? (
          <SwitchButtonActiveChecked />
        ) : (
          <SwitchButtonActiveUnchecked />
        )}
      </>
    );
  };

  const btnSwitch = renderSwitch(checked);

  //console.log('SwitchButton render');
  return (
    <StyledSwitchButton {...rest} checked={checked} disabled={disabled}>
      {btnSwitch}
      <StyledHiddenInput
        type="checkbox"
        defaultChecked={checked}
        disabled={disabled}
        onChange={onChange}
      />
    </StyledSwitchButton>
  );
};

SwitchButton.propTypes = {
  /** Disables the button default functionality  */
  disabled: PropTypes.bool,
  /** Makes SwitchButton checked. */
  checked: PropTypes.bool,
  /** The event triggered when the button is clicked */
  onChange: PropTypes.func,
};

SwitchButton.defaultProps = {
  disabled: false,
  checked: false,
};

export default SwitchButton;
