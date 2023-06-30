import React from "react";
import PropTypes from "prop-types";
import CrossIconReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import IconButton from "../icon-button";
import {
  StyledCloseButton,
  StyledSelectedTextBox,
  StyledSelectedItem,
  StyledText,
} from "./styled-selected-item";

const SelectedItem = (props) => {
  const { isDisabled, text, onClose, classNameCloseButton } = props;

  const onCloseClick = (e) => {
    !isDisabled && onClose && onClose(e);
  };

  //console.log("SelectedItem render");
  return (
    <StyledSelectedItem {...props}>
      <StyledSelectedTextBox>
        <StyledText as="span" truncate isDisabled={isDisabled} fontWeight={600}>
          {text}
        </StyledText>
      </StyledSelectedTextBox>
      <StyledCloseButton
        className={classNameCloseButton}
        onClick={onCloseClick}
        isDisabled={isDisabled}
      >
        <IconButton
          size={10}
          iconName={CrossIconReactSvgUrl}
          isFill={true}
          isDisabled={isDisabled}
        />
      </StyledCloseButton>
    </StyledSelectedItem>
  );
};

SelectedItem.propTypes = {
  /** Selected item text */
  text: PropTypes.string,
  /** Sets the 'display: inline-block' property */
  isInline: PropTypes.bool,
  /** Sets a callback function that is triggered when the selected item is clicked */
  onClose: PropTypes.func.isRequired,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Accepts class  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SelectedItem.defaultProps = {
  isInline: true,
  isDisabled: false,
};

export default SelectedItem;
