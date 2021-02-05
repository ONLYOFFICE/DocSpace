import React from "react";
import PropTypes from "prop-types";

import StyledButton from "./styled-selector-add-button"
import IconButton from "../icon-button";


const SelectorAddButton = (props) => {
  const { isDisabled, title, className, id, style } = props;

  const onClick = (e) => {
    !isDisabled && props.onClick && props.onClick(e);
  };

  return (
    <StyledButton
      isDisabled={isDisabled}
      title={title}
      onClick={onClick}
      className={className}
      id={id}
      style={style}
    >
      <IconButton
        color="#979797"
        size={14}
        iconName="PlusIcon"
        isFill={true}
        isDisabled={isDisabled}
        isClickable={!isDisabled}
      />
    </StyledButton>
  );
};

SelectorAddButton.propTypes = {
  title: PropTypes.string,
  onClick: PropTypes.func,
  isDisabled: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SelectorAddButton.defaultProps = {
  isDisabled: false,
};

export default SelectorAddButton;
