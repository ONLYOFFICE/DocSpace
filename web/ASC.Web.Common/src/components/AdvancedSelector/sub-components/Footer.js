import React from "react";
import PropTypes from "prop-types";
import { Button } from "asc-web-components";
import StyledFooter from "./StyledFooter";

const Footer = props => {
  const {
    selectButtonLabel,
    isDisabled,
    onClick,
    isVisible,
    className
  } = props;

  return (
    <StyledFooter isVisible={isVisible} className={className}>
      <Button
        className="add_members_btn"
        primary={true}
        size="big"
        label={selectButtonLabel}
        scale={true}
        isDisabled={isDisabled}
        onClick={onClick}
      />
    </StyledFooter>
  );
};

Footer.propTypes = {
  className: PropTypes.string,
  selectButtonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  onClick: PropTypes.func
};

export default Footer;
