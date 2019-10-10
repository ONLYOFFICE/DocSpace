import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import isEqual from "lodash/isEqual";
import TextInput from '../text-input'
import Email from '../../utils/email';

const borderColor = {
  default: '#D0D5DA',
  isValid: '#2DA7DB',
  isNotValid: '#c30'
};

// eslint-disable-next-line no-unused-vars
const SimpleInput = ({ onValidateInput, isValidEmail, ...props }) => <TextInput {...props}></TextInput>;

SimpleInput.propTypes = {
  onValidateInput: PropTypes.func,
  isValidEmail: PropTypes.bool
}

const StyledTextInput = styled(SimpleInput)`

  border-color: ${props => (props.isValidEmail ? borderColor.default : borderColor.isNotValid)};

    :hover {
      border-color: ${props => (props.isValidEmail ? borderColor.default : borderColor.isNotValid)};
    }

    :focus {
      border-color: ${props => (props.isValidEmail ? borderColor.isValid : borderColor.isNotValid)};
    }

`;

class EmailInput extends React.Component {

  constructor() {
    super();
    this.state = {
      isValidEmail: true,
      lastValidEmail: ''
    }
  }

  validationFirstEmail = value => {
    const emailUtility = new Email(value);
    const email = emailUtility.ParseAddress();
    return {
      isValidEmail: email[2],
      email: email[1]
    };
  }

  checkEmail = (value) => {

    const emailObj = this.validationFirstEmail(value);
    const { email, isValidEmail } = emailObj;

    email !== this.state.lastValidEmail && isValidEmail && this.setState({ lastValidEmail: email });

    this.props.onValidateInput
      && (isValidEmail !== this.state.isValidEmail || (email !== this.state.lastValidEmail && isValidEmail) || value.length === 0)
      && this.props.onValidateInput(emailObj);

    value.length === 0 ? this.setState({ isValidEmail: true }) : this.setState({ isValidEmail });

  }

  onChangeAction = (e) => {
    this.props.onChange && this.props.onChange(e);
    this.checkEmail(e.target.value);
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  render() {
    //console.log('EmailInput render()');
    const {
      isDisabled,
      scale,
      size,
      hasWarning,
      placeholder,
      tabIndex,
      maxLength,
      id,
      autoComplete,
      className,
      isAutoFocussed,
      isReadOnly,
      onFocus,
      onBlur,
      value,
      name,
      onValidateInput,
      withBorder
    } = this.props;

    const { isValidEmail } = this.state;

    return (
      <StyledTextInput
        isValidEmail={isValidEmail}
        id={id}
        name={name}
        value={value}
        autoComplete={autoComplete}
        onChange={this.onChangeAction}
        onFocus={onFocus}
        onBlur={onBlur}
        isAutoFocussed={isAutoFocussed}
        isDisabled={isDisabled}
        isReadOnly={isReadOnly}
        hasError={!isValidEmail}
        hasWarning={hasWarning}
        placeholder={placeholder}
        type='text'
        size={size}
        scale={scale}
        tabIndex={tabIndex}
        maxLength={maxLength}
        className={className}
        onValidateInput={onValidateInput}
        withBorder={withBorder}
      />
    );
  }
}

EmailInput.propTypes = {
  value: PropTypes.string,
  onChange: PropTypes.func,
  onValidateInput: PropTypes.func,
  inputWidth: PropTypes.string,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  maxLength: PropTypes.number,
  className: PropTypes.string,

  isDisabled: PropTypes.bool,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  scale: PropTypes.bool
}

EmailInput.defaultProps = {
  inputType: 'text',
  name: '',
  autoComplete: 'email',
  maxLength: 255,

  isDisabled: false,
  size: 'base',
  scale: false,
  withBorder: true,

  className: ''
}

export default EmailInput;
