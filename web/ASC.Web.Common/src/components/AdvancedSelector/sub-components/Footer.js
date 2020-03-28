import React from "react";
import PropTypes from "prop-types";
import { Button, ComboBox, Icons } from "asc-web-components";
import StyledFooter from "./StyledFooter";

const Footer = props => {
  const {
    selectButtonLabel,
    isDisabled,
    onClick,
    isVisible,
    className,
    comboBoxOptions
  } = props;

  return (
    <StyledFooter withComboBox={comboBoxOptions} isVisible={isVisible} className={className}>
      <Button
        className="add_members_btn"
        primary={true}
        size="big"
        label={selectButtonLabel}
        scale={true}
        isDisabled={isDisabled}
        onClick={onClick}
      />
      {comboBoxOptions && (
        <ComboBox
          advancedOptions={comboBoxOptions}
          options={[]}
          selectedOption={{ key: 0 }}
          size="content"
          className="ad-selector_combo-box"
          scaled={false}
          directionX="right"
          //isDisabled={isDisabled}
        >
          {React.createElement(Icons["EyeIcon"], {
            size: "medium"
            //color: this.state.currentIconColor,
            //isfill: isFill
          })}
        </ComboBox>
      )}
    </StyledFooter>
  );
};

Footer.propTypes = {
  className: PropTypes.string,
  selectButtonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  onClick: PropTypes.func,
  comboBoxOptions: PropTypes.any
};

export default Footer;
