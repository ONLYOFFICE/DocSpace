import React from "react";
import PropTypes from "prop-types";

import StyledButton from "./styled-selector-add-button";
import IconButton from "../icon-button";

import ActionsHeaderTouchReactSvgUrl from "PUBLIC_DIR/images/actions.header.touch.react.svg?url";

const SelectorAddButton = (props) => {
  const { isDisabled, title, className, id, style, iconName } = props;

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
      {...props}
    >
      <IconButton
        size={12}
        iconName={iconName}
        isFill={true}
        isDisabled={isDisabled}
        isClickable={!isDisabled}
      />
    </StyledButton>
  );
};

SelectorAddButton.propTypes = {
  /** Title text */
  title: PropTypes.string,
  /** Sets a callback function that is triggered when the button is clicked */
  onClick: PropTypes.func,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Attribute className  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Specifies the icon name */
  iconName: PropTypes.string,
};

SelectorAddButton.defaultProps = {
  isDisabled: false,
  iconName: ActionsHeaderTouchReactSvgUrl,
};

export default SelectorAddButton;
