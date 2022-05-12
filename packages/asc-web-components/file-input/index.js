import React, { Component } from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import IconButton from "../icon-button";
import TextInput from "../text-input";
import StyledFileInput from "./styled-file-input";

class FileInput extends Component {
  constructor(props) {
    super(props);

    this.inputRef = React.createRef();

    this.state = {
      fileName: "",
      file: null,
    };
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !equal(this.props, nextProps) || !equal(this.state, nextState);
  }

  onIconFileClick = (e) => {
    const { isDisabled } = this.props;

    if (isDisabled) {
      return false;
    }
    e.target.blur();
    this.inputRef.current.click();
  };

  onChangeHandler = (e) => {
    this.setState({
      fileName: e.target.value,
    });
  };

  onInputFile = () => {
    const { onInput } = this.props;

    if (this.inputRef.current.files.length > 0) {
      this.setState(
        {
          fileName: this.inputRef.current.files[0].name,
          file: this.inputRef.current.files[0],
        },
        () => {
          if (onInput) {
            this.inputRef.current.value = "";
            onInput(this.state.file);
          }
        }
      );
    }
  };

  render() {
    //console.log('render FileInput');
    const { fileName } = this.state;
    const {
      size,
      placeholder,
      isDisabled,
      scale,
      hasError,
      hasWarning,
      accept,
      id,
      onInput, // eslint-disable-line no-unused-vars
      ...rest
    } = this.props;

    return (
      <StyledFileInput
        size={size}
        scale={scale ? 1 : 0}
        hasError={hasError}
        hasWarning={hasWarning}
        isDisabled={isDisabled}
        {...rest}
      >
        <TextInput
          className="text-input"
          placeholder={placeholder}
          value={fileName}
          size={size}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          scale={scale}
          onFocus={this.onIconFileClick}
          onChange={this.onChangeHandler}
        />
        <input
          type="file"
          id={id}
          ref={this.inputRef}
          style={{ display: "none" }}
          accept={accept}
          onInput={this.onInputFile}
        />
        <div className="icon" onClick={this.onIconFileClick}>
          <IconButton
            className="icon-button"
            iconName={"/static/images/catalog.folder.react.svg"}
            // color={"#A3A9AE"}
            isDisabled={isDisabled}
          />
        </div>
      </StyledFileInput>
    );
  }
}

FileInput.propTypes = {
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
  /** Called when a file is selected */
  onInput: PropTypes.func,
  /**Specifies files visible for upload */
  accept: PropTypes.string,
};

FileInput.defaultProps = {
  size: "base",
  scale: false,
  hasWarning: false,
  hasError: false,
  isDisabled: false,
  accept: "",
};

export default FileInput;
