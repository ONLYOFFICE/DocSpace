import React from "react";
import PropTypes from "prop-types";

import IconButton from "@appserver/components/icon-button";
import TextInput from "@appserver/components/text-input";
import StyledFileInput from "@appserver/components/file-input/styled-file-input";

const FileInputWithFolderPath = (
  {
    folderPath,
    size,
    placeholder,
    isDisabled,
    scale,
    hasError,
    hasWarning,
    id,
    baseFolder,
    onClickInput,
    name,
    className,
  },
  ...rest
) => {
  return (
    <StyledFileInput
      size={size}
      scale={scale ? 1 : 0}
      hasError={hasError}
      hasWarning={hasWarning}
      isDisabled={isDisabled}
      className={className}
      {...rest}
    >
      <TextInput
        id={id}
        className="text-input-with-folder-path"
        placeholder={placeholder}
        value={folderPath || baseFolder}
        size={size}
        isDisabled={isDisabled}
        hasError={hasError}
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
          color={"#A3A9AE"}
          isDisabled={isDisabled}
        />
      </div>
    </StyledFileInput>
  );
};
FileInputWithFolderPath.propTypes = {
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
};

FileInputWithFolderPath.defaultProps = {
  size: "base",
  scale: false,
  hasWarning: false,
  hasError: false,
  isDisabled: false,
  baseFolder: "",
};

export default FileInputWithFolderPath;
