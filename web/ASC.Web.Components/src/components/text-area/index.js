import React from 'react';
import styled from 'styled-components';
import Scrollbar from '../scrollbar/index';
import PropTypes from 'prop-types';
import commonInputStyle from '../text-input/common-input-styles';
import TextareaAutosize from 'react-autosize-textarea';

const StyledScrollbar = styled(Scrollbar)`
  ${commonInputStyle};
    :focus-within {
      border-color: #2DA7DB;
    }
    :focus{
    outline: none;
    }
  width: 100% !important;
  height: 91px !important;
  background-color: ${props => props.isDisabled && '#F8F9F9'};

  @media only screen and (max-width: 768px) {
    height: 190px !important;
}

`;

const StyledTextarea = styled(TextareaAutosize)`
  ${commonInputStyle};
  width: 100%;
  height: 96%;
  border: none;
  outline: none;
  resize: none;
  overflow: hidden;
  padding: 5px 8px 2px 8px;

    :focus-within {
      border-color: #2DA7DB;
    }

    :focus {
    outline: none;
    }

`;

class TextArea extends React.PureComponent {

  onChange = (e) => this.props.onChange && this.props.onChange(e);

  render() {
    // console.log('TextArea render');
    return (
      <StyledScrollbar
        stype='preMediumBlack'
        isDisabled={this.props.isDisabled}
      >
        <StyledTextarea
          id={this.props.id}
          placeholder={this.props.placeholder}
          onChange={this.onChange}
          maxLength={this.props.maxLength}
          name={this.props.name}
          tabIndex={this.props.tabIndex}
          isDisabled={this.props.isDisabled}
          disabled={this.props.isDisabled}
          readOnly={this.props.isReadOnly}
        >
          {this.props.value}
        </StyledTextarea>
      </StyledScrollbar>
    )
  }
}

TextArea.propTypes = {
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  maxLength: PropTypes.number,
  name: PropTypes.string,
  onChange: PropTypes.func,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  value: PropTypes.string
}

TextArea.defaultProps = {
  isDisabled: false,
  isReadOnly: false,
  placeholder: '',
  value: '',
  tabIndex: -1,
}

export default TextArea;
