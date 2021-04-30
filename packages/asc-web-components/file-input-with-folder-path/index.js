import React, { Component } from "react";
import PropTypes from "prop-types";

import IconButton from "../icon-button";
import TextInput from "../text-input";
import StyledFileInput from "../file-input/styled-file-input";

import { inject, observer } from "mobx-react";

let path = "";

class FileInputWithFolderPath extends Component {
  constructor(props) {
    super(props);

    this.state = {
      fullFolderPath: "",
      iconSize: 0,
    };
  }

  componentDidUpdate(prevProps) {
    const { folderPath } = this.props;
    if (folderPath !== prevProps.folderPath) {
      this.getTitlesFolders();
    }
  }

  getTitlesFolders = () => {
    const { folderPath } = this.props;
    path = "";
    if (folderPath.length > 1) {
      for (let item of folderPath) {
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + "/" + " " + `${item.title}`;
      }
      this.setState({
        fullFolderPath: path,
      });
    } else {
      for (let item of folderPath) {
        path = `${item.title}`;
      }
      this.setState({
        fullFolderPath: path,
      });
    }
  };

  onClickInput = () => {
    const { setPanelVisible } = this.props;
    setPanelVisible(true);
  };

  render() {
    const { fullFolderPath } = this.state;
    const {
      folderPath,
      onClick,
      size,
      placeholder,
      isDisabled,
      scale,
      hasError,
      hasWarning,
      accept,
      id,
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
          value={fullFolderPath}
          size={size}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          scale={scale}
          onClick={this.onClickInput}
          isReadOnly
        />

        <div className="icon" onClick={!isDisabled ? this.onClickInput : null}>
          <IconButton
            className="icon-button"
            iconName={"/static/images/catalog.folder.react.svg"}
            color={"#A3A9AE"}
            isDisabled={isDisabled}
          />
        </div>
      </StyledFileInput>
    );
  }
}

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
  /**Specifies files visible for upload */
  accept: PropTypes.string,
};

FileInputWithFolderPath.defaultProps = {
  size: "base",
  scale: false,
  hasWarning: false,
  hasError: false,
  isDisabled: false,
  accept: "",
};

export default inject(({ auth }) => {
  const { setPanelVisible } = auth;
  const { folderPath } = auth.settingsStore;
  return {
    folderPath,
    setPanelVisible,
  };
})(observer(FileInputWithFolderPath));
