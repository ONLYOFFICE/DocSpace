import React from "react";
import PropTypes from "prop-types";

import IconButton from "../icon-button";
import {
  StyledCloseButton,
  StyledSelectedTextBox,
  StyledSelectedItem,
  StyledText,
} from "./styled-selected-item";

const SelectedItem = (props) => {
  const { isDisabled, text, onClose } = props;

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
      <StyledCloseButton onClick={onCloseClick} isDisabled={isDisabled}>
        <IconButton
          size={10}
          iconName="/static/images/cross.react.svg"
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
  /** What the selected item will trigger when clicked */
  onClose: PropTypes.func.isRequired,
  /** Tells when the button should present a disabled state */
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
