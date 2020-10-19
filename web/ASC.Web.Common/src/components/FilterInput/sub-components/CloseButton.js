import React from "react";
import { IconButton } from "asc-web-components";
import PropTypes from "prop-types";

const CloseButton = (props) => {
  //console.log("CloseButton render");
  const { className, isDisabled, onClick } = props;
  return (
    <div className={`styled-close-button ${className}`}>
      <IconButton
        className="close-button"
        color={"#A3A9AE"}
        clickColor={"#A3A9AE"}
        size={10}
        iconName={"CrossIcon"}
        isFill={true}
        isDisabled={isDisabled}
        onClick={!isDisabled ? onClick : undefined}
      />
    </div>
  );
};
CloseButton.propTypes = {
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func,
  className: PropTypes.string,
};
export default CloseButton;
