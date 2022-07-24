import React from "react";
import PropTypes from "prop-types";

import IconButton from "@docspace/components/icon-button";
import TextInput from "@docspace/components/text-input";
import StyledFileInput from "./StyledSimpleFileInput";

let iconSize;
const SimpleFileInput = ({
  size,
  placeholder,
  isDisabled,
  scale,
  isError,
  hasWarning,
  id,
  onClickInput,
  name,
  className,
  textField,
  ...rest
}) => {
  switch (size) {
    case "base":
      iconSize = 15;
      break;
    case "middle":
      iconSize = 15;
      break;
    case "big":
      iconSize = 16;
      break;
    case "huge":
      iconSize = 16;
      break;
    case "large":
      iconSize = 16;
      break;
  }

  return (
    <StyledFileInput
      size={size}
      scale={scale ? 1 : 0}
      hasError={isError}
      hasWarning={hasWarning}
      isDisabled={isDisabled}
      className={className}
      {...rest}
    >
      <TextInput
        id={id}
        className="file-text-input"
        placeholder={placeholder}
        value={textField}
        size={size}
        isDisabled={isDisabled}
        hasError={isError}
        hasWarning={hasWarning}
        scale={scale}
        onClick={onClickInput}
        isReadOnly
        name={name}
      />

      <div className="icon" onClick={!isDisabled ? onClickInput : null}>
        <IconButton
          className="icon-button"
          iconName={"/static/images/catalog.folder.react.svg"}
          // color={"#A3A9AE"}
          isDisabled={isDisabled}
          size={iconSize}
        />
      </div>
    </StyledFileInput>
  );
};
SimpleFileInput.propTypes = {
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** Supported size of the input fields */
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "large"]),
  /** Indicates the input field has scale */
  scale: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Indicates the input field has an error */
  hasError: PropTypes.bool,
  /** Indicates the input field has a warning */
  hasWarning: PropTypes.bool,
  /** Used as HTML `id` property */
  id: PropTypes.string,
  /** Indicates that the field cannot be used (e.g not authorised, or changes not saved) */
  isDisabled: PropTypes.bool,
  /** Used as HTML `name` property */
  name: PropTypes.string,
  fontSizeInput: PropTypes.string,
};

SimpleFileInput.defaultProps = {
  size: "base",
  scale: false,
  hasWarning: false,
  hasError: false,
  isDisabled: false,
  baseFolder: "",
};

export default SimpleFileInput;
