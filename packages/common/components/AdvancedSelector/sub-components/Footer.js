import React from "react";
import PropTypes from "prop-types";
import Button from "@docspace/components/button";
import StyledFooter from "./StyledFooter";

const Footer = (props) => {
  const {
    selectButtonLabel,
    isDisabled,
    onClick,
    isVisible,
    className,
    embeddedComponent,
    selectedLength,
    showCounter,
  } = props;

  return (
    <StyledFooter
      withEmbeddedComponent={embeddedComponent}
      isVisible={isVisible}
      className={className}
    >
      <Button
        className="add_members_btn"
        primary={true}
        size="normal"
        label={`${selectButtonLabel} ${
          selectedLength && showCounter ? `(${selectedLength})` : ""
        }`}
        scale={true}
        isDisabled={isDisabled}
        onClick={onClick}
      />
      {embeddedComponent && embeddedComponent}
    </StyledFooter>
  );
};

Footer.propTypes = {
  className: PropTypes.string,
  selectButtonLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  onClick: PropTypes.func,
  comboBoxOptions: PropTypes.any,
  embeddedComponent: PropTypes.any,
};

export default Footer;
