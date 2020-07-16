import React, { Component } from "react";
import PropTypes from "prop-types";
import InputBlock from "../input-block";

class FileInput extends Component { 
  constructor(props) {
    super(props);

    const inputRef = React.createRef();

    this.state = {
      fileName: '',
      path: ''
    }

  }

  onIconFileClick = e => {
    e.target.blur();
    this.inputRef.current.click();
  }

  onChangeFile = e => this.setState({ 
    path: e.target.value 
  });

  onInputFile = () => this.setState({
    fileName: this.inputRef.current.files[0].name
  });

  render() {
    const { fileName } = this.state;
    const { placeholder, size } = this.props;

    return(
      <InputBlock
        value={fileName}
        iconName={"CatalogFolderIcon"}
        placeholder={placeholder}
        onIconClick={this.oIconFileClick}
        onChange={this.onChangeFile}
        oFocus={this.onIconFileClick}
        size={size}
      >
        <input
          type="file"
          onInput={this.onInputFile}
          ref={this.inputRef}
          style={{ display: 'none' }}
        />
      </InputBlock>
    )
  }
}

FileInput.propTypes = {
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  placeholder: PropTypes.string,
  size: PropTypes.string
}

export default FileInput;