import React, { Component } from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import IconButton from "../icon-button";
import TextInput from "../text-input";
import StyledFileInput from "../file-input/styled-file-input";
import { inject, observer } from "mobx-react";
let path = "";
class FileInputPath extends Component {
  constructor(props) {
    super(props);

    this.inputRef = React.createRef();

    this.state = {
      fileName: "fileName",
      file: null,
      fullFolderPath: "",
    };
  }

  componentDidUpdate(prevProps) {
    const { folderPath } = this.props;
    if (folderPath !== prevProps.folderPath) {
      console.log("YES");
      this.getTitlesFolders();
    }
  }

  getTitlesFolders = () => {
    const { folderPath } = this.props;
    path = "";
    if (folderPath.length > 1) {
      for (let item of folderPath) {
        console.log("item", item.title);

        //debugger;
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + ">" + " " + `${item.title}`;
        console.log("path", path);
        console.log("this.state.fullFolderPath", this.state.fullFolderPath);
      }
      this.setState({
        fullFolderPath: path,
      });
    } else {
      for (let item of folderPath) {
        console.log("item", item.title);

        //debugger;
        path = `${item.title}`;
        console.log("path", path);
        console.log("this.state.fullFolderPath", this.state.fullFolderPath);
      }
      this.setState({
        fullFolderPath: path,
      });
    }
  };
  onChangeHandler = (e) => {
    const { folderPath } = this.props;
    //debugger;
    console.log("onChangeHandler");
    this.setState({
      fileName: e.target.value,
    });
  };

  onClickInput = () => {
    const { setPanelVisible } = this.props;
    setPanelVisible(true);
    const { folderPath } = this.props;

    //console.log("onChangeHandler", folderPath);
  };

  render() {
    const { fileName, fullFolderPath } = this.state;
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
      onInput, // eslint-disable-line no-unused-vars
      ...rest
    } = this.props;
    console.log("render FileInputPath", fullFolderPath);
    let iconSize = 0;

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
      <StyledFileInput>
        <TextInput
          className="text-input"
          placeholder={placeholder}
          value={fullFolderPath}
          size={size}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          scale={scale}
          onFocus={this.onClickInput}
          onChange={this.onChangeHandler}
        />

        <div className="icon" onClick={this.onClickInput}>
          <IconButton
            className="icon-button"
            iconName={"/static/images/catalog.folder.react.svg"}
            color={"#A3A9AE"}
            size={iconSize}
            isDisabled={isDisabled}
          />
        </div>
      </StyledFileInput>
    );
  }
}

FileInputPath.propTypes = {
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

FileInputPath.defaultProps = {
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
})(observer(FileInputPath));
