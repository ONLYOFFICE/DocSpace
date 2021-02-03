import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import equal from "fast-deep-equal/react";

import IconButton from "../icon-button";
import TextInput from "../text-input";

const StyledFileInput = styled.div`
  display: flex;
  position: relative;
  outline: none;
  width: ${(props) =>
    (props.scale && "100%") ||
    (props.size === "base" && "173px") ||
    (props.size === "middle" && "300px") ||
    (props.size === "big" && "350px") ||
    (props.size === "huge" && "500px") ||
    (props.size === "large" && "550px")};

  .text-input {
    border-color: ${(props) =>
      (props.hasError && "#c30") ||
      (props.hasWarning && "#f1ca92") ||
      (props.isDisabled && "#ECEEF1") ||
      "#D0D5DA"};
    text-overflow: ellipsis;
    padding-right: 40px;
    padding-right: ${(props) =>
      props.size === "large"
        ? "64px"
        : props.size === "huge"
        ? "58px"
        : props.size === "big"
        ? "53px"
        : props.size === "middle"
        ? "48px"
        : "37px"};
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }

  :hover {
    .icon {
      border-color: ${(props) =>
        (props.hasError && "#c30") ||
        (props.hasWarning && "#f1ca92") ||
        (props.isDisabled && "#ECEEF1") ||
        "#A3A9AE"};
    }
  }

  :active {
    .icon {
      border-color: ${(props) =>
        (props.hasError && "#c30") ||
        (props.hasWarning && "#f1ca92") ||
        (props.isDisabled && "#ECEEF1") ||
        "#2DA7DB"};
    }
  }

  .icon {
    display: flex;
    align-items: center;
    justify-content: center;

    position: absolute;
    right: 0;

    width: ${(props) =>
      props.size === "large"
        ? "48px"
        : props.size === "huge"
        ? "38px"
        : props.size === "big"
        ? "37px"
        : props.size === "middle"
        ? "36px"
        : "30px"};

    height: ${(props) =>
      props.size === "large"
        ? "43px"
        : props.size === "huge"
        ? "37px"
        : props.size === "big"
        ? "36px"
        : props.size === "middle"
        ? "36px"
        : "30px"};

    margin: 0;
    border: 1px solid;
    border-radius: 0 3px 3px 0;

    border-color: ${(props) =>
      (props.hasError && "#c30") ||
      (props.hasWarning && "#f1ca92") ||
      (props.isDisabled && "#ECEEF1") ||
      "#D0D5DA"};
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }

  .icon-button {
    cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
  }
`;

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
            iconName={"CatalogFolderIcon"}
            color={"#A3A9AE"}
            size={iconSize}
            isDisabled={isDisabled}
          />
        </div>
      </StyledFileInput>
    );
  }
}

FileInput.propTypes = {
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  placeholder: PropTypes.string,
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "large"]),
  scale: PropTypes.bool,
  className: PropTypes.string,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool,
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  name: PropTypes.string,
  onInput: PropTypes.func,
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
