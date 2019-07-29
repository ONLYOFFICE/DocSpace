import React from 'react'
import styled from 'styled-components'
import Scrollbar from '../scrollbar/index';
import { Text } from '../text'
import PropTypes from 'prop-types'


const StyledScrollbar = styled(Scrollbar)`
  width: 100% !important;
  height: 91px !important;

  @media only screen and (max-width: 768px) {
    height: 190px !important;
}

  border: 1px solid #D0D5DA;
  box-sizing: border-box;
  border-radius: 3px;

  & > div {
    padding: 5px 8px 2px 8px;
  }

  br {
  display: none;
  }
`;

class TextArea extends React.Component {

  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      value: this.props.children,
      placeholder: this.props.placeholder,
      isFocus: false
    };
    this.setValue = this.setValue.bind(this);
    this.toggleFocus = this.toggleFocus.bind(this);
  }


  setValue = (innerText) => innerText == '\n' ? this.setState({ value: '' }) : this.setState({ value: innerText });
  toggleFocus = (isFocus) => this.setState({ isFocus: isFocus });


  render() {
    // console.log('TextArea render');
    return (
      <StyledScrollbar
        stype='preMediumBlack'
        contentEditable={true}
        suppressContentEditableWarning={true}
        onInput={(e) => {
          this.setValue(e.target.innerText);
          this.props.onChange && this.props.onChange(e);
        }
        }
        onBlur={() => {
          this.toggleFocus(false);
        }
        }
        onFocus={() => {
          this.toggleFocus(true);
        }
        }
      >
        <Text.Body
          tag='span'
        >
          {this.props.children}
        </Text.Body>

        {
          this.state.value === '' && !this.state.isFocus &&
          <Text.Body
            tag='span'
            color='lightGray'
          >
            {this.props.placeholder}
          </Text.Body>
        }

      </StyledScrollbar>
    )
  }
}

TextArea.propTypes = {
  onChange: PropTypes.func,
  placeholder: PropTypes.string
}

TextArea.defaultProps = {
  placeholder: '',
}

export default TextArea;