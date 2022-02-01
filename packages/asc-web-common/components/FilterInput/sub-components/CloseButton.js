import React from "react";
import PropTypes from "prop-types";

import IconButton from "@appserver/components/icon-button";

const CloseButton = (props) => {
  //console.log("CloseButton render");
  const { className, isDisabled, onClick, isClickable } = props;
  return (
    <div className={`styled-close-button ${className}`}>
      <IconButton
        className="close-button"
        color={"#A3A9AE"}
        clickColor={"#A3A9AE"}
        size={10}
        iconName="/static/images/cross.react.svg"
        isFill={true}
        isDisabled={isDisabled}
        onClick={!isDisabled ? onClick : undefined}
        isClickable={isClickable}
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
