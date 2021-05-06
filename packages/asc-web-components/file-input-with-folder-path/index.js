import React, { Component } from "react";
import PropTypes from "prop-types";

import IconButton from "../icon-button";
import TextInput from "../text-input";
import StyledFileInput from "../file-input/styled-file-input";

let path = "";

class FileInputWithFolderPath extends Component {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      fullFolderPath: "",
      thirdParty: "",
      selectedInput: "",
    };
  }

  // componentDidUpdate(prevProps) {
  //   const { folderPath, selectedInput } = this.props;
  //   if (folderPath !== prevProps.folderPath) {
  //     //this.getTitlesFolders();
  //   }
  // }

  // getTitlesFolders = () => {
  //   const { folderPath, selectedInput } = this.props;
  //   //debugger;
  //   console.log("selectedInput", selectedInput);

  //   path = "";
  //   if (folderPath.length > 1) {
  //     for (let item of folderPath) {
  //       if (!path) {
  //         path = path + `${item.title}`;
  //       } else path = path + " " + "/" + " " + `${item.title}`;
  //     }
  //     this.setState({
  //       //fullFolderPath: path,
  //       [selectedInput]: path,
  //     });
  //   } else {
  //     for (let item of folderPath) {
  //       path = `${item.title}`;
  //     }
  //     this.setState({
  //       //fullFolderPath: path,
  //     });
  //   }
  // };

  handleOptionChange(e) {
    console.log("e.target.value", e.target.name);
    //console.log("this.inputRef.current", this.inputRef.current);
    const { curRef } = this.props;
    this.setState({
      //fullFolderPath: path,
      //[e.target.name]: "HELLO",
      selectedInput: this.inputRef.current,
    });
  }

  onChange = () => {
    console.log("on change!!!!");
  };
  render() {
    const { fullFolderPath, selectedInput } = this.state;
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
      baseFolder,
      onClickInput,
      name,
      onSelect,
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
          ref={this.inputRef}
          id={id}
          className="text-input"
          placeholder={placeholder}
          value={folderPath || baseFolder}
          size={size}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          scale={scale}
          //onClick={onClickInput}
          onClick={(e) => {
            this.handleOptionChange(e);
            onClickInput && onClickInput(e);
          }}
          isReadOnly
          name={name}
          onChange={this.onChange}
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
  baseFolder: "",
  accept: "",
};

export default FileInputWithFolderPath;
