import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import isEqual from "lodash/isEqual";
import TextInput from '../text-input'
import { EmailSettings, parseAddress, checkAndConvertEmailSettings, isEqualEmailSettings } from '../../utils/email/';

const borderColor = {
  default: '#D0D5DA',
  isValid: '#2DA7DB',
  isNotValid: '#c30'
};

// eslint-disable-next-line no-unused-vars
const SimpleInput = ({ onValidateInput, isValidEmail, emailSettings, ...props }) => <TextInput {...props}></TextInput>;

SimpleInput.propTypes = {
  onValidateInput: PropTypes.func,
  isValidEmail: PropTypes.bool,
  emailSettings: PropTypes.oneOfType([PropTypes.instanceOf(EmailSettings), PropTypes.objectOf(PropTypes.bool)])
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
  constructor(props) {
    super(props);

    const { value, emailSettings } = this.props;
    const validatedSettings = checkAndConvertEmailSettings(emailSettings);

    this.state = {
      isValidEmail: true,
      emailSettings: validatedSettings,
      inputValue: value
    }
  }

  componentDidUpdate(prevProps) {
    const { emailSettings } = this.props;

    const validatedSettings = checkAndConvertEmailSettings(emailSettings);

    this.setState({ emailSettings: validatedSettings }, function () {
      this.checkEmail(this.state.inputValue);
    });

    if (prevProps.value !== this.props.value) {
      this.setState({ inputValue: this.props.value });
    }

  }

  checkEmail = (value) => {
    const emailObj = parseAddress(value, this.state.emailSettings);
    const isValidEmail = emailObj.isValid();
    return isValidEmail;
  }

  onChange = (e) => {
    if (this.props.onChange) {
      this.props.onChange(e)
    }
    else {
      this.setState({ inputValue: e.target.value });
    }
    const isValidEmail = this.props.customValidateFunc ? this.props.customValidateFunc(e) : this.checkEmail(e.target.value);
    if (!e.target.value.length) {
      this.setState({ isValidEmail: true });
    }
    else {
      this.setState({ isValidEmail });
    }

    this.props.onValidateInput && this.props.onValidateInput(isValidEmail);
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  render() {
    //console.log('EmailInput render()');
    // eslint-disable-next-line no-unused-vars
    const { onValidateInput, emailSettings, onChange, isValid, value, ...rest } = this.props;

    const { isValidEmail, inputValue } = this.state;

    return (
      <StyledTextInput
        isValidEmail={isValid || isValidEmail}
        value={inputValue}
        onChange={this.onChange}
        type='text'
        onValidateInput={onValidateInput}
        {...rest}
      />
    );
  }
}

EmailInput.propTypes = {
  onValidateInput: PropTypes.func,
  onChange: PropTypes.func,
  customValidateFunc: PropTypes.func,
  value: PropTypes.string,
  isValid: PropTypes.bool,
  emailSettings: PropTypes.oneOfType([PropTypes.instanceOf(EmailSettings), PropTypes.objectOf(PropTypes.bool)]),
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
}

EmailInput.defaultProps = {
  id: '',
  name: '',
  autoComplete: 'email',
  maxLength: 255,
  value: '',
  isDisabled: false,
  isReadOnly: false,
  size: 'base',
  scale: false,
  withBorder: true,
  placeholder: '',
  className: '',
  title: '',
  isValid: undefined,

  emailSettings: new EmailSettings()
}

export default EmailInput;
