import React, { Component } from "react";
import PropTypes from "prop-types";
import styled, { css } from 'styled-components';

import IconButton from '../icon-button';
import TextInput from '../text-input';

const btnIconSize = css`

`;


const StyledFileInput = styled.div`
  display: flex;

  .icon {
    display: flex;
    align-items: center;
    justify-content: center;

    position: absolute;
    right: 16px;

    width: ${props => props.size === 'large' ? '48px'
      : props.size === 'huge' ? '38px'
        : props.size === 'big' ? '37px'
          : props.size === 'middle' ? '36px'
            : '30px'
    };

    height: ${props => props.size === 'large' ? '43px'
      : props.size === 'huge' ? '37px'
        : props.size === 'big' ? '36px'
          : props.size === 'middle' ? '36px'
            : '30px'
    };

    margin: 0;
    border: 1px solid #D0D5DA;
    border-radius: 0 3px 3px 0;

    :hover {
      cursor: pointer;
    }
  }
`;

class FileInput extends Component { 
  constructor(props) {
    super(props);

    this.inputRef = React.createRef();

    this.state = {
      fileName: '',
      path: ''
    }

  }

  onIconFileClick = e => {
    console.log('click')
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
    const { size, ...rest } = this.props;

    let iconSize = 0;

    switch (size) {
      case 'base':
        iconSize = 15;
        break;
      case 'middle':
        iconSize = 15;
        break;
      case 'big':
        iconSize = 16;
        break;
      case 'huge':
        iconSize = 16;
        break;
      case 'large': 
        iconSize = 16;
        break;
    }
    
    return( 
      <StyledFileInput size={size}>
        <TextInput
          value={fileName}
          onChange={this.onChangeFile}
          onFocus={this.onIconFileClick}
          size={size}
          {...rest}
        />
          <input
            type="file"
            onInput={this.onInputFile}
            ref={this.inputRef}
            style={{ display: 'none' }}
          />
        <div className="icon">
          <IconButton 
            iconName={"CatalogFolderIcon"}
            size={iconSize}
            onClick={this.onIconFileClick}
          />
        </div>
      </StyledFileInput>
    )
  }
}

FileInput.propTypes = {
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  placeholder: PropTypes.string,
  size: PropTypes.string
}

export default FileInput;