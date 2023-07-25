import React, { Component } from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";
import Dropzone from "react-dropzone";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import { withTranslation } from "react-i18next";
import IconButton from "../icon-button";
import Button from "../button";
import TextInput from "../text-input";
import StyledFileInput from "./styled-file-input";
import Loader from "../loader";
import toastr from "../toast/toastr";

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

  onDrop = (acceptedFiles) => {
    const { onInput, t } = this.props;

    if (acceptedFiles.length === 0) {
      toastr.error(t("Common:NotSupportedFormat"));
      return;
    }

    this.setState({
      fileName: acceptedFiles[0].name,
    });

    onInput(acceptedFiles[0]);
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
      buttonLabel,
      idButton,
      isLoading,
      ...rest
    } = this.props;

    let iconSize = 0;
    let buttonSize = "";

    switch (size) {
      case "base":
        iconSize = 15;
        buttonSize = "extrasmall";
        break;
      case "middle":
        iconSize = 15;
        buttonSize = "small";
        break;
      case "big":
        iconSize = 16;
        buttonSize = "normal";
        break;
      case "huge":
        iconSize = 16;
        buttonSize = "medium";
        break;
      case "large":
        iconSize = 16;
        buttonSize = "medium";
        break;
    }

    return (
      <Dropzone
        onDrop={this.onDrop}
        {...(accept && { accept: [accept] })}
        noClick={isDisabled || isLoading}
      >
        {({ getRootProps, getInputProps }) => (
          <StyledFileInput
            size={size}
            scale={scale ? 1 : 0}
            hasError={hasError}
            hasWarning={hasWarning}
            id={idButton}
            isDisabled={isDisabled}
            {...rest}
            {...getRootProps()}
          >
            <TextInput
              isReadOnly
              className="text-input"
              placeholder={placeholder}
              value={fileName}
              size={size}
              isDisabled={isDisabled || isLoading}
              hasError={hasError}
              hasWarning={hasWarning}
              scale={scale}
            />
            <input
              type="file"
              id={id}
              ref={this.inputRef}
              style={{ display: "none" }}
              {...getInputProps()}
            />

            {buttonLabel ? (
              <Button
                isDisabled={isDisabled}
                label={buttonLabel}
                size={buttonSize}
              />
            ) : (
              <div className="icon">
                {isLoading ? (
                  <Loader className="loader" size="20px" type="track" />
                ) : (
                  <IconButton
                    className="icon-button"
                    iconName={CatalogFolderReactSvgUrl}
                    color={"#A3A9AE"}
                    size={iconSize}
                    isDisabled={isDisabled}
                  />
                )}
              </div>
            )}
          </StyledFileInput>
        )}
      </Dropzone>
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
  /** Indicates that the input field has scale */
  scale: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Indicates that the input field has an error */
  hasError: PropTypes.bool,
  /** Indicates that the input field has a warning */
  hasWarning: PropTypes.bool,
  /** Used as HTML `id` property */
  id: PropTypes.string,
  /** Indicates that the field cannot be used (e.g not authorised, or changes not saved) */
  isDisabled: PropTypes.bool,
  /** Tells when the button should show loader icon */
  isLoading: PropTypes.bool,
  /** Used as HTML `name` property */
  name: PropTypes.string,
  /** Called when a file is selected */
  onInput: PropTypes.func,
  /** Specifies the files visible for upload */
  accept: PropTypes.string,
  /** Specifies the label for the upload button */
  buttonLabel: PropTypes.string,
};

FileInput.defaultProps = {
  size: "base",
  scale: false,
  hasWarning: false,
  hasError: false,
  isDisabled: false,
  isLoading: false,
  accept: "",
};

export default withTranslation("Common")(FileInput);
